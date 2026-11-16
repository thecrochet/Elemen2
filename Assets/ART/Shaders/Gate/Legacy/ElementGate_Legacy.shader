// ElementGate: the shader of the element gates (Fire, Earth, Water, Air, Aether, Base).
// ONE shader, six patterns: pick the element with "Element". The ElementGate script does it for you
// (it picks the material named M_Gate_<Element>).
//
// The gate is a tall see-through capsule that the ball runs THROUGH. Put it on a Quad that faces the camera.
// You can scale the Quad as you like (for example 1.5 x 3.4): the shader reads the scale, so the shape never stretches.
//
//   Fire    flame chevrons rising upward
//   Earth   cracked rock facets, with light pulsing in the cracks
//   Water   ripples spreading out from the middle
//   Air     a slow spiral
//   Aether  a dark void with small lights orbiting it (the rare, final-level power)
//   Base    a plain ring with ticks (electricity)
//
// There are no particles in here: only a little motion drawn by the shader.
// "Flash" is raised for a moment by the ElementGate script when the ball crosses the gate.
Shader "Elemento/Gate/ElementGate"
{
    Properties
    {
        [KeywordEnum(Fire, Earth, Water, Air, Aether, Base)] _Element ("Element", Float) = 0

        [Header(Colors)]
        [HDR] _ColorA ("Main Color", Color) = (2.5, 1.1, 0.25, 1)
        [HDR] _ColorB ("Accent Color", Color) = (4, 2.5, 0.8, 1)
        [HDR] _FillColor ("Inside Tint", Color) = (0.2, 0.06, 0.0, 1)
        _FillAlpha ("Inside Opacity", Range(0, 1)) = 0.18

        [Header(Shape)]
        _Width ("Width (fraction of the Quad)", Range(0.4, 0.95)) = 0.8
        _FrameWidth ("Frame Thickness", Range(0.01, 0.12)) = 0.04

        [Header(Motion)]
        _Speed ("Pattern Speed", Float) = 1
        _Scale ("Pattern Scale", Range(0.5, 4)) = 1

        [Header(Output)]
        _Flash ("Flash (set by the script)", Range(0, 3)) = 0
        _Opacity ("Opacity", Range(0, 1)) = 1
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
            #pragma shader_feature_local_fragment _ELEMENT_FIRE _ELEMENT_EARTH _ELEMENT_WATER _ELEMENT_AIR _ELEMENT_AETHER _ELEMENT_BASE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define GATE_TAU 6.28318531

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float4 _FillColor;
                float _FillAlpha;
                float _Width;
                float _FrameWidth;
                float _Speed;
                float _Scale;
                float _Flash;
                float _Opacity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 p          : TEXCOORD0;   // -1 .. +1 in height; the width runs -aspect .. +aspect
                float  aspect     : TEXCOORD1;   // width / height of the Quad in the world
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);

                // the size of the Quad in the world: the shape follows it, so a tall Quad gives a tall gate (no stretching)
                float sx = length(float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20));
                float sy = length(float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21));
                float aspect = sx / max(sy, 0.0001);

                OUT.aspect = aspect;
                OUT.p = float2((IN.uv.x * 2.0 - 1.0) * aspect, IN.uv.y * 2.0 - 1.0);
                return OUT;
            }

            // ---------------- drawing tools ----------------
            float Stroke(float d, float halfWidth)
            {
                float aa = clamp(fwidth(d) * 1.5, 0.0002, max(halfWidth * 3.0, 0.01));
                return saturate((halfWidth - abs(d)) / aa + 0.5);
            }

            float Soft(float d, float width)
            {
                return exp(-abs(d) / max(width, 0.0001));
            }

            float2 hash22(float2 p)
            {
                float3 q = frac(float3(p.x, p.y, p.x) * float3(0.1031, 0.1030, 0.0973));
                q += dot(q, q.yzx + 33.33);
                return frac((q.xx + q.yz) * q.zy);
            }

            // cracked rock: "edge" is small on the border between two cells (a crack), "id" says which cell
            void Cells(float2 p, out float edge, out float2 id)
            {
                float2 cell = floor(p);
                float2 local = frac(p);
                float best = 8.0;
                float second = 8.0;
                id = float2(0, 0);

                [unroll]
                for (int y = -1; y <= 1; y++)
                {
                    [unroll]
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float2 toPoint = offset + hash22(cell + offset) - local;
                        float dd = dot(toPoint, toPoint);

                        if (dd < best)
                        {
                            second = best;
                            best = dd;
                            id = cell + offset;
                        }
                        else if (dd < second)
                        {
                            second = dd;
                        }
                    }
                }

                edge = sqrt(second) - sqrt(best);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 p = IN.p;
                float t = _Time.y * _Speed;

                // ---------------- THE SHAPE: a tall capsule. d = distance to its border (negative inside) ----------------
                float hw = min(_Width * IN.aspect, 0.9);                // half width
                float h = max(0.95 - hw, 0.0);                          // half length of the straight part
                float d = length(p - float2(0.0, clamp(p.y, -h, h))) - hw;

                float aa = max(fwidth(d) * 1.5, 0.0001);
                float inside = saturate(-d / aa);
                float depth = saturate(-d / hw);                       // 0 on the border, 1 in the middle

                // the pattern is drawn in a space that is not stretched by the Quad, with the middle of the gate at (0,0)
                float2 q = p * _Scale;

                float4 res = float4(0, 0, 0, 0);
                float3 glow = float3(0, 0, 0);
                float fillAlpha = _FillAlpha;

                #if defined(_ELEMENT_EARTH)
                    // cracked facets: the cracks glow, each cell pulses on its own, slowly (earth is heavy)
                    float edge;
                    float2 id;
                    Cells(q * 1.6, edge, id);
                    float crack = 1.0 - smoothstep(0.0, 0.14, edge);
                    float pulse = 0.45 + 0.55 * sin(t * 0.9 + hash22(id).x * 6.2831);
                    glow += _ColorA.rgb * crack * pulse * 0.9 * inside;
                    glow += _ColorB.rgb * (1.0 - smoothstep(0.0, 0.04, edge)) * pulse * 0.5 * inside;
                #elif defined(_ELEMENT_WATER)
                    // ripples spreading out from the middle (a slightly squashed circle, to follow the capsule)
                    float r = length(q * float2(1.0, 0.45));
                    float ring = Stroke(frac(r * 2.2 - t * 0.35) - 0.5, 0.09);
                    glow += _ColorA.rgb * ring * (1.0 - depth * 0.3) * inside;
                    glow += _ColorB.rgb * Soft(r - frac(t * 0.17) * 2.0, 0.08) * 0.35 * inside;
                #elif defined(_ELEMENT_AIR)
                    // a slow spiral with two arms
                    float2 s = q * float2(1.0, 0.55);
                    float a = atan2(s.y, s.x) / GATE_TAU;
                    float arms = Stroke(frac(a * 2.0 + length(s) * 1.6 - t * 0.25) - 0.5, 0.07);
                    glow += _ColorA.rgb * arms * inside;
                    glow += _ColorB.rgb * Soft(length(s), 0.12) * 0.25 * inside;
                #elif defined(_ELEMENT_AETHER)
                    // a dark void with 5 small lights circling it, each on its own path and speed
                    fillAlpha = max(_FillAlpha, 0.9);

                    [unroll]
                    for (int k = 0; k < 5; k++)
                    {
                        float fk = (float)k;
                        float ang = t * (0.5 + fk * 0.17) * (fmod(fk, 2.0) < 0.5 ? 1.0 : -1.0) + fk * 1.9;
                        float2 orbit = float2(cos(ang) * hw * (0.35 + 0.08 * fk), sin(ang) * (0.35 + 0.17 * fk) * 0.9);
                        float od = length(p - orbit);
                        glow += _ColorB.rgb * ((1.0 - smoothstep(0.0, 0.035, od)) + Soft(od, 0.05) * 0.35) * inside;
                    }

                    glow += _ColorA.rgb * Soft(length(p * float2(1.0, 0.5)), 0.12) * 0.12 * inside;
                #elif defined(_ELEMENT_BASE)
                    // a plain ring with ticks around it (electricity)
                    float tickTurn = atan2(p.y * 0.55, p.x) / GATE_TAU;
                    float tickOn = step(0.5, frac(tickTurn * 24.0 + t * 0.1));
                    glow += _ColorA.rgb * Stroke(d + 0.09, 0.012) * tickOn;
                #else
                    // FIRE: flame chevrons climbing upward, fading toward the top
                    float w = q.y * 2.2 + abs(q.x) * 2.6 - t * 0.8 + sin(q.y * 5.0 + t * 3.0) * 0.06;
                    float chevron = Stroke(frac(w) - 0.5, 0.1);
                    float climb = lerp(1.0, 0.25, saturate(p.y * 0.5 + 0.5));
                    glow += _ColorA.rgb * chevron * climb * inside;
                    glow += _ColorB.rgb * Stroke(frac(w) - 0.5, 0.035) * climb * 0.8 * inside;
                #endif

                // ---------------- BODY ----------------
                res = float4(_FillColor.rgb * fillAlpha * inside, fillAlpha * inside);

                // ---------------- FRAME ----------------
                float frame = Stroke(d, _FrameWidth * 0.5);
                float halo = Soft(max(d, 0.0), 0.1) * 0.3;
                res.rgb += _ColorA.rgb * (frame * 1.3 + halo);
                res.rgb += glow;

                // ---------------- FLASH: a brief brightening when the ball crosses ----------------
                res.rgb *= 1.0 + _Flash * 1.5;
                res.rgb += _ColorB.rgb * _Flash * (frame * 0.8 + halo * 0.5);

                return res * _Opacity;
            }
            ENDHLSL
        }
    }
}
