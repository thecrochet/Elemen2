# Coding rules

## Style
- C#, one MonoBehaviour per file, class name = file name.
- Inspector-tunable fields are public or [SerializeField] with [Header] and [Tooltip].
- New Input System only. No legacy Input class.
- Editor-only code goes inside `#if UNITY_EDITOR`.
- Create-menu entries: `GameObject/Elemento/...` (groups: Level, Level/Block, Level/Bounce Pad, Hazards, Flow).
- Particle objects are named `FX_<VfxType>`. Vfx ignores any child without the FX_ prefix. Prefabs use `P_`, materials `M_`.

## Performance (pillars: smooth, snappy, 0.5 s respawn)
- No allocations in Update/FixedUpdate: no new, LINQ, string concat, GetComponent, or Find in per-frame code. Cache in Awake.
- No idle particles on level objects. Short event bursts are fine. Idle animation belongs in the shader.
- Use sqrMagnitude, cached hashes (Shader.PropertyToID, Animator hashes), and non-alloc physics queries.
- Do not add per-frame logging. Debug output is behind DebugOverlay or `#if UNITY_EDITOR`.
- Never add input smoothing or slow acceleration to the player. Controls stay tight in every world, including wind.

## Safety
- Anything that changes during a run implements IResettable (CommitState / ResetObject) and registers with RespawnManager, so a retry is identical.
- Null-check scene links in Awake and log one clear warning. Do not crash.
- Effects go through Vfx and Sfx by enum, not by scattered direct references.
- Persistent data uses PlayerPrefs with the existing key patterns only (see ARCHITECTURE.md).
- Enums: append only.

## Level-object rules
- Layers named in the project: Default 0, Player 3, Destructible 6, Hazard 7, Interactable 8. In practice most prefab roots are on Default; only Destructible (6) is used consistently. Do not build logic on layers; identify things by component. No tags are used.
- Saw, SpinArm and StompBlock roots sit on layer 15, which has no name (cleanup item).
- Shapes: square = safe, triangle = danger, hexagon = bounce.
- Level objects need a ResetObject and a gizmo for their range or direction when they have one.

## Shader rules
- Shaders are hand-written HLSL for URP. No Shader Graph for new work.
- Support the SRP Batcher (properties in UnityPerMaterial CBUFFER).
- Neon glow comes from HDR emission above 1 plus the ELEMENTO_NEON Bloom volume. Do not add other post effects.
- Do not edit a shader used by many materials without listing the materials it affects.
