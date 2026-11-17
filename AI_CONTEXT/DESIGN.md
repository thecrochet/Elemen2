# Design rules (gameplay only; tuning numbers live in code)

Status tags: [LOCKED] decided, do not change · [OPEN] undecided, ask · [BUILT] exists in project · [PLANNED] not built.

## Pillars [LOCKED]
1. Runs perfectly smoothly.
2. Death to running again in about 0.5 s; no loading, no menus; retries are identical.
3. Controls always tight: snappy, never floaty. Difficulty comes from level design and hazards, never sluggish input.
4. Many well-crafted levels.
Readability beats beauty.

## Player and powers
- 2.5D: 3D art, Z locked. The player is a rolling ball with one active element at a time. [LOCKED]
- Ball starts as dark Aether. Base (electricity look) is the neutral power given by a Base gate in the tutorial. [LOCKED]
- PowerUpType order: Fire0, Air1, Earth2, Water3, None4, Aether5. [LOCKED]
- Fire = dash (breaks blocks). Earth = ground pound (higher bounce on bounce pads). Air = double jump + glide. Water = wall jump (the existing one) + gravity flip. [LOCKED]
- Element gates are run-through discs; the most recently crossed gate's power replaces the previous one. [LOCKED][BUILT]
- Aether is the hidden last power, deliberately dark. Final level gives all powers at once. [LOCKED]
- Open: whether all powers stay active outside the final level. [OPEN]
- Optional run button (Sprint), toggleable. Sprint trail must be subtle. [BUILT]

## Level rules [LOCKED]
- Levels are 30–45 s, one continuous space, one new concept per level, max one checkpoint.
- Levels end with a flat safe zone before the portal.
- Control prompts only when a new input appears.
- Stars depend only on finish time (0–3). Early levels need a challenge or secret route for 3 stars.
- World 1 (Fire): tutorial + 20 levels. 1–7 teach, 8–13 test, 14–19 master, 20 boss. Dash is introduced around level 1–6.
- Collectibles: hidden Diamond (some levels only), Atom (2.5D levels). No coins. What they unlock is [OPEN].

## Level objects
- Built: Hazard, RotatingSaw, SpinArm, StompBlock, Cannon, FireJet, LavaLeaper, BouncePad, DestructibleObject, CrumblingBlock, MovingPlatform, OneWayPlatform, IceSurface, DashZone (modes Hint, Boost, Lane, Charge, SuperDash), ElementGate, Checkpoint, CameraZone, Diamond, Atom, LevelStart, LevelGoal, TutorialSign.
- Dash zone: chevron arrows; Boost accelerates while the player keeps pushing that way; SuperDash makes the next dash about 2x farther (pulsing ball, no persistent direction state).
- Destructibles show worn neon as a hint; cracks are symmetric fissures, max 3 stages.
- No saw tooth variants. Stomp block kept for Earth.
- Visual rule: no idle particles on level objects; short bursts only. FireJet and LavaLeaper may show warning signs.
- DashWall is [BUILT] (prefab plus Dash only, Pound only, Tough and weak variants in Prefabs/Level/Platforms). Planned per world: Fire (FireJet, dash rings, lava flames), Earth (ground pound block, crumbling, stomp, bounce pad), Air and Water objects are [OPEN].

## Bosses [LOCKED names, PLANNED builds]
- DEFENDER.BIN, CACHE_PURGE.BAT, BSOD.SYS; Fire boss OVERHEAT.SYS; Air boss FAN.DRV (alternate name [OPEN]).
- Each boss is a system failure to survive, a race like other levels. Wind boss/level makes movement harder through an outside force while controls stay snappy.
- Presented as a full-screen system alert card, never the word BOSS. [PLANNED]
