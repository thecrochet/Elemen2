#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FinalPurpleLevelBuilder
{
    private const string BaseDir = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/";

    [MenuItem("Elemento/Build All 35 FINAL_PURPLE Levels")]
    public static string BuildAll35()
    {
        string r30 = BuildAll30();
        string r31 = BuildLevel31();
        string r32 = BuildLevel32();
        string r33 = BuildLevel33();
        string r34 = BuildLevel34();
        string r35 = BuildLevel35();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT ALL 35 LEVELS!\n{r30}\n{r31}\n{r32}\n{r33}\n{r34}\n{r35}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 31 to 35")]
    public static string BuildLevels31To35()
    {
        string r31 = BuildLevel31();
        string r32 = BuildLevel32();
        string r33 = BuildLevel33();
        string r34 = BuildLevel34();
        string r35 = BuildLevel35();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT LEVELS 31 TO 35!\n{r31}\n{r32}\n{r33}\n{r34}\n{r35}\n{syncResult}";
    }

    [MenuItem("Elemento/Build All 30 FINAL_PURPLE Levels")]
    public static string BuildAll30()
    {
        string r1 = BuildLevel01();
        string r2 = BuildLevel02();
        string r3 = BuildLevel03();
        string r4 = BuildLevel04();
        string r5 = BuildLevel05();
        string r6 = BuildLevel06();
        string r7 = BuildLevel07();
        string r8 = BuildLevel08();
        string r9 = BuildLevel09();
        string r10 = BuildLevel10();
        string r11 = BuildLevel11();
        string r12 = BuildLevel12();
        string r13 = BuildLevel13();
        string r14 = BuildLevel14();
        string r15 = BuildLevel15();
        string r16 = BuildLevel16();
        string r17 = BuildLevel17();
        string r18 = BuildLevel18();
        string r19 = BuildLevel19();
        string r20 = BuildLevel20();
        string r21 = BuildLevel21();
        string r22 = BuildLevel22();
        string r23 = BuildLevel23();
        string r24 = BuildLevel24();
        string r25 = BuildLevel25();
        string r26 = BuildLevel26();
        string r27 = BuildLevel27();
        string r28 = BuildLevel28();
        string r29 = BuildLevel29();
        string r30 = BuildLevel30();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT ALL 30 LEVELS!\n{r1}\n{r2}\n{r3}\n{r4}\n{r5}\n{r6}\n{r7}\n{r8}\n{r9}\n{r10}\n{r11}\n{r12}\n{r13}\n{r14}\n{r15}\n{r16}\n{r17}\n{r18}\n{r19}\n{r20}\n{r21}\n{r22}\n{r23}\n{r24}\n{r25}\n{r26}\n{r27}\n{r28}\n{r29}\n{r30}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 26 to 30")]
    public static string BuildLevels26To30()
    {
        string r26 = BuildLevel26();
        string r27 = BuildLevel27();
        string r28 = BuildLevel28();
        string r29 = BuildLevel29();
        string r30 = BuildLevel30();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT LEVELS 26 TO 30!\n{r26}\n{r27}\n{r28}\n{r29}\n{r30}\n{syncResult}";
    }

    [MenuItem("Elemento/Build All 25 FINAL_PURPLE Levels")]
    public static string BuildAll25()
    {
        string r1 = BuildLevel01();
        string r2 = BuildLevel02();
        string r3 = BuildLevel03();
        string r4 = BuildLevel04();
        string r5 = BuildLevel05();
        string r6 = BuildLevel06();
        string r7 = BuildLevel07();
        string r8 = BuildLevel08();
        string r9 = BuildLevel09();
        string r10 = BuildLevel10();
        string r11 = BuildLevel11();
        string r12 = BuildLevel12();
        string r13 = BuildLevel13();
        string r14 = BuildLevel14();
        string r15 = BuildLevel15();
        string r16 = BuildLevel16();
        string r17 = BuildLevel17();
        string r18 = BuildLevel18();
        string r19 = BuildLevel19();
        string r20 = BuildLevel20();
        string r21 = BuildLevel21();
        string r22 = BuildLevel22();
        string r23 = BuildLevel23();
        string r24 = BuildLevel24();
        string r25 = BuildLevel25();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT ALL 25 LEVELS!\n{r1}\n{r2}\n{r3}\n{r4}\n{r5}\n{r6}\n{r7}\n{r8}\n{r9}\n{r10}\n{r11}\n{r12}\n{r13}\n{r14}\n{r15}\n{r16}\n{r17}\n{r18}\n{r19}\n{r20}\n{r21}\n{r22}\n{r23}\n{r24}\n{r25}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 21 to 25")]
    public static string BuildLevels21To25()
    {
        string r21 = BuildLevel21();
        string r22 = BuildLevel22();
        string r23 = BuildLevel23();
        string r24 = BuildLevel24();
        string r25 = BuildLevel25();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT LEVELS 21 TO 25!\n{r21}\n{r22}\n{r23}\n{r24}\n{r25}\n{syncResult}";
    }

    [MenuItem("Elemento/Build All 20 FINAL_PURPLE Levels")]
    public static string BuildAll20()
    {
        string r1 = BuildLevel01();
        string r2 = BuildLevel02();
        string r3 = BuildLevel03();
        string r4 = BuildLevel04();
        string r5 = BuildLevel05();
        string r6 = BuildLevel06();
        string r7 = BuildLevel07();
        string r8 = BuildLevel08();
        string r9 = BuildLevel09();
        string r10 = BuildLevel10();
        string r11 = BuildLevel11();
        string r12 = BuildLevel12();
        string r13 = BuildLevel13();
        string r14 = BuildLevel14();
        string r15 = BuildLevel15();
        string r16 = BuildLevel16();
        string r17 = BuildLevel17();
        string r18 = BuildLevel18();
        string r19 = BuildLevel19();
        string r20 = BuildLevel20();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT ALL 20 LEVELS!\n{r1}\n{r2}\n{r3}\n{r4}\n{r5}\n{r6}\n{r7}\n{r8}\n{r9}\n{r10}\n{r11}\n{r12}\n{r13}\n{r14}\n{r15}\n{r16}\n{r17}\n{r18}\n{r19}\n{r20}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 16 to 20")]
    public static string BuildLevels16To20()
    {
        string r16 = BuildLevel16();
        string r17 = BuildLevel17();
        string r18 = BuildLevel18();
        string r19 = BuildLevel19();
        string r20 = BuildLevel20();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT LEVELS 16 TO 20!\n{r16}\n{r17}\n{r18}\n{r19}\n{r20}\n{syncResult}";
    }

    [MenuItem("Elemento/Build All 15 FINAL_PURPLE Levels")]
    public static string BuildAll15()
    {
        string r1 = BuildLevel01();
        string r2 = BuildLevel02();
        string r3 = BuildLevel03();
        string r4 = BuildLevel04();
        string r5 = BuildLevel05();
        string r6 = BuildLevel06();
        string r7 = BuildLevel07();
        string r8 = BuildLevel08();
        string r9 = BuildLevel09();
        string r10 = BuildLevel10();
        string r11 = BuildLevel11();
        string r12 = BuildLevel12();
        string r13 = BuildLevel13();
        string r14 = BuildLevel14();
        string r15 = BuildLevel15();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT ALL 15 LEVELS!\n{r1}\n{r2}\n{r3}\n{r4}\n{r5}\n{r6}\n{r7}\n{r8}\n{r9}\n{r10}\n{r11}\n{r12}\n{r13}\n{r14}\n{r15}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 11 to 15")]
    public static string BuildLevels11To15()
    {
        string r11 = BuildLevel11();
        string r12 = BuildLevel12();
        string r13 = BuildLevel13();
        string r14 = BuildLevel14();
        string r15 = BuildLevel15();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT LEVELS 11 TO 15!\n{r11}\n{r12}\n{r13}\n{r14}\n{r15}\n{syncResult}";
    }

    [MenuItem("Elemento/Build All 10 FINAL_PURPLE Levels")]
    public static string BuildAll10()
    {
        string r1 = BuildLevel01();
        string r2 = BuildLevel02();
        string r3 = BuildLevel03();
        string r4 = BuildLevel04();
        string r5 = BuildLevel05();
        string r6 = BuildLevel06();
        string r7 = BuildLevel07();
        string r8 = BuildLevel08();
        string r9 = BuildLevel09();
        string r10 = BuildLevel10();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT ALL 10 LEVELS!\n{r1}\n{r2}\n{r3}\n{r4}\n{r5}\n{r6}\n{r7}\n{r8}\n{r9}\n{r10}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 06 to 10")]
    public static string BuildLevels06To10()
    {
        string r6 = BuildLevel06();
        string r7 = BuildLevel07();
        string r8 = BuildLevel08();
        string r9 = BuildLevel09();
        string r10 = BuildLevel10();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT LEVELS 06 TO 10!\n{r6}\n{r7}\n{r8}\n{r9}\n{r10}\n{syncResult}";
    }

    [MenuItem("Elemento/Build FINAL_PURPLE/Build Levels 01 to 05")]
    public static string BuildAll()
    {
        string r1 = BuildLevel01();
        string r2 = BuildLevel02();
        string r3 = BuildLevel03();
        string r4 = BuildLevel04();
        string r5 = BuildLevel05();

        string syncResult = ElementoFinalPurpleSync.SyncLevels();

        return $"BUILT FIRST 5 LEVELS!\n{r1}\n{r2}\n{r3}\n{r4}\n{r5}\n{syncResult}";
    }

    // =========================================================================
    // LEVEL 01: Onboarding & Momentum Basics (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 01 (Onboarding & Momentum Basics)")]
    public static string BuildLevel01()
    {
        string scenePath = BaseDir + "Level_01.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        // Flow: Portals & Signs
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 28f, 0f);

        // Frame & Bottom KillPit
        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 255f, 35f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 120f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(450f, 6f, 10f));

        // Continuous floor
        for (int x = 0; x <= 240; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Rolling & Dash Basics (X = 0 to 120)
        p.Spawn(p.Ledge8, env, 20f, 0f);

        // Solid Wall Test (normal roll stops; jump over or dash to break)
        p.Spawn(p.DashWall, inter, 35f, 0f);
        p.Spawn(p.OneWayPlatform, env, 35f, 2.5f);

        // First Chevron Boost Runway
        p.Spawn(p.DashBoostFast, inter, 50f, 0.5f);
        for (int y = 0; y <= 1; y++) p.Spawn(p.DashWall, inter, 62f, y);

        // Speed Tunnel with CameraZone_Tight
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 85f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(35f, 12f, 10f));

        p.Spawn(p.DashBoostFast, inter, 75f, 0.5f);
        for (int x = 86; x <= 87; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Checkpoint 1
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 115f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Twin-Wall Chimney & Victory Sprint (X = 120 to 240)
        p.Spawn(p.Wall12, env, 134f, 7f);
        p.Spawn(p.Wall12, env, 138f, 7f);
        p.Spawn(p.BouncePadMini, inter, 136f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 136f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 136f, 3f + i * 2.5f);
            SetAtomId(a, 1 + i);
        }

        // Upper balcony with Diamond 1
        p.Spawn(p.OneWayPlatform, env, 144f, 10f);
        GameObject d1 = p.Spawn(p.Diamond, col, 144f, 11.5f);
        SetDiamondId(d1, 1);

        // Ice slide
        p.Spawn(p.IceSurface, env, 165f, 0f);
        p.Spawn(p.IceSurface, env, 180f, 0f);

        // Final booster into DashWall
        p.Spawn(p.DashBoostFast, inter, 192f, 0.5f);
        for (int x = 205; x <= 206; x++)
            for (int y = 0; y <= 1; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Victory bridge
        p.Spawn(p.Ground20, env, 230f, 1f);
        GameObject d2 = p.Spawn(p.Diamond, col, 226f, 3f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 3f, 2f);
            SetAtomId(a, 4 + i);
        }

        FinalizeScene(scene);
        return $"Level_01 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 02: Vertical Excavation (X = 0 to 300)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 02 (Vertical Excavation)")]
    public static string BuildLevel02()
    {
        string scenePath = BaseDir + "Level_02.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 295f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 25f, 2f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 315f, 35f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 150f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(550f, 6f, 10f));

        for (int x = 0; x <= 300; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The First Drop (X = 0 to 100)
        p.Spawn(p.Slope45, env, 20f, 0f);
        p.Spawn(p.Ledge8, env, 32f, 5f);

        GameObject camDrop1 = p.Spawn(p.CameraZoneWide, flow, 45f, 2f);
        SetTriggerBoxSize(camDrop1, new Vector3(30f, 22f, 10f));

        // Sealed Excavation Shaft 1
        for (int x = 40; x <= 44; x++) p.Spawn(p.DashWall, inter, x, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 42f, 1f);

        // Chamber 1 at Y = -2
        p.Spawn(p.Ground20, env, 45f, -3f);
        GameObject d1 = p.Spawn(p.Diamond, col, 42f, -2f);
        SetDiamondId(d1, 1);
        GameObject a1 = p.Spawn(p.Atom, col, 39f, -2f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, col, 46f, -2f);
        SetAtomId(a2, 2);

        p.Spawn(p.BouncePadMega, inter, 55f, -2f);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 90f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Deep Cavern Pit (X = 100 to 200)
        p.Spawn(p.Slope45, env, 110f, 0f);
        p.Spawn(p.Ledge8, env, 122f, 5f);

        GameObject camDrop2 = p.Spawn(p.CameraZoneWide, flow, 140f, 0f);
        SetTriggerBoxSize(camDrop2, new Vector3(35f, 25f, 10f));

        for (int x = 132; x <= 136; x++) p.Spawn(p.DashWall, inter, x, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 134f, 1f);

        // Deep Chamber 2 at Y = -4
        p.Spawn(p.Ground20, env, 140f, -5f);
        p.Spawn(p.SpinArm, haz, 142f, -1.5f);
        GameObject d2 = p.Spawn(p.Diamond, col, 134f, -4f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 130f + i * 4f, -4f);
            SetAtomId(a, 3 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 160f, -4f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 190f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Chimney Ascent & Exit Trench (X = 200 to 300)
        p.Spawn(p.Wall12, env, 218f, 7f);
        p.Spawn(p.Wall12, env, 222f, 7f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 220f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(12f, 3f));

        for (int x = 219; x <= 221; x++) p.Spawn(p.DashWall, inter, x, 12f);

        p.Spawn(p.OneWayPlatform, env, 228f, 13f);
        GameObject d3 = p.Spawn(p.Diamond, col, 228f, 14.5f);
        SetDiamondId(d3, 3);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 234f + i * 4f, 11f);
            SetAtomId(a, 6 + i);
        }

        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 250f, 6f);
        ConfigureMovingPlatform(mp1, new Vector3(246f, 6f, 0f), new Vector3(260f, 2f, 0f), 4f);

        // Stepped quarry
        for (int x = 270; x <= 278; x++)
        {
            int h = (x <= 274) ? (x - 270 + 1) : (278 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        p.Spawn(p.Ground20, env, 290f, 1f);
        FinalizeScene(scene);
        return $"Level_02 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 03: Kinetic Chaining (X = 0 to 360)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 03 (Kinetic Chaining)")]
    public static string BuildLevel03()
    {
        string scenePath = BaseDir + "Level_03.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 355f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 20f, 0f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 380f, 35f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 180f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(650f, 6f, 10f));

        for (int x = 0; x <= 360; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Chained Boosters & Airborne Blocks (X = 0 to 120)
        p.Spawn(p.DashBoostFast, inter, 25f, 0.5f);
        for (int y = 1; y <= 2; y++) p.Spawn(p.DashWall, inter, 38f, y);

        p.Spawn(p.DashBoostFast, inter, 55f, 0.5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 68f, 2f);

        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 85f, 4f);
        ConfigureMovingPlatform(mp1, new Vector3(80f, 4f, 0f), new Vector3(100f, 4f, 0f), 3.5f);
        p.Spawn(p.SawSlow, haz, 90f, 7f);

        GameObject d1 = p.Spawn(p.Diamond, col, 90f, 5.5f);
        SetDiamondId(d1, 1);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 115f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Rocket Chimney & High-Speed Highway (X = 120 to 240)
        p.Spawn(p.Wall12, env, 138f, 7f);
        p.Spawn(p.Wall12, env, 142f, 7f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 140f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 139; x <= 141; x++) p.Spawn(p.DashWall, inter, x, 12f);

        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 180f, 10f);
        SetTriggerBoxSize(camTight1, new Vector3(50f, 12f, 10f));

        p.Spawn(p.OneWayPlatform, env, 150f, 10f);
        p.Spawn(p.OneWayPlatform, env, 165f, 10f);
        p.Spawn(p.DashBoostFast, inter, 170f, 10.5f);
        for (int y = 10; y <= 12; y++) p.Spawn(p.DashWall, inter, 182f, y);

        p.Spawn(p.OneWayPlatform, env, 188f, 10f);
        p.Spawn(p.DashBoostFast, inter, 192f, 10.5f);
        for (int y = 10; y <= 12; y++) p.Spawn(p.DashWall, inter, 204f, y);

        GameObject d2 = p.Spawn(p.Diamond, col, 175f, 12f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 160f + i * 12f, 11.5f);
            SetAtomId(a, 1 + i);
        }

        p.Spawn(p.Slope45Left, env, 218f, 6f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 230f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Excavation Trench & Canyon Mega-Bounce (X = 240 to 360)
        p.Spawn(p.Slope45, env, 245f, 0f);
        p.Spawn(p.Ledge8, env, 255f, 4f);

        p.Spawn(p.CrackedBlocks4x4, inter, 264f, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 264f, 1f);

        p.Spawn(p.Ground20, env, 265f, -3f);
        GameObject d3 = p.Spawn(p.Diamond, col, 264f, -2f);
        SetDiamondId(d3, 3);
        GameObject a5 = p.Spawn(p.Atom, col, 260f, -2f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, col, 268f, -2f);
        SetAtomId(a6, 6);

        p.Spawn(p.BouncePadMega, inter, 276f, -2f);

        // Triple Mega Bounce Canyon
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 315f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(65f, 25f, 10f));

        p.Spawn(p.BouncePadMega, inter, 295f, 0f);
        p.Spawn(p.BouncePadMega, inter, 315f, 0f);
        p.Spawn(p.BouncePadMega, inter, 335f, 0f);

        for (int i = 0; i < 4; i++)
        {
            float ax = 300f + i * 10f;
            float ay = 6f + Mathf.Sin(i * 0.8f) * 2f;
            GameObject a = p.Spawn(p.Atom, col, ax, ay);
            SetAtomId(a, 7 + i);
        }

        p.Spawn(p.Ground20, env, 350f, 1f);
        FinalizeScene(scene);
        return $"Level_03 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 04: Multi-Tier Cavern (X = 0 to 440)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 04 (Multi-Tier Cavern)")]
    public static string BuildLevel04()
    {
        string scenePath = BaseDir + "Level_04.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 430f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 460f, 35f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 220f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(750f, 6f, 10f));

        for (int x = 0; x <= 440; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Upper Highway vs Lower Trench (X = 0 to 150)
        // Upper Route: Twin-Wall Chimney 1
        p.Spawn(p.Wall12, env, 28f, 7f);
        p.Spawn(p.Wall12, env, 32f, 7f);
        p.Spawn(p.BouncePadMini, inter, 30f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 30f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        p.Spawn(p.OneWayPlatform, env, 38f, 10f);
        p.Spawn(p.OneWayPlatform, env, 50f, 10f);
        p.Spawn(p.Cannon, haz, 65f, 12f);
        GameObject d1 = p.Spawn(p.Diamond, col, 45f, 11.5f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 30f, 3f + i * 2.5f);
            SetAtomId(a, 1 + i);
        }

        // Lower Route: Booster smash
        p.Spawn(p.DashBoostFast, inter, 70f, 0.5f);
        for (int x = 84; x <= 85; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 145f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Hazard Gauntlet & Deep Vault (X = 150 to 300)
        p.Spawn(p.FireJet, haz, 165f, 0f);
        p.Spawn(p.StompBlock, haz, 180f, 5f);

        // Deep excavation shaft
        p.Spawn(p.Slope45, env, 185f, 0f);
        p.Spawn(p.Ledge8, env, 195f, 4f);

        GameObject camWide1 = p.Spawn(p.CameraZoneWide, flow, 210f, 0f);
        SetTriggerBoxSize(camWide1, new Vector3(40f, 25f, 10f));

        p.Spawn(p.CrackedBlocks4x4, inter, 204f, 4f);
        for (int x = 202; x <= 206; x++) p.Spawn(p.DashWall, inter, x, 1f);

        p.Spawn(p.Ground20, env, 210f, -5f);
        p.Spawn(p.LavaLeaper, haz, 212f, -4f);
        GameObject d2 = p.Spawn(p.Diamond, col, 204f, -4f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 200f + i * 4f, -4f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 226f, -4f);

        // Speed tunnel
        p.Spawn(p.DashBoostFast, inter, 245f, 0.5f);
        for (int x = 258; x <= 260; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 295f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Grand Ascent & Fortress Exit (X = 300 to 440)
        p.Spawn(p.Wall12, env, 318f, 7f);
        p.Spawn(p.Wall12, env, 322f, 7f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 320f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 319; x <= 321; x++) p.Spawn(p.DashWall, inter, x, 12f);

        p.Spawn(p.OneWayPlatform, env, 330f, 12f);
        GameObject d3 = p.Spawn(p.Diamond, col, 330f, 13.5f);
        SetDiamondId(d3, 3);

        // Stepped quarry
        for (int x = 350; x <= 360; x++)
        {
            int h = (x <= 355) ? (x - 350 + 1) : (360 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        GameObject mp2 = p.Spawn(p.MovingPlatform, inter, 380f, 3f);
        ConfigureMovingPlatform(mp2, new Vector3(375f, 3f, 0f), new Vector3(395f, 3f, 0f), 4f);

        GameObject d4 = p.Spawn(p.Diamond, col, 385f, 5f);
        SetDiamondId(d4, 4);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 400f + i * 6f, 2f);
            SetAtomId(a, 7 + i);
        }

        p.Spawn(p.Ground20, env, 425f, 1f);
        FinalizeScene(scene);
        return $"Level_04 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 05: World 1 Purple Finale (X = 0 to 600)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 05 (World 1 Purple Finale)")]
    public static string BuildLevel05()
    {
        string scenePath = BaseDir + "Level_05.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 595f, 4f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 22f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 160f, 4f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 620f, 40f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 300f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(850f, 6f, 10f));

        for (int x = 0; x <= 600; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The Supersonic Runway (X = 0 to 150)
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 65f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(70f, 12f, 10f));

        int[] b1X = new int[] { 30, 60, 90 };
        int[] w1X = new int[] { 42, 72, 102 };
        for (int i = 0; i < b1X.Length; i++)
        {
            p.Spawn(p.DashBoostFast, inter, b1X[i], 0.5f);
            for (int x = w1X[i]; x <= w1X[i] + 1; x++)
                for (int y = 0; y <= 2; y++)
                    p.Spawn(p.DashWall, inter, x, y);
        }

        GameObject d1 = p.Spawn(p.Diamond, col, 72f, 4.5f);
        SetDiamondId(d1, 1);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 140f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Grand Excavation Labyrinth (X = 150 to 300)
        p.Spawn(p.Slope45, env, 165f, 0f);
        p.Spawn(p.Ledge8, env, 175f, 5f);

        GameObject camDrop2 = p.Spawn(p.CameraZoneWide, flow, 195f, 0f);
        SetTriggerBoxSize(camDrop2, new Vector3(45f, 25f, 10f));

        // 3-layer excavation shaft
        for (int x = 182; x <= 186; x++) p.Spawn(p.DashWall, inter, x, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 184f, 2f);
        for (int x = 182; x <= 186; x++) p.Spawn(p.DashWall, inter, x, -1f);

        p.Spawn(p.Ground20, env, 190f, -6f);
        GameObject d2 = p.Spawn(p.Diamond, col, 184f, -5f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 180f + i * 3f, -5f);
            SetAtomId(a, 1 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 205f, -5f);

        // Chimney 1
        p.Spawn(p.Wall12, env, 248f, 7f);
        p.Spawn(p.Wall12, env, 252f, 7f);
        p.Spawn(p.BouncePadMini, inter, 250f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 250f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        p.Spawn(p.OneWayPlatform, env, 260f, 11f);
        GameObject d3 = p.Spawn(p.Diamond, col, 260f, 12.5f);
        SetDiamondId(d3, 3);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 290f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Kinetic Skyway & Crucible (X = 300 to 450)
        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 315f, 4f);
        ConfigureMovingPlatform(mp1, new Vector3(310f, 4f, 0f), new Vector3(330f, 4f, 0f), 4f);
        p.Spawn(p.SpinArm, haz, 320f, 5.5f);

        p.Spawn(p.IceSurface, env, 355f, 0f);
        p.Spawn(p.IceSurface, env, 370f, 0f);

        p.Spawn(p.DashBoostFast, inter, 380f, 0.5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 395f, 2f);

        // Chimney 2 Rocket Launch
        p.Spawn(p.Wall12, env, 408f, 7f);
        p.Spawn(p.Wall12, env, 412f, 7f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 410f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 409; x <= 411; x++) p.Spawn(p.DashWall, inter, x, 12f);

        p.Spawn(p.OneWayPlatform, env, 420f, 12f);
        GameObject d4 = p.Spawn(p.Diamond, col, 420f, 13.5f);
        SetDiamondId(d4, 4);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 415f + i * 5f, 11f);
            SetAtomId(a, 5 + i);
        }

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 440f, 2f);
        cp3.name = "Checkpoint_3";

        // Act 4: Grand Rocket Tower & Victory Canyon (X = 450 to 600)
        p.Spawn(p.Wall12, env, 468f, 8f);
        p.Spawn(p.Wall12, env, 472f, 8f);

        GameObject camWideGrand = p.Spawn(p.CameraZoneWide, flow, 470f, 10f);
        SetTriggerBoxSize(camWideGrand, new Vector3(30f, 30f, 10f));

        GameObject grandLane = p.Spawn(p.DashLaneUp, inter, 470f, 7f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(16f, 3f));

        for (int x = 469; x <= 471; x++) p.Spawn(p.DashWall, inter, x, 14f);

        GameObject d5 = p.Spawn(p.Diamond, col, 470f, 15f);
        SetDiamondId(d5, 5);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 470f, 4f + i * 2.5f);
            SetAtomId(a, 9 + i);
        }

        // Parabolic Mega Bounce Canyon
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 540f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(70f, 28f, 10f));

        p.Spawn(p.BouncePadMega, inter, 510f, 0f);
        p.Spawn(p.BouncePadMega, inter, 535f, 0f);
        p.Spawn(p.BouncePadMega, inter, 560f, 0f);

        for (int i = 0; i < 4; i++)
        {
            float ax = 515f + i * 14f;
            float ay = 7f - Mathf.Abs(i - 1.5f) * 1.5f;
            GameObject a = p.Spawn(p.Atom, col, ax, ay);
            SetAtomId(a, 13 + i);
        }

        p.Spawn(p.Ground20, env, 585f, 3f);
        FinalizeScene(scene);
        return $"Level_05 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 06: Twin-Shaft Excavation (X = 0 to 380)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 06 (Twin-Shaft Excavation)")]
    public static string BuildLevel06()
    {
        string scenePath = BaseDir + "Level_06.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 375f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 62f, 14f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 400f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 190f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(650f, 6f, 10f));

        for (int x = 0; x <= 380; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The Climb & The First Quarry Drop (X = 0 to 130)
        p.Spawn(p.Slope45, env, 18f, 0f);
        p.Spawn(p.Ledge8, env, 26f, 3f);

        // Shaft 1 (Ascent): Twin-Wall Chimney 1
        p.Spawn(p.Wall12, env, 30f, 9f);
        p.Spawn(p.Wall12, env, 34f, 9f);
        p.Spawn(p.BouncePadMini, inter, 32f, 3f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 32f, 9f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 22f, 10f));

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 32f, 5f + i * 3f);
            SetAtomId(a, 1 + i);
        }

        // High Catwalk at Y = 14
        p.Spawn(p.OneWayPlatform, env, 42f, 14f);
        p.Spawn(p.OneWayPlatform, env, 54f, 14f);
        p.Spawn(p.OneWayPlatform, env, 66f, 14f);

        // Shaft 1 (Descent - Ground Pound Drop)
        GameObject camDrop1 = p.Spawn(p.CameraZoneWide, flow, 72f, 4f);
        SetTriggerBoxSize(camDrop1, new Vector3(35f, 28f, 10f));

        for (int x = 70; x <= 74; x++) p.Spawn(p.DashWall, inter, x, 13f);
        p.Spawn(p.CrackedBlocks4x4, inter, 72f, 9f);
        for (int x = 70; x <= 74; x++) p.Spawn(p.DashWall, inter, x, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 72f, 1f);

        // Subterranean Room 1 at Y = -3
        p.Spawn(p.Ground20, env, 72f, -4f);
        for (int x = 66; x <= 67; x++) p.Spawn(p.DashWall, inter, x, -3f);
        for (int x = 77; x <= 78; x++) p.Spawn(p.DashWall, inter, x, -3f);

        GameObject d1 = p.Spawn(p.Diamond, col, 72f, -2.5f);
        SetDiamondId(d1, 1);
        GameObject a4 = p.Spawn(p.Atom, col, 69f, -2.5f);
        SetAtomId(a4, 4);
        GameObject a5 = p.Spawn(p.Atom, col, 75f, -2.5f);
        SetAtomId(a5, 5);

        p.Spawn(p.BouncePadMega, inter, 84f, -3f);

        // Speed tunnel
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 105f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(35f, 12f, 10f));

        p.Spawn(p.DashBoostFast, inter, 96f, 0.5f);
        for (int x = 108; x <= 110; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 125f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Second Twin-Shaft (X = 130 to 250)
        p.Spawn(p.FireJet, haz, 142f, 0f);

        // Shaft 2 (Ascent - Speed Rocket)
        p.Spawn(p.Wall12, env, 158f, 7f);
        p.Spawn(p.Wall12, env, 162f, 7f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 160f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 159; x <= 161; x++) p.Spawn(p.DashWall, inter, x, 12f);

        GameObject camWide2 = p.Spawn(p.CameraZoneWide, flow, 175f, 14f);
        SetTriggerBoxSize(camWide2, new Vector3(45f, 25f, 10f));

        p.Spawn(p.OneWayPlatform, env, 168f, 14f);
        p.Spawn(p.OneWayPlatform, env, 180f, 14f);
        p.Spawn(p.OneWayPlatform, env, 192f, 14f);

        GameObject d2 = p.Spawn(p.Diamond, col, 168f, 15.5f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 176f + i * 8f, 15.5f);
            SetAtomId(a, 6 + i);
        }

        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 186f, 14f);
        ConfigureMovingPlatform(mp1, new Vector3(175f, 14f, 0f), new Vector3(195f, 14f, 0f), 4f);

        // Shaft 2 (Descent - Deep Excavation Drop)
        for (int x = 209; x <= 213; x++) p.Spawn(p.DashWall, inter, x, 12f);
        p.Spawn(p.CrackedBlocks4x4, inter, 211f, 7f);
        p.Spawn(p.CrackedBlocks4x4, inter, 211f, 2f);

        // Subterranean Room 2 at Y = -4
        p.Spawn(p.Ground20, env, 215f, -5f);
        for (int x = 206; x <= 208; x++) p.Spawn(p.DashWall, inter, x, -4f);
        for (int x = 214; x <= 216; x++) p.Spawn(p.DashWall, inter, x, -4f);

        GameObject d3 = p.Spawn(p.Diamond, col, 211f, -3.5f);
        SetDiamondId(d3, 3);

        p.Spawn(p.BouncePadMega, inter, 226f, -4f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 245f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Quarry Gauntlet & Exit (X = 250 to 380)
        for (int x = 265; x <= 277; x++)
        {
            int h = (x <= 271) ? (x - 265 + 1) : (277 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        // Booster 1 into Wall
        p.Spawn(p.DashBoostFast, inter, 295f, 0.5f);
        for (int x = 308; x <= 309; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);
        GameObject d4 = p.Spawn(p.Diamond, col, 308f, 4f);
        SetDiamondId(d4, 4);

        // Booster 2 into Wall
        p.Spawn(p.DashBoostFast, inter, 322f, 0.5f);
        for (int x = 335; x <= 336; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 290f + i * 15f, 2f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 360f, 1f);
        GameObject d5 = p.Spawn(p.Diamond, col, 355f, 3f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_06 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 07: Skywalk & Sub-Tunnel (X = 0 to 420)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 07 (Skywalk & Sub-Tunnel)")]
    public static string BuildLevel07()
    {
        string scenePath = BaseDir + "Level_07.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 415f, 3f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 22f, 0f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 440f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 210f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(720f, 6f, 10f));

        for (int x = 0; x <= 420; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The Split (X = 0 to 140)
        // Upper Skywalk Fork (Y = 10)
        p.Spawn(p.Slope45, env, 22f, 0f);
        p.Spawn(p.Ledge8, env, 32f, 5f);
        for (int x = 42; x <= 90; x += 12) p.Spawn(p.OneWayPlatform, env, x, 10f);

        p.Spawn(p.DashBoostFast, inter, 45f, 10.5f);
        for (int x = 55; x <= 65; x++)
            for (int y = 10; y <= 11; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 60f, 11f);
        SetTriggerBoxSize(camTight1, new Vector3(35f, 12f, 10f));

        GameObject d1 = p.Spawn(p.Diamond, col, 75f, 11.5f);
        SetDiamondId(d1, 1);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 70f + i * 8f, 11.5f);
            SetAtomId(a, 1 + i);
        }

        // Lower Sub-Tunnel Fork (Y = -3)
        p.Spawn(p.CrackedBlocks4x4, inter, 35f, 1f);
        p.Spawn(p.Ground20, env, 45f, -4f);
        p.Spawn(p.Ground20, env, 65f, -4f);

        for (int x = 46; x <= 48; x++) p.Spawn(p.DashWall, inter, x, -3f);
        for (int x = 60; x <= 63; x++) p.Spawn(p.DashWall, inter, x, -3f);
        p.Spawn(p.SawSlow, haz, 54f, -1f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 50f + i * 6f, -3f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 85f, -3f);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 135f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Deep Quarry & Chevron Express (X = 140 to 280)
        p.Spawn(p.IceSurface, env, 150f, 0f);
        p.Spawn(p.IceSurface, env, 165f, 0f);

        // Upper Express
        p.Spawn(p.Wall12, env, 182f, 7f);
        p.Spawn(p.Wall12, env, 186f, 7f);
        p.Spawn(p.BouncePadMini, inter, 184f, 0f);

        p.Spawn(p.OneWayPlatform, env, 195f, 11f);
        p.Spawn(p.OneWayPlatform, env, 210f, 11f);
        p.Spawn(p.OneWayPlatform, env, 225f, 11f);

        p.Spawn(p.DashBoostFast, inter, 200f, 11.5f);
        for (int x = 212; x <= 214; x++)
            for (int y = 11; y <= 13; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject d2 = p.Spawn(p.Diamond, col, 213f, 14.5f);
        SetDiamondId(d2, 2);

        p.Spawn(p.DashBoostFast, inter, 225f, 11.5f);
        for (int x = 237; x <= 239; x++)
            for (int y = 11; y <= 13; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Lower Cavern
        p.Spawn(p.CrackedBlocks4x4, inter, 195f, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 195f, 1f);

        p.Spawn(p.Ground20, env, 210f, -6f);
        p.Spawn(p.Ground20, env, 230f, -6f);
        p.Spawn(p.LavaLeaper, haz, 215f, -5f);

        GameObject d3 = p.Spawn(p.Diamond, col, 225f, -4.5f);
        SetDiamondId(d3, 3);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 210f + i * 8f, -4.5f);
            SetAtomId(a, 7 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 245f, -5f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 275f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Kinetic Skyway & Grand Portal (X = 280 to 420)
        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 295f, 4f);
        ConfigureMovingPlatform(mp1, new Vector3(290f, 4f, 0f), new Vector3(310f, 4f, 0f), 4f);

        p.Spawn(p.CrackedBlocks4x4, inter, 315f, 4f);

        GameObject mp2 = p.Spawn(p.MovingPlatform, inter, 330f, 4f);
        ConfigureMovingPlatform(mp2, new Vector3(320f, 4f, 0f), new Vector3(340f, 4f, 0f), 4f);

        p.Spawn(p.DashBoostFast, inter, 345f, 1.5f);
        for (int x = 358; x <= 361; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject d4 = p.Spawn(p.Diamond, col, 360f, 4.5f);
        SetDiamondId(d4, 4);

        // Canyon Bounce
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 390f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(60f, 25f, 10f));

        p.Spawn(p.BouncePadMega, inter, 375f, 0f);
        p.Spawn(p.BouncePadMega, inter, 390f, 0f);
        p.Spawn(p.BouncePadMega, inter, 405f, 0f);

        for (int i = 0; i < 5; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 370f + i * 9f, 6f);
            SetAtomId(a, 10 + i);
        }

        p.Spawn(p.Ground20, env, 410f, 2f);
        GameObject d5 = p.Spawn(p.Diamond, col, 410f, 4f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_07 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 08: The Vertical Vault (X = 0 to 380)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 08 (The Vertical Vault)")]
    public static string BuildLevel08()
    {
        string scenePath = BaseDir + "Level_08.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 375f, 3f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 75f, 15f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 400f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 190f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(650f, 6f, 10f));

        for (int x = 0; x <= 380; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Tower 1 - Zig-Zag Ascent & Descent (X = 0 to 130)
        p.Spawn(p.Wall12, env, 28f, 7f);
        p.Spawn(p.Wall12, env, 32f, 7f);
        p.Spawn(p.BouncePadMini, inter, 30f, 0f);
        p.Spawn(p.BouncePadMini, inter, 30f, 4f);
        p.Spawn(p.BouncePadMini, inter, 30f, 8f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 30f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 22f, 10f));

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 30f, 2f + i * 3.5f);
            SetAtomId(a, 1 + i);
        }

        // Upper Bridge (Y = 14)
        for (int x = 38; x <= 74; x += 12) p.Spawn(p.OneWayPlatform, env, x, 14f);

        p.Spawn(p.DashBoostFast, inter, 42f, 14.5f);
        for (int y = 14; y <= 16; y++)
        {
            p.Spawn(p.DashWall, inter, 52f, y);
            p.Spawn(p.DashWall, inter, 60f, y);
            p.Spawn(p.DashWall, inter, 68f, y);
        }

        GameObject d1 = p.Spawn(p.Diamond, col, 60f, 17.5f);
        SetDiamondId(d1, 1);

        // Shaft B (Downward Drop)
        for (int x = 79; x <= 83; x++) p.Spawn(p.DashWall, inter, x, 13f);
        p.Spawn(p.CrackedBlocks4x4, inter, 81f, 9f);
        for (int x = 79; x <= 83; x++) p.Spawn(p.DashWall, inter, x, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 81f, 1f);

        // Sub-Vault 1 at Y = -3
        p.Spawn(p.Ground20, env, 85f, -4f);
        GameObject d2 = p.Spawn(p.Diamond, col, 81f, -2.5f);
        SetDiamondId(d2, 2);
        GameObject a4 = p.Spawn(p.Atom, col, 77f, -2.5f);
        SetAtomId(a4, 4);
        GameObject a5 = p.Spawn(p.Atom, col, 85f, -2.5f);
        SetAtomId(a5, 5);

        p.Spawn(p.BouncePadMega, inter, 95f, -3f);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 125f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Tower 2 - The Rocket Vault (X = 130 to 250)
        p.Spawn(p.Wall12, env, 148f, 7f);
        p.Spawn(p.Wall12, env, 152f, 7f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 150f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 149; x <= 151; x++) p.Spawn(p.DashWall, inter, x, 12f);

        GameObject camWide2 = p.Spawn(p.CameraZoneWide, flow, 165f, 14f);
        SetTriggerBoxSize(camWide2, new Vector3(45f, 25f, 10f));

        p.Spawn(p.OneWayPlatform, env, 158f, 14f);
        p.Spawn(p.OneWayPlatform, env, 172f, 14f);
        p.Spawn(p.OneWayPlatform, env, 186f, 14f);

        GameObject d3 = p.Spawn(p.Diamond, col, 165f, 16.5f);
        SetDiamondId(d3, 3);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 160f + i * 10f, 15.5f);
            SetAtomId(a, 6 + i);
        }

        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 175f, 14f);
        ConfigureMovingPlatform(mp1, new Vector3(165f, 14f, 0f), new Vector3(185f, 14f, 0f), 4f);

        // Shaft D (Heavy Excavation Drop)
        for (int x = 196; x <= 200; x++) p.Spawn(p.DashWall, inter, x, 13f);
        p.Spawn(p.CrackedBlocks4x4, inter, 198f, 8f);
        for (int x = 196; x <= 200; x++) p.Spawn(p.DashWall, inter, x, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 198f, 1f);

        // Deep Vault at Y = -5
        p.Spawn(p.Ground20, env, 205f, -6f);
        p.Spawn(p.SpinArm, haz, 205f, -3.5f);
        GameObject d4 = p.Spawn(p.Diamond, col, 198f, -4.5f);
        SetDiamondId(d4, 4);
        GameObject a9 = p.Spawn(p.Atom, col, 194f, -4.5f);
        SetAtomId(a9, 9);
        GameObject a10 = p.Spawn(p.Atom, col, 202f, -4.5f);
        SetAtomId(a10, 10);

        p.Spawn(p.BouncePadMega, inter, 218f, -5f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 245f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Fortress Vault & Exit (X = 250 to 380)
        for (int x = 265; x <= 277; x++)
        {
            int h = (x <= 271) ? (x - 265 + 1) : (277 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        GameObject camTight2 = p.Spawn(p.CameraZoneTight, flow, 310f, 2f);
        SetTriggerBoxSize(camTight2, new Vector3(45f, 12f, 10f));

        p.Spawn(p.DashBoostFast, inter, 295f, 0.5f);
        for (int x = 308; x <= 310; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        p.Spawn(p.DashBoostFast, inter, 320f, 0.5f);
        for (int x = 332; x <= 334; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        p.Spawn(p.BouncePad, inter, 345f, 0f);
        p.Spawn(p.BouncePad, inter, 355f, 0f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 342f + i * 6f, 3f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 365f, 2f);
        GameObject d5 = p.Spawn(p.Diamond, col, 365f, 4f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_08 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 09: Kinetic Reactor (X = 0 to 440)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 09 (Kinetic Reactor)")]
    public static string BuildLevel09()
    {
        string scenePath = BaseDir + "Level_09.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 435f, 3f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 460f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 220f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(750f, 6f, 10f));

        for (int x = 0; x <= 440; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Reactor Core - Lower Chamber & Airborne Shatter (X = 0 to 140)
        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 25f, 3f);
        ConfigureMovingPlatform(mp1, new Vector3(18f, 3f, 0f), new Vector3(36f, 3f, 0f), 4f);

        // Vertical Launch into Mid-Air Floating DashWalls
        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 45f, 2f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(12f, 3f));

        for (int x = 44; x <= 46; x++)
            for (int y = 8; y <= 10; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject camWide1 = p.Spawn(p.CameraZoneWide, flow, 50f, 10f);
        SetTriggerBoxSize(camWide1, new Vector3(35f, 22f, 10f));

        p.Spawn(p.OneWayPlatform, env, 52f, 11f);
        p.Spawn(p.OneWayPlatform, env, 64f, 11f);
        GameObject d1 = p.Spawn(p.Diamond, col, 52f, 12.5f);
        SetDiamondId(d1, 1);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 55f + i * 5f, 12.5f);
            SetAtomId(a, 1 + i);
        }

        // Drop Shaft at X = 74
        p.Spawn(p.CrackedBlocks4x4, inter, 74f, 4f);
        for (int x = 72; x <= 76; x++) p.Spawn(p.DashWall, inter, x, 1f);

        // Reactor Sub-Floor at Y = -3
        p.Spawn(p.Ground20, env, 80f, -4f);
        p.Spawn(p.StompBlock, haz, 82f, 1f);
        GameObject d2 = p.Spawn(p.Diamond, col, 74f, -2.5f);
        SetDiamondId(d2, 2);

        p.Spawn(p.BouncePadMega, inter, 94f, -3f);

        // Speed tunnel
        p.Spawn(p.DashBoostFast, inter, 104f, 0.5f);
        for (int x = 116; x <= 118; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 135f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Hazard Crucible & High Rocket Ascent (X = 140 to 280)
        p.Spawn(p.Cannon, haz, 150f, 8f);
        p.Spawn(p.FireJet, haz, 162f, 0f);

        // Twin-Wall Chimney Rocket
        p.Spawn(p.Wall12, env, 174f, 7f);
        p.Spawn(p.Wall12, env, 178f, 7f);

        GameObject laneUp2 = p.Spawn(p.DashLaneUp, inter, 176f, 5f);
        laneUp2.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp2, new Vector2(14f, 3f));

        p.Spawn(p.CrackedBlocks4x4, inter, 176f, 12f);

        // Sky Catwalk
        p.Spawn(p.OneWayPlatform, env, 188f, 14f);
        p.Spawn(p.OneWayPlatform, env, 202f, 14f);
        p.Spawn(p.OneWayPlatform, env, 216f, 14f);

        GameObject mp2 = p.Spawn(p.MovingPlatform, inter, 200f, 14f);
        ConfigureMovingPlatform(mp2, new Vector3(190f, 14f, 0f), new Vector3(210f, 14f, 0f), 4f);
        p.Spawn(p.SpinArm, haz, 205f, 14f);

        GameObject d3 = p.Spawn(p.Diamond, col, 195f, 15.5f);
        SetDiamondId(d3, 3);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 190f + i * 8f, 16f);
            SetAtomId(a, 4 + i);
        }

        // 3-story excavation drop
        for (int x = 236; x <= 240; x++) p.Spawn(p.DashWall, inter, x, 12f);
        p.Spawn(p.CrackedBlocks4x4, inter, 238f, 7f);
        p.Spawn(p.CrackedBlocks4x4, inter, 238f, 2f);

        // Subterranean vault at Y = -4
        p.Spawn(p.Ground20, env, 245f, -5f);
        GameObject d4 = p.Spawn(p.Diamond, col, 238f, -3.5f);
        SetDiamondId(d4, 4);

        p.Spawn(p.BouncePadMega, inter, 254f, -4f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 275f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Reactor Core Overload & Exit (X = 280 to 440)
        p.Spawn(p.IceSurface, env, 290f, 0f);
        p.Spawn(p.IceSurface, env, 305f, 0f);

        GameObject camTight2 = p.Spawn(p.CameraZoneTight, flow, 350f, 2f);
        SetTriggerBoxSize(camTight2, new Vector3(65f, 12f, 10f));

        int[] b9X = new int[] { 325, 350, 375 };
        int[] w9X = new int[] { 338, 363, 388 };
        for (int i = 0; i < b9X.Length; i++)
        {
            p.Spawn(p.DashBoostFast, inter, b9X[i], 0.5f);
            for (int x = w9X[i]; x <= w9X[i] + 1; x++)
                for (int y = 0; y <= 2; y++)
                    p.Spawn(p.DashWall, inter, x, y);
        }

        // Canyon Mega Bounce
        p.Spawn(p.BouncePadMega, inter, 400f, 0f);
        p.Spawn(p.BouncePadMega, inter, 415f, 0f);

        for (int i = 0; i < 5; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 395f + i * 6f, 4f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.Ground20, env, 430f, 2f);
        GameObject d5 = p.Spawn(p.Diamond, col, 430f, 4f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_09 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 10: Purple Citadel Finale (X = 0 to 600)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 10 (Purple Citadel Finale)")]
    public static string BuildLevel10()
    {
        string scenePath = BaseDir + "Level_10.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 595f, 3f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 20f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 60f, 14f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 620f, 40f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 300f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(850f, 6f, 10f));

        for (int x = 0; x <= 600; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The Citadel Gates & Lower Vaults (X = 0 to 150)
        p.Spawn(p.Wall12, env, 24f, 7f);
        p.Spawn(p.Wall12, env, 28f, 7f);
        p.Spawn(p.BouncePadMini, inter, 26f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 26f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 26f, 3f + i * 3f);
            SetAtomId(a, 1 + i);
        }

        // Gate Ramparts (Y = 13)
        p.Spawn(p.OneWayPlatform, env, 36f, 13f);
        p.Spawn(p.OneWayPlatform, env, 48f, 13f);

        p.Spawn(p.DashBoostFast, inter, 36f, 13.5f);
        for (int x = 48; x <= 50; x++)
            for (int y = 13; y <= 15; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Deep Citadel Excavation Shaft 1 (X = 66 to 72)
        GameObject camDrop1 = p.Spawn(p.CameraZoneWide, flow, 69f, 4f);
        SetTriggerBoxSize(camDrop1, new Vector3(40f, 28f, 10f));

        for (int x = 67; x <= 71; x++) p.Spawn(p.DashWall, inter, x, 12f);
        p.Spawn(p.CrackedBlocks4x4, inter, 69f, 8f);
        for (int x = 67; x <= 71; x++) p.Spawn(p.DashWall, inter, x, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 69f, 1f);

        // Subterranean Crypt at Y = -5
        p.Spawn(p.Ground20, env, 75f, -6f);
        GameObject d1 = p.Spawn(p.Diamond, col, 69f, -4.5f);
        SetDiamondId(d1, 1);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 64f + i * 5f, -4.5f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 85f, -5f);

        // Speed tunnel
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 115f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(45f, 12f, 10f));

        p.Spawn(p.DashBoostFast, inter, 105f, 0.5f);
        for (int x = 118; x <= 120; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 145f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Grand Catacombs & Rocket Spire (X = 150 to 300)
        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 165f, 5f);
        ConfigureMovingPlatform(mp1, new Vector3(155f, 5f, 0f), new Vector3(175f, 5f, 0f), 4f);
        p.Spawn(p.SpinArm, haz, 170f, 6.5f);

        // Ground Pound Drop into Catacombs Vault
        p.Spawn(p.CrackedBlocks4x4, inter, 190f, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 190f, 1f);

        p.Spawn(p.Ground20, env, 195f, -5f);
        GameObject d2 = p.Spawn(p.Diamond, col, 190f, -3.5f);
        SetDiamondId(d2, 2);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 185f + i * 5f, -3.5f);
            SetAtomId(a, 7 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 210f, -4f);

        // Rocket Spire (Chimney 2)
        p.Spawn(p.Wall12, env, 234f, 8f);
        p.Spawn(p.Wall12, env, 238f, 8f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 236f, 5f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(16f, 3f));

        for (int x = 235; x <= 237; x++) p.Spawn(p.DashWall, inter, x, 12f);

        // Sky Gallery at Y = 15
        GameObject camWide2 = p.Spawn(p.CameraZoneWide, flow, 250f, 14f);
        SetTriggerBoxSize(camWide2, new Vector3(40f, 25f, 10f));

        p.Spawn(p.OneWayPlatform, env, 246f, 15f);
        p.Spawn(p.OneWayPlatform, env, 260f, 15f);

        GameObject d3 = p.Spawn(p.Diamond, col, 250f, 16.5f);
        SetDiamondId(d3, 3);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 252f + i * 8f, 16.5f);
            SetAtomId(a, 10 + i);
        }

        p.Spawn(p.Slope45Left, env, 280f, 8f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 295f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Crucible & Quarry Labyrinth (X = 300 to 450)
        for (int x = 310; x <= 324; x++)
        {
            int h = (x <= 317) ? (x - 310 + 1) : (324 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        p.Spawn(p.StompBlock, haz, 345f, 5f);
        p.Spawn(p.FireJet, haz, 358f, 0f);
        p.Spawn(p.LavaLeaper, haz, 370f, -2f);

        // Excavation Drop 3
        p.Spawn(p.CrackedBlocks4x4, inter, 375f, 6f);
        p.Spawn(p.CrackedBlocks4x4, inter, 375f, 2f);

        p.Spawn(p.Ground20, env, 380f, -5f);
        GameObject d4 = p.Spawn(p.Diamond, col, 375f, -3.5f);
        SetDiamondId(d4, 4);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 372f + i * 5f, -3.5f);
            SetAtomId(a, 13 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 392f, -4f);

        p.Spawn(p.DashBoostFast, inter, 410f, 0.5f);
        for (int x = 422; x <= 424; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 445f, 2f);
        cp3.name = "Checkpoint_3";

        // Act 4: The Grand Sky Citadel & Supersonic Triumph (X = 450 to 600)
        // Grand Rocket Tower (Chimney 3)
        p.Spawn(p.Wall12, env, 472f, 9f);
        p.Spawn(p.Wall12, env, 476f, 9f);

        GameObject grandLane = p.Spawn(p.DashLaneUp, inter, 474f, 7f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(18f, 3f));

        for (int x = 473; x <= 475; x++) p.Spawn(p.DashWall, inter, x, 15f);

        GameObject camCitadel = p.Spawn(p.CameraZoneWide, flow, 490f, 16f);
        SetTriggerBoxSize(camCitadel, new Vector3(45f, 28f, 10f));

        p.Spawn(p.OneWayPlatform, env, 488f, 15f);
        p.Spawn(p.OneWayPlatform, env, 502f, 15f);
        p.Spawn(p.OneWayPlatform, env, 514f, 15f);

        GameObject d5 = p.Spawn(p.Diamond, col, 474f, 16.5f);
        SetDiamondId(d5, 5);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 480f + i * 8f, 16.5f);
            SetAtomId(a, 16 + i);
        }

        // Parabolic Canyon Mega Bounce
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 545f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(65f, 26f, 10f));

        p.Spawn(p.BouncePadMega, inter, 520f, 0f);
        p.Spawn(p.BouncePadMega, inter, 542f, 0f);
        p.Spawn(p.BouncePadMega, inter, 564f, 0f);

        GameObject a19 = p.Spawn(p.Atom, col, 535f, 8f);
        SetAtomId(a19, 19);
        GameObject a20 = p.Spawn(p.Atom, col, 555f, 8f);
        SetAtomId(a20, 20);

        // Supersonic Finale Runway
        p.Spawn(p.DashBoostFast, inter, 570f, 0.5f);
        for (int x = 582; x <= 584; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        p.Spawn(p.Ground20, env, 595f, 2f);

        FinalizeScene(scene);
        return $"Level_10 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 11: Subterranean Excavation Maze (X = 0 to 420)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 11 (Subterranean Excavation Maze)")]
    public static string BuildLevel11()
    {
        string scenePath = BaseDir + "Level_11.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 12f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 415f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 12f);
        p.Spawn(p.TutorialGroundPound, flow, 25f, 12f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 440f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 210f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(720f, 6f, 10f));

        for (int x = 0; x <= 420; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: High Ridge & The Deep Plunge (X = 0 to 140)
        // High Ridge at Y = 12
        for (int x = 0; x <= 25; x += 8) p.Spawn(p.Ledge8, env, x, 11f);

        // Deep Plunge Shaft (X = 30 to 36)
        GameObject camDrop1 = p.Spawn(p.CameraZoneWide, flow, 45f, 4f);
        SetTriggerBoxSize(camDrop1, new Vector3(45f, 28f, 10f));

        for (int x = 30; x <= 34; x++) p.Spawn(p.DashWall, inter, x, 11f);
        p.Spawn(p.CrackedBlocks4x4, inter, 32f, 7f);
        for (int x = 30; x <= 34; x++) p.Spawn(p.DashWall, inter, x, 3f);
        p.Spawn(p.CrackedBlocks4x4, inter, 32f, -1f);

        // Subterranean Cavern Maze (Y = -5 floor)
        p.Spawn(p.Ground20, env, 45f, -6f);
        p.Spawn(p.Ground20, env, 65f, -6f);
        p.Spawn(p.Ground20, env, 85f, -6f);

        // Hollow Box Vault with Diamond 1 inside
        for (int x = 58; x <= 62; x++)
        {
            p.Spawn(p.DashWall, inter, x, -2f);
            p.Spawn(p.DashWall, inter, x, 2f);
        }
        p.Spawn(p.DashWall, inter, 58f, -1f);
        p.Spawn(p.DashWall, inter, 58f, 0f);
        p.Spawn(p.DashWall, inter, 58f, 1f);
        p.Spawn(p.DashWall, inter, 62f, -1f);
        p.Spawn(p.DashWall, inter, 62f, 0f);
        p.Spawn(p.DashWall, inter, 62f, 1f);

        GameObject d1 = p.Spawn(p.Diamond, col, 60f, 0f);
        SetDiamondId(d1, 1);

        // Maze Hazards: StompBlock and FireJet
        p.Spawn(p.StompBlock, haz, 75f, 2f);
        p.Spawn(p.FireJet, haz, 90f, -5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 45f + i * 14f, -4.5f);
            SetAtomId(a, 1 + i);
        }

        GameObject d2 = p.Spawn(p.Diamond, col, 115f, -3.5f);
        SetDiamondId(d2, 2);

        p.Spawn(p.BouncePadMega, inter, 120f, -5f);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 135f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Checkerboard Cavern & Upper Catwalks (X = 140 to 280)
        p.Spawn(p.OneWayPlatform, env, 145f, 6f);
        p.Spawn(p.OneWayPlatform, env, 160f, 8f);

        // Checkerboard Box Grid (X = 175 to 205)
        for (int x = 175; x <= 205; x += 4)
        {
            p.Spawn(p.DashWall, inter, x, 0f);
            p.Spawn(p.CrackedBlocks4x4, inter, x + 2, 2f);
        }

        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 190f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(45f, 12f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 195f, 5.5f);
        SetDiamondId(d3, 3);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 175f + i * 10f, 4f);
            SetAtomId(a, 5 + i);
        }

        // Subterranean Excavation Trench 2
        p.Spawn(p.CrackedBlocks4x4, inter, 220f, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 220f, 1f);
        p.Spawn(p.Ground20, env, 230f, -6f);

        GameObject d4 = p.Spawn(p.Diamond, col, 230f, -4.5f);
        SetDiamondId(d4, 4);

        p.Spawn(p.BouncePadMega, inter, 245f, -5f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 270f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Quarry Maze & Ascent (X = 280 to 420)
        p.Spawn(p.DashBoostFast, inter, 290f, 0.5f);
        for (int x = 302; x <= 305; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Stepped Pyramid Quarry
        for (int x = 320; x <= 334; x++)
        {
            int h = (x <= 327) ? (x - 320 + 1) : (334 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        p.Spawn(p.BouncePadMega, inter, 350f, 0f);
        p.Spawn(p.BouncePadMega, inter, 375f, 0f);

        for (int i = 0; i < 7; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 345f + i * 8f, 5f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 405f, 1f);
        GameObject d5 = p.Spawn(p.Diamond, col, 405f, 3.5f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_11 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 12: Kinetic Pinball Canyon (X = 0 to 440)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 12 (Kinetic Pinball Canyon)")]
    public static string BuildLevel12()
    {
        string scenePath = BaseDir + "Level_12.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 14f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 430f, 4f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 14f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 460f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 220f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(750f, 6f, 10f));

        for (int x = 0; x <= 440; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The Giant Plunge & Ricochet Arc (X = 0 to 135)
        for (int x = 0; x <= 20; x += 8) p.Spawn(p.Ledge8, env, x, 13f);

        // Huge Momentum Slope
        p.Spawn(p.Slope45Large, env, 25f, 5f);
        p.Spawn(p.BouncePadMega, inter, 45f, 0f);

        // Mid-Air Floating DashWalls shattered mid-trajectory!
        for (int x = 54; x <= 56; x++)
            for (int y = 8; y <= 10; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject camPinball1 = p.Spawn(p.CameraZoneWide, flow, 65f, 8f);
        SetTriggerBoxSize(camPinball1, new Vector3(50f, 25f, 10f));

        GameObject d1 = p.Spawn(p.Diamond, col, 55f, 11.5f);
        SetDiamondId(d1, 1);

        // Ricochet chain
        p.Spawn(p.BouncePad, inter, 75f, 0f);
        p.Spawn(p.BouncePadMini, inter, 90f, 4f);
        p.Spawn(p.BouncePadMega, inter, 105f, 0f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 65f + i * 12f, 7f);
            SetAtomId(a, 1 + i);
        }

        GameObject d2 = p.Spawn(p.Diamond, col, 115f, 5.5f);
        SetDiamondId(d2, 2);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 130f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Diagonal Box Ramps & Diamond Vault (X = 135 to 270)
        p.Spawn(p.Slope45, env, 145f, 0f);
        p.Spawn(p.Ledge8, env, 155f, 4f);

        // Ascending Diagonal Box Ramp (X = 165 to 180)
        for (int i = 0; i < 8; i++)
        {
            for (int y = 0; y <= i; y++)
                p.Spawn(p.DashWall, inter, 165f + i * 2f, y);
        }

        // Suspended Diamond Box Vault at X = 205, Y = 10
        for (int x = 203; x <= 207; x++)
        {
            p.Spawn(p.DashWall, inter, x, 8f);
            p.Spawn(p.DashWall, inter, x, 12f);
        }
        p.Spawn(p.DashWall, inter, 203f, 9f);
        p.Spawn(p.DashWall, inter, 203f, 10f);
        p.Spawn(p.DashWall, inter, 203f, 11f);
        p.Spawn(p.DashWall, inter, 207f, 9f);
        p.Spawn(p.DashWall, inter, 207f, 10f);
        p.Spawn(p.DashWall, inter, 207f, 11f);

        GameObject d3 = p.Spawn(p.Diamond, col, 205f, 10f);
        SetDiamondId(d3, 3);

        p.Spawn(p.BouncePadMega, inter, 195f, 0f);
        p.Spawn(p.BouncePadMega, inter, 220f, 0f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 190f + i * 10f, 14f);
            SetAtomId(a, 5 + i);
        }

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 265f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Grand Pinball Finale (X = 270 to 440)
        p.Spawn(p.DashBoostFast, inter, 280f, 0.5f);
        for (int x = 292; x <= 294; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Quad Mega Bounce Arena
        GameObject camGrandPin = p.Spawn(p.CameraZoneWide, flow, 360f, 8f);
        SetTriggerBoxSize(camGrandPin, new Vector3(75f, 28f, 10f));

        p.Spawn(p.BouncePadMega, inter, 320f, 0f);
        p.Spawn(p.BouncePadMega, inter, 345f, 0f);
        p.Spawn(p.BouncePadMega, inter, 370f, 0f);
        p.Spawn(p.BouncePadMega, inter, 395f, 0f);

        // Floating shattered blocks over canyon
        for (int y = 6; y <= 8; y++)
        {
            p.Spawn(p.DashWall, inter, 335f, y);
            p.Spawn(p.DashWall, inter, 360f, y);
            p.Spawn(p.DashWall, inter, 385f, y);
        }

        GameObject d4 = p.Spawn(p.Diamond, col, 360f, 10.5f);
        SetDiamondId(d4, 4);

        for (int i = 0; i < 7; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 325f + i * 11f, 8f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 420f, 2f);
        GameObject d5 = p.Spawn(p.Diamond, col, 415f, 4f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_12 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 13: Glacier & Saw Foundry (X = 0 to 440)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 13 (Glacier & Saw Foundry)")]
    public static string BuildLevel13()
    {
        string scenePath = BaseDir + "Level_13.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 435f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 22f, 0f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 460f, 38f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 220f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(750f, 6f, 10f));

        for (int x = 0; x <= 440; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Glacier Runway & Crumbling Bridges (X = 0 to 140)
        p.Spawn(p.IceSurface, env, 20f, 0f);
        p.Spawn(p.IceSurface, env, 35f, 0f);

        // Crumbling block bridge over shallow saw pit
        p.Spawn(p.SawSlow, haz, 62f, -1f);
        for (int x = 55; x <= 75; x += 4) p.Spawn(p.CrumblingBlock, inter, x, 1f);

        GameObject d1 = p.Spawn(p.Diamond, col, 65f, 2.5f);
        SetDiamondId(d1, 1);

        // Staggered Vertical Box Pillars (X = 90 to 115)
        for (int y = 0; y <= 3; y++)
        {
            p.Spawn(p.DashWall, inter, 90f, y);
            p.Spawn(p.DashWall, inter, 98f, y);
            p.Spawn(p.DashWall, inter, 106f, y);
            p.Spawn(p.DashWall, inter, 114f, y);
        }

        GameObject d2 = p.Spawn(p.Diamond, col, 102f, 5.5f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 85f + i * 10f, 2f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 135f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Smelting Sub-Floor & Foundry Elevators (X = 140 to 280)
        p.Spawn(p.IceSurface, env, 150f, 0f);
        p.Spawn(p.IceSurface, env, 165f, 0f);

        // Vertical Elevator 1 (Y = -4 to 6)
        GameObject mp1 = p.Spawn(p.MovingPlatform, inter, 185f, 0f);
        ConfigureMovingPlatform(mp1, new Vector3(185f, -4f, 0f), new Vector3(185f, 8f, 0f), 3.5f);

        // Smelting Sub-Floor (Y = -5)
        p.Spawn(p.Ground20, env, 200f, -6f);
        p.Spawn(p.SpinArm, haz, 205f, -3.5f);

        GameObject d3 = p.Spawn(p.Diamond, col, 210f, -4.5f);
        SetDiamondId(d3, 3);

        for (int x = 195; x <= 220; x += 4) p.Spawn(p.CrumblingBlock, inter, x, -2f);

        p.Spawn(p.BouncePadMega, inter, 228f, -5f);

        // Upper Smelting Highway
        p.Spawn(p.OneWayPlatform, env, 240f, 8f);
        p.Spawn(p.OneWayPlatform, env, 252f, 8f);

        GameObject d4 = p.Spawn(p.Diamond, col, 252f, 10f);
        SetDiamondId(d4, 4);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 235f + i * 10f, 9.5f);
            SetAtomId(a, 5 + i);
        }

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 275f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Triple Booster Foundry Speedway (X = 280 to 440)
        p.Spawn(p.IceSurface, env, 290f, 0f);
        p.Spawn(p.IceSurface, env, 305f, 0f);

        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 350f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(65f, 12f, 10f));

        int[] b13X = new int[] { 320, 345, 370 };
        int[] w13X = new int[] { 332, 357, 382 };
        for (int i = 0; i < b13X.Length; i++)
        {
            p.Spawn(p.DashBoostFast, inter, b13X[i], 0.5f);
            for (int x = w13X[i]; x <= w13X[i] + 1; x++)
                for (int y = 0; y <= 3; y++)
                    p.Spawn(p.DashWall, inter, x, y);
        }

        for (int i = 0; i < 7; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 385f + i * 6f, 3f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 425f, 1f);
        GameObject d5 = p.Spawn(p.Diamond, col, 425f, 3.5f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_13 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 14: The Zig-Zag Spire (X = 0 to 420)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 14 (The Zig-Zag Spire)")]
    public static string BuildLevel14()
    {
        string scenePath = BaseDir + "Level_14.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 410f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 215f, 22f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 440f, 42f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 210f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(720f, 6f, 10f));

        for (int x = 0; x <= 420; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: Tier 1 & Tier 2 Switchbacks (X = 0 to 135)
        // Tier 1 Ascent: Chimney 1 (Y = 0 to 7)
        p.Spawn(p.Wall12, env, 28f, 7f);
        p.Spawn(p.Wall12, env, 32f, 7f);
        p.Spawn(p.BouncePadMini, inter, 30f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 30f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        p.Spawn(p.OneWayPlatform, env, 42f, 7f);
        p.Spawn(p.OneWayPlatform, env, 56f, 7f);
        p.Spawn(p.Cannon, haz, 65f, 10f);

        GameObject d1 = p.Spawn(p.Diamond, col, 56f, 8.5f);
        SetDiamondId(d1, 1);

        // Tier 2 Launch: DashLane_UP at X = 75 (Y = 7 to 14)
        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 75f, 7f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(14f, 3f));

        for (int x = 74; x <= 76; x++) p.Spawn(p.DashWall, inter, x, 14f);

        p.Spawn(p.OneWayPlatform, env, 86f, 14f);
        p.Spawn(p.OneWayPlatform, env, 100f, 14f);

        GameObject d2 = p.Spawn(p.Diamond, col, 100f, 15.5f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 80f + i * 10f, 15.5f);
            SetAtomId(a, 1 + i);
        }

        p.Spawn(p.Slope45Left, env, 115f, 6f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 125f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: Tier 3 Apex & The Spire Core Plummet (X = 135 to 260)
        // Chimney 2 to Apex (Y = 0 to 22)
        p.Spawn(p.Wall12, env, 158f, 11f);
        p.Spawn(p.Wall12, env, 162f, 11f);
        p.Spawn(p.Wall12, env, 158f, 17f);
        p.Spawn(p.Wall12, env, 162f, 17f);

        p.Spawn(p.BouncePadMini, inter, 160f, 0f);
        p.Spawn(p.BouncePadMini, inter, 160f, 6f);
        p.Spawn(p.BouncePadMini, inter, 160f, 12f);

        // Apex Catwalk at Y = 22
        p.Spawn(p.OneWayPlatform, env, 175f, 22f);
        p.Spawn(p.OneWayPlatform, env, 195f, 22f);
        p.Spawn(p.OneWayPlatform, env, 215f, 22f);

        GameObject camApex = p.Spawn(p.CameraZoneWide, flow, 195f, 20f);
        SetTriggerBoxSize(camApex, new Vector3(50f, 25f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 210f, 23.5f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 175f + i * 12f, 23.5f);
            SetAtomId(a, 5 + i);
        }

        // THE SPIRE CORE PLUMMET (X = 225 to 231)
        // 5-Deck Ground Pound Drop from Y = 20 to Y = -2!
        for (int x = 226; x <= 230; x++) p.Spawn(p.DashWall, inter, x, 20f);
        p.Spawn(p.CrackedBlocks4x4, inter, 228f, 15f);
        for (int x = 226; x <= 230; x++) p.Spawn(p.DashWall, inter, x, 10f);
        p.Spawn(p.CrackedBlocks4x4, inter, 228f, 5f);
        for (int x = 226; x <= 230; x++) p.Spawn(p.DashWall, inter, x, 1f);

        // Deep Spire Vault at Y = -3
        p.Spawn(p.Ground20, env, 235f, -4f);
        GameObject d4 = p.Spawn(p.Diamond, col, 228f, -2.5f);
        SetDiamondId(d4, 4);

        p.Spawn(p.BouncePadMega, inter, 245f, -3f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 260f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Catacomb Escape & Finale (X = 260 to 420)
        for (int x = 275; x <= 289; x++)
        {
            int h = (x <= 282) ? (x - 275 + 1) : (289 - x + 1);
            for (int y = 0; y <= h; y++) p.Spawn(p.DashWall, inter, x, y);
        }

        p.Spawn(p.DashBoostFast, inter, 310f, 0.5f);
        for (int x = 322; x <= 324; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Canyon Mega Bounce
        p.Spawn(p.BouncePadMega, inter, 345f, 0f);
        p.Spawn(p.BouncePadMega, inter, 370f, 0f);

        for (int i = 0; i < 7; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 340f + i * 8f, 5f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 400f, 1f);
        GameObject d5 = p.Spawn(p.Diamond, col, 400f, 3.5f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_14 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 15: Chaos Citadel Finale (X = 0 to 620)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 15 (Chaos Citadel Finale)")]
    public static string BuildLevel15()
    {
        string scenePath = BaseDir + "Level_15.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 615f, 3f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        p.Spawn(p.TutorialJump, flow, 12f, 0f);
        p.Spawn(p.TutorialDash, flow, 20f, 0f);
        p.Spawn(p.TutorialGroundPound, flow, 55f, 12f);

        GameObject frame = p.Spawn(p.LevelFrame, env, -15f, -1f);
        AdjustLevelFrame(frame, -16f, 640f, 42f, -6f);
        GameObject killPit = p.Spawn(p.KillPit, haz, 310f, -20f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(900f, 6f, 10f));

        for (int x = 0; x <= 620; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Act 1: The Labyrinth Gate (X = 0 to 150)
        p.Spawn(p.Wall12, env, 24f, 7f);
        p.Spawn(p.Wall12, env, 28f, 7f);
        p.Spawn(p.BouncePadMini, inter, 26f, 0f);

        GameObject camClimb1 = p.Spawn(p.CameraZoneClimb, flow, 26f, 8f);
        SetTriggerBoxSize(camClimb1, new Vector3(16f, 20f, 10f));

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 26f, 3f + i * 3f);
            SetAtomId(a, 1 + i);
        }

        // Ramparts at Y = 13
        p.Spawn(p.OneWayPlatform, env, 38f, 13f);
        p.Spawn(p.OneWayPlatform, env, 50f, 13f);

        // Deep 4-Layer Excavation Shaft 1 (X = 60 to 66)
        for (int x = 61; x <= 65; x++) p.Spawn(p.DashWall, inter, x, 12f);
        p.Spawn(p.CrackedBlocks4x4, inter, 63f, 8f);
        for (int x = 61; x <= 65; x++) p.Spawn(p.DashWall, inter, x, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 63f, 1f);

        // Subterranean Crypt at Y = -5
        p.Spawn(p.Ground20, env, 70f, -6f);
        GameObject d1 = p.Spawn(p.Diamond, col, 63f, -4.5f);
        SetDiamondId(d1, 1);
        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 58f + i * 5f, -4.5f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 80f, -5f);

        // Tight Dash Tunnel
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 110f, 2f);
        SetTriggerBoxSize(camTight1, new Vector3(45f, 12f, 10f));

        p.Spawn(p.DashBoostFast, inter, 100f, 0.5f);
        for (int x = 112; x <= 114; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 145f, 2f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Pinball Spire & Diamond Vault (X = 150 to 300)
        p.Spawn(p.Slope45Large, env, 160f, 4f);
        p.Spawn(p.BouncePadMega, inter, 180f, 0f);

        // Spire Rocket Launch
        p.Spawn(p.Wall12, env, 198f, 8f);
        p.Spawn(p.Wall12, env, 202f, 8f);

        GameObject laneUp1 = p.Spawn(p.DashLaneUp, inter, 200f, 6f);
        laneUp1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(laneUp1, new Vector2(16f, 3f));

        for (int x = 199; x <= 201; x++) p.Spawn(p.DashWall, inter, x, 14f);

        // Hollow Box Diamond Vault at X = 220, Y = 16
        for (int x = 218; x <= 222; x++)
        {
            p.Spawn(p.DashWall, inter, x, 14f);
            p.Spawn(p.DashWall, inter, x, 18f);
        }
        p.Spawn(p.DashWall, inter, 218f, 15f);
        p.Spawn(p.DashWall, inter, 218f, 16f);
        p.Spawn(p.DashWall, inter, 218f, 17f);
        p.Spawn(p.DashWall, inter, 222f, 15f);
        p.Spawn(p.DashWall, inter, 222f, 16f);
        p.Spawn(p.DashWall, inter, 222f, 17f);

        GameObject d2 = p.Spawn(p.Diamond, col, 220f, 16f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 210f + i * 8f, 19f);
            SetAtomId(a, 7 + i);
        }

        p.Spawn(p.Slope45Left, env, 275f, 8f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 295f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Foundry Crucible & Magma Trench (X = 300 to 450)
        p.Spawn(p.IceSurface, env, 310f, 0f);
        p.Spawn(p.IceSurface, env, 325f, 0f);

        p.Spawn(p.SpinArm, haz, 340f, 4f);
        p.Spawn(p.FireJet, haz, 355f, 0f);

        // 4-Layer Excavation Drop to Magma Trench (X = 370 to 376)
        for (int x = 371; x <= 375; x++) p.Spawn(p.DashWall, inter, x, 12f);
        p.Spawn(p.CrackedBlocks4x4, inter, 373f, 8f);
        for (int x = 371; x <= 375; x++) p.Spawn(p.DashWall, inter, x, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 373f, 1f);

        p.Spawn(p.Ground20, env, 380f, -6f);
        p.Spawn(p.LavaLeaper, haz, 375f, -5f);

        GameObject d3 = p.Spawn(p.Diamond, col, 373f, -4.5f);
        SetDiamondId(d3, 3);
        GameObject d4 = p.Spawn(p.Diamond, col, 385f, -4.5f);
        SetDiamondId(d4, 4);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 365f + i * 6f, -4.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 395f, -5f);

        // Chained Boosters
        p.Spawn(p.DashBoostFast, inter, 410f, 0.5f);
        for (int x = 422; x <= 424; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 445f, 2f);
        cp3.name = "Checkpoint_3";

        // Act 4: Citadel Apex & Grand Triumphal Run (X = 450 to 620)
        // 18m Grand Rocket Tower (Chimney 3)
        p.Spawn(p.Wall12, env, 472f, 9f);
        p.Spawn(p.Wall12, env, 476f, 9f);

        GameObject grandLane = p.Spawn(p.DashLaneUp, inter, 474f, 7f);
        grandLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(grandLane, new Vector2(18f, 3f));

        for (int x = 473; x <= 475; x++) p.Spawn(p.DashWall, inter, x, 16f);

        GameObject camCitadel = p.Spawn(p.CameraZoneWide, flow, 490f, 18f);
        SetTriggerBoxSize(camCitadel, new Vector3(50f, 30f, 10f));

        for (int x = 488; x <= 514; x += 12) p.Spawn(p.OneWayPlatform, env, x, 17f);

        for (int i = 0; i < 2; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 495f + i * 15f, 18.5f);
            SetAtomId(a, 15 + i);
        }

        // Quad Mega Bounce Canyon
        GameObject camCanyon = p.Spawn(p.CameraZoneWide, flow, 555f, 6f);
        SetTriggerBoxSize(camCanyon, new Vector3(70f, 26f, 10f));

        p.Spawn(p.BouncePadMega, inter, 525f, 0f);
        p.Spawn(p.BouncePadMega, inter, 545f, 0f);
        p.Spawn(p.BouncePadMega, inter, 565f, 0f);
        p.Spawn(p.BouncePadMega, inter, 585f, 0f);

        GameObject a17 = p.Spawn(p.Atom, col, 535f, 8f);
        SetAtomId(a17, 17);
        GameObject a18 = p.Spawn(p.Atom, col, 555f, 8f);
        SetAtomId(a18, 18);
        GameObject a19 = p.Spawn(p.Atom, col, 575f, 8f);
        SetAtomId(a19, 19);
        GameObject a20 = p.Spawn(p.Atom, col, 590f, 8f);
        SetAtomId(a20, 20);

        // Grand Finale Archway Smash (3-Layer Archway)
        p.Spawn(p.DashBoostFast, inter, 595f, 0.5f);
        for (int x = 605; x <= 607; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, inter, x, y);

        p.Spawn(p.Ground20, env, 615f, 2f);
        GameObject d5 = p.Spawn(p.Diamond, col, 610f, 5f);
        SetDiamondId(d5, 5);

        FinalizeScene(scene);
        return $"Level_15 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 16: Cobalt Caverns & The Blue Highway (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 16 (Cobalt Caverns & The Blue Highway)")]
    public static string BuildLevel16()
    {
        string scenePath = BaseDir + "Level_16.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        // Flow: Portals & Signs
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        // Frame
        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 24f, -8f);

        // Act 1: The Blue Carpet (X = 0 to 55)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.TutorialJump, flow, 6f, 0f);
        p.Spawn(p.Slope45Left, env, 12f, 0f);
        p.Spawn(p.Ledge8, env, 16f, 3f);
        p.Spawn(p.Slope45, env, 22f, 0f);

        // Dense 2-layer carpet of TouchWallBlue on the slope and runway
        p.SpawnTouchWallBlock(inter, 14f, 28f, 1f, 2f);

        p.Spawn(p.TutorialDash, flow, 32f, 0f);
        // Yellow DashWall barricade 2-thick (requires Dash)
        for (int x = 35; x <= 36; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 20f + i * 8f, 4f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: Deep Cobalt Drop (X = 55 to 115)
        for (int x = 60; x <= 100; x += 20) p.Spawn(p.Ground20, env, x, -6f);

        p.Spawn(p.Wall12, env, 66f, 2f);
        p.Spawn(p.Wall12, env, 74f, 2f);

        // Drop layers: Blue touch cushion, Yellow cracked blocks, Blue touch cushion
        p.SpawnTouchWallBlock(inter, 67f, 73f, 4f, 4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 70f, 1f);
        p.SpawnTouchWallBlock(inter, 67f, 73f, -2f, -1f);

        GameObject camCavern = p.Spawn(p.CameraZoneWide, flow, 75f, -2f);
        SetTriggerBoxSize(camCavern, new Vector3(40f, 20f, 10f));

        GameObject d1 = p.Spawn(p.Diamond, col, 78f, -4.5f);
        SetDiamondId(d1, 1);

        p.Spawn(p.LavaLeaper, haz, 84f, -6f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 72f + i * 8f, -4.5f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 92f, -5f);
        p.Spawn(p.Ledge8, env, 104f, 1f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 110f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: Twin-Wall Chimney & Sky Balcony (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.Wall12, env, 123f, 6f);
        p.Spawn(p.Wall12, env, 127f, 6f);

        // Sealed blue touch roof: head-butting shatters it
        p.SpawnTouchWallRun(inter, 124f, 126f, 12f);

        for (int x = 132; x <= 156; x += 8) p.Spawn(p.OneWayPlatform, env, x, 11f);

        p.SpawnTouchWallBlock(inter, 143f, 147f, 12f, 14f);
        GameObject d2 = p.Spawn(p.Diamond, col, 145f, 13f);
        SetDiamondId(d2, 2);

        p.Spawn(p.StompBlock, haz, 142f, 4f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 135f + i * 10f, 12.5f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.Slope45, env, 162f, 5f);
        p.Spawn(p.Slope45, env, 166f, 1f);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: Supersonic Blue Tunnel & Victory Arch (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 178f, 0.5f);

        for (int x = 186; x <= 210; x += 8) p.Spawn(p.Ledge8, env, x, 3f);
        p.SpawnTouchWallBlock(inter, 186f, 205f, 1f, 2f);

        for (int y = 0; y <= 2; y++)
        {
            p.Spawn(p.DashWall, inter, 195f, y);
            p.Spawn(p.DashWall, inter, 208f, y);
        }

        GameObject camTunnel = p.Spawn(p.CameraZoneTight, flow, 195f, 1f);
        SetTriggerBoxSize(camTunnel, new Vector3(30f, 8f, 10f));

        GameObject camVictory = p.Spawn(p.CameraZoneWide, flow, 225f, 4f);
        SetTriggerBoxSize(camVictory, new Vector3(40f, 20f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 222f, 3f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 2.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, 1f);

        FinalizeScene(scene);
        return $"Level_16 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 17: Azure Abyss & Magma Trenches (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 17 (Azure Abyss & Magma Trenches)")]
    public static string BuildLevel17()
    {
        string scenePath = BaseDir + "Level_17.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 8f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 4f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 26f, -8f);

        // Act 1: The Trench Descent (X = 0 to 55)
        p.Spawn(p.Ground20, env, 10f, 7f);
        p.SpawnTouchWallRun(inter, 15f, 18f, 8f);

        p.Spawn(p.Ground20, env, 28f, 3f);
        p.Spawn(p.FireJet, haz, 25f, 4f);
        p.SpawnTouchWallRun(inter, 32f, 35f, 4f);

        p.Spawn(p.Ground20, env, 45f, -1f);
        p.Spawn(p.FireJet, haz, 42f, 0f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 12f + i * 14f, 5f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: Azure Pinball Ascent (X = 55 to 115)
        p.Spawn(p.Ground20, env, 70f, -3f);
        p.Spawn(p.BouncePad, inter, 62f, -2f);

        // Floating TouchWallBlue targets
        p.Spawn(p.TouchWallBlue, inter, 65f, 3f);
        p.Spawn(p.TouchWallBlue, inter, 67f, 5f);
        p.Spawn(p.TouchWallBlue, inter, 69f, 7f);
        p.Spawn(p.TouchWallBlue, inter, 71f, 9f);

        // Upper catwalk at Y = 10
        for (int x = 80; x <= 100; x += 8) p.Spawn(p.Ledge8, env, x, 10f);

        p.Spawn(p.SpinArm, haz, 90f, 12f);

        // Yellow DashWall alcove hiding Diamond 1
        for (int y = 10; y <= 12; y++) p.Spawn(p.DashWall, inter, 81f, y);
        GameObject d1 = p.Spawn(p.Diamond, col, 83f, 11f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 78f + i * 8f, 11.5f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 105f, 6f);
        p.Spawn(p.OneWayPlatform, env, 107f, 2f);

        p.Spawn(p.Ground20, env, 110f, -1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 110f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Vault of Azure (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Yellow DashWall outer fortress (X = 135..145, Y = 0..5)
        for (int y = 0; y <= 4; y++)
        {
            p.Spawn(p.DashWall, inter, 135f, y);
            p.Spawn(p.DashWall, inter, 145f, y);
        }
        for (int x = 136; x <= 144; x++) p.Spawn(p.DashWall, inter, x, 5f);

        // Solid core of TouchWallBlue enclosing Diamond 2
        p.SpawnTouchWallBlock(inter, 138f, 142f, 1f, 3f);
        GameObject d2 = p.Spawn(p.Diamond, col, 140f, 2f);
        SetDiamondId(d2, 2);

        p.Spawn(p.Cannon, haz, 158f, 3f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 150f + i * 8f, 1.5f);
            SetAtomId(a, 8 + i);
        }

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: Rocket Spire & Magma Trench (X = 175 to 240)
        p.Spawn(p.Ground20, env, 185f, -5f);
        p.Spawn(p.LavaLeaper, haz, 188f, -5f);

        for (int x = 180; x <= 196; x += 8) p.Spawn(p.OneWayPlatform, env, x, 3f);

        // Rocket Spire
        p.Spawn(p.Wall12, env, 203f, 7f);
        p.Spawn(p.Wall12, env, 207f, 7f);

        GameObject rocketLane = p.Spawn(p.DashLaneUp, inter, 205f, 6f);
        rocketLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(rocketLane, new Vector2(16f, 3f));

        // Stacked TouchWallBlue decks inside chimney
        p.SpawnTouchWallRun(inter, 204f, 206f, 5f);
        p.SpawnTouchWallRun(inter, 204f, 206f, 10f);
        p.SpawnTouchWallRun(inter, 204f, 206f, 15f);

        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 215f, 12f);
        SetTriggerBoxSize(camWide, new Vector3(50f, 24f, 10f));

        p.Spawn(p.Ground20, env, 225f, 3f);
        p.Spawn(p.Ground20, env, 235f, 3f);

        GameObject d3 = p.Spawn(p.Diamond, col, 225f, 5f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 5f);
            SetAtomId(a, 11 + i);
        }

        FinalizeScene(scene);
        return $"Level_17 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 18: The Mosaic Matrix (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 18 (The Mosaic Matrix)")]
    public static string BuildLevel18()
    {
        string scenePath = BaseDir + "Level_18.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 0f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 24f, -8f);

        // Act 1: The Dual-Color Slalom (X = 0 to 55)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Alternating columns: Blue Touch vs Yellow DashWall
        for (int y = 0; y <= 2; y++) p.Spawn(p.TouchWallBlue, inter, 16f, y);
        for (int y = 0; y <= 2; y++) p.Spawn(p.DashWall, inter, 26f, y);
        for (int y = 0; y <= 2; y++) p.Spawn(p.TouchWallBlue, inter, 36f, y);
        for (int y = 0; y <= 2; y++) p.Spawn(p.DashWall, inter, 46f, y);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 21f + i * 10f, 2f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Mosaic Shaft (X = 55 to 115)
        for (int x = 60; x <= 100; x += 20) p.Spawn(p.Ground20, env, x, -6f);

        p.Spawn(p.Wall12, env, 65f, 1f);
        p.Spawn(p.Wall12, env, 75f, 1f);

        // Checkerboard decks in shaft
        // Deck 1 (Y = 5)
        p.Spawn(p.TouchWallBlue, inter, 67f, 5f);
        p.Spawn(p.DashWall, inter, 69f, 5f);
        p.Spawn(p.TouchWallBlue, inter, 71f, 5f);
        p.Spawn(p.DashWall, inter, 73f, 5f);

        // Deck 2 (Y = 2)
        p.Spawn(p.DashWall, inter, 67f, 2f);
        p.Spawn(p.TouchWallBlue, inter, 69f, 2f);
        p.Spawn(p.DashWall, inter, 71f, 2f);
        p.Spawn(p.TouchWallBlue, inter, 73f, 2f);

        // Deck 3 (Y = -1)
        p.Spawn(p.TouchWallBlue, inter, 67f, -1f);
        p.Spawn(p.DashWall, inter, 69f, -1f);
        p.Spawn(p.TouchWallBlue, inter, 71f, -1f);
        p.Spawn(p.DashWall, inter, 73f, -1f);

        p.Spawn(p.StompBlock, haz, 85f, -1f);
        GameObject d1 = p.Spawn(p.Diamond, col, 92f, -4.5f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 78f + i * 6f, -4.5f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 102f, -5f);
        p.Spawn(p.Ledge8, env, 108f, 0f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 110f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: Elevator & Saw Mill (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        GameObject elevator = p.Spawn(p.MovingPlatform, env, 122f, 0f);
        ConfigureMovingPlatform(elevator, new Vector3(122f, 0f, 0f), new Vector3(122f, 10f, 0f), 4f);

        p.Spawn(p.SawSlow, haz, 132f, 5f);

        p.Spawn(p.Slope45, env, 140f, 4f);
        p.Spawn(p.Slope45, env, 146f, 8f);
        p.SpawnTouchWallBlock(inter, 141f, 153f, 5f, 10f);

        GameObject d2 = p.Spawn(p.Diamond, col, 152f, 12f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 138f + i * 8f, 7f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 162f, 6f);
        p.Spawn(p.OneWayPlatform, env, 165f, 2f);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: Supersonic Mosaic Expressway (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 178f, 0.5f);
        p.Spawn(p.DashBoostFast, inter, 192f, 0.5f);

        // Wave-like TouchWallBlue arches
        p.SpawnTouchWallBlock(inter, 185f, 208f, 2f, 3f);

        // 3-layer yellow fortress breach
        for (int x = 214; x <= 216; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject camExp = p.Spawn(p.CameraZoneWide, flow, 225f, 3f);
        SetTriggerBoxSize(camExp, new Vector3(40f, 16f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 222f, 2f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 1.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, -1f);

        FinalizeScene(scene);
        return $"Level_18 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 19: The Sky Foundry (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 19 (The Sky Foundry)")]
    public static string BuildLevel19()
    {
        string scenePath = BaseDir + "Level_19.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 4f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 26f, -8f);

        // Act 1: The Ice Slide & Cloud Breach (X = 0 to 55)
        p.Spawn(p.Ground20, env, 10f, 3f);
        for (int x = 12; x <= 32; x += 8) p.Spawn(p.IceSurface, env, x, 4f);

        // Blue touch cloud
        p.SpawnTouchWallBlock(inter, 20f, 28f, 4f, 5f);

        // Yellow DashWall barricade
        for (int y = 4; y <= 6; y++) p.Spawn(p.DashWall, inter, 36f, y);

        p.Spawn(p.Slope45, env, 42f, 0f);
        p.Spawn(p.Ground20, env, 50f, -1f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 16f + i * 10f, 5.5f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: Crumbling Bridge & Safety Pit (X = 55 to 115)
        for (int x = 60; x <= 100; x += 20) p.Spawn(p.Ground20, env, x, -6f);

        // Crumbling block bridge at Y = 6
        for (int x = 66; x <= 88; x += 3) p.Spawn(p.CrumblingBlock, env, x, 6f);

        // Blue cushion bed in the pit below
        p.SpawnTouchWallBlock(inter, 70f, 84f, -3f, -2f);

        GameObject d1 = p.Spawn(p.Diamond, col, 76f, -4.5f);
        SetDiamondId(d1, 1);

        p.Spawn(p.BouncePadMega, inter, 94f, -5f);
        p.Spawn(p.Cannon, haz, 102f, 4f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 70f + i * 8f, 7.5f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.Ledge8, env, 108f, 1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 110f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Turbine Spire (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.Wall12, env, 124f, 6f);
        p.Spawn(p.Wall12, env, 128f, 6f);

        GameObject spireLane = p.Spawn(p.DashLaneUp, inter, 126f, 6f);
        spireLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(spireLane, new Vector2(18f, 3f));

        // Horizontal rings of TouchWallBlue
        p.SpawnTouchWallRun(inter, 125f, 127f, 6f);
        p.SpawnTouchWallRun(inter, 125f, 127f, 11f);
        p.SpawnTouchWallRun(inter, 125f, 127f, 16f);

        // Summit catwalk at Y = 18
        for (int x = 132; x <= 156; x += 8) p.Spawn(p.OneWayPlatform, env, x, 18f);

        p.Spawn(p.SpinArm, haz, 142f, 20f);
        GameObject d2 = p.Spawn(p.Diamond, col, 150f, 19.5f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 136f + i * 8f, 19.5f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 162f, 12f);
        p.Spawn(p.OneWayPlatform, env, 165f, 6f);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: Parabolic Blue Skywalk (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.BouncePad, inter, 185f, 0f);
        p.Spawn(p.BouncePad, inter, 200f, 0f);
        p.Spawn(p.BouncePad, inter, 215f, 0f);

        // Floating diamond formations of TouchWallBlue
        p.SpawnTouchWallBlock(inter, 191f, 193f, 6f, 8f);
        p.SpawnTouchWallBlock(inter, 206f, 208f, 8f, 10f);
        p.SpawnTouchWallBlock(inter, 221f, 223f, 6f, 8f);

        GameObject d3 = p.Spawn(p.Diamond, col, 207f, 11f);
        SetDiamondId(d3, 3);

        GameObject camSky = p.Spawn(p.CameraZoneWide, flow, 225f, 4f);
        SetTriggerBoxSize(camSky, new Vector3(40f, 20f, 10f));

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 190f + i * 10f, 8f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, 1f);

        FinalizeScene(scene);
        return $"Level_19 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 20: The Dual-Chroma Citadel Finale (X = 0 to 250)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 20 (The Dual-Chroma Citadel Finale)")]
    public static string BuildLevel20()
    {
        string scenePath = BaseDir + "Level_20.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 245f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 255f, 28f, -10f);

        // Act 1: The Sonic Entry (X = 0 to 58)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 8f, 0.5f);
        p.Spawn(p.DashBoostFast, inter, 20f, 0.5f);
        p.Spawn(p.DashBoostFast, inter, 32f, 0.5f);

        // Supersonic tunnel with dense TouchWallBlue & Yellow security gates
        p.SpawnTouchWallBlock(inter, 35f, 52f, 1f, 2f);
        for (int y = 0; y <= 2; y++)
        {
            p.Spawn(p.DashWall, inter, 40f, y);
            p.Spawn(p.DashWall, inter, 48f, y);
        }

        GameObject camSonic = p.Spawn(p.CameraZoneTight, flow, 42f, 1f);
        SetTriggerBoxSize(camSonic, new Vector3(25f, 8f, 10f));

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 15f + i * 10f, 2f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 55f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Sunken Crypt (X = 58 to 120)
        for (int x = 65; x <= 105; x += 20) p.Spawn(p.Ground20, env, x, -7f);

        p.Spawn(p.Wall12, env, 68f, 2f);
        p.Spawn(p.Wall12, env, 78f, 2f);

        // 4-layer excavation drop
        p.SpawnTouchWallBlock(inter, 69f, 77f, 6f, 6f);
        p.Spawn(p.CrackedBlocks4x4, inter, 73f, 2f);
        p.SpawnTouchWallBlock(inter, 69f, 77f, -1f, -1f);
        for (int x = 71; x <= 75; x++) p.Spawn(p.DashWall, inter, x, -4f);

        p.Spawn(p.LavaLeaper, haz, 82f, -7f);

        GameObject cryptElevator = p.Spawn(p.MovingPlatform, env, 88f, -6f);
        ConfigureMovingPlatform(cryptElevator, new Vector3(88f, -6f, 0f), new Vector3(88f, 0f, 0f), 3.5f);

        GameObject d1 = p.Spawn(p.Diamond, col, 80f, -5.5f);
        SetDiamondId(d1, 1);

        p.Spawn(p.BouncePadMega, inter, 98f, -6f);
        p.Spawn(p.StompBlock, haz, 108f, 5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 75f + i * 8f, -5.5f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.Ground20, env, 115f, 1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 118f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: Pinball Spire & Twin Chimneys (X = 120 to 180)
        for (int x = 125; x <= 165; x += 20) p.Spawn(p.Ground20, env, x, 1f);

        p.Spawn(p.Wall12, env, 128f, 8f);
        p.Spawn(p.Wall12, env, 132f, 8f);
        p.SpawnTouchWallRun(inter, 129f, 131f, 14f);

        p.Spawn(p.BouncePad, inter, 140f, 10f);
        p.SpawnTouchWallBlock(inter, 141f, 144f, 13f, 15f);

        // Sky bridge at Y = 16
        for (int x = 148; x <= 168; x += 8) p.Spawn(p.Ledge8, env, x, 16f);

        p.Spawn(p.SawSlow, haz, 158f, 18f);
        GameObject d2 = p.Spawn(p.Diamond, col, 162f, 18f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 146f + i * 6f, 17.5f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 172f, 11f);
        p.Spawn(p.OneWayPlatform, env, 175f, 6f);

        p.Spawn(p.Ground20, env, 180f, 1f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 178f, 2f);
        cp3.name = "Checkpoint_3";

        // Act 4: The Grand Chromatic Gauntlet (X = 180 to 250)
        for (int x = 190; x <= 230; x += 20) p.Spawn(p.Ground20, env, x, 1f);

        p.Spawn(p.DashBoostFast, inter, 185f, 2.5f);
        p.Spawn(p.DashBoostFast, inter, 196f, 2.5f);

        // 3 alternating blue and yellow security walls
        for (int y = 2; y <= 5; y++) p.Spawn(p.TouchWallBlue, inter, 190f, y);
        for (int y = 2; y <= 5; y++) p.Spawn(p.DashWall, inter, 200f, y);
        for (int y = 2; y <= 5; y++) p.Spawn(p.TouchWallBlue, inter, 206f, y);

        // Grand 18m rocket tower
        p.Spawn(p.Wall12, env, 210f, 10f);
        p.Spawn(p.Wall12, env, 214f, 10f);

        GameObject finalRocket = p.Spawn(p.DashLaneUp, inter, 212f, 8f);
        finalRocket.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(finalRocket, new Vector2(20f, 3f));

        p.SpawnTouchWallRun(inter, 211f, 213f, 5f);
        p.SpawnTouchWallRun(inter, 211f, 213f, 9f);
        p.SpawnTouchWallRun(inter, 211f, 213f, 13f);
        p.SpawnTouchWallRun(inter, 211f, 213f, 17f);

        GameObject camCitadel = p.Spawn(p.CameraZoneWide, flow, 225f, 14f);
        SetTriggerBoxSize(camCitadel, new Vector3(60f, 30f, 10f));

        // 4-layer Grand Victory Archway
        for (int y = 2; y <= 6; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 236f, y);
            p.Spawn(p.DashWall, inter, 237f, y);
            p.Spawn(p.TouchWallBlue, inter, 238f, y);
            p.Spawn(p.DashWall, inter, 239f, y);
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 238f, 4f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 228f + i * 4f, 4.5f);
            SetAtomId(a, 13 + i);
        }

        p.Spawn(p.Ground20, env, 245f, 1f);

        FinalizeScene(scene);
        return $"Level_20 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 21: Blue Avalanche Descent (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 21 (Blue Avalanche Descent)")]
    public static string BuildLevel21()
    {
        string scenePath = BaseDir + "Level_21.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 16f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 0f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 28f, -10f);

        // Act 1: The Mountain Summit & First Cascade (X = 0 to 55)
        p.Spawn(p.Ground20, env, 6f, 15f);

        // Giant Slope_45_Large descent from Y = 15 down to Y = 3
        p.Spawn(p.Slope45Large, env, 18f, 3f);

        // Cascading avalanche of TouchWallBlue coating the diagonal slide
        for (int i = 0; i < 10; i++)
        {
            float x = 14f + i;
            float y = 14f - i;
            p.Spawn(p.TouchWallBlue, inter, x, y);
            p.Spawn(p.TouchWallBlue, inter, x, y + 1f);
        }

        // Landing terrace at Y = 2
        p.Spawn(p.Ground20, env, 32f, 1f);

        // Structural CrackedBlocks_4x4 plug requiring Ground Pound down to Y = -2
        p.Spawn(p.CrackedBlocks4x4, inter, 32f, 2f);
        p.Spawn(p.OneWayPlatform, env, 38f, 5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 16f + i * 8f, 12f - i * 2.5f);
            SetAtomId(a, 1 + i);
        }

        p.Spawn(p.Ground20, env, 48f, -3f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 50f, -2f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Cascading Terraces (X = 55 to 115)
        // Terrace 1: Y = 4
        p.Spawn(p.Ground20, env, 65f, 3f);
        p.SpawnTouchWallBlock(inter, 60f, 68f, 4f, 5f);

        // Terrace 2: Y = 0
        p.Spawn(p.Ground20, env, 80f, -1f);
        for (int x = 78; x <= 79; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn(p.DashWall, inter, x, y);

        // Terrace 3 & Subterranean Gully: Y = -6
        for (int x = 90; x <= 110; x += 20) p.Spawn(p.Ground20, env, x, -7f);

        p.SpawnTouchWallBlock(inter, 94f, 98f, -4f, -3f);
        GameObject d1 = p.Spawn(p.Diamond, col, 100f, -4.5f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 92f + i * 6f, -5.5f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 106f, -5f);
        p.Spawn(p.Ground20, env, 115f, -1f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 112f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: Avalanche Gulch & Ice Flume (X = 115 to 175)
        p.Spawn(p.Ground20, env, 135f, -1f);
        p.Spawn(p.Ground20, env, 155f, -1f);

        for (int x = 120; x <= 144; x += 8) p.Spawn(p.IceSurface, env, x, 0f);

        p.Spawn(p.SawSlow, haz, 132f, 2f);

        // Dense wave of TouchWallBlue
        p.SpawnTouchWallBlock(inter, 135f, 148f, 1f, 2f);

        // Yellow DashWall stopper
        for (int y = 0; y <= 3; y++) p.Spawn(p.DashWall, inter, 150f, y);

        p.Spawn(p.BouncePadMini, inter, 158f, 0f);
        GameObject d2 = p.Spawn(p.Diamond, col, 158f, 4f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 140f + i * 8f, 2.5f);
            SetAtomId(a, 8 + i);
        }

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: The Canyon Basin & Supersonic Smash (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -3f);

        p.Spawn(p.Slope45, env, 176f, -2f);
        p.Spawn(p.DashBoostFast, inter, 182f, -1.5f);

        // Cascading blue touch walls and yellow gate posts
        p.SpawnTouchWallBlock(inter, 190f, 210f, -1f, 0f);
        for (int y = -2; y <= 1; y++)
        {
            p.Spawn(p.DashWall, inter, 200f, y);
            p.Spawn(p.DashWall, inter, 215f, y);
        }

        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 215f, 2f);
        SetTriggerBoxSize(camWide, new Vector3(50f, 24f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 224f, 0f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 0.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, -1f);

        FinalizeScene(scene);
        return $"Level_21 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 22: Twin-Chimney Sky-Vaults (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 22 (Twin-Chimney Sky-Vaults)")]
    public static string BuildLevel22()
    {
        string scenePath = BaseDir + "Level_22.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 0f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 28f, -8f);

        // Act 1: Chimney Alpha & Sky-Vault 1 (X = 0 to 55)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        // Chimney Alpha
        p.Spawn(p.Wall12, env, 18f, 6f);
        p.Spawn(p.Wall12, env, 22f, 6f);

        // Sky-Vault 1 floor at Y = 12
        for (int x = 28; x <= 44; x += 8) p.Spawn(p.OneWayPlatform, env, x, 12f);

        // Checkerboard cube in Sky-Vault 1
        for (int x = 30; x <= 36; x += 2)
        {
            p.Spawn(p.TouchWallBlue, inter, x, 13f);
            p.Spawn(p.DashWall, inter, x + 1f, 13f);
            p.Spawn(p.DashWall, inter, x, 14f);
            p.Spawn(p.TouchWallBlue, inter, x + 1f, 14f);
        }

        p.Spawn(p.FireJet, haz, 40f, 11.5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 26f + i * 6f, 14f);
            SetAtomId(a, 1 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 48f, 6f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Rocket Spire & Sky-Vault 2 (X = 55 to 115)
        for (int x = 60; x <= 100; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.Wall12, env, 66f, 8f);
        p.Spawn(p.Wall12, env, 70f, 8f);

        GameObject rocketLane = p.Spawn(p.DashLaneUp, inter, 68f, 6f);
        rocketLane.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(rocketLane, new Vector2(18f, 3f));

        p.SpawnTouchWallRun(inter, 67f, 69f, 5f);
        p.SpawnTouchWallRun(inter, 67f, 69f, 10f);
        p.SpawnTouchWallRun(inter, 67f, 69f, 15f);

        // Sky-Vault 2 at Y = 16
        for (int x = 76; x <= 100; x += 8) p.Spawn(p.Ledge8, env, x, 16f);

        p.Spawn(p.Cannon, haz, 100f, 18f);

        // Yellow DashWall vault enclosing Diamond 1
        for (int y = 16; y <= 19; y++)
        {
            p.Spawn(p.DashWall, inter, 85f, y);
            p.Spawn(p.DashWall, inter, 91f, y);
        }
        for (int x = 86; x <= 90; x++) p.Spawn(p.DashWall, inter, x, 19f);

        GameObject d1 = p.Spawn(p.Diamond, col, 88f, 17f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 78f + i * 8f, 17.5f);
            SetAtomId(a, 5 + i);
        }

        p.SpawnTouchWallRun(inter, 104f, 106f, 11f);
        p.SpawnTouchWallRun(inter, 104f, 106f, 5f);

        p.Spawn(p.Ground20, env, 110f, -1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 112f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: Chimney Beta & The Projectile Gauntlet (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.Wall12, env, 124f, 6f);
        p.Spawn(p.Wall12, env, 128f, 6f);
        p.SpawnTouchWallRun(inter, 125f, 127f, 12f);

        for (int x = 132; x <= 156; x += 8) p.Spawn(p.OneWayPlatform, env, x, 12f);

        p.Spawn(p.Cannon, haz, 155f, 14f);

        p.SpawnTouchWallBlock(inter, 142f, 148f, 13f, 14f);
        GameObject d2 = p.Spawn(p.Diamond, col, 145f, 14f);
        SetDiamondId(d2, 2);

        GameObject camClimb = p.Spawn(p.CameraZoneClimb, flow, 135f, 10f);
        SetTriggerBoxSize(camClimb, new Vector3(30f, 20f, 10f));

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 136f + i * 8f, 13.5f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.Slope45, env, 162f, 5f);
        p.Spawn(p.Slope45, env, 166f, 1f);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: High Skyway & Supersonic Breaker (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 178f, 0.5f);
        p.Spawn(p.DashBoostFast, inter, 190f, 0.5f);

        for (int y = 0; y <= 3; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 195f, y);
            p.Spawn(p.DashWall, inter, 205f, y);
            p.Spawn(p.TouchWallBlue, inter, 215f, y);
        }

        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 220f, 4f);
        SetTriggerBoxSize(camWide, new Vector3(40f, 20f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 222f, 2f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 1.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, -1f);

        FinalizeScene(scene);
        return $"Level_22 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 23: Ricochet Pinball Foundry (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 23 (Ricochet Pinball Foundry)")]
    public static string BuildLevel23()
    {
        string scenePath = BaseDir + "Level_23.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 0f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 26f, -8f);

        // Act 1: The Pinball Foyer (X = 0 to 55)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.BouncePad, inter, 14f, 0f);
        p.Spawn(p.BouncePadMini, inter, 22f, 5f);
        p.SpawnTouchWallBlock(inter, 24f, 32f, 6f, 8f);

        p.Spawn(p.Ledge8, env, 36f, 4f);
        for (int y = 4; y <= 6; y++)
        {
            p.Spawn(p.DashWall, inter, 40f, y);
            p.Spawn(p.DashWall, inter, 41f, y);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 18f + i * 8f, 7f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Moving Foundry Shuttles (X = 55 to 115)
        for (int x = 60; x <= 100; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        GameObject shuttleH = p.Spawn(p.MovingPlatform, env, 65f, 4f);
        ConfigureMovingPlatform(shuttleH, new Vector3(65f, 4f, 0f), new Vector3(80f, 4f, 0f), 3.5f);

        p.Spawn(p.SpinArm, haz, 75f, 8f);

        GameObject shuttleV = p.Spawn(p.MovingPlatform, env, 88f, 2f);
        ConfigureMovingPlatform(shuttleV, new Vector3(88f, 2f, 0f), new Vector3(88f, 10f, 0f), 4f);

        // Yellow DashWall vault floating at Y = 8..11 enclosing Diamond 1
        for (int y = 8; y <= 11; y++)
        {
            p.Spawn(p.DashWall, inter, 94f, y);
            p.Spawn(p.DashWall, inter, 98f, y);
        }
        for (int x = 95; x <= 97; x++) p.Spawn(p.DashWall, inter, x, 11f);

        GameObject d1 = p.Spawn(p.Diamond, col, 96f, 9.5f);
        SetDiamondId(d1, 1);

        p.Spawn(p.SawSlow, haz, 104f, 4f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 70f + i * 12f, 6f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.Ground20, env, 110f, -1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 112f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: Ricochet Canyon & Mega Bounce Chains (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.BouncePadMega, inter, 124f, 0f);
        p.Spawn(p.BouncePadMega, inter, 140f, 0f);
        p.Spawn(p.BouncePadMega, inter, 156f, 0f);

        p.SpawnTouchWallBlock(inter, 122f, 126f, 8f, 10f);
        p.SpawnTouchWallBlock(inter, 138f, 142f, 8f, 10f);
        p.SpawnTouchWallBlock(inter, 154f, 158f, 8f, 10f);

        GameObject d2 = p.Spawn(p.Diamond, col, 148f, 11f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 130f + i * 15f, 9f);
            SetAtomId(a, 8 + i);
        }

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: Supersonic Foundry Blast (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 178f, 0.5f);
        p.Spawn(p.DashBoostFast, inter, 192f, 0.5f);

        p.SpawnTouchWallBlock(inter, 185f, 208f, 1f, 3f);

        for (int x = 211; x <= 213; x++)
            for (int y = 0; y <= 3; y++)
                p.Spawn(p.DashWall, inter, x, y);

        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 220f, 3f);
        SetTriggerBoxSize(camWide, new Vector3(40f, 20f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 224f, 2f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 1.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, -1f);

        FinalizeScene(scene);
        return $"Level_23 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 24: Subterranean Labyrinth (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 24 (Subterranean Labyrinth)")]
    public static string BuildLevel24()
    {
        string scenePath = BaseDir + "Level_24.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 235f, 0f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 26f, -10f);

        // Act 1: The Labyrinth Entrance (X = 0 to 55)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.Slope45Left, env, 15f, 0f);
        for (int x = 20; x <= 32; x += 8) p.Spawn(p.IceSurface, env, x, 5f);

        // Lower path touch blocks
        p.SpawnTouchWallBlock(inter, 20f, 35f, 1f, 2f);

        for (int y = 0; y <= 2; y++) p.Spawn(p.DashWall, inter, 45f, y);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 18f + i * 8f, 3f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: Deep Excavation Vaults (X = 55 to 115)
        for (int x = 60; x <= 100; x += 20) p.Spawn(p.Ground20, env, x, -7f);

        p.Spawn(p.Wall12, env, 65f, 1f);
        p.Spawn(p.Wall12, env, 75f, 1f);

        p.SpawnTouchWallBlock(inter, 67f, 73f, 2f, 2f);
        p.Spawn(p.CrackedBlocks4x4, inter, 70f, -1f);
        p.SpawnTouchWallBlock(inter, 67f, 73f, -4f, -4f);

        // Dense grid of TouchWallBlue hiding Diamond 1
        p.SpawnTouchWallBlock(inter, 74f, 78f, -6f, -4f);
        GameObject d1 = p.Spawn(p.Diamond, col, 76f, -4.5f);
        SetDiamondId(d1, 1);

        p.Spawn(p.StompBlock, haz, 88f, -1f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 82f + i * 6f, -5.5f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.BouncePadMega, inter, 100f, -5f);
        p.Spawn(p.Ground20, env, 110f, -1f);

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 110f, 0f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Crumbling Skyway (X = 115 to 175)
        for (int x = 120; x <= 160; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        for (int x = 125; x <= 150; x += 3) p.Spawn(p.CrumblingBlock, env, x, 7f);

        p.SpawnTouchWallBlock(inter, 128f, 146f, 1f, 2f);
        p.Spawn(p.SawSlow, haz, 138f, 2f);

        GameObject d2 = p.Spawn(p.Diamond, col, 138f, 8.5f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 130f + i * 8f, 8.5f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 158f, 4f);
        p.Spawn(p.OneWayPlatform, env, 164f, 2f);

        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 170f, 0f);
        cp3.name = "Checkpoint_3";

        // Act 4: Supersonic Labyrinth Exit (X = 175 to 240)
        for (int x = 180; x <= 220; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 178f, 0.5f);

        p.SpawnTouchWallBlock(inter, 185f, 210f, 1f, 3f);

        for (int y = 0; y <= 3; y++)
        {
            p.Spawn(p.DashWall, inter, 198f, y);
            p.Spawn(p.DashWall, inter, 212f, y);
        }

        GameObject camExit = p.Spawn(p.CameraZoneWide, flow, 220f, 3f);
        SetTriggerBoxSize(camExit, new Vector3(40f, 20f, 10f));

        GameObject d3 = p.Spawn(p.Diamond, col, 224f, 2f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 218f + i * 5f, 1.5f);
            SetAtomId(a, 11 + i);
        }

        p.Spawn(p.Ground20, env, 235f, -1f);

        FinalizeScene(scene);
        return $"Level_24 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 25: Purple Hyper-Gauntlet (X = 0 to 255)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 25 (Purple Hyper-Gauntlet)")]
    public static string BuildLevel25()
    {
        string scenePath = BaseDir + "Level_25.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        GameObject startPortal = p.Spawn(p.StartPortal, flow, 2f, 0f);
        startPortal.name = "StartPortal";
        GameObject endPortal = p.Spawn(p.EndPortal, flow, 250f, 2f);
        endPortal.name = "End_Portal";
        WirePortals(startPortal, endPortal);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 260f, 28f, -10f);

        // Act 1: The Hyper Ignition (X = 0 to 60)
        for (int x = 10; x <= 50; x += 20) p.Spawn(p.Ground20, env, x, -1f);

        p.Spawn(p.DashBoostFast, inter, 10f, 0.5f);
        p.Spawn(p.DashBoostFast, inter, 24f, 0.5f);

        p.SpawnTouchWallBlock(inter, 28f, 50f, 1f, 2f);
        for (int y = 0; y <= 2; y++)
        {
            p.Spawn(p.DashWall, inter, 35f, y);
            p.Spawn(p.DashWall, inter, 45f, y);
        }

        GameObject camTight = p.Spawn(p.CameraZoneTight, flow, 36f, 1f);
        SetTriggerBoxSize(camTight, new Vector3(25f, 8f, 10f));

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 15f + i * 10f, 2f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 56f, 0f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Magma Abyss Excavation (X = 60 to 125)
        for (int x = 65; x <= 110; x += 20) p.Spawn(p.Ground20, env, x, -7f);

        p.Spawn(p.Wall12, env, 70f, 2f);
        p.Spawn(p.Wall12, env, 80f, 2f);

        // 4-layer excavation drop
        p.SpawnTouchWallBlock(inter, 71f, 79f, 6f, 6f);
        p.Spawn(p.CrackedBlocks4x4, inter, 75f, 2f);
        p.SpawnTouchWallBlock(inter, 71f, 79f, -1f, -1f);
        for (int x = 73; x <= 77; x++) p.Spawn(p.DashWall, inter, x, -4f);

        p.Spawn(p.LavaLeaper, haz, 85f, -7f);
        p.Spawn(p.FireJet, haz, 94f, -6.5f);

        GameObject d1 = p.Spawn(p.Diamond, col, 88f, -5f);
        SetDiamondId(d1, 1);

        p.Spawn(p.BouncePadMega, inter, 104f, -6f);
        p.Spawn(p.StompBlock, haz, 114f, 6f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 80f + i * 8f, -5.5f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.Ground20, env, 120f, 1f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 120f, 2f);
        cp2.name = "Checkpoint_2";

        // Act 3: The Spire of Saws & Twin Chimneys (X = 125 to 185)
        for (int x = 130; x <= 170; x += 20) p.Spawn(p.Ground20, env, x, 1f);

        p.Spawn(p.Wall12, env, 132f, 8f);
        p.Spawn(p.Wall12, env, 136f, 8f);
        p.SpawnTouchWallRun(inter, 133f, 135f, 14f);

        for (int x = 142; x <= 170; x += 8) p.Spawn(p.Ledge8, env, x, 16f);

        p.Spawn(p.SpinArm, haz, 148f, 18f);
        p.Spawn(p.SawSlow, haz, 162f, 18f);

        GameObject d2 = p.Spawn(p.Diamond, col, 155f, 18.5f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 144f + i * 7f, 17.5f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.OneWayPlatform, env, 175f, 10f);
        p.Spawn(p.OneWayPlatform, env, 178f, 5f);

        p.Spawn(p.Ground20, env, 185f, 1f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 182f, 2f);
        cp3.name = "Checkpoint_3";

        // Act 4: The Grand Hyper-Climax (X = 185 to 255)
        for (int x = 195; x <= 235; x += 20) p.Spawn(p.Ground20, env, x, 1f);

        p.Spawn(p.DashBoostFast, inter, 190f, 2.5f);
        p.Spawn(p.DashBoostFast, inter, 202f, 2.5f);

        for (int y = 2; y <= 5; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 196f, y);
            p.Spawn(p.DashWall, inter, 206f, y);
        }

        // 18m Grand Rocket Chimney
        p.Spawn(p.Wall12, env, 213f, 10f);
        p.Spawn(p.Wall12, env, 217f, 10f);

        GameObject finalRocket = p.Spawn(p.DashLaneUp, inter, 215f, 8f);
        finalRocket.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        SetDashZoneSize(finalRocket, new Vector2(20f, 3f));

        p.SpawnTouchWallRun(inter, 214f, 216f, 5f);
        p.SpawnTouchWallRun(inter, 214f, 216f, 9f);
        p.SpawnTouchWallRun(inter, 214f, 216f, 13f);
        p.SpawnTouchWallRun(inter, 214f, 216f, 17f);

        GameObject camClimax = p.Spawn(p.CameraZoneWide, flow, 230f, 14f);
        SetTriggerBoxSize(camClimax, new Vector3(60f, 30f, 10f));

        // 4-layer Grand Victory Archway
        for (int y = 2; y <= 6; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 242f, y);
            p.Spawn(p.DashWall, inter, 243f, y);
            p.Spawn(p.TouchWallBlue, inter, 244f, y);
            p.Spawn(p.DashWall, inter, 245f, y);
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 244f, 4f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 234f + i * 4f, 4.5f);
            SetAtomId(a, 13 + i);
        }

        p.Spawn(p.Ground20, env, 250f, 1f);

        FinalizeScene(scene);
        return $"Level_25 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 26: Purple Cascade Quarry (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 26 (Purple Cascade Quarry)")]
    public static string BuildLevel26()
    {
        string scenePath = BaseDir + "Level_26.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        // Start & End Portals
        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 13f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 235f, 5f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 26f, -12f);

        // Act 1: Upper Quarry Rim & First Excavation Trench (X = 0 to 60)
        p.Spawn(p.Ground20, env, 6f, 12f);
        p.Spawn(p.Slope45, env, 16f, 5f);

        // Cascading stair of TouchWallBlue coating the diagonal slope
        for (int i = 0; i < 7; i++)
        {
            float x = 16f + i;
            float y = 11f - i;
            p.Spawn(p.TouchWallBlue, inter, x, y);
            p.Spawn(p.TouchWallBlue, inter, x, y + 1f);
        }

        // Quarry terrace at Y = 5
        p.Spawn(p.Ground20, env, 28f, 4f);

        // Yellow CrackedBlocks_4x4 plug over deep trench
        p.Spawn(p.CrackedBlocks4x4, inter, 28f, 5f);

        // Subterranean excavation trench at Y = -2
        p.Spawn(p.Ground20, env, 28f, -3f);
        p.Spawn(p.Ground20, env, 45f, -3f);
        p.SpawnTouchWallBlock(inter, 32f, 40f, -2f, -1f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 34f + i * 4f, -0.5f);
            SetAtomId(a, 1 + i);
        }

        // BouncePad_Mega launches back to upper terrace
        p.Spawn(p.BouncePadMega, inter, 46f, -2f);
        p.Spawn(p.Ground20, env, 54f, 9f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 54f, 10f);
        cp1.name = "Checkpoint_1";

        // Act 2: Multi-Tier Quarry Shelves & Derricks (X = 60 to 125)
        // High Scaffolding at Y = 13
        p.Spawn(p.OneWayPlatform, env, 68f, 13f);
        p.Spawn(p.OneWayPlatform, env, 76f, 13f);
        p.Spawn(p.OneWayPlatform, env, 84f, 13f);
        p.SpawnTouchWallRun(inter, 68f, 84f, 14f, 2f);

        // Mid Quarry Shelf at Y = 5
        p.Spawn(p.Ground20, env, 75f, 4f);
        p.Spawn(p.Ground20, env, 95f, 4f);

        // Diamond 1 inside DashWall vault on mid-shelf
        for (int x = 84; x <= 88; x++)
        {
            p.Spawn(p.DashWall, inter, x, 5f);
            p.Spawn(p.DashWall, inter, x, 7f);
        }
        p.Spawn(p.DashWall, inter, 84f, 6f);
        p.Spawn(p.DashWall, inter, 88f, 6f);
        GameObject d1 = p.Spawn(p.Diamond, col, 86f, 6f);
        SetDiamondId(d1, 1);

        p.Spawn(p.SawSlow, haz, 96f, 5.5f);

        // Twin-wall chimney from Y = 0 to 12
        p.Spawn(p.Wall12, env, 106f, 6f);
        p.Spawn(p.Wall12, env, 112f, 6f);
        p.Spawn(p.TouchWallBlue, inter, 107f, 7f);
        p.Spawn(p.TouchWallBlue, inter, 111f, 9f);

        // Safety floor below
        p.Spawn(p.Ground20, env, 75f, -3f);
        p.Spawn(p.Ground20, env, 95f, -3f);
        p.Spawn(p.Ground20, env, 115f, -3f);
        p.Spawn(p.KillPit, haz, 95f, -7f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 70f + i * 12f, 6f);
            SetAtomId(a, 4 + i);
        }

        p.Spawn(p.Ground20, env, 122f, 5f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 122f, 6f);
        cp2.name = "Checkpoint_2";

        // Act 3: Stepped Descent & Bedrock Excavation (X = 125 to 185)
        p.Spawn(p.Ground20, env, 134f, 3f);
        p.Spawn(p.Ground20, env, 146f, 0f);
        p.Spawn(p.Ground20, env, 158f, -4f);
        p.Spawn(p.Ground20, env, 170f, -8f);

        p.SpawnTouchWallBlock(inter, 134f, 138f, 4f, 5f);
        p.SpawnTouchWallBlock(inter, 144f, 148f, 1f, 2f);
        p.SpawnTouchWallBlock(inter, 154f, 158f, -3f, -2f);

        // Bedrock Yellow CrackedBlocks plug
        p.Spawn(p.CrackedBlocks4x4, inter, 158f, -4f);
        GameObject d2 = p.Spawn(p.Diamond, col, 158f, -7f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 136f + i * 10f, 2f - i * 2.5f);
            SetAtomId(a, 8 + i);
        }

        // Catapult out of bedrock
        p.Spawn(p.BouncePadMega, inter, 172f, -7f);
        p.SpawnTouchWallRun(inter, 172f, 172f, 0f, 1f);

        p.Spawn(p.Ground20, env, 182f, 4f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 182f, 5f);
        cp3.name = "Checkpoint_3";

        // Act 4: Quarry Sifter & Final Blast Run (X = 185 to 240)
        p.Spawn(p.Ground20, env, 202f, 3f);
        p.Spawn(p.Ground20, env, 222f, 3f);
        p.Spawn(p.Ground20, env, 238f, 4f);

        p.Spawn(p.DashBoostFast, inter, 192f, 4f);

        // Alternating 3-tier walls of TouchWallBlue and DashWall
        for (int y = 4; y <= 6; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 204f, y);
            p.Spawn(p.DashWall, inter, 205f, y);

            p.Spawn(p.TouchWallBlue, inter, 214f, y);
            p.Spawn(p.DashWall, inter, 215f, y);

            p.Spawn(p.TouchWallBlue, inter, 224f, y);
            p.Spawn(p.DashWall, inter, 225f, y);
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 224f, 8f);
        SetDiamondId(d3, 3);
        p.Spawn(p.OneWayPlatform, env, 224f, 7f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 198f + i * 8f, 4.5f);
            SetAtomId(a, 12 + i);
        }

        FinalizeScene(scene);
        return $"Level_26 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 27: Twin-Shaft Velocity Tower (X = 0 to 235)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 27 (Twin-Shaft Velocity Tower)")]
    public static string BuildLevel27()
    {
        string scenePath = BaseDir + "Level_27.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        // Start & End Portals
        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 2f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 228f, 4f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 240f, 32f, -8f);

        // Act 1: Tower Base & Shaft Alpha Ascent (X = 0 to 55)
        p.Spawn(p.Ground20, env, 8f, 0f);

        // Shaft Alpha twin walls from Y = 1 to 24
        p.Spawn(p.Wall12, env, 18f, 6f);
        p.Spawn(p.Wall12, env, 18f, 18f);
        p.Spawn(p.Wall12, env, 24f, 6f);
        p.Spawn(p.Wall12, env, 24f, 18f);

        // DashLane_UP at base of Shaft Alpha
        p.Spawn(p.DashLaneUp, inter, 21f, 1f);

        // Rings of TouchWallBlue inside shaft
        for (int y = 5; y <= 15; y += 4)
        {
            p.Spawn(p.TouchWallBlue, inter, 20f, y);
            p.Spawn(p.TouchWallBlue, inter, 22f, y);
        }

        // Upper Sky Bridge at Y = 18
        p.Spawn(p.Ground20, env, 34f, 17f);
        p.Spawn(p.Ground20, env, 48f, 17f);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 28f + i * 8f, 18.5f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 48f, 18f);
        cp1.name = "Checkpoint_1";

        // Act 2: High Sky Bridges & Cannon Battery (X = 55 to 115)
        GameObject mp1 = p.Spawn(p.MovingPlatform, env, 62f, 18f);
        ConfigureMovingPlatform(mp1, new Vector3(58f, 18f, 0f), new Vector3(78f, 18f, 0f), 3f);

        // Lower floor with FireJet hazards
        p.Spawn(p.Ground20, env, 68f, 3f);
        p.Spawn(p.Ground20, env, 88f, 3f);
        p.Spawn(p.FireJet, haz, 68f, 4f);
        p.Spawn(p.FireJet, haz, 88f, 4f);

        // High ledge at Y = 18
        p.Spawn(p.Ground20, env, 92f, 17f);
        p.Spawn(p.Cannon, haz, 88f, 19f);

        // Secret roof crane with Diamond 1
        p.Spawn(p.BouncePad, inter, 82f, 18f);
        p.Spawn(p.OneWayPlatform, env, 82f, 25f);
        p.Spawn(p.DashWall, inter, 81f, 26f);
        p.Spawn(p.DashWall, inter, 83f, 26f);
        GameObject d1 = p.Spawn(p.Diamond, col, 82f, 26f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 64f + i * 8f, 19.5f);
            SetAtomId(a, 4 + i);
        }

        // Descent chute cushioned with TouchWallBlue
        for (int y = 6; y <= 16; y += 2)
        {
            p.Spawn(p.TouchWallBlue, inter, 102f, y);
            p.Spawn(p.TouchWallBlue, inter, 103f, y);
        }

        p.Spawn(p.Ground20, env, 110f, 4f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 110f, 5f);
        cp2.name = "Checkpoint_2";

        // Act 3: Shaft Beta Ascent & Twin-Wall Switchbacks (X = 115 to 175)
        p.Spawn(p.Ground20, env, 126f, 4f);

        // Shaft Beta at X = 132 to 138
        p.Spawn(p.Wall12, env, 132f, 10f);
        p.Spawn(p.Wall12, env, 138f, 10f);
        p.Spawn(p.DashLaneUp, inter, 135f, 5f);

        p.Spawn(p.OneWayPlatform, env, 135f, 12f);
        p.Spawn(p.TouchWallBlue, inter, 135f, 13f);

        // Upper catwalk at Y = 20
        p.Spawn(p.Ground20, env, 150f, 19f);
        p.SpawnTouchWallRun(inter, 144f, 156f, 20f);

        GameObject mp2 = p.Spawn(p.MovingPlatform, env, 162f, 20f);
        ConfigureMovingPlatform(mp2, new Vector3(156f, 20f, 0f), new Vector3(174f, 20f, 0f), 3.5f);

        p.Spawn(p.FireJet, haz, 162f, 5f);
        p.Spawn(p.Ground20, env, 162f, 4f);

        GameObject d2 = p.Spawn(p.Diamond, col, 164f, 23f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 140f + i * 8f, 21.5f);
            SetAtomId(a, 8 + i);
        }

        p.Spawn(p.Ground20, env, 176f, 20f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 176f, 21f);
        cp3.name = "Checkpoint_3";

        // Act 4: High-Altitude Dive & Grand Portal Pier (X = 175 to 235)
        p.Spawn(p.Slope45Large, env, 184f, 3f);
        p.SpawnTouchWallBlock(inter, 182f, 188f, 8f, 10f);

        p.Spawn(p.Ground20, env, 200f, 2f);
        p.Spawn(p.Ground20, env, 220f, 2f);
        p.Spawn(p.Ground20, env, 232f, 3f);

        p.Spawn(p.DashBoostFast, inter, 195f, 3f);

        // Barricades of DashWall & TouchWallBlue
        for (int y = 3; y <= 5; y++)
        {
            p.Spawn(p.DashWall, inter, 205f, y);
            p.Spawn(p.TouchWallBlue, inter, 206f, y);

            p.Spawn(p.DashWall, inter, 215f, y);
            p.Spawn(p.TouchWallBlue, inter, 216f, y);

            p.Spawn(p.DashWall, inter, 222f, y);
            p.Spawn(p.TouchWallBlue, inter, 223f, y);
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 222f, 6.5f);
        SetDiamondId(d3, 3);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 200f + i * 6f, 3.5f);
            SetAtomId(a, 12 + i);
        }

        FinalizeScene(scene);
        return $"Level_27 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 28: Neon Pinball Reactor (X = 0 to 240)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 28 (Neon Pinball Reactor)")]
    public static string BuildLevel28()
    {
        string scenePath = BaseDir + "Level_28.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        // Start & End Portals
        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 4f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 234f, 3f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 245f, 28f, -8f);

        // Act 1: The Reactor Flipper Chamber (X = 0 to 60)
        p.Spawn(p.Ground20, env, 8f, 2f);
        p.Spawn(p.DashBoostFast, inter, 12f, 3f);

        p.Spawn(p.Slope45Large, env, 18f, 3f);
        p.Spawn(p.BouncePadMega, inter, 28f, 14f);

        // Overhead TouchWallBlue canopy
        for (int x = 28; x <= 36; x++)
        {
            p.Spawn(p.TouchWallBlue, inter, x, 16f);
            p.Spawn(p.TouchWallBlue, inter, x, 17f);
        }

        p.Spawn(p.SpinArm, haz, 42f, 8f);

        p.Spawn(p.Ground20, env, 45f, 2f);
        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 20f + i * 8f, 5f + i * 2f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 52f, 4f);
        cp1.name = "Checkpoint_1";

        // Act 2: The Bumper Matrix (X = 60 to 125)
        // Stepped terraces
        p.Spawn(p.Ground20, env, 70f, 1f);
        p.Spawn(p.Ground20, env, 90f, 1f);
        p.Spawn(p.Ground20, env, 110f, 1f);

        // IceSurface recovery floor
        for (int x = 65; x <= 110; x += 10)
        {
            p.Spawn(p.IceSurface, env, x, 2f);
        }

        // Pinball bounce bumpers
        p.Spawn(p.BouncePad, inter, 68f, 4f);
        p.Spawn(p.BouncePadMini, inter, 78f, 7f);
        p.Spawn(p.BouncePadMega, inter, 88f, 11f);
        p.Spawn(p.BouncePadMini, inter, 98f, 7f);
        p.Spawn(p.BouncePad, inter, 108f, 4f);

        // Clusters of TouchWallBlue like pinball targets
        p.SpawnTouchWallBlock(inter, 76f, 80f, 8f, 9f);
        p.SpawnTouchWallBlock(inter, 96f, 100f, 8f, 9f);

        // Diamond 1 inside DashWall vault
        for (int x = 86; x <= 90; x++)
        {
            p.Spawn(p.DashWall, inter, x, 13f);
            p.Spawn(p.DashWall, inter, x, 15f);
        }
        p.Spawn(p.DashWall, inter, 86f, 14f);
        p.Spawn(p.DashWall, inter, 90f, 14f);
        GameObject d1 = p.Spawn(p.Diamond, col, 88f, 14f);
        SetDiamondId(d1, 1);

        p.Spawn(p.SawSlow, haz, 82f, 7f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 66f + i * 12f, 4f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.Ground20, env, 120f, 2f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 120f, 3f);
        cp2.name = "Checkpoint_2";

        // Act 3: Diagonal Ricochet Chute & Slingshot (X = 125 to 185)
        p.Spawn(p.Slope45, env, 128f, 1f);
        p.Spawn(p.Ground20, env, 145f, 0f);
        p.Spawn(p.Ground20, env, 165f, 0f);

        p.Spawn(p.DashBoostFast, inter, 135f, 1f);
        p.Spawn(p.BouncePadMega, inter, 145f, 1f);

        // High ceiling matrix
        p.SpawnTouchWallBlock(inter, 150f, 160f, 16f, 17f);
        GameObject d2 = p.Spawn(p.Diamond, col, 155f, 18f);
        SetDiamondId(d2, 2);

        // Vertical column of DashWall requiring Ground Pound
        for (int y = 4; y <= 12; y += 2)
        {
            p.Spawn(p.DashWall, inter, 165f, y);
            p.Spawn(p.TouchWallBlue, inter, 166f, y);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 138f + i * 8f, 4f + i * 2f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 178f, 2f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 178f, 3f);
        cp3.name = "Checkpoint_3";

        // Act 4: High-Velocity Reactor Overdrive (X = 185 to 240)
        p.Spawn(p.Ground20, env, 195f, 1f);
        p.Spawn(p.Ground20, env, 215f, 1f);
        p.Spawn(p.Ground20, env, 235f, 2f);

        p.Spawn(p.DashBoostFast, inter, 188f, 2f);
        p.Spawn(p.DashBoostFast, inter, 202f, 2f);

        // Alternating smash walls
        for (int y = 2; y <= 5; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 196f, y);
            p.Spawn(p.DashWall, inter, 197f, y);

            p.Spawn(p.TouchWallBlue, inter, 208f, y);
            p.Spawn(p.DashWall, inter, 209f, y);
        }

        p.Spawn(p.SpinArm, haz, 216f, 5f);

        GameObject d3 = p.Spawn(p.Diamond, col, 226f, 6f);
        SetDiamondId(d3, 3);
        p.Spawn(p.OneWayPlatform, env, 226f, 5f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 192f + i * 8f, 3.5f);
            SetAtomId(a, 13 + i);
        }

        FinalizeScene(scene);
        return $"Level_28 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 29: Subterranean Crystal Vaults (X = 0 to 245)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 29 (Subterranean Crystal Vaults)")]
    public static string BuildLevel29()
    {
        string scenePath = BaseDir + "Level_29.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        // Start & End Portals
        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 8f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 236f, 5f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 250f, 26f, -12f);

        // Act 1: Stalactite Cavern & Frozen Chute (X = 0 to 60)
        p.Spawn(p.Ground20, env, 6f, 6f);
        p.Spawn(p.Ground20, env, 24f, 6f);

        // Stalactite ceiling
        for (int x = 6; x <= 45; x += 8)
        {
            p.Spawn(p.DashWall, inter, x, 14f);
            p.Spawn(p.TouchWallBlue, inter, x + 1f, 13f);
        }

        // IceSurface runway
        for (int x = 12; x <= 26; x += 7)
        {
            p.Spawn(p.IceSurface, env, x, 7f);
            p.Spawn(p.TouchWallBlue, inter, x, 8f);
        }

        // Crumbling bridges over FireJet pit
        p.Spawn(p.CrumblingBlock, env, 32f, 6f);
        p.Spawn(p.CrumblingBlock, env, 37f, 6f);
        p.Spawn(p.FireJet, haz, 34f, 0f);
        p.Spawn(p.Ground20, env, 34f, -1f);

        // Yellow CrackedBlocks_4x4 vault plug
        p.Spawn(p.Ground20, env, 46f, 2f);
        p.Spawn(p.CrackedBlocks4x4, inter, 46f, 6f);

        GameObject d1 = p.Spawn(p.Diamond, col, 46f, 3.5f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 3; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 14f + i * 8f, 8f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 55f, 3f);
        cp1.name = "Checkpoint_1";

        // Act 2: Branching Crystal Labyrinth (X = 60 to 125)
        // High Route (Upper Grotto, Y = 11)
        p.Spawn(p.BouncePad, inter, 63f, 3f);
        p.Spawn(p.OneWayPlatform, env, 72f, 10f);
        p.Spawn(p.OneWayPlatform, env, 82f, 10f);
        p.Spawn(p.OneWayPlatform, env, 92f, 10f);
        p.SpawnTouchWallBlock(inter, 76f, 88f, 11f, 12f);

        // Low Route (Subterranean Flume, Y = -2)
        p.Spawn(p.Ground20, env, 75f, -3f);
        p.Spawn(p.Ground20, env, 95f, -3f);
        for (int x = 70; x <= 95; x += 8)
        {
            p.Spawn(p.IceSurface, env, x, -2f);
            p.Spawn(p.DashWall, inter, x + 4f, -1f);
        }

        // Convergence at crystal cavern platform
        p.Spawn(p.Ground20, env, 110f, 4f);
        for (int i = 0; i < 5; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 70f + i * 9f, 6f);
            SetAtomId(a, 4 + i);
        }

        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 118f, 5f);
        cp2.name = "Checkpoint_2";

        // Act 3: Subterranean Vaults & Crumbling Chasms (X = 125 to 185)
        p.Spawn(p.CrumblingBlock, env, 130f, 4f);
        p.Spawn(p.CrumblingBlock, env, 136f, 4f);
        p.Spawn(p.CrumblingBlock, env, 142f, 4f);
        p.Spawn(p.CrumblingBlock, env, 148f, 4f);

        p.Spawn(p.KillPit, haz, 140f, -8f);

        // Yellow DashWall vault enclosing Diamond 2
        for (int x = 152; x <= 156; x++)
        {
            p.Spawn(p.DashWall, inter, x, 5f);
            p.Spawn(p.DashWall, inter, x, 7f);
        }
        p.Spawn(p.DashWall, inter, 152f, 6f);
        p.Spawn(p.DashWall, inter, 156f, 6f);
        GameObject d2 = p.Spawn(p.Diamond, col, 154f, 6f);
        SetDiamondId(d2, 2);

        p.SpawnTouchWallBlock(inter, 150f, 158f, 8f, 9f);

        // Mega bounce pad into high tunnel
        p.Spawn(p.Ground20, env, 164f, 1f);
        p.Spawn(p.BouncePadMega, inter, 164f, 2f);

        p.Spawn(p.Ground20, env, 174f, 9f);
        p.Spawn(p.Slope45, env, 180f, 4f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 132f + i * 10f, 6f);
            SetAtomId(a, 9 + i);
        }

        p.Spawn(p.Ground20, env, 186f, 4f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 186f, 5f);
        cp3.name = "Checkpoint_3";

        // Act 4: Crystal Core Escape (X = 185 to 245)
        p.Spawn(p.Ground20, env, 204f, 3f);
        p.Spawn(p.Ground20, env, 224f, 3f);
        p.Spawn(p.Ground20, env, 238f, 4f);

        for (int x = 190; x <= 218; x += 7)
        {
            p.Spawn(p.IceSurface, env, x, 4f);
        }

        p.Spawn(p.DashBoostFast, inter, 192f, 4f);

        // Thick barriers of TouchWallBlue and DashWall
        for (int y = 4; y <= 6; y++)
        {
            p.Spawn(p.TouchWallBlue, inter, 204f, y);
            p.Spawn(p.DashWall, inter, 205f, y);

            p.Spawn(p.TouchWallBlue, inter, 214f, y);
            p.Spawn(p.DashWall, inter, 215f, y);

            p.Spawn(p.TouchWallBlue, inter, 224f, y);
            p.Spawn(p.DashWall, inter, 225f, y);
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 222f, 7.5f);
        SetDiamondId(d3, 3);
        p.Spawn(p.OneWayPlatform, env, 222f, 7f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 198f + i * 8f, 5f);
            SetAtomId(a, 13 + i);
        }

        FinalizeScene(scene);
        return $"Level_29 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 30: Purple Grandmaster Finale (X = 0 to 265)
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 30 (Purple Grandmaster Finale)")]
    public static string BuildLevel30()
    {
        string scenePath = BaseDir + "Level_30.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        // Start & End Portals
        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 4f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 252f, 4f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 270f, 32f, -14f);

        // Act 1: The Ascent of Champions (X = 0 to 65) - CameraZone_Climb
        GameObject camClimb = p.Spawn(p.CameraZoneClimb, flow, 25f, 10f);
        SetTriggerBoxSize(camClimb, new Vector3(35f, 24f, 10f));

        p.Spawn(p.Ground20, env, 6f, 2f);
        p.Spawn(p.Slope45, env, 16f, 2f);

        // Chimney at X = 20 to 25
        p.Spawn(p.Wall12, env, 20f, 12f);
        p.Spawn(p.Wall12, env, 25f, 12f);
        p.Spawn(p.TouchWallBlue, inter, 21f, 10f);
        p.Spawn(p.TouchWallBlue, inter, 24f, 14f);

        // DashLane_UP launching up to Y = 18
        p.Spawn(p.Ground20, env, 34f, 2f);
        p.Spawn(p.DashLaneUp, inter, 32f, 3f);

        for (int y = 6; y <= 16; y += 3)
        {
            p.Spawn(p.TouchWallBlue, inter, 31f, y);
            p.Spawn(p.TouchWallBlue, inter, 33f, y);
        }

        // Catwalk at Y = 18
        p.Spawn(p.Ground20, env, 46f, 17f);
        p.Spawn(p.Ground20, env, 60f, 17f);
        p.SpawnTouchWallRun(inter, 42f, 56f, 18f, 2f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 18f + i * 10f, 8f + i * 2.5f);
            SetAtomId(a, 1 + i);
        }

        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 58f, 18f);
        cp1.name = "Checkpoint_1";

        // Act 2: Multi-Hazard Crucible (X = 65 to 135) - CameraZone_Wide
        GameObject camWide = p.Spawn(p.CameraZoneWide, flow, 100f, 12f);
        SetTriggerBoxSize(camWide, new Vector3(55f, 26f, 10f));

        p.Spawn(p.Ground20, env, 74f, 17f);
        p.Spawn(p.StompBlock, haz, 72f, 21f);

        // Lower hazardous valley
        p.Spawn(p.Ground20, env, 85f, 3f);
        p.Spawn(p.Ground20, env, 105f, 3f);
        p.Spawn(p.FireJet, haz, 82f, 4f);
        p.Spawn(p.FireJet, haz, 102f, 4f);
        p.Spawn(p.LavaLeaper, haz, 95f, 4f);

        // High MovingPlatform crossing over the crucible
        GameObject mp1 = p.Spawn(p.MovingPlatform, env, 95f, 14f);
        ConfigureMovingPlatform(mp1, new Vector3(82f, 14f, 0f), new Vector3(108f, 14f, 0f), 3f);

        p.Spawn(p.SpinArm, haz, 115f, 14f);

        // Diamond 1 roof vault
        p.Spawn(p.BouncePad, inter, 92f, 14f);
        p.Spawn(p.OneWayPlatform, env, 92f, 22f);
        for (int x = 90; x <= 94; x++)
        {
            p.Spawn(p.DashWall, inter, x, 23f);
            p.Spawn(p.DashWall, inter, x, 25f);
        }
        p.Spawn(p.DashWall, inter, 90f, 24f);
        p.Spawn(p.DashWall, inter, 94f, 24f);
        GameObject d1 = p.Spawn(p.Diamond, col, 92f, 24f);
        SetDiamondId(d1, 1);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 78f + i * 10f, 15f);
            SetAtomId(a, 5 + i);
        }

        p.Spawn(p.Ground20, env, 128f, 11f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 128f, 12f);
        cp2.name = "Checkpoint_2";

        // Act 3: Deep Excavation Cavern (X = 135 to 195) - CameraZone_Tight
        GameObject camTight1 = p.Spawn(p.CameraZoneTight, flow, 162f, 0f);
        SetTriggerBoxSize(camTight1, new Vector3(30f, 32f, 10f));

        // Plunge from Y = 11 down to Y = -10!
        p.Spawn(p.Ground20, env, 145f, 10f);

        // 4 Layers of CrackedBlocks_4x4 / DashWall plugs with TouchWallBlue cushions
        // Layer 1 at Y = 8
        p.Spawn(p.CrackedBlocks4x4, inter, 160f, 8f);
        p.SpawnTouchWallBlock(inter, 158f, 162f, 9f, 9f);

        // Layer 2 at Y = 2
        p.Spawn(p.CrackedBlocks4x4, inter, 160f, 2f);
        p.SpawnTouchWallBlock(inter, 158f, 162f, 3f, 3f);

        // Layer 3 at Y = -4
        p.Spawn(p.CrackedBlocks4x4, inter, 160f, -4f);
        p.SpawnTouchWallBlock(inter, 158f, 162f, -3f, -3f);

        // Deepest bedrock at Y = -10
        p.Spawn(p.Ground20, env, 162f, -11f);
        p.Spawn(p.Ground20, env, 178f, -11f);

        GameObject d2 = p.Spawn(p.Diamond, col, 162f, -9.5f);
        SetDiamondId(d2, 2);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 152f + i * 5f, 4f - i * 4f);
            SetAtomId(a, 9 + i);
        }

        // Mega BouncePad catapults player up to Y = 3
        p.Spawn(p.BouncePadMega, inter, 176f, -10f);
        p.SpawnTouchWallRun(inter, 176f, 176f, -4f, 1f);

        p.Spawn(p.Ground20, env, 186f, 2f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 186f, 3f);
        cp3.name = "Checkpoint_3";

        // Act 4: Supersonic Grand Finale (X = 195 to 265) - CameraZone_Tight
        GameObject camTight2 = p.Spawn(p.CameraZoneTight, flow, 228f, 5f);
        SetTriggerBoxSize(camTight2, new Vector3(40f, 16f, 10f));

        p.Spawn(p.Ground20, env, 204f, 2f);
        p.Spawn(p.Ground20, env, 224f, 2f);
        p.Spawn(p.Ground20, env, 244f, 2f);
        p.Spawn(p.Ground20, env, 260f, 3f);

        p.Spawn(p.DashBoostFast, inter, 198f, 3f);
        p.Spawn(p.DashBoostFast, inter, 214f, 3f);

        // Monumental 12x4 wall of alternating TouchWallBlue and DashWall (X = 224 to 235, Y = 3 to 6)
        for (int x = 224; x <= 235; x++)
        {
            for (int y = 3; y <= 6; y++)
            {
                if ((x + y) % 2 == 0)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
                else
                    p.Spawn(p.DashWall, inter, x, y);
            }
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 230f, 8f);
        SetDiamondId(d3, 3);
        p.Spawn(p.OneWayPlatform, env, 230f, 7f);

        for (int i = 0; i < 4; i++)
        {
            GameObject a = p.Spawn(p.Atom, col, 202f + i * 8f, 3.5f);
            SetAtomId(a, 13 + i);
        }

        FinalizeScene(scene);
        return $"Level_30 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 31: Thermal Updraft Caverns (X = 0 to 255)
    // Gliding air-currents, tall BouncePad_Mega & DashLane_UP, touch clouds, yellow air-barriers
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 31 (Thermal Updraft Caverns)")]
    public static string BuildLevel31()
    {
        string scenePath = BaseDir + "Level_31.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 2f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 252f, 6f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 265f, 30f, -10f);

        // Safety Floor Base (Continuous ground under all caverns)
        for (float fx = 0f; fx <= 260f; fx += 20f)
        {
            p.Spawn(p.Ground20, env, fx, 0f);
        }
        p.Spawn(p.KillPit, haz, 130f, -6f);

        // Act 1: Updraft Launch & Maiden Glide (X = 0 to 65)
        p.Spawn(p.Ground20, env, 6f, 1f);
        p.Spawn(p.Ground20, env, 18f, 1f);
        p.Spawn(p.Ground20, env, 28f, 1f);

        p.Spawn(p.BouncePadMega, inter, 32f, 2f);
        GameObject cz1 = p.Spawn(p.CameraZoneWide, flow, 45f, 18f);
        SetTriggerBoxSize(cz1, new Vector3(40f, 16f, 10f));

        // Floating Purple Touch Cloud 1
        p.SpawnTouchWallBlock(inter, 36f, 48f, 16f, 18f, 2f, 1f);
        p.Spawn(p.OneWayPlatform, env, 42f, 15f);
        GameObject d1 = p.Spawn(p.Diamond, col, 42f, 16f);
        SetDiamondId(d1, 1);

        GameObject a1 = p.Spawn(p.Atom, col, 38f, 19f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, col, 46f, 19f);
        SetAtomId(a2, 2);

        // Yellow DashWall air barrier (requires air-dash while gliding)
        p.Spawn(p.DashWall, inter, 54f, 16f);
        p.Spawn(p.DashWall, inter, 54f, 17f);
        p.Spawn(p.DashWall, inter, 54f, 18f);

        p.Spawn(p.BouncePadMini, inter, 40f, 1f);
        p.Spawn(p.BouncePadMini, inter, 52f, 1f);

        p.Spawn(p.Ground20, env, 66f, 13f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 62f, 15f);
        cp1.name = "Checkpoint_1";

        // Act 2: Thermal Chimney & Aerial Archipelago (X = 65 to 135)
        p.Spawn(p.OneWayPlatform, env, 72f, 11f);
        p.Spawn(p.Ground20, env, 80f, 7f);

        p.Spawn(p.Wall12, env, 88f, 12f);
        p.Spawn(p.Wall12, env, 94f, 12f);
        p.Spawn(p.DashLaneUp, inter, 91f, 8f);
        GameObject czClimb = p.Spawn(p.CameraZoneClimb, flow, 91f, 16f);
        SetTriggerBoxSize(czClimb, new Vector3(8f, 20f, 10f));

        // Cloud Archipelago
        p.SpawnTouchWallBlock(inter, 102f, 108f, 20f, 22f, 2f, 1f);
        GameObject a3 = p.Spawn(p.Atom, col, 104f, 23f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, col, 106f, 23f);
        SetAtomId(a4, 4);

        p.SpawnTouchWallBlock(inter, 114f, 120f, 17f, 19f, 2f, 1f);
        GameObject a5 = p.Spawn(p.Atom, col, 116f, 20f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, col, 118f, 20f);
        SetAtomId(a6, 6);

        // Yellow DashWall air barrier 2
        p.Spawn(p.DashWall, inter, 122f, 17f);
        p.Spawn(p.DashWall, inter, 122f, 18f);
        p.Spawn(p.DashWall, inter, 122f, 19f);

        // Crumbling suspension bridge below
        for (int i = 0; i < 5; i++)
        {
            p.Spawn(p.CrumblingBlock, env, 106f + i * 2.5f, 9f);
        }
        GameObject a7 = p.Spawn(p.Atom, col, 109f, 10.5f);
        SetAtomId(a7, 7);
        GameObject a8 = p.Spawn(p.Atom, col, 115f, 10.5f);
        SetAtomId(a8, 8);

        p.Spawn(p.Ground20, env, 136f, 11f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 132f, 13f);
        cp2.name = "Checkpoint_2";

        // Act 3: Stepped Descent & Glide Vault (X = 135 to 195)
        p.Spawn(p.OneWayPlatform, env, 144f, 9f);
        p.Spawn(p.OneWayPlatform, env, 152f, 6f);
        p.Spawn(p.Ground20, env, 150f, 3f);
        p.Spawn(p.FireJet, haz, 146f, 4f);
        p.Spawn(p.FireJet, haz, 156f, 4f);

        p.Spawn(p.BouncePadMega, inter, 162f, 4f);
        p.SpawnTouchWallBlock(inter, 168f, 178f, 17f, 19f, 2f, 1f);

        // Hollow DashWall air vault enclosing Diamond 2
        for (int x = 171; x <= 173; x++)
        {
            p.Spawn(p.DashWall, inter, x, 17f);
            p.Spawn(p.DashWall, inter, x, 19f);
        }
        p.Spawn(p.DashWall, inter, 171f, 18f);
        p.Spawn(p.DashWall, inter, 173f, 18f);
        GameObject d2 = p.Spawn(p.Diamond, col, 172f, 18f);
        SetDiamondId(d2, 2);

        GameObject a9 = p.Spawn(p.Atom, col, 166f, 18.5f);
        SetAtomId(a9, 9);
        GameObject a10 = p.Spawn(p.Atom, col, 178f, 18.5f);
        SetAtomId(a10, 10);

        p.Spawn(p.Ground20, env, 192f, 7f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 188f, 9f);
        cp3.name = "Checkpoint_3";

        // Act 4: Grand Thermal Sky-Ascent & Victory Drop (X = 195 to 255)
        p.Spawn(p.DashLaneUp, inter, 198f, 8f);
        p.Spawn(p.OneWayPlatform, env, 206f, 15f);
        p.Spawn(p.BouncePadMega, inter, 208f, 16f);

        GameObject czWide = p.Spawn(p.CameraZoneWide, flow, 225f, 20f);
        SetTriggerBoxSize(czWide, new Vector3(45f, 16f, 10f));

        p.SpawnTouchWallBlock(inter, 214f, 222f, 20f, 22f, 2f, 1f);
        p.SpawnTouchWallBlock(inter, 230f, 238f, 18f, 20f, 2f, 1f);

        p.Spawn(p.OneWayPlatform, env, 228f, 19f);
        GameObject d3 = p.Spawn(p.Diamond, col, 228f, 20f);
        SetDiamondId(d3, 3);

        GameObject a11 = p.Spawn(p.Atom, col, 216f, 23f);
        SetAtomId(a11, 11);
        GameObject a12 = p.Spawn(p.Atom, col, 220f, 23f);
        SetAtomId(a12, 12);
        GameObject a13 = p.Spawn(p.Atom, col, 232f, 21f);
        SetAtomId(a13, 13);
        GameObject a14 = p.Spawn(p.Atom, col, 236f, 21f);
        SetAtomId(a14, 14);
        GameObject a15 = p.Spawn(p.Atom, col, 240f, 18f);
        SetAtomId(a15, 15);
        GameObject a16 = p.Spawn(p.Atom, col, 244f, 15f);
        SetAtomId(a16, 16);

        // Final 4-block vertical DashWall air barrier
        p.Spawn(p.DashWall, inter, 244f, 13f);
        p.Spawn(p.DashWall, inter, 244f, 14f);
        p.Spawn(p.DashWall, inter, 244f, 15f);
        p.Spawn(p.DashWall, inter, 244f, 16f);

        p.Spawn(p.Ground20, env, 250f, 5f);

        FinalizeScene(scene);
        return $"Level_31 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 32: Shatter-Core Spiral (X = 0 to 250)
    // Multi-story vertical zig-zag, twin-wall chimneys, elevators, layered Ground Pound shafts
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 32 (Shatter-Core Spiral)")]
    public static string BuildLevel32()
    {
        string scenePath = BaseDir + "Level_32.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 2f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 244f, 4f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 255f, 28f, -10f);

        // Continuous safety floor base at Y = -7
        for (float fx = 0f; fx <= 250f; fx += 20f)
        {
            p.Spawn(p.Ground20, env, fx, -7f);
        }
        p.Spawn(p.KillPit, haz, 125f, -10f);

        // Act 1: Chimney 1 & First Excavation Shaft A (X = 0 to 60)
        p.Spawn(p.Ground20, env, 6f, 1f);
        p.Spawn(p.Ground20, env, 18f, 1f);

        p.Spawn(p.Wall12, env, 20f, 7f);
        p.Spawn(p.Wall12, env, 26f, 7f);
        p.Spawn(p.TouchWallBlue, inter, 21f, 5f);
        p.Spawn(p.TouchWallBlue, inter, 25f, 8f);
        p.Spawn(p.TouchWallBlue, inter, 21f, 11f);

        p.Spawn(p.Ground20, env, 34f, 12f);
        GameObject a1 = p.Spawn(p.Atom, col, 28f, 14f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, col, 34f, 14f);
        SetAtomId(a2, 2);

        // Shaft A (X = 40 to 46): plunges from Y = 12 to Y = -4
        p.SpawnTouchWallBlock(inter, 41f, 45f, 9f, 9f, 1f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 43f, 5f);
        p.Spawn(p.CrackedBlocks4x4, inter, 43f, 1f);

        p.Spawn(p.Ground20, env, 43f, -5f);
        GameObject d1 = p.Spawn(p.Diamond, col, 43f, -3f);
        SetDiamondId(d1, 1);
        GameObject a3 = p.Spawn(p.Atom, col, 41f, -3f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, col, 45f, -3f);
        SetAtomId(a4, 4);

        p.Spawn(p.BouncePadMega, inter, 48f, -4f);
        p.Spawn(p.Ground20, env, 58f, 5f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 54f, 7f);
        cp1.name = "Checkpoint_1";

        // Act 2: Moving Elevator & Deep Core Excavation Shaft B (X = 60 to 125)
        GameObject elevator = p.Spawn(p.MovingPlatform, env, 64f, 7f);
        ConfigureMovingPlatform(elevator, new Vector3(64f, 7f, 0f), new Vector3(64f, 18f, 0f), 3f);

        p.Spawn(p.Ground20, env, 74f, 17f);
        p.Spawn(p.SawSlow, haz, 78f, 19f);
        GameObject a5 = p.Spawn(p.Atom, col, 72f, 19f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, col, 76f, 19f);
        SetAtomId(a6, 6);

        // Shaft B (X = 86 to 92): plunges from Y = 17 to Y = -5
        p.SpawnTouchWallBlock(inter, 87f, 91f, 14f, 14f, 1f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 89f, 9f);
        for (int x = 87; x <= 91; x++)
        {
            if (x % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, 4f);
            else p.Spawn(p.DashWall, inter, x, 4f);
        }
        p.Spawn(p.CrackedBlocks4x4, inter, 89f, 0f);

        p.Spawn(p.Ground20, env, 89f, -6f);
        p.Spawn(p.FireJet, haz, 85f, -5f);
        GameObject d2 = p.Spawn(p.Diamond, col, 89f, -4f);
        SetDiamondId(d2, 2);
        GameObject a7 = p.Spawn(p.Atom, col, 87f, -4f);
        SetAtomId(a7, 7);
        GameObject a8 = p.Spawn(p.Atom, col, 91f, -4f);
        SetAtomId(a8, 8);

        GameObject shuttle = p.Spawn(p.MovingPlatform, env, 102f, -4f);
        ConfigureMovingPlatform(shuttle, new Vector3(98f, -4f, 0f), new Vector3(112f, -4f, 0f), 3.5f);

        p.Spawn(p.BouncePadMega, inter, 116f, -4f);
        p.Spawn(p.Ground20, env, 124f, 5f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 122f, 7f);
        cp2.name = "Checkpoint_2";

        // Act 3: Chimney 2 & Shaft C (X = 125 to 185)
        p.Spawn(p.Wall12, env, 132f, 12f);
        p.Spawn(p.Wall12, env, 138f, 12f);

        p.Spawn(p.OneWayPlatform, env, 142f, 17f);
        p.Spawn(p.OneWayPlatform, env, 148f, 17f);
        p.Spawn(p.OneWayPlatform, env, 154f, 17f);
        p.Spawn(p.StompBlock, haz, 150f, 20f);
        GameObject a9 = p.Spawn(p.Atom, col, 144f, 18.5f);
        SetAtomId(a9, 9);
        GameObject a10 = p.Spawn(p.Atom, col, 154f, 18.5f);
        SetAtomId(a10, 10);

        // Shaft C (X = 162 to 168): plunges from Y = 17 to Y = 1
        p.Spawn(p.CrackedBlocks4x4, inter, 165f, 13f);
        p.SpawnTouchWallBlock(inter, 163f, 167f, 8f, 8f, 1f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 165f, 3f);

        p.Spawn(p.Ground20, env, 165f, 0f);
        GameObject a11 = p.Spawn(p.Atom, col, 163f, 2f);
        SetAtomId(a11, 11);
        GameObject a12 = p.Spawn(p.Atom, col, 167f, 2f);
        SetAtomId(a12, 12);

        p.Spawn(p.Ground20, env, 178f, 0f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 178f, 1f);
        cp3.name = "Checkpoint_3";

        // Act 4: Shatter Climax & Apex Glide (X = 185 to 250)
        p.Spawn(p.DashBoostFast, inter, 186f, 1f);

        // 6-block thick strata wall of alternating blocks
        for (int x = 192; x <= 198; x++)
        {
            for (int y = 1; y <= 4; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }

        p.Spawn(p.BouncePadMega, inter, 204f, 1f);
        GameObject czWide = p.Spawn(p.CameraZoneWide, flow, 220f, 16f);
        SetTriggerBoxSize(czWide, new Vector3(40f, 16f, 10f));

        p.SpawnTouchWallBlock(inter, 218f, 226f, 13f, 15f, 2f, 1f);
        p.Spawn(p.OneWayPlatform, env, 222f, 13f);
        GameObject d3 = p.Spawn(p.Diamond, col, 222f, 14f);
        SetDiamondId(d3, 3);

        GameObject a13 = p.Spawn(p.Atom, col, 212f, 16f);
        SetAtomId(a13, 13);
        GameObject a14 = p.Spawn(p.Atom, col, 218f, 14.5f);
        SetAtomId(a14, 14);
        GameObject a15 = p.Spawn(p.Atom, col, 226f, 14.5f);
        SetAtomId(a15, 15);
        GameObject a16 = p.Spawn(p.Atom, col, 234f, 10f);
        SetAtomId(a16, 16);

        p.Spawn(p.Ground20, env, 240f, 3f);
        p.Spawn(p.Ground20, env, 248f, 3f);

        FinalizeScene(scene);
        return $"Level_32 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 33: Velocity Slalom Foundry (X = 0 to 255)
    // Diagonal slopes, ice tracks, chained Chevron Boost Pads, alternating gates, hazards
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 33 (Velocity Slalom Foundry)")]
    public static string BuildLevel33()
    {
        string scenePath = BaseDir + "Level_33.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 16f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 252f, 3f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 260f, 28f, -10f);

        // Continuous safety floor base at Y = 0
        for (float fx = 0f; fx <= 260f; fx += 20f)
        {
            p.Spawn(p.Ground20, env, fx, 0f);
        }
        p.Spawn(p.KillPit, haz, 130f, -6f);

        // Act 1: Downhill Momentum Slide & Boost 1 (X = 0 to 65)
        p.Spawn(p.Ground20, env, 6f, 15f);
        p.Spawn(p.IceSurface, env, 10f, 15f);

        p.Spawn(p.Slope45Large, env, 18f, 7f);
        for (int i = 0; i < 7; i++)
        {
            p.Spawn(p.TouchWallBlue, inter, 18f + i, 14f - i);
        }

        p.Spawn(p.DashBoostFast, inter, 28f, 4f);
        p.Spawn(p.Ground20, env, 40f, 3f);

        for (int y = 4; y <= 7; y++)
        {
            p.Spawn(p.DashWall, inter, 46f, y);
        }

        p.Spawn(p.SpinArm, haz, 54f, 7f);

        GameObject a1 = p.Spawn(p.Atom, col, 22f, 10f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, col, 34f, 5f);
        SetAtomId(a2, 2);
        GameObject a3 = p.Spawn(p.Atom, col, 42f, 5f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, col, 50f, 5f);
        SetAtomId(a4, 4);

        p.Spawn(p.Ground20, env, 66f, 3f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 62f, 5f);
        cp1.name = "Checkpoint_1";

        // Act 2: Double Slalom & Fast Cannon Alley (X = 65 to 135)
        p.Spawn(p.BouncePadMini, inter, 68f, 4f);
        p.Spawn(p.Slope45Large, env, 76f, 10f);
        p.Spawn(p.CannonFast, haz, 86f, 14f);

        p.Spawn(p.DashBoostFast, inter, 90f, 4f);
        p.SpawnTouchWallBlock(inter, 96f, 98f, 4f, 6f, 1f, 1f);

        p.Spawn(p.DashBoostFast, inter, 102f, 4f);
        for (int x = 108; x <= 110; x++)
        {
            for (int y = 4; y <= 6; y++)
            {
                p.Spawn(p.DashWall, inter, x, y);
            }
        }

        p.Spawn(p.BouncePadMini, inter, 114f, 4f);
        p.Spawn(p.OneWayPlatform, env, 116f, 9f);
        GameObject d1 = p.Spawn(p.Diamond, col, 116f, 10f);
        SetDiamondId(d1, 1);

        GameObject a5 = p.Spawn(p.Atom, col, 80f, 8f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, col, 94f, 5f);
        SetAtomId(a6, 6);
        GameObject a7 = p.Spawn(p.Atom, col, 100f, 5f);
        SetAtomId(a7, 7);
        GameObject a8 = p.Spawn(p.Atom, col, 106f, 5f);
        SetAtomId(a8, 8);

        p.Spawn(p.Ground20, env, 128f, 3f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 124f, 5f);
        cp2.name = "Checkpoint_2";

        // Act 3: Hazard Weave & Pinball Ricochet (X = 135 to 195)
        p.Spawn(p.IceSurface, env, 134f, 4f);
        p.Spawn(p.SawSlow, haz, 142f, 6f);

        p.Spawn(p.BouncePad, inter, 148f, 4f);
        p.SpawnTouchWallBlock(inter, 152f, 160f, 11f, 13f, 2f, 1f);

        for (int x = 157; x <= 159; x++)
        {
            p.Spawn(p.DashWall, inter, x, 11f);
            p.Spawn(p.DashWall, inter, x, 13f);
        }
        p.Spawn(p.DashWall, inter, 157f, 12f);
        p.Spawn(p.DashWall, inter, 159f, 12f);
        GameObject d2 = p.Spawn(p.Diamond, col, 158f, 12f);
        SetDiamondId(d2, 2);

        p.Spawn(p.SpinArm, haz, 168f, 14f);

        p.Spawn(p.Slope45Large, env, 174f, 5f);
        p.Spawn(p.DashBoostFast, inter, 184f, 3f);

        for (int x = 188; x <= 192; x++)
        {
            for (int y = 3; y <= 6; y++)
            {
                if (x % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }

        GameObject a9 = p.Spawn(p.Atom, col, 138f, 5f);
        SetAtomId(a9, 9);
        GameObject a10 = p.Spawn(p.Atom, col, 146f, 5f);
        SetAtomId(a10, 10);
        GameObject a11 = p.Spawn(p.Atom, col, 164f, 13f);
        SetAtomId(a11, 11);
        GameObject a12 = p.Spawn(p.Atom, col, 178f, 6f);
        SetAtomId(a12, 12);

        p.Spawn(p.Ground20, env, 198f, 3f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 195f, 4f);
        cp3.name = "Checkpoint_3";

        // Act 4: Supersonic Expressway Climax (X = 195 to 255)
        p.Spawn(p.DashLaneUp, inter, 200f, 4f);
        p.Spawn(p.Ground20, env, 212f, 15f);
        p.Spawn(p.IceSurface, env, 212f, 16f);
        GameObject czTight = p.Spawn(p.CameraZoneTight, flow, 212f, 17f);
        SetTriggerBoxSize(czTight, new Vector3(20f, 6f, 10f));

        p.Spawn(p.Slope45Large, env, 222f, 9f);
        p.Spawn(p.DashBoostFast, inter, 228f, 3f);
        for (int y = 3; y <= 6; y++) p.Spawn(p.TouchWallBlue, inter, 232f, y);

        p.Spawn(p.DashBoostFast, inter, 236f, 3f);
        for (int y = 3; y <= 6; y++) p.Spawn(p.DashWall, inter, 240f, y);

        p.Spawn(p.DashBoostFast, inter, 244f, 3f);
        for (int x = 247; x <= 249; x++)
        {
            for (int y = 3; y <= 6; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }

        GameObject d3 = p.Spawn(p.Diamond, col, 236f, 8f);
        SetDiamondId(d3, 3);

        GameObject a13 = p.Spawn(p.Atom, col, 206f, 17f);
        SetAtomId(a13, 13);
        GameObject a14 = p.Spawn(p.Atom, col, 216f, 17f);
        SetAtomId(a14, 14);
        GameObject a15 = p.Spawn(p.Atom, col, 230f, 4f);
        SetAtomId(a15, 15);
        GameObject a16 = p.Spawn(p.Atom, col, 242f, 4f);
        SetAtomId(a16, 16);

        p.Spawn(p.Ground20, env, 252f, 2f);

        FinalizeScene(scene);
        return $"Level_33 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 34: Echoing Vaults (X = 0 to 250)
    // Branching subterranean excavation, upper glide catwalks, hollow vaults, timings
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 34 (Echoing Vaults)")]
    public static string BuildLevel34()
    {
        string scenePath = BaseDir + "Level_34.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 14f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 245f, 2f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 255f, 28f, -10f);

        // Continuous safety floor base at Y = -4
        for (float fx = 0f; fx <= 250f; fx += 20f)
        {
            p.Spawn(p.Ground20, env, fx, -4f);
        }
        p.Spawn(p.KillPit, haz, 125f, -8f);

        // Act 1: Split Path - High Glide Catwalk vs Low Crypt (X = 0 to 65)
        p.Spawn(p.Ground20, env, 6f, 13f);

        // High Route
        p.Spawn(p.OneWayPlatform, env, 16f, 13f);
        p.Spawn(p.CrumblingBlock, env, 24f, 13f);
        p.Spawn(p.OneWayPlatform, env, 32f, 13f);
        p.Spawn(p.CrumblingBlock, env, 40f, 13f);
        GameObject a1 = p.Spawn(p.Atom, col, 18f, 14.5f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, col, 30f, 14.5f);
        SetAtomId(a2, 2);
        p.SpawnTouchWallBlock(inter, 20f, 22f, 15f, 16f, 1f, 1f);
        p.SpawnTouchWallBlock(inter, 34f, 36f, 15f, 16f, 1f, 1f);

        // Low Route
        p.Spawn(p.CrackedBlocks4x4, inter, 24f, 7f);
        p.Spawn(p.Ground20, env, 30f, -1f);
        p.Spawn(p.Ground20, env, 45f, -1f);

        // Hollow DashWall Vault with Diamond 1
        for (int x = 35; x <= 37; x++)
        {
            p.Spawn(p.DashWall, inter, x, 1f);
            p.Spawn(p.DashWall, inter, x, 3f);
        }
        p.Spawn(p.DashWall, inter, 35f, 2f);
        p.Spawn(p.DashWall, inter, 37f, 2f);
        GameObject d1 = p.Spawn(p.Diamond, col, 36f, 2f);
        SetDiamondId(d1, 1);

        GameObject a3 = p.Spawn(p.Atom, col, 28f, 1f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, col, 44f, 1f);
        SetAtomId(a4, 4);
        p.Spawn(p.BouncePadMega, inter, 48f, 0f);

        p.Spawn(p.Ground20, env, 60f, 7f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 56f, 9f);
        cp1.name = "Checkpoint_1";

        // Act 2: Crumbling Bridge & StompBlock Timings (X = 65 to 130)
        for (int i = 0; i < 6; i++)
        {
            p.Spawn(p.CrumblingBlock, env, 66f + i * 3f, 8f);
        }
        GameObject a5 = p.Spawn(p.Atom, col, 68f, 9.5f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, col, 76f, 9.5f);
        SetAtomId(a6, 6);
        GameObject a7 = p.Spawn(p.Atom, col, 82f, 9.5f);
        SetAtomId(a7, 7);

        // Lower hazard trench
        p.Spawn(p.Ground20, env, 74f, 2f);
        p.Spawn(p.FireJet, haz, 70f, 3f);
        p.Spawn(p.FireJet, haz, 78f, 3f);

        // Secret vault in trench with Diamond 2
        p.Spawn(p.DashWall, inter, 73f, 3f);
        p.Spawn(p.DashWall, inter, 75f, 3f);
        p.Spawn(p.DashWall, inter, 74f, 4f);
        GameObject d2 = p.Spawn(p.Diamond, col, 74f, 3f);
        SetDiamondId(d2, 2);

        // Hallway with StompBlock
        p.Spawn(p.Ground20, env, 92f, 7f);
        p.Spawn(p.StompBlock, haz, 94f, 11f);
        p.Spawn(p.DashWall, inter, 94f, 8f);
        GameObject a8 = p.Spawn(p.Atom, col, 90f, 8.5f);
        SetAtomId(a8, 8);

        // Twin-wall chimney
        p.Spawn(p.Wall12, env, 106f, 12f);
        p.Spawn(p.Wall12, env, 112f, 12f);
        p.Spawn(p.TouchWallBlue, inter, 107f, 11f);
        p.Spawn(p.TouchWallBlue, inter, 111f, 13f);
        GameObject a9 = p.Spawn(p.Atom, col, 109f, 13f);
        SetAtomId(a9, 9);

        p.Spawn(p.Ground20, env, 124f, 9f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 120f, 11f);
        cp2.name = "Checkpoint_2";

        // Act 3: Aerial Vaults & High-Altitude Glide (X = 130 to 190)
        p.Spawn(p.BouncePadMega, inter, 128f, 10f);
        GameObject czWide = p.Spawn(p.CameraZoneWide, flow, 155f, 18f);
        SetTriggerBoxSize(czWide, new Vector3(45f, 16f, 10f));

        // Floating Vault A
        p.Spawn(p.DashWall, inter, 141f, 17f);
        p.Spawn(p.DashWall, inter, 143f, 17f);
        p.Spawn(p.DashWall, inter, 142f, 16f);
        p.Spawn(p.DashWall, inter, 142f, 18f);
        GameObject a10 = p.Spawn(p.Atom, col, 142f, 17f);
        SetAtomId(a10, 10);

        // Floating Vault B with Diamond 3
        for (int x = 153; x <= 155; x++)
        {
            p.Spawn(p.DashWall, inter, x, 15f);
            p.Spawn(p.DashWall, inter, x, 17f);
        }
        p.Spawn(p.DashWall, inter, 153f, 16f);
        p.Spawn(p.DashWall, inter, 155f, 16f);
        GameObject d3 = p.Spawn(p.Diamond, col, 154f, 16f);
        SetDiamondId(d3, 3);

        // Floating Vault C
        p.Spawn(p.DashWall, inter, 165f, 15f);
        p.Spawn(p.DashWall, inter, 167f, 15f);
        p.Spawn(p.DashWall, inter, 166f, 14f);
        p.Spawn(p.DashWall, inter, 166f, 16f);
        GameObject a11 = p.Spawn(p.Atom, col, 166f, 15f);
        SetAtomId(a11, 11);

        p.SpawnTouchWallBlock(inter, 146f, 150f, 16f, 18f, 2f, 1f);
        p.SpawnTouchWallBlock(inter, 158f, 162f, 15f, 17f, 2f, 1f);

        p.Spawn(p.Ground20, env, 140f, 1f);
        p.Spawn(p.Ground20, env, 160f, 1f);
        p.Spawn(p.Ground20, env, 180f, 1f);

        p.Spawn(p.Ground20, env, 184f, 7f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 180f, 9f);
        cp3.name = "Checkpoint_3";

        // Act 4: Stepped Crypt Descent & Grand Vault Break (X = 190 to 250)
        p.Spawn(p.CrackedBlocks4x4, inter, 192f, 6f);
        p.SpawnTouchWallBlock(inter, 200f, 204f, 3f, 3f, 1f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 212f, 0f);

        GameObject a12 = p.Spawn(p.Atom, col, 194f, 7f);
        SetAtomId(a12, 12);
        GameObject a13 = p.Spawn(p.Atom, col, 202f, 4f);
        SetAtomId(a13, 13);
        GameObject a14 = p.Spawn(p.Atom, col, 210f, 1f);
        SetAtomId(a14, 14);

        p.Spawn(p.Ground20, env, 216f, -1f);
        p.Spawn(p.DashBoostFast, inter, 218f, 0f);

        // Grand Hollow DashWall Chamber
        for (int x = 224; x <= 232; x++)
        {
            p.Spawn(p.DashWall, inter, x, 0f);
            p.Spawn(p.DashWall, inter, x, 3f);
        }
        for (int y = 1; y <= 2; y++)
        {
            p.Spawn(p.DashWall, inter, 224f, y);
            p.Spawn(p.DashWall, inter, 232f, y);
        }
        p.SpawnTouchWallBlock(inter, 226f, 230f, 1f, 2f, 1f, 1f);

        GameObject a15 = p.Spawn(p.Atom, col, 226f, 1.5f);
        SetAtomId(a15, 15);
        GameObject a16 = p.Spawn(p.Atom, col, 230f, 1.5f);
        SetAtomId(a16, 16);

        p.Spawn(p.Ground20, env, 242f, 1f);

        FinalizeScene(scene);
        return $"Level_34 built ({scenePath})";
    }

    // =========================================================================
    // LEVEL 35: Purple Apex Gauntlet (X = 0 to 265)
    // 4-act milestone campaign finale! Full hazard orchestra, excavation, apex gauntlet
    // =========================================================================
    [MenuItem("Elemento/Build FINAL_PURPLE/Level 35 (Purple Apex Gauntlet)")]
    public static string BuildLevel35()
    {
        string scenePath = BaseDir + "Level_35.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);

        SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        PrefabLib p = new PrefabLib();

        GameObject startP = p.Spawn(p.StartPortal, flow, 2f, 2f);
        startP.name = "StartPortal";
        GameObject endP = p.Spawn(p.EndPortal, flow, 256f, 4f);
        endP.name = "End_Portal";
        WirePortals(startP, endP);

        GameObject frame = p.Spawn(p.LevelFrame, env, 0f, 0f);
        AdjustLevelFrame(frame, -10f, 270f, 30f, -12f);

        // Continuous safety floor base at Y = -7
        for (float fx = 0f; fx <= 265f; fx += 20f)
        {
            p.Spawn(p.Ground20, env, fx, -7f);
        }
        p.Spawn(p.KillPit, haz, 135f, -11f);

        // Act 1: The Aerial Apex Glide (X = 0 to 65)
        p.Spawn(p.Ground20, env, 6f, 1f);
        p.Spawn(p.Ground20, env, 18f, 1f);

        p.Spawn(p.DashBoostFast, inter, 12f, 2f);
        for (int y = 2; y <= 4; y++) p.Spawn(p.TouchWallBlue, inter, 16f, y);

        p.Spawn(p.DashLaneUp, inter, 22f, 2f);
        GameObject czClimb = p.Spawn(p.CameraZoneClimb, flow, 22f, 12f);
        SetTriggerBoxSize(czClimb, new Vector3(8f, 20f, 10f));

        GameObject czWide1 = p.Spawn(p.CameraZoneWide, flow, 40f, 20f);
        SetTriggerBoxSize(czWide1, new Vector3(40f, 16f, 10f));

        p.SpawnTouchWallBlock(inter, 28f, 34f, 18f, 20f, 2f, 1f);
        GameObject a1 = p.Spawn(p.Atom, col, 28f, 21f);
        SetAtomId(a1, 1);
        GameObject a2 = p.Spawn(p.Atom, col, 32f, 21f);
        SetAtomId(a2, 2);

        // Mid-glide air barrier
        p.Spawn(p.DashWall, inter, 38f, 17f);
        p.Spawn(p.DashWall, inter, 38f, 18f);
        p.Spawn(p.DashWall, inter, 38f, 19f);

        p.Spawn(p.OneWayPlatform, env, 42f, 16f);
        GameObject d1 = p.Spawn(p.Diamond, col, 42f, 17f);
        SetDiamondId(d1, 1);

        GameObject a3 = p.Spawn(p.Atom, col, 46f, 18f);
        SetAtomId(a3, 3);
        GameObject a4 = p.Spawn(p.Atom, col, 52f, 15f);
        SetAtomId(a4, 4);

        p.Spawn(p.Ground20, env, 64f, 9f);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 60f, 11f);
        cp1.name = "Checkpoint_1";

        // Act 2: Magma Crucible & 4-Story Excavation Pit (X = 65 to 135)
        p.SpawnTouchWallBlock(inter, 76f, 82f, 7f, 7f, 1f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 79f, 3f);
        for (int x = 76; x <= 82; x++)
        {
            if (x % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, -1f);
            else p.Spawn(p.DashWall, inter, x, -1f);
        }
        p.Spawn(p.CrackedBlocks4x4, inter, 79f, -5f);

        p.Spawn(p.Ground20, env, 79f, -7f);
        GameObject d2 = p.Spawn(p.Diamond, col, 79f, -4f);
        SetDiamondId(d2, 2);
        GameObject a5 = p.Spawn(p.Atom, col, 77f, -4f);
        SetAtomId(a5, 5);
        GameObject a6 = p.Spawn(p.Atom, col, 81f, -4f);
        SetAtomId(a6, 6);

        p.Spawn(p.LavaLeaper, haz, 88f, -6f);
        p.Spawn(p.BouncePadMega, inter, 84f, -6f);

        p.Spawn(p.Ground20, env, 102f, 5f);
        p.Spawn(p.Ground20, env, 118f, 5f);
        p.Spawn(p.FireJet, haz, 98f, 6f);
        p.Spawn(p.FireJet, haz, 112f, 6f);
        p.Spawn(p.StompBlock, haz, 106f, 11f);

        GameObject a7 = p.Spawn(p.Atom, col, 100f, 7f);
        SetAtomId(a7, 7);
        GameObject a8 = p.Spawn(p.Atom, col, 110f, 7f);
        SetAtomId(a8, 8);

        p.Spawn(p.Ground20, env, 128f, 5f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 124f, 6f);
        cp2.name = "Checkpoint_2";

        // Act 3: Pinball Spire & Hazard Orchestra (X = 135 to 195)
        p.Spawn(p.BouncePad, inter, 134f, 5f);

        GameObject plat = p.Spawn(p.MovingPlatform, env, 144f, 12f);
        ConfigureMovingPlatform(plat, new Vector3(140f, 12f, 0f), new Vector3(152f, 12f, 0f), 3f);
        p.Spawn(p.SpinArm, haz, 146f, 15f);

        GameObject a9 = p.Spawn(p.Atom, col, 142f, 14f);
        SetAtomId(a9, 9);
        GameObject a10 = p.Spawn(p.Atom, col, 150f, 14f);
        SetAtomId(a10, 10);

        p.Spawn(p.Wall12, env, 158f, 16f);
        p.Spawn(p.Wall12, env, 164f, 16f);

        GameObject czWide2 = p.Spawn(p.CameraZoneWide, flow, 172f, 22f);
        SetTriggerBoxSize(czWide2, new Vector3(30f, 14f, 10f));

        // Hollow DashWall Vault for Diamond 3
        for (int x = 171; x <= 173; x++)
        {
            p.Spawn(p.DashWall, inter, x, 20f);
            p.Spawn(p.DashWall, inter, x, 22f);
        }
        p.Spawn(p.DashWall, inter, 171f, 21f);
        p.Spawn(p.DashWall, inter, 173f, 21f);
        GameObject d3 = p.Spawn(p.Diamond, col, 172f, 21f);
        SetDiamondId(d3, 3);

        // Stepped shaft
        p.Spawn(p.CrackedBlocks4x4, inter, 180f, 15f);
        p.SpawnTouchWallBlock(inter, 178f, 182f, 10f, 10f, 1f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 180f, 5f);

        GameObject a11 = p.Spawn(p.Atom, col, 178f, 16f);
        SetAtomId(a11, 11);
        GameObject a12 = p.Spawn(p.Atom, col, 182f, 11f);
        SetAtomId(a12, 12);

        p.Spawn(p.Ground20, env, 194f, 3f);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 190f, 5f);
        cp3.name = "Checkpoint_3";

        // Act 4: The Supersonic Apex Climax (X = 195 to 265)
        GameObject czTight = p.Spawn(p.CameraZoneTight, flow, 210f, 5f);
        SetTriggerBoxSize(czTight, new Vector3(24f, 6f, 10f));

        p.Spawn(p.Ground20, env, 210f, 2f);
        p.Spawn(p.Ground20, env, 230f, 2f);

        p.Spawn(p.DashBoostFast, inter, 204f, 3f);
        for (int y = 3; y <= 6; y++) p.Spawn(p.TouchWallBlue, inter, 210f, y);

        p.Spawn(p.DashBoostFast, inter, 214f, 3f);
        for (int y = 3; y <= 6; y++) p.Spawn(p.DashWall, inter, 220f, y);

        p.Spawn(p.DashBoostFast, inter, 224f, 3f);
        for (int x = 230; x <= 232; x++)
        {
            for (int y = 3; y <= 6; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }

        p.Spawn(p.BouncePadMega, inter, 238f, 3f);

        GameObject a13 = p.Spawn(p.Atom, col, 206f, 4f);
        SetAtomId(a13, 13);
        GameObject a14 = p.Spawn(p.Atom, col, 216f, 4f);
        SetAtomId(a14, 14);
        GameObject a15 = p.Spawn(p.Atom, col, 226f, 4f);
        SetAtomId(a15, 15);
        GameObject a16 = p.Spawn(p.Atom, col, 234f, 4f);
        SetAtomId(a16, 16);

        // Triumphal victory gate archway
        p.SpawnTouchWallBlock(inter, 248f, 258f, 5f, 9f, 2f, 1f);

        p.Spawn(p.Ground20, env, 254f, 3f);

        FinalizeScene(scene);
        return $"Level_35 built ({scenePath})";
    }

    // =========================================================================
    // HELPER METHODS
    // =========================================================================
    private static void SetupLevelCore()
    {
        GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Managers/LevelCore.prefab");
        GameObject coreObj = (GameObject)PrefabUtility.InstantiatePrefab(corePrefab);
        coreObj.name = "LevelCore";
        coreObj.transform.position = Vector3.zero;

        // Ensure exactly one Vfx exists
        var vfxList = Object.FindObjectsByType<Vfx>(FindObjectsSortMode.None);
        if (vfxList.Length > 1)
        {
            for (int i = 1; i < vfxList.Length; i++)
                Object.DestroyImmediate(vfxList[i].gameObject);
        }

        // Disable Debug HUD overlay & PlayerLab stats sheet
        foreach (var d in Object.FindObjectsByType<DebugOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            d.StartVisible = false;
            d.enabled = false;
            d.gameObject.SetActive(false);
            EditorUtility.SetDirty(d);
        }

        foreach (var lab in Object.FindObjectsByType<PlayerLab>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            lab.ShowPanel = false;
            lab.MixPowers = false;
            lab.enabled = false;
            lab.gameObject.SetActive(false);
            EditorUtility.SetDirty(lab);
        }

        foreach (var cc in Object.FindObjectsByType<CC_ELEMENTO>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            cc.DefaultDash = true;
            cc.DefaultDoubleJump = true;
            cc.DefaultWallJump = true;
            cc.DefaultGroundPound = true;
            cc.DefaultGlide = true;
            cc.GlideNeedsNewPress = false;
            cc.StartingPower = PowerUpType.Aether;
            EditorUtility.SetDirty(cc);
        }
    }

    private static void WirePortals(GameObject startPortal, GameObject endPortal)
    {
        var respawnMgr = Object.FindFirstObjectByType<RespawnManager>();
        if (respawnMgr != null && startPortal != null)
        {
            respawnMgr.SpawnPoint = startPortal.transform;
            EditorUtility.SetDirty(respawnMgr);
            PrefabUtility.RecordPrefabInstancePropertyModifications(respawnMgr);
        }

        var levelGoal = endPortal.GetComponent<LevelGoal>();
        var exitSeq = endPortal.GetComponent<LevelExitSequence>();
        if (levelGoal != null && exitSeq != null)
        {
            levelGoal.ExitSequence = exitSeq;
            EditorUtility.SetDirty(levelGoal);
            PrefabUtility.RecordPrefabInstancePropertyModifications(levelGoal);
        }
    }

    private static void FinalizeScene(Scene scene)
    {
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        EnforceZZero(env);
        EnforceZZero(inter);
        EnforceZZero(haz);
        EnforceZZero(col);
        EnforceZZero(cp);
        EnforceZZero(flow);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
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
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");
        public GameObject LevelFrame = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/LevelFrame_40x14.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject CrumblingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/CrumblingBlock.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject TouchWallBlue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/TouchWall_Blue.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMini = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mini.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");

        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject Cannon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Cannon.prefab");
        public GameObject CannonFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Cannon_Fast.prefab");
        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject StompBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/StompBlock.prefab");
        public GameObject SawSlow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Saw_Slow.prefab");
        public GameObject LavaLeaper = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/LavaLeaper.prefab");

        public GameObject Atom = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Atom.prefab");
        public GameObject Diamond = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Diamond.prefab");
        public GameObject Checkpoint = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/CheckPoint.prefab");
        public GameObject StartPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/StartPortal.prefab");
        public GameObject EndPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/End_Portal.prefab");
        public GameObject TutorialJump = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Jump.prefab");
        public GameObject TutorialDash = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_Dash.prefab");
        public GameObject TutorialGroundPound = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/TutorialSIgn_GroundPound.prefab");
        public GameObject CameraZoneWide = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Wide.prefab");
        public GameObject CameraZoneTight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Tight.prefab");
        public GameObject CameraZoneClimb = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Climb.prefab");

        public GameObject Spawn(GameObject prefab, Transform parent, float x, float y)
        {
            if (prefab == null)
            {
                Debug.LogError("[FinalPurpleLevelBuilder] Null prefab passed to Spawn!");
                return null;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.position = new Vector3(x, y, 0f);
            instance.transform.rotation = Quaternion.identity;
            return instance;
        }

        public void SpawnTouchWallRun(Transform parent, float startX, float endX, float y, float spacing = 1.0f)
        {
            for (float x = startX; x <= endX; x += spacing)
            {
                Spawn(TouchWallBlue, parent, x, y);
            }
        }

        public void SpawnTouchWallBlock(Transform parent, float startX, float endX, float startY, float endY, float stepX = 1.0f, float stepY = 1.0f)
        {
            for (float x = startX; x <= endX; x += stepX)
            {
                for (float y = startY; y <= endY; y += stepY)
                {
                    Spawn(TouchWallBlue, parent, x, y);
                }
            }
        }
    }
}
#endif
