# Elemento: Game Design & Level Design Master Reference

Comprehensive gameplay, progression, physics metrics, and narrative specification.

---

## 1. Core Design Pillars
- **Silky Performance [IMPLEMENTED]**: 60+ FPS lock, zero garbage-collection allocations during gameplay, smooth camera tracking.
- **Sub-Second Respawn [IMPLEMENTED]**: Death to running again in $\approx 0.5\text{ s}$. Instantaneous reset, zero reload screens, identical deterministic retries via `IResettable`.
- **Absolute Control & Momentum [IMPLEMENTED]**: Controls are always tight and snappy, never floaty or sluggish. Challenge derives from level hazards and spatial navigation, never input latency.
- **Readability Over Beauty [IMPLEMENTED]**: Shape language dictates gameplay function:
  - **Square**: Safe ground / walkable platform.
  - **Triangle**: Hazard / danger (Spikes, Saws, Fire).
  - **Hexagon**: Bounce pads / directional redirection.
  - **Diamond**: Rare collectible.

---

## 2. Player Controller, Powers & Progression

### 2.1 Elemental Verbs & Abilities
The player controls a rolling ball confined to the 2.5D plane ($Z = 0$).
- **Base Spark (Neutral) [IMPLEMENTED]**: Gold sphere with electrical arcs. Neutral state granted at the tutorial exit.
- **Dark Aether (Seed) [IMPLEMENTED]**: Dormant starting ball, deliberately dark and unlit.
- **Fire (Dash) [IMPLEMENTED]**: Instant horizontal dash (velocity $28\text{ u/s}$, duration $0.15\text{ s}$, distance $4.2\text{ units}$, zero gravity). Breaks destructible blocks. Cooldown $0.25\text{ s}$, one air use per landing.
- **Air (Double Jump & Glide) [IMPLEMENTED]**: Second jump at apex reaches height $6.3\text{ units}$. Holding jump glides with capped fall speed ($3\text{ u/s}$).
- **Earth (Ground Pound) [IMPLEMENTED]**: Down in mid-air initiates rapid vertical slam ($40\text{ u/s}$) with landing shockwave (radius $2.5\text{ units}$). Hits bounce pads for a super launch (height $11.4\text{ units}$).
- **Water (Wall Jump & Gravity Flip) [IMPLEMENTED]**: Wall jump pushes away at $12\text{ u/s}$ with height $2.86\text{ units}$. Gravity flip instantly accelerates toward the opposite surface ($10\text{ u/s}$).
- **Sprint [IMPLEMENTED]**: Run button accelerates from walk ($11\text{ u/s}$) to run ($16\text{ u/s}$) with a subtle speed trail.

### 2.2 Progression System (Nov 17 Specification)
- **Permanent Power Accumulation [PLANNED]** *(Prototype in `PlayerLab.cs` [IMPLEMENTED])*:
  - World order is fixed: Fire $\rightarrow$ Earth $\rightarrow$ Air $\rightarrow$ Water.
  - Beating a world boss permanently grants its power. Powers accumulate across the campaign without an in-game selector button (each power has its own distinct input).
- **Loadout Masks [PLANNED]**:
  - `UnlockedPowers` (saved bitmask in PlayerPrefs) vs `AllowedPowers` (per-level era loadout). Effective abilities = `Unlocked & Allowed`.
- **Game Modes [PLANNED]**:
  - **Ranked / Time Attack**: Uses the era's `AllowedPowers`. Timer and 3-star records saved.
  - **Free Mode**: All unlocked abilities available; no timer or stars; collectibles saved only upon level completion.
- **Dynamic Ball Morph [PLANNED]**:
  - Ball rests in gold Base Spark, morphing into the elemental shader for the duration of a move ($0.1\text{ s}$ in, $0.25\text{ s}$ out), then returns to gold.
  - Spent charge dims the gold ball; charge return triggers a bright pulse.
- **New Progression Mechanics [PLANNED]**:
  - **Recharge Orb**: Mid-air floating orb that instantly restores air charges (dash, double jump, flip). Returns after $2\text{ s}$ or respawn.
  - **One-Shot Earth Bounce Pad**: Hexagonal bounce pad that crumbles upon ground-pound launch and resets after $2\text{ s}$ or respawn.
  - **Permanent Unlock Gate**: Post-boss ceremony gate that writes to `UnlockedPowers`.

---

## 3. Level Design Metrics & Physics Ruler

All measurements derived from live `CC_ELEMENTO` values: Gravity $70$, Jump Velocity $21$, Walk Speed $11\text{ u/s}$, Run Speed $16\text{ u/s}$, Fall Speed Limit $28\text{ u/s}$.

### 3.1 Core Jumping & Reach Metrics [IMPLEMENTED]
| Movement Action | Height / Metric | Walking Gap (Flat) | Running Gap (Flat) |
|---|---|---|---|
| **Full Jump** (held) | $3.15\text{ units}$ (keep steps $\le 2.7$) | $6.1\text{ units}$ | $8.9\text{ units}$ |
| **Short Hop** (tapped) | $1.26\text{ units}$ | $\approx 2.5\text{ units}$ | $\approx 3.8\text{ units}$ |
| **Double Jump** | Max height $6.3\text{ units}$ | $10.5\text{ units}$ | $15.3\text{ units}$ |
| **Dash Alone** | Horizontal flat $4.2\text{ units}$ | — | — |
| **Jump + Apex Dash** | Height $3.15\text{ units}$ | $\approx 10.3\text{ units}$ | $\approx 13.0\text{ units}$ |
| **Glide from Jump Apex** | Fall rate $3\text{ u/s}$ | $\approx 14.9\text{ units}$ | $\approx 21.6\text{ units}$ |
| **Glide with 3u Drop** | Fall rate $3\text{ u/s}$ | $\approx 25.9\text{ units}$ | $\approx 37.6\text{ units}$ |
| **Bounce Pad (Normal)** | Launch height $4.8\text{ units}$ | $7.5\text{ units}$ | $11.0\text{ units}$ |
| **Bounce Pad (Pound)** | Launch height $11.4\text{ units}$ | $11.6\text{ units}$ | $16.9\text{ units}$ |
| **Wall Jump Shaft** | Optimal width: $3\text{ to }5\text{ units}$ | Climbs $2.4\text{ to }2.8\text{ units}$ per hop | — |

### 3.2 Design Constraints & Pacing Rules [IMPLEMENTED]
- **Comfortable Margins**: Design standard platforming at $\le 85\%$ of theoretical maximums ($<65\%$ Easy, $65-85\%$ Normal, $85-95\%$ Expert/Challenge, $>95\%$ avoided).
- **Stepping Rule**: Standard vertical steps must never exceed $2.7\text{ units}$ (prevents mandatory frame-perfect jumps).
- **Temporal Pacing**: Standard levels last $30\text{ to }45\text{ seconds}$ in a single continuous space (no camera room transitions).
- **Safe Endings**: Every level ends with a flat, obstacle-free run-up ($\ge 6\text{ units}$) before the exit portal tunnel.
- **Checkpoint Rule**: Maximum of 1 checkpoint per level, placed strictly on a stable horizontal floor (never on ceilings or moving objects).

---

## 4. Level Objects & Interactive Kit

### 4.1 Implemented Systems [IMPLEMENTED]
- **Hazards**:
  - `Hazard`: Base trigger killer (Layer 7).
  - `RotatingSaw`: Circular hazard blade (single blade design; teeth variants retired).
  - `SpinArm`: Rotating perimeter danger arm with configurable pivot and speed.
  - `StompBlock`: Heavy vertical hazard with warning shake/pulse and crushing descent.
  - `FireJet`: Periodic horizontal/vertical flame column with telegraphed warning phase.
  - `LavaLeaper`: Parabolic fireball leaper erupting from lava pits with splash VFX.
  - `Cannon`: Periodic ballistic projectile launcher.
- **Interactables & Platforms**:
  - `BouncePad`: Hexagonal pad providing standard launch or Earth ground-pound mega launch.
  - `DestructibleObject` / `CrackedBlock`: Breakable blocks with stages 0–3, scratch hints, chain reactions along hit direction, and `CrackEffect` hit feedback.
  - `DashWall`: Directional barriers requiring Dash or Ground Pound to shatter.
  - `CrumblingBlock`: Platforms that shake and fall upon contact, resetting on respawn.
  - `MovingPlatform`: Kinematic platform using `IMoverController` with configurable pause intervals.
  - `OneWayPlatform`: Allows jumping through from beneath and dropping down via Down + Jump.
  - `IceSurface`: Low-friction surface maintaining ball momentum.
  - `DashZone`: Acceleration volumes with modes: `Hint`, `Boost`, `Lane`, `Charge`, `SuperDash`.
  - `ElementGate`: Run-through disc applying power, burst VFX, and material emission flash.
  - `LevelStart`, `Checkpoint`, `LevelGoal`, `LevelExitSequence`: Complete scene flow hierarchy with zoom-in exit tunnels.
- **Collectibles**:
  - `Diamond`: Rare, hidden collectible per level; saves persistently to PlayerPrefs; unlocks cosmetic ball skins [IMPLEMENTED].
  - `Atom`: Precision collectible placed off-route; unlocks lore documents [IMPLEMENTED].
  - `Bandage`: [RETIRED] (superseded by Diamond/Atom).

### 4.2 Planned Systems [PLANNED]
- **Air World Objects**: Updraft fans, platform-spawning cannon, falling poison petals.
- **Water World Objects**: Dynamic ice blocks, sticky wall panels, gravity flip fields, drifting jellyfish hazards.

---

## 5. World 1 (Fire) Level Design & Campaign Structure

World 1 consists of a contemplative tutorial plus 20 levels. Each level introduces exactly one new mechanic.

| Level | Focus / New Concept | Status |
|---|---|---|
| **Tutorial: The Void** | Roll, Jump, Run; Camera Orbit; Base Gate Spark; Grid Reveal | [IMPLEMENTED] (`00_Tutorial.unity`) |
| **1-1** | Core Loop: Timer, End Portal, 3 Stars, Fast Restart | [IMPLEMENTED] (`W1_L01.unity`) |
| **1-2** | Stationary Spikes & Shape Language | [PLANNED] |
| **1-3** | One-Way Platforms & Drop-Through | [PLANNED] |
| **1-4** | Checkpoint in an extended course | [PLANNED] |
| **1-5** | Rotating Saws | [PLANNED] |
| **1-6** | Fire Gate & Horizontal Dash | [IMPLEMENTED] (`W1_L06.unity`) |
| **1-7** | Slopes & Momentum Conservation | [PLANNED] |
| **1-8** | Omnidirectional Dash (Upward & Diagonal) | [PLANNED] |
| **1-9** | Lava Leaper Hazard | [PLANNED] |
| **1-10** | Moving Platforms | [PLANNED] |
| **1-11** | Mid-Level Element Gate & Boost Ring | [IMPLEMENTED] (`W1_L11.unity`) |
| **1-12** | Hidden Atom Off-Path Challenge | [PLANNED] |
| **1-13** | DashWall Shattering | [PLANNED] |
| **1-14** | Chained Destructible Blocks | [PLANNED] |
| **1-15** | FireJet Hazard Timing | [PLANNED] |
| **1-16** | Ballistic Cannon Projectiles | [PLANNED] |
| **1-17** | Dash Lanes & Super Dash Zones | [PLANNED] |
| **1-18** | Spin Arm Timing Obstacles | [PLANNED] |
| **1-19** | World 1 Mastery (Combining All Fire Mechanics) | [IMPLEMENTED] (`W1_L19.unity`) |
| **1-20** | Boss Battle: OVERHEAT.SYS | [PLANNED] |

---

## 6. Narrative, Bosses & Presentation

### 6.1 Lore & Setting [PLANNED]
- **The World**: Set inside an antique arcade cabinet in an overgrown research laboratory. Forty years after a nuclear strike, the terminal reboots when a water drop bridges its circuitry.
- **The Protagonist**: The player is a dormant digital seed—an Aether sphere placed into the machine by its dying creator (The Architect).
- **The 60-Second Intro Film**:
  1. *0–10s*: Nuclear detonation outside the laboratory window; power cuts.
  2. *10–15s*: The Architect presses START and inserts the white seed into the cabinet.
  3. *15–35s*: 40 years pass; nature slowly reclaims the ruins.
  4. *35–42s*: Violent wind storm tips the cabinet over.
  5. *42–50s*: Nuclear winter freezes the surrounding vegetation.
  6. *50–60s*: Thaw arrives; a water droplet short-circuits the logic board; machine restarts with "PRESS START".

### 6.2 Boss Encounters: System Failures [PLANNED]
Bosses are treated as critical mainframe faults rather than animate villains. Battles take the form of high-intensity, continuous $60\text{ to }90\text{ second}$ survival sprints.
- **Presentation**: Announced via full-screen UI **System Alert Cards** (e.g., `SYSTEM ALERT // 001 - OVERHEAT: THERMAL PROTECTION FAILURE`). The word "BOSS" is never used.
- **World 1 (Fire) — OVERHEAT.SYS**:
  - *Threat*: Thermal runaway. A destructive heat wall sweeps left-to-right, melting architecture into slag.
  - *Mastery*: Speed and shattering destructible barriers before they dissolve.
- **World 2 (Earth) — DEFENDER.BIN**:
  - *Threat*: Mainframe security golem slamming vertically from above.
  - *Mastery*: Slam impacts spawn temporary bounce pads; bounce onto the golem's crown and ground-pound into an internal 5-node security core.
- **World 3 (Air) — FAN.DRV / COOLING.SYS**:
  - *Threat*: Massive ventilation driver blowing right-to-left.
  - *Mastery*: Overcoming heavy atmospheric drag through double jumps, low glides, and moving platform timing.
- **World 4 (Water) — CACHE_PURGE.BAT**:
  - *Threat*: Deletion wave rising bottom-to-top.
  - *Mastery*: Rapid alternating between gravity flips (ceiling vapor) and floor running (ice), using wall holds to hang above the crest.
- **Final Encounter — BSOD.SYS**:
  - *Threat*: Complete core purge combining all four failures in a 60-second multi-directional sprint.
  - *Resolution*: The core ignites the seed into blinding white Aether, tilting the camera from 2.5D into the 3D hub.
