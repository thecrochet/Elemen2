// NeonGrid v2: Tron look. See-through glass inside, a bright white-hot core line, a soft glow around it,
// and a faint grid (one cell = one box). Change the look in the MATERIAL, never in the code.
//
// HOW THE OUTLINE WORKS (beginner version)
//   Each face of a cube is a flat rectangle. For every pixel we measure how far it is from the nearest
//   border of its face, in WORLD UNITS (the object's size is taken into account).
//   Very close to the border = the white-hot core line. A bit further = the soft colored glow.
//   So a platform scaled to 12 x 1 x 1 keeps the SAME line thickness as a small block.
//
// USE IT ON: Unity's default Cube, scaled with the Transform (the NeonBlock script does it for you).
// For other meshes, turn on "Use mesh UVs" (see the property at the bottom).
Shader "Elemento/NeonGrid"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _EdgeColor ("Edge Color (HDR)", Color) = (0, 2.2, 2.8, 1)
        [HDR] _FillColor ("Glass Tint (HDR, keep it very dark)", Color) = (0, 0.02, 0.03, 1)
        [HDR] _GridColor ("Grid Color (HDR)", Color) = (0, 0.5, 0.7, 1)

        [Header(Edge glow)]
        _EdgeWidth ("Core Line Width (world units)", Range(0.005, 0.2)) = 0.04
        _CoreWhite ("Core White-Hot Amount", Range(0, 1)) = 0.5
        _GlowWidth ("Soft Glow Width (world units)", Range(0.02, 1.5)) = 0.3
        _GlowStrength ("Soft Glow Strength", Range(0, 3)) = 0.8
        _TopBoost ("Top Face Boost (walkable)", Range(1, 4)) = 1.5

        [Header(Grid)]
        _GridAmount ("Grid Strength", Range(0, 1)) = 0.35
        _GridSize ("Grid Cell Size (world units)", Float) = 1
        _GridWidth ("Grid Line Width (world units)", Range(0.002, 0.1)) = 0.02

        [Header(Depth look)]
        _BackDim ("Back Faces Brightness", Range(0, 1)) = 0.35

        [Header(Pulse for danger)]
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0
        _PulseSpeed ("Pulse Speed", Float) = 6

        [Header(Fade into the void)]
        _FadeStart ("Fade Start Distance", Float) = 60
        _FadeEnd ("Fade End Distance", Float) = 140

        [Header(Transparency)]
        // Additive (One / One) = pure light, the Tron look.
        // Glass (SrcAlpha / OneMinusSrcAlpha) = a see-through box with a body, uses Glass Opacity.
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination Blend", Float) = 1
        _FillAlpha ("Glass Opacity (Glass blend only)", Range(0, 1)) = 0.08
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull (Off = see the back edges)", Float) = 0

        [Header(Mesh type)]
        [Toggle(_UV_MODE)] _UVMode ("Use mesh UVs instead of cube bounds", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite Off
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _UV_MODE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor;
                float4 _FillColor;
                float4 _GridColor;
                float _EdgeWidth;
                float _CoreWhite;
                float _GlowWidth;
                float _GlowStrength;
                float _TopBoost;
                float _GridAmount;
                float _GridSize;
                float _GridWidth;
                float _BackDim;
                float _PulseAmount;
                float _PulseSpeed;
                float _FadeStart;
                float _FadeEnd;
                float _SrcBlend;
                float _DstBlend;
                float _FillAlpha;
                float _Cull;
                float _UVMode;
            CBUFFER_END

            // Global, set by GridReveal. 1 = grid hidden, 0 = normal. Never set = 0, so other levels are untouched.
            float _ElementoGridHide;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 posOS      : TEXCOORD0;   // position inside the cube (-0.5 to 0.5)
                float3 normalOS   : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float2 uv         : TEXCOORD3;
                float3 scale      : TEXCOORD4;   // size of the object in world units
                float  camDist    : TEXCOORD5;   // distance to the camera
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.posOS = IN.positionOS.xyz;
                OUT.normalOS = IN.normalOS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;

                // the length of each column of the matrix = the scale of the object on that axis
                float4x4 m = GetObjectToWorldMatrix();
                OUT.scale = float3(length(m._m00_m10_m20), length(m._m01_m11_m21), length(m._m02_m12_m22));

                OUT.camDist = distance(GetCameraPositionWS(), pos.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN, FRONT_FACE_TYPE isFacing : FRONT_FACE_SEMANTIC) : SV_Target
            {
                // 1) Find the 2D position on THIS face (t) and the size of the face (size), in world units
                float2 t;
                float2 size;

                #if defined(_UV_MODE)
                    t = IN.uv;
                    size = float2(1, 1);
                #else
                    float3 q = IN.posOS + 0.5;          // 0 to 1 across the cube
                    float3 a = abs(IN.normalOS);        // which way does this face look?

                    if (a.x >= a.y && a.x >= a.z)       // left / right face
                    {
                        t = q.yz * IN.scale.yz;
                        size = IN.scale.yz;
                    }
                    else if (a.y >= a.z)                // top / bottom face
                    {
                        t = q.xz * IN.scale.xz;
                        size = IN.scale.xz;
                    }
                    else                                // front / back face
                    {
                        t = q.xy * IN.scale.xy;
                        size = IN.scale.xy;
                    }
                #endif

                // 2) Distance to the nearest border of the face
                float2 toBorder = min(t, size - t);
                float edgeDist = min(toBorder.x, toBorder.y);

                // 3) CORE: the thin white-hot line (fwidth keeps it sharp but not jagged)
                float edgeAA = fwidth(edgeDist);
                float core = 1.0 - smoothstep(_EdgeWidth - edgeAA, _EdgeWidth + edgeAA, edgeDist);

                // 4) HALO: a soft glow that fades away from the border (it works even without Bloom)
                float halo = exp(-edgeDist / max(_GlowWidth, 0.001)) * _GlowStrength;

                // 5) GRID: distance to the nearest grid line (a repeating triangle wave). One cell = one box.
                float cell = max(_GridSize, 0.001);
                float2 gLine = abs(frac(t / cell + 0.5) - 0.5) * cell;
                float2 gAA = fwidth(gLine);
                float2 gMask = 1.0 - smoothstep(_GridWidth - gAA, _GridWidth + gAA, gLine);
                float grid = max(gMask.x, gMask.y);

                // 6) Walkable faces (looking up) glow stronger: you read the floor first
                float top = step(0.9, normalize(IN.normalWS).y);
                float boost = lerp(1.0, _TopBoost, top);

                // 7) Pulse: only dims, never over-brightens (0 amount = steady)
                float pulse = 1.0 - _PulseAmount * (0.5 + 0.5 * sin(_Time.y * _PulseSpeed));

                // 8) Back faces (seen through the glass) are dimmer, so the front reads first
                float depthDim = lerp(_BackDim, 1.0, IS_FRONT_VFACE(isFacing, 1.0, 0.0));

                // 9) The core line is the edge color pushed toward white
                float3 coreColor = lerp(_EdgeColor.rgb, max(max(_EdgeColor.r, _EdgeColor.g), _EdgeColor.b).xxx, _CoreWhite);

                // 10) Mix everything as LIGHT (the inside stays transparent)
                float3 col = _FillColor.rgb;
                col += _GridColor.rgb * grid * _GridAmount;
                col += _EdgeColor.rgb * halo * boost;
                col += coreColor * core * boost;
                col *= pulse * depthDim;

                // 11) Far objects melt into the black void
                float fade = saturate((_FadeEnd - IN.camDist) / max(_FadeEnd - _FadeStart, 0.001));
                fade *= fade;
                col *= fade;

                // alpha is only used by the Glass blend (SrcAlpha / OneMinusSrcAlpha)
                float alpha = saturate(_FillAlpha + core + halo * 0.5) * fade;

                // tutorial grid reveal: fade the whole grid in and out with one global value
                float reveal = 1.0 - _ElementoGridHide;
                col *= reveal;
                alpha *= reveal;

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
