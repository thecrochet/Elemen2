#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder07
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO_07.unity";

    [MenuItem("Elemento/Build Diego Level 07 (Chevron Impulse Showcase & Excavation)")]
    public static string Build()
    {
        Debug.Log("[DiegoLevelBuilder07] Creating new scene: " + TargetScenePath);

        // 1. Create a brand-new empty scene and save it to W_TEST_DIEGO_07.unity
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
        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 65f, 4f);

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
        // ACT 1: "Impulse Ignition & Solid Wall Verification" (X = 0 to 180)
        // =============================================================

        // Section 1: Solid Wall Test (normal rolling does NOT break block)
        p.Spawn(p.Ledge8, environment, 20f, 0f);
        // Single DashWall at X = 32: player rolling down hits it as a solid wall!
        p.Spawn(p.DashWall, interactables, 32f, 0f);
        p.Spawn(p.OneWayPlatform, environment, 32f, 2.5f); // safe hop over

        // Section 2: First Golden Chevron Boost Runway (Auto-Smash Showcase)
        p.Spawn(p.DashBoostFast, interactables, 45f, 0.5f);
        // 2-thick, 3-high block wall at X = 55-56: boost impulse smashes right through!
        for (int x = 55; x <= 56; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Section 3: Excavation Drop 1 (Ground Pound Downward Shaft)
        p.Spawn(p.Slope45, environment, 66f, 0f);
        p.Spawn(p.OneWayPlatform, environment, 76f, 5f);

        // Camera Zone: Wide for Excavation Drop 1
        GameObject camDrop1 = p.Spawn(p.CameraZoneWide, flow, 90f, 2f);
        SetTriggerBoxSize(camDrop1, new Vector3(35f, 25f, 10f));

        // Sealed Excavation Drop: Layer 1 at Y = 4, Layer 2 at Y = 1
        for (int x = 86; x <= 90; x++)
            p.Spawn(p.DashWall, interactables, x, 4f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 88f, 1f);

        // Subterranean Chamber 1 (Y = -2 floor)
        p.Spawn(p.Ground20, environment, 90f, -3f);
        GameObject d1 = p.Spawn(p.Diamond, collectibles, 88f, -2f);
        SetDiamondId(d1, 1);
        GameObject a1 = p.Spawn(p.Atom, collectibles, 85f, -2f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, collectibles, 92f, -2f);
        SetAtomId(a2, 2);

        // Mega Bounce Pad launch out of lower chamber
        p.Spawn(p.BouncePadMega, interactables, 98f, -2f);

        // Section 4: Non-Linear Fork (Upper Chimney vs Lower Tunnel)
        // Upper Route: Twin-Wall Chimney 1 (X = 114 to 118)
        p.Spawn(p.Wall12, environment, 114f, 7f);
        p.Spawn(p.Wall12, environment, 118f, 7f);
        p.Spawn(p.BouncePadMini, interactables, 116f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 116f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        GameObject a3 = p.Spawn(p.Atom, collectibles, 116f, 4f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, collectibles, 116f, 8f);
        SetAtomId(a4, 4);

        // Upper Balcony
        p.Spawn(p.OneWayPlatform, environment, 124f, 11f);
        p.Spawn(p.OneWayPlatform, environment, 136f, 10f);

        // Lower Route: Tight smash tunnel with CameraZone_Tight
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 135f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(35f, 12f, 10f));

        p.Spawn(p.DashBoostFast, interactables, 125f, 0.5f);
        for (int x = 138; x <= 140; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Hazards on lower floor
        p.Spawn(p.SpikeRow6, hazards, 155f, 0f);
        p.Spawn(p.BouncePad, interactables, 165f, 0f);

        // CHECKPOINT 1
        GameObject cp1 = p.Spawn(p.Checkpoint, checkpoints, 175f, 2f);
        cp1.name = "Checkpoint_1";

        // =============================================================
        // ACT 2: "The Supersonic Chevron Quarry" (X = 180 to 360)
        // =============================================================

        // Runway 2: Speed Boost -> 4x4 Cracked Blocks Fortress
        p.Spawn(p.DashBoostFast, interactables, 192f, 0.5f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 206f, 2f);

        // Speed Tunnel (CameraZone_Tight) with chained DashWalls
        GameObject camTight2 = p.Spawn(p.CameraZoneTight, flow, 240f, 2f);
        SetTriggerBoxSize(camTight2, new Vector3(45f, 12f, 10f));

        p.Spawn(p.DashBoostFast, interactables, 225f, 0.5f);
        for (int x = 236; x <= 237; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        for (int x = 248; x <= 249; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Excavation Shaft 2 (X = 265 to 285)
        p.Spawn(p.Slope45, environment, 260f, 0f);
        p.Spawn(p.Ledge8, environment, 270f, 5f);

        // Double Ground Pound Deck
        for (int x = 274; x <= 278; x++)
            p.Spawn(p.DashWall, interactables, x, 5f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 276f, 1f);

        // Deep Chamber 2 (Y = -4 floor)
        p.Spawn(p.Ground20, environment, 276f, -5f);
        GameObject d2 = p.Spawn(p.Diamond, collectibles, 276f, -4f);
        SetDiamondId(d2, 2);
        GameObject a5 = p.Spawn(p.Atom, collectibles, 272f, -4f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, collectibles, 280f, -4f);
        SetAtomId(a6, 6);

        p.Spawn(p.BouncePadMega, interactables, 288f, -4f);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 2 (X = 308 to 312): Vertical Rocket Launch
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 308f, 7f);
        p.Spawn(p.Wall12, environment, 312f, 7f);

        // Upward Dash Lane in chimney
        GameObject laneUp1 = p.Spawn(p.DashLaneUp, interactables, 310f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(12f, 3f));

        // Breakable ceiling above rocket launch
        for (int x = 309; x <= 311; x++)
            p.Spawn(p.DashWall, interactables, x, 12f);

        // Sky terrace above chimney
        GameObject camWide1 = p.Spawn(p.CameraZoneWide, flow, 310f, 13f);
        SetTriggerBoxSize(camWide1, new Vector3(25f, 20f, 10f));

        p.Spawn(p.OneWayPlatform, environment, 318f, 13f);
        GameObject d3 = p.Spawn(p.Diamond, collectibles, 318f, 14.5f);
        SetDiamondId(d3, 3);
        GameObject a7 = p.Spawn(p.Atom, collectibles, 324f, 12f);
        SetAtomId(a7, 7);
        GameObject a8 = p.Spawn(p.Atom, collectibles, 330f, 10f);
        SetAtomId(a8, 8);

        // Moving platform bridging back to ground
        GameObject mp1 = p.Spawn(p.MovingPlatform, interactables, 338f, 6f);
        ConfigureMovingPlatform(mp1, new Vector3(334f, 6f, 0f), new Vector3(348f, 2f, 0f), 4f);

        // CHECKPOINT 2
        GameObject cp2 = p.Spawn(p.Checkpoint, checkpoints, 355f, 2f);
        cp2.name = "Checkpoint_2";

        // =============================================================
        // ACT 3: "Twin-Wall Heights & Deep Trench" (X = 360 to 540)
        // =============================================================

        // Sloped Ice approach into Speed Runway
        p.Spawn(p.IceSurface, environment, 370f, 0f);
        p.Spawn(p.IceSurface, environment, 385f, 0f);

        // Solid Wall Test 2: DashWall at X = 398 on ice!
        // Rolling fast down ice into it stops player; manual Dash or Boost required to break
        p.Spawn(p.DashWall, interactables, 398f, 0f);
        p.Spawn(p.OneWayPlatform, environment, 398f, 2.5f);

        // Chevron Boost Runway 3
        p.Spawn(p.DashBoostFast, interactables, 410f, 0.5f);
        // Dense 3x3 block barricade shattered by impulse
        for (int x = 422; x <= 424; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // -------------------------------------------------------------
        // TWIN-WALL CHIMNEY 3 (X = 442 to 446): Mini-bounce staircase
        // -------------------------------------------------------------
        p.Spawn(p.Wall12, environment, 442f, 7f);
        p.Spawn(p.Wall12, environment, 446f, 7f);

        GameObject camClimb3 = p.Spawn(p.CameraZoneClimb, flow, 444f, 8f);
        SetTriggerBoxSize(camClimb3, new Vector3(16f, 22f, 10f));

        p.Spawn(p.BouncePadMini, interactables, 443f, 0f);
        p.Spawn(p.BouncePadMini, interactables, 445f, 4f);
        p.Spawn(p.BouncePadMini, interactables, 443f, 8f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 444f, 2f + i * 2.5f);
            SetAtomId(a, 9 + i);
        }

        // High shelf
        p.Spawn(p.OneWayPlatform, environment, 454f, 12f);
        p.Spawn(p.Slope45Left, environment, 464f, 8f);

        // Deep Trench Excavation Shaft (X = 472 to 492)
        p.Spawn(p.CrackedBlocks4x4, interactables, 478f, 4f);
        p.Spawn(p.CrackedBlocks4x4, interactables, 478f, 1f);

        // Lava Trench Subterranean Room (Y = -3)
        p.Spawn(p.Ground20, environment, 480f, -4f);
        p.Spawn(p.LavaLeaper, hazards, 480f, -3f);

        GameObject d4 = p.Spawn(p.Diamond, collectibles, 484f, -2.5f);
        SetDiamondId(d4, 4);

        p.Spawn(p.BouncePadMega, interactables, 496f, -3f);

        // Stepped DashWall quarry pyramid
        for (int x = 508; x <= 516; x++)
        {
            int h = (x <= 512) ? (x - 508 + 1) : (516 - x + 1);
            for (int y = 0; y <= h; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        }

        // CHECKPOINT 3
        GameObject cp3 = p.Spawn(p.Checkpoint, checkpoints, 535f, 2f);
        cp3.name = "Checkpoint_3";

        // =============================================================
        // ACT 4: "The Grand Impulse Gauntlet & Sky Citadel" (X = 540 to 720)
        // =============================================================

        // Triple Chevron Speed Boosters -> 3x3 Block Barricades
        GameObject camTight3 = p.Spawn(p.CameraZoneTight, flow, 575f, 3f);
        SetTriggerBoxSize(camTight3, new Vector3(50f, 14f, 10f));

        int[] boostX = new int[] { 550, 572, 594 };
        int[] wallX = new int[] { 562, 584, 606 };
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

        GameObject camWideGrand = p.Spawn(p.CameraZoneWide, flow, 620f, 10f);
        SetTriggerBoxSize(camWideGrand, new Vector3(30f, 30f, 10f));

        GameObject grandLane = p.Spawn(p.DashLaneUp, interactables, 620f, 7f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(16f, 3f));

        // Breakable ceiling shattered by upward launch
        for (int x = 619; x <= 621; x++)
            p.Spawn(p.DashWall, interactables, x, 14f);

        // Grand Parabolic Canyon (CameraZone_Wide)
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 675f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(65f, 30f, 10f));

        p.Spawn(p.BouncePadMega, interactables, 648f, 0f);
        p.Spawn(p.BouncePadMega, interactables, 670f, 0f);
        p.Spawn(p.BouncePadMega, interactables, 692f, 0f);

        // Rainbow Atom arc
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
        return $"BUILD 07 COMPLETED! Saved to {TargetScenePath}. Validation errors: {errors}";
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
                Debug.LogError("[DiegoLevelBuilder07] Null prefab passed to Spawn!");
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
