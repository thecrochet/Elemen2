// Portal_NeonArch: a Tron-style arch: a double neon frame, a light pulse that climbs the sides,
// and a see-through energy veil with scan lines. The bottom can be left open (it sits on the floor).
// Use it on a Quad. Keep the Quad SQUARE (scale X = scale Y) and change the shape with Half Width / Half Height.
// Needs PortalCommon.hlsl in the same folder.
Shader "Elemento/Portal/NeonArch"
{
    Properties
    {
        [Header(Frame)]
        [HDR] _EdgeColor ("Frame Color", Color) = (0, 2.5, 3, 1)
        [HDR] _PulseColor ("Pulse Color", Color) = (3, 5, 6, 1)
        _HalfWidth ("Half Width", Range(0.2, 0.9)) = 0.55
        _HalfHeight ("Half Height", Range(0.3, 0.95)) = 0.9
        _FrameWidth ("Frame Thickness", Range(0.01, 0.1)) = 0.035
        _InnerGap ("Gap To Inner Line", Range(0.02, 0.2)) = 0.07
        [Toggle] _OpenBottom ("Open Bottom", Float) = 1

        [Header(Veil)]
        [HDR] _FillColor ("Veil Color", Color) = (0, 0.15, 0.2, 1)
        _FillAlpha ("Veil Opacity", Range(0, 1)) = 0.25
        _ScanLines ("Scan Line Count", Range(10, 120)) = 40
        _PulseSpeed ("Pulse Speed", Float) = 0.6

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
                float4 _PulseColor;
                float _HalfWidth;
                float _HalfHeight;
                float _FrameWidth;
                float _InnerGap;
                float _OpenBottom;
                float4 _FillColor;
                float _FillAlpha;
                float _ScanLines;
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

                float d = ArchSDF(p, _HalfWidth, _HalfHeight);   // negative inside the arch
                float aa = max(fwidth(d) * 1.5, 0.0001);

                // an open bottom: the lines fade out near the floor
                float openMask = lerp(1.0, smoothstep(-_HalfHeight + 0.03, -_HalfHeight + 0.09, p.y), _OpenBottom);

                // a bright band of light that climbs the arch and meets at the top
                float phase = frac((p.y + _HalfHeight) * 0.4 - t * _PulseSpeed);
                float pulse = smoothstep(0.0, 0.06, phase) * (1.0 - smoothstep(0.06, 0.25, phase));

                float4 res = float4(0, 0, 0, 0);

                // ---------------- THE VEIL ----------------
                float inside = saturate(-d / aa) * openMask;
                float scan = 0.5 + 0.5 * sin(pBack.y * _ScanLines - t * 2.0);
                float veil = fbm2(pBack * 3.0 + float2(0.0, -t * 0.3));
                float edgeGlow = Soft(d, 0.12) * saturate(-d / aa);

                res = Over(res, _FillColor.rgb, _FillAlpha * inside);
                res = AddGlow(res, _EdgeColor.rgb * (scan * 0.1 + veil * 0.12 + edgeGlow * 0.4) * inside);

                // ---------------- THE FRAME ----------------
                float3 frameColor = lerp(_EdgeColor.rgb, _PulseColor.rgb, pulse);
                float outer = Stroke(d, _FrameWidth * 0.5);
                float innerLine = Stroke(ArchSDF(pFront, _HalfWidth, _HalfHeight) + _InnerGap, 0.006);
                float halo = Soft(max(d, 0.0), 0.1) * 0.3;

                res = AddGlow(res, frameColor * (outer * (1.0 + pulse) + innerLine * 0.6 + halo) * openMask);

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
