// ElementCommon.hlsl: the shared toolbox of the 6 ball shaders.
// Keep this file in the SAME FOLDER as the Ball_*.shader files (they include it by name).
//
// WHAT IS IN HERE
//   1) the vertex shader (the same for all 6 balls)
//   2) noise tools: hash (random numbers), noise3, fbm3 (cloudy noise), Voronoi3 (cell patterns)
//   3) the "see-through sphere" trick: one pixel shows the FRONT of the ball AND its BACK (seen through the glass)
//   4) ComposeGlass: stacks front pattern, glass body, inner object and back pattern into one color
//
// THE BIG IDEA (patterns that roll with the ball)
//   Every pattern is computed from "u": a point ON the ball, in the ball's own space (a direction from its center).
//   BallVisuals rotates the ball mesh when it rolls, so u rotates with it, and the pattern rolls too.

#ifndef ELEMENT_COMMON_INCLUDED
#define ELEMENT_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// ------------------------------------------------------------------
// 1) VERTEX SHADER
// ------------------------------------------------------------------
struct Attributes
{
    float4 positionOS : POSITION;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 posOS      : TEXCOORD0;   // the point on the ball, in the ball's own space
    float3 viewDirOS  : TEXCOORD1;   // from the camera to that point, in the ball's own space
};

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.posOS = IN.positionOS.xyz;

    float3 cameraOS = TransformWorldToObject(GetCameraPositionWS());
    OUT.viewDirOS = IN.positionOS.xyz - cameraOS;
    return OUT;
}

// ------------------------------------------------------------------
// 2) NOISE TOOLS
// ------------------------------------------------------------------
// hash = a "random" number made from a position (the same position always gives the same number)
float hash13(float3 p3)
{
    p3 = frac(p3 * 0.1031);
    p3 += dot(p3, p3.zyx + 31.32);
    return frac((p3.x + p3.y) * p3.z);
}

float3 hash33(float3 p3)
{
    p3 = frac(p3 * float3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yxz + 33.33);
    return frac((p3.xxy + p3.yxx) * p3.zyx);
}

// noise3 = smooth random blobs (0 to 1)
float noise3(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    f = f * f * (3.0 - 2.0 * f);   // smooth the steps between the random values

    float n000 = hash13(i);
    float n100 = hash13(i + float3(1, 0, 0));
    float n010 = hash13(i + float3(0, 1, 0));
    float n110 = hash13(i + float3(1, 1, 0));
    float n001 = hash13(i + float3(0, 0, 1));
    float n101 = hash13(i + float3(1, 0, 1));
    float n011 = hash13(i + float3(0, 1, 1));
    float n111 = hash13(i + float3(1, 1, 1));

    return lerp(lerp(lerp(n000, n100, f.x), lerp(n010, n110, f.x), f.y),
                lerp(lerp(n001, n101, f.x), lerp(n011, n111, f.x), f.y), f.z);
}

// fbm3 = noise3 added 4 times, each time smaller and finer: clouds, rock, frost (0 to 1)
float fbm3(float3 p)
{
    float value = 0.0;
    float amplitude = 0.5;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        value += amplitude * noise3(p);
        p = p * 2.03 + 17.1;
        amplitude *= 0.5;
    }

    return value / 0.9375;
}

// Voronoi3 = cut space into random cells. Gives:
//   f1 = distance to the nearest random point      f2 = distance to the second nearest
//   (f2 - f1) is close to 0 on the BORDER between two cells: that is how we draw cracks and facets
//   id = which cell it is (use hash13(id) to give each cell its own random value)
//   nearestPoint = the position of the random point of the nearest cell (a star!)
void Voronoi3(float3 p, out float f1, out float f2, out float3 id, out float3 nearestPoint)
{
    float3 cell = floor(p);
    float3 local = frac(p);

    float best = 8.0;
    float second = 8.0;
    id = float3(0, 0, 0);
    nearestPoint = float3(0, 0, 0);

    [unroll]
    for (int z = -1; z <= 1; z++)
    {
        [unroll]
        for (int y = -1; y <= 1; y++)
        {
            [unroll]
            for (int x = -1; x <= 1; x++)
            {
                float3 offset = float3(x, y, z);
                float3 randomPoint = hash33(cell + offset);
                float3 toPoint = offset + randomPoint - local;
                float d = dot(toPoint, toPoint);

                if (d < best)
                {
                    second = best;
                    best = d;
                    id = cell + offset;
                    nearestPoint = cell + offset + randomPoint;
                }
                else if (d < second)
                {
                    second = d;
                }
            }
        }
    }

    f1 = sqrt(best);
    f2 = sqrt(second);
}

// ------------------------------------------------------------------
// 3) THE SEE-THROUGH SPHERE TRICK
// ------------------------------------------------------------------
// We only draw the front of the ball (Cull Back). But a ray of light entering at the front point
// leaves the sphere at a BACK point. We calculate that back point with simple math,
// so the same pixel can also show the back of the ball seen through the glass.
// No extra pass, no sorting problems.
struct SphereView
{
    float3 front;    // the point on the front of the ball (length 1)
    float3 back;     // the point on the back of the ball, on the same line of sight (length 1)
    float3 dir;      // the direction we look in (camera to ball)
    float facing;    // 1 = looking straight at this point, 0 = on the silhouette edge
    float impact;    // 0 = the line of sight goes through the center, 1 = it only touches the edge
};

SphereView GetSphereView(Varyings IN)
{
    SphereView v;
    v.front = normalize(IN.posOS);
    v.dir = normalize(IN.viewDirOS);
    v.back = normalize(v.front - v.dir * (2.0 * dot(v.front, v.dir)));
    v.facing = saturate(dot(v.front, -v.dir));
    v.impact = sqrt(saturate(1.0 - v.facing * v.facing));
    return v;
}

// ------------------------------------------------------------------
// 4) STACKING LAYERS
// ------------------------------------------------------------------
// Colors are "premultiplied": rgb is the light we ADD, a is how much of what is behind we HIDE.
// (the shaders use Blend One OneMinusSrcAlpha)
// Over(top, bottom) puts the top layer in front of the bottom layer.
float4 Over(float4 top, float4 bottom)
{
    return top + bottom * (1.0 - top.a);
}

// Order from the camera: front pattern, glass body, inner object (mid), back pattern.
// Then the rim glow (bright edge of the glass) is added on top.
float4 ComposeGlass(float4 front, float4 mid, float4 back, SphereView v,
                    float3 tint, float bodyAlpha, float backDim,
                    float3 rimColor, float rimPower, float rimStrength)
{
    float4 body = float4(tint * bodyAlpha, bodyAlpha);
    float4 backDimmed = float4(back.rgb * backDim, back.a);

    float4 result = Over(front, Over(body, Over(mid, backDimmed)));

    float rim = pow(1.0 - v.facing, rimPower);
    result.rgb += rimColor * rim * rimStrength;

    result.a = saturate(result.a);
    return result;
}

#endif
