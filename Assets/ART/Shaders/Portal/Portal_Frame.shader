// Portal_Frame: THE portal shader (it replaces the six old ones).
// A glowing neon frame with a SEE-THROUGH middle, so the tunnel behind it (NeonPortalTunnel) shows through.
// Pick the shape with "Shape": Circle, Hexagon or Arch.
//
//   - frame line + a thin inner line + a soft halo
//   - a light that travels around the frame (the only animation; there are no idle particles)
//   - small nodes on the corners (hexagon) or on the four sides (circle)
//   - "Brightness": a script can raise it for a moment (checkpoint reached, level start...)
//
// Use it on a Quad that faces the camera (keep the Quad SQUARE).
// Needs PortalCommon.hlsl in the same folder.
Shader "Elemento/Portal/Frame"
{
    Properties
    {
        [Header(Shape)]
        [KeywordEnum(Circle, Hexagon, Arch)] _Shape ("Shape", Float) = 0
        _Radius ("Size (Circle / Hexagon)", Range(0.3, 0.95)) = 0.55
        _HalfWidth ("Arch: Half Width", Range(0.2, 0.9)) = 0.55
        _HalfHeight ("Arch: Half Height", Range(0.3, 0.95)) = 0.9
        [Toggle] _OpenBottom ("Arch: Open Bottom", Float) = 1

        [Header(Colors)]
        [HDR] _EdgeColor ("Frame Color", Color) = (0, 2.5, 3, 1)
        [HDR] _PulseColor ("Traveling Light Color", Color) = (3, 5, 6, 1)
        [HDR] _FillColor ("Inside Tint", Color) = (0, 0.1, 0.15, 1)
        _FillAlpha ("Inside Opacity (0 = fully see-through)", Range(0, 1)) = 0.06

        [Header(Lines)]
        _FrameWidth ("Frame Thickness", Range(0.01, 0.1)) = 0.035
        _InnerGap ("Gap To Inner Line", Range(0.02, 0.2)) = 0.07
        _NodeSize ("Node Size", Range(0, 0.12)) = 0.05

        [Header(Motion)]
        _PulseSpeed ("Traveling Light Speed", Float) = 0.35
        _PulseLength ("Traveling Light Tail", Range(1, 16)) = 6

        [Header(Output)]
        _Brightness ("Brightness (scripts raise it for a moment)", Range(0, 4)) = 1
        _Opacity ("Opacity (fade in / out)", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _SHAPE_CIRCLE _SHAPE_HEXAGON _SHAPE_ARCH
            #include "PortalCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor;
                float4 _PulseColor;
                float4 _FillColor;
                float _FillAlpha;
                float _Radius;
                float _HalfWidth;
                float _HalfHeight;
                float _OpenBottom;
                float _FrameWidth;
                float _InnerGap;
                float _NodeSize;
                float _PulseSpeed;
                float _PulseLength;
                float _Brightness;
                float _Opacity;
            CBUFFER_END

            float4 frag(Varyings IN) : SV_Target
            {
                float2 p = IN.p;
                float t = _Time.y;

                // ---------------- THE SHAPE: d = distance to the frame (negative inside) ----------------
                float d;
                float nodeCount;     // how many nodes sit on the frame
                float nodeRadius;    // and how far from the center

                #if defined(_SHAPE_HEXAGON)
                    d = HexDist(p) - _Radius * 0.8660254;
                    nodeCount = 6.0;
                    nodeRadius = _Radius;
                #elif defined(_SHAPE_ARCH)
                    d = ArchSDF(p, _HalfWidth, _HalfHeight);
                    nodeCount = 0.0;
                    nodeRadius = 0.0;
                #else
                    d = length(p) - _Radius;
                    nodeCount = 4.0;
                    nodeRadius = _Radius;
                #endif

                float aa = max(fwidth(d) * 1.5, 0.0001);
                float inside = saturate(-d / aa);

                // an open arch has no line on the floor
                float frameMask = 1.0;
                #if defined(_SHAPE_ARCH)
                    float floorY = -_HalfHeight + _FrameWidth * 0.5 + 0.01;
                    frameMask = lerp(1.0, saturate((p.y - floorY) / 0.01), _OpenBottom);
                #endif

                float4 res = float4(0, 0, 0, 0);

                // ---------------- INSIDE (almost nothing: the tunnel shows through) ----------------
                res = Over(res, _FillColor.rgb, _FillAlpha * inside);

                // ---------------- FRAME ----------------
                float frame = Stroke(d, _FrameWidth * 0.5) * frameMask;
                float inset = Stroke(d + _InnerGap, 0.006) * frameMask;
                float halo = Soft(max(d, 0.0), 0.1) * 0.3 * frameMask;
                res = AddGlow(res, _EdgeColor.rgb * (frame * 1.2 + inset * 0.6 + halo));

                // ---------------- THE TRAVELING LIGHT ----------------
                // "turn" goes 0 to 1 around the center; the bright head moves, the tail fades behind it
                float turn = atan2(p.y, p.x) / PORTAL_TAU;
                float behind = frac(turn - t * _PulseSpeed);
                float comet = pow(1.0 - behind, _PulseLength);
                res = AddGlow(res, _PulseColor.rgb * comet * (frame + inset * 0.5));

                // ---------------- NODES ----------------
                float nodes = 0.0;

                [unroll]
                for (int k = 0; k < 6; k++)
                {
                    float on = step((float)k + 0.5, nodeCount);
                    float angle = (float)k * PORTAL_TAU / max(nodeCount, 1.0);
                    float nd = length(p - nodeRadius * float2(cos(angle), sin(angle)));
                    nodes += on * ((1.0 - smoothstep(0.0, _NodeSize, nd)) + Soft(nd, 0.08) * 0.3);
                }

                res = AddGlow(res, _PulseColor.rgb * nodes * 0.8);

                // ---------------- OUTPUT ----------------
                res.rgb *= _Brightness;
                return res * _Opacity;
            }
            ENDHLSL
        }
    }
}
