# Current Task

Goal: Build level `W2_L01` ("First Earth Level") at `Assets/Scenes/Levels/World2_Earth/W2_L01.unity` by duplicating `Assets/Scenes/Templates/LEVEL_TEMPLATE.unity` and assembling all pieces via `unity-pipeline`.
Why: Introduce the Earth power (Ground Pound) for the first time—playing a Fire/dash-only first half as a review, then granting Earth at the midpoint gate and testing Ground Pound through a floor plug, a one-shot EarthHoverPad climb, and a vertical breakable shaft.
Mode: implement (scene + placed prefabs + `M_Level_Earth.mat`). No new C# scripts unless a required prefab/builder is missing (stop and ask first if any C# change is needed).
Files you may touch:
- `Assets/Scenes/Levels/World2_Earth/W2_L01.unity` (new scene duplicated from `Assets/Scenes/Templates/LEVEL_TEMPLATE.unity`)
- `Assets/ART/Materials/Level/M_Level_Earth.mat` (duplicated from `M_Level_Orange.mat`)
Files you must not touch: All protected assets in `AGENTS.md` (`Assets/Scripts/**`, shaders, existing prefabs, `Assets/Scenes/Templates/LEVEL_TEMPLATE.unity`, `Assets/Scenes/Levels/World1_Fire/**`, `Build Settings`, `Project Settings`, `KCC`, `Legacy`).
Exact values: See `LEVEL SPEC: W2_L01` below (`Z = 0`, root `localScale = (1, 1, 1)`).
Done when: All 9 Editor & Play Mode checks in Section 9 pass, `Elemento > Validate Current Level` reports 0 errors/warnings, the Editor is not left in Play Mode, and a Scene view screenshot is captured.
Docs to read: `AI_CONTEXT/ARCHITECTURE.md`, `AI_CONTEXT/DESIGN.md` (also inspect `Assets/Scenes/Levels/World1_Fire/W1_L06.unity` as reference for gate/dash setup and `Assets/Settings/InputSystem_Player.inputactions` for the exact Ground Pound key/button label).
Report: standard format from `AGENTS.md` plus the 5 extra items in Section 10.

---

## LEVEL SPEC: W2_L01

### 0. Rules for This Task
- Follow `AGENTS.md`. If this spec and the live project disagree, trust the project and report the discrepancy.
- Use **`unity-pipeline` ONLY** (never `unity-cli`, never create temporary `.cs` files on disk, never write `.unity` or `.prefab` YAML by hand).
- Duplicate `Assets/Scenes/Templates/LEVEL_TEMPLATE.unity` to `Assets/Scenes/Levels/World2_Earth/W2_L01.unity` (create folder if missing) and organize all root GameObjects into the **7 standard root containers at `Z = 0`**: `LevelCore`, `Environment`, `Interactables`, `Hazards`, `Collectibles`, `Checkpoints`, `Flow`.
- Everything sits at **`Z = 0`** with root **`Transform.localScale = (1, 1, 1)`**.
- **NeonBlock Units Check:** Inspect `Block_1x1` / `Ground_20x1` in `Assets/Prefabs/Level/Foundation/` first to confirm `NeonBlock.Size` cell scale (`0.5` unit per cell -> `Size = 2 * length in units`) and apply whole-number `NeonBlock.Size` values using `TopLeft` anchor on floors/ledges/ceilings.
- User approval is already granted in this task for:
  1. Duplicating `M_Level_Orange.mat` to `Assets/ART/Materials/Level/M_Level_Earth.mat`.
  2. Instantiating or creating `ControlPrompt` (`GameObject > Elemento > UI > Control Prompt`) and `EarthHoverPad` (`GameObject > Elemento > Level > Earth Hover Pad` only if no prefab exists under `Assets/Prefabs/`).
- Never leave the Editor in Play Mode. Check Console after every step (0 errors, 0 new warnings).

### 1. Design Intent & Rhythm
- **One New Concept:** Ground Pound (Earth). Everything before the Earth gate is Fire/dash-only review with **no prompts** and **no hazards other than `FireJet`s** (no spikes or saws in the first half).
- **Target Width & Pacing:** Total width ~`310 to 350` units (target `~330 u`), highest point `~+9 u` above main floor, deepest point `~-10 u` below main floor. Par (fast route with secret road) `~26 s`; moderate player `~40 s`.
- **Main Route vs. Secret Road:** Main route gaps/jumps `<= 85%` of max (`steps <= 2.7 u`). Secret road may reach up to `95%` and saves `~2 s`.

### 2. First Half: Dash-Only Review (`X = 0` to `~160`, No Prompts, No Spikes/Saws)
Place left-to-right under `Environment`, `Interactables`, `Hazards`, and `Flow` (all using `M_Level_Earth.mat` on terrain blocks):

- **A1 — Start Floor (`16 u` long, `X = 0..16`, top `Y = 0`):**
  - Place `StartPortal` under `Flow` at `(2, 0, 0)` (left end, `2 u` back margin).
  - **CRITICAL:** Move `LevelCore/Player` to `StartPortal/SpawnPoint` in Edit Mode and wire `StartPortal/SpawnPoint` into `RespawnManager.SpawnPoint`.
- **A2 — Dash Gap 1 (`10 u` gap `X = 16..26`, then `10 u` floor `X = 26..36`, top `Y = 0`):**
  - Place a `KillPit` below the gap. `10 u` exceeds plain run-jump (`8.9 u`), requiring jump + apex dash (`~77%` range).
- **A3 — Dash Wall Room (`14 u` floor `X = 36..50`, top `Y = 0`; ceiling `3.4 u` above, underside `Y = 3.4`):**
  - Place one `DashWall_DashOnly` (`Assets/Prefabs/Level/Interactables/Variants/`) in the middle (`X = 43`), filling the full `3.4 u` room height (`Y = 0..3.4`). Nothing else here can kill.
- **A4 — Fire Jet Stretch (`26 u` open floor `X = 50..76`, top `Y = 0`):**
  - Place two upward-firing `FireJet` instances at `X = 58` (`+8 u`) and `X = 67` (`+17 u`, leaving `9 u` flat floor between jets), seated on/flush with floor notches so the ball can stand safely beside the nozzle.
  - Configure both: `OnTime = 1.2`, `OffTime = 1.6`, `WarnTime = 0.4`. Set `TimeOffset = 0` on Jet 1 and `TimeOffset = 1.4` on Jet 2.
- **A5 — Step Platform (`12 u` floor `X = 76..88`, top `Y = 1.5`):**
  - `+1.5 u` step up for rest and elevation change.
- **A6 — Dash Gap 2 (`9 u` gap `X = 88..97`, then `12 u` floor `X = 97..109`, top `Y = 0.5`):**
  - `KillPit` below gap; lands `1.0 u` lower (`Y = 0.5`).
- **A7 — Dash Wall Pair (`20 u` floor `X = 109..129`, top `Y = 0.5`; ceiling `3.4 u` above, underside `Y = 3.9`, top `Y = 4.4`):**
  - Place two `DashWall_DashOnly` barriers `4 u` apart (`X = 117` and `X = 121`), each filling `Y = 0.5..3.9`.
  - **Do NOT use `DashBoost`, `DashLane`, or `DashZone` anywhere in this level.**
- **A8 — Safe Room Approach (`10 u` flat floor `X = 129..139`, top `Y = 0.5`, empty).**

#### Secret Road (3-Star Shortcut — Above A5 to A7, No Hazards)
- From A5's right edge (`X = 88, Y = 1.5`), a full held jump (`3.15 u` apex) reaches **Secret Ledge 1**: `Ledge_8x1` (`8 u` long) at `X = 88.5..96.5`, top `Y = 4.3` (`+2.8 u` above A5).
- A `10 u` jump + dash (`X = 96.5..106.5`) reaches **Secret Ledge 2**: `Ledge_8x1` (`10–12 u` long) at `X = 106.5..118.5`, top `Y = 4.8` (cleanly above A7's ceiling top at `Y = 4.4` with zero collider overlap).
- Drop off the right end of Secret Ledge 2 onto A8 (`X = 129, Y = 0.5`) **before** the checkpoint. Ensure camera framing keeps the secret road visible.

### 3. Safe Room: Checkpoint & Earth Gate (`X = 139..165`, Top `Y = 0.5`)
- **A9 Floor:** `26 u` flat floor (`X = 139..165`, top `Y = 0.5`).
- **Checkpoint:** Place `CheckPoint` under `Checkpoints` at `X = 151` (`gate X 159 - 8`), grounded on the flat floor (`RespawnPoint` `0.1 u` above floor).
- **8 u Plain Floor:** `X = 151..159`.
- **Earth Gate:** Place `ElementGate_Earth` (`Assets/Prefabs/Level/Interactables/Variants/`) under `Interactables` at `X = 159`, center `Y = 2.1` (`1.6 u` above `Y = 0.5` floor), `Z = 0`, `Element = Earth`.
  - Place a ceiling `3.4 u` above the floor over the gate (`underside Y = 3.9`) and ensure the gate trigger (`~1.2 x 3.2..3.4 x 1.5`) spans floor-to-ceiling so it is impossible to jump over.
  - Enable `PlayCollectBurst = true` and wire `CrossEffect` to a child `ParticleSystem` copied from `CheckPoint`'s `ActivateEffect` (`Play On Awake = false`, `Looping = false`).
- **Post-Gate Floor:** `6 u` flat floor (`X = 159..165`, top `Y = 0.5`).

#### Power & Loadout Rules (Verify in Editor)
- On `LevelCore` (`LevelLoadout`): `AllowedPowers = Fire | Earth`, `freeMode = false` (Play mode), `unlockAll = false`.
- On `LevelCore/Player` scene instance (`CC_ELEMENTO`): `StartingPower = None`, `UseSavedPowers = true`, `CheatOwnedPowers = Fire` ONLY (do NOT check `Earth`, and do not edit the `Player` prefab asset).
- Remember to run `Elemento > Clear Saved Powers` before Play Mode testing so `Unlocked_Powers` does not pre-grant Earth.

### 4. Second Half: The Earth Test (`X = 165` to `~330`)
Read `Assets/Settings/InputSystem_Player.inputactions` first to get the exact current binding label for `GroundPound` / `GroudPound` (e.g., `West / Enter`) and use that real label in `ControlPrompt` (never write `"Down in the air"`).

- **B1 — ControlPrompt (Post-Gate):**
  - Place `ControlPrompt` under `Flow` covering `X = 159..165` (`6 u` wide, centered at player height `Y = 1.5`), text `"Ground Pound"` + real binding label, `ShowOnce = false` (must reappear on retries after death). No other prompts in the level.
- **B2 — Floor Plug Test (`X = 165..189`):**
  - Upper floor `12 u` long (`X = 165..177`, top `Y = 0.5`), followed by a `4 u` wide x `1 u` thick floor plug (`X = 177..181`, top `Y = 0.5`) built from `DashWall_PoundOnly` (`BreakableByDash = false`, `BreakableByGroundPound = true`, `HitsToBreak = 1`).
  - Place a solid vertical wall at `X = 181` blocking forward movement on the upper floor so going down through the plug is mandatory.
  - Below the plug (`X = 176..189`), build a lower corridor with floor `5 u` below the upper floor (`top Y = -4.5`) and ceiling `3.5 u` above the lower floor (`underside Y = -1.0`). No hazards.
- **B3 — Lower Corridor (`26 u` long, `X = 189..215`, floor top `Y = -4.5`, ceiling underside `Y = -1.0`):**
  - Flat hazard-free run opening at `X = 215` into a tall vertical climb room.
- **B4 — One-Shot Earth Hover Pad Climb (`X = 215..235`):**
  - Check `Assets/Prefabs/` for `EarthHoverPad` first (or create via `GameObject > Elemento > Level > Earth Hover Pad`). Place at `X = 218, Y = -1.5` (`+3 u` above `-4.5` corridor floor) with `SlamLaunchStrength = 40`, `ReturnDelay = 2`.
  - Place top landing `Ledge_8x1` at `X = 222..230` (`4 u` right of pad), top `Y = +3.5` (`+8 u` above `-4.5` corridor floor, `~70%` of `11.4 u` pound-pad launch).
  - Place a shorter catch ledge on the left `3 u` below (`X = 215..219`, top `Y = +0.5`) for short launches. Safe corridor floor below (`Y = -4.5`, no hazard).
  - Cover the climb with `CameraZone_Climb` (`Assets/Prefabs/Level/Flow/Variants/`) so the `+8` ledge is clearly visible.
- **B5 — Ground Pound Shaft Payoff (`X = 235..265`):**
  - From the top ledge (`Y = +3.5`), build a vertical shaft `3 u` wide (`X = 236..239`, between `3` and `5 u` wide) and `10 u` deep (`Y = +3.5` down to `-6.5`) with flat vertical solid walls.
  - Fill the upper half of the shaft with a vertical column of `4–5` `DashWall_PoundOnly` blocks (or `GroundPoundChainRow 2` configured with `BreakableByDash = false`, `BreakableByGroundPound = true`, `ChainReaction = true`, `ChainDelay = 0.05`), leaving `~5 u` of open vertical drop below the column onto a `3 u` wide catch floor (`top Y = -6.5`) opening right into B6. No hazards in the shaft.
  - Add `CameraZone_Tight` or `CameraZone_Wide` only if needed so the shaft is framed cleanly.
- **B6 — Exit Stretch (`X = 265..330`, top `Y = -6.5`):**
  - Flat, hazard-free safe floor leading to `End_Portal` at `X = 324` (`>= 14 u` clear run-up, total level width `~330 u`).
  - Leave `LevelManager.NextSceneName` and `LevelGoal.NextSceneName` **empty** (`W2_L02` does not exist yet). Do NOT modify `Build Settings`.

### 5. Look & Materials
- Duplicate `Assets/ART/Materials/Level/M_Level_Orange.mat` to `Assets/ART/Materials/Level/M_Level_Earth.mat` (`Elemento/Level/NeonGrid` shader).
- Set only the grid color to a darker amber-brown `(0.9, 0.5, 0.18)` so the Earth gate (`HDR 1.8, 1.05, 0.5`) stands out clearly, and apply `M_Level_Earth` to all floors, walls, ceilings, and ledges. Keep hazard colors and worn-neon breakable block hints unchanged. No idle particles on static objects.

### 6. Level Configuration (`LevelCore`)
- `LevelManager`: `EnableStars = true`, `StartOnFirstInput = true`, `ThreeStarTime = 34`, `TwoStarTime = 52`, `OneStarTime = 104`, `NextSceneName = ""`.
- `RespawnManager`: `KillY = -11` (4 units below `-6.5` shaft floor), `KillTopY = +13` (4 units above `+9` upper ceilings/ledges), `SpawnPoint = StartPortal/SpawnPoint`.
- `DebugOverlay`: `StartVisible = false` (and `PlayerLab` disabled if present). No `Diamond` or `Atom` collectibles in this level.

### 7. Done When (Run All 9 Checks & Report Results)
1. Scene compiles and runs `Elemento > Validate Current Level` with **0 errors and 0 new warnings**; Editor is not left in Play Mode.
2. With `CheatOwnedPowers = Fire` only and saved powers cleared (`Elemento > Clear Saved Powers`), both A2 (`10 u`) and A6 (`9 u`) require jump + dash; plain run-jumps fall into `KillPit`.
3. Timer starts on first input. Crossing `ElementGate_Earth` turns the ball Earth, plays the burst, shows `ControlPrompt` with the verified binding label, enables Ground Pound, and preserves Dash.
4. Both the B2 floor plug and B5 shaft column resist Dash and shatter on Ground Pound.
5. Without Earth (skipping the gate or dying before it), B2 cannot be passed; dying after the gate respawns the ball at the checkpoint in gold Base Spark (no Earth) so the player must recross the gate; jumping over the gate is impossible.
6. `EarthHoverPad` ignores normal landings, launches onto the `+8` ledge (`Y = +3.5`) on Ground Pound, and resets after `2 s` or on respawn.
7. The secret road (`A5 -> Ledge 1 -> Ledge 2 -> A8`) works, saves `~2 s`, and rejoins before `CheckPoint`.
8. Par fast run is `~26 s` (3 stars `<= 34 s`) and moderate run is `~40 s` (2 stars `<= 52 s`)—time both in Play Mode and report exact times.
9. `End_Portal` triggers finish cleanly with `NextSceneName` empty and `Build Settings` untouched.

### 8. Extra Report Lines (Append to Standard 4-Point Report)
- Exact `X` positions of `StartPortal`, `CheckPoint`, `ElementGate_Earth`, B2 plug, `EarthHoverPad`, B5 shaft, `End_Portal`, and actual total level width.
- Exact RGB/HDR color values saved on `M_Level_Earth.mat`.
- Whether an existing `EarthHoverPad` prefab was found or created via the builder menu.
- Any discrepancies between this spec and the live project (prefab names, `NeonBlock.Size` units, `GroundPound` binding label, `FireJet` default orientation).
- Any Inspector slots the user must verify/wire by hand.