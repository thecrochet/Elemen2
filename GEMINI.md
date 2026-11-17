# Elemento: Agent Rules (source of truth, auto-loaded as GEMINI.md)

Elemento is a 2.5D precision platformer in Unity 6 (6000.3, URP 17.3, New Input System, Cinemachine 3, Kinematic Character Controller). The user designs with Claude and hand-builds prefabs, particles and materials in the Inspector. You execute one task at a time, mostly C#.

## Read order (load only what the task needs)
1. This file. 2. `AI_CONTEXT/CURRENT_TASK.md` (the task). 3. `CODING_RULES.md` + `ARCHITECTURE.md` for code. 4. `DESIGN.md` for gameplay/levels. 5. `KNOWN_ISSUES.md` only when asked.

## Truth and scope
- The project is the truth; docs are a Nov 2026 snapshot. If they disagree, trust the project and report it. Never invent numbers.
- Gameplay numbers live in the Inspector (scene/prefab values). C# defaults may differ: read the live value through the CLI before relying on or tuning a number, and never overwrite live Inspector values with C# defaults.
- Expose new tunables as public/[SerializeField] fields with [Header]/[Range]; never hardcode magic numbers.
- Do only what CURRENT_TASK asks. No extra refactors, renames or cleanups. Unknown = write "not verified".
- Never delete, rename or move assets, and never create or delete `.meta` files by hand: use AssetDatabase calls, and only if the task says so.
- Never reorder enum values (PowerUpType, DashZone modes, NeonBlock anchor, VfxType, SfxType): append only.
- Do not touch KinematicCharacterController, Legacy folders, or empty placeholder ScriptableObjects.
- Do not save or modify scenes unless the task names them. Do not change project, quality or build settings. No package installs.
- Links to scene objects outside a prefab are lost when the prefab is placed elsewhere: say so whenever a new slot needs one.

## Cost rules (these protect the user's quota)
- **Cheapest tool first:** (1) a file Claude provides: copy it into place, never retype it; (2) the live Editor through the Unity CLI: `unity command <group> ...` with `--caller plugin --skill <skill-name>` (groups: gameobject, component, scene, prefab; do not guess flags, run help first), or a small C# `[MenuItem]` Editor script using AssetDatabase + SerializedObject; (3) hand-written C#.
- **YAML:** never read whole, create, duplicate-and-edit or patch `.prefab`, `.unity`, `.mat`, `.anim` or `.asset` files as text. A targeted read-only `grep` for one value is allowed. Changes go through the Editor (rule above) or the user does them in the Inspector.
- **Particles, materials, prefab layouts, UI layouts are the user's job** unless the task explicitly asks for an Editor script.
- **Size cap:** if your plan touches more than 3 files, or writes more than 300 lines in one file, post a 5-line plan and wait for a go.
- **Long files:** never read a script over 400 lines whole (CC_ELEMENTO.cs is about 1,700). Grep the symbol, then read at most 80 lines around it.
- **Encoding:** the project is on Windows, so do not use the Unix `file` command. Before editing a `.cs`, check it is UTF-8 in PowerShell: `[IO.File]::ReadAllText('<path>',(New-Object Text.UTF8Encoding($false,$true))) | Out-Null` (an error means it is not UTF-8). If so, stop and report. Keep CRLF endings.
- **Retry cap:** after 2 failed fix attempts on the same error, stop and report it. Never loop.
- One task per chat. Do not re-read a file you just edited. Do not paste whole files into the report. Batch independent actions.

## Unity rules
- Compile after every script change and read only the new Console lines. Bar: zero new errors and warnings.
- Do not enter Play mode or take screenshots unless CURRENT_TASK.md says `Mode: verify`. Never leave the Editor in Play mode.
- Delete any temporary helper scripts or screenshots you made before finishing.
- Do not update the docs in `AI_CONTEXT/` yourself. The user confirms testing first, then Claude updates them.

## Report format (end of every task, 5 lines max)
1. Files changed (path, one line each). 2. How to verify in the Editor. 3. Inspector slots the user must wire (prefab, child, field, value). 4. Anything unverified, risky or surprising. 5. Rough cost: files touched, lines written.
