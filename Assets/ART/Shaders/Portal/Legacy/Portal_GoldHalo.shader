// Portal_GoldHalo: a bright golden gate of turning rings, a spiral and drifting sparks around a glowing core.
// Looks like the golden portal at the end of the path. Good for the FINISH of a level (it reads as "go here").
// Use it on a Quad. Needs PortalLegacyCommon.hlsl in the same folder.
Shader "Elemento/Portal/GoldHalo"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _GoldColor ("Ring Color", Color) = (3, 1.8, 0.3, 1)
        [HDR] _CoreColor ("Core Color", Color) = (4, 3.2, 1.2, 1)
        [HDR] _HaloColor ("Spiral and Halo Color", Color) = (1.6, 0.9, 0.15, 1)
        [HDR] _FillColor ("Inside Tint", Color) = (0.15, 0.06, 0, 1)
        _FillAlpha ("Inside Opacity", Range(0, 1)) = 0.2

        [Header(Shape)]
        _Radius ("Main Ring Radius", Range(0.2, 0.8)) = 0.5
        [IntRange] _Arms ("Spiral Arms", Range(1, 6)) = 3
        _SpiralTightness ("Spiral Tightness", Range(0, 12)) = 6

        [Header(Motion)]
        _SpinSpeed ("Spin Speed", Float) = 0.5
        _PulseSpeed ("Pulse Speed", Float) = 1.2
        _SparkScale ("Spark Density", Range(4, 30)) = 10

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
                float4 _GoldColor;
                float4 _CoreColor;
                float4 _HaloColor;
                float4 _FillColor;
                float _FillAlpha;
                float _Radius;
                float _Arms;
                float _SpiralTightness;
                float _SpinSpeed;
                float _PulseSpeed;
                float _SparkScale;
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
                float rFront = length(pFront);
                float turnsFront = atan2(pFront.y, pFront.x) / PORTAL_TAU + 0.5;
                float t = _Time.y;
                float r = length(p);
                float turns = atan2(p.y, p.x) / PORTAL_TAU + 0.5;
                float aaR = max(fwidth(r) * 1.5, 0.0001);
                float pulse = 0.8 + 0.2 * sin(t * _PulseSpeed);

                float4 res = float4(0, 0, 0, 0);

                // inside tint (so the portal stays readable on a bright background)
                float inside = saturate((_Radius - r) / aaR);
                res = Over(res, _FillColor.rgb, _FillAlpha * inside);

                // the spiral: arms that turn slowly around the middle (the arm count must be a whole number)
                float spiral = frac(turnsBack * _Arms - log(rBack + 0.05) * _SpiralTightness - t * _SpinSpeed);
                float arm = smoothstep(0.0, 0.5, spiral) * (1.0 - smoothstep(0.5, 1.0, spiral));
                float armMask = (1.0 - smoothstep(_Radius * 0.4, _Radius, rBack)) * smoothstep(0.03, 0.2, rBack);
                res = AddGlow(res, _HaloColor.rgb * arm * armMask * 0.5);

                // the core
                float core = Soft(rBack, 0.18) * 0.6 + (1.0 - smoothstep(0.0, 0.08, rBack));
                res = AddGlow(res, _CoreColor.rgb * core * pulse);

                // the main ring
                res = AddGlow(res, _GoldColor.rgb * (Stroke(r - _Radius, 0.016) + Soft(r - _Radius, 0.07) * 0.35) * pulse);

                // a dashed ring inside that turns one way
                float dashes = step(frac(turnsBack * 14.0 + t * _SpinSpeed), 0.5);
                res = AddGlow(res, _HaloColor.rgb * Stroke(rBack - _Radius * 0.62, 0.008) * dashes);

                // long arcs outside that turn the other way
                float arcs = step(frac(turnsFront * 3.0 - t * _SpinSpeed * 0.7), 0.55);
                res = AddGlow(res, _GoldColor.rgb * Stroke(rFront - _Radius * 1.25, 0.008) * arcs * 0.8);

                // a wide soft halo and sparks flying around
                res = AddGlow(res, _HaloColor.rgb * Soft(r - _Radius, 0.25) * 0.15);
                float sparks = Sparks(Rot2(pFront, t * 0.3), _SparkScale, t * 0.4) * (1.0 - smoothstep(0.3, 1.0, r));
                res = AddGlow(res, _CoreColor.rgb * sparks * 1.5);

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
