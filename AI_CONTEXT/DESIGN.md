# Design Rules (Gameplay & Level Metrics)

Status tags: `[LOCKED]` decided, do not change · `[OPEN]` undecided, ask · `[BUILT]` exists in project · `[PLANNED]` not built.

## Pillars [LOCKED]
1. Runs perfectly smoothly (60+ FPS, zero per-frame allocations).
2. Death to running again in about 0.5 s; no loading, no menus; retries are identical (`IResettable`).
3. Controls always tight: snappy, never floaty. Difficulty comes from level design and hazards, never sluggish input.
4. Readability beats beauty: Square = safe ground, Triangle = danger/hazard, Hexagon = bounce pad.

## Player & Powers
- **2.5D Plane:** 3D visuals with `Z = 0` locked. `[LOCKED]`
- **Power Order (`PowerUpType`):** `Fire = 0`, `Air = 1`, `Earth = 2`, `Water = 3`, `None = 4`, `Aether = 5` (never reorder). `[LOCKED]`
- **Elemental Verbs:**
  - **Base Spark (`None`):** Neutral gold ball with electric arcs; resting state between moves. `[LOCKED][BUILT]`
  - **Dark Aether (`Aether`):** Dormant dark seed in the tutorial; returns as the hidden final power. `[LOCKED][BUILT]`
  - **Fire:** Horizontal Dash (`28 u/s`, `0.15 s`, `4.2 u` reach; breaks `DashWall` / `CrackedBlock`). `[LOCKED][BUILT]`
  - **Earth:** Ground Pound (`40 u/s` slam; breaks stacked blocks; super-launches off `BouncePad` and `EarthHoverPad`). `[LOCKED][BUILT]`
  - **Air:** Double Jump + Glide (`3 u/s` max fall speed). `[LOCKED][BUILT]`
  - **Water:** Wall Jump + Gravity Flip. `[LOCKED][BUILT]`
- **Progression & Gates:** Bosses permanently unlock powers (`Unlocked_Powers`); per-level `LevelLoadout` sets `AllowedPowers`. Crossing an `ElementGate` grants that element for the current run (resets to checkpoint/start state on death). `[LOCKED][BUILT]`
- **Ball Morph & Cues:** Ball rests in gold Base Spark, morphs into the active element shader during moves, dims when mid-air charges are spent, and flashes when recharged (landing or `RechargeOrb`). `[LOCKED][BUILT]`
- **Optional Sprint:** Toggleable Run button (`16 u/s` vs `11 u/s` walk) with subtle trail. `[BUILT]`

## Physics & Reach Ruler (Keep Standard Platforming <= 85% of Max) [LOCKED]
- **Full Jump (held):** height `3.15 u` (keep vertical steps `<= 2.7 u`), flat gap `6.1 u` (walk) / `8.9 u` (run).
- **Short Hop (tapped):** height `1.26 u`, flat gap `2.5 u` (walk) / `3.8 u` (run).
- **Fire Dash:** horizontal `4.2 u` (`28 u/s` for `0.15 s`); Jump + Apex Dash reaches `10.3 u` (walk) / `13.0 u` (run).
- **Earth Ground Pound & Bounce Pad:** Normal pad height `4.8 u` (`7.5 u` walk / `11.0 u` run); Ground-Pound pad height `11.4 u` (`11.6 u` walk / `16.9 u` run).
- **Air Double Jump & Glide:** Double jump height `6.3 u` (`10.5 u` walk / `15.3 u` run); Glide fall rate `3 u/s` (`14.9 u` walk / `21.6 u` run from jump apex).
- **Water Wall Jump:** Optimal shaft width `3 to 5 u` (`2.4 to 2.8 u` vertical gain per hop).

## Level Rules & Pacing [LOCKED]
- Levels are `30–45 s` in one continuous space, introducing one new concept per level.
- **Checkpoint Rule:** Maximum 1 checkpoint per level, placed strictly on a flat, stable floor.
- **Safety & Endings:** Line continuous solid safety floors under long aerial/glide sections to prevent unfair void drops, and end every level with a flat, obstacle-free run-up (`>= 6 u`) before `End_Portal`.
- **Control Prompts:** Show `TutorialSIgn_*` / `ControlPrompt` only when a new input appears.
- **Stars & Collectibles:** Stars (0–3) depend solely on finish time. Hidden `Diamond` (some levels) and off-route `Atom` collectibles save persistently upon level completion.
- **World 1 (Fire) Structure:** Tutorial + 20 levels (`1–7` teach, `8–13` test, `14–19` master, `20` boss). Dash is introduced at `1-6`.

## Level Objects
- **Built (`[BUILT]`):** `Hazard`, `RotatingSaw` (`Saw_Slow`, `Saw_Fast`), `SpinArm`, `StompBlock`, `Cannon`, `FireJet`, `LavaLeaper`, `BouncePad`, `EarthHoverPad`, `DestructibleObject` (`CrackedBlock`), `DashWall` (`DashWall_DashOnly`, `DashWall_Tough`, `DashWall_ChainRow`), `CrumblingBlock`, `MovingPlatform`, `OneWayPlatform`, `IceSurface`, `DashZone` (`Hint`, `Boost`, `Lane`, `Charge`, `SuperDash`), `ElementGate` (`Fire`, `Earth`, `Water`, `Air`, `Aether`, `Base`), `RechargeOrb`, `Checkpoint`, `CameraZone`, `Diamond`, `Atom`, `StartPortal`, `End_Portal`, `TutorialSIgn_*`.
- **Visual Rule:** No idle particles on static level objects; short event bursts only (`FireJet` and `LavaLeaper` may show warning effects).

## Bosses [LOCKED Names, PLANNED Builds]
- `OVERHEAT.SYS` (Fire), `DEFENDER.BIN` (Earth), `FAN.DRV` (Air, alternate name `[OPEN]`), `CACHE_PURGE.BAT` (Water), `BSOD.SYS` (Final).
- Presented as high-intensity 60–90 s survival sprints announced via full-screen **System Alert Cards** (never the word "BOSS"). `[PLANNED]`