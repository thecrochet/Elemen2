// PortalCommon.hlsl: the shared toolbox of the 6 portal / arch shaders.
// Keep this file in the SAME FOLDER as the Portal_*.shader files (they include it by name).
//
// WHAT IS IN HERE
//   1) the vertex shader (the same for all 6 portals)
//   2) noise tools: hash, noise2, fbm2 (cloudy noise), Rot2 (turn a point), PosMod
//   3) drawing tools: Stroke (a thin line), Soft (a soft glow), Sparks (little twinkling dots)
//   4) shape tools: HexDist, HexCell, CellEdge (hexagons) and ArchSDF (an arch)
//   5) stacking tools: Over (paint a layer on top) and AddGlow (add light)
//
// THE BIG IDEA (everything is drawn from one number)
//   For every pixel of the Quad we know its position "p" in SHAPE SPACE: (0, 0) is the center of the Quad,
//   and the portal itself has a size of about 1 (so a ring of radius 0.5 is half of the portal).
//   Then we measure a DISTANCE (for example d = length(p) - radius = how far from a ring)
//   and draw with it: a thin line where d is near 0 (Stroke), a soft glow around it (Soft)...
//
// HOW TO USE
//   Put the material on a Quad (GameObject > 3D Object > Quad) that faces the camera.
//   The Quad covers -1.25 .. +1.25 in shape space: the portal fills the middle, the rest is room for glow and flames.

#ifndef PORTAL_COMMON_INCLUDED
#define PORTAL_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#define PORTAL_PI 3.14159265
#define PORTAL_TAU 6.28318531

// the Quad covers -QUAD_HALF .. +QUAD_HALF in shape space
#define QUAD_HALF 1.25

// ------------------------------------------------------------------
// 1) VERTEX SHADER
// ------------------------------------------------------------------
struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS   : NORMAL;
    float4 tangentOS  : TANGENT;
    float2 uv         : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 p          : TEXCOORD0;   // the position of this pixel in shape space (0,0 = the center)
    float2 viewShift  : TEXCOORD1;   // how far a layer slides for each unit of depth (see the DEPTH HELPERS below)
    float  faceZ      : TEXCOORD2;   // Z of the face normal: -1 = the front face of a Quad or Cube, 0 = a side face
};

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.p = (IN.uv * 2.0 - 1.0) * QUAD_HALF;

    // DEPTH TRICK (parallax): which way is the camera, seen from the Quad's own space?
    // A perspective camera looks at each vertex from a different direction; an orthographic one looks everywhere the same way.
    float3 viewWS = (unity_OrthoParams.w > 0.5)
        ? -GetViewForwardDir()
        : GetCameraPositionWS() - TransformObjectToWorld(IN.positionOS.xyz);
    float3 viewOS = TransformWorldToObjectDir(viewWS, false);

    // The face has its own little coordinate system (T = along U, B = along V, N = out of the face).
    // This is what lets the effect work on ANY face of a Cube, not only on a Quad.
    float3 N = normalize(IN.normalOS);
    float3 T = IN.tangentOS.xyz;
    if (dot(T, T) < 0.0001) T = float3(1.0, 0.0, 0.0);   // a mesh without tangents: assume U runs along X
    T = normalize(T);
    float3 B = cross(N, T) * IN.tangentOS.w;

    // sideways shift per unit of depth (divide by the distance in front of the face, like parallax mapping).
    // Clamped, so a very flat angle can't tear the layers apart.
    float2 viewTS = float2(dot(viewOS, T), dot(viewOS, B));
    OUT.viewShift = clamp(viewTS / max(abs(dot(viewOS, N)), 0.2), -1.5, 1.5);
    OUT.faceZ = N.z;
    return OUT;
}

// ------------------------------------------------------------------
// DEPTH (parallax) HELPERS
// ------------------------------------------------------------------
// A layer that sits BEHIND the portal ring must slide the other way when the camera moves; a layer IN FRONT slides with it.
// Use:   float2 pBack  = PortalBack(p, IN.viewShift, _DepthBack);    // for the inside layers
//        float2 pFront = PortalFront(p, IN.viewShift, _DepthFront);  // for sparks and outer details
// then draw those layers with pBack / pFront instead of p. The ring itself keeps using p (it is the anchor).
float2 PortalBack(float2 p, float2 viewShift, float depth)  { return p - viewShift * depth; }
float2 PortalFront(float2 p, float2 viewShift, float depth) { return p + viewShift * depth; }

// PortalSideFace: what the OTHER faces of a Cube look like (when "Portal On Front Face Only" is on):
// a dark body with a glowing border. p = the position in shape space.
float4 PortalSideFace(float2 p, float3 color)
{
    float edge = QUAD_HALF - max(abs(p.x), abs(p.y));        // 0 on the border of the face, bigger inside
    float border = 1.0 - smoothstep(0.0, 0.05, edge);        // a glowing border
    float3 rgb = color * (0.12 + border * 1.2);
    return float4(rgb, 1.0);
}

// ------------------------------------------------------------------
// 2) NOISE TOOLS
// ------------------------------------------------------------------
// hash = a "random" number made from a position (the same position always gives the same number)
float hash21(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return frac(p.x * p.y);
}

float2 hash22(float2 p)
{
    float3 q = frac(float3(p.x, p.y, p.x) * float3(0.1031, 0.1030, 0.0973));
    q += dot(q, q.yzx + 33.33);
    return frac((q.xx + q.yz) * q.zy);
}

// smooth random blobs (0 to 1)
float noise2(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    f = f * f * (3.0 - 2.0 * f);   // smooth the steps between the random values

    float a = hash21(i);
    float b = hash21(i + float2(1.0, 0.0));
    float c = hash21(i + float2(0.0, 1.0));
    float d = hash21(i + float2(1.0, 1.0));

    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
}

// noise2 added 4 times, each time smaller and finer: smoke, flames, clouds (0 to 1)
float fbm2(float2 p)
{
    float value = 0.0;
    float amplitude = 0.5;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        value += amplitude * noise2(p);

        // turn a little and zoom in for the next layer (0.8776 and 0.4794 = cos and sin of 0.5)
        p = float2(0.8776 * p.x - 0.4794 * p.y, 0.4794 * p.x + 0.8776 * p.y) * 2.02 + 11.7;
        amplitude *= 0.5;
    }

    return value / 0.9375;
}

// turns a point around the center by an angle (in radians)
float2 Rot2(float2 p, float a)
{
    float s, c;
    sincos(a, s, c);
    return float2(c * p.x - s * p.y, s * p.x + c * p.y);
}

// positive modulo (fmod gives negative numbers for negative inputs, we do not want that)
float PosMod(float x, float m)
{
    return x - m * floor(x / m);
}

// ------------------------------------------------------------------
// 3) DRAWING TOOLS
// ------------------------------------------------------------------
// Stroke: a thin line. d = a distance (0 on the line), halfWidth = half the thickness of the line.
// Returns 1 on the line and 0 away from it, with a soft one-pixel edge (no jagged lines).
float Stroke(float d, float halfWidth)
{
    // the size of one pixel in this distance; capped so a jump in d (like a frac wrapping) can't smear a line
    float aa = clamp(fwidth(d) * 1.5, 0.0002, max(halfWidth * 3.0, 0.01));
    return saturate((halfWidth - abs(d)) / aa + 0.5);
}

// Soft: a glow that is 1 at d = 0 and fades away on both sides. width = how far the glow reaches.
float Soft(float d, float width)
{
    return exp(-abs(d) / max(width, 0.0001));
}

// Sparks: tiny twinkling dots. p = position, scale = how many dots (bigger = more, smaller dots), time = the animation clock.
// Each cell of a grid has at most one dot, and only some cells have one.
float Sparks(float2 p, float scale, float time)
{
    float2 g = p * scale;
    float2 id = floor(g);
    float2 f = frac(g) - 0.5;

    float h = hash21(id);
    float2 offset = (hash22(id + 7.3) - 0.5) * 0.6;
    float dotShape = smoothstep(0.0, 0.18, 0.18 - length(f - offset));
    float twinkle = 0.5 + 0.5 * sin(time * 8.0 + h * 40.0);

    return dotShape * step(0.75, h) * twinkle;
}

// ------------------------------------------------------------------
// 4) SHAPE TOOLS
// ------------------------------------------------------------------
// HexDist: "distance" from the center for a hexagon with its corners on the left and right.
// On the border of a hexagon with corner radius R it is R * 0.866 (the apothem), so (HexDist(p) - apothem) is 0 on the edge.
float HexDist(float2 p)
{
    float2 a = abs(p);
    return max(a.y, a.x * 0.8660254 + a.y * 0.5);
}

// HexCell: cuts the plane into a grid of hexagons. gv = position inside the cell (0,0 = its center), id = which cell it is.
void HexCell(float2 p, out float2 gv, out float2 id)
{
    const float2 s = float2(1.0, 1.7320508);

    float4 hC = floor(float4(p, p - float2(1.0, 0.5)) / s.xyxy) + 0.5;
    float4 h = float4(p - hC.xy * s, p - (hC.zw + 0.5) * s);

    bool first = dot(h.xy, h.xy) < dot(h.zw, h.zw);
    gv = first ? h.xy : h.zw;
    id = first ? hC.xy : hC.zw + 0.5;
}

// CellEdge: how far from the center of a hexagon cell (0.5 = on its border)
float CellEdge(float2 gv)
{
    gv = abs(gv);
    return max(dot(gv, normalize(float2(1.0, 1.7320508))), gv.x);
}

// ArchSDF: an arch standing on the floor: straight sides, a round top. Negative inside, 0 on the edge, positive outside.
// halfWidth must be smaller than halfHeight. The floor is at y = -halfHeight.
float ArchSDF(float2 p, float halfWidth, float halfHeight)
{
    float cy = halfHeight - halfWidth;
    float d = (p.y > cy) ? length(p - float2(0.0, cy)) - halfWidth : abs(p.x) - halfWidth;
    return max(d, -p.y - halfHeight);
}

// ------------------------------------------------------------------
// 5) STACKING TOOLS
// ------------------------------------------------------------------
// Colors are "premultiplied": rgb is the light we ADD, a is how much of what is behind we HIDE.
// (the shaders use Blend One OneMinusSrcAlpha)

// Over: paints a layer of color "rgb" on top of "res". a = how solid the layer is (1 = hides what is behind).
float4 Over(float4 res, float3 rgb, float a)
{
    a = saturate(a);
    return float4(rgb * a, a) + res * (1.0 - a);
}

// AddGlow: adds light on top of "res" without hiding anything behind it (like additive blending).
float4 AddGlow(float4 res, float3 rgb)
{
    res.rgb += rgb;
    return res;
}

#endif
