// Ball_Earth: a brown cracked glass shell around a solid rocky lump. This is the only ball that is LIT:
// the rock core has bumps and shading from a fixed light direction.
// Put this material in BallVisuals > Earth Material.
Shader "Elemento/Ball/Earth"
{
    Properties
    {
        [Header(Glass shell)]
        [HDR] _TintColor ("Glass Tint", Color) = (0.45, 0.22, 0.08, 1)
        _GlassAlpha ("Glass Body Opacity", Range(0, 1)) = 0.4
        [HDR] _RimColor ("Rim Color", Color) = (1.6, 0.8, 0.3, 1)
        _RimPower ("Rim Sharpness", Range(0.5, 8)) = 3
        _RimStrength ("Rim Strength", Range(0, 4)) = 0.9
        _BackDim ("Back Side Brightness", Range(0, 1)) = 0.6
        _CrackScale ("Shell Crack Scale", Range(0.5, 5)) = 1.8
        [HDR] _CrackGlow ("Shell Crack Glint", Color) = (1.2, 0.6, 0.2, 1)

        [Header(Rock core)]
        _CoreRadius ("Core Size", Range(0.2, 0.9)) = 0.62
        _RockLight ("Rock Light Color", Color) = (0.75, 0.42, 0.18, 1)
        _RockDark ("Rock Dark Color", Color) = (0.2, 0.1, 0.05, 1)
        _RockScale ("Rock Detail Scale", Range(1, 10)) = 4
        _BumpStrength ("Bump Strength", Range(0, 2)) = 0.6

        [Header(Light)]
        _LightDir ("Light Direction (world)", Vector) = (-0.4, 0.7, -0.6, 0)
        _Ambient ("Ambient Light", Range(0, 1)) = 0.35
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
                float _CrackScale;
                float4 _CrackGlow;
                float _CoreRadius;
                float4 _RockLight;
                float4 _RockDark;
                float _RockScale;
                float _BumpStrength;
                float4 _LightDir;
                float _Ambient;
            CBUFFER_END

            // The shell: thin dark cracks (Voronoi borders) with a faint warm glint
            float4 ShellLayer(float3 u)
            {
                float f1, f2;
                float3 id, np;
                Voronoi3(u * _CrackScale, f1, f2, id, np);
                float edge = f2 - f1;

                float crack = 1.0 - smoothstep(0.01, 0.04 + fwidth(edge), edge);
                return float4(_CrackGlow.rgb * crack * 0.3, crack * 0.7);
            }

            float RockHeight(float3 n)
            {
                return fbm3(n * _RockScale);
            }

            // The rocky lump inside: a smaller sphere that we find with the same ray math
            float4 RockCore(SphereView v)
            {
                float R = _CoreRadius;
                float b = v.impact;                                  // how far the line of sight is from the center
                float s = sqrt(max(R * R - b * b, 0.0));             // half the length of the line inside the core
                float s0 = -dot(v.front, v.dir);                     // distance to the closest point to the center
                float3 hit = v.front + v.dir * (s0 - s);             // where the line enters the core
                float3 n = normalize(hit);                           // direction from the center = the surface normal

                // bumps: the slope of the rock height (checked on 3 axes)
                float h = RockHeight(n);
                float e = 0.04;
                float3 grad = float3(RockHeight(n + float3(e, 0, 0)) - h,
                                     RockHeight(n + float3(0, e, 0)) - h,
                                     RockHeight(n + float3(0, 0, e)) - h) / e;
                float3 bumped = normalize(n - grad * _BumpStrength * 0.3);

                // light: the normal is turned into world space, because the light does not roll with the ball
                float3 normalWS = normalize(TransformObjectToWorldNormal(bumped));
                float diffuse = saturate(dot(normalWS, normalize(_LightDir.xyz)));
                float lit = _Ambient + (1.0 - _Ambient) * diffuse;

                float3 color = lerp(_RockDark.rgb, _RockLight.rgb, smoothstep(0.3, 0.7, h));
                color *= lerp(0.5, 1.0, smoothstep(0.25, 0.45, h));   // dark crevices
                color *= lit;

                float cover = 1.0 - smoothstep(R - 0.025, R, b);      // soft edge of the core
                return float4(color * cover, cover);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                SphereView v = GetSphereView(IN);

                float4 front = ShellLayer(v.front);
                float4 back = ShellLayer(v.back);
                float4 core = RockCore(v);

                float4 col = ComposeGlass(front, core, back, v, _TintColor.rgb, _GlassAlpha, _BackDim,
                                          _RimColor.rgb, _RimPower, _RimStrength);
                return half4(col);
            }
            ENDHLSL
        }
    }
}
