// Portal_Vortex: a ring of whirling flames (energy fire) around a see-through center with floating hexagons.
// Looks like the cyan start portal. Good for the START of a level.
// Use it on a Quad. Needs PortalCommon.hlsl in the same folder.
Shader "Elemento/Portal/Vortex"
{
    Properties
    {
        [Header(Flames)]
        [HDR] _FlameColor ("Flame Color", Color) = (0.2, 1.6, 2.4, 1)
        [HDR] _CoreColor ("Hot Ring Color", Color) = (2, 4, 5, 1)
        _Radius ("Ring Radius", Range(0.2, 0.8)) = 0.55
        _FlameHeight ("Flame Height", Range(0.05, 0.5)) = 0.3
        _FlameSpeed ("Flame Speed", Float) = 1
        _Turbulence ("Wobble Amount", Range(0, 0.6)) = 0.25

        [Header(Inside)]
        [HDR] _InteriorColor ("Inside Tint", Color) = (0.02, 0.12, 0.18, 1)
        _InteriorAlpha ("Inside Opacity", Range(0, 1)) = 0.25
        [HDR] _HexColor ("Hexagon Color", Color) = (0.4, 2.4, 3, 1)
        _HexScale ("Hexagon Density", Range(2, 12)) = 5

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
                float4 _FlameColor;
                float4 _CoreColor;
                float _Radius;
                float _FlameHeight;
                float _FlameSpeed;
                float _Turbulence;
                float4 _InteriorColor;
                float _InteriorAlpha;
                float4 _HexColor;
                float _HexScale;
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
                float r = length(p);
                float turns = atan2(p.y, p.x) / PORTAL_TAU + 0.5;
                float aaR = max(fwidth(r) * 1.5, 0.0001);

                float4 res = float4(0, 0, 0, 0);

                // ---------------- THE INSIDE ----------------
                float inside = saturate((_Radius - r) / aaR);
                res = Over(res, _InteriorColor.rgb, _InteriorAlpha * inside);

                // floating hexagons: they are always faint, and some of them light up in turns
                float2 gv;
                float2 id;
                HexCell(pBack * _HexScale + float2(0.0, t * 0.3), gv, id);
                float h = hash21(id);
                float lit = step(0.8, h) * (0.5 + 0.5 * sin(t * 1.5 + h * 20.0));
                float hexLine = Stroke(CellEdge(gv) - 0.42, 0.03);
                float fadeToRing = saturate(1.2 - r / _Radius);
                res = AddGlow(res, _HexColor.rgb * hexLine * (0.12 + lit * 1.4) * fadeToRing * inside);

                // ---------------- THE FLAMES ----------------
                // we bend the space a little with noise, so the ring wobbles like a flame
                float2 warp = float2(fbm2(p * 3.0 + float2(0.0, -t * 0.8)),
                                     fbm2(p * 3.0 + float2(5.2, 1.3) - t * 0.5)) - 0.5;
                float2 q = p + warp * _Turbulence;
                float rq = length(q);
                float d = rq - _Radius;                                   // negative inside the ring, positive outside

                // tongues of flame that rise outward and drift around the ring
                float2 dirQ = q / max(rq, 0.001);
                float tongue = fbm2(dirQ * 2.2 + float2(1.0, 0.6) * (rq * 6.0 - t * _FlameSpeed));
                float outward = saturate(d / _FlameHeight);               // 0 on the ring, 1 at the tip of the flame
                float flame = (1.0 - outward) * smoothstep(0.25, 0.8, tongue + (1.0 - outward) * 0.5) * step(-0.04, d);

                float ringLine = Stroke(d, 0.018);
                float glow = Soft(d, 0.06);

                res = AddGlow(res, _FlameColor.rgb * (flame * 1.2 + glow * 0.5));
                res = AddGlow(res, _CoreColor.rgb * ringLine * (0.8 + 0.4 * tongue));

                // embers that fly up from the flames
                float embers = Sparks(Rot2(pFront, t * 0.2), 12.0, t * 0.5) * Soft(max(d, 0.0), 0.2) * step(0.0, d);
                res = AddGlow(res, _CoreColor.rgb * embers * 1.5);

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
