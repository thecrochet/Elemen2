#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ElementoFinalPurpleSync
{
    public const string FinalPurpleDir = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE";

    [MenuItem("Elemento/Sync FINAL_PURPLE Levels")]
    public static string SyncLevels()
    {
        Debug.Log("[ElementoFinalPurpleSync] Starting sync of FINAL_PURPLE levels...");

        if (!Directory.Exists(FinalPurpleDir))
        {
            Directory.CreateDirectory(FinalPurpleDir);
            AssetDatabase.Refresh();
        }

        // 1. Gather and sort scenes numerically
        string[] rawFiles = Directory.GetFiles(FinalPurpleDir, "Level_*.unity", SearchOption.TopDirectoryOnly);
        var sortedScenePaths = rawFiles
            .Select(p => p.Replace('\\', '/'))
            .OrderBy(GetSceneNumber)
            .ToList();

        if (sortedScenePaths.Count == 0)
        {
            string msg = $"[ElementoFinalPurpleSync] No Level_*.unity scenes found in {FinalPurpleDir}";
            Debug.LogWarning(msg);
            return msg;
        }

        Debug.Log($"[ElementoFinalPurpleSync] Found {sortedScenePaths.Count} levels to sync.");

        // 2. Wire End_Portal, LevelGoal, LevelExitSequence, LevelManager in each scene
        for (int i = 0; i < sortedScenePaths.Count; i++)
        {
            string scenePath = sortedScenePaths[i];
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            string nextSceneName = (i + 1 < sortedScenePaths.Count)
                ? Path.GetFileNameWithoutExtension(sortedScenePaths[i + 1])
                : ""; // Last level falls back to Hub / loop

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // LevelGoal & ExitSequence
            var goal = UnityEngine.Object.FindFirstObjectByType<LevelGoal>();
            var exitSeq = UnityEngine.Object.FindFirstObjectByType<LevelExitSequence>();
            if (goal != null)
            {
                goal.NextSceneName = nextSceneName;
                if (exitSeq != null)
                {
                    goal.ExitSequence = exitSeq;
                }
                EditorUtility.SetDirty(goal);
                PrefabUtility.RecordPrefabInstancePropertyModifications(goal);
            }

            if (exitSeq != null)
            {
                exitSeq.NextSceneName = nextSceneName;
                EditorUtility.SetDirty(exitSeq);
                PrefabUtility.RecordPrefabInstancePropertyModifications(exitSeq);
            }

            // LevelManager
            var lm = UnityEngine.Object.FindFirstObjectByType<LevelManager>();
            if (lm != null)
            {
                lm.NextSceneName = nextSceneName;
                EditorUtility.SetDirty(lm);
                PrefabUtility.RecordPrefabInstancePropertyModifications(lm);
            }

            // RespawnManager wiring check
            var respawnMgr = UnityEngine.Object.FindFirstObjectByType<RespawnManager>();
            var startPortal = GameObject.Find("StartPortal");
            if (respawnMgr != null && startPortal != null)
            {
                respawnMgr.SpawnPoint = startPortal.transform;
                EditorUtility.SetDirty(respawnMgr);
                PrefabUtility.RecordPrefabInstancePropertyModifications(respawnMgr);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[ElementoFinalPurpleSync] Wired {sceneName} -> Next: '{(string.IsNullOrEmpty(nextSceneName) ? "[AUTO / END]" : nextSceneName)}'");
        }

        // 3. Register in EditorBuildSettings in exact sorted order
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

        // Keep non-FINAL_PURPLE scenes (e.g. 00_Tutorial) if they exist
        foreach (var existing in EditorBuildSettings.scenes)
        {
            if (!existing.path.Replace('\\', '/').Contains("/FINAL_PURPLE/"))
            {
                buildScenes.Add(existing);
            }
        }

        // Add FINAL_PURPLE scenes in sorted order
        foreach (string path in sortedScenePaths)
        {
            buildScenes.Add(new EditorBuildSettingsScene(path, true));
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        AssetDatabase.SaveAssets();

        string summary = $"[ElementoFinalPurpleSync] SUCCESS: Synced {sortedScenePaths.Count} levels in {FinalPurpleDir} and updated EditorBuildSettings ({buildScenes.Count} total scenes).";
        Debug.Log(summary);
        return summary;
    }

    private static int GetSceneNumber(string scenePath)
    {
        string name = Path.GetFileNameWithoutExtension(scenePath);
        Match match = Regex.Match(name, @"Level_(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
        {
            return num;
        }
        return int.MaxValue;
    }
}
#endif
