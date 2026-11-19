// Ball_Aether: dark purple glass with a starfield inside. Stars are the random points of a Voronoi grid,
// thin lines link each star to its neighbors (constellations), plus a little dust and a faint nebula.
// No slot in BallVisuals yet: a spare look for a 5th element, a secret power or the goal.
Shader "Elemento/Ball/Aether"
{
    Properties
    {
        [Header(Glass)]
        [HDR] _TintColor ("Glass Tint", Color) = (0.25, 0.05, 0.5, 1)
        _GlassAlpha ("Glass Body Opacity", Range(0, 1)) = 0.5
        [HDR] _RimColor ("Rim Color", Color) = (1.4, 0.5, 2.2, 1)
        _RimPower ("Rim Sharpness", Range(0.5, 8)) = 2.5
        _RimStrength ("Rim Strength", Range(0, 4)) = 1.3
        _BackDim ("Back Side Brightness", Range(0, 1)) = 0.6

        [Header(Stars)]
        [HDR] _StarColor ("Star Color", Color) = (2.5, 1.4, 3, 1)
        _StarScale ("Star Density (more = more stars)", Range(0.6, 3)) = 1.4
        _StarSize ("Star Size", Range(0.01, 0.2)) = 0.07
        _TwinkleSpeed ("Twinkle Speed", Float) = 2

        [Header(Constellation lines)]
        [HDR] _LineColor ("Line Color", Color) = (1.2, 0.5, 1.8, 1)
        _LinkDist ("Max Line Length", Range(0.3, 1.8)) = 1.1
        _LineWidth ("Line Width", Range(0.003, 0.05)) = 0.012

        [Header(Dust and nebula)]
        _DustAmount ("Dust Amount", Range(0, 1)) = 0.5
        [HDR] _NebulaColor ("Nebula Color", Color) = (0.3, 0.06, 0.6, 1)
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
                float4 _StarColor;
                float _StarScale;
                float _StarSize;
                float _TwinkleSpeed;
                float4 _LineColor;
                float _LinkDist;
                float _LineWidth;
                float _DustAmount;
                float4 _NebulaColor;
            CBUFFER_END

            // Global, set by IntroSequence. 1 = ball invisible, 0 = normal. Never set = 0, so nothing else changes.
            float _ElementoBallHide;

            float4 Layer(float3 u)
            {
                float t = _Time.y;
                float3 p = u * _StarScale;

                // the nearest star (the random point of the nearest cell)
                float f1, f2;
                float3 id, star;
                Voronoi3(p, f1, f2, id, star);

                // the star itself: a small bright dot that twinkles, each star with its own size and rhythm
                float size = _StarSize * (0.5 + hash13(id + 5.0));
                float twinkle = 0.65 + 0.35 * sin(t * _TwinkleSpeed + hash13(id) * 6.2831);
                float starGlow = pow(saturate(1.0 - distance(p, star) / size), 2.0) * twinkle;

                // constellation lines: from this star to the stars of the 26 neighbor cells (if they are close enough)
                float lines = 0.0;

                [unroll]
                for (int z = -1; z <= 1; z++)
                {
                    [unroll]
                    for (int y = -1; y <= 1; y++)
                    {
                        [unroll]
                        for (int x = -1; x <= 1; x++)
                        {
                            float3 neighborCell = id + float3(x, y, z);
                            float3 other = neighborCell + hash33(neighborCell);
                            float3 along = other - star;
                            float len = length(along);

                            // 0 for itself and for stars that are too far
                            float valid = step(0.01, len) * (1.0 - smoothstep(_LinkDist * 0.7, _LinkDist, len));

                            // distance from this pixel to the segment star-other
                            float3 toPixel = p - star;
                            float along01 = saturate(dot(toPixel, along) / max(dot(along, along), 0.00001));
                            float dSeg = length(toPixel - along * along01);

                            lines = max(lines, valid * (1.0 - smoothstep(_LineWidth * 0.5, _LineWidth + fwidth(dSeg), dSeg)));
                        }
                    }
                }

                // dust: tiny dim dots, at most one per small cell
                float3 dp = u * 14.0;
                float3 dCell = floor(dp);
                float3 dOffset = 0.25 + 0.5 * hash33(dCell);
                float dust = step(0.8, hash13(dCell + 3.0)) * smoothstep(0.12, 0.0, distance(frac(dp), dOffset)) * _DustAmount;

                // nebula: a soft slow cloud
                float nebula = fbm3(u * 2.0 + t * 0.02);

                float3 rgb = starGlow * _StarColor.rgb + lines * _LineColor.rgb + dust * _StarColor.rgb * 0.5
                           + nebula * _NebulaColor.rgb * 0.4;
                float a = saturate(starGlow + lines * 0.6 + nebula * 0.25);
                return float4(rgb, a);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                SphereView v = GetSphereView(IN);

                float4 front = Layer(v.front);
                float4 back = Layer(v.back);

                float4 col = ComposeGlass(front, float4(0, 0, 0, 0), back, v, _TintColor.rgb, _GlassAlpha, _BackDim,
                                          _RimColor.rgb, _RimPower, _RimStrength);
                // tutorial intro: fade the whole ball (colour and alpha together, it is premultiplied)
                col *= 1.0 - _ElementoBallHide;
                return half4(col);
            }
            ENDHLSL
        }
    }
}
