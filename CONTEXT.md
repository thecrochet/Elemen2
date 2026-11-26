# Elemento: Project Architecture & Technical Context

Single-source technical reference and campaign status for the **Elemento** Unity 6 workspace (**Active Branch:** `NEW-MAIN-DIEGO`).

## 1. Project Specifications & Packages
- **Engine & Pipeline**: Unity 6 (`6000.3.23f1`), Universal Render Pipeline (`URP 17.3.0`).
- **Installed Packages (`Packages/manifest.json`)**: Input System (`1.20.0`), Cinemachine (`3.1.7`), AI Navigation (`2.0.14`), UGUI (`2.0.0`), Test Framework (`1.6.0`), Timeline (`1.8.13`), Visual Scripting (`1.9.12`), and Kinematic Character Controller (`Assets/ThirdParty/KCC`).
- **Layers & Tags**: Default `0`, Player `3`, Destructible `6`, Hazard `7`, Interactable `8`. No Unity tags are used (objects are identified by component or interface).
- **Rendering & Glow**: URP Bloom (`ELEMENTO_NEON`) with HDR emission values (`> 1.0`) and hand-written HLSL shaders supporting SRP Batcher (`UnityPerMaterial` CBUFFER).

## 2. Live Codebase & Directory Map
- **`Assets/SCRIPTS/Core/`**:
  - `LevelManager.cs`: Level timer, 3-star thresholds, `ResolveNextSceneName()` (`Level_XX` -> `Level_(XX+1)`), and `LoadNextLevel()` (after the last `FINAL_PURPLE` level in `EditorBuildSettings`, searches for `Hub`, `MainMenu`, or `00_Tutorial`—currently resolving to `00_Tutorial.unity` at `buildIndex = 1`—before falling back to `0`).
  - `RespawnManager.cs`: Deterministic 0.25s respawn handler (`<= 0.5s` pillar), KillY bounds, and `IResettable` state commit/reset registry.
  - `Vfx.cs`: Singleton dispatching 22 `VfxType` particle slots (`12` one-shots, `4` movement loops, `6` ambient loops).
  - `Sfx.cs`: Multi-voice audio dispatcher managing 35 `SfxType` events (`Jump`..`StarLost`, pitch jitter `±0.05`, default 12 pooled voices).
- **`Assets/SCRIPTS/Player/` & Root**:
  - `PowerUpType.cs` (`Assets/SCRIPTS/`): Ordered enum `Fire=0`, `Air=1`, `Earth=2`, `Water=3`, `None=4`, `Aether=5` (append-only).
  - `CC_ELEMENTO.cs`: KCC player controller implementing Run, Jump, Double Jump, Dash, Ground Pound, Wall Jump, Glide, Gravity Flip, `BoostLaunch`, and `EnterHold`. Purple Ball campaign configuration starts with `StartingPower = PowerUpType.Aether` and all 5 native abilities enabled (`DefaultDash`, `DefaultDoubleJump`, `DefaultWallJump`, `DefaultGroundPound`, `DefaultGlide = true`, `GlideNeedsNewPress = false`; `BoostSmashDuration = 0.5f`, `BoostMomentumTime = 1.2f`, `BoostMaxSpeed = 30f`).
  - `BallVisuals.cs`: Updates ball visuals and element effects.
- **`Assets/SCRIPTS/Camera/` & `Flow/`**:
  - `CameraZoneBlender.cs`: Smooth Cinemachine 3 camera zone blending.
  - `IntroSequence.cs`, `GridReveal.cs`, `TutorialBuilders.cs`: Tutorial reveal and intro sequence tools.
- **`Assets/SCRIPTS/Level/`**:
  - `Flow/`: `LevelStart.cs`, `LevelGoal.cs`, `LevelExitSequence.cs`, `Checkpoint.cs`, `CameraZone.cs`, `DashZone.cs` (6 `ZoneMode` values: `Hint`, `Boost`, `Lane`, `Charge`, `SuperDash`, `Hold`), `Diamond.cs`, `Atom.cs`, `TutorialSign.cs`, `Bandage.cs` (legacy unused collectible).
  - `Hazards/`: `Hazard.cs`, `RotatingSaw.cs`, `SpinArm.cs`, `StompBlock.cs` (`IMoverController`), `Cannon.cs`, `FireJet.cs`, `LavaLeaper.cs`.
  - `Interactables/`: `BouncePad.cs`, `DestructibleObject.cs` (configurable `HitsToBreak`, `PreCracked` stages, forward `ChainReaction`, `IResettable`), `BreakableWallFeedback.cs` (debris bursts, camera impulse, hitstop), `DebrisKickable.cs`, `ElementGate.cs`.
  - `Platforms/`: `MovingPlatform.cs` (`IMoverController`), `CrumblingBlock.cs`, `IceSurface.cs`, `OneWayPlatform.cs`.
  - `Builders/`: `NeonBlock.cs`, `NeonShapeMesh.cs`, `NeonPortalTunnel.cs`, `ElementoPrefabBuilders.cs`, `ElementoUIBuilders.cs`, `ElementoStarHudTools.cs`.
- **`Assets/SCRIPTS/Debug/` & `UI/`**:
  - `Debug/`: `ElementoLevelValidator.cs` (11-rule validation under `Elemento > Validate Current Level`), `PlayerLab.cs`, `DebugOverlay.cs`, `ElementoParticleDump.cs`.
  - `UI/`: `LevelHUD.cs`, `PauseMenu.cs`, `ScreenFade.cs`, `ControlPrompt.cs`, `NeonWaveButton.cs`.
- **`Assets/Editor/`**:
  - `ElementoFinalPurpleSync.cs`: Scans `FINAL_PURPLE/Level_*.unity`, sorts numerically, wires sequential `LevelGoal` / `LevelExitSequence` / `LevelManager` links and `RespawnManager.SpawnPoint`, and syncs `ProjectSettings/EditorBuildSettings.asset` (`Elemento > Sync FINAL_PURPLE Levels`).
  - `BlueBlockPrefabCreator.cs`: Configures `TouchWall_Blue.prefab` (`0f` hitstop, purple emission) and `DashWall.prefab` (`0.02s` micro-hitstop).
  - `FinalPurpleLevelBuilder.cs` (`Level_01`–`Level_35`), `Level36Builder.cs`–`Level49Builder.cs`, `Level51Builder.cs`, `Level52Builder.cs`, and `DiegoLevelBuilder*.cs` (`01`–`07`).

## 3. Core Gameplay Rules, Scene Standards & Pipeline
- **Purple Ball Only**: All campaign scenes use `PowerUpType.Aether` with Dash, Earth Ground Pound, Double Jump, Wall Jump, and Gliding enabled from spawn (`ElementGate` count = `0`, `DebugOverlay.StartVisible = false`, `PlayerLab.ShowPanel = false`).
- **7-Container Scene Standard (`Z = 0`)**: Every level organizes all gameplay objects at `Z = 0` inside 7 root containers:
  1. `LevelCore`: Managers, Cinemachine rig (`CameraDistance = 54` on baseline Position Composer), `CC_ELEMENTO`, HUD, kill triggers.
  2. `Environment`: Enclosed framing geometry (`Ground_20x1`, `Ceiling_20x1`, `Wall_1x12`, `Slope_45`, `Slope_45_Large`, `Slope_45_Left`, `Block_Cyan`, `Block_Green Variant`, `Block_Magenta Variant`, `MovingPlatform 1`, `CrumblingBlock`, `IceSurface`, `OneWayPlatform` / `_Large`, `KillPit`).
  3. `Interactables`: Purple touch-blocks (`TouchWall_Blue`), Yellow breakables (`DashWall`, `DashWall_ChainRow 1`, `DashWall_Cracked`, `CrackedBlocks_4x4`, `GroundPoundChainRow 2`), bounce pads (`BouncePad`, `BouncePad_Mini`, `BouncePad_Mega`), and barrel/boost triggers (`DashBoost_Fast`, `DashBoost_Left`, `DashBoost_Up`, `DashZone`, `DashHold`, `DashLane_UP`, `DashCharge`).
  4. `Hazards`: `SpinArm`, `StompBlock`, `Saw_Slow` / `Saw_Fast`, `Cannon` / `Cannon_Fast`, `FireJet`, `LavaLeaper`.
  5. `Collectibles`: `Diamond` (unique IDs `1..3` per level) and `Atom` (unique IDs `1..16` per level).
  6. `Checkpoints`: `CheckPoint` triggers saving progress and committing `IResettable` objects.
  7. `Flow`: `StartPortal` (`RespawnManager.SpawnPoint`), `End_Portal` (`LevelGoal` + `LevelExitSequence`), and `CameraZone` volumes (`CameraZone_Wide`, `CameraZone_Climb`, `CameraZone_Tight`).
- **CLI-First Asset & Validation Pipeline**: Never hand-edit raw `.unity`, `.prefab`, `.asset`, or `.meta` YAML files. Create/update scenes via Unity CLI + `Assets/Editor/` builders, sync sequential progression and `EditorBuildSettings.asset` with `Elemento > Sync FINAL_PURPLE Levels`, verify zero errors via `Elemento > Validate Current Level`, and confirm Play Mode physics and `0` console errors.
- **Save Schema (`PlayerPrefs`)**: `BestTime_<sceneName>`, `Diamond_<sceneName>_<id>`, `Atom_<sceneName>_<id>`, and `Unlocked_Powers`.

## 4. Dual Breakable Block System
- **Purple Touch Blocks (`TouchWall_Blue.prefab` & `DebrisCube_Blue.prefab`)**:
  - `BreakableByTouch = true`, `HitsToBreak = 1`: shatter immediately on any player contact or rolling without needing Dash, Ground Pound, or boost impulse.
  - `hitStopDuration = 0f` (`0f` hitstop): preserves full speed through carpets, curtains, clouds, and barrel chains, using `M_Level_Blue_Touch.mat` (`_EdgeColor: 1.75, 0, 2.8`) and neon purple VFX/debris.
- **Yellow Breakable Blocks (`DashWall.prefab`, `DashWall_ChainRow 1`, `DashWall_Cracked`, `CrackedBlocks_4x4`, `GroundPoundChainRow 2`, `DebrisCube_Yellow.prefab`)**:
  - Shatter ONLY on active manual Dash, active Earth Ground Pound, or active Chevron Boost / Barrel impulse (`isBoostImpulseActive == true`, including `boostSmashDuration = 0.5f` post-boost). Normal walking, rolling, or slope sliding treats them as solid walls.
  - Tuned `hitStopDuration = 0.02s` micro-hitstop (`0.15s` shared cooldown in `BreakableWallFeedback`) for snappy multi-block chain punches without stalling momentum.

## 5. Dynamic Level Design Guidelines (`FINAL_PURPLE` Flagship Standard)
- **Enclosed *Dead Cells* Biome Silhouettes**: Grounded, asymmetrical architectural chambers framed with roofs, structural bedrock piers, stepped floors, and neon trim (`Block_Cyan`, `Block_Green Variant`, `Block_Magenta Variant`) — never floating islands in empty space or repeated corridor templates.
- **Omni-Directional *Donkey Kong* Barrel-Cannon Chains**: Continuous kinetic chaining in every act using `DashBoost_Fast`, `DashBoost_Left`, `DashBoost_Up`, `DashZone`, `DashHold` (aimable hold barrels), `DashLane_UP`, `DashCharge`, and `BouncePad*` (`4.8m` / `9.3m` apex), calibrated with zero solid trajectory clips (`MaxXGap <= 18m`, clear vertical bounce skylights).
- **Dense Tactile Destruction & Multi-Directional Routing**: Heavy `TouchWall_Blue` ribbons, hanging curtains, blast-pockets, and stepped shields paired cleanly with Yellow `DashWall*` gates and GP plunge strata across descents, vertical climbs, backwards spans, secret vaults, and finale bastions (`YOut18m = 0`).
- **Living Terrain, Camera Lead-In & Forgiving Recovery**: Dynamic platforms (`MovingPlatform 1`, `CrumblingBlock`) coupled within `10m` of timed hazards (`>= 75%` coupling target; `OnLaunch<3m = 0`), early overlapping `CameraZone` boundaries (`>= 6m` runway past launchers), calm `CheckPoint` sanctuaries (`3` per level), and continuous solid safety catch-lanes with recovery launchers underneath air/ceiling routes.

## 6. Purple Ball Campaign Status (`Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/`)
- **Verified Campaign Scope**: **`52` levels on disk** (`Level_01.unity` through `Level_52.unity`), all **`52` enabled in `ProjectSettings/EditorBuildSettings.asset`** (`indices 3..54`; `55` total entries alongside `DEMO_SCENE.unity` disabled `[0]`, `00_Tutorial.unity` `[1]`, and `PrefabLab.unity` `[2]`).
- **Verified Linking**: Every level links sequentially `Level_01` -> `Level_02` -> ... -> `Level_51` -> `Level_52`, and `Level_52.unity` completes with empty explicit next (`ResolveNextSceneName` = `""`), falling back cleanly via `LevelManager.LoadNextLevel()` to `00_Tutorial.unity` (`buildIndex = 1`, or `Hub`/`MainMenu` when present).
- **Campaign Inventory (`Level_01`–`Level_52`)**:
  - `Level_01.unity`–`Level_35.unity` (*Built via `Assets/Editor/FinalPurpleLevelBuilder.cs`*): Progressive 35-level foundation introducing dual-block chaining, Ground Pound shafts, pinball canyons, ice/slope momentum foundry runs, gliding updrafts, and showcase milestones (`Level_05`, `Level_10`, `Level_15`, `Level_20`, `Level_25`, `Level_30`, `Level_35`).
  - `Level_36.unity` ("Dead Cells Showcase", `Level36Builder.cs`): 4 enclosed battlements/catacombs/clockwork/citadel wings (`X = 0..334`) with secret Diamond alcoves and glide shortcuts.
  - `Level_37.unity` ("Ancient Sewers & Fractured Shrines", `Level37Builder.cs`): Conduit drops, split Upper Catwalk / Lower Floodway Cistern, fractured spire climb, and sanctum gauntlet (`X = 0..362`).
  - `Level_38.unity` ("Slumbering Sanctuary & Crystal Core", `Level38Builder.cs`): High-start temple -> 36m Crystal Quarry GP descent -> Awakening vertical breakout -> Throne glide finale (`X = 0..368`).
  - `Level_39.unity` ("Barrel-Cannon Hyper-Smash", `Level39Builder.cs`): Chained barrel-cannon velocity showcase tearing oversized multi-layer monoliths (`X = 0..386`).
  - `Level_40.unity` ("Purple Crown Finale", `Level40Builder.cs`): Historical 40-level milestone fusing Dead Cells vaults with 4-link barrel crown monolith smashes (`X = 0..406`).
  - `Level_41.unity` ("Multi-Direction Barrel Chaos", `Level41Builder.cs`): Crooked descent -> tower climb -> backwards right-to-left cathedral -> Catacomb Blast-Well -> diagonal up-right Chaos Spire (`X = 0..238`, `Y = -28..45`).
  - `Level_42.unity` ("Figure-8 Barrel Citadel", `Level42Builder.cs`): Bottom-up Figure-8 loopback (`47` momentum devices, `14` hazards, `759` destructibles, Play Mode verified).
  - `Level_43.unity` ("Orbit Barrel Citadel", `Level43Builder.cs`): Center-start Counter-Clockwise Orbit (`36` momentum devices, `11` hazards, `719` destructibles, Play Mode verified).
  - `Level_44.unity` ("Kinetic Sawtooth Pendulum", `Level44Builder.cs`): Dynamic-coupled sawtooth route (`DynRatio = 11.0%`, `53` momentum devices, `12` hazards, `942` destructibles, Play Mode verified).
  - `Level_45.unity` ("Observatory Mega-Slingshot", `Level45Builder.cs`): Top-right Meteor Deck dive -> Left Ricochet Prism -> Collapsing Zenith Span -> Central Vertical Mega-Slingshot into Zenith Vault (`923` destructibles, `0` Validator errors, Play Mode verified).
  - `Level_46.unity` ("Twin-Tower Pendulum Engine", `Level46Builder.cs`): Bottom-center start -> X-shaped crossfire wings -> Heart-Slash Sub-Spawn Crown Vault (`DynRatio = 16.6%`, `58` momentum devices, `22` hazards, `1239` destructibles, Play Mode verified).
  - `Level_47.unity` ("Abyssal Ziggurat Descent", `Level47Builder.cs`): Top-to-bottom 4-stratum switchback (`DynRatio = 20.0%`, `55` momentum devices, `20` hazards, `1494` destructibles, Play Mode verified).
  - `Level_48.unity` ("Barrel-Blast Highway: Multi-Vector Box Smash", `Level48Builder.cs`): Forward L->R multi-angle barrel highway (`DynRatio = 14.5%`, `58` momentum devices, `14` hazards, `1138` destructibles, Play Mode verified).
  - `Level_49.unity` ("The Sky-Shatter Rollercoaster", `Level49Builder.cs`): 400m high-elevation barrel rollercoaster (`DynRatio = 11.7%`, `62` momentum devices, `17` hazards, `881` destructibles).
  - `Level_50.unity`: Preserved prior campaign scene (`X = 0..418`, `19` barrels, `9` `CameraZone`s) wired into `Level_51.unity`.
  - `Level_51.unity` ("The Ricochet Reactor — Corkscrew Barrel Pipeline", `Level51Builder.cs`): Cryo-Slingshot intake, Twin-Conduit Corkscrew alternates, Turbine Pinball Dome, and 6-link Overdrive Core Breach (`DynRatio = 12.5%`, `72` momentum devices, `21` hazards, `862` destructibles, Play Mode verified).
  - `Level_52.unity` ("The Shatter-Lock Viaduct — Over-Under Archway Blitz", `Level52Builder.cs`): Enclosed multi-arch viaduct & sub-pier showcase spanning Floodgate Lock Breach (1-hit GP hatch + `IceSurface` + `Slope_45_Large` + Over-Arch #1 Keystone smash), Sub-Pier Undercut Gauntlet (`GroundPoundChainRow 2` plunge + Pier #2 `Diamond 1` vault), Twin-Arch Sky Slalom (Gliding slalom + Upper Rafter `Diamond 2` express), and 3 escalating Shatter-Pyramids (`Diamond 3` + 8-deep Yellow Containment Bastion -> `End_Portal@(406, 31.8)`). Audited cleanly (`Spatial Overlaps = 0`, `DynRatio = 13.0%`, `62` momentum devices / `23` barrels, `MaxXGap = 18.0m`, `SolidClips = 0`, `PadCeil = 0`, `MPSweepHits = 0`, `16/19` hazards coupled `<= 10m`, `894` destructibles, `Validator errors = 0`, `13/13` Play Mode physics checks passing).

## 7. Next Pending Milestone
- **Neon-Glowing Pause Menu (`PauseMenu.prefab`)**: Upgrade with `NeonWaveButton.cs` pulsing neon-wave shaders, unified Purple/Cyan/Gold hover & selection states, clean layout spacing, and full keyboard/gamepad UI navigation.
- **Dynamic Level Select Screen**: Build a paginated interactive selector dynamically reading all **`52` synced `FINAL_PURPLE` scenes** (`Level_01`–`Level_52`) from `EditorBuildSettings`, displaying per-level best times, star thresholds, `Diamond` (`0–3`) / `Atom` (`0–16`) progress from `PlayerPrefs`, and direct level launch.
