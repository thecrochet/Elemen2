// CrackOverlay: cracks drawn on top of a breakable block. 4 pictures: 0 = clean, 1 = a first crack, 2 = more, 3 = nearly broken.
// The DestructibleObject script sets "_Stage" for you (CrackRenderer field). You never touch it by hand.
//
// USE IT ON: a Quad child of the block (name it "Cracks"), no collider, placed in FRONT of the block.
// The cracks are drawn once per 1x1 square of the quad, so a wide block gets several X shaped cracks.
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

            // The cracks of ONE square (0..1): x0, y0, x1, y1. SEGMENT_STAGE says from which stage each one shows.
            // Stage 1 = a small X-shaped break in the middle, stage 2 = it grows and branches, stage 3 = it reaches the corners.
            #define SEGMENT_COUNT 14
            static const float4 SEGMENTS[SEGMENT_COUNT] =
            {
                float4(0.50, 0.50, 0.30, 0.70),
                float4(0.50, 0.50, 0.66, 0.34),
                float4(0.50, 0.50, 0.70, 0.68),
                float4(0.50, 0.50, 0.32, 0.30),
                float4(0.30, 0.70, 0.22, 0.86),
                float4(0.66, 0.34, 0.80, 0.30),
                float4(0.30, 0.70, 0.08, 0.92),
                float4(0.66, 0.34, 0.92, 0.10),
                float4(0.70, 0.68, 0.92, 0.90),
                float4(0.32, 0.30, 0.10, 0.08),
                float4(0.70, 0.68, 0.84, 0.62),
                float4(0.32, 0.30, 0.20, 0.40),
                float4(0.08, 0.92, 0.04, 0.60),
                float4(0.92, 0.10, 0.96, 0.40)
            };
            static const float SEGMENT_STAGE[SEGMENT_COUNT] = { 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };

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

            float4 frag(Varyings input) : SV_Target
            {
                if (_Stage < 0.5)
                    return 0;

                // position in world units, then split into squares of 1 x 1
                float2 world = input.uv * input.size;
                float2 square = floor(world);
                float2 local = frac(world);

                // every square gets its own mirror, so a wide block does not repeat the same crack
                float h = Hash21(square);
                if (h > 0.5) local.x = 1.0 - local.x;
                if (frac(h * 7.0) > 0.5) local.y = 1.0 - local.y;

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

                float3 color = _Color.rgb * core + _GlowColor.rgb * glow;
                return float4(color, 1);
            }
            ENDHLSL
        }
    }
}
