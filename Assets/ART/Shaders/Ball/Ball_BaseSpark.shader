// Ball_BaseSpark: the no-power ball. Yellow glass, a pulsing white-hot core, lightning, thin energy rings.
// Put this material in BallVisuals > No Power Material.
Shader "Elemento/Ball/BaseSpark"
{
    Properties
    {
        [Header(Glass)]
        [HDR] _TintColor ("Glass Tint", Color) = (1.0, 0.85, 0.2, 1)
        _GlassAlpha ("Glass Body Opacity", Range(0, 1)) = 0.18
        [HDR] _RimColor ("Rim Color", Color) = (2.0, 1.6, 0.4, 1)
        _RimPower ("Rim Sharpness", Range(0.5, 8)) = 3
        _RimStrength ("Rim Strength", Range(0, 4)) = 1.2
        _BackDim ("Back Side Brightness", Range(0, 1)) = 0.6

        [Header(Core)]
        [HDR] _CoreColor ("Core Color", Color) = (4, 3.2, 1, 1)
        _CoreSize ("Core Size", Range(0.05, 1)) = 0.3
        _PulseSpeed ("Core Pulse Speed", Float) = 3
        _PulseAmount ("Core Pulse Amount", Range(0, 1)) = 0.3

        [Header(Lightning)]
        [HDR] _BoltColor ("Lightning Color", Color) = (3, 2.4, 0.6, 1)
        _BoltScale ("Lightning Scale", Range(0.5, 8)) = 3
        _BoltSpeed ("Lightning Speed", Float) = 1.5
        _BoltWidth ("Lightning Width", Range(0.005, 0.2)) = 0.05

        [Header(Rings)]
        [HDR] _RingColor ("Ring Color", Color) = (2, 1.6, 0.4, 1)
        _RingWidth ("Ring Width", Range(0.002, 0.05)) = 0.012
        _RingSpeed ("Ring Drift Speed", Float) = 0.4
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
                float4 _CoreColor;
                float _CoreSize;
                float _PulseSpeed;
                float _PulseAmount;
                float4 _BoltColor;
                float _BoltScale;
                float _BoltSpeed;
                float _BoltWidth;
                float4 _RingColor;
                float _RingWidth;
                float _RingSpeed;
            CBUFFER_END

            // The pattern on ONE side of the ball. u = the point on the ball.
            float4 Layer(float3 u)
            {
                float t = _Time.y;

                // lightning: the thin line where the animated noise crosses 0.5
                float n = fbm3(u * _BoltScale + float3(0, t * _BoltSpeed, 0));
                float ridge = abs(n - 0.5);
                float bolt = 1.0 - smoothstep(0.0, max(_BoltWidth, fwidth(ridge) * 1.5), ridge);

                // rings: great circles (like the equator) around slowly drifting axes
                float rings = 0.0;

                [unroll]
                for (int i = 0; i < 5; i++)
                {
                    float k = (float)i;
                    float3 axis = normalize(float3(sin(t * _RingSpeed + k * 1.9),
                                                   cos(t * _RingSpeed * 0.7 + k * 2.3),
                                                   sin(t * _RingSpeed * 0.5 + k * 3.1 + 1.0)));
                    float d = abs(dot(u, axis));
                    rings = max(rings, 1.0 - smoothstep(0.0, max(_RingWidth, fwidth(d) * 1.5), d));
                }

                float3 rgb = bolt * _BoltColor.rgb + rings * _RingColor.rgb;
                float a = saturate(bolt * 0.5 + rings * 0.4);
                return float4(rgb, a);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                SphereView v = GetSphereView(IN);

                float4 front = Layer(v.front);
                float4 back = Layer(v.back);

                // the core: bright in the middle of the ball, seen through the glass
                float pulse = 1.0 + _PulseAmount * sin(_Time.y * _PulseSpeed);
                float x = v.impact / _CoreSize;
                float core = exp(-x * x) * pulse;
                float4 mid = float4(_CoreColor.rgb * core, saturate(core) * 0.6);

                float4 col = ComposeGlass(front, mid, back, v, _TintColor.rgb, _GlassAlpha, _BackDim,
                                          _RimColor.rgb, _RimPower, _RimStrength);
                return half4(col);
            }
            ENDHLSL
        }
    }
}
