// CrackOverlay: cracks and scratches drawn on top of a breakable block.
//   CRACKS  4 pictures: 0 = clean, 1 = a hairline fissure, 2 = it grows and branches, 3 = nearly broken.
//           The DestructibleObject script sets "_Stage" for you (CrackRenderer field). You never touch it by hand.
//   WEAR    faint scratches and grazes that are always there (even at stage 0), to hint that the block can be hit.
//           Set "Scratches" above 0 to show them. Use one material with Scratches 0 (only cracks) and another
//           (M_Crack_Worn) with Scratches 0.45 for every breakable block.
//
// USE IT ON: a Quad child of the block (name it "Cracks"), no collider, placed in FRONT of the block.
// The cracks are ONE network over the whole face (not repeated per square): thin jagged lines like dry earth or cooled lava.
// Every block gets its own pattern (seeded by its position). Stage 1 = a few cracks, 2 = more, 3 = a dense network.
Shader "Elemento/CrackOverlay"
{
    Properties
    {
        [HDR] _Color ("Crack Color (HDR)", Color) = (2.2, 2.2, 2.2, 1)
        [HDR] _GlowColor ("Glow Color (HDR)", Color) = (0.4, 0.9, 1.2, 1)
        _LineWidth ("Crack Width (world units)", Range(0.005, 0.15)) = 0.035
        _GlowWidth ("Glow Width (world units)", Range(0.01, 0.4)) = 0.09
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.6
        _CellSize ("Crack Cell Size (world units, bigger = fewer cracks)", Range(0.5, 3)) = 1.4
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
                float _CellSize;
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
                float seed : TEXCOORD2;
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
                output.seed = frac(unity_ObjectToWorld._m03 * 0.137 + unity_ObjectToWorld._m13 * 0.291) * 40.0;
                return output;
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

            float2 Hash22v(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            // Distance to the nearest border between two cells of a jittered grid (a "cracked earth" pattern).
            // Returns it in cell units, and a number 0..1 that is the same for both cells sharing that border,
            // so the border can be shown or hidden as a whole (that is how the stages work).
            float CrackDistance(float2 x, out float edgeId)
            {
                float2 n = floor(x);
                float2 f = frac(x);

                float2 mg = float2(0, 0);
                float2 mr = float2(0, 0);
                float md = 8.0;

                for (int j = -1; j <= 1; j++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        float2 g = float2(i, j);
                        float2 r = g + Hash22v(n + g) - f;
                        float d = dot(r, r);
                        if (d < md)
                        {
                            md = d;
                            mr = r;
                            mg = g;
                        }
                    }
                }

                md = 8.0;
                edgeId = 0.0;

                for (int j2 = -2; j2 <= 2; j2++)
                {
                    for (int i2 = -2; i2 <= 2; i2++)
                    {
                        float2 g = mg + float2(i2, j2);
                        float2 r = g + Hash22v(n + g) - f;
                        float2 diff = r - mr;

                        if (dot(diff, diff) > 0.00001)
                        {
                            float d = dot(0.5 * (mr + r), normalize(diff));
                            if (d < md)
                            {
                                md = d;
                                edgeId = Hash21((n + mg) + (n + g));
                            }
                        }
                    }
                }

                return md;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // position in world units
                float2 world = input.uv * input.size;

                // WEAR: scratches that are always visible
                float3 color = _WearColor.rgb * Scratches(world) * 0.6;

                if (_Stage < 0.5)
                    return float4(color, 1);

                // the crack network, in cell units. A small wobble makes the lines jagged instead of straight.
                float2 x = world / _CellSize + input.seed;
                x += 0.10 * float2(sin(x.y * 6.1 + 1.3), sin(x.x * 5.3));

                float edgeId;
                float dCell = CrackDistance(x, edgeId);
                float nearest = dCell * _CellSize;                 // world units

                // stage 1 shows about a third of the borders, stage 2 about two thirds, stage 3 nearly all
                float shown = saturate(0.1 + 0.3 * _Stage);   // 0.4, 0.7, 1.0
                float visible = step(edgeId, shown);

                float aa = fwidth(nearest) + 0.0001;
                // the lines also get thicker and brighter with every stage, so a hit is always visible, even on a small block
                float lineW = _LineWidth * (0.7 + 0.15 * _Stage);
                float core = (1.0 - smoothstep(lineW - aa, lineW + aa, nearest)) * visible;
                float glow = exp(-nearest / max(_GlowWidth, 0.001)) * _GlowStrength * (0.6 + 0.2 * _Stage) * visible;

                color += _Color.rgb * core + _GlowColor.rgb * glow;
                return float4(color, 1);
            }
            ENDHLSL
        }
    }
}
