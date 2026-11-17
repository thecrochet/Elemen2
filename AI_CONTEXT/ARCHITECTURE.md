# Architecture (snapshot Nov 2026, built from the audit; verify before relying)

Scripts live under `Assets/Scripts/`. Legacy folders and KCC are out of scope.

## Core managers (children of the LevelCore prefab, one per scene; standalone GameManager, LevelManager and RespawnManager prefabs also exist in Prefabs/Managers)
- `Core/LevelManager` (IResettable): level clock, best time, level finish, loads next scene (by name or build index).
- `Core/RespawnManager`: respawn after 0.25 s, kill floor Y -25 / kill top Y 75, commits/resets every IResettable. Also defines `IResettable`.
- `Core/Vfx`: plays `FX_<VfxType>` children by enum (one-shots, loops, ambients, tinted, collect burst). Keep one per scene.
- `Core/Sfx`: 34 SfxType events (verified) with fallback to PowerUp sound.
- `Debug/DebugOverlay`: editor debug display.
- `PowerUpType` (enum, order fixed). `PowerUpPickup.cs` is fully commented out (dead file).

## Player
- `Player/CC_ELEMENTO`: the KCC character controller (about 1,700 lines; never read whole). Includes Boost momentum, Slope Climb (wall run: needs speed >= ClimbMinSpeed 17 and the run must come off a slope of >= 20 degrees within 0.3 s; fields start with `Climb`) and the Dash Hold barrel (HandleHold / EnterHold / ReleaseHold). States: Default, Dashing, GroundPound, Dead, Finished, Held (appended last; never reorder). Moves, jump, double jump, dash, ground pound, wall jump, glide, gravity flip.
- `Player/BallVisuals`: ball material and effects per power.
- Camera: `Camera/CameraZoneBlender` blends Cinemachine settings between CameraZones.

## Level flow
- `LevelStart` spawn and start frame · `Checkpoint` · `LevelGoal` (finish) + `LevelExitSequence` (portal tunnel, fade, finish panel) · `CameraZone` · `DashZone` · `TutorialSign` · `Diamond` / `Atom` (collectibles, IResettable). `Bandage.cs` is still a live, working script (key `Bandage_<scene>_<id>`) but has no prefab and is replaced by Diamond in the design.
## Hazards
- `Hazard` (base kill trigger) · `RotatingSaw` · `SpinArm` · `StompBlock` (IMoverController) · `Cannon` · `FireJet` · `LavaLeaper`.
## Interactables and platforms
- `Flow/DashZone`: ZoneMode order Hint, Boost, Lane, Charge, SuperDash, Hold (append only). Hold = barrel: the ball hovers, Jump throws it along the zone arrow (26 u/s for 0.25 s), auto-throw after HoldMaxTime 4 s. Zones can move or chain. Direction = transform.right; use the Size field, never scale the root.
- `BouncePad` · `DestructibleObject` (HitSource enum: dash, ground pound) · `ElementGate` (gives a power, plays collect burst) · `CrumblingBlock` · `IceSurface` · `MovingPlatform` (IMoverController) · `OneWayPlatform`.
## Builders (Editor/create-menu helpers, `Level/Builders/`)
- `ElementoPrefabBuilders` (ElementoMaterials and prefab builders) · `ElementoUIBuilders` (level UI, control prompt) · `NeonBlock` (anchor Center/Left/TopLeft) · `NeonShapeMesh` · `NeonPortalTunnel`.
## UI
- `UI/LevelHUD` (timer, attempts) · `UI/PauseMenu` (resume, restart, quit; reloads scene) · `UI/ScreenFade` (alpha 1 on Awake at runtime) · `UI/ControlPrompt` (world-space prompt trigger).

## Scene flow
`TEMPLATE_SCENE` top level has only: StartPortal, FX_Start, PrefabLab, LevelCore, End_Portal, LevelFrame_40x14, ControlPrompt. **Player, Vfx, Sfx, CameraRig, LevelUI (HUD, PausePanel, FinishPanel, ScreenFade, EventSystem), KillPit, KillPit2, FX_FinalFrame are children inside the LevelCore prefab**, together with RespawnManager, LevelManager and DebugOverlay.
LevelGoal -> LevelExitSequence -> LevelManager.LoadNextLevel: loads NextSceneName if set, otherwise the next build index. **After the last scene it wraps back to scene 0** (meant for a hub or menu later; it does not stop).

## Hub (prefabs exist, mostly unwired; scripts not audited)
Prefabs/Hub: BossArena, ElementPedestal, HubFloor, HubManager, HubPlatform, HubSpawnPoint, HubWall, LevelPortal, StarCounter, WorldGate. Also Player_hub, Camera/HubCamera, Camera/CameraRigX. Treat the hub as unfinished; ask before building on it.

## Worlds
Prefabs/Worlds/World1_Fire holds World1_Skyline and World1_VolumeProfile.

## Save keys (PlayerPrefs)
- `BestTime_<sceneName>` (float, -1 = none).
- `Diamond_<sceneName>_<id>`, `Atom_<sceneName>_<id>`, Bandage pattern (int 1 = collected).
No other save data exists yet.

## Layers and input
- Layers: Player 3, Destructible 6, Hazard 7, Interactable 8. No tags. Collision matrix is all ticked.
- Input map Gameplay (CC_ELEMENTO's Gravity Flip Action is linked to Gameplay/Flip; Glide Action is empty and falls back to the jump button): Move, Jump, Dash, GroudPound (sic), Sprint, Flip, Interact, Attack, Look, Restart, PauseGame. UI map: standard + UnpauseGame.
- Camera: Cinemachine Position Composer, distance 32, focal length 32, Brain on Player Camera.
