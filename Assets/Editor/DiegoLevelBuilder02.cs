#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder02
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO_02.unity";

    [MenuItem("Elemento/Build Diego Level 02 (Destruction & Excavation)")]
    public static string Build()
    {
        Debug.Log("[DiegoLevelBuilder02] Creating new scene: " + TargetScenePath);

        // 1. Create a brand-new empty scene and save it to W_TEST_DIEGO_02.unity
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
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 870f, 3.5f);
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
        p.Spawn(p.TutorialDash, flow, 12f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 218f, 0f);
        p.Spawn(p.TutorialJump, flow, 450f, 2f);

        // Camera Zones
        GameObject camClimb = p.Spawn(p.CameraZoneClimb, flow, 290f, 24f);
        SetTriggerBoxSize(camClimb, new Vector3(30f, 40f, 10f));

        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 480f, 10f);
        SetTriggerBoxSize(camWide, new Vector3(90f, 30f, 10f));

        // Global Frame & Bottom KillPit
        GameObject levelFrame = p.Spawn(p.LevelFrame, environment, -15f, -1f);
        levelFrame.name = "LevelFrame";
        AdjustLevelFrame(levelFrame, -16f, 890f, 45f, -1f);

        GameObject globalKillPit = p.Spawn(p.KillPit, hazards, 440f, -12f);
        globalKillPit.name = "KillPit_GlobalBottom";
        SetBoxSize(globalKillPit, new Vector3(1050f, 6f, 10f));

        // =============================================================
        // ACT 1: "The Dash Tunnel" (X = 0 to 220)
        // Horizontal smash excavation with thick DashWall barricades
        // =============================================================
        p.Spawn(p.ElementGateFire, interactables, 6f, 0f); // Fire Dash gate

        // Tunnel floor & ceiling
        p.Spawn(p.Ground20, environment, 0f, -1f);
        p.Spawn(p.Ground20, environment, 20f, -1f);
        p.Spawn(p.Ceiling20, environment, 20f, 6f);
        p.Spawn(p.Ground20, environment, 40f, -1f);
        p.Spawn(p.Ceiling20, environment, 40f, 6f);

        // Barricade 1: 4-deep, 4-tall DashWall wall (16 blocks)
        for (int x = 25; x <= 28; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Rolling pocket with DashBoost
        p.Spawn(p.DashBoostFast, interactables, 33f, 0.5f);

        // Barricade 2: 4-deep, 4-tall DashWall wall (16 blocks)
        for (int x = 38; x <= 41; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Hazard gap: CrumblingBlock over spikes
        p.Spawn(p.CrumblingBlock, interactables, 45f, 0f);
        p.Spawn(p.CrumblingBlock, interactables, 48f, 0f);
        p.Spawn(p.SpikeRow6, hazards, 46.5f, -4f);

        // Barricade 3: with CrackBlock
        p.Spawn(p.Ground20, environment, 52f, -1f);
        p.Spawn(p.Ceiling20, environment, 52f, 6f);
        for (int x = 52; x <= 55; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        p.Spawn(p.CrackBlock, interactables, 53f, 1f);

        // Open lava jump with Mega BouncePad
        p.Spawn(p.BouncePadMega, interactables, 60f, 0f);
        p.Spawn(p.LavaLeaper, hazards, 68f, -4f);
        GameObject atom0 = p.Spawn(p.Atom, collectibles, 68f, 7.5f);
        SetAtomId(atom0, 0);

        // Barricade 4: 5-deep, 4-tall DashWall wall (20 blocks)
        p.Spawn(p.Ground20, environment, 74f, -1f);
        p.Spawn(p.Ceiling20, environment, 74f, 6f);
        for (int x = 76; x <= 80; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // DashHold mid-air aiming pocket
        p.Spawn(p.DashHold, interactables, 86f, 2f);

        // Barricade 5: 6-deep, 4-tall DashWall wall (24 blocks)
        p.Spawn(p.Ground20, environment, 94f, -1f);
        p.Spawn(p.Ceiling20, environment, 94f, 6f);
        for (int x = 94; x <= 99; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Saws & Staggered Pillars
        p.Spawn(p.Ground20, environment, 114f, -1f);
        p.Spawn(p.SawSlow, hazards, 118f, 2f);
        for (int y = 0; y <= 3; y++) p.Spawn(p.DashWall, interactables, 126, y);
        p.Spawn(p.Ground20, environment, 134f, -1f);
        for (int y = 0; y <= 3; y++) p.Spawn(p.DashWall, interactables, 138, y);

        // Wall jump climb into elevated corridor
        p.Spawn(p.Wall12, environment, 146f, 6f);
        p.Spawn(p.Wall12, environment, 154f, 6f);
        p.Spawn(p.OneWayPlatform, interactables, 150f, 8f);
        p.Spawn(p.Ledge8, environment, 160f, 8f);
        p.Spawn(p.FireJetRight, hazards, 156f, 9f);
        p.Spawn(p.FireJetLeft, hazards, 172f, 9f);

        // Barricade 6
        p.Spawn(p.Ledge8, environment, 170f, 8f);
        for (int x = 175; x <= 178; x++)
            for (int y = 8; y <= 11; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Descent to Act 1 finale
        p.Spawn(p.Slope45Left, environment, 185f, 6f);
        p.Spawn(p.Slope45Left, environment, 195f, 3f);
        p.Spawn(p.Ground20, environment, 205f, -1f);

        // CHECKPOINT 1
        GameObject cp1 = p.Spawn(p.Checkpoint, checkpoints, 205f, 0f);
        cp1.name = "Checkpoint_Act1";

        // =============================================================
        // ACT 2: "Ground Pound Excavation Shaft" (X = 220 to 380)
        // Earth power gate, vertical climb, and 4-tier downward slam
        // =============================================================
        p.Spawn(p.ElementGateEarth, interactables, 214f, 0f); // Earth power gate

        // Vertical Ascent Section
        p.Spawn(p.Ledge8, environment, 225f, 3f);
        p.Spawn(p.OneWayLarge, interactables, 235f, 8f);
        p.Spawn(p.CannonFast, hazards, 245f, 15f);
        p.Spawn(p.OneWayPlatform, interactables, 248f, 14f);

        p.Spawn(p.Wall12, environment, 255f, 22f);
        p.Spawn(p.Wall12, environment, 263f, 22f);
        p.Spawn(p.SpikeLeft, hazards, 262.5f, 18f);
        p.Spawn(p.SpikeRight, hazards, 255.5f, 24f);
        p.Spawn(p.OneWayPlatform, interactables, 259f, 26f);

        p.Spawn(p.Ledge8, environment, 270f, 30f);
        p.Spawn(p.StompBlock, hazards, 274f, 36f);
        p.Spawn(p.OneWayLarge, interactables, 282f, 35f);

        // Perch above excavation shaft
        p.Spawn(p.Ledge8, environment, 290f, 38f);

        // Secret Diamond (ID 0) on upper ledge
        p.Spawn(p.Ledge8, environment, 266f, 40f);
        GameObject diamond = p.Spawn(p.Diamond, collectibles, 268f, 41.5f);
        SetDiamondId(diamond, 0);

        // The Vertical Excavation Shaft:
        // Left & Right boundary walls
        p.Spawn(p.Wall12, environment, 286f, 32f);
        p.Spawn(p.Wall12, environment, 286f, 20f);
        p.Spawn(p.Wall12, environment, 286f, 8f);
        p.Spawn(p.Wall12, environment, 298f, 32f);
        p.Spawn(p.Wall12, environment, 298f, 20f);
        p.Spawn(p.Wall12, environment, 298f, 8f);

        // Floor Tier 1 (Y = 30): CrackedBlocks 4x4
        p.Spawn(p.CrackedBlocks4x4, interactables, 292f, 30f);

        // Floor Tier 2 (Y = 22): 8-wide DashWall floor
        for (int x = 288; x <= 296; x++)
            p.Spawn(p.DashWall, interactables, x, 22);

        // Hazards between tiers: FireJet_Down & Spikes
        p.Spawn(p.FireJetDown, hazards, 292f, 20f);
        p.Spawn(p.SpikeRowDown, hazards, 288f, 18f);

        // Floor Tier 3 (Y = 12): CrackedBlocks 4x4
        p.Spawn(p.CrackedBlocks4x4, interactables, 292f, 12f);

        // Floor Tier 4 (Y = 4): Double-layer DashWall floor
        for (int x = 288; x <= 296; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 4);
            p.Spawn(p.DashWall, interactables, x, 5);
        }

        // Shaft bottom landing & BouncePad
        p.Spawn(p.Ground20, environment, 292f, -1f);
        p.Spawn(p.BouncePad, interactables, 292f, 0f);

        // Exit corridor with moving platform
        p.Spawn(p.Ground20, environment, 310f, -1f);
        GameObject atom1 = p.Spawn(p.Atom, collectibles, 312f, 3.5f);
        SetAtomId(atom1, 1);

        GameObject movPlat = p.Spawn(p.MovingPlatform, interactables, 330f, 2f);
        ConfigureMovingPlatform(movPlat, new Vector3(330f, 2f, 0f), new Vector3(365f, 2f, 0f), 3.5f);
        p.Spawn(p.SawSlow, hazards, 348f, 5f);

        // Landing & CHECKPOINT 2
        p.Spawn(p.Ground20, environment, 375f, 1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, checkpoints, 375f, 2f);
        cp2.name = "Checkpoint_Act2";

        // =============================================================
        // ACT 3: "Destruction Cavern" (X = 380 to 620)
        // Massive open cavern filled with dense DashWall quarry grid
        // =============================================================
        p.Spawn(p.ElementGateFire, interactables, 386f, 2f);
        p.Spawn(p.ElementGateEarth, interactables, 390f, 2f);

        p.Spawn(p.Ground20, environment, 395f, 1f);
        p.Spawn(p.Ground20, environment, 415f, 1f);
        p.Spawn(p.Ground20, environment, 435f, 1f);

        // Dense Lower Quarry (Over 60 DashWall blocks)
        for (int x = 405; x <= 445; x += 3)
        {
            for (int y = 2; y <= 6; y++)
            {
                p.Spawn(p.DashWall, interactables, x, y);
            }
        }
        // IceSurface underneath quarry
        p.Spawn(p.IceSurface, interactables, 415f, 2f);
        p.Spawn(p.IceSurface, interactables, 435f, 2f);

        // Hidden items inside quarry
        p.Spawn(p.BouncePadMini, interactables, 417f, 2f);
        p.Spawn(p.DashBoostFast, interactables, 432f, 2.5f);

        // Mid-cavern chasm with LavaLeaper & SpinArm
        p.Spawn(p.LavaLeaperTall, hazards, 455f, -4f);
        p.Spawn(p.SpinArm, hazards, 465f, 5f);
        p.Spawn(p.Ledge8, environment, 465f, 2f);
        p.Spawn(p.CannonUp, hazards, 475f, 0f);
        p.Spawn(p.SawFast, hazards, 485f, 6f);

        // Upper Quarry (Y = 10 to 18)
        p.Spawn(p.OneWayPlatform, interactables, 495f, 6f);
        p.Spawn(p.Ledge8, environment, 505f, 10f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 515f, 12f);

        // DashCharge in mid-air
        p.Spawn(p.DashCharge, interactables, 525f, 14f);

        // Water Gate guarding secret Atom 2
        p.Spawn(p.ElementGateWater, interactables, 535f, 12f);
        p.Spawn(p.Ledge8, environment, 542f, 12f);
        GameObject atom2 = p.Spawn(p.Atom, collectibles, 545f, 14f);
        SetAtomId(atom2, 2);

        // Slopes down to Act 3 exit
        p.Spawn(p.Slope45Large, environment, 560f, 10f);
        p.Spawn(p.Ground20, environment, 580f, 1f);
        p.Spawn(p.Ground20, environment, 600f, 1f);

        // CHECKPOINT 3
        GameObject cp3 = p.Spawn(p.Checkpoint, checkpoints, 605f, 2f);
        cp3.name = "Checkpoint_Act3";

        // =============================================================
        // ACT 4: "Dash-and-Slam Gauntlet & Finale" (X = 620 to 880)
        // Alternating horizontal dash smashes, vertical slams, & highway
        // =============================================================
        p.Spawn(p.ElementGateFire, interactables, 616f, 2f);
        p.Spawn(p.ElementGateEarth, interactables, 620f, 2f);

        // Dash smash 1
        p.Spawn(p.Ground20, environment, 625f, 1f);
        for (int y = 2; y <= 5; y++) p.Spawn(p.DashWall, interactables, 635, y);

        // Mega BouncePad into high slam
        p.Spawn(p.BouncePadMega, interactables, 645f, 2f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 655f, 12f);
        p.Spawn(p.DashBoostFast, interactables, 665f, 2.5f);

        // Barricade smash
        p.Spawn(p.Ground20, environment, 665f, 1f);
        for (int x = 673; x <= 676; x++)
            for (int y = 2; y <= 5; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Wall jump past fire jets
        p.Spawn(p.Wall12, environment, 685f, 10f);
        p.Spawn(p.Wall12, environment, 693f, 10f);
        p.Spawn(p.FireJetRight, hazards, 686f, 8f);
        p.Spawn(p.FireJetLeft, hazards, 692f, 12f);
        p.Spawn(p.OneWayPlatform, interactables, 689f, 16f);

        // Slam through DashWall ceiling floor onto CrumblingBlocks
        for (int x = 700; x <= 706; x++)
            p.Spawn(p.DashWall, interactables, x, 14);

        p.Spawn(p.CrumblingBlock, interactables, 702f, 6f);
        p.Spawn(p.CrumblingBlock, interactables, 706f, 6f);
        p.Spawn(p.CrumblingBlock, interactables, 710f, 6f);

        GameObject atom3 = p.Spawn(p.Atom, collectibles, 706f, 8.5f);
        SetAtomId(atom3, 3);

        // Transition to Final Speed Highway
        p.Spawn(p.Ledge8, environment, 720f, 2f);
        p.Spawn(p.ElementGateFire, interactables, 726f, 2f);

        // Final Speed Tunnel: Ground & Ceilings
        p.Spawn(p.Ground20, environment, 735f, 1f);
        p.Spawn(p.Ground20, environment, 755f, 1f);
        p.Spawn(p.Ground20, environment, 775f, 1f);
        p.Spawn(p.Ground20, environment, 795f, 1f);
        p.Spawn(p.Ground20, environment, 815f, 1f);

        p.Spawn(p.Ceiling20, environment, 735f, 8f);
        p.Spawn(p.Ceiling20, environment, 755f, 8f);
        p.Spawn(p.Ceiling20, environment, 775f, 8f);
        p.Spawn(p.Ceiling20, environment, 795f, 8f);
        p.Spawn(p.Ceiling20, environment, 815f, 8f);

        // Triple Mega-Barricades of DashWall
        // Mega Wall 1 (X = 750)
        for (int x = 749; x <= 752; x++)
            for (int y = 2; y <= 6; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        p.Spawn(p.DashBoostFast, interactables, 762f, 2.5f);

        // Mega Wall 2 (X = 775)
        for (int x = 774; x <= 777; x++)
            for (int y = 2; y <= 6; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        p.Spawn(p.DashBoostFast, interactables, 787f, 2.5f);

        // Mega Wall 3 (X = 800)
        for (int x = 799; x <= 802; x++)
            for (int y = 2; y <= 6; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Air Gate celebratory glide
        p.Spawn(p.ElementGateAir, interactables, 818f, 2f);

        // Finale Bridge & Mega BouncePad
        p.Spawn(p.Ground20, environment, 835f, 1f);
        p.Spawn(p.Ground20, environment, 855f, 1f);
        p.Spawn(p.BouncePadMega, interactables, 850f, 2f);

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
        return $"BUILD 02 COMPLETED! Saved to {TargetScenePath}. Validation errors: {errors}";
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
        public GameObject ElementGateBase = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/ElementGate_Base.prefab");
        public GameObject ElementGateFire = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate _Fire.prefab");
        public GameObject ElementGateEarth = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Earth.prefab");
        public GameObject ElementGateWater = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Water.prefab");
        public GameObject ElementGateAir = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Air.prefab");

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
                Debug.LogError("[DiegoLevelBuilder02] Null prefab passed to Spawn!");
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
