#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// A level checker. Click  Elemento > Validate Current Level  and read the Console:
// red = something is broken, yellow = look at it, white = information. Click a line to select the object.
// It only READS the open scene (nothing is changed or saved).
//
// Checks: missing scripts, double or missing managers (Vfx, Sfx, RespawnManager, LevelManager), empty important slots
// (player, exit sequence, fade group, HUD, pause menu), two Diamonds or Atoms with the same Id, more than one Checkpoint,
// Destructible objects not on layer 6, unnamed layers, particle objects under Vfx without a valid FX_ name,
// scaled DashZone / NeonBlock roots, and gameplay objects that are not on the player's Z plane.
public static class ElementoLevelValidator
{
    private static int _errors;
    private static int _warnings;

    [MenuItem("Elemento/Validate Current Level")]
    public static void ValidateMenu()
    {
        Run();
    }

    // Public so a CLI call or another script can use it. Returns the number of errors.
    public static int Run()
    {
        _errors = 0;
        _warnings = 0;

        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("[Validate] Scene: " + scene.name);

        CheckMissingScripts();
        CheckManagers();
        CheckSlots();
        CheckCollectibleIds();
        CheckCheckpoints();
        CheckLayers();
        CheckVfxNames();
        CheckScales();
        CheckPlane();

        string summary = "[Validate] Done: " + _errors + " errors, " + _warnings + " warnings.";

        if (_errors > 0) Debug.LogError(summary);
        else if (_warnings > 0) Debug.LogWarning(summary);
        else Debug.Log(summary + " All good.");

        return _errors;
    }

    // ------------------------------------------------------------------
    private static void Error(string message, Object context)
    {
        _errors++;
        Debug.LogError("[Validate] " + message, context);
    }

    private static void Warn(string message, Object context)
    {
        _warnings++;
        Debug.LogWarning("[Validate] " + message, context);
    }

    private static T[] All<T>() where T : Object
    {
        return Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    // ------------------------------------------------------------------
    private static void CheckMissingScripts()
    {
        foreach (Transform t in All<Transform>())
        {
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);

            if (count > 0)
                Error("Missing Script (" + count + ") on '" + t.name + "'.", t.gameObject);
        }
    }

    private static void CheckManagers()
    {
        ExactlyOne(All<RespawnManager>(), "RespawnManager");
        ExactlyOne(All<LevelManager>(), "LevelManager");
        ExactlyOne(All<Vfx>(), "Vfx");
        ExactlyOne(All<Sfx>(), "Sfx");

        if (All<LevelStart>().Length == 0)
            Warn("No LevelStart in the scene.", null);

        if (All<LevelGoal>().Length == 0)
            Warn("No LevelGoal in the scene (the level cannot be finished).", null);
    }

    private static void ExactlyOne<T>(T[] found, string label) where T : Object
    {
        if (found.Length == 0)
            Warn("No " + label + " in the scene.", null);
        else if (found.Length > 1)
            Error(found.Length + " x " + label + " in the scene (only one allowed).", found[0]);
    }

    private static void CheckSlots()
    {
        foreach (RespawnManager r in All<RespawnManager>())
        {
            if (r.Player == null) Error("RespawnManager: Player is empty.", r);
            if (r.SpawnPoint == null) Warn("RespawnManager: SpawnPoint is empty.", r);
            if (r.RestartAction == null) Warn("RespawnManager: RestartAction is empty (no restart key).", r);
        }

        foreach (LevelManager m in All<LevelManager>())
        {
            if (m.Player == null) Error("LevelManager: Player is empty.", m);
            if (m.MoveAction == null || m.JumpAction == null) Warn("LevelManager: Move/Jump action is empty (the clock cannot start on input).", m);
        }

        foreach (LevelGoal g in All<LevelGoal>())
        {
            if (g.ExitSequence == null) Warn("LevelGoal '" + g.name + "': ExitSequence is empty (no exit animation).", g);
        }

        foreach (LevelExitSequence e in All<LevelExitSequence>())
        {
            if (e.PortalCenter == null) Warn("LevelExitSequence '" + e.name + "': PortalCenter is empty.", e);
            // an empty Fade Group is fine when the level has a ScreenFade: the exit sequence uses it
            if (e.FadeGroup == null && All<ScreenFade>().Length == 0) Warn("LevelExitSequence '" + e.name + "': FadeGroup is empty and the level has no ScreenFade (no fade to black).", e);
        }

        foreach (LevelHUD h in All<LevelHUD>())
        {
            if (h.TimerText == null) Warn("LevelHUD: TimerText is empty.", h);
            if (h.AttemptsText == null) Warn("LevelHUD: AttemptsText is empty.", h);
            if (h.FinishPanel == null) Warn("LevelHUD: FinishPanel is empty (no finish screen).", h);
            if (h.FinishTimeText == null || h.BestTimeText == null) Warn("LevelHUD: FinishTimeText or BestTimeText is empty.", h);
        }

        foreach (PauseMenu p in All<PauseMenu>())
        {
            if (p.PausePanel == null) Warn("PauseMenu: PausePanel is empty.", p);
            if (p.ResumeButton == null) Warn("PauseMenu: ResumeButton is empty.", p);
            // PauseAction is optional: when empty the menu listens to Esc and the gamepad Start button
        }

        foreach (Hazard h in All<Hazard>())
        {
            if (h.GetComponent<Collider>() == null && h.GetComponentInParent<Collider>() == null)
                Warn("Hazard '" + h.name + "' has no Collider (it cannot kill).", h);
        }
    }

    private static void CheckCollectibleIds()
    {
        Dictionary<int, Diamond> diamonds = new Dictionary<int, Diamond>();

        foreach (Diamond d in All<Diamond>())
        {
            if (diamonds.ContainsKey(d.Id))
                Error("Two Diamonds share Id " + d.Id + " ('" + d.name + "' and '" + diamonds[d.Id].name + "'): they would share one save slot.", d);
            else
                diamonds.Add(d.Id, d);

            if (d.CollectEffect == null) Warn("Diamond '" + d.name + "': CollectEffect is empty.", d);
        }

        Dictionary<int, Atom> atoms = new Dictionary<int, Atom>();

        foreach (Atom a in All<Atom>())
        {
            if (atoms.ContainsKey(a.Id))
                Error("Two Atoms share Id " + a.Id + " ('" + a.name + "' and '" + atoms[a.Id].name + "'): they would share one save slot.", a);
            else
                atoms.Add(a.Id, a);

            if (a.CollectEffect == null) Warn("Atom '" + a.name + "': CollectEffect is empty.", a);
        }
    }

    private static void CheckCheckpoints()
    {
        Checkpoint[] list = All<Checkpoint>();

        if (list.Length > 1)
            Warn(list.Length + " Checkpoints in the level (the design allows one).", list[0]);
    }

    private static void CheckLayers()
    {
        Dictionary<int, int> unnamed = new Dictionary<int, int>();
        Dictionary<int, GameObject> firstUnnamed = new Dictionary<int, GameObject>();

        foreach (Transform t in All<Transform>())
        {
            int layer = t.gameObject.layer;

            if (string.IsNullOrEmpty(LayerMask.LayerToName(layer)))
            {
                if (!unnamed.ContainsKey(layer))
                {
                    unnamed.Add(layer, 0);
                    firstUnnamed.Add(layer, t.gameObject);
                }

                unnamed[layer]++;
            }
        }

        foreach (KeyValuePair<int, int> pair in unnamed)
            Warn(pair.Value + " objects are on layer " + pair.Key + " which has no name (first one: '" + firstUnnamed[pair.Key].name + "').", firstUnnamed[pair.Key]);

        foreach (DestructibleObject d in All<DestructibleObject>())
        {
            if (d.gameObject.layer != 6)
                Warn("DestructibleObject '" + d.name + "' is not on layer 6 (Destructible).", d);
        }
    }

    private static void CheckVfxNames()
    {
        foreach (Vfx vfx in All<Vfx>())
        {
            foreach (ParticleSystem ps in vfx.GetComponentsInChildren<ParticleSystem>(true))
            {
                // only the top particle object of each effect has to follow the naming
                if (ps.transform.parent != vfx.transform) continue;

                if (!ps.name.StartsWith("FX_"))
                    Warn("Vfx child '" + ps.name + "' does not start with FX_ (it is ignored).", ps);
                else if (!System.Enum.TryParse<VfxType>(ps.name.Substring(3), out _))
                    Warn("Vfx child '" + ps.name + "' is not a known VfxType (it is ignored).", ps);
            }
        }
    }

    private static void CheckScales()
    {
        foreach (DashZone z in All<DashZone>())
            if (z.transform.localScale != Vector3.one)
                Warn("DashZone '" + z.name + "' has a root scale. Resize with the Size field instead.", z);

        foreach (NeonBlock b in All<NeonBlock>())
            if (b.transform.localScale != Vector3.one)
                Warn("NeonBlock '" + b.name + "' has a root scale. Resize with its Size fields instead.", b);
    }

    // Gameplay objects should sit on the same Z plane as the player
    private static void CheckPlane()
    {
        CC_ELEMENTO[] players = All<CC_ELEMENTO>();

        if (players.Length == 0) return;

        float planeZ = players[0].transform.position.z;

        CheckZ(All<Hazard>(), planeZ);
        CheckZ(All<Checkpoint>(), planeZ);
        CheckZ(All<Diamond>(), planeZ);
        CheckZ(All<Atom>(), planeZ);
        CheckZ(All<ElementGate>(), planeZ);
        CheckZ(All<DashZone>(), planeZ);
        CheckZ(All<MovingPlatform>(), planeZ);
        CheckZ(All<BouncePad>(), planeZ);
    }

    private static void CheckZ<T>(T[] list, float planeZ) where T : Component
    {
        foreach (T c in list)
        {
            if (Mathf.Abs(c.transform.position.z - planeZ) > 0.05f)
                Warn(typeof(T).Name + " '" + c.name + "' is at Z " + c.transform.position.z.ToString("0.##") + " (the player is at " + planeZ.ToString("0.##") + ").", c);
        }
    }
}
#endif
