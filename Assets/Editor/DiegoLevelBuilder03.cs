#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiegoLevelBuilder03
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/W_TEST_DIEGO_03.unity";

    [MenuItem("Elemento/Build Diego Level 03 (Easy High-Flow)")]
    public static string Build()
    {
        Debug.Log("[DiegoLevelBuilder03] Creating new scene: " + TargetScenePath);

        // 1. Create a brand-new empty scene and save it to W_TEST_DIEGO_03.unity
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

        // Tutorial Signs
        p.Spawn(p.TutorialDash, flow, 14f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 192f, 3f);
        p.Spawn(p.TutorialJump, flow, 646f, 3f);

        // Camera Zones
        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 212f, 8f);
        SetTriggerBoxSize(camWide, new Vector3(60f, 35f, 10f));

        GameObject camGlide = p.Spawn(p.CameraZoneWide, flow, 690f, 10f);
        SetTriggerBoxSize(camGlide, new Vector3(80f, 35f, 10f));

        // Global Frame & Bottom KillPit (covers full extent)
        GameObject levelFrame = p.Spawn(p.LevelFrame, environment, -15f, -1f);
        levelFrame.name = "LevelFrame";
        AdjustLevelFrame(levelFrame, -16f, 780f, 40f, -2f);

        GameObject globalKillPit = p.Spawn(p.KillPit, hazards, 380f, -16f);
        globalKillPit.name = "KillPit_GlobalBottom";
        SetBoxSize(globalKillPit, new Vector3(950f, 6f, 10f));

        // =============================================================
        // ACT 1: "The Dash Highway" (X = 0 to 180)
        // High speed, forgiving continuous runway packed with breakable walls
        // =============================================================
        p.Spawn(p.ElementGateFire, interactables, 8f, 0f);

        // Continuous solid floor (Zero pits in Act 1)
        for (int fx = 0; fx <= 160; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, -1f);
        }
        for (int cx = 20; cx <= 100; cx += 20)
        {
            p.Spawn(p.Ceiling20, environment, cx, 8f);
        }

        // Wall Cluster 1 (X = 24..25, Y = 0..2)
        for (int x = 24; x <= 25; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        p.Spawn(p.DashBoost, interactables, 33f, 0.5f);

        // Wall Cluster 2 (X = 42..44, Y = 0..2) + Diamond 1
        for (int x = 42; x <= 44; x++)
            for (int y = 0; y <= 2; y++)
            {
                if (x == 43 && y == 1)
                {
                    GameObject d1 = p.Spawn(p.Diamond, collectibles, x, y);
                    SetDiamondId(d1, 1);
                }
                else
                {
                    p.Spawn(p.DashWall, interactables, x, y);
                }
            }

        p.Spawn(p.DashCharge, interactables, 52f, 0.5f);

        // Wall Cluster 3 (X = 62..65, Y = 0..3) Mega Gate
        for (int x = 62; x <= 65; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Elevated Platforms with generous landing zones
        p.Spawn(p.OneWayLarge, environment, 75f, 3f);
        p.Spawn(p.OneWayLarge, environment, 88f, 4f);

        // Mid-air Dash blocks (X = 92..94, Y = 4..5)
        for (int x = 92; x <= 94; x++)
            for (int y = 4; y <= 5; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Atoms in an arc
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 78f + i * 4f, 4.5f + (i == 1 || i == 2 ? 1f : 0f));
            SetAtomId(a, 1 + i);
        }

        p.Spawn(p.DashHold, interactables, 110f, 1.5f);

        // Sequential rhythmic hurdles (chain dashes)
        int[] hurdleX = new int[] { 122, 128, 134, 140 };
        for (int h = 0; h < hurdleX.Length; h++)
        {
            p.Spawn(p.DashWall, interactables, hurdleX[h], 0);
            p.Spawn(p.DashWall, interactables, hurdleX[h], 1);
        }

        // Bounce pad into elevated plateau
        p.Spawn(p.BouncePad, interactables, 152f, 0f);
        p.Spawn(p.Ledge8, environment, 162f, 2f);
        p.Spawn(p.Ground20, environment, 175f, 2f);

        // CHECKPOINT 1
        GameObject cp1 = p.Spawn(p.Checkpoint, checkpoints, 175f, 3f);
        cp1.name = "Checkpoint_1";

        // =============================================================
        // ACT 2: "Excavation Pit & Bounce Safety" (X = 180 to 360)
        // Satisfying downward Earth slams with mega bounce pads catching falls
        // =============================================================
        p.Spawn(p.ElementGateEarth, interactables, 185f, 3f);

        // High starting ledge
        p.Spawn(p.Ledge8, environment, 198f, 13f);

        // Shaft 1 Chamber Walls
        for (int y = 0; y <= 14; y++)
        {
            p.Spawn(p.BlockCyan, environment, 204f, y);
            p.Spawn(p.BlockCyan, environment, 220f, y);
        }

        // Shaft 1: 3 layers of breakables
        p.Spawn(p.CrackedBlocks4x4, interactables, 212f, 9f);
        for (int x = 206; x <= 218; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 5f);
            p.Spawn(p.DashWall, interactables, x, 2f);
        }

        // Safety floor & Mega Bounce Pad at the bottom
        p.Spawn(p.Ground20, environment, 212f, -2f);
        p.Spawn(p.BouncePadMega, interactables, 212f, -1f);

        // Diamond 2 at the height of the bounce
        GameObject d2 = p.Spawn(p.Diamond, collectibles, 212f, 15f);
        SetDiamondId(d2, 2);

        // Landing shelf after the bounce
        p.Spawn(p.OneWayLarge, environment, 228f, 12f);
        p.Spawn(p.Ledge8, environment, 242f, 11f);

        // Shaft 2 (X = 245..275)
        for (int x = 246; x <= 256; x++)
        {
            p.Spawn(p.DashWall, interactables, x, 8f);
            p.Spawn(p.DashWall, interactables, x, 4f);
        }

        // Safety floor and bounce pad
        p.Spawn(p.Ground20, environment, 260f, -1f);
        p.Spawn(p.BouncePad, interactables, 250f, 0f);

        // Moving platform carrying player across safely
        GameObject mp1 = p.Spawn(p.MovingPlatform, interactables, 268f, 3f);
        ConfigureMovingPlatform(mp1, new Vector3(268f, 3f, 0f), new Vector3(295f, 3f, 0f), 3.5f);

        // Atoms along the moving platform path
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 275f + i * 6f, 4.5f);
            SetAtomId(a, 5 + i);
        }

        // Non-lethal high decorative saw
        p.Spawn(p.SawSlow, hazards, 282f, 8.5f);

        // Stepped climb
        p.Spawn(p.OneWayLarge, environment, 305f, 4f);
        p.Spawn(p.OneWayLarge, environment, 320f, 6f);
        p.Spawn(p.OneWayLarge, environment, 335f, 8f);

        // Breakables on climb
        for (int y = 4; y <= 6; y++)
        {
            p.Spawn(p.DashWall, interactables, 310f, y);
            p.Spawn(p.DashWall, interactables, 325f, y);
        }

        // Safety floor underneath climb
        p.Spawn(p.Ground20, environment, 320f, -1f);
        p.Spawn(p.Ground20, environment, 350f, 2f);

        // CHECKPOINT 2
        GameObject cp2 = p.Spawn(p.Checkpoint, checkpoints, 355f, 3f);
        cp2.name = "Checkpoint_2";

        // =============================================================
        // ACT 3: "Water Oasis & High-Flow Quarry" (X = 360 to 540)
        // Ice sliding speed, fluid wall jumps, and wide quarry shattering
        // =============================================================
        p.Spawn(p.ElementGateWater, interactables, 365f, 3f);

        // Continuous safety floor across the bottom
        for (int fx = 370; fx <= 530; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, -1f);
        }

        // Ice surface on flat ground
        p.Spawn(p.IceSurface, environment, 380f, 0f);
        p.Spawn(p.IceSurface, environment, 440f, 0f);
        p.Spawn(p.IceSurface, environment, 500f, 0f);

        // Wall jumping towers (spaced generously for easy wall bounce)
        for (int y = 0; y <= 8; y++)
        {
            p.Spawn(p.BlockCyan, environment, 392f, y);
            p.Spawn(p.BlockCyan, environment, 398f, y);
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 395f, 2f + i * 2.5f);
            SetAtomId(a, 8 + i);
        }

        // Upper Fire gate to alternate dash & slide
        p.Spawn(p.ElementGateFire, interactables, 420f, 4f);
        p.Spawn(p.OneWayLarge, environment, 420f, 3f);

        // Blockade A: Ice dash smash (X = 434..436, Y = 0..2)
        for (int x = 434; x <= 436; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, interactables, x, y);

        // Blockade B: Cracked blocks with Diamond 3 inside
        p.Spawn(p.CrackedBlocks4x4, interactables, 460f, 2f);
        GameObject d3 = p.Spawn(p.Diamond, collectibles, 460f, 2f);
        SetDiamondId(d3, 3);

        // Atoms
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, collectibles, 470f + i * 3f, 1.5f);
            SetAtomId(a, 11 + i);
        }

        // Blockade C: Stepped Quarry Wall (X = 482..488)
        for (int x = 482; x <= 488; x++)
        {
            int h = (x <= 485) ? (x - 482 + 1) : (488 - x + 1);
            for (int y = 0; y <= h; y++)
                p.Spawn(p.DashWall, interactables, x, y);
        }

        p.Spawn(p.BouncePad, interactables, 515f, 0f);
        p.Spawn(p.Ground20, environment, 530f, 3f);

        // CHECKPOINT 3
        GameObject cp3 = p.Spawn(p.Checkpoint, checkpoints, 535f, 4f);
        cp3.name = "Checkpoint_3";

        // =============================================================
        // ACT 4: "Joyride Gauntlet & Grand Finale" (X = 540 to 760)
        // Chained speed boosters, wall vaporizing, and open canyon glide
        // =============================================================
        p.Spawn(p.ElementGateFire, interactables, 542f, 4f);

        // High-speed booster runway floor
        for (int fx = 550; fx <= 630; fx += 20)
        {
            p.Spawn(p.Ground20, environment, fx, 2f);
        }

        // Chained Dash Boosters & Walls (Pure joyride rhythm)
        int[] boostX = new int[] { 552, 572, 592, 612 };
        int[] wallX = new int[] { 562, 582, 602, 622 };
        for (int i = 0; i < boostX.Length; i++)
        {
            p.Spawn(p.DashBoostFast, interactables, boostX[i], 3.5f);
            for (int x = wallX[i]; x <= wallX[i] + 1; x++)
                for (int y = 3; y <= 5; y++)
                    p.Spawn(p.DashWall, interactables, x, y);
        }

        // Air Element Gate for the grand canyon glide
        p.Spawn(p.ElementGateAir, interactables, 638f, 3f);

        // Open canyon with bottom mega bounce pads (Safety net)
        p.Spawn(p.Ground20, environment, 660f, -2f);
        p.Spawn(p.Ground20, environment, 680f, -2f);
        p.Spawn(p.Ground20, environment, 700f, -2f);

        p.Spawn(p.BouncePadMega, interactables, 665f, -1f);
        p.Spawn(p.BouncePadMega, interactables, 685f, -1f);
        p.Spawn(p.BouncePadMega, interactables, 705f, -1f);

        // Atoms arc across the canyon
        for (int i = 0; i < 5; i++)
        {
            float ax = 660f + i * 12f;
            float ay = 8f - Mathf.Abs(i - 2) * 1.5f;
            GameObject a = p.Spawn(p.Atom, collectibles, ax, ay);
            SetAtomId(a, 15 + i);
        }

        // Diamond 4 in mid-air glide
        GameObject d4 = p.Spawn(p.Diamond, collectibles, 685f, 9f);
        SetDiamondId(d4, 4);

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
        return $"BUILD 03 COMPLETED! Saved to {TargetScenePath}. Validation errors: {errors}";
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
        public GameObject Wall12 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Wall_1x12.prefab");
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
        public GameObject ElementGateFire = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate _Fire.prefab");
        public GameObject ElementGateEarth = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Earth.prefab");
        public GameObject ElementGateWater = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Water.prefab");
        public GameObject ElementGateAir = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/ElementGate_Air.prefab");

        public GameObject SawSlow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Saw_Slow.prefab");

        public GameObject Atom = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Atom.prefab");
        public GameObject Diamond = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Diamond.prefab");
        public GameObject Checkpoint = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/CheckPoint.prefab");
        public GameObject StartPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/StartPortal.prefab");
        public GameObject EndPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/End_Portal.prefab");
        public GameObject TutorialJump = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Jump.prefab");
        public GameObject TutorialDash = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Dash.prefab");
        public GameObject TutorialGroundPound = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_GroundPound.prefab");
        public GameObject CameraZoneWide = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Wide.prefab");

        public GameObject Spawn(GameObject prefab, Transform parent, float x, float y)
        {
            if (prefab == null)
            {
                Debug.LogError("[DiegoLevelBuilder03] Null prefab passed to Spawn!");
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
