// NeonShape: the Tron outline for ANY mesh made by the NeonShapeMesh script (hexagon pads, pyramids, spikes...).
// Dark glass inside, a bright white-hot line on every edge, and a soft glow next to it.
// Change the look in the MATERIAL, never in the code (green = safe, cyan = neutral, red/magenta = danger...).
//
// HOW THE OUTLINE WORKS (beginner version)
//   The mesh made by NeonShapeMesh stores, in every corner of every triangle, 3 numbers: how far that corner
//   is from each edge of its triangle. Between the corners the GPU smoothly blends these numbers,
//   so for every pixel we know "how close am I to an edge". Near 0 = on the line.
//   We turn that into world units (so lines keep the same thickness on a big hexagon and a small one).
//
// USE IT ON: a mesh built by NeonShapeMesh. (For cubes use the Elemento/NeonGrid shader.)
Shader "Elemento/NeonShape"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _EdgeColor ("Edge Color (HDR)", Color) = (0, 2.2, 2.8, 1)
        [HDR] _FillColor ("Glass Tint (HDR, keep it very dark)", Color) = (0, 0.02, 0.03, 1)
        _FillAlpha ("Glass Opacity", Range(0, 1)) = 0.2

        [Header(Edge glow)]
        _EdgeWidth ("Core Line Width (world units)", Range(0.005, 0.2)) = 0.04
        _CoreWhite ("Core White-Hot Amount", Range(0, 1)) = 0.5
        _GlowWidth ("Soft Glow Width (world units)", Range(0.02, 1.5)) = 0.25
        _GlowStrength ("Soft Glow Strength", Range(0, 3)) = 0.8
        _TopBoost ("Top Face Boost (walkable)", Range(1, 4)) = 1.5

        [Header(Depth look)]
        _BackDim ("Back Faces Brightness", Range(0, 1)) = 0.4

        [Header(Pulse for danger)]
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0
        _PulseSpeed ("Pulse Speed", Float) = 6

        [Header(Fade into the void)]
        _FadeStart ("Fade Start Distance", Float) = 60
        _FadeEnd ("Fade End Distance", Float) = 140

        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull (Off = see the back edges)", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            // premultiplied: rgb = the light we ADD, alpha = how much of the background the glass hides
            Blend One OneMinusSrcAlpha
            ZWrite Off
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Global reveal properties (outside UnityPerMaterial to stay SRP-Batcher-compatible)
            float  _GridRevealEnabled;
            float3 _GridRevealOrigin;
            float  _GridRevealRadius;
            float  _GridRevealSoftEdge;

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor;
                float4 _FillColor;
                float _FillAlpha;
                float _EdgeWidth;
                float _CoreWhite;
                float _GlowWidth;
                float _GlowStrength;
                float _TopBoost;
                float _BackDim;
                float _PulseAmount;
                float _PulseSpeed;
                float _FadeStart;
                float _FadeEnd;
                float _Cull;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float3 edgeData   : TEXCOORD1;   // written by NeonShapeMesh: the distance to the 3 edges of the triangle
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 edgeData   : TEXCOORD2;
                float  camDist    : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.edgeData = IN.edgeData;
                OUT.camDist = distance(GetCameraPositionWS(), pos.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN, FRONT_FACE_TYPE isFacing : FRONT_FACE_SEMANTIC) : SV_Target
            {
                // 1) how far is this pixel from the nearest drawn edge, in world units?
                //    edgeData / its change per pixel = the distance in pixels (a number kept at 1 = an edge that is not drawn)
                float3 edgePixels = IN.edgeData / max(fwidth(IN.edgeData), 0.00001);
                float pixelsToEdge = min(edgePixels.x, min(edgePixels.y, edgePixels.z));

                //    one pixel is this many world units wide (so lines have the same thickness at any zoom)
                float worldPerPixel = max(length(fwidth(IN.positionWS)), 0.00001);
                float dist = pixelsToEdge * worldPerPixel;

                // 2) the white-hot core line (with a soft one-pixel edge) and the soft glow around it
                float aa = worldPerPixel * 1.5;
                float core = saturate((_EdgeWidth * 0.5 - dist) / aa + 0.5);
                float glow = exp(-dist / max(_GlowWidth, 0.001)) * _GlowStrength;

                // the core goes towards white-hot: the brightest channel of the edge color, on all three channels
                float hot = max(_EdgeColor.r, max(_EdgeColor.g, _EdgeColor.b));
                float3 coreColor = lerp(_EdgeColor.rgb, float3(hot, hot, hot), _CoreWhite);

                // 3) the faces that look up (the ones you walk on) are brighter
                float up = saturate(normalize(IN.normalWS).y);
                float boost = lerp(1.0, _TopBoost, up);

                // 4) the pulse (danger), the back faces seen through the glass, and the fade into the void
                float pulse = 1.0 - _PulseAmount * (0.5 + 0.5 * sin(_Time.y * _PulseSpeed));
                float depthDim = lerp(_BackDim, 1.0, IS_FRONT_VFACE(isFacing, 1.0, 0.0));
                float fade = saturate((_FadeEnd - IN.camDist) / max(_FadeEnd - _FadeStart, 0.001));
                fade *= fade;

                float3 light = (coreColor * core + _EdgeColor.rgb * glow * 0.5) * boost * pulse * depthDim;
                float3 rgb = (light + _FillColor.rgb * _FillAlpha) * fade;
                float alpha = saturate(_FillAlpha + core) * fade;

                // Tutorial reveal sphere: pitch-black outside the wave, smooth ramp at the edge
                // _GridRevealEnabled == 0 -> multiplier is 1 (non-tutorial scenes unchanged)
                float revealMask = smoothstep(_GridRevealRadius,
                                             _GridRevealRadius - max(_GridRevealSoftEdge, 0.001),
                                             distance(IN.positionWS, _GridRevealOrigin));
                float reveal = lerp(1.0, revealMask, step(0.5, _GridRevealEnabled));
                rgb   *= reveal;
                alpha *= reveal;

                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }
}
