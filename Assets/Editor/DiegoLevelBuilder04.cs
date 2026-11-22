#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder04
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO_04.unity";

    [MenuItem("Elemento/Build Diego Level 04 (Purple Ball Speed Smash)")]
    public static string Build()
    {
        Debug.Log("[DiegoLevelBuilder04] Creating new scene: " + TargetScenePath);

        // 1. Create a brand-new empty scene and save it to W_TEST_DIEGO_04.unity
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool created = EditorSceneManager.SaveScene(scene, TargetScenePath);
        if (!created)
        {
            return "ERROR: Failed to create and save " + TargetScenePath;
        }

        // 2. Clone LevelCore from LevelCore.prefab
        GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Managers/LevelCore.prefab");
        if (corePrefab == null) return "ERROR: LevelCore.prefab not found";
        GameObject coreObj = (GameObject)PrefabUtility.InstantiatePrefab(corePrefab);
        coreObj.name = "LevelCore";
        coreObj.transform.position = Vector3.zero;

        // Ensure exactly one Vfx exists in scene
        var vfxList = Object.FindObjectsByType<Vfx>(FindObjectsSortMode.None);
        if (vfxList.Length > 1)
        {
            for (int i = 1; i < vfxList.Length; i++)
            {
                Object.DestroyImmediate(vfxList[i].gameObject);
            }
        }

        // Ensure clean root containers
        Transform environment = EnsureContainer("Environment");
        Transform interactables = EnsureContainer("Interactables");
        Transform hazards = EnsureContainer("Hazards");
        Transform collectibles = EnsureContainer("Collectibles");
        Transform checkpoints = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        // 3. Load Prefab Library
        var p = new PrefabLib();

        // 4. Setup Flow: StartPortal & Goal
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";

        // Wire RespawnManager spawn point to StartPortal
        var respawnMgr = Object.FindFirstObjectByType<RespawnManager>();
        if (respawnMgr != null)
        {
            respawnMgr.SpawnPoint = startPortal.transform;
            EditorUtility.SetDirty(respawnMgr);
            PrefabUtility.RecordPrefabInstancePropertyModifications(respawnMgr);
        }

        // End Portal at the grand finale
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 755f, 3f);
        endPortal.name = "End_Portal";

        var levelGoal = endPortal.GetComponent<LevelGoal>();
        var exitSeq = endPortal.GetComponent<LevelExitSequence>();
        if (levelGoal != null && exitSeq != null)
        {
            levelGoal.ExitSequence = exitSeq;
            EditorUtility.SetDirty(levelGoal);
            PrefabUtility.RecordPrefabInstancePropertyModifications(levelGoal);
        }

        // Tutorial Signs (Jump only - NO element gates)
        p.Spawn(p.TutorialJump, flow, 14f, 0f);
        p.Spawn(p.TutorialJump, flow, 340f, 3f);

        // Camera Zones
        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 208f, 10f);
        SetTriggerBoxSize(camWide, new Vector3(60f, 35f, 10f));

        GameObject camGauntlet = p.Spawn(p.CameraZoneWide, flow, 650f, 10f);
        SetTriggerBoxSize(camGauntlet, new Vector3(90f, 35f, 10f));

        // Global Frame & Bottom KillPit (covers full extent)
        GameObject levelFrame = p.Spawn(p.LevelFrame, environment, -15f, -1f);
        levelFrame.name = "LevelFrame";
        AdjustLevelFrame(levelFrame, -16f, 780f, 40f, -2f);

        GameObject globalKillPit = p.Spawn(p.KillPit, hazards, 380f, -16f);
        globalKillPit.name = "KillPit_GlobalBottom";
        SetBoxSize(globalKillPit, new Vector3(950f, 6f, 10f));

        // =============================================================
        // ACT 1: "The Booster Highway" (X = 0 to 180)
        // High speed Purple Ball smashing through breakable blockades
        // =============================================================

        // Continuous solid floor (Zero pits in Act 1)
        for (int fx = 0; fx <= 160; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, -1f);
        }
        for (int cx = 20; cx <= 100; cx += 20)
        {
            p.Spawn(p.Ceiling20, environment, cx, 8f);
        }

        // Boost 1 & Wall Cluster 1 (X = 20..30)
        p.Spawn(p.DashBoost, interactables, 20f, 0.5f);
        for (int x = 28; x <= 30; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Boost 2 & Diamond 1 embedded in Wall Cluster 2 (X = 40..62)
        p.Spawn(p.DashCharge, interactables, 40f, 0.5f);
        p.Spawn(p.DashBoostFast, interactables, 50f, 0.5f);

        for (int x = 60; x <= 62; x++)
            for (int y = 0; y <= 3; y++)
            {
                if (x == 61 && y == 1)
                {
                    GameObject d1 = p.Spawn(p.Diamond, collectibles, x, y);
                    SetDiamondId(d1, 1);
                }
                else
                {
                    p.Spawn(p.DashWall, interactables, x, y);
                }
            }

        // Elevated Platforms with generous landing zones
        p.Spawn(p.OneWayLarge, environment, 75f, 3f);
        p.Spawn(p.OneWayLarge, environment, 90f, 4f);

        // Mid-air DashBoost & suspended DashWall cluster
        p.Spawn(p.DashBoost, interactables, 95f, 4.5f);
        for (int x = 105; x <= 108; x++)
            for (int y = 4; y <= 6; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Atoms in an arc
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 78f + i * 4f, 4.5f + (i == 1 || i == 2 ? 1f : 0f));
            SetAtomId(a, 1 + i);
        }

        // DashHold barrel launching into massive 4x4 wall
        p.Spawn(p.DashHold, interactables, 125f, 2f);
        for (int x = 138; x <= 141; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Bounce pad into elevated plateau
        p.Spawn(p.BouncePad, interactables, 158f, 0f);
        p.Spawn(p.Ledge8, environment, 168f, 2f);
        p.Spawn(p.Ground20, environment, 180f, 2f);

        // CHECKPOINT 1
        GameObject cp1 = p.Spawn(p.Checkpoint, checkpoints, 180f, 3f);
        cp1.name = "Checkpoint_1";

        // =============================================================
        // ACT 2: "Speed Wall Rocket Ascent" (X = 180 to 360)
        // Riding DashLane_UP speed walls upward through stacked breakable floors
        // =============================================================
        p.Spawn(p.Ledge8, environment, 195f, 2f);

        // Shaft 1 Chamber Walls
        for (int y = 0; y <= 16; y++)
        {
            p.Spawn(p.BlockCyan, environment, 204f, y);
            p.Spawn(p.BlockCyan, environment, 212f, y);
        }

        // Vertical Speed Wall: DashLane_UP
        GameObject laneUp1 = p.Spawn(p.DashLaneUp, interactables, 208f, 8f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(16f, 3f));

        // 3 Breakable layers inside the speed wall
        for (int x = 206; x <= 210; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 5f);
            p.Spawn(p.DashWall, interactables, x, 14f);
        }
        p.Spawn(p.CrackedBlocks4x4, interactables, 208f, 10f);

        // Diamond 2 at the rocket peak
        GameObject d2 = p.Spawn(p.Diamond, collectibles, 208f, 18f);
        SetDiamondId(d2, 2);

        // Landing shelves
        p.Spawn(p.OneWayLarge, environment, 222f, 14f);
        p.Spawn(p.Ledge8, environment, 235f, 12f);

        // Safety Mega Bounce Pad below the descent
        p.Spawn(p.Ground20, environment, 245f, -1f);
        p.Spawn(p.BouncePadMega, interactables, 245f, 0f);

        // Moving platform carrying player across
        GameObject mp1 = p.Spawn(p.MovingPlatform, interactables, 260f, 3f);
        ConfigureMovingPlatform(mp1, new Vector3(260f, 3f, 0f), new Vector3(290f, 3f, 0f), 3.5f);

        // Horizontal speed zone blasting through double wall
        GameObject horizLane = p.Spawn(p.DashZone, interactables, 275f, 4f);
        SetDashZoneMode(horizLane, DashZone.ZoneMode.Lane);
        SetDashZoneSize(horizLane, new Vector2(8f, 3f));

        for (int x = 285; x <= 286; x++)
            for (int y = 3; y <= 5; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Atoms along the moving platform path
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 265f + i * 6f, 5.5f);
            SetAtomId(a, 5 + i);
        }

        // Speed Wall 2 (Ascent 2)
        for (int y = 0; y <= 12; y++)
        {
            p.Spawn(p.BlockCyan, environment, 306f, y);
            p.Spawn(p.BlockCyan, environment, 314f, y);
        }
        GameObject laneUp2 = p.Spawn(p.DashLaneUp, interactables, 310f, 6f);
        laneUp2.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp2, new Vector2(12f, 3f));

        p.Spawn(p.CrackedBlocks4x4, interactables, 310f, 7f);

        p.Spawn(p.OneWayLarge, environment, 325f, 10f);
        p.Spawn(p.OneWayLarge, environment, 338f, 6f);
        p.Spawn(p.Ground20, environment, 350f, 2f);

        // CHECKPOINT 2
        GameObject cp2 = p.Spawn(p.Checkpoint, checkpoints, 355f, 3f);
        cp2.name = "Checkpoint_2";

        // =============================================================
        // ACT 3: "Quarry Blast & Barrel Cannons" (X = 360 to 540)
        // DashHold barrel hops, ice sliding, and speed lane smashing
        // =============================================================

        // Continuous safety floor across the bottom
        for (int fx = 360; fx <= 520; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, -1f);
        }

        // Barrel Hop 1 (DashHold aiming slightly up-right)
        GameObject hold1 = p.Spawn(p.DashHold, interactables, 375f, 3f);
        hold1.transform.rotation = Quaternion.Euler(0f, 0f, 15f);

        for (int x = 390; x <= 392; x++)
            for (int y = 4; y <= 6; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Barrel Hop 2 (DashHold aiming forward-down into cracked blocks)
        GameObject hold2 = p.Spawn(p.DashHold, interactables, 405f, 3f);
        hold2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        p.Spawn(p.CrackedBlocks4x4, interactables, 420f, 1f);
        GameObject d3 = p.Spawn(p.Diamond, collectibles, 420f, 1f);
        SetDiamondId(d3, 3);

        // Ice slide and multi-boost runway
        p.Spawn(p.IceSurface, environment, 435f, 0f);
        p.Spawn(p.IceSurface, environment, 465f, 0f);
        p.Spawn(p.IceSurface, environment, 495f, 0f);

        p.Spawn(p.DashBoostFast, interactables, 445f, 0.5f);
        for (int x = 455; x <= 457; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        p.Spawn(p.DashBoostFast, interactables, 468f, 0.5f);
        for (int x = 478; x <= 480; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Atoms along the ice run
        for (int i = 0; i < 6; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 440f + i * 8f, 1.5f);
            SetAtomId(a, 9 + i);
        }

        // Mega bounce pad launch to plateau
        p.Spawn(p.BouncePadMega, interactables, 510f, 0f);
        p.Spawn(p.Ground20, environment, 530f, 4f);

        // CHECKPOINT 3
        GameObject cp3 = p.Spawn(p.Checkpoint, checkpoints, 535f, 5f);
        cp3.name = "Checkpoint_3";

        // =============================================================
        // ACT 4: "The Supersonic Gauntlet" (X = 540 to 760)
        // Chained speed boosters, wall vaporizing, and grand canyon launcher
        // =============================================================

        // Chained booster runway floor
        for (int fx = 540; fx <= 620; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, 2f);
        }

        // Chained Boosters & Walls (4 consecutive speed smashes)
        int[] boostX = new int[] { 550, 570, 590, 610 };
        int[] wallX = new int[] { 560, 580, 600, 620 };
        for (int i = 0; i < boostX.Length; i++)
        {
            p.Spawn(p.DashBoostFast, interactables, boostX[i], 3.5f);
            for (int x = wallX[i]; x <= wallX[i] + 1; x++)
                for (int y = 3; y <= 5; y++)
                    p.Spawn(p.DashWall, interactables, x, y);
        }

        // Grand Speed Wall Launcher
        for (int y = 2; y <= 18; y++)
        {
            p.Spawn(p.BlockCyan, environment, 631f, y);
            p.Spawn(p.BlockCyan, environment, 639f, y);
        }
        GameObject grandLane = p.Spawn(p.DashLaneUp, interactables, 635f, 10f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(16f, 3f));

        p.Spawn(p.CrackedBlocks4x4, interactables, 635f, 10f);
        GameObject d4 = p.Spawn(p.Diamond, collectibles, 635f, 10f);
        SetDiamondId(d4, 4);

        // Mega Bounce Canyon (Safety floor with mega pads)
        p.Spawn(p.Ground20, environment, 660f, -2f);
        p.Spawn(p.Ground20, environment, 680f, -2f);
        p.Spawn(p.Ground20, environment, 700f, -2f);

        p.Spawn(p.BouncePadMega, interactables, 665f, -1f);
        p.Spawn(p.BouncePadMega, interactables, 685f, -1f);
        p.Spawn(p.BouncePadMega, interactables, 705f, -1f);

        // Atoms arc across the canyon
        for (int i = 0; i < 6; i++)
        {
            float ax = 650f + i * 11f;
            float ay = 10f - Mathf.Abs(i - 2.5f) * 1.5f;
            GameObject a = p.Spawn(p.Atom, collectibles, ax, ay);
            SetAtomId(a, 15 + i);
        }

        // Victory Bridge
        p.Spawn(p.Ground20, environment, 735f, 2f);
        p.Spawn(p.Ground20, environment, 755f, 2f);

        // Diamond 5 celebratory pickup
        GameObject d5 = p.Spawn(p.Diamond, collectibles, 742f, 4f);
        SetDiamondId(d5, 5);

        // 5. Ensure strictly Z = 0 on all spawned objects
        EnforceZZero(environment);
        EnforceZZero(interactables);
        EnforceZZero(hazards);
        EnforceZZero(collectibles);
        EnforceZZero(checkpoints);
        EnforceZZero(flow);

        // 6. Save scene and validate
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        int errors = ElementoLevelValidator.Run();
        return $"BUILD 04 COMPLETED! Saved to {TargetScenePath}. Validation errors: {errors}";
    }

    private static Transform EnsureContainer(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject(name);
            go.transform.position = Vector3.zero;
        }
        return go.transform;
    }

    private static void SetBoxSize(GameObject go, Vector3 size)
    {
        var box = go.GetComponent<BoxCollider>();
        if (box != null) box.size = size;
    }

    private static void SetTriggerBoxSize(GameObject go, Vector3 size)
    {
        var box = go.GetComponent<BoxCollider>();
        if (box != null)
        {
            box.isTrigger = true;
            box.size = size;
        }
    }

    private static void SetDashZoneSize(GameObject go, Vector2 size)
    {
        var zone = go.GetComponent<DashZone>();
        if (zone != null)
        {
            zone.Size = size;
            zone.Apply();
            EditorUtility.SetDirty(zone);
        }
    }

    private static void SetDashZoneMode(GameObject go, DashZone.ZoneMode mode)
    {
        var zone = go.GetComponent<DashZone>();
        if (zone != null)
        {
            zone.Mode = mode;
            zone.Apply();
            EditorUtility.SetDirty(zone);
        }
    }

    private static void SetAtomId(GameObject atomGo, int id)
    {
        var atom = atomGo.GetComponent<Atom>();
        if (atom != null)
        {
            var so = new SerializedObject(atom);
            so.FindProperty("Id").intValue = id;
            so.ApplyModifiedProperties();
        }
    }

    private static void SetDiamondId(GameObject diamondGo, int id)
    {
        var diamond = diamondGo.GetComponent<Diamond>();
        if (diamond != null)
        {
            var so = new SerializedObject(diamond);
            so.FindProperty("Id").intValue = id;
            so.ApplyModifiedProperties();
        }
    }

    private static void ConfigureMovingPlatform(GameObject plat, Vector3 start, Vector3 end, float speed)
    {
        var mp = plat.GetComponent<MovingPlatform>();
        if (mp != null)
        {
            var so = new SerializedObject(mp);
            var pointsProp = so.FindProperty("Points");
            if (pointsProp != null && pointsProp.isArray)
            {
                pointsProp.arraySize = 2;
                pointsProp.GetArrayElementAtIndex(0).vector3Value = start;
                pointsProp.GetArrayElementAtIndex(1).vector3Value = end;
            }
            so.FindProperty("Speed").floatValue = speed;
            so.ApplyModifiedProperties();
        }
    }

    private static void AdjustLevelFrame(GameObject frame, float leftX, float rightX, float ceilingY, float groundY)
    {
        Transform wl = frame.transform.Find("Wall_Left");
        if (wl != null) wl.position = new Vector3(leftX, 13f, 0f);

        Transform wr = frame.transform.Find("Wall_Right");
        if (wr != null) wr.position = new Vector3(rightX, 16f, 0f);

        Transform c = frame.transform.Find("Ceiling_Y14");
        if (c != null) c.position = new Vector3(leftX, ceilingY, 0f);

        Transform g = frame.transform.Find("Ground_40x1");
        if (g != null) g.position = new Vector3(leftX, groundY, 0f);
    }

    private static void EnforceZZero(Transform container)
    {
        foreach (Transform t in container.GetComponentsInChildren<Transform>(true))
        {
            Vector3 pos = t.position;
            if (Mathf.Abs(pos.z) > 0.001f)
            {
                t.position = new Vector3(pos.x, pos.y, 0f);
            }
        }
    }

    private class PrefabLib
    {
        public GameObject Ground20 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ground_20x1.prefab");
        public GameObject Ledge8 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ledge_8x1.prefab");
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject Ceiling20 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ceiling_20x1.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");
        public GameObject LevelFrame = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/LevelFrame_40x14.prefab");

        public GameObject OneWayLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashBoost = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashBoost.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashHold = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashHold.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");

        public GameObject Atom = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Atom.prefab");
        public GameObject Diamond = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Diamond.prefab");
        public GameObject Checkpoint = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/CheckPoint.prefab");
        public GameObject StartPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/StartPortal.prefab");
        public GameObject EndPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/End_Portal.prefab");
        public GameObject TutorialJump = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Jump.prefab");
        public GameObject CameraZoneWide = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Wide.prefab");

        public GameObject Spawn(GameObject prefab, Transform parent, float x, float y)
        {
            if (prefab == null)
            {
                Debug.LogError("[DiegoLevelBuilder04] Null prefab passed to Spawn!");
                return null;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.position = new Vector3(x, y, 0f);
            instance.transform.rotation = Quaternion.identity;
            return instance;
        }
    }
}
#endif
