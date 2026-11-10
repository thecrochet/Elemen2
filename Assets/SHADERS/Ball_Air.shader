// Ball_Air: almost clear glass with white swirling lines (two spirals) and a light haze inside.
// Put this material in BallVisuals > Air Material.
Shader "Elemento/Ball/Air"
{
    Properties
    {
        [Header(Glass)]
        [HDR] _TintColor ("Glass Tint", Color) = (0.8, 0.92, 1, 1)
        _GlassAlpha ("Glass Body Opacity", Range(0, 1)) = 0.12
        [HDR] _RimColor ("Rim Color", Color) = (1.6, 2.2, 2.6, 1)
        _RimPower ("Rim Sharpness", Range(0.5, 8)) = 2.5
        _RimStrength ("Rim Strength", Range(0, 4)) = 1.4
        _BackDim ("Back Side Brightness", Range(0, 1)) = 0.6

        [Header(Swirl)]
        [HDR] _SwirlColor ("Swirl Color", Color) = (1.6, 2.2, 2.6, 1)
        _SwirlSpacing ("Lines Spacing (more = more lines)", Range(0.5, 6)) = 2.2
        _SwirlWidth ("Line Width", Range(0.01, 0.3)) = 0.08
        _SwirlSpeed ("Swirl Speed", Float) = 0.25
        _Warp ("Wobble (makes the lines wavy)", Range(0, 1)) = 0.3

        [Header(Haze)]
        _HazeAmount ("Haze Amount", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "ElementCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _TintColor;
                float _GlassAlpha;
                float4 _RimColor;
                float _RimPower;
                float _RimStrength;
                float _BackDim;
                float4 _SwirlColor;
                float _SwirlSpacing;
                float _SwirlWidth;
                float _SwirlSpeed;
                float _Warp;
                float _HazeAmount;
            CBUFFER_END

            // A spiral around the point "center" (a direction on the ball).
            // phi = angle away from the center, theta = angle around it. A spiral line = phi and theta change together.
            float Spiral(float3 u, float3 center)
            {
                float t = _Time.y;

                // two directions that are flat against the center (to measure the turn around it)
                float3 helper = abs(center.y) < 0.9 ? float3(0, 1, 0) : float3(1, 0, 0);
                float3 t1 = normalize(cross(center, helper));
                float3 t2 = cross(center, t1);

                float theta = atan2(dot(u, t2), dot(u, t1));
                float phi = acos(clamp(dot(u, center), -1.0, 1.0));

                // a bit of noise makes the lines wavy
                phi += (noise3(u * 2.5 + t * 0.1) - 0.5) * _Warp;

                float f = frac(phi * _SwirlSpacing - theta / 6.2831853 + t * _SwirlSpeed);
                float d = abs(f - 0.5);
                return 1.0 - smoothstep(_SwirlWidth * 0.5, _SwirlWidth + fwidth(d), d);
            }

            float4 Layer(float3 u)
            {
                float3 c1 = normalize(float3(0.2, 1.0, 0.1));
                float3 c2 = normalize(float3(-0.7, -0.3, 0.6));

                // each spiral only covers its own half of the ball, so they don't fight
                float w1 = smoothstep(-0.2, 0.5, dot(u, c1));
                float w2 = smoothstep(-0.2, 0.5, dot(u, c2));
                float swirl = max(Spiral(u, c1) * w1, Spiral(u, c2) * w2);

                float haze = fbm3(u * 2.0 + _Time.y * 0.05) * _HazeAmount;

                float3 rgb = swirl * _SwirlColor.rgb + haze * 0.25;
                float a = saturate(swirl * 0.5 + haze * 0.3);
                return float4(rgb, a);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                SphereView v = GetSphereView(IN);

                float4 front = Layer(v.front);
                float4 back = Layer(v.back);

                float4 col = ComposeGlass(front, float4(0, 0, 0, 0), back, v, _TintColor.rgb, _GlassAlpha, _BackDim,
                                          _RimColor.rgb, _RimPower, _RimStrength);
                return half4(col);
            }
            ENDHLSL
        }
    }
}
