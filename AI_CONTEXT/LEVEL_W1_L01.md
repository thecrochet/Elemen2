# Level 1-1 "The Loop": layout (main path)

Units are world units. X grows to the right. Y = height of the walking surface (top of the platform). All gameplay at Z = 0. Par 8 s, about 109 units wide. One new idea: the loop itself (timer, end portal, instant restart). The pit is the only way to die, so there are no hazards, no checkpoint and no camera zone.

## Platforms (8 pieces, left to right)

Use the Ground prefab (exact path in PREFAB_PATHS.md). It is a NeonBlock with Anchor TopLeft, so its pivot is the TOP-LEFT corner: Position = (left edge X, surface Y, 0). Set only NeonBlock Size X; keep Size Y 1 and Size Z 2.

Size is in cells, not units. Read the CellSize from PREFAB_PATHS.md and set Size X = length / CellSize. State the CellSize you used in your report.

| # | Left edge X | Surface Y | Length | Gap before it |
| --- | --- | --- | --- | --- |
| P1 | 0 | 0 | 16 | none (start) |
| P2 | 19 | 0 | 8 | 3 |
| P3 | 31 | 0 | 8 | 4 |
| P4 | 44 | 0 | 8 | 5 |
| P5 | 58 | 0 | 8 | 6 |
| P6 | 66 | 1.5 | 8 | none (a 1.5 step up, touching P5) |
| P7 | 79 | 1.5 | 8 | 5 |
| P8 | 95 | 1.5 | 14 | 8 (needs a running jump) |

Check: P8 ends at X = 109. The gaps are 3, 4, 5, 6, 5, 8 in that order.

## Flow prefabs (paths in PREFAB_PATHS.md)

- LevelCore: at 0,0,0. It holds the player, camera, managers and UI.
- StartPortal: root on the floor of P1 at X = 3, Y = 0, Rotation 0. Do not change its children.
- End_Portal (the exit): near the right end of P8 at X = 104, with its root on the floor of P8 (Y = 1.5). Keep its rotation as in the prefab; its tunnel must point away from the camera (+Z). If the prefab pivot is not at its feet, place it so the ball runs into it on the floor, and say what you did.
- No Checkpoint, no CameraZone, no LevelFrame (it is 40 wide; this level is about 109).

## Wiring

1. Create a new empty scene with the CLI and save it as W1_L01_TheLoop in Assets/Scenes/Levels/World1_Fire. Remove default Main Camera and Directional Light.
2. Add LevelCore, StartPortal, End_Portal, then the 8 platforms. Add anything under "Scene essentials" in PREFAB_PATHS.md.
3. Link StartPortal > SpawnPoint into RespawnManager > Spawn Point (inside LevelCore), and put the player on that point.
4. LevelManager > Next Scene Name: set to W1_L01_TheLoop, so finishing reloads the level for now (no 1-2 exists yet).
5. Leave Build Settings alone. I will add the scene by hand.
6. Exactly one camera, one AudioListener, one EventSystem, one Vfx, one Sfx.
7. Leave RespawnManager KillY at its prefab value; falling into a gap is the death.

## Do not
- Edit any prefab asset or script.
- Add the secret road, hazards or collectibles. That is the next task.
- Change starting power or any player value.
