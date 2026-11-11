// Portal_HexGate: a hexagon gate: glowing frame, a node on each corner, a slowly spinning inner hexagon,
// and a grid of hexagon cells that lights up near the edges. Hexagon = "special / bounce" in your shape language.
// Use it on a Quad. Needs PortalCommon.hlsl in the same folder.
Shader "Elemento/Portal/HexGate"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _EdgeColor ("Frame Color", Color) = (2.6, 0.2, 2, 1)
        [HDR] _NodeColor ("Corner Node Color", Color) = (5, 2, 4.5, 1)
        [HDR] _FillColor ("Inside Tint", Color) = (0.12, 0.01, 0.1, 1)
        _FillAlpha ("Inside Opacity", Range(0, 1)) = 0.25

        [Header(Shape)]
        _Radius ("Hexagon Size", Range(0.3, 0.95)) = 0.75
        _FrameWidth ("Frame Thickness", Range(0.01, 0.1)) = 0.03
        _HexScale ("Cell Density", Range(2, 14)) = 6

        [Header(Motion)]
        _InnerSpin ("Inner Hexagon Spin", Float) = 0.3
        _PulseSpeed ("Pulse Speed", Float) = 1.3

        [Header(Depth)]
        _DepthBack ("Depth: Inner Layers Behind", Range(0, 0.6)) = 0.15
        _DepthFront ("Depth: Outer Layers In Front", Range(0, 0.4)) = 0.08

        [Header(Tunnel)]
        [IntRange] _Slices ("Tunnel: Depth Slices (1 = off)", Range(1, 8)) = 4
        _TunnelDepth ("Tunnel: How Deep", Range(0, 1.5)) = 0.5

        [Header(Cube)]
        [Toggle] _FrontOnly ("Portal On Front Face Only", Float) = 0
        [HDR] _SideColor ("Other Faces Color", Color) = (0, 0.8, 1.2, 1)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull (Off = Quad, Back = Cube)", Float) = 0

        [Header(Output)]
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
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "PortalCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor;
                float4 _NodeColor;
                float4 _FillColor;
                float _FillAlpha;
                float _Radius;
                float _FrameWidth;
                float _HexScale;
                float _InnerSpin;
                float _PulseSpeed;
                float _Opacity;
                float _DepthBack;
                float _DepthFront;
                float _Slices;
                float _TunnelDepth;
                float _FrontOnly;
                float4 _SideColor;
            CBUFFER_END

            // the portal drawn ONCE at the position pPix (the tunnel below calls it several times)
            float4 PortalPixel(Varyings IN, float2 pPix)
            {
                float2 p = pPix;
                float2 pBack = PortalBack(p, IN.viewShift, _DepthBack);
                float2 pFront = PortalFront(p, IN.viewShift, _DepthFront);
                float t = _Time.y;

                // distance to the frame (negative inside). A hexagon with corners at _Radius has HexDist = _Radius * 0.866 on its border
                float apothem = _Radius * 0.8660254;
                float d = HexDist(p) - apothem;
                float aa = max(fwidth(d) * 1.5, 0.0001);
                float inside = saturate(-d / aa);

                float4 res = float4(0, 0, 0, 0);

                // ---------------- INSIDE ----------------
                res = Over(res, _FillColor.rgb, _FillAlpha * inside);

                // a grid of hexagons (turned 30 degrees so it matches the frame): cells near the edge are brighter, a few light up
                float2 gv;
                float2 id;
                HexCell(Rot2(pBack, PORTAL_PI / 6.0) * _HexScale + float2(0.0, t * 0.2), gv, id);
                float h = hash21(id);
                float nearEdge = 1.0 - saturate(-d / (_Radius * 0.8));
                float lit = step(0.7, h) * (0.5 + 0.5 * sin(t * _PulseSpeed + h * 30.0));
                float cellLine = Stroke(CellEdge(gv) - 0.44, 0.025);
                res = AddGlow(res, _EdgeColor.rgb * cellLine * (0.1 * nearEdge + lit * 0.9) * inside);

                // a small hexagon in the middle that turns slowly
                float innerHex = HexDist(Rot2(pBack, t * _InnerSpin)) - apothem * 0.3;
                res = AddGlow(res, _EdgeColor.rgb * (Stroke(innerHex, 0.012) + Soft(innerHex, 0.05) * 0.2));

                // ---------------- FRAME ----------------
                float frame = Stroke(d, _FrameWidth * 0.5);
                float inset = Stroke(d + 0.07, 0.006);
                float halo = Soft(max(d, 0.0), 0.1) * 0.3;
                res = AddGlow(res, _EdgeColor.rgb * (frame * 1.2 + inset * 0.6 + halo));

                // a glowing node on each of the 6 corners
                float nodes = 0.0;

                [unroll]
                for (int k = 0; k < 6; k++)
                {
                    float angle = k * PORTAL_PI / 3.0;
                    float nd = length(pFront - _Radius * float2(cos(angle), sin(angle)));
                    float blink = 0.7 + 0.3 * sin(t * _PulseSpeed * 1.5 + k);
                    nodes += ((1.0 - smoothstep(0.0, 0.06, nd)) + Soft(nd, 0.08) * 0.4) * blink;
                }

                res = AddGlow(res, _NodeColor.rgb * nodes);

                return res;
            }

            // ---------------------------------------------------------------
            // THE REAL FRAGMENT SHADER: the portal drawn several times, one copy deeper than the last (a tunnel)
            // ---------------------------------------------------------------
            float4 frag(Varyings IN) : SV_Target
            {
                // slice 0 = the portal itself, at the surface
                float4 res = PortalPixel(IN, IN.p);

                // the slices behind it: each one slides with the camera (parallax), is smaller, and is darker
                int count = (int)_Slices;

                [loop]
                for (int i = 1; i < count; i++)
                {
                    float k = (float)i / (float)(count - 1);                              // 0 to 1: how deep this slice is
                    float depth = k * _TunnelDepth;
                    float2 q = (IN.p - IN.viewShift * depth) * (1.0 + depth * 0.6);   // slide, then shrink toward the middle
                    float4 layer = PortalPixel(IN, q) * ((1.0 - k) * (1.0 - k));        // the deeper, the darker

                    // paint it BEHIND what is already there (it only shows through the see-through parts)
                    res = res + layer * (1.0 - res.a);
                }

                // the other faces of a cube: dark with a glowing border (or the portal again, if the option is off)
                float4 side = PortalSideFace(IN.p, _SideColor.rgb);
                float isSide = step(0.5, _FrontOnly) * step(-0.5, IN.faceZ);
                res = lerp(res, side, isSide);

                return res * _Opacity;
            }
            ENDHLSL
        }
    }
}
