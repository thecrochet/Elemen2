# Design rules (gameplay only; tuning numbers live in code)

Status tags: [LOCKED] decided, do not change · [PROPOSED] waiting for the user · [OPEN] undecided, ask · [BUILT] exists in project · [PLANNED] not built.

## Pillars [LOCKED]
1. Runs perfectly smoothly.
2. Death to running again in about 0.5 s; no loading, no menus; retries are identical.
3. Controls always tight: snappy, never floaty. Difficulty comes from level design and hazards, never sluggish input.
4. Many well-crafted levels.
Readability beats beauty. Elemental Mastery, controls keep momentum, 1 second respawns.

## Player and powers
- 2.5D: 3D art, Z locked. The player is a rolling ball. [LOCKED]
- Ball starts as dark Aether. Base (gold electricity look) is the neutral state given by a Base gate in the tutorial. [LOCKED]
- PowerUpType order: Fire0, Air1, Earth2, Water3, None4, Aether5. [LOCKED]
- Fire = dash (breaks blocks). Earth = ground pound (higher bounce on bounce pads). Air = double jump + glide. Water = wall jump (the existing one) + gravity flip. [LOCKED]
- World order is fixed: Fire, Earth, Air, Water. Each world can use every earlier power. [LOCKED]
- Beating a boss grants its power forever. Powers accumulate; there is no selector button. [LOCKED, PLANNED in code]
- Today's code still works the old way: the last element gate crossed replaces the power. [BUILT] The accumulate system is [PLANNED]; do not build on it unless a task says so.
- A gate does not have to sit at the start of a level. A level may let the player platform without the power, find a gate, and need it afterwards; put the Checkpoint about 5 units after the gate. [LOCKED]
- Final level uses all four powers at once; nothing special is granted. [LOCKED]
- Time Attack uses the powers of that world's era (ranked loadout). Free mode: all unlocked powers, no timer, no stars; collectibles save only if the level is finished. [LOCKED]
- Aether is the hidden last power, deliberately dark. [LOCKED]
- Optional run button (Sprint), toggleable. Sprint trail must be subtle. [BUILT]

## Level rules [LOCKED]
- Levels are 30–45 s, one continuous space, one new concept per level, max one checkpoint.
- Levels end with a flat safe zone before the portal.
- Control prompts only when a new input appears.
- Stars depend only on finish time (0–3). Early levels need a challenge or secret route for 3 stars.
- World 1 (Fire): tutorial + 20 levels. 1–7 teach, 8–13 test, 14–19 master, 20 boss. Dash is introduced at 1-6.
- Collectibles: hidden Diamond (some levels only), Atom (2.5D levels). No coins. Diamonds unlock ball skins, Atoms unlock lore notes. Place them out of reach of the ranked path. [LOCKED]
- Stars unlock secret levels and a secret world, not the main path. Each world opens when the previous boss is beaten. [PROPOSED]

## Level objects
- Built: Hazard, RotatingSaw, SpinArm, StompBlock, Cannon, FireJet, LavaLeaper, BouncePad, DestructibleObject, CrumblingBlock, MovingPlatform, OneWayPlatform, IceSurface, DashZone (modes Hint, Boost, Lane, Charge, SuperDash), ElementGate, Checkpoint, CameraZone, Diamond, Atom, LevelStart, LevelGoal, TutorialSign.
- Dash zone: chevron arrows; Boost accelerates while the player keeps pushing that way; SuperDash makes the next dash about 2x farther (pulsing ball, no persistent direction state).
- Destructibles show worn neon as a hint; cracks are symmetric fissures, max 3 stages.
- No saw tooth variants. Stomp block kept for Earth.
- Visual rule: no idle particles on level objects; short bursts only. FireJet and LavaLeaper may show warning signs.
- DashWall is [BUILT] (prefab plus Dash only, Pound only, Tough and weak variants in Prefabs/Level/Platforms). Planned per world: Fire (FireJet, dash rings, lava flames), Earth (ground pound block, crumbling, stomp, bounce pad), Air and Water objects are [OPEN].
- Planned objects, not built: recharge orb (refills dash, double jump, flip; returns after about 2 s), Earth one-shot bounce pad. [PLANNED]

## Bosses [LOCKED names, PLANNED builds]
- World 1 Fire OVERHEAT.SYS · World 2 Earth DEFENDER.BIN · World 3 Air FAN.DRV (alternate name [OPEN]) · World 4 Water CACHE_PURGE.BAT · final BSOD.SYS.
- Each boss is a system failure to survive, built as a normal level (same LevelManager, timer, stars and 0.5 s respawn; no boss system). Wind makes movement harder through an outside force while controls stay snappy.
- Presented as a full-screen system alert card, never the word BOSS. [PLANNED]
