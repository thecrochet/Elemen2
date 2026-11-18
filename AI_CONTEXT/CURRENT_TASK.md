# Current task

Goal: Implement IResettable on Assets/Scripts/Level/GridRevealTrigger.cs so its _fired state commits at checkpoints and resets on respawn when dying before a checkpoint.
Why: Currently GridRevealController resets the grid back to dark on pre-checkpoint death, while GridRevealTrigger stays permanently fired and never re-triggers the wave on retry.
Mode: implement
Files you may touch: Assets/Scripts/Level/GridRevealTrigger.cs
Files you must not touch: Assets/Scripts/Player/CC_ELEMENTO.cs, KinematicCharacterController, Legacy folders
Exact values: Match the exact IResettable registration pattern used in GridRevealController.cs (read GridRevealController.cs first; do not scan scenes). Commit _fired into _committedFired in CommitState(), and restore _fired = _committedFired in ResetObject().
Done when: Project compiles with 0 errors/warnings; dying before a checkpoint resets the trigger so crossing it again re-fires the reveal wave, while dying after a checkpoint keeps it fired.
Docs to read: CODING_RULES
Report: standard format from GEMINI.md