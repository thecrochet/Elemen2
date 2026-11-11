// Portal_CrystalRing: a dark swirling void inside a ring of spiked glowing crystals, with runes.
// Looks like the red crystal portal. Change the colors for another world (red = fire, cyan = water...).
// Use it on a Quad. Needs PortalCommon.hlsl in the same folder.
Shader "Elemento/Portal/CrystalRing"
{
    Properties
    {
        [Header(Crystals)]
        [HDR] _CrystalColor ("Crystal Body", Color) = (0.55, 0.02, 0.03, 1)
        [HDR] _CrystalLight ("Crystal Light Edges", Color) = (3, 0.4, 0.25, 1)
        [HDR] _RuneColor ("Rune Color", Color) = (4, 1.3, 0.5, 1)
        [IntRange] _SpikeCount ("Big Crystals", Range(6, 24)) = 14
        _SpikeHeight ("Big Crystal Height", Range(0.05, 0.4)) = 0.26
        [IntRange] _DetailCount ("Small Crystals", Range(12, 60)) = 36
        _DetailHeight ("Small Crystal Height", Range(0.02, 0.25)) = 0.12

        [Header(Void and rim)]
        _InnerRadius ("Void Radius", Range(0.2, 0.8)) = 0.55
        [HDR] _VoidColor ("Void Color", Color) = (0.03, 0, 0.005, 1)
        [HDR] _SwirlColor ("Swirl Color", Color) = (2.2, 0.3, 0.1, 1)
        [HDR] _RimColor ("Rim Glow Color", Color) = (4, 1.4, 0.5, 1)
        _SwirlSpeed ("Swirl Speed", Float) = 0.4
        _SwirlAmount ("Swirl Twist", Range(0, 8)) = 3
        _SparkScale ("Spark Density", Range(4, 30)) = 14
        _PulseSpeed ("Pulse Speed", Float) = 1.5

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
                float4 _CrystalColor;
                float4 _CrystalLight;
                float4 _RuneColor;
                float _SpikeCount;
                float _SpikeHeight;
                float _DetailCount;
                float _DetailHeight;
                float _InnerRadius;
                float4 _VoidColor;
                float4 _SwirlColor;
                float4 _RimColor;
                float _SwirlSpeed;
                float _SwirlAmount;
                float _SparkScale;
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
                float r = length(p);
                float turns = atan2(p.y, p.x) / PORTAL_TAU + 0.5;   // 0 to 1 around the ring
                float aaR = max(fwidth(r) * 1.5, 0.0001);

                float4 res = float4(0, 0, 0, 0);

                // ---------------- THE VOID (inside) ----------------
                // the closer to the middle, the less the swirl turns: it looks like a whirlpool
                float2 swirlP = Rot2(pBack, _SwirlAmount * (1.0 - r / _InnerRadius) + t * _SwirlSpeed);
                float wisps = smoothstep(0.5, 0.85, fbm2(swirlP * 3.0 + t * 0.1));
                float sparks = Sparks(swirlP, _SparkScale, t * 0.3);
                float voidMask = saturate((_InnerRadius - r) / aaR);

                res = Over(res, _VoidColor.rgb, voidMask * 0.97);
                res = AddGlow(res, (_SwirlColor.rgb * wisps * 0.7 + _RimColor.rgb * sparks) * voidMask);

                // ---------------- THE GLOWING RIM ----------------
                float rimNoise = fbm2(p * 7.0 + float2(t * 0.5, -t * 0.8));
                float rim = Stroke(r - _InnerRadius, 0.012) * (0.7 + rimNoise);
                float rimGlow = Soft(r - _InnerRadius, 0.05) * (0.5 + rimNoise);
                res = AddGlow(res, _RimColor.rgb * (rim + rimGlow * 0.6));

                // ---------------- THE CRYSTALS ----------------
                // the outer edge of the ring is a "sawtooth": a radius that goes up and down around the circle
                float xa = turns * _SpikeCount;
                float ua = frac(xa);
                float tipA = 1.0 - abs(ua - 0.5) * 2.0;                  // 1 on the tip, 0 in the valley
                float varyA = 0.75 + 0.25 * hash21(float2(floor(xa), 3.1));
                float heightA = _SpikeHeight * varyA * pow(tipA, 1.3);

                float xb = turns * _DetailCount + 0.37;
                float ub = frac(xb);
                float tipB = 1.0 - abs(ub - 0.5) * 2.0;
                float varyB = 0.6 + 0.4 * hash21(float2(PosMod(floor(xb), _DetailCount), 1.7));
                float heightB = _DetailHeight * varyB * pow(tipB, 1.2);

                float baseR = _InnerRadius + 0.18;
                float outerR = baseR + max(heightA, heightB);
                float innerR = _InnerRadius + 0.03;

                float body = saturate((outerR - r) / aaR) * saturate((r - innerR) / aaR);
                float along = saturate((r - innerR) / max(outerR - innerR, 0.001));   // 0 at the base, 1 at the tip

                // each crystal has a bright side and a dark side, like cut gems
                bool bigOnTop = heightA >= heightB;
                float u = bigOnTop ? ua : ub;
                float seed = bigOnTop ? hash21(float2(floor(xa), 9.3)) : hash21(float2(PosMod(floor(xb), _DetailCount), 5.1));
                float facet = (u < 0.5 ? 1.0 : 0.35) * (0.6 + 0.4 * seed);

                float3 crystal = lerp(_CrystalColor.rgb, _CrystalLight.rgb, facet * pow(along, 1.4) * 0.45);
                res = Over(res, crystal, body);

                // light from the portal bleeds into the base of the crystals
                res = AddGlow(res, _RimColor.rgb * (1.0 - along) * 0.25 * body);

                // bright outline on the crystals and a ridge down the middle of the big ones
                float edgeLine = Stroke(r - outerR, 0.006);
                float ridge = Stroke(ua - 0.5, 0.012) * along * step(heightB, heightA);
                res = AddGlow(res, _CrystalLight.rgb * (edgeLine * 0.8 + ridge * 0.5) * body);

                // ---------------- THE RUNES ----------------
                // one glowing diamond on every big crystal (the space is turned so the crystal points to the right)
                float spikeAngle = ((floor(xa) + 0.5) / _SpikeCount - 0.5) * PORTAL_TAU;
                float2 q = Rot2(pFront, -spikeAngle) - float2(_InnerRadius + 0.14, 0.0);
                float diamond = abs(q.x) + abs(q.y);
                float rune = Stroke(diamond - 0.05, 0.009) + Stroke(diamond - 0.022, 0.007);
                float runePulse = 0.8 + 0.2 * sin(t * _PulseSpeed + floor(xa));
                res = AddGlow(res, _RuneColor.rgb * (rune + Soft(diamond - 0.05, 0.03) * 0.25) * runePulse * body);

                // a thin dashed rune ring between the void and the crystals
                float dashes = step(frac(turns * 48.0), 0.6);
                res = AddGlow(res, _RuneColor.rgb * Stroke(r - (_InnerRadius + 0.075), 0.004) * dashes * 0.8 * body);

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
