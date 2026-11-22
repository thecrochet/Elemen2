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
  - `CC_ELEMENTO.cs`: KinematicCharacterController implementation. Movement, Jump, Double Jump, Dash, Ground Pound, Wall Jump, Glide, Gravity Flip. Native Purple Ball (Base/Aether) has full powers unlocked by default: `defaultDash=true`, `defaultDoubleJump=true`, `defaultWallJump=true`, `defaultGroundPound=true`, `defaultGlide=true` (`glideNeedsNewPress=false`).
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
  - `BreakableWallFeedback.cs`: Particle, hitstop, and debris spawning feedback for destructible blocks.
  - `DebrisKickable.cs`: Physics debris interaction with one-way nudge and auto-cleanup.
- **`Level/Builders/` & `Flow/`**:
  - `FinalPurpleLevelBuilder.cs`: Campaign level generation and scene assembly tool.
  - `ElementoFinalPurpleSync.cs`: Sequential level linking and EditorBuildSettings sync tool.
  - `BlueBlockPrefabCreator.cs`: Touch block authoring, hitstop configuration, ability restoration, and batch validator.
  - `ElementoPrefabBuilders.cs` & `ElementoUIBuilders.cs`: Editor creation tools.
  - `NeonBlock.cs`, `NeonShapeMesh.cs`, `NeonPortalTunnel.cs`: Procedural neon mesh construction.
  - `IntroSequence.cs` & `GridReveal.cs`: Tutorial intro sequence, Made with Unity logo fade, and shader grid reveal (`_ElementoGridHide`).
- **`Debug/`**:
  - `ElementoLevelValidator.cs`: 11-rule automated level health validator (`Elemento > Validate Current Level`).
  - `PlayerLab.cs`: Runtime power mixer (keys 1-6) and jump reach stats sheet (F5 export).
  - `DebugOverlay.cs` & `ElementoParticleDump.cs`: Performance overlay and VFX auditor.
- **`UI/`**: `LevelHUD.cs` (timer, stars, attempts), `PauseMenu.cs`, `ScreenFade.cs`, `ControlPrompt.cs`, `NeonWaveButton.cs`.

## 3. Core Prefab Hierarchy & Scene Architecture
- **7-Container Scene Standard**: Every playable level strictly adheres to 7 root containers aligned at $Z = 0$:
  1. `LevelCore`: Central per-level managers, camera rig, player motor, UI canvas, and kill boundaries.
  2. `Environment`: Static foundations (`Ground_20x1`, `Ledge_8x1`, `Wall_1x12`, `Slope_45`, `Slope_45_Large`), moving platforms, crumbling bridges, ice surfaces, one-way ledges, and level frames.
  3. `Interactables`: Yellow breakable blocks (`DashWall`, `CrackedBlocks_4x4`), Purple touch blocks (`TouchWall_Blue`), bounce pads (`BouncePad`, `BouncePad_Mini`, `BouncePad_Mega`), and chevron speed lanes (`DashBoost_Fast`, `DashZone`, `DashLane_UP`).
  4. `Hazards`: Rotating saws (`Saw_Slow`, `Saw_Fast`), spinning arms (`SpinArm`), cannons (`Cannon`, `Cannon_Fast`), fire jets (`FireJet`), stompers (`StompBlock`), and magma leapers (`LavaLeaper`).
  5. `Collectibles`: `Diamond` (3 per level with unique scene IDs 1–3) and `Atom` (16 per level with unique scene IDs 1–16).
  6. `Checkpoints`: `CheckPoint` triggers (typically 3 per level) saving active state and committing `IResettable` objects.
  7. `Flow`: `StartPortal` (wired to `RespawnManager.SpawnPoint`), `End_Portal` (wired with `LevelGoal` and `LevelExitSequence`), and dynamic `CameraZone` triggers (`CameraZone_Wide`, `CameraZone_Climb`, `CameraZone_Tight`).
- **Continuous Safety Floors**: Solid foundation ledges (`Ground_20x1`, `Ledge_8x1`) line the floor beneath airborne sections, long glides, and excavation drops, guaranteeing accessible recovery and preventing frustrating softlocks or unfair void deaths.
- **Camera & Dynamic Zooms**:
  - Baseline Camera: Cinemachine 3 Position Composer configured to `CameraDistance = 54.0f` (+35% wider follow distance) providing expansive forward visibility.
  - Dynamic `CameraZone` Transitions: Seamlessly blend framing for vertical ascents (`CameraZone_Climb`), wide cavern glides (`CameraZone_Wide`), and tight supersonic dash corridors (`CameraZone_Tight`).
- **Disabled Startup Debug HUD**: Debug telemetry (`DebugOverlay.StartVisible = false`, `PlayerLab.ShowPanel = false` / `MixPowers = false`) disabled across all levels for clean presentation.
- **Save Schema (PlayerPrefs)**:
  - `BestTime_<sceneName>` (float, -1 = uncompleted).
  - `Diamond_<sceneName>_<id>` and `Atom_<sceneName>_<id>` (int 1 = collected).
  - `Unlocked_Powers` (bitmask for permanent unlocks).

## 4. Master Lists Summary
- **VFX System**:
  - **Ball Effects (22)**: 12 one-shots (`Jump`, `DoubleJump`, `WallJump`, `Land`, `DashStart`, `GroundPoundStart`, `GroundPoundImpact`, `GlideOpen`, `GravityFlip`, `PowerUpCollect`, `Respawn`, `Death`), 10 loops (`DashTrail`, `RunTrail`, `WallSlide`, `GlideStream`, 6 Ambients). `FX_AmbientAether.prefab` exists.
  - **Object Effect Slots (20)**: `FX_CheckpointActivate`, `FX_BreakBurst`, `FX_CrackHit`, `FX_PadLaunch`, `FX_PadLaunchPound`, `FX_Crumble`, `FX_StompImpact`, `FX_Finish`, `FX_GateCross`, `FX_DiamondCollect`, `FX_AtomCollect`, `FX_DashBoost`, `FX_FireJetFlame`, `FX_FireJetWarn`, `FX_LeaperTrail`, `FX_LeaperWarn`, `FX_LeaperSplash`, `FX_CannonShoot`.
- **SFX System**: 34 events in `SfxType` + procedural `SFX_StarLost.wav`. Master Sfx object manages 12 voices, distance 25, pitch jitter $\pm 0.05$.
- **Materials & Shaders**:
  - Shaders (`Assets/ART/Shaders/`): `NeonShape`, `NeonGrid`, `NeonParticle`, `CrackOverlay`, `DashZone`, `Gate/ElementGate`, `Portal/Frame`, `Portal/TunnelWall`, `Ball/*`.
  - Materials (`Assets/ART/Materials/`): 9 white `P_*` particle materials, `M_Level_*` grid blocks, `M_Shape_*` danger/interactive solids, `M_Crack`, `M_Gate_*`, `M_Portal_*`, `M_Level_Blue_Touch` (neon purple emission: `1.75, 0, 2.8`).

## 5. Dual Breakable Block System & Mechanics
- **Yellow Breakable Blocks (`DashWall.prefab`, `CrackedBlocks_4x4.prefab`)**:
  - **Breaking Conditions**: Shatter ONLY when the player impacts them under 3 explicit states:
    1. Actively manual Dashing (`isDashing == true`).
    2. Actively Earth Ground Pounding (`isGroundPounding == true`).
    3. Active Chevron Boost Pad impulse (`isBoostImpulseActive == true`) granted exclusively by golden speed impulse pads (`DashBoost_Fast`, `DashZone`, `DashLane_UP`, `DashHold`) for `boostSmashDuration = 0.5f`.
  - **Solid Wall Rule**: Normal rolling, walking, or downhill slope sliding treats Yellow blocks as impenetrable solid barriers. The player stops solidly with 0 damage to the block and 0 damage to the player.
  - **Snappy Micro-Hitstop**: Hitstop duration tuned to `0.02s` (reduced from legacy `0.045s`), delivering crisp tactile crunch without stalling momentum during multi-block smash chains.
  - **Debris**: Spawns `DebrisCube_Yellow.prefab` with low-bounce physics and gentle outward tumble.
- **Purple Touch Blocks (`TouchWall_Blue.prefab` & `DebrisCube_Blue.prefab`)**:
  - **Breaking Conditions**: Shatter immediately on any player contact, collision, or rolling without requiring Dash, Ground Pound, or speed impulse (`BreakableByTouch = true`, `HitsToBreak = 1`).
  - **Zero Hitstop (`0f`)**: Instantaneous demolition with `hitStopDuration = 0f`, preserving full rolling and motor velocity through long carpets and suspended aerial clouds.
  - **Visuals & VFX**: Assigned `M_Level_Blue_Touch.mat` recolored to glowing neon purple (`_EdgeColor: 1.75, 0, 2.8`) matching the Purple Ball's exact HDR emission hue. Emits matching neon-purple sparks, shockwaves, wisps, and explosions, spawning `DebrisCube_Blue.prefab` pieces that tumble non-destructively via `DebrisKickable.cs`.

## 6. Purple Ball Campaign (`Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/`)
The definitive 35-level campaign for World 1 Purple Ball (100% element-gate free, native Dash, Earth Ground Pound, Double Jump, Wall Jump, and Gliding all enabled by default from level start, tightened dual breakable block mechanics, and clean presentation with on-screen debug HUD overlays disabled by default):
- **Sequential Level Linking**: `LevelManager.ResolveNextSceneName()` automatically resolves `Level_XX` to `Level_(XX+1)` in `EditorBuildSettings`, supporting both explicit scene names and auto-progression.
- **Level Sync Tool (`ElementoFinalPurpleSync.cs`)**: Reusable Editor menu (`Elemento > Sync FINAL_PURPLE Levels`) that scans `FINAL_PURPLE/`, sorts levels numerically, wires each level's `End_Portal` (`LevelGoal` / `LevelExitSequence` / `LevelManager`) to the next level, and registers all levels in `EditorBuildSettings` in exact sequential order (indices 3 to 37).
- **Campaign Levels (`Level_01` to `Level_35`)**:
  - `Level_01.unity` ("Onboarding & Momentum Basics"): Warm-up, DashWall test, Chevron Boost Pad smash, twin-wall climb, dynamic camera zoom.
  - `Level_02.unity` ("Vertical Excavation"): Multi-layer sealed Ground Pound shafts into subterranean rooms ($Y = -2, -4$), Mega Bounce Pad launches, Chimney rocket.
  - `Level_03.unity` ("Kinetic Chaining"): Chained boosters, airborne DashWalls, moving platform hazard dodging, high-speed upper highway, canyon mega bounce.
  - `Level_04.unity` ("Multi-Tier Cavern"): Non-linear upper highway vs lower trench, timed FireJet and StompBlock hazards, deep subterranean vault with LavaLeaper, stepped quarry.
  - `Level_05.unity` ("World 1 Purple Finale"): Grand 4-act gauntlet combining all mechanics, triple chained boosters, 3-layer excavation labyrinth, moving platform skyway, grand 16m rocket chimney into CameraZone_Wide, and celebratory victory bridge.
  - `Level_06.unity` ("Twin-Shaft Excavation"): Alternates tall wall-jump ascents (Chimney 1 & 2) with 4-layer Ground Pound drops into box-packed subterranean chambers ($Y = -3, -4$), moving platform bridge, and stepped quarry.
  - `Level_07.unity` ("Skywalk & Sub-Tunnel"): Dual-tier split level between an elevated high-speed Chevron Boost Pad runway breaking long horizontal DashWall corridors ($Y = 10$) and deep lower underground cavern excavation ($Y = -4, -5$), reconverging at checkpoints.
  - `Level_08.unity` ("The Vertical Vault"): Multi-story zig-zag tower where player climbs up Shaft A via mini bounce pads, smashes across a box-filled upper bridge ($Y = 14$), ground-pounds down parallel Shaft B through 4 decks into Sub-Vault 1, and repeats through Rocket Spire C/D into Deep Vault 2.
  - `Level_09.unity` ("Kinetic Reactor"): Reactor core combining moving platform navigation, vertical `DashLane_UP` launches into floating mid-air `DashWall` clusters, layered Ground Pound floors over StompBlock/FireJet hazard pockets, and a triple-booster finale.
  - `Level_10.unity` ("Purple Citadel Finale"): Grand 4-act multi-tier citadel finale weaving deep underground crypt box excavation, twin-wall chimneys, dynamic camera zoom zones (`CameraZone_Wide`, `CameraZone_Climb`, close-zoom dash tunnels), a 18m rocket chimney into the grand sky citadel, and a high-speed triple-wall smash into `End_Portal`.
  - `Level_11.unity` ("Subterranean Excavation Maze"): Deep underground maze with branching caverns, hollow box vaults with hidden diamonds, `StompBlock`/`FireJet` hazard pockets, and checkerboard box floors.
  - `Level_12.unity` ("Kinetic Pinball Canyon"): Giant pinball canyon with `Slope_45_Large` momentum slides, ricochet bounce chains, mid-air suspended box clusters, and diagonal ascending box ramps.
  - `Level_13.unity` ("Glacier & Saw Foundry"): Industrial ice foundry pairing slippery `IceSurface` runways with `CrumblingBlock` bridges, vertical `MovingPlatform 1` elevators, rotating `SpinArm`/`Saw_Slow` hazards, and staggered vertical box pillars.
  - `Level_14.unity` ("The Zig-Zag Spire"): Towering vertical switchback spire ascending to $Y = 22$ with alternating chimneys, `DashLane_UP` launches, and a breathtaking 20-unit Ground Pound plummet through 5 stacked box decks down the core.
  - `Level_15.unity` ("Chaos Citadel Finale"): High-variety 4-act showcase combining rapid camera zoom shifts (`CameraZone_Wide`, `CameraZone_Climb`, tight dash-tunnel zooms), pinball spire launches, magma trench excavation, and a supersonic 4-layer archway smash into `End_Portal`.
  - `Level_16.unity` ("Cobalt Caverns & The Blue Highway"): Introducing touch-breakable blocks in a downhill rolling carpet, deep excavation drop, twin-wall chimney with touch roof, and a supersonic tunnel smash into `End_Portal`.
  - `Level_17.unity` ("Azure Abyss & Magma Trenches"): Tiered trench descents through trapdoors, pinball mega-bounce ascent into floating touch constellations, a fortified yellow vault enclosing Diamond #2, and a 16m rocket spire launch.
  - `Level_18.unity` ("The Mosaic Matrix"): Rhythmic dual-color slalom alternating touch and yellow dash columns, checkerboard shaft decks down to a subterranean cavern, moving elevator dodging rotating saws, and a supersonic mosaic expressway.
  - `Level_19.unity` ("The Sky Foundry"): Frictionless ice runway smashing through touch clouds, crumbling bridge suspended over a safe cushion pit, 16m turbine rocket tower, and parabolic bounce skywalk.
  - `Level_20.unity` ("The Dual-Chroma Citadel Finale"): Climax of World 1 Purple Ball! Triple chained supersonic booster entry, 4-tier subterranean crypt excavation ($Y = -7$), pinball spire climb, and an 18-unit rocket tower into a grand 4-layer victory archway smash into `End_Portal`.
  - `Level_21.unity` ("Purple Avalanche Descent"): Descending canyon starting at $Y = 16$, giant `Slope_45_Large` slide through a cascading 3-layer avalanche of touch blocks, stepped terraces, and subterranean gully excavation.
  - `Level_22.unity` ("Twin-Chimney Sky-Vaults"): Dual twin-wall `Wall_1x12` chimneys and `DashLane_UP` launches into suspended sky-vaults ($Y = 12$ to $16$) with checkerboard box formations, `Cannon`/`FireJet` hazards, and `CameraZoneClimb`.
  - `Level_23.unity` ("Ricochet Pinball Foundry"): Kinetic pinball foundry with horizontal/vertical `MovingPlatform 1` shuttles, diagonal ricochet bounce angles through airborne touch blocks, triple `BouncePadMega` canyon, and rotating `SpinArm`/`Saw_Slow` hazards.
  - `Level_24.unity` ("Subterranean Labyrinth"): Multi-story underground labyrinth featuring split routes (upper ice slide vs lower purple carpet), `CrumblingBlock` skyway over safe cushion pits, and dense excavation vaults hiding diamonds.
  - `Level_25.unity` ("Purple Hyper-Gauntlet"): Milestone campaign finale! High-momentum 4-act gauntlet combining supersonic dash tunnels (`CameraZoneTight`), 4-layer magma abyss drop ($Y = -7$) with `LavaLeaper`/`FireJet`, sky bridge with rotating saws, and an 18m rocket tower into a grand 4-layer victory archway smash into `End_Portal`.
  - `Level_26.unity` ("Purple Cascade Quarry"): Multi-tier excavation quarry packed with cascading clusters of Purple touch blocks and Yellow `DashWall` / `CrackedBlocks_4x4` load-bearing floors, alternating `BouncePadMega` ascents with chained Earth Ground Pound drops down to bedrock trenches.
  - `Level_27.unity` ("Twin-Shaft Velocity Tower"): Vertical switchback tower utilizing twin-wall `Wall_1x12` jump chimneys, high-speed `DashLane_UP` vertical shafts, and high-altitude `MovingPlatform 1` sky bridges lined with Purple touch blocks and guarded by `Cannon` and `FireJet` timings.
  - `Level_28.unity` ("Neon Pinball Reactor"): High-speed kinetic pinball reactor featuring diagonal `Slope_45_Large` ramps, `BouncePadMini` and `BouncePad` ricochet angles, slick `IceSurface` recovery lanes, and chevron boost pad runways blasting through alternating purple and yellow block barriers while dodging `SpinArm` hazards.
  - `Level_29.unity` ("Subterranean Crystal Vaults"): Branching underground cavern network featuring stalactite ceilings, low-friction `IceSurface` lanes, `CrumblingBlock` bridges over magma pits, and dense grids of Purple touch blocks concealing atoms and diamonds inside Yellow `DashWall` and `CrackedBlocks_4x4` vaults.
  - `Level_30.unity` ("Purple Grandmaster Finale"): Milestone 4-act campaign finale combining dynamic camera zoom shifts (`CameraZone_Wide`, `CameraZone_Climb`, tight dash-tunnel zooms), steep ascent chimneys, a multi-hazard crucible (`StompBlock`, `FireJet`, `LavaLeaper`, `SpinArm`), a massive 4-layer Ground Pound plunge down to $Y = -10$, and a supersonic 12x4 block wall smash into the glowing `End_Portal`.
  - `Level_31.unity` ("Thermal Updraft Caverns"): Built around tall `BouncePadMega` and `DashLane_UP` updraft launches that transition into long **Gliding** air-currents through suspended clouds of Purple touch blocks (`TouchWall_Blue.prefab`), punctuated by Yellow `DashWall` air-barriers requiring mid-glide air-dashes to smash through over continuous solid safety floors.
  - `Level_32.unity` ("Shatter-Core Spiral"): Multi-story vertical zig-zag complex combining twin-wall `Wall_1x12` jump chimneys, `MovingPlatform 1` elevators, and deep multi-layer Earth Ground Pound excavation shafts sealed by stacked Yellow `CrackedBlocks_4x4` and Purple `TouchWall_Blue` strata.
  - `Level_33.unity` ("Velocity Slalom Foundry"): High-speed diagonal `Slope_45_Large` and `IceSurface` momentum tracks feeding into Chevron Boost Pad (`DashBoost_Fast`) runways that blast straight through alternating Purple and Yellow block gates while weaving around `SpinArm`, `Saw_Slow`, and `Cannon_Fast` hazards.
  - `Level_34.unity` ("Echoing Vaults"): Branching subterranean excavation network with upper glide-catwalks and lower box-packed chambers (`CrumblingBlock` bridges, `OneWayPlatform` shortcuts, `FireJet` & `StompBlock` timings) concealing `Atom` and `Diamond` collectibles inside hollow Yellow `DashWall` vaults.
  - `Level_35.unity` ("Purple Apex Gauntlet"): 4-act milestone campaign finale combining dynamic camera zoom transitions (`CameraZone_Wide`, `CameraZone_Climb`, and tight dash-tunnel zooms), long glide + mid-air dash chains, heavy Purple + Yellow block excavation, mixed hazards (`LavaLeaper`, `FireJet`, `SpinArm`, `StompBlock`), and a high-speed triple-wall Chevron Boost smash into `End_Portal.prefab`.
  - Built via `Assets/Editor/FinalPurpleLevelBuilder.cs` (`Elemento > Build All 35 FINAL_PURPLE Levels` / `Build Levels 31 to 35`).

## 7. Next Planned Task
- **Pause Menu UI/UX Redesign**: Upgrade `PauseMenu.prefab` with dynamic glowing neon wave styling (`NeonWaveButton.cs`), unified color-state palette (Purple/Cyan/Gold accents), animated button pulses, clean layout styling, and gamepad navigation support.
- **Dynamic Level Select Screen**: Implement an interactive, paginated level selection screen supporting all 35 `FINAL_PURPLE` campaign levels, displaying best times, 3-star thresholds, atom/diamond collection counts, and direct scene loading.
