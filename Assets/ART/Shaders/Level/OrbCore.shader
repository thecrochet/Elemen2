// OrbCore: the glowing sphere in the middle of a recharge orb. Hand-written HLSL, unlit, SRP Batcher friendly.
// A solid ball of energy: a slow swirl moving inside it, a bright rim (fresnel) where the surface turns away from the camera,
// and a gentle pulse. Everything is set in the MATERIAL (one material per element), never in the code.
//
// HOW THE LOOK WORKS (beginner version)
//   RIM    : the more a surface point turns away from the camera, the more it glows (the "edge glow" of a sphere).
//   SWIRL  : two waves of sin() run through the object's own positions and change with time, so bright and dark
//            patches slowly drift across the surface. No texture needed.
//   PULSE  : the whole sphere gets a little brighter and darker, over and over.
//   FADE   : far away it fades into the void like every other Elemento object.
//
// USE IT ON: a normal Unity Sphere (no collider). Materials: M_OrbCore_Fire, M_OrbCore_Air, M_OrbCore_Water.
Shader "Elemento/OrbCore"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _CoreColor ("Body Color (HDR)", Color) = (2.2, 0.55, 0.08, 1)
        [HDR] _RimColor ("Rim Color (HDR)", Color) = (3.5, 1.6, 0.3, 1)
        _RimPower ("Rim Sharpness (higher = thinner rim)", Range(0.5, 8)) = 2.5

        [Header(Swirl inside the ball)]
        _SwirlAmount ("Swirl Strength", Range(0, 1)) = 0.35
        _SwirlScale ("Swirl Size (higher = smaller patches)", Range(1, 20)) = 6
        _SwirlSpeed ("Swirl Speed", Range(0, 6)) = 1.2

        [Header(Pulse)]
        _PulseAmount ("Pulse Amount", Range(0, 0.6)) = 0.12
        _PulseSpeed ("Pulse Speed", Float) = 3

        [Header(Fade into the void)]
        _FadeStart ("Fade Start Distance", Float) = 60
        _FadeEnd ("Fade End Distance", Float) = 140
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _CoreColor;
                float4 _RimColor;
                float _RimPower;
                float _SwirlAmount;
                float _SwirlScale;
                float _SwirlSpeed;
                float _PulseAmount;
                float _PulseSpeed;
                float _FadeStart;
                float _FadeEnd;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 positionOS : TEXCOORD2;
                float  camDist    : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionOS = IN.positionOS.xyz;
                OUT.camDist = distance(GetCameraPositionWS(), pos.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);
                float3 viewDir = normalize(GetCameraPositionWS() - IN.positionWS);

                // 1) rim: 0 in the middle of the ball (facing us), 1 at its outline
                float rim = pow(1.0 - saturate(dot(normal, viewDir)), _RimPower);

                // 2) swirl: two moving waves through the object's positions, 0..1
                float3 p = IN.positionOS * _SwirlScale;
                float t = _Time.y * _SwirlSpeed;
                float waveA = sin(p.y * 2.0 + t + sin(p.x * 2.0 - t * 0.7) * 1.5) * 0.5 + 0.5;
                float waveB = sin(p.x * 3.0 - t * 0.6 + p.z * 2.0) * 0.5 + 0.5;
                float swirl = waveA * waveB;

                // 3) the body color, brighter or darker with the swirl, then the rim on top
                float3 body = _CoreColor.rgb * (1.0 + (swirl - 0.35) * 2.0 * _SwirlAmount);
                float3 rgb = lerp(body, _RimColor.rgb, rim);

                // 4) pulse, and the fade into the void
                rgb *= 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;

                float fade = saturate((_FadeEnd - IN.camDist) / max(_FadeEnd - _FadeStart, 0.001));
                rgb *= fade * fade;

                return half4(rgb, 1.0);
            }
            ENDHLSL
        }

        // so the sphere is in the camera depth texture (soft particles, fog and effects that read depth)
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex vertDepth
            #pragma fragment fragDepth

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct AttributesDepth
            {
                float4 positionOS : POSITION;
            };

            struct VaryingsDepth
            {
                float4 positionCS : SV_POSITION;
            };

            VaryingsDepth vertDepth(AttributesDepth IN)
            {
                VaryingsDepth OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 fragDepth(VaryingsDepth IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
