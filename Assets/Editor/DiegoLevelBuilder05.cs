#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder05
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO_05.unity";

    [MenuItem("Elemento/Build Diego Level 05 (Continuous Speed & Twin Chimneys)")]
    public static string Build()
    {
        Debug.Log("[DiegoLevelBuilder05] Creating new scene: " + TargetScenePath);

        // 1. Create a brand-new empty scene and save it to W_TEST_DIEGO_05.unity
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
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 720f, 3f);
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
        p.Spawn(p.TutorialJump, flow, 345f, 3f);

        // Camera Zones
        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 212f, 10f);
        SetTriggerBoxSize(camWide, new Vector3(60f, 35f, 10f));

        GameObject camGauntlet = p.Spawn(p.CameraZoneWide, flow, 640f, 10f);
        SetTriggerBoxSize(camGauntlet, new Vector3(80f, 35f, 10f));

        // Global Frame & Bottom KillPit (covers full extent)
        GameObject levelFrame = p.Spawn(p.LevelFrame, environment, -15f, -1f);
        levelFrame.name = "LevelFrame";
        AdjustLevelFrame(levelFrame, -16f, 745f, 40f, -2f);

        GameObject globalKillPit = p.Spawn(p.KillPit, hazards, 360f, -16f);
        globalKillPit.name = "KillPit_GlobalBottom";
        SetBoxSize(globalKillPit, new Vector3(950f, 6f, 10f));

        // =============================================================
        // CONTINUOUS BASE FLOOR (Zero large empty voids from X = 0 to 720)
        // =============================================================
        for (int fx = 0; fx <= 720; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, -1f);
        }

        // =============================================================
        // ACT 1: "The Kinetic Highway" (X = 0 to 180)
        // High speed Purple Ball smashing, forgiving hazards, and Chimney 1
        // =============================================================
        for (int cx = 20; cx <= 80; cx += 20)
        {
            p.Spawn(p.Ceiling20, environment, cx, 8f);
        }

        // Boost 1 into 3x3 DashWall (auto-smash)
        p.Spawn(p.DashBoostFast, interactables, 22f, 0.5f);
        for (int x = 30; x <= 32; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Forgiving hazards: Slow saw high up + small spike row with jump bounce pad
        p.Spawn(p.SawSlow, hazards, 46f, 5.5f);
        p.Spawn(p.BouncePad, interactables, 44f, 0f);
        p.Spawn(p.SpikeRow6, hazards, 48f, 0.5f);

        // Slope 45 transition up
        p.Spawn(p.Slope45, environment, 58f, 0f);

        // DashCharge & Diamond 1 embedded in Wall Cluster
        p.Spawn(p.DashCharge, interactables, 66f, 1.5f);
        p.Spawn(p.DashBoostFast, interactables, 70f, 1.5f);

        for (int x = 76; x <= 78; x++)
            for (int y = 1; y <= 3; y++)
            {
                if (x == 77 && y == 2)
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
        p.Spawn(p.OneWayPlatform, environment, 88f, 3f);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 1 (X = 104 to 108): Classic Wall-to-Wall Jumping
        // Spaced 4 units apart with breakable DashWall roof
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 104f, 7f);
        p.Spawn(p.Wall12, environment, 108f, 7f);

        // Optional BouncePad_Mini at the base of the chimney for accessibility
        p.Spawn(p.BouncePadMini, interactables, 106f, 0f);

        // Atoms in the chimney
        GameObject a1 = p.Spawn(p.Atom, collectibles, 106f, 3f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, collectibles, 106f, 6.5f);
        SetAtomId(a2, 2);
        GameObject a3 = p.Spawn(p.Atom, collectibles, 106f, 10f);
        SetAtomId(a3, 3);

        // Breakable roof at the top of Chimney 1
        for (int x = 105; x <= 107; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 13f);
        }

        // Exit shelf and slope descent
        p.Spawn(p.OneWayPlatform, environment, 114f, 12f);
        p.Spawn(p.Slope45Left, environment, 122f, 8f);
        p.Spawn(p.OneWayPlatform, environment, 128f, 5f);

        // Forgiving hazards: Telegraphed FireJet & StompBlock
        p.Spawn(p.FireJet, hazards, 138f, 0f);
        p.Spawn(p.StompBlock, hazards, 152f, 6f);

        // Bounce pad into elevated plateau
        p.Spawn(p.BouncePad, interactables, 162f, 0f);
        p.Spawn(p.Ledge8, environment, 168f, 2f);
        p.Spawn(p.Ground20, environment, 180f, 2f);

        // CHECKPOINT 1
        GameObject cp1 = p.Spawn(p.Checkpoint, checkpoints, 175f, 3f);
        cp1.name = "Checkpoint_1";

        // =============================================================
        // ACT 2: "The Vertical Citadel" (X = 180 to 360)
        // Chimney 2 (Speed Wall Rocket) + Dynamic Arena + Chimney 3
        // =============================================================
        p.Spawn(p.Ledge8, environment, 195f, 2f);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 2 (X = 210 to 214): Speed Wall Rocket Ascent
        // With CrackedBlocks_4x4 floor and DashWalls inside
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 210f, 8f);
        p.Spawn(p.Wall12, environment, 214f, 8f);

        // Vertical Speed Wall: DashLane_UP inside the chimney
        GameObject laneUp1 = p.Spawn(p.DashLaneUp, interactables, 212f, 7f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        p.Spawn(p.CrackedBlocks4x4, interactables, 212f, 9f);
        for (int x = 211; x <= 213; x++)
            p.Spawn(p.DashWall, interactables, x, 14f);

        // Diamond 2 at the rocket peak
        GameObject d2 = p.Spawn(p.Diamond, collectibles, 212f, 16f);
        SetDiamondId(d2, 2);

        // Exit shelf & CrumblingBlock bridge
        p.Spawn(p.OneWayPlatform, environment, 222f, 13f);
        p.Spawn(p.CrumblingBlock, environment, 230f, 11f);
        p.Spawn(p.CrumblingBlock, environment, 235f, 10f);

        // Mega bounce pad safety below the descent
        p.Spawn(p.BouncePadMega, interactables, 245f, 0f);

        // Arena: LavaLeaper, MovingPlatform 1, and SpinArm
        p.Spawn(p.LavaLeaper, hazards, 255f, 0f);

        GameObject mp1 = p.Spawn(p.MovingPlatform, interactables, 270f, 3f);
        ConfigureMovingPlatform(mp1, new Vector3(270f, 3f, 0f), new Vector3(298f, 3f, 0f), 3.5f);

        p.Spawn(p.SpinArm, hazards, 284f, 7f);

        // Atoms along the moving platform path
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 274f + i * 7f, 5f);
            SetAtomId(a, 4 + i);
        }

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 3 (X = 320 to 324): Wall-to-Wall Jumping
        // Spaced 4 units apart with breakable layers
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 320f, 7f);
        p.Spawn(p.Wall12, environment, 324f, 7f);

        p.Spawn(p.BouncePadMini, interactables, 322f, 0f);
        p.Spawn(p.DashWall, interactables, 322f, 2f);
        p.Spawn(p.DashWall, interactables, 322f, 13f);

        // Exit stairs down
        p.Spawn(p.OneWayPlatform, environment, 332f, 11f);
        p.Spawn(p.OneWayPlatform, environment, 342f, 7f);
        p.Spawn(p.OneWayPlatform, environment, 350f, 3f);

        // CHECKPOINT 2
        GameObject cp2 = p.Spawn(p.Checkpoint, checkpoints, 355f, 1f);
        cp2.name = "Checkpoint_2";

        // =============================================================
        // ACT 3: "The Frost & Fire Quarry" (X = 360 to 540)
        // Ice slide, Cannon dodge, Chimney 4 (Zig-Zag mini-bounce), and StompBlock
        // =============================================================
        p.Spawn(p.IceSurface, environment, 375f, 0f);
        p.Spawn(p.IceSurface, environment, 400f, 0f);

        p.Spawn(p.DashBoostFast, interactables, 375f, 0.5f);

        // Cannon firing left over ice
        p.Spawn(p.Cannon, hazards, 410f, 3.5f);

        // 4-wide wall smash on ice
        for (int x = 388; x <= 391; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // CrackedBlocks with Diamond 3
        p.Spawn(p.CrackedBlocks4x4, interactables, 418f, 1f);
        GameObject d3 = p.Spawn(p.Diamond, collectibles, 418f, 1f);
        SetDiamondId(d3, 3);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 4 (X = 448 to 452): Zig-Zag Mini-Bounces
        // Alternating mini bounce pads on walls for rapid climb
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 448f, 7f);
        p.Spawn(p.Wall12, environment, 452f, 7f);

        p.Spawn(p.BouncePadMini, interactables, 449f, 3.5f);
        p.Spawn(p.BouncePadMini, interactables, 451f, 7.5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 450f, 2f + i * 2.5f);
            SetAtomId(a, 8 + i);
        }

        for (int x = 449; x <= 451; x++)
            p.Spawn(p.DashWall, interactables, x, 13f);

        // Exit shelf
        p.Spawn(p.OneWayPlatform, environment, 460f, 11f);
        p.Spawn(p.Slope45Left, environment, 470f, 7f);

        // StompBlock & SpinArm
        p.Spawn(p.StompBlock, hazards, 485f, 5f);
        p.Spawn(p.SpinArm, hazards, 505f, 5f);
        p.Spawn(p.BouncePad, interactables, 505f, 0f);

        // Stepped DashWall quarry pyramid
        for (int x = 515; x <= 522; x++)
        {
            int h = (x <= 518) ? (x - 515 + 1) : (522 - x + 1);
            for (int y = 0; y <= h; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        }

        p.Spawn(p.Slope45, environment, 528f, 0f);

        // CHECKPOINT 3
        GameObject cp3 = p.Spawn(p.Checkpoint, checkpoints, 535f, 2f);
        cp3.name = "Checkpoint_3";

        // =============================================================
        // ACT 4: "The Supersonic Grand Finale" (X = 540 to 720)
        // Chained Boosters, Chimney 5 (Rocket Tower), and Victory Canyon
        // =============================================================

        // Chained Boosters & 3x3 Walls (3 consecutive smashes)
        int[] boostX = new int[] { 550, 572, 594 };
        int[] wallX = new int[] { 560, 582, 604 };
        for (int i = 0; i < boostX.Length; i++)
        {
            p.Spawn(p.DashBoostFast, interactables, boostX[i], 1.5f);
            for (int x = wallX[i]; x <= wallX[i] + 1; x++)
                for (int y = 0; y <= 2; y++)
                    p.Spawn(p.DashWall, interactables, x, y);
        }

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 5 (X = 628 to 632): Grand Rocket Tower
        // Speed Wall inside blowing through double cracked block tier
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 628f, 8f);
        p.Spawn(p.Wall12, environment, 632f, 8f);

        GameObject grandLane = p.Spawn(p.DashLaneUp, interactables, 630f, 7f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(14f, 3f));

        p.Spawn(p.CrackedBlocks4x4, interactables, 630f, 8f);
        GameObject d4 = p.Spawn(p.Diamond, collectibles, 630f, 9f);
        SetDiamondId(d4, 4);

        for (int x = 629; x <= 631; x++)
            p.Spawn(p.DashWall, interactables, x, 14f);

        // Mega Bounce Canyon across continuous ground
        p.Spawn(p.BouncePadMega, interactables, 655f, 0f);
        p.Spawn(p.BouncePadMega, interactables, 675f, 0f);
        p.Spawn(p.BouncePadMega, interactables, 695f, 0f);

        // Parabolic Atom arc
        for (int i = 0; i < 9; i++)
        {
            float ax = 645f + i * 7f;
            float ay = 8f - Mathf.Abs(i - 4) * 1.0f;
            GameObject a = p.Spawn(p.Atom, collectibles, ax, ay);
            SetAtomId(a, 12 + i);
        }

        // Victory Bridge & End Portal
        p.Spawn(p.Ground20, environment, 715f, 2f);

        GameObject d5 = p.Spawn(p.Diamond, collectibles, 712f, 4f);
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
        return $"BUILD 05 COMPLETED! Saved to {TargetScenePath}. Validation errors: {errors}";
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
        public GameObject Wall12 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Wall_1x12.prefab");
        public GameObject Ceiling20 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ceiling_20x1.prefab");
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");
        public GameObject LevelFrame = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/LevelFrame_40x14.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject CrumblingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/CrumblingBlock.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMini = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mini.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");

        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject Cannon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Cannon.prefab");
        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject StompBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/StompBlock.prefab");
        public GameObject SawSlow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Saw_Slow.prefab");
        public GameObject LavaLeaper = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/LavaLeaper.prefab");
        public GameObject SpikeRow6 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/SpikeRow_6.prefab");

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
                Debug.LogError("[DiegoLevelBuilder05] Null prefab passed to Spawn!");
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
