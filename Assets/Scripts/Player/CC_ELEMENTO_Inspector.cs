#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// EDITOR ONLY. Adds a panel at the top of the CC_ELEMENTO Inspector: one checkbox per power you OWN FOR GOOD
// (the same ones a boss will give, saved on this computer). Tick one and the ball has it at once, even while you play.
//
// Each row also tells you if the power really WORKS in this scene, and if not, why
// (the usual reason: the level's LevelLoadout does not allow it).
[CustomEditor(typeof(CC_ELEMENTO))]
public class CC_ELEMENTO_Inspector : Editor
{
    private static readonly PowerUpType[] Elements = { PowerUpType.Fire, PowerUpType.Air, PowerUpType.Earth, PowerUpType.Water };

    public override void OnInspectorGUI()
    {
        CC_ELEMENTO player = (CC_ELEMENTO)target;
        LevelLoadout loadout = FindFirstObjectByType<LevelLoadout>();

        // ---- what the game will compute (the same rules as CC_ELEMENTO.OwnsPower) ----
        int unlocked = (int)player.CheatOwnedPowers;

        if (player.UseSavedPowers)
            unlocked |= PowerSave.Mask;

        bool free = loadout != null && loadout.IsFreeMode;

        if (loadout != null && loadout.DebugUnlockAll)
            unlocked = int.MaxValue;

        int allowed = loadout != null ? (int)loadout.AllowedPowers : int.MaxValue;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Powers owned for good (saved on this computer)", EditorStyles.boldLabel);

        if (!player.UseSavedPowers)
        {
            EditorGUILayout.HelpBox("'Use Saved Powers' is OFF below, so these ticks have no effect yet.", MessageType.Warning);

            if (GUILayout.Button("Turn on 'Use Saved Powers'"))
            {
                serializedObject.Update();
                serializedObject.FindProperty("UseSavedPowers").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
        }

        bool changed = false;

        foreach (PowerUpType element in Elements)
        {
            int bit = PowerMaskUtil.Bit(element);
            bool saved = PowerSave.Has(element);

            // what does the game do with this power in THIS scene?
            string status;

            if ((unlocked & bit) == 0)
                status = "not owned";
            else if (!free && (allowed & bit) == 0)
                status = "owned, but this level does not allow it";
            else
                status = "works";

            EditorGUILayout.BeginHorizontal();
            bool now = EditorGUILayout.ToggleLeft(element.ToString(), saved, GUILayout.Width(110));
            EditorGUILayout.LabelField(status, status == "works" ? EditorStyles.label : EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            if (now != saved)
            {
                PowerSave.Set(element, now);
                changed = true;
            }
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Own all four"))
        {
            PowerSave.UnlockAll();
            changed = true;
        }

        if (GUILayout.Button("Clear all"))
        {
            PowerSave.Clear();
            changed = true;
        }

        EditorGUILayout.EndHorizontal();

        if (loadout == null)
            EditorGUILayout.HelpBox("No LevelLoadout in this scene: every power you own is allowed.", MessageType.None);
        else
            EditorGUILayout.HelpBox("LevelLoadout: Mode = " + loadout.Mode + ". In Play mode a power works only if you own it AND it is in 'Allowed Powers'. Free mode ignores the list.", MessageType.None);

        EditorGUILayout.EndVertical();

        if (changed && Application.isPlaying)
            player.RefreshPowers();

        EditorGUILayout.Space();
        DrawDefaultInspector();
    }
}
#endif
