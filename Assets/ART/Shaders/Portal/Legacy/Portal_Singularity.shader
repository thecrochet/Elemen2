// Portal_Singularity: a pure black disc with a thin hot ring and a glowing disk that turns and pulls light inward.
// The most READABLE of the six (a black hole in a dark world pops out): good when the level is already busy.
// Use it on a Quad. Needs PortalLegacyCommon.hlsl in the same folder.
Shader "Elemento/Portal/Singularity"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _RingColor ("Ring Color", Color) = (2.5, 1.6, 0.6, 1)
        [HDR] _HotColor ("Hot Spot Color", Color) = (4, 3.5, 3, 1)
        [HDR] _DiskColor ("Disk Color", Color) = (1.2, 0.35, 0.08, 1)

        [Header(Shape)]
        _Radius ("Black Disc Radius", Range(0.1, 0.7)) = 0.42
        _RingWidth ("Ring Thickness", Range(0.005, 0.08)) = 0.025
        _DiskWidth ("Disk Reach", Range(0.05, 0.6)) = 0.28

        [Header(Motion)]
        _SpinSpeed ("Spin Speed", Float) = 0.7
        _InflowSpeed ("Inflow Speed", Float) = 0.8

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
            #include "PortalLegacyCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _RingColor;
                float4 _HotColor;
                float4 _DiskColor;
                float _Radius;
                float _RingWidth;
                float _DiskWidth;
                float _SpinSpeed;
                float _InflowSpeed;
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
                float rBack = length(pBack);
                float turnsBack = atan2(pBack.y, pBack.x) / PORTAL_TAU + 0.5;
                float t = _Time.y;
                float r = length(p);
                float angle = atan2(p.y, p.x);
                float turns = angle / PORTAL_TAU + 0.5;
                float aaR = max(fwidth(r) * 1.5, 0.0001);
                float d = r - _Radius;                              // negative inside the black disc

                float4 res = float4(0, 0, 0, 0);

                // ---------------- THE DISK OF LIGHT ----------------
                // it turns faster near the middle (the farther, the slower)
                float2 turned = Rot2(pBack, -t * _SpinSpeed / (1.0 + rBack * 4.0));
                float streaks = pow(fbm2(turned * 6.0), 2.0);
                float diskMask = step(0.0, d) * Soft(d, _DiskWidth * 0.4);
                res = AddGlow(res, _DiskColor.rgb * (0.25 + streaks * 2.0) * diskMask);

                // thin lines of light that fall into the hole (a phase that shrinks over time = moving inward)
                float cell = floor(turnsBack * 60.0);
                float h = hash21(float2(cell, 1.0));
                float across = frac(turnsBack * 60.0) - 0.5;
                float flow = frac(rBack * 2.5 + t * _InflowSpeed * (0.5 + h) + h * 7.0);
                float fall = step(0.7, h) * (1.0 - smoothstep(0.0, 0.1, abs(across))) * smoothstep(0.0, 0.2, flow) * (1.0 - smoothstep(0.5, 1.0, flow));
                res = AddGlow(res, _RingColor.rgb * fall * step(0.0, d) * Soft(d, 0.25) * 0.8);

                // ---------------- THE BLACK DISC ----------------
                res = Over(res, float3(0, 0, 0), saturate(-d / aaR));

                // ---------------- THE RING ----------------
                // one side of the ring is brighter, and that side travels around
                float bright = 0.55 + 0.45 * cos(angle - t * _SpinSpeed);
                float ring = Stroke(d - _RingWidth * 0.5, _RingWidth * 0.5);
                float ringGlow = Soft(d, 0.04);
                res = AddGlow(res, _RingColor.rgb * (ring + ringGlow * 0.4) * bright);
                res = AddGlow(res, _HotColor.rgb * ring * pow(bright, 4.0));

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
