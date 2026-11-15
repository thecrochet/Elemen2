// DashZone: a field of scrolling chevron arrows (>>>) that looks like it is rushing forward. Made for the fire world.
// The arrows point along the +X (red arrow) of the quad: rotate the quad around Z to change the direction.
// The size comes from the object scale, the arrows keep their shape (Spacing = distance between two chevrons).
//
// THREE COLORS: Arrow Color (the body), Hot Edge Color (the front edge of each arrow), Tail Color (the trail behind it).
// Speed Streaks = thin lines racing along the direction of travel, each row at its own speed.
//
// USE IT ON: a Quad (no collider). The DashZone script builds it for you.
Shader "Elemento/DashZone"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _Color ("Arrow Color (HDR)", Color) = (3, 1.1, 0.1, 1)
        [HDR] _HotColor ("Hot Edge Color (HDR, front of each arrow)", Color) = (6, 4, 1.5, 1)
        [HDR] _TailColor ("Tail Color (HDR, trail behind each arrow)", Color) = (2.2, 0.25, 0.03, 1)

        [Header(Arrows)]
        _Spacing ("Distance Between Arrows (world units)", Range(0.3, 3)) = 1
        _Sharpness ("Arrow Sharpness (bigger = narrower)", Range(0.2, 2)) = 0.8
        _Thickness ("Arrow Thickness (world units)", Range(0.03, 0.4)) = 0.12
        _Speed ("Scroll Speed (arrows per second)", Range(0, 8)) = 2.5

        [Header(Trail and streaks)]
        _TailLength ("Tail Length (world units)", Range(0, 2)) = 0.8
        _TailStrength ("Tail Strength", Range(0, 2)) = 1
        _StreakStrength ("Speed Streaks", Range(0, 2)) = 0.6
        _StreakSpacing ("Streak Row Height (world units)", Range(0.1, 1)) = 0.3

        [Header(Glow and frame)]
        _GlowWidth ("Glow Width (world units)", Range(0.02, 0.6)) = 0.18
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.6
        _FadeLength ("Fade At The Ends (world units)", Range(0, 2)) = 0.6
        [HDR] _FrameColor ("Frame Color (HDR)", Color) = (1.6, 0.5, 0.05, 1)
        _FrameWidth ("Frame Width (world units, 0 = none)", Range(0, 0.1)) = 0.03
        _Fill ("Background Tint", Range(0, 0.3)) = 0.06
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _HotColor;
                float4 _TailColor;
                float4 _FrameColor;
                float _Spacing;
                float _Sharpness;
                float _Thickness;
                float _Speed;
                float _TailLength;
                float _TailStrength;
                float _StreakStrength;
                float _StreakSpacing;
                float _GlowWidth;
                float _GlowStrength;
                float _FadeLength;
                float _FrameWidth;
                float _Fill;
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

                float3 axisX = float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20);
                float3 axisY = float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21);
                output.size = float2(length(axisX), length(axisY));
                return output;
            }

            float Hash11(float n)
            {
                return frac(sin(n * 127.1) * 43758.5453);
            }

            float4 frag(Varyings input) : SV_Target
            {
                // position in world units, (0,0) = the middle of the zone
                float2 p = (input.uv - 0.5) * input.size;
                float2 halfSize = input.size * 0.5;

                // the chevron ">" : its tip is on the middle line, its arms go back along X
                float along = p.x + abs(p.y) * _Sharpness;
                float phase = along / _Spacing - _Time.y * _Speed;
                float s = frac(phase);                      // 0..1 between two arrows, the arrow line sits at 0.5

                // signed distance to the arrow line, in world units: negative = BEHIND the arrow, positive = in front
                float slopeFix = sqrt(1.0 + _Sharpness * _Sharpness);
                float dSigned = (s - 0.5) * _Spacing / slopeFix;
                float d = abs(dSigned);

                float aa = fwidth(d) + 0.0001;
                float core = 1.0 - smoothstep(_Thickness * 0.5 - aa, _Thickness * 0.5 + aa, d);
                float glow = exp(-d / max(_GlowWidth, 0.001)) * _GlowStrength;

                // the front half of the line is the hottest part
                float hot = core * smoothstep(-_Thickness * 0.4, _Thickness * 0.4, dSigned);

                // the trail: a glow that fades out behind each arrow (the same chevron shape, smeared backward)
                float tail = dSigned < 0.0 ? exp(dSigned / max(_TailLength, 0.001)) : 0.0;

                // speed streaks: thin lines along the direction of travel, each row with its own speed and offset
                float rowF = p.y / _StreakSpacing;
                float rowId = floor(rowF);
                float rowRnd = Hash11(rowId + 3.7);
                float yDist = abs(frac(rowF) - 0.5) * _StreakSpacing;
                float thin = 1.0 - smoothstep(0.008, 0.02, yDist);
                float sx = frac(p.x / 2.5 - _Time.y * _Speed * (0.6 + rowRnd) * 0.5 + rowRnd * 9.0);
                float streak = thin * pow(sx, 5.0) * step(0.35, rowRnd);

                float3 color = _Color.rgb * (core + glow * 0.5);
                color += (_HotColor.rgb - _Color.rgb) * hot;
                color += _TailColor.rgb * tail * _TailStrength * 0.7;
                color += _Color.rgb * streak * _StreakStrength;

                // fade near the ends of the zone, so the arrows appear and vanish softly
                float fade = _FadeLength > 0.001 ? saturate((halfSize.x - abs(p.x)) / _FadeLength) : 1.0;
                float side = saturate((halfSize.y - abs(p.y)) / max(_FadeLength * 0.5, 0.001));
                color *= fade * side;

                // a thin frame so the player sees where the zone ends
                float frame = 0.0;
                if (_FrameWidth > 0.0)
                {
                    float edge = min(halfSize.x - abs(p.x), halfSize.y - abs(p.y));
                    frame = 1.0 - smoothstep(_FrameWidth, _FrameWidth + aa * 2.0 + 0.001, edge);
                }

                color += _FrameColor.rgb * frame;
                color += _Color.rgb * _Fill * 0.5;
                return float4(color, 1);
            }
            ENDHLSL
        }
    }
}
