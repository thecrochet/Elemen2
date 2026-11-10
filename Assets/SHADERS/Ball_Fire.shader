// Ball_Fire: dark red glass with a network of glowing lava cracks (they breathe) and a small ember in the middle.
// Put this material in BallVisuals > Fire Material.
Shader "Elemento/Ball/Fire"
{
    Properties
    {
        [Header(Glass)]
        [HDR] _TintColor ("Glass Tint", Color) = (0.45, 0.03, 0.01, 1)
        _GlassAlpha ("Glass Body Opacity", Range(0, 1)) = 0.5
        [HDR] _RimColor ("Rim Color", Color) = (2.5, 0.5, 0.1, 1)
        _RimPower ("Rim Sharpness", Range(0.5, 8)) = 3
        _RimStrength ("Rim Strength", Range(0, 4)) = 1.0
        _BackDim ("Back Side Brightness", Range(0, 1)) = 0.6

        [Header(Lava)]
        [HDR] _HotColor ("Hot Center Color", Color) = (4, 3, 0.8, 1)
        [HDR] _LavaColor ("Lava Color", Color) = (3.5, 1.1, 0.1, 1)
        _CrackScale ("Crack Scale (more = more cracks)", Range(0.5, 6)) = 2.2
        _CrackWidth ("Hot Line Width", Range(0.005, 0.2)) = 0.05
        _GlowWidth ("Orange Glow Width", Range(0.02, 0.5)) = 0.18
        _Coverage ("Lava Coverage", Range(0, 1)) = 0.5
        _FlowSpeed ("Lava Flow Speed", Float) = 0.3
        _PulseSpeed ("Breathing Speed", Float) = 2
        _CoreGlow ("Middle Ember Strength", Range(0, 1)) = 0.35
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
                float4 _HotColor;
                float4 _LavaColor;
                float _CrackScale;
                float _CrackWidth;
                float _GlowWidth;
                float _Coverage;
                float _FlowSpeed;
                float _PulseSpeed;
                float _CoreGlow;
            CBUFFER_END

            float4 Layer(float3 u)
            {
                float t = _Time.y;

                // the crack network = the borders between Voronoi cells
                float f1, f2;
                float3 id, np;
                Voronoi3(u * _CrackScale, f1, f2, id, np);
                float edge = f2 - f1;

                // each cell breathes at its own rhythm, and slow noise makes the lava "flow"
                float breathing = 0.75 + 0.25 * sin(t * _PulseSpeed + hash13(id) * 6.2831);
                float flow = 0.7 + 0.6 * noise3(u * 4.0 + float3(0, -t * _FlowSpeed, 0));

                // only part of the ball is cracked open (a big soft noise decides where)
                float mask = smoothstep(1.0 - _Coverage - 0.12, 1.0 - _Coverage + 0.12, fbm3(u * 1.3 + 7.0));

                float aa = fwidth(edge);
                float hot = 1.0 - smoothstep(_CrackWidth * 0.5, _CrackWidth + aa, edge);   // thin yellow center
                float glow = 1.0 - smoothstep(0.0, _GlowWidth, edge);                      // wider orange glow
                float lava = mask * breathing * flow;

                float3 rgb = (hot * _HotColor.rgb + glow * glow * _LavaColor.rgb) * lava;

                // outside the lava: thin dark cracks in the cold crust
                float thin = (1.0 - smoothstep(0.005, 0.03 + aa, edge)) * (1.0 - mask);

                float a = saturate(glow * lava * 0.9 + thin * 0.6);
                return float4(rgb, a);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                SphereView v = GetSphereView(IN);

                float4 front = Layer(v.front);
                float4 back = Layer(v.back);

                // a small ember in the middle of the ball
                float x = v.impact / 0.45;
                float ember = exp(-x * x) * _CoreGlow;
                float4 mid = float4(_LavaColor.rgb * ember, 0.0);

                float4 col = ComposeGlass(front, mid, back, v, _TintColor.rgb, _GlassAlpha, _BackDim,
                                          _RimColor.rgb, _RimPower, _RimStrength);
                return half4(col);
            }
            ENDHLSL
        }
    }
}
