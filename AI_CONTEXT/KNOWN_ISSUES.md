# Known issues and open decisions (read only on request)

## Verified bugs
1. Diamond/Atom `SetVisible` disables every renderer including the ParticleSystemRenderer, so the collect burst never shows. Fix pending (Kit10: skip ParticleSystemRenderer).
2. DestructibleObject has no CrackEffect slot; FX_CrackHit cannot play on non-breaking hits. Fix pending (Kit10).
3. ScreenFade is visible in the Scene view at edit time (alpha only forced at runtime). Fix pending (Kit10: Reset sets alpha 0). Hide the layer in the Scene view meanwhile.
4. ElementGate collect burst and CollectEffect slots empty on several prefabs; burst appears in the wrong place and untinted.
5. `PowerUpPickup.cs` is entirely commented out (dead code). Safe to delete only if the user says so.
6. Glide Action slot on CC_ELEMENTO is empty; glide uses the jump button (no dedicated glide input).
7. (Dropped Nov 17: input conflicts are not an issue, every button has a power.)
8. `FX_AmbientAether` does not exist (Vfx.cs treats it as optional and skips it). First task: create it.
9. `LevelManager` wraps to scene 0 after the last scene instead of stopping (not a bug yet, a decision for the hub).
10. Prefab `SecretDoor` exists in Prefabs/Level/Flow but no SecretDoor script was found (not verified).
11. Prefab filenames `TutorialSIgn_*` have a capital I typo. `ElementoPrefabBuilders.cs` holds two classes (ElementoMaterials, ElementoPrefabBuilders); harmless.
12. Not verified: compile errors and warnings, and per-field null Inspector slots (needs an Editor compile run).
13. `Saw` prefab links `Hazard.HitEffect` to the project asset FX_HazardHit.prefab, not to a child, so it never plays.
14. `LevelGoal.FinishEffect` is empty in End_Portal; the scene links a scene object, so the link is lost in a new scene.
15. LevelCore contains two nested Vfx instances (one disabled). Only one may exist.
16. All six `M_Gate_*` materials and `M_DashZone_Fire` point at Legacy shader copies; re-point them before editing the live shaders.

## Risks
- Several materials still use legacy shaders; they may turn pink on an upgrade.
- Links to scene objects outside a prefab are lost when the prefab is placed in another scene (LevelCore overrides wire Player to managers/camera).
- A second Vfx instance in a scene breaks Vfx.Instance.
- Portal exit animation / menu not appearing after finish: unresolved. End_Portal's LevelExitSequence and LevelGoal.ExitSequence slot look intact in the scan; check Missing Script, duplicate canvases, LevelHUD and PauseMenu slots, Console. The Inspector errors (MissingReferenceException, SerializedObjectNotCreatableException) point at a destroyed object still selected; deselect before Play.
- FX_AmbientAir is too strong: suggested Simulation Space Local, Start Speed 0–0.3, Orbital 0, Radius 0.55, Rate about 4 (not applied).
- Enums are serialized as numbers: never reorder PowerUpType, DashZone modes or NeonBlock anchors.

## Cleanup list
Apply Kit10 (user decides) · add Glide action · rename `GroudPound` (needs re-link, low priority) · replace legacy shaders · wire or remove Attack/Interact/Look · empty placeholder ScriptableObjects and Legacy folders stay untouched.

## Open design decisions
Fan boss alternate name · ending glow vs dark Aether · Air/Water world objects · which bonus play mode Diamonds + Atoms unlock · secret level format · how a Time Attack loadout is displayed. (Resolved Nov 17: powers accumulate after each boss, so "all powers outside the final level" is answered.)
