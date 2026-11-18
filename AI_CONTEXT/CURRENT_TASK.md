# Current task

Goal: Create an Editor menu script Assets/Scripts/Editor/W1L01LevelBuilder.cs ("Elemento/Build W1_L01") and run it to generate World 1 Level 1 from TEMPLATE_SCENE with a 15-second pit-rhythm course, collectibles, checkpoint, and 3-star time thresholds.
Why: Builds the first playable World 1 level using existing prefabs and exact jump-arc distances without manual YAML editing.
Mode: implement
Files you may touch: Assets/Scripts/Editor/W1L01LevelBuilder.cs (via Editor script only for the generated scene asset)
Files you must not touch: Assets/Scenes/Templates/TEMPLATE_SCENE.unity, Assets/Scripts/Player/CC_ELEMENTO.cs, KinematicCharacterController, Legacy folders
Exact values:
- Read Assets/Scripts/Level/Builders/NeonBlock.cs, Assets/Scripts/Core/LevelManager.cs, Assets/Scripts/Level/Atom.cs, and Assets/Scripts/Level/Diamond.cs first to use their exact serialized field names (do not guess).
- Copy Assets/Scenes/Templates/TEMPLATE_SCENE.unity to Assets/Scenes/Levels/World1/W1_L01.unity (create World1 folder via AssetDatabase if missing), open it, remove PrefabLab and ControlPrompt if present, and instantiate prefabs from Assets/Prefabs/Level/.
- All top surfaces sit at Y = 0, Z = 0 unless noted. Place StartPortal, LevelCore/RespawnPoint, and LevelCore/Player at (2, 0, 0) with StartingPower = PowerUpType.None.
- Platforms (X start -> X end, top Y): P1 (0..18, Y=0), P2 (21.5..36, Y=0), P3 (40.5..48.5, Y=0), P4 (53..61, Y=1.5), P5 (66..82, Y=0), Low P6A (82..90, Y=0), Low P6B (95.5..105.5, Y=0), High Ledge1 (88..96, Y=2.8), High Ledge2 (102.5..110.5, Y=2.8), Final P7 (111.5..152, Y=0).
- Flow & Collectibles: Atom at (50.75, 3.2, 0); Checkpoint at (80, 0, 0); Diamond at (106.5, 3.8, 0); End_Portal at (144, 0, 0). Tile 4 LevelFrame_40x14 across X = 0..160 and span KillPit across X = -10..170 at Y = -5.
- Set LevelManager star times to 12.5s (3 stars), 15.5s (2 stars), 22.0s (1 star), and wire End_Portal LevelExitSequence.FadeGroup to LevelCore/LevelUI/ScreenFade if empty.
Done when: Project compiles with 0 errors/warnings, Assets/Scenes/Levels/World1/W1_L01.unity is saved cleanly, and Editor is left out of Play mode.
Docs to read: ARCHITECTURE, CODING_RULES, DESIGN
Report: standard format from GEMINI.md