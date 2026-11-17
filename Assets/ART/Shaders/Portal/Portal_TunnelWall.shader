// Portal_TunnelWall: the inside of a REAL tunnel (a tube mesh made by the NeonPortalTunnel script).
// Glowing rings slide toward the far end, lines run along the walls, a pulse of light travels inward,
// and everything fades to black with distance. At the far end there is a glowing disc (the "cap").
// Because the tunnel is real 3D geometry, the camera's own perspective gives the depth: no fake tricks needed.
// This shader is opaque (it hides what is behind it). Put the transparent portal on a Quad IN FRONT of the mouth of the tube.
//
// The mesh gives us:
//   uv0.x = position around the tube, counted in SIDES (one unit = one side of the polygon)
//   uv0.y = distance from the mouth, in world units (0 at the mouth, Depth at the far end)
//   uv1.x = 0 in the middle of the cap and 1 at its border, uv1.y = 1 on the cap and 0 on the walls
Shader "Elemento/Portal/TunnelWall"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _WallColor ("Wall Base Color", Color) = (0.004, 0.01, 0.02, 1)
        [HDR] _RingColor ("Ring Color", Color) = (0.3, 1.6, 2.4, 1)
        [HDR] _LineColor ("Line Color", Color) = (0.2, 1.0, 1.6, 1)
        [HDR] _CoreColor ("Far End Glow", Color) = (1.5, 3, 4, 1)

        [Header(Rings)]
        _RingSpacing ("Ring Spacing (units)", Range(0.3, 3)) = 1
        _RingWidth ("Ring Thickness (units)", Range(0.01, 0.3)) = 0.06
        _FlowSpeed ("Flow Speed (rings per second)", Float) = 0.5

        [Header(Lines)]
        _LineEvery ("A Line Every N Sides", Range(1, 16)) = 4
        _LineWidth ("Line Thickness", Range(0.005, 0.2)) = 0.04

        [Header(Light)]
        _PulseSpeed ("Pulse Speed", Float) = 0.4
        _Falloff ("Fade To Black (higher = darker sooner)", Range(0, 1.5)) = 0.25
        _Brightness ("Brightness", Range(0, 3)) = 1

        [Header(Render)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull (Front = see the inside)", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _WallColor;
                float4 _RingColor;
                float4 _LineColor;
                float4 _CoreColor;
                float _RingSpacing;
                float _RingWidth;
                float _FlowSpeed;
                float _LineEvery;
                float _LineWidth;
                float _PulseSpeed;
                float _Falloff;
                float _Brightness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float2 uv1        : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 cap        : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.cap = IN.uv1;
                return OUT;
            }

            // a thin line: d = distance to the line, halfWidth = half its thickness (1 on the line, 0 away, soft pixel edge)
            float Thin(float d, float halfWidth)
            {
                float aa = max(fwidth(d) * 1.5, 0.0001);
                return saturate((halfWidth - d) / aa + 0.5);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y;
                float depth = IN.uv.y;
                float fade = exp(-depth * _Falloff);                       // 1 at the mouth, closer to 0 far away

                // ---------------- THE WALLS ----------------
                // rings: distance (in world units) to the nearest ring, and the rings slide toward the far end
                float ringPos = depth / _RingSpacing - t * _FlowSpeed;
                float ringD = abs(frac(ringPos + 0.5) - 0.5) * _RingSpacing;
                float ring = Thin(ringD, _RingWidth * 0.5);
                float ringGlow = exp(-ringD / (_RingWidth * 3.0));

                // lines along the tube (a line every N sides)
                float lineD = abs(frac(IN.uv.x / _LineEvery + 0.5) - 0.5);
                float lines = Thin(lineD, _LineWidth * 0.5);

                // a pulse of light that travels inward along the tube
                float phase = frac(depth / (_RingSpacing * 8.0) - t * _PulseSpeed);
                float pulse = smoothstep(0.0, 0.04, phase) * (1.0 - smoothstep(0.04, 0.2, phase));

                float3 wall = _WallColor.rgb;
                wall += _RingColor.rgb * (ring + ringGlow * 0.3) * (0.6 + pulse) * fade;
                wall += _LineColor.rgb * lines * (0.5 + pulse * 1.5) * fade;

                // ---------------- THE FAR END (cap) ----------------
                float radial = IN.cap.x;                                   // 0 in the middle, 1 on the border
                float core = exp(-radial * radial * 6.0);
                float rr = abs(frac(radial * 3.0 - t * _FlowSpeed * 0.3 + 0.5) - 0.5);
                float capRings = Thin(rr, 0.04) * radial;

                float3 cap = _WallColor.rgb + _CoreColor.rgb * core * 1.2 + _RingColor.rgb * capRings * 0.6 * fade;

                // IN.cap.y is 1 on the cap and 0 on the walls
                float3 color = lerp(wall, cap, IN.cap.y);
                return float4(color * _Brightness, 1.0);
            }
            ENDHLSL
        }
    }
}
