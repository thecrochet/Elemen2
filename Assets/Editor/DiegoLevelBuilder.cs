#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder
{
    private const string ScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO.unity";

    [MenuItem("Elemento/Build Diego Level (W_TEST_DIEGO)")]
    public static string Build()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        Debug.Log("[DiegoLevelBuilder] Starting build for: " + scene.name);

        // 1. Clean up or find root containers
        Transform levelCore = GameObject.Find("LevelCore")?.transform;
        if (levelCore == null)
        {
            GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Managers/LevelCore.prefab");
            if (corePrefab != null)
            {
                GameObject coreObj = (GameObject)PrefabUtility.InstantiatePrefab(corePrefab);
                coreObj.name = "LevelCore";
                levelCore = coreObj.transform;
            }
        }

        // Clean up any test debris or temporary objects
        GameObject staleTest = GameObject.Find("TestDebrisLayer8");
        if (staleTest != null) Object.DestroyImmediate(staleTest);

        // Ensure clean root containers
        Transform environment = EnsureContainer("Environment");
        Transform interactables = EnsureContainer("Interactables");
        Transform hazards = EnsureContainer("Hazards");
        Transform collectibles = EnsureContainer("Collectibles");
        Transform checkpoints = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        // Clear previous generated contents in containers if any, while preserving LevelCore
        ClearContainer(environment);
        ClearContainer(interactables);
        ClearContainer(hazards);
        ClearContainer(collectibles);
        ClearContainer(checkpoints);
        ClearContainer(flow);

        // Clean up old loose objects from original W_TEST if still present
        CleanOldLooseObjects();

        // 2. Load Prefab Library
        var p = new PrefabLib();

        // 3. Setup Flow: StartPortal & Goal
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

        // End Portal will be placed at Section 6 finish
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 1080f, 4.5f);
        endPortal.name = "End_Portal";

        // Wire LevelGoal exit sequence if needed
        var levelGoal = endPortal.GetComponent<LevelGoal>();
        var exitSeq = endPortal.GetComponent<LevelExitSequence>();
        if (levelGoal != null && exitSeq != null)
        {
            levelGoal.ExitSequence = exitSeq;
        }

        // Tutorial Signs
        p.Spawn(p.TutorialJump, flow, 8f, 0f);
        p.Spawn(p.TutorialDash, flow, 16f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 785f, 16f);

        // Camera Zones
        GameObject camClimb = p.Spawn(p.CameraZoneClimb, flow, 453f, 12f);
        SetTriggerBoxSize(camClimb, new Vector3(20f, 25f, 10f));

        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 590f, 16f);
        SetTriggerBoxSize(camWide, new Vector3(40f, 25f, 10f));

        // 4. Setup Level Frame & Global KillPit
        GameObject levelFrame = p.Spawn(p.LevelFrame, environment, -15f, -1f);
        levelFrame.name = "LevelFrame";
        AdjustLevelFrame(levelFrame, -16f, 1105f, 32f, -1f);

        // Global bottom KillPit
        GameObject globalKillPit = p.Spawn(p.KillPit, hazards, 540f, -8f);
        globalKillPit.name = "KillPit_GlobalBottom";
        SetBoxSize(globalKillPit, new Vector3(1250f, 4f, 10f));

        // -------------------------------------------------------------
        // SECTION 1: "Ignition Grounds" (X = 0 to 160)
        // -------------------------------------------------------------
        // Start highway
        p.Spawn(p.Ground20, environment, 0f, -1f);
        p.Spawn(p.Ground20, environment, 20f, -1f);

        // Stepping stone color blocks
        p.Spawn(p.BlockCyan, environment, 28f, 1f);
        p.Spawn(p.BlockGreen, environment, 32f, 2f);
        p.Spawn(p.BlockMagenta, environment, 36f, 3f);

        // Collectible: Atom 0
        GameObject atom0 = p.Spawn(p.Atom, collectibles, 36f, 4.8f);
        SetAtomId(atom0, 0);

        // Elevated ledge & slopes
        p.Spawn(p.Ledge8, environment, 40f, 3f);
        p.Spawn(p.Slope45, environment, 48f, 2f);
        p.Spawn(p.Slope45, environment, 49f, 1f);

        // Ground with spike gap
        p.Spawn(p.Ground20, environment, 50f, -1f);
        p.Spawn(p.SpikeRow6, hazards, 58f, 0f);
        p.Spawn(p.BouncePad, interactables, 54f, 0f);
        p.Spawn(p.Ledge8, environment, 66f, 3f);

        // Lava pit gap with LavaLeapers
        p.Spawn(p.Ledge8, environment, 76f, 0f);
        p.Spawn(p.Ledge8, environment, 88f, 0f);
        p.Spawn(p.Ledge8, environment, 100f, 0f);
        p.Spawn(p.LavaLeaper, hazards, 82f, -4f);
        p.Spawn(p.LavaLeaper, hazards, 94f, -4f);
        p.Spawn(p.LavaLeaper, hazards, 106f, -4f);

        // Section 1 Smash Gate: 5-wide, 4-tall DashWall corridor
        p.Spawn(p.Ground20, environment, 114f, -1f);
        p.Spawn(p.Ceiling20, environment, 114f, 5f);
        for (int x = 121; x <= 125; x++)
        {
            for (int y = 0; y <= 3; y++)
            {
                p.Spawn(p.DashWall, interactables, x, y);
            }
        }

        // SpinArm hazard exiting corridor
        p.Spawn(p.SpinArm, hazards, 136f, 3f);
        p.Spawn(p.Ledge8, environment, 144f, 2f);

        // -------------------------------------------------------------
        // SECTION 2: "Volcanic Ascents & Timed Flames" (X = 160 to 360)
        // -------------------------------------------------------------
        // FireJet Staircase
        p.Spawn(p.Ledge8, environment, 162f, 4.5f);
        p.Spawn(p.FireJet, hazards, 167f, 4.5f);

        p.Spawn(p.Ledge8, environment, 172f, 7.0f);
        p.Spawn(p.FireJetLeft, hazards, 177f, 7.0f);

        p.Spawn(p.Ledge8, environment, 182f, 9.5f);
        p.Spawn(p.FireJetRight, hazards, 187f, 9.5f);

        p.Spawn(p.Ledge8, environment, 193f, 15.5f);
        p.Spawn(p.FireJet, hazards, 198f, 15.5f);

        p.Spawn(p.Ledge8, environment, 203f, 18.0f);
        p.Spawn(p.FireJetDown, hazards, 208f, 18.0f);

        // High Secret Ledge: The Diamond (ID 0)
        p.Spawn(p.Ledge8, environment, 194f, 20.0f);
        GameObject diamond = p.Spawn(p.Diamond, collectibles, 198f, 21.5f);
        SetDiamondId(diamond, 0);

        // Cannon Artillery Descent
        p.Spawn(p.Ledge8, environment, 216f, 14.0f);
        p.Spawn(p.Cannon, hazards, 221f, 21.0f);

        p.Spawn(p.Ledge8, environment, 231f, 10.0f);
        p.Spawn(p.CannonFast, hazards, 236f, 17.0f);

        p.Spawn(p.Ledge8, environment, 246f, 6.0f);
        p.Spawn(p.Cannon, hazards, 251f, 13.0f);

        p.Spawn(p.Ledge8, environment, 261f, 2.0f);
        p.Spawn(p.CannonFast, hazards, 266f, 9.0f);

        p.Spawn(p.Ledge8, environment, 276f, 0.0f);

        // Aerial Lava Gap with DashZone & HoldZone
        p.Spawn(p.DashZone, interactables, 290f, 1.5f);
        p.Spawn(p.LavaLeaperTall, hazards, 290f, -4.0f);
        p.Spawn(p.Ledge8, environment, 297f, 0.0f);

        p.Spawn(p.DashZone, interactables, 308f, 2.0f);
        p.Spawn(p.LavaLeaperTall, hazards, 324f, -4.0f);
        p.Spawn(p.Ledge8, environment, 315f, 2.0f);

        p.Spawn(p.DashHold, interactables, 329f, 6.5f);
        p.Spawn(p.DashHold, interactables, 343f, 9.0f);
        p.Spawn(p.LavaLeaper, hazards, 348f, -4.0f);
        p.Spawn(p.Ledge8, environment, 355f, 4.0f);

        // -------------------------------------------------------------
        // SECTION 3: "The Mechanical Core" (X = 360 to 560)
        // -------------------------------------------------------------
        // Moving platform over chasm
        GameObject movPlat = p.Spawn(p.MovingPlatform, interactables, 372f, 4.0f);
        ConfigureMovingPlatform(movPlat, new Vector3(372f, 4f, 0f), new Vector3(412f, 4f, 0f), 3.5f);
        p.Spawn(p.SawSlow, hazards, 392f, 7.0f);

        // Landing & StompBlock gauntlet on IceSurface
        p.Spawn(p.Ground20, environment, 420f, 3.0f);
        p.Spawn(p.IceSurface, interactables, 425f, 4.0f);
        p.Spawn(p.StompBlock, hazards, 432f, 9.0f);
        p.Spawn(p.CannonUp, hazards, 442f, 3.0f);

        // Vertical Wall Jump Shaft
        p.Spawn(p.Wall12, environment, 450f, 10.0f);
        p.Spawn(p.Wall12, environment, 458f, 10.0f);
        p.Spawn(p.SpikeLeft, hazards, 457.5f, 6.0f);
        p.Spawn(p.SpikeRight, hazards, 450.5f, 9.0f);
        p.Spawn(p.OneWayPlatform, interactables, 454f, 12.0f);
        p.Spawn(p.OneWayLarge, interactables, 466f, 14.0f);

        // Crumbling Block bridge
        p.Spawn(p.CrumblingBlock, interactables, 478f, 12.0f);
        p.Spawn(p.CrumblingBlock, interactables, 483f, 12.0f);
        p.Spawn(p.CrumblingBlock, interactables, 488f, 12.0f);
        p.Spawn(p.CrumblingBlock, interactables, 493f, 12.0f);
        p.Spawn(p.SpikeRowDown, hazards, 485f, 6.0f);

        // Collectible: Atom 1
        GameObject atom1 = p.Spawn(p.Atom, collectibles, 488f, 14.0f);
        SetAtomId(atom1, 1);

        // Slope descent to Central Sanctuary
        p.Spawn(p.Slope45Left, environment, 505f, 10.0f);
        p.Spawn(p.Slope45Left, environment, 515f, 7.0f);
        p.Spawn(p.Ground20, environment, 530f, 3.0f);

        // CENTRAL CHECKPOINT (Exactly one in the scene)
        GameObject cp = p.Spawn(p.Checkpoint, checkpoints, 545f, 4.0f);
        cp.name = "Checkpoint_Central";

        // -------------------------------------------------------------
        // SECTION 4: "Aerial Transit & Dash Transit" (X = 560 to 780)
        // -------------------------------------------------------------
        // Mega Bounce Pad launch into DashHold
        p.Spawn(p.BouncePadMega, interactables, 565f, 4.0f);
        p.Spawn(p.DashHold, interactables, 575f, 16.0f);
        p.Spawn(p.DashLaneUp, interactables, 590f, 18.0f);

        // Mid-air DashWall Column
        for (int y = 17; y <= 20; y++)
        {
            p.Spawn(p.DashWall, interactables, 605, y);
        }
        p.Spawn(p.Ledge8, environment, 615f, 14.0f);

        // High-altitude fire tunnel
        p.Spawn(p.Ceiling20, environment, 630f, 19.0f);
        p.Spawn(p.SpikeRowDown, hazards, 630f, 18.0f);
        p.Spawn(p.SpikeRowDown, hazards, 640f, 18.0f);

        p.Spawn(p.OneWayShort, interactables, 630f, 12.0f);
        p.Spawn(p.FireJetRight, hazards, 626f, 13.0f);

        p.Spawn(p.DashBoostFast, interactables, 642f, 12.5f);
        p.Spawn(p.OneWayShort, interactables, 654f, 12.0f);
        p.Spawn(p.FireJetLeft, hazards, 668f, 13.0f);

        // Mid-air DashCharge orb
        p.Spawn(p.DashCharge, interactables, 678f, 13.0f);
        p.Spawn(p.Ledge8, environment, 690f, 10.0f);

        // Upper branch with Fire Gate and Atom 2
        p.Spawn(p.ElementGateFire, interactables, 705f, 15.0f);
        p.Spawn(p.Ledge8, environment, 712f, 15.0f);
        GameObject atom2 = p.Spawn(p.Atom, collectibles, 714f, 16.5f);
        SetAtomId(atom2, 2);

        // Slope descent
        p.Spawn(p.Slope45Large, environment, 725f, 12.0f);
        p.Spawn(p.Ground20, environment, 745f, 4.0f);
        p.Spawn(p.SawFast, hazards, 755f, 7.0f);
        p.Spawn(p.Ledge8, environment, 770f, 5.0f);

        // -------------------------------------------------------------
        // SECTION 5: "The Elemental Forge & Shatter Gauntlet" (X = 780 to 960)
        // -------------------------------------------------------------
        // Ground Pound Shatter Tower
        p.Spawn(p.Ledge8, environment, 785f, 15.0f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 792f, 12.0f);
        p.Spawn(p.IceSurface, interactables, 792f, 3.0f);
        p.Spawn(p.StompBlock, hazards, 804f, 9.0f);
        p.Spawn(p.CannonUp, hazards, 816f, 2.0f);

        p.Spawn(p.Ledge8, environment, 825f, 4.0f);

        // Rapid BouncePad Mini chain over leaping lava
        p.Spawn(p.BouncePadMini, interactables, 842f, 3.0f);
        p.Spawn(p.LavaLeaperTall, hazards, 852f, -4.0f);

        p.Spawn(p.BouncePadMini, interactables, 862f, 3.0f);
        // Atom 3 in mid-air above bounce chain
        GameObject atom3 = p.Spawn(p.Atom, collectibles, 862f, 8.5f);
        SetAtomId(atom3, 3);

        p.Spawn(p.LavaLeaperCeiling, hazards, 872f, 14.0f);
        p.Spawn(p.BouncePadMini, interactables, 882f, 3.0f);

        // Elemental Gate Water & approach
        p.Spawn(p.Ground20, environment, 895f, 3.0f);
        p.Spawn(p.ElementGateWater, interactables, 915f, 4.0f);
        p.Spawn(p.FireJetDown, hazards, 930f, 11.0f);
        p.Spawn(p.OneWayPlatform, interactables, 930f, 5.0f);
        p.Spawn(p.Ledge8, environment, 945f, 4.0f);

        // -------------------------------------------------------------
        // SECTION 6: "The Grand Victory Run" (X = 960 to 1100)
        // -------------------------------------------------------------
        // Grand Speed Tunnel: Ground & Ceiling
        p.Spawn(p.Ground20, environment, 960f, 3.0f);
        p.Spawn(p.Ground20, environment, 980f, 3.0f);
        p.Spawn(p.Ground20, environment, 1000f, 3.0f);
        p.Spawn(p.Ground20, environment, 1020f, 3.0f);

        p.Spawn(p.Ceiling20, environment, 960f, 9.0f);
        p.Spawn(p.Ceiling20, environment, 980f, 9.0f);
        p.Spawn(p.Ceiling20, environment, 1000f, 9.0f);
        p.Spawn(p.Ceiling20, environment, 1020f, 9.0f);

        // Triple cascading DashWall smash gates
        // Gate 1 (X = 975)
        for (int y = 4; y <= 7; y++)
        {
            p.Spawn(p.DashWall, interactables, 975, y);
        }
        p.Spawn(p.DashBoostFast, interactables, 985f, 4.5f);

        // Gate 2 (X = 995)
        for (int y = 4; y <= 7; y++)
        {
            p.Spawn(p.DashWall, interactables, 995, y);
        }
        p.Spawn(p.DashBoostFast, interactables, 1005f, 4.5f);

        // Gate 3 (X = 1015)
        for (int y = 4; y <= 7; y++)
        {
            p.Spawn(p.DashWall, interactables, 1015, y);
        }

        // Final celebratory bridge & Mega BouncePad
        p.Spawn(p.Ground20, environment, 1040f, 3.0f);
        p.Spawn(p.Ground20, environment, 1060f, 3.0f);
        p.Spawn(p.BouncePadMega, interactables, 1065f, 4.0f);

        // Final landing platform for End_Portal
        p.Spawn(p.Ground20, environment, 1075f, 3.0f);

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
        return $"BUILD COMPLETED! Level length ~1090 units. Validation errors: {errors}";
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

    private static void ClearContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(container.GetChild(i).gameObject);
        }
    }

    private static void CleanOldLooseObjects()
    {
        string[] oldRoots = new string[]
        {
            "ENVIRONMENT", "LavaLeaper", "LavaLeaper (1)", "LavaLeaper (2)", "LavaLeaper (3)",
            "LavaLeaper (4)", "LavaLeaper (5)", "LavaLeaper (6)", "Cannon", "Cannon (1)",
            "Cannon (2)", "Cannon (3)", "DashZone", "DashZone (1)", "DashZone (2)",
            "HoldZone", "HoldZone (1)", "DashWall_ChainRow 1", "DashWall_ChainRow 1 (1)",
            "DashWall_ChainRow 1 (2)", "DashWall_ChainRow 1 (3)", "DashWall_ChainRow 1 (4)",
            "SpinArm", "CheckPoint", "CheckPoint (1)", "FireJet", "FireJet (1)",
            "FireJet (2)", "FireJet (3)", "FireJet (4)", "LevelFrame_40x14", "ControlPrompt",
            "StartPortal", "End_Portal", "Final_Platform"
        };

        foreach (string name in oldRoots)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null && obj.transform.parent == null)
            {
                Object.DestroyImmediate(obj);
            }
        }
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
        public GameObject Block1x1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Block_1x1.prefab");
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject BlockGreen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Green Variant.prefab");
        public GameObject BlockMagenta = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab");
        public GameObject Wall12 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Wall_1x12.prefab");
        public GameObject Ceiling20 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ceiling_20x1.prefab");
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");
        public GameObject LevelFrame = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/LevelFrame_40x14.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject OneWayLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject OneWayShort = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject CrumblingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/CrumblingBlock.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject CrackBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/CrackBlock.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject BouncePadMini = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mini.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");
        public GameObject DashBoost = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashBoost.prefab");
        public GameObject DashBoostUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Up.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashHold = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashHold.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject ElementGateFire = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate _Fire.prefab");
        public GameObject ElementGateWater = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Water.prefab");

        public GameObject Spike = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Spike.prefab");
        public GameObject SpikeLeft = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Spike_Left.prefab");
        public GameObject SpikeRight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Spike_Right.prefab");
        public GameObject SpikeRow6 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/SpikeRow_6.prefab");
        public GameObject SpikeRowDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/SpikeRow_Down.prefab");
        public GameObject SawSlow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Saw_Slow.prefab");
        public GameObject SawFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Saw_Fast.prefab");
        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject Cannon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Cannon.prefab");
        public GameObject CannonFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Cannon_Fast.prefab");
        public GameObject CannonUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Cannon_Up.prefab");
        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject FireJetLeft = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Left.prefab");
        public GameObject FireJetRight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Right.prefab");
        public GameObject FireJetDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Down.prefab");
        public GameObject LavaLeaper = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/LavaLeaper.prefab");
        public GameObject LavaLeaperTall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/LavaLeaper_Tall.prefab");
        public GameObject LavaLeaperCeiling = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/LavaLeaper_Ceiling.prefab");
        public GameObject StompBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/StompBlock.prefab");

        public GameObject Atom = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Atom.prefab");
        public GameObject Diamond = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Diamond.prefab");
        public GameObject Checkpoint = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/CheckPoint.prefab");
        public GameObject StartPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/StartPortal.prefab");
        public GameObject EndPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/End_Portal.prefab");
        public GameObject TutorialJump = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Jump.prefab");
        public GameObject TutorialDash = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Dash.prefab");
        public GameObject TutorialGroundPound = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_GroundPound.prefab");
        public GameObject CameraZoneClimb = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Climb.prefab");
        public GameObject CameraZoneWide = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Wide.prefab");

        public GameObject Spawn(GameObject prefab, Transform parent, float x, float y)
        {
            if (prefab == null)
            {
                Debug.LogError("[DiegoLevelBuilder] Null prefab passed to Spawn!");
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
