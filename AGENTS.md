# Elemento: Agent Rules (source of truth)

Elemento is a 2.5D precision platformer in Unity 6 (6000.3, URP 17.3, New Input System, Cinemachine 3, Kinematic Character Controller). The user designs with Claude and hand-builds prefabs. You execute one task at a time.

## Read order (load only what the task needs)
1. This file (always).
2. `AI_CONTEXT/CURRENT_TASK.md` (always: it is the task).
3. `AI_CONTEXT/CODING_RULES.md` and `AI_CONTEXT/ARCHITECTURE.md` for any code task.
4. `AI_CONTEXT/DESIGN.md` for gameplay or level tasks.
5. `AI_CONTEXT/KNOWN_ISSUES.md` only when asked, or when you hit something that looks like a bug.

## Source of truth
- The Unity project files are the truth. The docs are a snapshot of November 2026 and may be stale. If a doc and the project disagree, trust the project, say so in your report, and do not "fix" the doc silently.
- Never invent numbers. Tuning values live in the scripts and prefabs; read them there.
- If something is not verified, write "not verified".

## Scope and safety
- Do only what CURRENT_TASK.md asks. No extra refactors, renames, or cleanups.
- Never delete, rename, or move files or assets unless the task says so. Ask first.
- Never reorder enum values (PowerUpType, DashZone modes, NeonBlock anchor, VfxType, SfxType): they are serialized as numbers. Append only.
- Do not touch KCC (KinematicCharacterController folder), Legacy folders, or the empty placeholder ScriptableObjects.
- Do not edit scenes or prefabs unless the task names them. The user builds prefabs by hand; you may create scripts and builder menu items.
- Links to scene objects outside a prefab are lost when the prefab is placed in another scene. Say so whenever a new slot needs one.

## Unity CLI rules
- Use the `unity-cli` skill for Editor actions. Do not guess command names; list them first.
- Compile after every script change and read the Console. Zero new errors and zero new warnings is the bar.
- Never leave the Editor in Play mode. Do not change project, quality, or build settings unless the task says so.
- No package installs or updates unless the task says so.

## Efficiency rules
- Search before you read. Open only the files the task touches, and read ranges instead of whole files when a file is long.
- Do not re-read a file you just edited. Do not paste whole files into your report.
- Never write a whole prefab, scene or asset as raw YAML from scratch (thousands of lines, huge token cost). Duplicate the closest existing asset and change only the fields the task lists, or use an Editor script/menu command.
- One task per chat: start a new conversation for each CURRENT_TASK.md so old reads are not re-sent.
- Batch independent actions. Stop and ask when the task is ambiguous instead of guessing.
- Skills: use only the ones the task needs (unity-cli, ui-ugui, fix-3d-physics-collisions, validate-urp-render-graph-renderer-feature, optimize-text-mesh-pro, unity-package-management).

## Report format (end of every task)
1. Files changed (path, one line each).
2. How to verify in the Editor (exact steps).
3. Inspector slots the user must wire by hand (prefab, child, field, value).
4. Anything unverified, risky, or surprising.
Keep it short.
