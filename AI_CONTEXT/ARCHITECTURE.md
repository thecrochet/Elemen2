# Architecture (Verified Project Reference)

Scripts live under `Assets/Scripts/`. Third-party KCC (`Assets/ThirdParty/KCC` / `KinematicCharacterController`) and `Legacy` folders are out of scope.

## Template Scene & 7-Container Standard
- **Template Scene:** `Assets/Scenes/Templates/LEVEL_TEMPLATE.unity` (always duplicate this exact file for new levels; never search for other template scenes).
- **7 Root Containers (Every playable level organizes root GameObjects cleanly at `Z = 0`):**
  1. `LevelCore` (existing from template: `Player` with `CC_ELEMENTO` / `BallVisuals` / `KinematicCharacterMotor`, `CameraRig`, `LevelManager`, `RespawnManager`, `Vfx`, `Sfx`, `LevelUI`, `DebugOverlay`, kill pits)
  2. `Environment` (`Ground_20x1`, `Block_1x1`, `Ledge_8x1`, `Wall_1x12`, `Ceiling_20x1`, `Slope_45`, `Slope_45_Large`, `MovingPlatform`, `CrumblingBlock`, `IceSurface`, `OneWayPlatform`, `LevelFrame_40x14`)
  3. `Interactables` (`ElementGate_Fire`, `ElementGate_Earth`, `ElementGate_Water`, `ElementGate_Air`, `ElementGate_Aether`, `ElementGate_Base`, `DashWall`, `DashWall_DashOnly`, `DashWall_Tough`, `DashWall_ChainRow`, `CrackedBlock`, `BouncePad`, `BouncePad_Mini`, `BouncePad_Mega`, `DashZone`, `DashBoost_Fast`, `DashLane_UP`, `DashHold`, `RechargeOrb`, `EarthHoverPad`)
  4. `Hazards` (`Spike`, `Saw_Slow`, `Saw_Fast`, `SpinArm`, `StompBlock`, `FireJet`, `Cannon`, `Cannon_Fast`, `LavaLeaper`)
  5. `Collectibles` (`Diamond`, `Atom` — each with unique scene ID)
  6. `Checkpoints` (`Checkpoint` / `CheckPoint` — max 1 per level per `DESIGN.md`)
  7. `Flow` (`StartPortal`, `End_Portal`, `CameraZone`, `CameraZone_Wide`, `CameraZone_Climb`, `CameraZone_Tight`, `TutorialSIgn_*` — note capital `I` in prefab filenames)
- **Other Prefab Folders (`Assets/Prefabs/`):** `Camera/`, `Player/`, `Managers/`, `UI/`, `Props/`, `VFX/`, `Worlds/World1_Fire/`. Each category folder under `Assets/Prefabs/Level/` contains its own `Variants/` subfolder.
- **Prefab Warning:** Two `DashWall` prefabs exist—always instantiate breakable walls from `Assets/Prefabs/Level/Interactables/`, never the non-breakable copy in `Level/Platforms/`.
- **Clean Runtime Presentation:** Ensure `DebugOverlay.StartVisible = false` and `PlayerLab.ShowPanel = false` / `MixPowers = false` on `LevelCore`.
- **Automated Validator:** `Assets/Scripts/Debug/ElementoLevelValidator.cs` runs 11 automated scene checks (`Elemento > Validate Current Level`).

## Core Systems & Managers
- `Core/LevelManager` (`IResettable`): level clock, best time (`BestTime_<sceneName>`), level finish, scene loading (`NextSceneName` or next build index).
- `Core/RespawnManager`: 0.25 s respawn delay, `KillY` / `KillTopY`, commits/resets every `IResettable` at start and checkpoints.
- `Core/Vfx`: singleton playing `FX_<VfxType>` children by enum (22 ball effects + tinted one-shots). Keep one active instance per scene.
- `Core/Sfx`: 34 `SfxType` events with fallback logic.
- `Core/PowerUpType`: ordered enum `Fire = 0, Air = 1, Earth = 2, Water = 3, None = 4, Aether = 5` (append only).
- `Core/LevelLoadout` & `PowerMask`: scene loadout mask (`AllowedPowers`, `freeMode`, `unlockAll`) intersected with permanent `PowerSave` (`Unlocked_Powers`).

## Player & Camera
- `Player/CC_ELEMENTO`: KCC controller (`IsSideScroller = true`, `Z = 0`). States: `Default`, `Dashing`, `GroundPound`, `Dead`, `Finished`, `Held`. Supports Jump, Double Jump, Fire Dash (with block momentum), Earth Ground Pound (with Mario slam through stacked breakables), Water Wall Jump & Gravity Flip, Air Glide, Slope Climb, One-Way Drop-Through, `GiveRecharge`, and `LaunchFromPadSlamOnly`.
- `Player/BallVisuals`: rests in Base Spark (or Aether), morphs into element material during moves (`MoveStarted` + state morphing), dims when air charges are spent, pulses on recharge.
- `Camera/CameraZoneBlender`: blends Cinemachine 3 Position Composer settings between `CameraZone` triggers.

## Level Builders, Layers & Input
- `NeonBlock`: procedural mesh + collider builder (`Anchor`: `TopLeft` on ground/ledge/one-way, `Center` on `DashWall`/moving/crumbling). Resize via whole-number `NeonBlock.Size`, never root `Transform.localScale`.
- **Layers & Tags:** Default (0), Player (3), Destructible (6), Hazard (7), Interactable (8). **Zero custom tags**—never create or assign custom tags; scripts identify objects by component/interface.
- **Save Keys (`PlayerPrefs`):** `BestTime_<sceneName>`, `Diamond_<sceneName>_<id>`, `Atom_<sceneName>_<id>`, `Unlocked_Powers`.