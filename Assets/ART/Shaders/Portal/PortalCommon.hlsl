// PortalCommon.hlsl: the small toolbox of the ONE portal shader (Portal_Frame).
// Keep this file in the SAME FOLDER as Portal_Frame.shader (it is included by name).
//
// WHAT IS IN HERE
//   1) the vertex shader: gives every pixel of the Quad a position "p" in SHAPE SPACE
//      (0,0 = the center of the Quad, the Quad covers -1.25 .. +1.25, the portal itself has a size of about 1)
//   2) drawing tools: Stroke (a thin line), Soft (a soft glow)
//   3) shape tools: HexDist (hexagon) and ArchSDF (arch)
//   4) stacking tools: Over (paint a layer) and AddGlow (add light)

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
    float2 uv         : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 p          : TEXCOORD0;   // the position of this pixel in shape space
};

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.p = (IN.uv * 2.0 - 1.0) * QUAD_HALF;
    return OUT;
}

// ------------------------------------------------------------------
// 2) DRAWING TOOLS
// ------------------------------------------------------------------
// Stroke: a thin line. d = a distance (0 on the line), halfWidth = half the thickness.
// Returns 1 on the line and 0 away from it, with a soft one-pixel edge (no jagged lines).
float Stroke(float d, float halfWidth)
{
    float aa = clamp(fwidth(d) * 1.5, 0.0002, max(halfWidth * 3.0, 0.01));
    return saturate((halfWidth - abs(d)) / aa + 0.5);
}

// Soft: a glow that is 1 at d = 0 and fades away on both sides. width = how far the glow reaches.
float Soft(float d, float width)
{
    return exp(-abs(d) / max(width, 0.0001));
}

// ------------------------------------------------------------------
// 3) SHAPE TOOLS
// ------------------------------------------------------------------
// HexDist: "distance" from the center for a hexagon with its corners on the left and right.
// On the border of a hexagon with corner radius R it is R * 0.866 (the apothem).
float HexDist(float2 p)
{
    float2 a = abs(p);
    return max(a.y, a.x * 0.8660254 + a.y * 0.5);
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
// 4) STACKING TOOLS (premultiplied colors, the shader uses Blend One OneMinusSrcAlpha)
// ------------------------------------------------------------------
float4 Over(float4 res, float3 rgb, float a)
{
    a = saturate(a);
    return float4(rgb * a, a) + res * (1.0 - a);
}

float4 AddGlow(float4 res, float3 rgb)
{
    res.rgb += rgb;
    return res;
}

#endif
