# Elemento: Agent Rules (source of truth)

Elemento is a 2.5D precision platformer in Unity 6 (6000.3, URP 17.3, New Input System, Cinemachine 3, Kinematic Character Controller). The user designs with Claude. You execute one task at a time: scripts, builder tools, and building levels in scenes from existing prefabs.

## Read order (load only what the task needs)
1. This file (always).
2. `AI_CONTEXT/CURRENT_TASK.md` (always: it is the task).
3. `AI_CONTEXT/PREFAB_PATHS.md` for any level or scene task (exact prefab paths and scene essentials). If this file does not exist yet, stop and say so.
4. `AI_CONTEXT/CODING_RULES.md` and `AI_CONTEXT/ARCHITECTURE.md` for any code task.
5. `AI_CONTEXT/DESIGN.md` for gameplay or level tasks.
6. `AI_CONTEXT/KNOWN_ISSUES.md` only when asked, or when you hit something that looks like a bug.

## Source of truth
- The Unity project files are the truth. The docs are a snapshot of November 2026 and may be stale. If a doc and the project disagree, trust the project, say so in your report, and do not "fix" the doc silently.
- Never invent numbers. Tuning values live in the scripts and prefabs; read them there. Level layouts come from the task.
- If something is not verified, write "not verified".

## Scope and safety
- Do only what CURRENT_TASK.md asks. No extra refactors, renames, or cleanups.
- Never delete, rename, or move files or assets unless the task says so. Ask first.
- Never reorder enum values (PowerUpType, DashZone modes, NeonBlock anchor, VfxType, SfxType): they are serialized as numbers. Append only.
- Do not touch KCC (KinematicCharacterController folder), Legacy folders, or the empty placeholder ScriptableObjects.
- Work only on the git branch the user created for the task. Never run git commands that change history or branches. Read-only git (`git status`, `git branch --show-current`) is fine.

## Levels, scenes and prefabs
You may create level scenes and place prefabs in them. Use the paths in `AI_CONTEXT/PREFAB_PATHS.md`.
- NO SEARCHING. Never search the project for templates, scenes or prefabs. Take every path from PREFAB_PATHS.md. If something is missing from it, ask once in your report and stop; do not hunt for it.
- A new level is a NEW EMPTY SCENE created with the Unity CLI, saved as `Assets/Scenes/Levels/<World>/W<w>_L<nn>_<Name>.unity`. Do not copy or open `TEMPLATE_SCENE`; it is reference only.
- A new scene must contain only: LevelCore, one StartPortal, one End_Portal, and the level pieces from the task, plus anything listed under "Scene essentials" in PREFAB_PATHS.md. Remove any default objects the new scene creates (Main Camera, Directional Light), because CameraRig already holds the camera. Exactly one camera, one AudioListener, one EventSystem (inside LevelUI), one Vfx, one Sfx.
- Place existing prefab instances. Do not unpack prefab instances. Do not rebuild a prefab out of loose GameObjects when one exists.
- Per-instance changes are allowed as overrides on the scene instance: position, rotation, NeonBlock Size, and Inspector values the task lists (Travel, Delay, Element, Message, and so on).
- Prefab assets are shared by every level. Do not edit, rename, or re-save a prefab asset unless the task names it. If a level needs a change to a prefab, report it instead.
- All gameplay objects sit at Z = 0. Place on whole numbers; NeonBlock Size is in cells (see CellSize in PREFAB_PATHS.md).
- Wiring for every level: StartPortal `SpawnPoint` into `RespawnManager > Spawn Point`, the player on the spawn point, `LevelManager > Next Scene Name` as given in the task, and the scene added to Build Settings only if the task says so. At most one Checkpoint.
- Links from a prefab to a scene object outside it are lost when the prefab is placed. Whenever a placed prefab has such a slot, wire it in the scene and say so in your report.
- Moving hazards run on the level clock: set Delay and TimeOffset from the task values, never guessed.
- Never write a whole prefab, scene or asset as raw YAML from scratch (thousands of lines, huge token cost). Place instances, change only the fields the task lists, or use an Editor script, a builder menu command, or the Unity CLI.
- Scene edits belong in the one scene the task names. Do not touch other scenes.

## Unity CLI rules
- Use the `unity-cli` skill for Editor actions. Do not guess command names; list them first. Known commands: `unity command scene open`, `scene save`, `prefab instantiate`, `component set_field`, `asset copy`, `unity logs --console`, `unity status`.
- The Editor must be open on this project. If `unity status` fails, stop and say so; do not try to open or restart it.
- Compile after every script change and read the Console. Zero new errors and zero new warnings is the bar.
- Enter Play mode only if the task asks for a test, and always exit Play mode before you finish. Never leave the Editor in Play mode.
- Do not change project, quality, or build settings unless the task says so.
- No package installs or updates unless the task says so.

## Efficiency rules
- Do not scan the whole project. Open only the files the task touches, and read ranges instead of whole files when a file is long.
- Do not re-read a file you just edited. Do not paste whole files into your report.
- One task per chat: start a new conversation for each CURRENT_TASK.md so old reads are not re-sent.
- Batch independent actions. Stop and ask when the task is ambiguous instead of guessing.
- Skills: use only the ones the task needs (unity-cli, ui-ugui, fix-3d-physics-collisions, validate-urp-render-graph-renderer-feature, optimize-text-mesh-pro, unity-package-management).

## Report format (end of every task)
1. Files changed (path, one line each), including the scene.
2. How to verify in the Editor (exact steps).
3. Inspector slots the user must wire by hand (scene or prefab, object, field, value).
4. Anything unverified, risky, or surprising.
Keep it short.
