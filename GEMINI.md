# Elemento: Agent & Workflow Rules

Elemento is a 2.5D precision platformer built in Unity 6 (6000.3.23f1, URP 17.3, New Input System, Cinemachine 3, Kinematic Character Controller).

## 1. Operating Rules & Source of Truth
- **Source of Truth**: Live project files (`Assets/`, `Packages/manifest.json`, `ProjectSettings/`) are the ultimate truth. Docs represent historical milestones (Nov 2026); if project and docs conflict, trust the project and note the discrepancy.
- **Scope & Safety**: Execute only what `CURRENT_TASK.md` specifies. Never perform unsolicited refactors, renames, or deletions. Ask first before moving files.
- **Enum Safety**: Never reorder enum values (`PowerUpType`, `DashZoneMode`, `VfxType`, `SfxType`, `HitSource`). They are serialized as integers. Append only.
- **Untouchable Zones**: Do not touch `Assets/ThirdParty/KCC/`, legacy folders (`Assets/ART/Materials/Legacy/`, `Assets/ART/Shaders/*/Legacy/`), or empty placeholder assets.
- **Workflow / Handshake**: The user hand-builds scenes and prefabs. Agents write C# scripts, editor tooling, and run validation.

## 2. Unity & CLI Workflow
- **Never Hand-Edit Assets**: NEVER write or edit `.unity` (scene), `.prefab`, or `.asset` raw YAML directly (avoids file corruption and huge token burn). Create or configure assets via Editor scripts (`MenuItem`) or duplicate existing ones in the Editor.
- **Meta File Integrity**: Never hand-edit `.meta` GUIDs. Let Unity generate or manage metadata.
- **Editor Scripts Placement**: Place new Editor-only tools in `Assets/Editor/` (or wrap them strictly inside `#if UNITY_EDITOR` blocks to match existing tools in `Debug/` and `Level/Builders/`). Do not move existing scripts without permission.
- **Validation Before Complete**: Validate every C# edit before declaring done. Target is **0 compile errors and 0 new warnings**.
  - **Headless CLI Validation (When Editor is Closed)**:
    ```powershell
    & "C:\Program Files\Unity\Hub\Editor\6000.3.23f1\Editor\Unity.exe" -batchmode -quit -nographics -projectPath . -logFile compile.log
    Get-Content compile.log | Select-String "error|warning"
    Remove-Item compile.log -ErrorAction SilentlyContinue
    ```
  - **Live Editor Validation (When Editor is Open)**:
    Check `unity status` or trigger compilation via `unity recompile` / Editor Console. Note that Unity locks the project folder against secondary batchmode processes when open.
  - **Level Scene Validation**: Run `ElementoLevelValidator` (`Elemento > Validate Current Level`) whenever a level scene is modified or wired to verify scripts, slots, managers, layers, and Z plane alignment.

## 3. C# Coding Rules
- **Structure**: One MonoBehaviour/class per file; file name must match class name.
- **Input System**: New Input System only (`UnityEngine.InputSystem`). Legacy `Input` class is strictly forbidden.
- **Inspector Tuning**: Serialized fields must be `[SerializeField] private` or `public` with descriptive `[Header]` and `[Tooltip]`.
- **Menu Hierarchy**: Place create-menus under `GameObject/Elemento/...` (subgroups: `Level`, `Hazards`, `Flow`, `Interactables`, `UI`).
- **Naming Conventions**:
  - Particle GameObjects: `FX_<VfxType>` (e.g., `FX_Jump`, `FX_BreakBurst`).
  - Particle Materials: `P_<Shape>` (e.g., `P_Circle`, `P_Ring`).
  - Level Materials: `M_<Name>` (e.g., `M_Level_Cyan`, `M_Gate_Fire`).
  - Prefabs: `P_` or clean CamelCase (e.g., `StompBlock`, `ElementGate_Base`).

## 4. Performance & Determinism (0.5s Respawn Pillar)
- **Zero Allocations in Update/FixedUpdate**: No `new`, LINQ, string concatenation, `GetComponent`, or `GameObject.Find` in frame loops. Cache all references and components in `Awake()`.
- **Math & Physics**: Use `sqrMagnitude` instead of `magnitude`/`Vector3.Distance`. Cache property IDs (`Shader.PropertyToID`) and use non-alloc physics queries (`Physics.OverlapSphereNonAlloc`, cached collider arrays).
- **Deterministic Retries**: Any level object with dynamic state must implement `IResettable` (`CommitState` and `ResetObject`) and register with `RespawnManager`.
- **Particles**: No idle particle loops on static level objects. Use shader effects for idle visuals; particles are reserved for short event bursts or warning states (`FireJet`, `LavaLeaper`).
- **Audio & VFX**: Route effects centrally through `Vfx.cs` (`VfxType`) and `Sfx.cs` (`SfxType`) rather than direct component references.

## 5. Layers & Level Design Rules
- **Layers**: Default `0`, Player `3`, Destructible `6`, Hazard `7`, Interactable `8`. No Unity tags are used (identify objects by component/interface).
- **Shape Language**: Square = Safe platform, Triangle = Danger/Hazard, Hexagon = Bounce pad.
- **Gizmos**: All interactive or moving level objects (`MovingPlatform`, `SpinArm`, `LavaLeaper`) must implement `OnDrawGizmos()` showing travel paths, trigger radii, or warning bounds.
- **Z Plane**: Gameplay occurs strictly on the 2D plane ($Z = 0$). Level objects must maintain position $Z = 0$.

## 6. Report Format (End of Every Task)
1. **Files Changed**: One-line path per file modified or created.
2. **Verification Steps**: Exact steps to test or verify in the Unity Editor.
3. **Inspector Wiring Required**: Specific slots/references the user must wire by hand (`[Prefab/Object] -> [Component] -> [Field] = [Target]`).
4. **Risks / Unverified Items**: Any potential side-effects, unresolved edge cases, or assumptions made.
