# Known Issues & Open Decisions (Read Only on Request)

## Verified Bugs & Quirks
1. **Duplicate `DashWall` Prefab:** `Assets/Prefabs/Level/Platforms/` contains a non-breakable `DashWall` copy. Always instantiate breakable walls from `Assets/Prefabs/Level/Interactables/`.
2. **Template Defaults:** `Assets/Scenes/Templates/LEVEL_TEMPLATE.unity` places `Player` at `X = -10` and sets `LevelManager.NextSceneName = "PrefabLab"`. Every duplicated level must move `Player` to `StartPortal/SpawnPoint` and update `NextSceneName`.
3. **Whole-Number `NeonBlock.Size`:** `NeonBlock.Size` uses whole numbers; never pass fractional floats like `14.5` to `NeonBlock.Size`.
4. **Prefab Filename Typos:** `TutorialSIgn_*` prefabs have a capital `I`, and `ElementGate _Fire` may have a space before the underscore. Always search `Assets/Prefabs/` flexibly when instantiating.
5. **Duplicate `Vfx` in `LevelCore`:** `LevelCore` has historically contained two nested `Vfx` instances (one disabled). Keep only one active `Vfx` per scene so `Vfx.Instance` is unambiguous.
6. **Diamond / Atom Collect Burst:** `SetVisible` previously disabled `ParticleSystemRenderer` or lacked a child `FX_*` reference on older prefab copies; verify `CollectEffect` slots when placing collectibles.
7. **ScreenFade Edit-Time Alpha:** `ScreenFade` forces alpha at runtime; ensure edit-time alpha is `0` so it does not obscure the Scene view.
8. **Input Conflicts:** `Enter` and gamepad West overlap on `GroudPound` (spelled without `n` in the Input Action asset) and `Attack`; `GlideAction` slot on `CC_ELEMENTO` is optional/empty (falls back to Jump button).
9. **`PowerUpPickup.cs`:** Dead commented-out file; leave untouched unless explicitly asked to delete.
10. **`LevelManager` Scene Wrap:** Wraps back to build index `0` after the last scene in `EditorBuildSettings`.

## Risks
- **Scene References Inside Prefabs:** Links to scene objects outside a prefab are lost when placed in a new scene (`LevelCore` overrides wire `Player` to managers and `CameraRig`; `End_Portal` uses `ScreenFade` automatically when `FadeGroup` is empty).
- **Legacy Shaders:** Materials inside `Legacy/` folders must stay untouched; active materials must use URP `Elemento/*` shaders.
- **Root Scale vs. `NeonBlock.Size`:** Never stretch `Transform.localScale` on `NeonBlock` prefabs; always resize via `NeonBlock.Size`.

## Open Design Decisions
- `FAN.DRV` alternate name for Air boss.
- Ending glow vs. dark Aether state.
- What `Diamond` and `Atom` collectibles unlock in the Hub.