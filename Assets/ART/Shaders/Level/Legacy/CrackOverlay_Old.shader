// CrackOverlay: cracks and scratches drawn on top of a breakable block.
//   CRACKS  4 pictures: 0 = clean, 1 = a hairline fissure, 2 = it grows and branches, 3 = nearly broken.
//           The DestructibleObject script sets "_Stage" for you (CrackRenderer field). You never touch it by hand.
//   WEAR    faint scratches and grazes that are always there (even at stage 0), to hint that the block can be hit.
//           Set "Scratches" above 0 to show them. Use one material with Scratches 0 (only cracks) and another
//           (M_Crack_Worn) with Scratches 0.45 for every breakable block.
//
// USE IT ON: a Quad child of the block (name it "Cracks"), no collider, placed in FRONT of the block.
// The fissure is drawn once per 1x1 square of the quad, always symmetric left to right (a trunk with branches).
Shader "Elemento/CrackOverlay"
{
    Properties
    {
        [HDR] _Color ("Crack Color (HDR)", Color) = (2.2, 2.2, 2.2, 1)
        [HDR] _GlowColor ("Glow Color (HDR)", Color) = (0.4, 0.9, 1.2, 1)
        _LineWidth ("Crack Width (world units)", Range(0.005, 0.15)) = 0.035
        _GlowWidth ("Glow Width (world units)", Range(0.01, 0.4)) = 0.09
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.6
        _Stage ("Stage (set by the script, 0 to 3)", Range(0, 3)) = 0

        [Header(Wear)]
        [HDR] _WearColor ("Scratch Color (HDR)", Color) = (0.7, 1.0, 1.3, 1)
        _Wear ("Scratches (0 = none, 1 = many)", Range(0, 1)) = 0
        _WearWidth ("Scratch Width (world units)", Range(0.004, 0.05)) = 0.014
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One One      // pure light, like the neon lines of the block
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _GlowColor;
                float _LineWidth;
                float _GlowWidth;
                float _GlowStrength;
                float _Stage;
                float4 _WearColor;
                float _Wear;
                float _WearWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 size : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;

                // the size of the quad in world units (so the cracks keep their thickness)
                float3 axisX = float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20);
                float3 axisY = float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21);
                output.size = float2(length(axisX), length(axisY));
                return output;
            }

            // The fissure of ONE square (0..1): x0, y0, x1, y1. Only the RIGHT half is listed:
            // the shader mirrors it to the left, so the picture is always symmetric (a straight trunk with branches).
            // SEGMENT_STAGE says from which stage each segment shows.
            // Stage 1 = a hairline in the middle, stage 2 = it grows and branches, stage 3 = it reaches the edges.
            #define SEGMENT_COUNT 16
            static const float4 SEGMENTS[SEGMENT_COUNT] =
            {
                float4(0.50, 0.82, 0.50, 0.42),
                float4(0.50, 0.62, 0.62, 0.74),
                float4(0.50, 0.50, 0.60, 0.58),
                float4(0.50, 0.95, 0.50, 0.82),
                float4(0.50, 0.42, 0.50, 0.20),
                float4(0.62, 0.74, 0.72, 0.80),
                float4(0.50, 0.38, 0.64, 0.30),
                float4(0.64, 0.30, 0.74, 0.34),
                float4(0.60, 0.58, 0.74, 0.56),
                float4(0.50, 1.00, 0.50, 0.95),
                float4(0.50, 0.20, 0.50, 0.00),
                float4(0.72, 0.80, 0.92, 0.86),
                float4(0.74, 0.56, 1.00, 0.52),
                float4(0.74, 0.34, 0.96, 0.24),
                float4(0.50, 0.12, 0.70, 0.04),
                float4(0.64, 0.30, 0.70, 0.14)
            };
            static const float SEGMENT_STAGE[SEGMENT_COUNT] = { 1, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3 };

            float SegmentDistance(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = saturate(dot(pa, ba) / max(dot(ba, ba), 0.0001));
                return length(pa - ba * h);
            }

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // Faint scratches and grazes: short thin lines, mostly slanted the same way, one at most per 0.55 x 0.55 cell.
            // Returns 0..1 (how much of a scratch is here).
            float Scratches(float2 world)
            {
                float cellSize = 0.55;
                float2 g = world / cellSize;
                float2 cell = floor(g);
                float2 f = frac(g) - 0.5;

                float h1 = Hash21(cell + 17.3);
                float h2 = Hash21(cell + 91.7);
                float h3 = Hash21(cell + 5.1);
                float h4 = Hash21(cell + 63.9);

                // does this cell have a scratch at all?
                float present = step(h1, _Wear);

                // a slanted line (about 20 to 65 degrees), shifted a little inside the cell
                float ang = lerp(0.35, 1.15, h2);
                float2 dir = float2(cos(ang), sin(ang));
                float2 centre = (float2(h3, h4) - 0.5) * 0.3;
                float halfLen = lerp(0.12, 0.32, h3);

                float2 pc = f - centre;
                float along = clamp(dot(pc, dir), -halfLen, halfLen);
                float dist = length(pc - dir * along) * cellSize;      // distance to the scratch, in world units

                float aa = fwidth(dist) + 0.0001;
                float line1 = 1.0 - smoothstep(_WearWidth * 0.5 - aa, _WearWidth * 0.5 + aa, dist);

                // it fades toward both ends, like a graze
                float fadeEnds = 1.0 - smoothstep(halfLen * 0.4, halfLen, abs(dot(pc, dir)));
                return line1 * fadeEnds * present;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // position in world units, then split into squares of 1 x 1
                float2 world = input.uv * input.size;

                // WEAR: scratches that are always visible
                float3 color = _WearColor.rgb * Scratches(world) * 0.6;

                if (_Stage < 0.5)
                    return float4(color, 1);

                float2 square = floor(world);
                float2 local = frac(world);

                // every square gets its own up/down flip so a tall block does not repeat the same fissure.
                // Left/right is NOT random: the fissure is mirrored around the middle so it stays symmetric.
                float h = Hash21(square);
                if (h > 0.5) local.y = 1.0 - local.y;
                local.x = 0.5 + abs(local.x - 0.5);

                // distance to the closest visible crack, in world units
                float nearest = 10.0;

                [unroll]
                for (int i = 0; i < SEGMENT_COUNT; i++)
                {
                    if (SEGMENT_STAGE[i] <= _Stage + 0.01)
                        nearest = min(nearest, SegmentDistance(local, SEGMENTS[i].xy, SEGMENTS[i].zw));
                }

                float aa = fwidth(nearest) + 0.0001;
                float core = 1.0 - smoothstep(_LineWidth - aa, _LineWidth + aa, nearest);
                float glow = exp(-nearest / max(_GlowWidth, 0.001)) * _GlowStrength;

                color += _Color.rgb * core + _GlowColor.rgb * glow;
                return float4(color, 1);
            }
            ENDHLSL
        }
    }
}
