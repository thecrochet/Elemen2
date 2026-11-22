# Elemento Prefab Index & Scene Essentials

Source of truth for all level prefabs and scene setup. Never search the project; use these exact paths.
All gameplay sits at Z = 0 on whole numbers. NeonBlock Size is in cells: world size = `Size * CellSize`.
Anchor TopLeft: pivot is top-left corner (position Y = walking surface). Anchor Center: pivot is center.

## Scene Essentials (TEMPLATE_SCENE Reference)
TEMPLATE_SCENE (`Assets/Scenes/Levels/Tutorial/LEVEL_TEMPLATE.unity`) contains only 5 root objects:
1. `LevelCore` (`Assets/Prefabs/Managers/LevelCore.prefab`): contains all singletons (LevelManager, RespawnManager, Vfx, Sfx [AudioListener], LevelUI [EventSystem, HUD, PauseMenu, FinishPanel, ScreenFade], DebugOverlay, KillPits, Player [CC_ELEMENTO], CameraRig [CinemachineCamera, Player Camera]).
2. `StartPortal` (`Assets/Prefabs/LEVEL/Flow/StartPortal.prefab`): entry portal + child `SpawnPoint`.
3. `End_Portal` (`Assets/Prefabs/LEVEL/Flow/End_Portal.prefab`): exit portal + child `FX_Goal`.
4. `LevelFrame_40x14` (`Assets/Prefabs/LEVEL/Foundation/LevelFrame_40x14.prefab`): boundary frame for 40x14 level.
5. `ControlPrompt` (`Assets/Prefabs/UI/ControlPrompt.prefab`): world-space tutorial prompt trigger.

- Lighting & Skybox: RenderSettings uses Default-Skybox, Skybox ambient mode (intensity 1.0), Fog disabled. NO Directional Light exists in the scene; levels are dark with pitch-black backgrounds.
- Camera & Neon Glow: Camera in `CameraRig` has Clear Flags = SolidColor (Black `0,0,0,0`) and Post-Processing enabled. Neon glow is produced by ELEMENTO HDR emissive materials blooming under URP post-processing.
- Global Volume: For custom world bloom/color, use `Assets/Prefabs/Worlds/World1_Fire/World1_VolumeProfile.prefab` (profile: `Assets/Settings/ELEMENTO_NEON.asset`).
- Per-Level Scene Wiring:
  - Wire `StartPortal/SpawnPoint` -> `LevelCore > RespawnManager > Spawn Point`.
  - Position `Player` at `StartPortal > Spawn Point` (Z = 0).
  - Set `LevelCore > LevelManager > Next Scene Name` to the destination scene.
  - Wire `End_Portal > LevelGoal > FinishEffect` to its child `FX_Goal` ParticleSystem.

## Core & Flow
| Name | Exact Asset Path | Key Components | NeonBlock | Wiring / Empty Slots |
| :--- | :--- | :--- | :--- | :--- |
| LevelCore | `Assets/Prefabs/Managers/LevelCore.prefab` | LevelManager, RespawnManager, Vfx, Sfx, Player, CameraRig, LevelUI | - | `RespawnManager.SpawnPoint` (wire StartPortal), `NextSceneName` |
| StartPortal | `Assets/Prefabs/LEVEL/Flow/StartPortal.prefab` | LevelStart, ParticleSystem (FX_Start) | - | Child `SpawnPoint` wires to LevelCore |
| End_Portal | `Assets/Prefabs/LEVEL/Flow/End_Portal.prefab` | NeonPortalTunnel, LevelGoal, LevelExitSequence | - | `LevelGoal.FinishEffect` (wire child FX_Goal) |
| CheckPoint | `Assets/Prefabs/LEVEL/Flow/CheckPoint.prefab` | Checkpoint, BoxCollider (Trigger), ParticleSystem | - | Pre-wired (RespawnPoint & ActivateEffect) |
| CameraZone | `Assets/Prefabs/LEVEL/Flow/CameraZone.prefab` | CameraZone, BoxCollider (Trigger) | - | None (CameraDistance 40, BlendTime 0.8) |
| CameraZone_Climb | `Assets/Prefabs/LEVEL/Flow/Variants/CameraZone_Climb.prefab` | CameraZone, BoxCollider (Trigger) | - | None (ChangeDistance=0, keeps current zoom) |
| CameraZone_Tight | `Assets/Prefabs/LEVEL/Flow/Variants/CameraZone_Tight.prefab` | CameraZone, BoxCollider (Trigger) | - | None (CameraDistance 24, BlendTime 0.5) |
| CameraZone_Wide | `Assets/Prefabs/LEVEL/Flow/Variants/CameraZone_Wide.prefab` | CameraZone, BoxCollider (Trigger) | - | None (CameraDistance 48, BlendTime 1.2) |
| TutorialSign | `Assets/Prefabs/LEVEL/Flow/TutorialSign.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None (set Message string per instance) |
| TutorialSIgn_Dash | `Assets/Prefabs/LEVEL/Flow/Variants/TutorialSIgn_Dash.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None ("Press Dash to boost through") |
| TutorialSIgn_Glide | `Assets/Prefabs/LEVEL/Flow/Variants/TutorialSIgn_Glide.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None ("Hold Jump to Glide") |
| TutorialSIgn_Gravity | `Assets/Prefabs/LEVEL/Flow/Variants/TutorialSIgn_Gravity.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None ("Press Flip to reverse gravity") |
| TutorialSIgn_GroundPound | `Assets/Prefabs/LEVEL/Flow/Variants/TutorialSIgn_GroundPound.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None ("Press GroundPound in air") |
| TutorialSIgn_Jump | `Assets/Prefabs/LEVEL/Flow/Variants/TutorialSIgn_Jump.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None ("Press Jump to leap") |
| TutorialSign_Run | `Assets/Prefabs/LEVEL/Flow/Variants/TutorialSign_Run.prefab` | TutorialSign, TextMeshPro, BoxCollider (Trigger) | - | None ("Move with Left Stick / WASD") |
| Diamond | `Assets/Prefabs/LEVEL/Flow/Diamond.prefab` | NeonShapeMesh, Diamond, SphereCollider, ParticleSystem | - | None (set Id per instance) |
| Atom | `Assets/Prefabs/LEVEL/Flow/Atom.prefab` | Atom, SphereCollider (Trigger), ParticleSystem | - | None (set Id per instance) |
| SecretDoor | `Assets/Prefabs/LEVEL/Flow/SecretDoor.prefab` | Transform (scene marker) | - | None |
| ControlPrompt | `Assets/Prefabs/UI/ControlPrompt.prefab` | ControlPrompt, Canvas, TextMeshProUGUI, BoxCollider | - | None (world-space button prompt) |

## Foundation
| Name | Exact Asset Path | Key Components | NeonBlock | Wiring / Empty Slots |
| :--- | :--- | :--- | :--- | :--- |
| Block | `Assets/Prefabs/LEVEL/Foundation/Block.prefab` | MeshFilter, MeshRenderer (visual only) | - | None |
| Block_1x1 | `Assets/Prefabs/LEVEL/Foundation/Block_1x1.prefab` | NeonBlock, BoxCollider | Cell: 0.5 \| TopLeft \| Size: (1, 1, 1) | None |
| Block_Cyan | `Assets/Prefabs/LEVEL/Foundation/Variants/Block_Cyan.prefab` | NeonBlock, BoxCollider (Cyan mat) | Cell: 0.5 \| TopLeft \| Size: (1, 1, 1) | None |
| Block_Green Variant | `Assets/Prefabs/LEVEL/Foundation/Variants/Block_Green Variant.prefab` | NeonBlock, BoxCollider (Green mat) | Cell: 0.5 \| TopLeft \| Size: (1, 1, 1) | None |
| Block_Magenta Variant | `Assets/Prefabs/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab` | NeonBlock, BoxCollider (Magenta mat) | Cell: 0.5 \| TopLeft \| Size: (1, 1, 1) | None |
| Ceiling_20x1 | `Assets/Prefabs/LEVEL/Foundation/Ceiling_20x1.prefab` | NeonBlock, BoxCollider | Cell: 1 \| TopLeft \| Size: (20, 1, 2) | None |
| Ground_20x1 | `Assets/Prefabs/LEVEL/Foundation/Ground_20x1.prefab` | NeonBlock, BoxCollider | Cell: 1 \| TopLeft \| Size: (20, 1, 2) | None |
| KillPit | `Assets/Prefabs/LEVEL/Foundation/KillPit.prefab` | Hazard, BoxCollider, ParticleSystem | - | None |
| Ledge_8x1 | `Assets/Prefabs/LEVEL/Foundation/Ledge_8x1.prefab` | NeonBlock, BoxCollider | Cell: 0.5 \| TopLeft \| Size: (8, 1, 4) | None |
| LevelFrame_40x14 | `Assets/Prefabs/LEVEL/Foundation/LevelFrame_40x14.prefab` | Compound (Ceiling/Ground 42x1, Walls 1x14) | Cell: 1 \| TopLeft \| Size: (42, 14, 2) | None |
| Slope_45 | `Assets/Prefabs/LEVEL/Foundation/Slope_45.prefab` | NeonShapeMesh, BoxCollider, MeshCollider | - | None |
| Slope_45_Large | `Assets/Prefabs/LEVEL/Foundation/Variants/Slope_45_Large.prefab` | NeonShapeMesh (scale 2), BoxCollider, MeshCollider | - | None |
| Slope_45_Left | `Assets/Prefabs/LEVEL/Foundation/Variants/Slope_45_Left.prefab` | NeonShapeMesh (facing left), BoxCollider | - | None |
| Wall_1x12 | `Assets/Prefabs/LEVEL/Foundation/Wall_1x12.prefab` | NeonBlock, BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 12, 2) | None |

## Platforms
| Name | Exact Asset Path | Key Components | NeonBlock | Wiring / Empty Slots |
| :--- | :--- | :--- | :--- | :--- |
| CrumblingBlock | `Assets/Prefabs/LEVEL/Platforms/CrumblingBlock.prefab` | NeonBlock, CrumblingBlock, BoxCollider, ParticleSystem | Cell: 1 \| Center \| Size: (2, 1, 2) | None |
| DashWall | `Assets/Prefabs/LEVEL/Platforms/DashWall.prefab` | NeonBlock, BoxCollider | Cell: 1 \| Center \| Size: (1, 1, 1) | None |
| IceSurface | `Assets/Prefabs/LEVEL/Platforms/IceSurface.prefab` | NeonBlock, IceSurface, BoxCollider | Cell: 1 \| TopLeft \| Size: (6, 1, 2) | None |
| MovingPlatform 1 | `Assets/Prefabs/LEVEL/Platforms/MovingPlatform 1.prefab` | NeonBlock, PhysicsMover, MovingPlatform, Rigidbody | Cell: 1 \| Center \| Size: (4, 1, 2) | None (Travel, Duration, Delay) |
| OneWayPlatform | `Assets/Prefabs/LEVEL/Platforms/OneWayPlatform.prefab` | NeonBlock, BoxCollider | Cell: 0.5 \| TopLeft \| Size: (8, 1, 4) | None |
| OneWayPlatform_Large | `Assets/Prefabs/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab` | NeonBlock, BoxCollider | Cell: 0.5 \| TopLeft \| Size: (16, 1, 4) | None |
| OneWayPlatform_Short | `Assets/Prefabs/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab` | NeonBlock, BoxCollider | Cell: 0.5 \| TopLeft \| Size: (4, 1, 4) | None |

## Hazards
| Name | Exact Asset Path | Key Components | NeonBlock | Wiring / Empty Slots |
| :--- | :--- | :--- | :--- | :--- |
| Cannon | `Assets/Prefabs/LEVEL/Hazards/Cannon.prefab` | Hazard, Cannon, NeonShapeMesh, BoxCollider, ParticleSystem | - | None |
| Cannon_Fast | `Assets/Prefabs/LEVEL/Hazards/Variants/Cannon_Fast.prefab` | Hazard, Cannon (FireInterval 1.5), BoxCollider | - | None |
| Cannon_Up | `Assets/Prefabs/LEVEL/Hazards/Variants/Cannon_Up.prefab` | Hazard, Cannon (Upward), BoxCollider | - | None |
| FireJet | `Assets/Prefabs/LEVEL/Hazards/FireJet.prefab` | FireJet, Hazard, BoxCollider, ParticleSystem | - | None (FireDuration, RestDuration, Delay) |
| FireJet_Down | `Assets/Prefabs/LEVEL/Hazards/Variants/FireJet_Down.prefab` | FireJet (Down), Hazard, BoxCollider, ParticleSystem | - | None |
| FireJet_Left | `Assets/Prefabs/LEVEL/Hazards/Variants/FireJet_Left.prefab` | FireJet (Left), Hazard, BoxCollider, ParticleSystem | - | None |
| FireJet_Long | `Assets/Prefabs/LEVEL/Hazards/Variants/FireJet_Long.prefab` | FireJet (Length 6), Hazard, BoxCollider, ParticleSystem | - | None |
| FireJet_Right | `Assets/Prefabs/LEVEL/Hazards/Variants/FireJet_Right.prefab` | FireJet (Right), Hazard, BoxCollider, ParticleSystem | - | None |
| LavaLeaper | `Assets/Prefabs/LEVEL/Hazards/LavaLeaper.prefab` | LavaLeaper, Hazard, SphereCollider, Rigidbody | - | None (LeapHeight, RestTime, Delay) |
| LavaLeaper_Ceiling | `Assets/Prefabs/LEVEL/Hazards/Variants/LavaLeaper_Ceiling.prefab` | LavaLeaper (Ceiling), Hazard, SphereCollider | - | None |
| LavaLeaper_Fast | `Assets/Prefabs/LEVEL/Hazards/Variants/LavaLeaper_Fast.prefab` | LavaLeaper (Rest 1s), Hazard, SphereCollider | - | None |
| LavaLeaper_Tall | `Assets/Prefabs/LEVEL/Hazards/Variants/LavaLeaper_Tall.prefab` | LavaLeaper (Height 12), Hazard, SphereCollider | - | None |
| Saw | `Assets/Prefabs/LEVEL/Hazards/Saw.prefab` | Hazard, RotatingSaw, NeonShapeMesh, SphereCollider | - | None (Speed 180) |
| Saw_Fast | `Assets/Prefabs/LEVEL/Hazards/Variants/Saw_Fast.prefab` | Hazard, RotatingSaw (Speed 360), SphereCollider | - | None |
| Saw_Slow | `Assets/Prefabs/LEVEL/Hazards/Variants/Saw_Slow.prefab` | Hazard, RotatingSaw (Speed 90), SphereCollider | - | None |
| Spike | `Assets/Prefabs/LEVEL/Hazards/Spike.prefab` | NeonShapeMesh, Hazard, BoxCollider, ParticleSystem | - | None |
| Spike_Down | `Assets/Prefabs/LEVEL/Hazards/Variants/Spike_Down.prefab` | NeonShapeMesh, Hazard (Down), BoxCollider | - | None |
| Spike_Left | `Assets/Prefabs/LEVEL/Hazards/Variants/Spike_Left.prefab` | NeonShapeMesh, Hazard (Left), BoxCollider | - | None |
| Spike_Right | `Assets/Prefabs/LEVEL/Hazards/Variants/Spike_Right.prefab` | NeonShapeMesh, Hazard (Right), BoxCollider | - | None |
| Spike_Pyramid Variant | `Assets/Prefabs/LEVEL/Hazards/Variants/Spike_Pyramid Variant.prefab` | NeonShapeMesh (Pyramid), Hazard, BoxCollider | - | None |
| SpikeRow_6 | `Assets/Prefabs/LEVEL/Hazards/Variants/SpikeRow_6.prefab` | NeonShapeMesh (6 spikes), Hazard, BoxCollider | - | None |
| SpikeRow_6_Pyramid | `Assets/Prefabs/LEVEL/Hazards/Variants/SpikeRow_6_Pyramid.prefab` | NeonShapeMesh (6 pyramids), Hazard, BoxCollider | - | None |
| SpikeRow_Down | `Assets/Prefabs/LEVEL/Hazards/Variants/SpikeRow_Down.prefab` | NeonShapeMesh, Hazard (Row Down), BoxCollider | - | None |
| SpikeRow_Left | `Assets/Prefabs/LEVEL/Hazards/Variants/SpikeRow_Left.prefab` | NeonShapeMesh, Hazard (Row Left), BoxCollider | - | None |
| SpikeRow_Right | `Assets/Prefabs/LEVEL/Hazards/Variants/SpikeRow_Right.prefab` | NeonShapeMesh, Hazard (Row Right), BoxCollider | - | None |
| SpinArm | `Assets/Prefabs/LEVEL/Hazards/SpinArm.prefab` | NeonShapeMesh, Hazard, SpinArm, BoxCollider, Rigidbody | - | `Hazard.HitEffect` (optional) |
| StompBlock | `Assets/Prefabs/LEVEL/Hazards/StompBlock.prefab` | StompBlock, Hazard, PhysicsMover, BoxCollider, Rigidbody | - | None (DropDistance, RiseTime, WaitTime) |

## Interactables
| Name | Exact Asset Path | Key Components | NeonBlock | Wiring / Empty Slots |
| :--- | :--- | :--- | :--- | :--- |
| BouncePad | `Assets/Prefabs/LEVEL/Interactables/BouncePad.prefab` | BouncePad, NeonShapeMesh, BoxCollider, ParticleSystem | - | None (BounceForce 16) |
| BouncePad_Mega | `Assets/Prefabs/LEVEL/Interactables/Variants/BouncePad_Mega.prefab` | BouncePad (BounceForce 22), NeonShapeMesh, BoxCollider | - | None |
| BouncePad_Mini | `Assets/Prefabs/LEVEL/Interactables/Variants/BouncePad_Mini.prefab` | BouncePad (BounceForce 12), NeonShapeMesh, BoxCollider | - | None |
| CrackedBlocks_4x4 | `Assets/Prefabs/LEVEL/Interactables/CrackedBlocks_4x4.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| TopLeft \| Size: (2, 2, 2) | `DestructibleObject.CrackRenderer` (optional) |
| CrackBlock | `Assets/Prefabs/LEVEL/Interactables/Variants/CrackBlock.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashBoost | `Assets/Prefabs/LEVEL/Interactables/DashBoost.prefab` | DashZone (Mode: Boost), BoxCollider (Trigger), ParticleSystem | - | None |
| DashBoost_Fast | `Assets/Prefabs/LEVEL/Interactables/Variants/DashBoost_Fast.prefab` | DashZone (Boost, Speed 32), BoxCollider (Trigger) | - | None |
| DashBoost_Left | `Assets/Prefabs/LEVEL/Interactables/Variants/DashBoost_Left.prefab` | DashZone (Boost, Dir Left), BoxCollider (Trigger) | - | None |
| DashBoost_Long | `Assets/Prefabs/LEVEL/Interactables/Variants/DashBoost_Long.prefab` | DashZone (Boost, Length 3), BoxCollider (Trigger) | - | None |
| DashBoost_Up | `Assets/Prefabs/LEVEL/Interactables/Variants/DashBoost_Up.prefab` | DashZone (Boost, Dir Up), BoxCollider (Trigger) | - | None |
| DashCharge | `Assets/Prefabs/LEVEL/Interactables/DashCharge.prefab` | DashZone (Mode: Charge), BoxCollider (Trigger), ParticleSystem | - | None |
| DashHold | `Assets/Prefabs/LEVEL/Interactables/DashHold.prefab` | DashZone (Mode: Hold), BoxCollider (Trigger), ParticleSystem | - | None |
| DashLane_UP | `Assets/Prefabs/LEVEL/Interactables/DashLane_UP.prefab` | DashZone (Mode: Lane, Dir Up), BoxCollider (Trigger) | - | None |
| DashWall | `Assets/Prefabs/LEVEL/Interactables/DashWall.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashWall_ChainRow 1 | `Assets/Prefabs/LEVEL/Interactables/DashWall_ChainRow 1.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashWall_Cracked | `Assets/Prefabs/LEVEL/Interactables/DashWall_Cracked.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| Center \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashWall_DashOnly | `Assets/Prefabs/LEVEL/Interactables/Variants/DashWall_DashOnly.prefab` | NeonBlock, DestructibleObject (Dash source only), BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashWall_PoundOnly | `Assets/Prefabs/LEVEL/Interactables/Variants/DashWall_PoundOnly.prefab` | NeonBlock, DestructibleObject (Pound source only), BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashWall_Tough | `Assets/Prefabs/LEVEL/Interactables/Variants/DashWall_Tough.prefab` | NeonBlock, DestructibleObject (Hits Required: 2), BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DasWall_weak | `Assets/Prefabs/LEVEL/Interactables/Variants/DasWall_weak.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| Center \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
| DashZone | `Assets/Prefabs/LEVEL/Interactables/DashZone.prefab` | DashZone, BoxCollider (Trigger) | - | `DashZone.BoostEffect` (optional) |
| ElementGate_Base | `Assets/Prefabs/LEVEL/Interactables/ElementGate_Base.prefab` | ElementGate (Power: None), BoxCollider (Trigger) | - | None |
| ElementGate _Fire | `Assets/Prefabs/LEVEL/Interactables/Variants/ElementGate _Fire.prefab` | ElementGate (Power: Fire=0), BoxCollider (Trigger) | - | None |
| ElementGate_Aether | `Assets/Prefabs/LEVEL/Interactables/Variants/ElementGate_Aether.prefab` | ElementGate (Power: Aether=5), BoxCollider (Trigger) | - | None |
| ElementGate_Air | `Assets/Prefabs/LEVEL/Interactables/Variants/ElementGate_Air.prefab` | ElementGate (Power: Air=1), BoxCollider (Trigger) | - | None |
| ElementGate_Earth | `Assets/Prefabs/LEVEL/Interactables/Variants/ElementGate_Earth.prefab` | ElementGate (Power: Earth=2), BoxCollider (Trigger) | - | None |
| ElementGate_Water | `Assets/Prefabs/LEVEL/Interactables/Variants/ElementGate_Water.prefab` | ElementGate (Power: Water=3), BoxCollider (Trigger) | - | None |
| GroundPoundChainRow 2 | `Assets/Prefabs/LEVEL/Interactables/GroundPoundChainRow 2.prefab` | NeonBlock, DestructibleObject, BoxCollider | Cell: 1 \| TopLeft \| Size: (1, 1, 1) | `DestructibleObject.CrackRenderer` (optional) |
