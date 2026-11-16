// DashZone: a field of scrolling chevron arrows (>>>). It shows "go this way, fast". Made for the fire world.
// The arrows point along the +X (red arrow) of the quad: rotate the quad around Z to change the direction.
// The size comes from the object scale, the arrows keep their shape (Spacing = distance between two chevrons).
//
// USE IT ON: a Quad (no collider). The DashZone script builds it for you.
Shader "Elemento/DashZone"
{
    Properties
    {
        [HDR] _Color ("Arrow Color (HDR)", Color) = (3, 1.1, 0.1, 1)
        [HDR] _FrameColor ("Frame Color (HDR)", Color) = (1.6, 0.5, 0.05, 1)
        _Spacing ("Distance Between Arrows (world units)", Range(0.3, 3)) = 1
        _Sharpness ("Arrow Sharpness (bigger = narrower)", Range(0.2, 2)) = 0.8
        _Thickness ("Arrow Thickness (world units)", Range(0.03, 0.4)) = 0.12
        _Speed ("Scroll Speed (arrows per second)", Range(0, 4)) = 1
        _GlowWidth ("Glow Width (world units)", Range(0.02, 0.6)) = 0.18
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.6
        _FadeLength ("Fade At The Ends (world units)", Range(0, 2)) = 0.6
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
                float4 _FrameColor;
                float _Spacing;
                float _Sharpness;
                float _Thickness;
                float _Speed;
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

            float4 frag(Varyings input) : SV_Target
            {
                // position in world units, (0,0) = the middle of the zone
                float2 p = (input.uv - 0.5) * input.size;
                float2 halfSize = input.size * 0.5;

                // the chevron ">" : its tip is on the middle line, its arms go back along X
                float along = p.x + abs(p.y) * _Sharpness;
                float phase = along / _Spacing - _Time.y * _Speed;

                // distance to the closest arrow line, measured along X, then converted to a real distance
                float d = abs(frac(phase) - 0.5) * _Spacing;
                d = (_Spacing * 0.5 - d) ;                 // 0 on the line, grows away from it
                d = d / sqrt(1.0 + _Sharpness * _Sharpness);

                float aa = fwidth(d) + 0.0001;
                float core = 1.0 - smoothstep(_Thickness * 0.5 - aa, _Thickness * 0.5 + aa, d);
                float glow = exp(-d / max(_GlowWidth, 0.001)) * _GlowStrength;

                // fade near the ends of the zone, so the arrows appear and vanish softly
                float fade = _FadeLength > 0.001 ? saturate((halfSize.x - abs(p.x)) / _FadeLength) : 1.0;
                float side = saturate((halfSize.y - abs(p.y)) / max(_FadeLength * 0.5, 0.001));

                float3 color = _Color.rgb * (core + glow * 0.5) * fade * side;

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
