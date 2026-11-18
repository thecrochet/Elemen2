#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Generates World 1 Level 1 (W1_L01) from TEMPLATE_SCENE.
// Menu: Elemento > Build W1_L01
public static class W1L01LevelBuilder
{
    private struct PlatformDef
    {
        public string Name;
        public float XStart;
        public float XEnd;
        public float TopY;

        public PlatformDef(string name, float xStart, float xEnd, float topY)
        {
            Name = name;
            XStart = xStart;
            XEnd = xEnd;
            TopY = topY;
        }
    }

    private static readonly PlatformDef[] Platforms = new PlatformDef[]
    {
        new PlatformDef("P1", 0f, 18f, 0f),
        new PlatformDef("P2", 21.5f, 36f, 0f),
        new PlatformDef("P3", 40.5f, 48.5f, 0f),
        new PlatformDef("P4", 53f, 61f, 1.5f),
        new PlatformDef("P5", 66f, 82f, 0f),
        new PlatformDef("Low P6A", 82f, 90f, 0f),
        new PlatformDef("Low P6B", 95.5f, 105.5f, 0f),
        new PlatformDef("High Ledge1", 88f, 96f, 2.8f),
        new PlatformDef("High Ledge2", 102.5f, 110.5f, 2.8f),
        new PlatformDef("Final P7", 111.5f, 152f, 0f)
    };

    [MenuItem("Elemento/Build W1_L01", false, 1)]
    public static void Build()
    {
        Debug.Log("[W1L01LevelBuilder] Starting build of W1_L01...");

        // 1. Ensure target folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Scenes/Levels"))
            AssetDatabase.CreateFolder("Assets/Scenes", "Levels");

        if (!AssetDatabase.IsValidFolder("Assets/Scenes/Levels/World1"))
            AssetDatabase.CreateFolder("Assets/Scenes/Levels", "World1");

        string targetScenePath = "Assets/Scenes/Levels/World1/W1_L01.unity";

        // 2. Identify template scene source
        string sourceScenePath = "Assets/Scenes/Templates/TEMPLATE_SCENE.unity";
        if (!File.Exists(sourceScenePath))
        {
            if (File.Exists("Assets/Scenes/Templates/TEMPLATE_SCENE_COPY.unity"))
                sourceScenePath = "Assets/Scenes/Templates/TEMPLATE_SCENE_COPY.unity";
            else if (File.Exists("Assets/Scenes/Levels/World1_Fire/TEMPLATE_SCENE.unity"))
                sourceScenePath = "Assets/Scenes/Levels/World1_Fire/TEMPLATE_SCENE.unity";
        }

        Debug.Log($"[W1L01LevelBuilder] Copying template from {sourceScenePath} to {targetScenePath}");

        if (File.Exists(targetScenePath))
        {
            AssetDatabase.DeleteAsset(targetScenePath);
        }

        bool copied = AssetDatabase.CopyAsset(sourceScenePath, targetScenePath);
        if (!copied)
        {
            Debug.LogError($"[W1L01LevelBuilder] Failed to copy {sourceScenePath} to {targetScenePath}");
            return;
        }

        AssetDatabase.Refresh();

        // 3. Open scene
        Scene scene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[W1L01LevelBuilder] Failed to open scene at {targetScenePath}");
            return;
        }

        // 4. Remove PrefabLab and ControlPrompt if present
        GameObject prefabLab = GameObject.Find("PrefabLab");
        if (prefabLab != null)
        {
            Debug.Log("[W1L01LevelBuilder] Removing PrefabLab from scene.");
            Object.DestroyImmediate(prefabLab);
        }

        GameObject controlPrompt = GameObject.Find("ControlPrompt");
        if (controlPrompt != null)
        {
            Debug.Log("[W1L01LevelBuilder] Removing ControlPrompt from scene.");
            Object.DestroyImmediate(controlPrompt);
        }

        // Remove any existing LevelFrame_40x14 in template to cleanly re-tile 4 frames
        GameObject existingFrame = GameObject.Find("LevelFrame_40x14");
        while (existingFrame != null)
        {
            Object.DestroyImmediate(existingFrame);
            existingFrame = GameObject.Find("LevelFrame_40x14");
        }

        // 5. Setup StartPortal, LevelCore/RespawnPoint, and LevelCore/Player at (2, 0, 0)
        GameObject startPortal = GameObject.Find("StartPortal");
        if (startPortal != null)
        {
            startPortal.transform.position = new Vector3(2f, 0f, 0f);
            startPortal.transform.rotation = Quaternion.identity;
            EditorUtility.SetDirty(startPortal);
        }
        else
        {
            Debug.LogWarning("[W1L01LevelBuilder] StartPortal not found in template scene.");
        }

        GameObject levelCore = GameObject.Find("LevelCore");
        Transform respawnPointT = null;
        if (levelCore != null)
        {
            // Player
            Transform playerT = levelCore.transform.Find("Player");
            if (playerT != null)
            {
                playerT.position = new Vector3(2f, 0f, 0f);
                CC_ELEMENTO player = playerT.GetComponent<CC_ELEMENTO>();
                if (player != null)
                {
                    player.StartingPower = PowerUpType.None;
                    EditorUtility.SetDirty(player);
                }
                EditorUtility.SetDirty(playerT);
            }
            else
            {
                Debug.LogWarning("[W1L01LevelBuilder] Player not found under LevelCore.");
            }

            // RespawnPoint
            respawnPointT = levelCore.transform.Find("RespawnPoint");
            if (respawnPointT == null)
            {
                GameObject rpGo = new GameObject("RespawnPoint");
                rpGo.transform.SetParent(levelCore.transform, false);
                respawnPointT = rpGo.transform;
            }
            respawnPointT.position = new Vector3(2f, 0f, 0f);
            EditorUtility.SetDirty(respawnPointT);

            // Wire RespawnManager
            RespawnManager rm = levelCore.GetComponentInChildren<RespawnManager>(true);
            if (rm != null)
            {
                rm.SpawnPoint = respawnPointT;
                EditorUtility.SetDirty(rm);
            }
        }
        else
        {
            Debug.LogError("[W1L01LevelBuilder] LevelCore not found in scene!");
        }

        // 6. Instantiate Platforms
        GameObject blockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Foundation/Block_1x1.prefab");
        if (blockPrefab == null)
        {
            Debug.LogError("[W1L01LevelBuilder] Could not find Block_1x1.prefab at Assets/Prefabs/Level/Foundation/Block_1x1.prefab");
            return;
        }

        GameObject platformsRoot = new GameObject("Platforms");
        platformsRoot.transform.position = Vector3.zero;

        foreach (PlatformDef def in Platforms)
        {
            CreatePlatform(blockPrefab, platformsRoot.transform, def);
        }

        // 7. Tile 4 LevelFrame_40x14 across X = 0..160
        GameObject framePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Foundation/LevelFrame_40x14.prefab");
        if (framePrefab != null)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject frame = PrefabUtility.InstantiatePrefab(framePrefab) as GameObject;
                if (frame == null) frame = Object.Instantiate(framePrefab);
                frame.name = $"LevelFrame_40x14_{i}";
                frame.transform.position = new Vector3(i * 40f, 0f, 0f);
                frame.transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            Debug.LogWarning("[W1L01LevelBuilder] LevelFrame_40x14 prefab not found.");
        }

        // 8. Span KillPit across X = -10..170 at Y = -5
        GameObject killPitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Foundation/KillPit.prefab");
        if (killPitPrefab != null)
        {
            GameObject killPit = PrefabUtility.InstantiatePrefab(killPitPrefab) as GameObject;
            if (killPit == null) killPit = Object.Instantiate(killPitPrefab);
            killPit.name = "KillPit";

            // Center is (-10 + 170) / 2 = 80, width is 180
            killPit.transform.position = new Vector3(80f, -5f, 0f);
            killPit.transform.rotation = Quaternion.identity;
            killPit.transform.localScale = Vector3.one;

            BoxCollider col = killPit.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.center = Vector3.zero;
                col.size = new Vector3(180f, col.size.y, col.size.z);
                EditorUtility.SetDirty(col);
            }

            Transform visual = killPit.transform.Find("Visual");
            if (visual != null)
            {
                visual.localPosition = Vector3.zero;
                visual.localScale = new Vector3(180f, visual.localScale.y, visual.localScale.z);
                EditorUtility.SetDirty(visual);
            }
        }
        else
        {
            Debug.LogWarning("[W1L01LevelBuilder] KillPit prefab not found.");
        }

        // 9. Flow & Collectibles
        // Atom at (50.75, 3.2, 0)
        GameObject atomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Flow/Atom.prefab");
        if (atomPrefab != null)
        {
            GameObject atom = PrefabUtility.InstantiatePrefab(atomPrefab) as GameObject;
            if (atom == null) atom = Object.Instantiate(atomPrefab);
            atom.name = "Atom";
            atom.transform.position = new Vector3(50.75f, 3.2f, 0f);
            atom.transform.rotation = Quaternion.identity;
            Atom atomComp = atom.GetComponent<Atom>();
            if (atomComp != null)
            {
                atomComp.Id = 0;
                EditorUtility.SetDirty(atomComp);
            }
        }
        else
        {
            Debug.LogWarning("[W1L01LevelBuilder] Atom prefab not found.");
        }

        // Checkpoint at (80, 0, 0)
        GameObject checkpointPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Flow/CheckPoint.prefab");
        if (checkpointPrefab != null)
        {
            GameObject checkpoint = PrefabUtility.InstantiatePrefab(checkpointPrefab) as GameObject;
            if (checkpoint == null) checkpoint = Object.Instantiate(checkpointPrefab);
            checkpoint.name = "CheckPoint";
            checkpoint.transform.position = new Vector3(80f, 0f, 0f);
            checkpoint.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("[W1L01LevelBuilder] CheckPoint prefab not found.");
        }

        // Diamond at (106.5, 3.8, 0)
        GameObject diamondPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Flow/Diamond.prefab");
        if (diamondPrefab != null)
        {
            GameObject diamond = PrefabUtility.InstantiatePrefab(diamondPrefab) as GameObject;
            if (diamond == null) diamond = Object.Instantiate(diamondPrefab);
            diamond.name = "Diamond";
            diamond.transform.position = new Vector3(106.5f, 3.8f, 0f);
            diamond.transform.rotation = Quaternion.identity;
            Diamond diamondComp = diamond.GetComponent<Diamond>();
            if (diamondComp != null)
            {
                diamondComp.Id = 0;
                EditorUtility.SetDirty(diamondComp);
            }
        }
        else
        {
            Debug.LogWarning("[W1L01LevelBuilder] Diamond prefab not found.");
        }

        // End_Portal at (144, 0, 0)
        GameObject endPortal = GameObject.Find("End_Portal");
        if (endPortal == null)
        {
            GameObject endPortalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Flow/End_Portal.prefab");
            if (endPortalPrefab != null)
            {
                endPortal = PrefabUtility.InstantiatePrefab(endPortalPrefab) as GameObject;
                if (endPortal == null) endPortal = Object.Instantiate(endPortalPrefab);
                endPortal.name = "End_Portal";
            }
        }

        if (endPortal != null)
        {
            endPortal.transform.position = new Vector3(144f, 0f, 0f);
            endPortal.transform.rotation = Quaternion.identity;
            EditorUtility.SetDirty(endPortal);

            // Wire LevelExitSequence.FadeGroup to LevelCore/LevelUI/ScreenFade if empty
            LevelExitSequence exitSeq = endPortal.GetComponentInChildren<LevelExitSequence>();
            if (exitSeq != null && exitSeq.FadeGroup == null)
            {
                ScreenFade screenFade = Object.FindAnyObjectByType<ScreenFade>();
                if (screenFade != null)
                {
                    CanvasGroup group = screenFade.GetComponent<CanvasGroup>();
                    if (group != null)
                    {
                        exitSeq.FadeGroup = group;
                        EditorUtility.SetDirty(exitSeq);
                        Debug.Log("[W1L01LevelBuilder] Wired End_Portal LevelExitSequence.FadeGroup to ScreenFade CanvasGroup.");
                    }
                }
            }
        }

        // 10. Star times on LevelManager
        LevelManager lm = Object.FindAnyObjectByType<LevelManager>();
        if (lm != null)
        {
            SerializedObject lmSo = new SerializedObject(lm);
            bool setStarTimes = false;

            string[] star3Props = { "Star3Time", "ThreeStarTime", "StarTime3", "GoldStarTime" };
            string[] star2Props = { "Star2Time", "TwoStarTime", "StarTime2", "SilverStarTime" };
            string[] star1Props = { "Star1Time", "OneStarTime", "StarTime1", "BronzeStarTime" };

            foreach (string p in star3Props)
            {
                SerializedProperty prop = lmSo.FindProperty(p);
                if (prop != null) { prop.floatValue = 12.5f; setStarTimes = true; }
            }
            foreach (string p in star2Props)
            {
                SerializedProperty prop = lmSo.FindProperty(p);
                if (prop != null) { prop.floatValue = 15.5f; setStarTimes = true; }
            }
            foreach (string p in star1Props)
            {
                SerializedProperty prop = lmSo.FindProperty(p);
                if (prop != null) { prop.floatValue = 22.0f; setStarTimes = true; }
            }

            SerializedProperty starArray = lmSo.FindProperty("StarTimes");
            if (starArray != null && starArray.isArray)
            {
                starArray.arraySize = 3;
                starArray.GetArrayElementAtIndex(0).floatValue = 22.0f;
                starArray.GetArrayElementAtIndex(1).floatValue = 15.5f;
                starArray.GetArrayElementAtIndex(2).floatValue = 12.5f;
                setStarTimes = true;
            }

            if (setStarTimes)
            {
                lmSo.ApplyModifiedProperties();
                Debug.Log("[W1L01LevelBuilder] Applied star times (3★: 12.5s, 2★: 15.5s, 1★: 22.0s) to LevelManager.");
            }
            else
            {
                Debug.Log("[W1L01LevelBuilder] Note: LevelManager has no serialized star time fields in the codebase. Star thresholds (3★: 12.5s, 2★: 15.5s, 1★: 22.0s) noted.");
            }
        }

        // 11. Save Scene
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[W1L01LevelBuilder] Completed building W1_L01 at {targetScenePath}. Scene saved: {saved}.");
    }

    private static GameObject CreatePlatform(GameObject prefab, Transform parent, PlatformDef def)
    {
        float width = def.XEnd - def.XStart;
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (go == null) go = Object.Instantiate(prefab);

        go.name = def.Name;
        if (parent != null)
            go.transform.SetParent(parent, false);

        go.transform.position = new Vector3(def.XStart, def.TopY, 0f);
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        NeonBlock block = go.GetComponent<NeonBlock>();
        if (block != null)
        {
            block.Anchor = NeonBlock.AnchorType.TopLeft;
            block.CellSize = 0.5f;
            block.Size = new Vector3Int(Mathf.RoundToInt(width * 2f), 2, 4);
            block.Apply();
            EditorUtility.SetDirty(block);
        }

        EditorUtility.SetDirty(go);
        return go;
    }
}
#endif
