# Coding rules

## Style
- C#, one MonoBehaviour per file, class name = file name.
- Inspector-tunable fields are public or [SerializeField] with [Header] and [Tooltip].
- New Input System only. No legacy Input class.
- Editor-only code goes inside `#if UNITY_EDITOR`.
- Create-menu entries: `GameObject/Elemento/...` (groups: Level, Level/Block, Level/Bounce Pad, Hazards, Flow).
- Builder tools and menu items must say in their menu name or tooltip what they create. They never run automatically and never overwrite an existing asset.
- Naming: particle objects and effect prefabs are `FX_<name>` (ball effects are `FX_<VfxType>`; Vfx ignores a ball-effect child without the FX_ prefix). Particle materials are `P_`. Every other material is `M_`.

## Performance (pillars: smooth, snappy, 0.5 s respawn)
- No allocations in Update/FixedUpdate: no new, LINQ, string concat, GetComponent, or Find in per-frame code. Cache in Awake.
- No idle particles on level objects. Short event bursts are fine. Idle animation belongs in the shader. Exception: a warn or active state loop on a hazard (FireJet, LavaLeaper) that runs only while the hazard is warning or active.
- Use sqrMagnitude, cached hashes (Shader.PropertyToID, Animator hashes), and non-alloc physics queries.
- Do not add per-frame logging. Debug output is behind DebugOverlay or `#if UNITY_EDITOR`.
- Never add input smoothing or slow acceleration to the player. Controls stay tight in every world, including wind.

## Safety
- Anything that changes during a run implements IResettable (CommitState / ResetObject) and registers with RespawnManager, so a retry is identical.
- Null-check scene links in Awake and log one clear warning. Do not crash.
- Effects go through Vfx and Sfx by enum, not by scattered direct references.
- Persistent data uses PlayerPrefs with the existing key patterns only (see ARCHITECTURE.md).
- Enums: append only (PowerUpType, VfxType, SfxType, DashZone modes, NeonBlock anchor).

## Level-object rules
- Every killer uses Hazard (layer Hazard). Triggers and collectibles use layer Interactable. Breakables use Destructible.
- Layers: Default 0, Player 3, Destructible 6, Hazard 7, Interactable 8. No tags are used; identify things by component.
- Shapes: square = safe, triangle = danger, hexagon = bounce.
- Level objects need a ResetObject and a gizmo for their range or direction when they have one.
- A prefab's root holds the gameplay (scripts, colliders). A child named Visual holds only the picture. Root scale stays 1,1,1.

## Shader rules
- Shaders are hand-written HLSL for URP. No Shader Graph for new work.
- Support the SRP Batcher (properties in UnityPerMaterial CBUFFER).
- Neon glow comes from HDR emission above 1 plus the ELEMENTO_NEON Bloom volume. Do not add other post effects.
- Do not edit a shader used by many materials without listing the materials it affects.
