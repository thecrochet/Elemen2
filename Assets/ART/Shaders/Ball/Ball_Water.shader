// Ball_Water: a faceted ice crystal. Every facet is a Voronoi cell with its own shade,
// bright edges between facets, frost near the edges and a few sparkles.
// Put this material in BallVisuals > Water Material.
Shader "Elemento/Ball/Water"
{
    Properties
    {
        [Header(Glass)]
        [HDR] _TintColor ("Glass Tint", Color) = (0.35, 0.7, 0.95, 1)
        _GlassAlpha ("Glass Body Opacity", Range(0, 1)) = 0.3
        [HDR] _RimColor ("Rim Color", Color) = (0.8, 1.8, 2.4, 1)
        _RimPower ("Rim Sharpness", Range(0.5, 8)) = 2.5
        _RimStrength ("Rim Strength", Range(0, 4)) = 1.2
        _BackDim ("Back Side Brightness", Range(0, 1)) = 0.6

        [Header(Facets)]
        _FacetScale ("Facet Scale (more = smaller facets)", Range(0.5, 4)) = 1.6
        _FacetLight ("Light Facet Shade", Color) = (0.55, 0.85, 1, 1)
        _FacetDark ("Dark Facet Shade", Color) = (0.1, 0.3, 0.55, 1)
        [HDR] _EdgeColor ("Edge Color", Color) = (1.2, 2.2, 2.8, 1)
        _EdgeWidth ("Edge Width", Range(0.005, 0.15)) = 0.05

        [Header(Frost and sparkle)]
        _FrostAmount ("Frost Amount", Range(0, 1)) = 0.5
        _Sparkle ("Sparkle Strength", Range(0, 2)) = 0.6
        _SparkleSpeed ("Sparkle Speed", Float) = 0.6
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
                float _FacetScale;
                float4 _FacetLight;
                float4 _FacetDark;
                float4 _EdgeColor;
                float _EdgeWidth;
                float _FrostAmount;
                float _Sparkle;
                float _SparkleSpeed;
            CBUFFER_END

            float4 Layer(float3 u)
            {
                float f1, f2;
                float3 id, np;
                Voronoi3(u * _FacetScale, f1, f2, id, np);
                float edge = f2 - f1;

                // each facet gets its own random shade
                float h = hash13(id);
                float3 facet = lerp(_FacetDark.rgb, _FacetLight.rgb, h);

                // bright lines on the borders between facets
                float edgeLine = 1.0 - smoothstep(_EdgeWidth * 0.5, _EdgeWidth + fwidth(edge), edge);

                // frost: white fluff, only near the edges
                float frostMask = smoothstep(0.5, 0.75, fbm3(u * 9.0));
                float frost = (1.0 - smoothstep(0.0, 0.3, edge)) * frostMask * _FrostAmount;

                // sparkle: a facet lights up now and then (each one at its own moment)
                float sparkle = pow(saturate(sin(_Time.y * _SparkleSpeed + h * 40.0)), 16.0) * _Sparkle;

                float3 rgb = facet * 0.25
                           + edgeLine * _EdgeColor.rgb
                           + frost * float3(0.8, 0.95, 1.0) * 0.6
                           + sparkle * facet * 2.0;
                float a = saturate(0.25 + frost * 0.4 + edgeLine * 0.5);
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
