# Coding Rules

## Style
- C#, one MonoBehaviour per file, class name = file name.
- Inspector-tunable fields are `public` or `[SerializeField] private` with `[Header]` and `[Tooltip]`.
- New Input System only (`UnityEngine.InputSystem`). Legacy `Input` class is strictly forbidden.
- Editor-only code goes inside `#if UNITY_EDITOR` or `Assets/Editor/`.
- Create-menu entries: `GameObject/Elemento/...` (subgroups: `Level`, `Level/Block`, `Level/Bounce Pad`, `Hazards`, `Interactables`, `Flow`, `UI`).
- Particle objects are named `FX_<VfxType>`. `Vfx` ignores any child without the `FX_` prefix. Particle materials use `P_`, level/shape materials use `M_`.

## Performance (Pillars: Smooth, Snappy, 0.5 s Respawn)
- **Zero allocations in `Update`/`FixedUpdate`:** no `new`, LINQ, string concatenation, `GetComponent`, or `Find` in per-frame code. Cache references in `Awake`.
- **No idle particles on level objects:** short event bursts are fine; idle animation belongs in the shader.
- Use `sqrMagnitude`, cached hashes (`Shader.PropertyToID`, Animator hashes), and non-alloc physics queries (`OverlapSphereNonAlloc`).
- Do not add per-frame logging. Debug output belongs behind `DebugOverlay` or `#if UNITY_EDITOR`.
- Never add input smoothing or sluggish acceleration to `CC_ELEMENTO`. Controls stay tight in every world.

## Safety & Determinism
- Anything that changes state during a run implements `IResettable` (`CommitState` / `ResetObject`) and registers with `RespawnManager`, so retries are identical.
- Null-check scene links in `Awake` and log one clear warning instead of throwing exceptions.
- Route effects centrally through `Vfx` and `Sfx` by enum.
- Persistent data uses `PlayerPrefs` with existing key patterns only (see `ARCHITECTURE.md`).
- Enums: append only; never reorder serialized values.

## Level-Object Rules
- Every killer uses `Hazard` (Layer 7 `Hazard`). Triggers and collectibles use Layer 8 `Interactable`. Breakables use Layer 6 `Destructible`.
- Layers: Default `0`, Player `3`, Destructible `6`, Hazard `7`, Interactable `8`. **Zero custom tags**—identify objects by component/interface.
- Interactive/moving objects (`MovingPlatform`, `SpinArm`, `LavaLeaper`) should implement `OnDrawGizmos()` showing travel paths or trigger bounds.

## Shader Rules
- Shaders are hand-written HLSL for URP. No Shader Graph for new work.
- Keep `.hlsl` include files (`ElementCommon.hlsl`) in the same folder as the shaders that include them.
- Support the SRP Batcher (properties inside `UnityPerMaterial` `CBUFFER`).
- Neon glow comes from HDR emission (`> 1.0`) plus the `ELEMENTO_NEON` Bloom volume.