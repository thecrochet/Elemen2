// UI_NeonWaveOutline: URP-compatible UI shader for glowing neon buttons.
// Renders a dark semi-transparent fill with a crisp, glowing neon outline,
// soft outer bloom falloff, and animated perimeter wave ripples driven by unscaled time.
Shader "Elemento/UI/NeonWaveOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Fill)]
        _FillColor ("Fill Color", Color) = (0.02, 0.05, 0.09, 0.88)

        [Header(Border and Glow)]
        [HDR] _BorderColor ("Border Color (HDR)", Color) = (0.0, 2.2, 2.8, 1.0)
        _BorderThickness ("Border Thickness (px)", Range(1.0, 10.0)) = 3.0
        _CornerRadius ("Corner Radius (px)", Range(0.0, 40.0)) = 14.0
        _CoreWhite ("Core White-Hot Amount", Range(0.0, 1.0)) = 0.5
        _GlowSpread ("Outer Glow Spread (px)", Range(1.0, 30.0)) = 8.0
        _GlowStrength ("Outer Glow Strength", Range(0.0, 3.0)) = 1.0
        _InnerGlowSpread ("Inner Glow Spread (px)", Range(0.5, 20.0)) = 6.0
        _InnerGlowStrength ("Inner Glow Strength", Range(0.0, 2.0)) = 0.4

        [Header(Perimeter Wave)]
        _WaveAmplitude ("Wave Amplitude", Range(0.0, 1.0)) = 0.35
        _WaveFrequency ("Wave Frequency", Range(0.001, 0.05)) = 0.015
        _WaveSpeed ("Wave Speed", Range(0.0, 20.0)) = 4.0

        [Header(Geometry and Time)]
        _Dimensions ("Dimensions (Width, Height)", Vector) = (420.0, 84.0, 0, 0)
        _Padding ("Glow Inset Padding (px)", Range(0.0, 30.0)) = 8.0
        _UnscaledTime ("Unscaled Time Override", Float) = 0.0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 worldPosition: TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _FillColor;
                float4 _BorderColor;
                float4 _Dimensions;
                float4 _ClipRect;
                float _BorderThickness;
                float _CornerRadius;
                float _CoreWhite;
                float _GlowSpread;
                float _GlowStrength;
                float _InnerGlowSpread;
                float _InnerGlowStrength;
                float _WaveAmplitude;
                float _WaveFrequency;
                float _WaveSpeed;
                float _Padding;
                float _UnscaledTime;
            CBUFFER_END

            Texture2D _MainTex;
            SamplerState sampler_MainTex;

            // 2D Signed Distance Function for a Rounded Box
            float sdRoundedBox(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
            }

            // Continuous perimeter distance along a rectangle of half-size b
            float getPerimeterDistance(float2 p, float2 b)
            {
                float kw = max(abs(p.x) * b.y, 0.0001);
                float kh = max(abs(p.y) * b.x, 0.0001);

                float s = 0.0;
                if (kh >= kw)
                {
                    // Top or Bottom edge
                    if (p.y >= 0.0)
                    {
                        // Top edge: traveling right from center (0, b.y)
                        s = p.x;
                    }
                    else
                    {
                        // Bottom edge: traveling left from (b.x, -b.y)
                        s = b.x + 2.0 * b.y + (b.x - p.x);
                    }
                }
                else
                {
                    // Right or Left edge
                    if (p.x >= 0.0)
                    {
                        // Right edge: traveling down from (b.x, b.y)
                        s = b.x + (b.y - p.y);
                    }
                    else
                    {
                        // Left edge: traveling up from (-b.x, -b.y)
                        s = 3.0 * b.x + 2.0 * b.y + (p.y + b.y);
                    }
                }

                float totalPerimeter = 4.0 * b.x + 4.0 * b.y;
                if (s < 0.0) s += totalPerimeter;
                return s;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = IN.positionOS;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Soft edge clamp near quad border so glow never hard-clips against image quad
                float quadFade = smoothstep(0.0, 0.03, IN.uv.x) * smoothstep(1.0, 0.97, IN.uv.x) *
                                 smoothstep(0.0, 0.03, IN.uv.y) * smoothstep(1.0, 0.97, IN.uv.y);

                float2 dims = max(_Dimensions.xy, float2(10.0, 10.0));
                float2 pos = (IN.uv - 0.5) * dims;
                float pad = max(_Padding, 0.0);
                float2 halfSize = max(dims * 0.5 - pad, float2(4.0, 4.0));
                float maxR = min(halfSize.x, halfSize.y) * 0.95;
                float radius = clamp(_CornerRadius, 0.0, maxR);

                // Signed distance to the outer edge of the button outline
                float sd = sdRoundedBox(pos, halfSize, radius);

                // Anti-aliasing width
                float aa = max(fwidth(sd), 1.0);

                // Perimeter wave calculation
                float perimDist = getPerimeterDistance(pos, halfSize);
                float timeVal = _UnscaledTime > 0.0 ? _UnscaledTime : _Time.y;
                float wave1 = sin(perimDist * _WaveFrequency - timeVal * _WaveSpeed);
                float wave2 = sin(perimDist * (_WaveFrequency * 1.6) + timeVal * (_WaveSpeed * 0.7));
                float wave = wave1 * 0.7 + wave2 * 0.3;
                float waveMod = 1.0 + _WaveAmplitude * wave;

                // Border core line calculation:
                // Border sits between -_BorderThickness and 0
                float borderOuter = 1.0 - smoothstep(0.0, aa, sd);
                float borderInner = smoothstep(-_BorderThickness - aa, -_BorderThickness, sd);
                float borderMask = saturate(borderOuter * borderInner);

                // Fill calculation (inside the button, beyond the inner border)
                float fillMask = 1.0 - smoothstep(-_BorderThickness, -_BorderThickness + aa, sd);

                // Outer bloom falloff (sd > 0)
                float outerDist = max(sd, 0.0);
                float bloom = exp(-outerDist / max(_GlowSpread, 0.001)) * _GlowStrength;

                // Inner glow near the border inside the button
                float innerDist = max(-sd - _BorderThickness, 0.0);
                float innerBloom = exp(-innerDist / max(_InnerGlowSpread, 0.001)) * _InnerGlowStrength;

                // White-hot core highlight
                float hot = max(_BorderColor.r, max(_BorderColor.g, _BorderColor.b));
                float3 coreColor = lerp(_BorderColor.rgb, float3(hot, hot, hot), _CoreWhite);

                // Premultiplied Color composition:
                // 1. Fill (dark translucent glass)
                float fillAlpha = _FillColor.a * fillMask;
                float3 fillRGB = _FillColor.rgb * fillAlpha;

                // 2. Crisp Border
                float3 borderRGB = coreColor * (borderMask * waveMod);
                float borderAlpha = borderMask;

                // 3. Bloom / Soft Halo (additive light)
                float totalGlow = (bloom + innerBloom * fillMask) * waveMod;
                float3 glowRGB = _BorderColor.rgb * totalGlow;
                float glowAlpha = bloom * 0.6;

                // Combined Premultiplied RGB and Alpha
                float3 finalRGB = fillRGB + borderRGB + glowRGB;
                float finalAlpha = saturate(fillAlpha + borderAlpha + glowAlpha) * quadFade;

                // Premultiplied alpha blend with UI vertex color & alpha
                finalRGB *= IN.color.rgb * IN.color.a * quadFade;
                finalAlpha *= IN.color.a;

                #ifdef UNITY_UI_CLIP_RECT
                float2 clipMin = _ClipRect.xy;
                float2 clipMax = _ClipRect.zw;
                float2 clipTest = step(clipMin, IN.worldPosition.xy) * step(IN.worldPosition.xy, clipMax);
                finalAlpha *= clipTest.x * clipTest.y;
                finalRGB *= clipTest.x * clipTest.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(finalAlpha - 0.001);
                #endif

                return half4(finalRGB, finalAlpha);
            }
            ENDHLSL
        }
    }
}
