#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder06
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO_06.unity";

    [MenuItem("Elemento/Build Diego Level 06 (Non-Linear Excavation & Dynamic Zoom)")]
    public static string Build()
    {
        Debug.Log("[DiegoLevelBuilder06] Creating new scene: " + TargetScenePath);

        // 1. Create a brand-new empty scene and save it to W_TEST_DIEGO_06.unity
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
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 720f, 4f);
        endPortal.name = "End_Portal";

        var levelGoal = endPortal.GetComponent<LevelGoal>();
        var exitSeq = endPortal.GetComponent<LevelExitSequence>();
        if (levelGoal != null && exitSeq != null)
        {
            levelGoal.ExitSequence = exitSeq;
            EditorUtility.SetDirty(levelGoal);
            PrefabUtility.RecordPrefabInstancePropertyModifications(levelGoal);
        }

        // Tutorial Signs
        p.Spawn(p.TutorialJump, flow, 14f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 36f, 5f);
        p.Spawn(p.TutorialJump, flow, 345f, 3f);

        // Global Frame & Bottom KillPit (covers full extent)
        GameObject levelFrame = p.Spawn(p.LevelFrame, environment, -15f, -1f);
        levelFrame.name = "LevelFrame";
        AdjustLevelFrame(levelFrame, -16f, 745f, 40f, -6f);

        GameObject globalKillPit = p.Spawn(p.KillPit, hazards, 360f, -20f);
        globalKillPit.name = "KillPit_GlobalBottom";
        SetBoxSize(globalKillPit, new Vector3(950f, 6f, 10f));

        // =============================================================
        // CONTINUOUS BASE FLOOR (Zero empty voids from X = 0 to 720)
        // =============================================================
        for (int fx = 0; fx <= 720; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, -1f);
        }

        // =============================================================
        // ACT 1: "Excavation Drop & The High Road" (X = 0 to 180)
        // Downward Ground Pound smash, high/low routes, and Chimney 1
        // =============================================================

        // Section 1: Elevated approach into Excavation Shaft 1
        p.Spawn(p.Slope45, environment, 24f, 0f);
        p.Spawn(p.OneWayPlatform, environment, 35f, 5f);

        // Camera Zone: Wide for Excavation Drop 1
        GameObject camDrop1 = p.Spawn(p.CameraZoneWide, flow, 52f, 2f);
        SetTriggerBoxSize(camDrop1, new Vector3(35f, 25f, 10f));

        // Sealed Excavation Drop 1 (X = 46 to 52)
        // Stacked horizontal breakables: Layer 1 at Y = 4, Layer 2 at Y = 1
        for (int x = 46; x <= 50; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 4f);
        }
        p.Spawn(p.CrackedBlocks4x4, interactables, 48f, 1f);

        // Lower Subterranean Room at Y = -2 (solid floor)
        p.Spawn(p.Ground20, environment, 50f, -3f);
        GameObject d1 = p.Spawn(p.Diamond, collectibles, 48f, -2f);
        SetDiamondId(d1, 1);

        GameObject a1 = p.Spawn(p.Atom, collectibles, 45f, -2f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, collectibles, 52f, -2f);
        SetAtomId(a2, 2);

        // Mega bounce pad launch out of lower room
        p.Spawn(p.BouncePadMega, interactables, 62f, -2f);

        // Section 2: Non-Linear Fork (Upper vs Lower)
        // Lower Route: Tight smash tunnel with CameraZone_Tight
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 95f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(35f, 12f, 10f));

        p.Spawn(p.DashBoostFast, interactables, 80f, 0.5f);
        for (int x = 90; x <= 92; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Upper Route: Twin-Wall Chimney 1 (X = 78 to 82)
        p.Spawn(p.Wall12, environment, 78f, 7f);
        p.Spawn(p.Wall12, environment, 82f, 7f);
        p.Spawn(p.BouncePadMini, interactables, 80f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 80f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        GameObject a3 = p.Spawn(p.Atom, collectibles, 80f, 4f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, collectibles, 80f, 8f);
        SetAtomId(a4, 4);

        // Upper Balcony
        p.Spawn(p.OneWayPlatform, environment, 88f, 11f);
        p.Spawn(p.OneWayPlatform, environment, 100f, 10f);
        p.Spawn(p.DashBoostFast, interactables, 102f, 10.5f);
        for (int x = 112; x <= 114; x++)
            p.Spawn(p.DashWall, interactables, x, 10f);

        // Slope descent connecting back
        p.Spawn(p.Slope45Left, environment, 124f, 6f);
        p.Spawn(p.Ground20, environment, 140f, 2f);

        // Forgiving hazards
        p.Spawn(p.FireJet, hazards, 145f, 2f);
        p.Spawn(p.SawSlow, hazards, 158f, 6.5f);
        p.Spawn(p.BouncePad, interactables, 162f, 2f);

        // CHECKPOINT 1
        GameObject cp1 = p.Spawn(p.Checkpoint, checkpoints, 175f, 4f);
        cp1.name = "Checkpoint_1";

        // =============================================================
        // ACT 2: "Deep Caverns & The Rocket Chimney" (X = 180 to 360)
        // Multi-story deep ground-pound descent & Speed Wall rocket
        // =============================================================
        p.Spawn(p.Ground20, environment, 195f, 3f);

        // Camera Zone: Wide for Deep Cavern Descent
        GameObject camDrop2 = p.Spawn(p.CameraZoneWide, flow, 215f, 0f);
        SetTriggerBoxSize(camDrop2, new Vector3(45f, 30f, 10f));

        // Deep 3-Tier Sealed Excavation Shaft (X = 208 to 216)
        // Stacked layers: Y = 3 (DashWall), Y = 0 (DashWall), Y = -3 (CrackedBlocks)
        for (int x = 208; x <= 214; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 3f);
            p.Spawn(p.DashWall, interactables, x, 0f);
        }
        p.Spawn(p.CrackedBlocks4x4, interactables, 211f, -3f);

        // Deep Subterranean Cavern at Y = -6
        p.Spawn(p.Ground20, environment, 212f, -7f);
        p.Spawn(p.Ground20, environment, 232f, -7f);

        GameObject d2 = p.Spawn(p.Diamond, collectibles, 211f, -5f);
        SetDiamondId(d2, 2);

        GameObject a5 = p.Spawn(p.Atom, collectibles, 206f, -5f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, collectibles, 218f, -5f);
        SetAtomId(a6, 6);

        // Bounce launch back to surface
        p.Spawn(p.BouncePadMega, interactables, 226f, -6f);
        p.Spawn(p.OneWayPlatform, environment, 238f, 2f);

        // Arena: LavaLeaper, MovingPlatform 1, and SpinArm
        p.Spawn(p.LavaLeaper, hazards, 252f, 0f);

        GameObject mp1 = p.Spawn(p.MovingPlatform, interactables, 265f, 3f);
        ConfigureMovingPlatform(mp1, new Vector3(265f, 3f, 0f), new Vector3(292f, 3f, 0f), 3.5f);

        p.Spawn(p.SpinArm, hazards, 278f, 7f);

        GameObject a7 = p.Spawn(p.Atom, collectibles, 272f, 5f);
        SetAtomId(a7, 7);
        GameObject a8 = p.Spawn(p.Atom, collectibles, 285f, 5f);
        SetAtomId(a8, 8);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 2 (X = 306 to 310): Speed Wall Rocket Tower
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 306f, 8f);
        p.Spawn(p.Wall12, environment, 310f, 8f);

        GameObject camClimb2 = p.Spawn(p.CameraZoneClimb, flow, 308f, 9f);
        SetTriggerBoxSize(camClimb2, new Vector3(16f, 22f, 10f));

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, interactables, 308f, 7f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 307; x <= 309; x++)
            p.Spawn(p.DashWall, interactables, x, 13f);

        // Elevated catwalk exit
        p.Spawn(p.OneWayPlatform, environment, 318f, 13f);
        p.Spawn(p.OneWayPlatform, environment, 330f, 9f);
        p.Spawn(p.OneWayPlatform, environment, 342f, 5f);

        // CHECKPOINT 2
        GameObject cp2 = p.Spawn(p.Checkpoint, checkpoints, 355f, 3f);
        cp2.name = "Checkpoint_2";

        // =============================================================
        // ACT 3: "The Multi-Story Quarry" (X = 360 to 540)
        // Tight smash expressway, Excavation Pit 3, and Chimney 3
        // =============================================================

        // Section 1: Tight Smash Expressway with CameraZone_Tight
        GameObject camTight2 = p.Spawn(p.CameraZoneTight, flow, 395f, 3f);
        SetTriggerBoxSize(camTight2, new Vector3(45f, 14f, 10f));

        p.Spawn(p.Ceiling20, environment, 385f, 6f);
        p.Spawn(p.Ceiling20, environment, 405f, 6f);

        p.Spawn(p.IceSurface, environment, 380f, 0f);
        p.Spawn(p.IceSurface, environment, 400f, 0f);

        p.Spawn(p.DashBoostFast, interactables, 375f, 0.5f);
        for (int x = 386; x <= 389; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        p.Spawn(p.DashBoostFast, interactables, 398f, 0.5f);
        for (int x = 408; x <= 410; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Cannon overhead hazard
        p.Spawn(p.Cannon, hazards, 418f, 4f);

        // Section 2: Excavation Pit 3 (Down to Y = -4)
        GameObject camDrop3 = p.Spawn(p.CameraZoneWide, flow, 432f, 0f);
        SetTriggerBoxSize(camDrop3, new Vector3(30f, 22f, 10f));

        p.Spawn(p.CrackedBlocks4x4, interactables, 430f, 0f);
        p.Spawn(p.Ground20, environment, 432f, -5f);

        GameObject d3 = p.Spawn(p.Diamond, collectibles, 430f, -3f);
        SetDiamondId(d3, 3);

        p.Spawn(p.BouncePadMega, interactables, 438f, -4f);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 3 (X = 452 to 456): Alternating Mini-Bounces
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 452f, 7f);
        p.Spawn(p.Wall12, environment, 456f, 7f);

        GameObject camClimb3 = p.Spawn(p.CameraZoneClimb, flow, 454f, 8f);
        SetTriggerBoxSize(camClimb3, new Vector3(16f, 20f, 10f));

        p.Spawn(p.BouncePadMini, interactables, 453f, 3.5f);
        p.Spawn(p.BouncePadMini, interactables, 455f, 7.5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 454f, 2.5f + i * 2.5f);
            SetAtomId(a, 9 + i);
        }

        for (int x = 453; x <= 455; x++)
            p.Spawn(p.DashWall, interactables, x, 13f);

        // Exit shelf
        p.Spawn(p.OneWayPlatform, environment, 464f, 11f);
        p.Spawn(p.Slope45Left, environment, 474f, 7f);

        // StompBlock & SpinArm
        p.Spawn(p.StompBlock, hazards, 488f, 5f);
        p.Spawn(p.SpinArm, hazards, 508f, 5f);
        p.Spawn(p.BouncePad, interactables, 508f, 0f);

        // Stepped DashWall quarry pyramid
        for (int x = 516; x <= 524; x++)
        {
            int h = (x <= 520) ? (x - 516 + 1) : (524 - x + 1);
            for (int y = 0; y <= h; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        }

        // CHECKPOINT 3
        GameObject cp3 = p.Spawn(p.Checkpoint, checkpoints, 535f, 2f);
        cp3.name = "Checkpoint_3";

        // =============================================================
        // ACT 4: "The Supersonic Citadel" (X = 540 to 720)
        // Chained express smash, Grand Rocket Chimney, and Canyon
        // =============================================================

        // Chained Boosters & 3x3 Walls (CameraZone_Tight)
        GameObject camTight3 = p.Spawn(p.CameraZoneTight, flow, 575f, 3f);
        SetTriggerBoxSize(camTight3, new Vector3(45f, 14f, 10f));

        int[] boostX = new int[] { 550, 570, 590 };
        int[] wallX = new int[] { 560, 580, 600 };
        for (int i = 0; i < boostX.Length; i++)
        {
            p.Spawn(p.DashBoostFast, interactables, boostX[i], 1.5f);
            for (int x = wallX[i]; x <= wallX[i] + 1; x++)
                for (int y = 0; y <= 2; y++)
                    p.Spawn(p.DashWall, interactables, x, y);
        }

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 4 (X = 618 to 622): Grand Rocket Tower
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 618f, 8f);
        p.Spawn(p.Wall12, environment, 622f, 8f);

        // Camera Zone: Wide for Grand Rocket Tower Launch
        GameObject camWideGrand = p.Spawn(p.CameraZoneWide, flow, 620f, 10f);
        SetTriggerBoxSize(camWideGrand, new Vector3(30f, 30f, 10f));

        GameObject grandLane = p.Spawn(p.DashLaneUp, interactables, 620f, 7f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(16f, 3f));

        p.Spawn(p.CrackedBlocks4x4, interactables, 620f, 9f);
        GameObject d4 = p.Spawn(p.Diamond, collectibles, 620f, 10f);
        SetDiamondId(d4, 4);

        for (int x = 619; x <= 621; x++)
            p.Spawn(p.DashWall, interactables, x, 14f);

        // Grand Bounce Canyon (CameraZone_Wide)
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 675f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(65f, 30f, 10f));

        p.Spawn(p.BouncePadMega, interactables, 650f, 0f);
        p.Spawn(p.BouncePadMega, interactables, 672f, 0f);
        p.Spawn(p.BouncePadMega, interactables, 694f, 0f);

        // Parabolic Atom arc
        for (int i = 0; i < 8; i++)
        {
            float ax = 644f + i * 8f;
            float ay = 8f - Mathf.Abs(i - 3.5f) * 1.0f;
            GameObject a = p.Spawn(p.Atom, collectibles, ax, ay);
            SetAtomId(a, 13 + i);
        }

        // Victory Bridge & End Portal
        p.Spawn(p.Ground20, environment, 715f, 3f);

        GameObject d5 = p.Spawn(p.Diamond, collectibles, 712f, 5f);
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
        return $"BUILD 06 COMPLETED! Saved to {TargetScenePath}. Validation errors: {errors}";
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
        public GameObject TutorialGroundPound = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_GroundPound.prefab");
        public GameObject CameraZoneWide = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Wide.prefab");
        public GameObject CameraZoneTight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Tight.prefab");
        public GameObject CameraZoneClimb = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Climb.prefab");

        public GameObject Spawn(GameObject prefab, Transform parent, float x, float y)
        {
            if (prefab == null)
            {
                Debug.LogError("[DiegoLevelBuilder06] Null prefab passed to Spawn!");
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
