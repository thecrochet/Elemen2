# Elemento: Project Architecture & Technical Context

Comprehensive technical map and architecture reference (Nov 2026 Audit).

## 1. Project Specifications & Packages
- **Engine**: Unity 6 (`6000.3.23f1`), Universal Render Pipeline (URP `17.3.0`).
- **Core Packages**: Input System (`1.20.0`), Cinemachine (`3.1.7`), AI Navigation (`2.0.14`), UGUI (`2.0.0`), Kinematic Character Controller (`ThirdParty/KCC`).
- **Collision & Layers**: Default `0`, Player `3`, Destructible `6`, Hazard `7`, Interactable `8`. No Unity tags are used.
- **Rendering & Glow**: URP Bloom volume (`ELEMENTO_NEON`) with HDR emission values ($> 1.0$). Shaders are hand-written HLSL with SRP Batcher support (`UnityPerMaterial` CBUFFER).

## 2. Live Codebase & Directory Map (`Assets/SCRIPTS/`)
- **`Core/`**:
  - `LevelManager.cs`: Level clock, 3-star time tracking, finish sequence, loads next scene by name/index (wraps to scene 0 on completion).
  - `RespawnManager.cs`: Handles 0.25s respawns, kill floors ($Y = -25 / +75$), registers and commits/resets every `IResettable`.
  - `Vfx.cs`: Singleton managing 22 ball particle effects and tinted one-shots by `VfxType` enum.
  - `Sfx.cs`: Audio dispatch managing 34 `SfxType` events with fallback logic.
  - `PowerUpType.cs`: Ordered enum: `Fire=0`, `Air=1`, `Earth=2`, `Water=3`, `None=4`, `Aether=5` (append only).
- **`Player/`**:
  - `CC_ELEMENTO.cs`: KinematicCharacterController implementation. Movement, Jump, Double Jump, Dash, Ground Pound, Wall Jump, Glide, Gravity Flip.
  - `BallVisuals.cs`: Updates ball body material and effects per element.
- **`Level/Flow/`**:
  - `LevelStart.cs`, `LevelGoal.cs`, `LevelExitSequence.cs`: Scene flow and zoom-in portal transitions.
  - `Checkpoint.cs`: Saves active player power and commits all `IResettable` states.
  - `CameraZone.cs`: Blends Cinemachine framing and distance.
  - `DashZone.cs`: Multi-mode dash triggers (`Hint`, `Boost`, `Lane`, `Charge`, `SuperDash`).
  - `Diamond.cs` & `Atom.cs`: Collectibles with persistent PlayerPrefs save, identical reset loop, and particle renderer preservation.
  - `Bandage.cs`: Legacy collectible script (no prefab, replaced by Diamond).
- **`Level/Hazards/`**:
  - `Hazard.cs` (base kill trigger), `RotatingSaw.cs`, `SpinArm.cs`, `StompBlock.cs` (IMoverController), `Cannon.cs`, `FireJet.cs`, `LavaLeaper.cs`.
- **`Level/Interactables/` & `Platforms/`**:
  - `BouncePad.cs`, `DestructibleObject.cs` (hits 1-3, cracked overlay stages, chain breaks, `CrackEffect`), `ElementGate.cs` (run-through element gate).
  - `MovingPlatform.cs` (IMoverController), `CrumblingBlock.cs`, `IceSurface.cs`, `OneWayPlatform.cs`.
- **`Level/Builders/` & `Flow/`**:
  - `ElementoPrefabBuilders.cs` & `ElementoUIBuilders.cs`: Editor creation tools.
  - `ElementoStarHudTools.cs`: Generates `Star.png` sprite, `SFX_StarLost.wav`, and configures LevelHUD.
  - `NeonBlock.cs`, `NeonShapeMesh.cs`, `NeonPortalTunnel.cs`: Procedural neon mesh construction.
  - `IntroSequence.cs` & `GridReveal.cs`: Tutorial intro sequence, Made with Unity logo fade, and shader grid reveal (`_ElementoGridHide`).
- **`Debug/`**:
  - `ElementoLevelValidator.cs`: 11-rule automated level health validator (`Elemento/Validate Current Level`).
  - `PlayerLab.cs`: Runtime power mixer (keys 1-6) and jump reach stats sheet (F5 export).
  - `DebugOverlay.cs` & `ElementoParticleDump.cs`: Performance overlay and VFX auditor.
- **`UI/`**: `LevelHUD.cs` (timer, stars, attempts), `PauseMenu.cs`, `ScreenFade.cs`, `ControlPrompt.cs`.

## 3. Core Prefab Hierarchy & Scene Architecture
- **`LevelCore.prefab`**: Central per-level package. Contains:
  - `Managers`: `LevelManager`, `RespawnManager`, `Vfx`, `Sfx`, `DebugOverlay`.
  - `Player`: `CC_ELEMENTO`, `KinematicCharacterMotor`, `BallVisuals`, AudioSource.
  - `CameraRig`: Cinemachine Camera, Position Composer, Brain.
  - `LevelUI`: Canvas, `LevelHUD`, `PauseMenu`, `FinishPanel`, `ScreenFade`, `EventSystem`.
  - Kill boundaries (`KillPit`, `KillPit2`) and `FX_FinalFrame`.
- **Scene Flow**: `TEMPLATE_SCENE` contains only `StartPortal`, `LevelCore`, level geometry, hazards, and `End_Portal`.
- **Save Schema (PlayerPrefs)**:
  - `BestTime_<sceneName>` (float, -1 = uncompleted).
  - `Diamond_<sceneName>_<id>` and `Atom_<sceneName>_<id>` (int 1 = collected).
  - `Unlocked_Powers` (planned bitmask for permanent unlocks).

## 4. Master Lists Summary
- **VFX System**:
  - **Ball Effects (22)**: 12 one-shots (`Jump`, `DoubleJump`, `WallJump`, `Land`, `DashStart`, `GroundPoundStart`, `GroundPoundImpact`, `GlideOpen`, `GravityFlip`, `PowerUpCollect`, `Respawn`, `Death`), 10 loops (`DashTrail`, `RunTrail`, `WallSlide`, `GlideStream`, 6 Ambients). `FX_AmbientAether.prefab` exists.
  - **Object Effect Slots (20)**: `FX_CheckpointActivate`, `FX_BreakBurst`, `FX_CrackHit`, `FX_PadLaunch`, `FX_PadLaunchPound`, `FX_Crumble`, `FX_StompImpact`, `FX_Finish`, `FX_GateCross`, `FX_DiamondCollect`, `FX_AtomCollect`, `FX_DashBoost`, `FX_FireJetFlame`, `FX_FireJetWarn`, `FX_LeaperTrail`, `FX_LeaperWarn`, `FX_LeaperSplash`, `FX_CannonShoot`.
- **SFX System**: 34 events in `SfxType` + procedural `SFX_StarLost.wav`. Master Sfx object manages 12 voices, distance 25, pitch jitter $\pm 0.05$.
- **Materials & Shaders**:
  - Shaders (`Assets/ART/Shaders/`): `NeonShape`, `NeonGrid`, `NeonParticle`, `CrackOverlay`, `DashZone`, `Gate/ElementGate`, `Portal/Frame`, `Portal/TunnelWall`, `Ball/*`.
  - Materials (`Assets/ART/Materials/`): 9 white `P_*` particle materials, `M_Level_*` grid blocks, `M_Shape_*` danger/interactive solids, `M_Crack`, `M_Gate_*`, `M_Portal_*`.

## 5. Audit & Known Issues Status
### Resolved in Live Project
- **DestructibleObject**: `CrackEffect` slot implemented and operational.
- **ScreenFade**: Edit-time black screen fixed via `Reset()` setting alpha to 0.
- **Diamond & Atom**: `SetVisible()` properly excludes `ParticleSystemRenderer` so bursts play.
- **Ambient Aether**: `FX_AmbientAether.prefab` built and available in `Assets/Prefabs/VFX/Player/World/`.

### Active Issues & Watchlist
- **Unscripted Prefab**: `Assets/Prefabs/LEVEL/Flow/SecretDoor.prefab` exists with no script attached.
- **Typo Assets**:
  - `TutorialSIgn_*.prefab` (capital `I` in Dash, Glide, Gravity, GroundPound, Jump variants).
  - `DasWall_weak.prefab` (missing `h`).
  - `ElementGate _Fire.prefab` (leading whitespace).
  - `InputSystem_Player.inputactions`: action spelled `GroudPound`.
- **Architectural Cleanup**:
  - `ElementoPrefabBuilders.cs` holds two classes (`ElementoMaterials` and `ElementoPrefabBuilders`).
  - `PowerUpPickup.cs` and `Bandage.cs` are obsolete dead code awaiting formal deletion.
  - Editor tools (`ElementoLevelValidator.cs`, `PlayerLab.cs`, etc.) currently sit in runtime script folders rather than `Assets/Editor/`.
