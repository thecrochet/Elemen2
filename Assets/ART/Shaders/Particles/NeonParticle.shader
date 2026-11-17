// NeonParticle: ONE particle shader for every neon effect (no textures).
// The shape is DRAWN by math inside the shader: circle, square, triangle, hexagon, ring or streak.
// The COLOR comes from the Particle System ("Start Color", "Color over Lifetime"), so one material
// per shape works for every world. The glow strength is the HDR "Glow Tint" of the material.
//
// MATERIALS TO MAKE (shader = Elemento/NeonParticle):
//   P_Square, P_SquareHollow (Outline Only on), P_Triangle, P_Hex, P_HexHollow, P_Circle, P_Ring, P_Streak
Shader "Elemento/NeonParticle"
{
    Properties
    {
        [Enum(Circle,0,Square,1,Triangle,2,Hexagon,3,Ring,4,Streak,5)] _Shape ("Shape", Float) = 1
        [HDR] _Tint ("Glow Tint (HDR: raise the intensity for more glow)", Color) = (2, 2, 2, 1)
        _ShapeSize ("Shape Size (inside the quad)", Range(0.2, 1)) = 0.6
        [Toggle] _Hollow ("Outline Only", Float) = 0
        _LineWidth ("Outline Width", Range(0.02, 0.6)) = 0.2
        _Glow ("Glow Size", Range(0.02, 1)) = 0.25
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One One        // pure light: particles add up, so no sorting problems
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Shape;
                float4 _Tint;
                float _ShapeSize;
                float _Hollow;
                float _LineWidth;
                float _Glow;
                float _GlowStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;       // the color of the particle (from the Particle System)
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.uv = IN.uv;
                return OUT;
            }

            // Distance functions: negative inside the shape, 0 on its border, positive outside
            float SdTriangle(float2 p)
            {
                const float k = 1.7320508;
                p.x = abs(p.x) - 1.0;
                p.y = p.y + 1.0 / k;

                if (p.x + k * p.y > 0.0)
                    p = float2(p.x - k * p.y, -k * p.x - p.y) * 0.5;

                p.x -= clamp(p.x, -2.0, 0.0);
                return -length(p) * sign(p.y);
            }

            float SdHexagon(float2 p, float r)
            {
                const float3 k = float3(-0.866025404, 0.5, 0.577350269);
                p = abs(p);
                p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
                p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
                return length(p) * sign(p.y);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 p = IN.uv * 2.0 - 1.0;      // -1 to 1 across the quad, 0 in the middle
                float2 q = p / _ShapeSize;         // the shape lives in a smaller area, so the glow has room
                int shape = (int)(_Shape + 0.5);
                float d;

                if (shape == 0)
                {
                    d = length(q) - 1.0;                                // circle
                }
                else if (shape == 1)
                {
                    d = max(abs(q.x), abs(q.y)) - 0.85;                 // square
                }
                else if (shape == 2)
                {
                    d = SdTriangle(q * 1.25) / 1.25;                    // triangle
                }
                else if (shape == 3)
                {
                    d = SdHexagon(q, 0.9);                              // hexagon
                }
                else if (shape == 4)
                {
                    d = abs(length(q) - 0.85) - 0.1;                    // ring
                }
                else
                {
                    float2 s = q;                                       // streak (a short line)
                    s.x -= clamp(s.x, -1.0, 1.0);
                    d = length(s) - 0.18;
                }

                // outline only: keep a band of "Outline Width" just inside the border
                if (_Hollow > 0.5 && shape != 4)
                    d = abs(d + _LineWidth * 0.5) - _LineWidth * 0.5;

                float core = 1.0 - smoothstep(-fwidth(d), fwidth(d), d);                  // the sharp shape
                float glow = exp(-max(d, 0.0) / max(_Glow, 0.001)) * _GlowStrength;       // the soft glow around it
                float edgeFade = 1.0 - smoothstep(0.7, 1.0, length(p));                    // the quad border stays black

                float3 rgb = IN.color.rgb * _Tint.rgb * (core + glow * edgeFade) * IN.color.a;
                return half4(rgb, 1);
            }
            ENDHLSL
        }
    }
}
