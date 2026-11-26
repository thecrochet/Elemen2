#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level47Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_47.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 47 (Abyssal Ziggurat Descent)")]
    public static string Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, TargetScenePath);

        Transform levelCore = SetupLevelCore();
        Transform env = EnsureContainer("Environment");
        Transform inter = EnsureContainer("Interactables");
        Transform haz = EnsureContainer("Hazards");
        Transform col = EnsureContainer("Collectibles");
        Transform cp = EnsureContainer("Checkpoints");
        Transform flow = EnsureContainer("Flow");

        var p = new PrefabLib();

        // =====================================================================
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES (X = 0..224, Y = -28..56)
        // Ziggurat Stepped Enclosure: Top Tier 28..198 -> Base Tier 6..218!
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 112f, -36f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(360f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(360f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -8 to 232)
        for (int x = -8; x < 232; x += 20)
            p.SpawnGroundSized(env, x, -28f, Mathf.Min(20, 232 - x));

        // Outer Ziggurat Fortress Walls & Zenith Crown Roof (Y = 56)
        p.SpawnBlock(p.BlockCyan, env, 0f, 55f, 4, 166, 4);                // West Outer Citadel Wall X=0..2, Y=-28..55
        p.SpawnBlock(p.BlockCyan, env, 220f, 55f, 8, 166, 4);              // East Outer Citadel Wall X=220..224, Y=-28..55
        for (int x = 0; x < 224; x += 20)
            p.SpawnCeilingSized(env, x, 56f, Mathf.Min(20, 224 - x));      // Roof Y=55..56

        // =====================================================================
        // FLOW: TOP-LEFT RAMPART START -> DEEP ABYSSAL HEART VAULT END PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 32f, 40f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 34f, -20.2f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (100% Coverage + Generous Switchback & Drop Lead-In)
        // =====================================================================
        GameObject cz1Start = p.Spawn(p.CameraZoneTight, flow, 38f, 46f);
        cz1Start.name = "CameraZone_Tight_T1StartTurret";
        SetTriggerBoxSize(cz1Start, new Vector3(30f, 16f, 10f));        // X=23..53, Y=38..54

        GameObject cz1Rampart = p.Spawn(p.CameraZoneWide, flow, 120f, 47f);
        cz1Rampart.name = "CameraZone_Wide_T1RampartCascade";
        SetTriggerBoxSize(cz1Rampart, new Vector3(144f, 18f, 10f));     // X=48..192, Y=38..56

        GameObject cz1Breach = p.Spawn(p.CameraZoneClimb, flow, 184f, 32f);
        cz1Breach.name = "CameraZone_Climb_T1to2EastBreach";
        SetTriggerBoxSize(cz1Breach, new Vector3(40f, 28f, 10f));       // X=164..204, Y=18..46 (10m lead past 174!)

        GameObject cz2Piston = p.Spawn(p.CameraZoneWide, flow, 114f, 29f);
        cz2Piston.name = "CameraZone_Wide_T2ShiftingPiston";
        SetTriggerBoxSize(cz2Piston, new Vector3(174f, 18f, 10f));      // X=27..201, Y=20..38

        GameObject cz2Drop = p.Spawn(p.CameraZoneClimb, flow, 24f, 13f);
        cz2Drop.name = "CameraZone_Climb_T2to3DoubleDrop";
        SetTriggerBoxSize(cz2Drop, new Vector3(36f, 26f, 10f));         // X=6..42, Y=0..26

        GameObject cz3Chasm = p.Spawn(p.CameraZoneWide, flow, 114f, 10f);
        cz3Chasm.name = "CameraZone_Wide_T3CrystalChasm";
        SetTriggerBoxSize(cz3Chasm, new Vector3(176f, 20f, 10f));       // X=26..202, Y=0..20

        GameObject cz3Vault = p.Spawn(p.CameraZoneTight, flow, 166f, 14f);
        cz3Vault.name = "CameraZone_Tight_T3DiamondVault";
        SetTriggerBoxSize(cz3Vault, new Vector3(24f, 10f, 10f));        // X=154..178, Y=9..19

        GameObject cz3Well = p.Spawn(p.CameraZoneClimb, flow, 202f, -9f);
        cz3Well.name = "CameraZone_Climb_T3to4EastWell";
        SetTriggerBoxSize(cz3Well, new Vector3(40f, 34f, 10f));         // X=182..222, Y=-26..8 (11m lead past 193!)

        GameObject cz4Furnace = p.Spawn(p.CameraZoneWide, flow, 126f, -12f);
        cz4Furnace.name = "CameraZone_Wide_T4AbyssalBlitz";
        SetTriggerBoxSize(cz4Furnace, new Vector3(156f, 24f, 10f));    // X=48..204, Y=-24..0

        GameObject cz4Vault = p.Spawn(p.CameraZoneTight, flow, 38f, -15f);
        cz4Vault.name = "CameraZone_Tight_T4HeartVault";
        SetTriggerBoxSize(cz4Vault, new Vector3(54f, 18f, 10f));        // X=11..65, Y=-24..-6

        // =====================================================================
        // TIER 1: THE UPPER RAMPART CASCADE (Start Top-Left -> Surge RIGHT!)
        // X = 26..196, Y = 40..55
        // =====================================================================
        p.SpawnBlock(p.BlockCyan, env, 2f, 55f, 44, 30, 4);               // Top-Left Ziggurat Shoulder X=2..24, Y=40..55
        p.SpawnBlock(p.BlockCyan, env, 24f, 55f, 4, 32, 4);               // Start Turret Back Wall X=24..26, Y=39..55!
        p.SpawnGroundSized(env, 26f, 40f, 30);                           // Rampart Start -> Ice Floor X=26..56, Y=39..40
        p.Spawn(p.IceSurface, env, 56f, 40f);                            // 56..62, Y=39..40
        p.SpawnGroundSized(env, 62f, 40f, 114);                          // Tier 1 Main Floor X=62..176, Y=39..40

        // Stepped Ziggurat East Flanking Shoulder (cleanly stops above 198..220!)
        p.SpawnBlock(p.BlockCyan, env, 198f, 55f, 44, 30, 4);             // East Upper Ziggurat Shoulder X=198..220, Y=40..55
        p.Spawn(p.OneWayPlatformLarge, env, 176f, 40f);                  // GP Breach Mouth 176..184
        p.Spawn(p.OneWayPlatformLarge, env, 184f, 40f);                  // 184..192
        p.SpawnGroundSized(env, 192f, 40f, 6);                           // 192..198 flush against Shoulder!
        p.SpawnBlock(p.BlockGreen, env, 26f, 55f, 20, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 158f, 55f, 20, 4, 4);

        // Undulating Opening Speed Waves (37..74): Slopes, Ice, FireJet, Crumbling, Saw_Slow
        GameObject db1Init = p.Spawn(p.DashBoostFast, inter, 37f, 41.5f);
        ConfigureDashZoneBoost(db1Init, new Vector2(3.0f, 3.0f), 26f, 0.14f, 18f); // Ends ~40.6 cleanly at Slope foot!
        p.Spawn(p.Slope45Large, env, 43f, 40f);                          // Rises UP-RIGHT 41..45, Y=40..44!
        p.Spawn(p.Slope45Left, env, 46f, 42f);                           // Descends RIGHT 45..47, Y=44..42
        p.SpawnBlock(p.BlockCyan, env, 45f, 42f, 4, 4, 4);               // Solid rampart backing 45..47, Y=40..42!
        p.Spawn(p.Slope45Left, env, 48f, 40f);                           // Descends RIGHT 47..49, Y=42..40
        p.Spawn(p.FireJet, haz, 60f, 40f);
        p.Spawn(p.DashCharge, inter, 61f, 44.5f);
        p.Spawn(p.BouncePad, inter, 53f, 40f);
        p.Spawn(p.CrumblingBlock, env, 65f, 42.5f);                      // Interlocked between FireJet & Saw!
        GameObject saw1 = p.Spawn(p.SawSlow, haz, 70f, 44f);
        ConfigureSaw(saw1, new Vector3(0f, 5f, 0f), 3.0f);
        p.Spawn(p.DashCharge, inter, 70f, 48f);
        for (int x = 51; x <= 55; x++)
            for (int y = 45; y <= 49; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 60f, 43f), 1);

        // Rapid-Fire Rampart Barrel Chain (75 -> 148) + Secret Battlement Vault (112..126, Y=47..54)!
        GameObject db1A = p.Spawn(p.DashBoostFast, inter, 76f, 41.5f);
        ConfigureDashZoneBoost(db1A, new Vector2(3.2f, 3.2f), 34f, 0.34f, 22f); // -> ~X=87.5 (clean before Bounce@89!)
        for (int x = 81; x <= 86; x++)
            for (int y = 41; y <= 46; y++)
                if (Mathf.Abs(x - 83.5f) + (46f - y) * 0.6f <= 4.2f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Stepped Purple Arch #1!

        p.Spawn(p.BouncePad, inter, 89f, 40f);
        GameObject hold1A = p.Spawn(p.DashHold, inter, 94f, 44.5f);
        ConfigureDashZoneHold(hold1A, 6f, new Vector2(3.4f, 3.4f), 34f, 0.36f, 22f, 0.22f); // -> (106.2, 45.8)
        for (int x = 97; x <= 103; x++)
            for (int y = 42; y <= 47; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Purple Arch #2!
        SetAtomId(p.Spawn(p.Atom, col, 100f, 49f), 2);

        // Tier 1 Secret Battlement Vault (X = 110..126, Y = 48..55 -> False Yellow DashWall @ 111!)
        p.SpawnGroundSized(env, 111f, 48f, 15);                          // Vault Floor X=111..126, Y=47..48
        p.SpawnBlock(p.BlockCyan, env, 125f, 55f, 2, 14, 4);             // East Vault Seal X=125..126, Y=48..55
        p.Spawn(p.DashCharge, inter, 107f, 46.5f);
        p.Spawn(p.BouncePadMini, inter, 106f, 40f);
        for (int y = 49; y <= 55; y++) p.Spawn(p.DashWall, inter, 111f, y); // False Yellow Door X=111..112!
        GameObject stomp1Sec = p.Spawn(p.StompBlock, haz, 118f, 53.5f);
        ConfigureStompBlock(stomp1Sec, 0.32f);
        p.Spawn(p.CrumblingBlock, env, 118f, 49.5f);                     // Interlocked under StompBlock!
        SetAtomId(p.Spawn(p.Atom, col, 123f, 50f), 3);

        // Lower Rampart Piston & Gatehouse Smash (114 -> 151) -> Sanctuary 1
        GameObject hold1B = p.Spawn(p.DashHold, inter, 115f, 42.5f);
        ConfigureDashZoneHold(hold1B, 0f, new Vector2(3.4f, 3.4f), 34f, 0.28f, 22f, 0.22f); // -> ~124.5, stops cleanly before mp1!
        p.Spawn(p.BouncePadMini, inter, 124f, 40f);
        GameObject mp1 = p.Spawn(p.MovingPlatform, env, 128f, 42f);
        ConfigureMovingPlatform(mp1, new Vector3(4f, 0f, 0f), 3.0f, 0.2f);
        GameObject spin1 = p.Spawn(p.SpinArm, haz, 130f, 45f);
        ConfigureSpinArm(spin1, 90f);
        for (int x = 132; x <= 135; x++)
            for (int y = 47; y <= 51; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject db1Gate = p.Spawn(p.DashBoostFast, inter, 137f, 41.5f);
        ConfigureDashZoneBoost(db1Gate, new Vector2(3.2f, 3.2f), 36f, 0.42f, 24f); // -> X=152.1!
        for (int x = 141; x <= 144; x++)
            for (int y = 41; y <= 46; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2.5m Clear Gap before Gate!
        for (int x = 147; x <= 149; x++)
            for (int y = 41; y <= 47; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);   // Yellow Gatehouse!
        p.Spawn(p.BouncePadMini, inter, 153f, 40f);                      // Handoff booster after Gatehouse!

        // Sanctuary 1 (155..176, Y=40)
        p.SpawnCeilingSized(env, 156f, 50f, 20);
        p.SpawnBlock(p.BlockGreen, env, 158f, 49f, 14, 2, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 165f, 41.5f);
        cp1.name = "Checkpoint_1";

        // Tier 1 -> Tier 2 EAST GROUND POUND BREACH (X = 176..196, Y = 40 -> 22!)
        p.SpawnBlock(p.BlockCyan, env, 175f, 39f, 2, 16, 4);             // Shaft West Divider X=175..176, Y=31..39 (open 22..31!)
        p.SpawnBlock(p.BlockCyan, env, 196f, 39f, 2, 34, 4);             // Shaft East Frame X=196..197, Y=22..39
        p.Spawn(p.CrackedBlocks4x4, inter, 178f, 38f);                   // Y=36..38
        p.Spawn(p.CrackedBlocks4x4, inter, 182f, 38f);
        p.Spawn(p.CrackedBlocks4x4, inter, 186f, 38f);
        p.Spawn(p.CrackedBlocks4x4, inter, 190f, 38f);
        for (int x = 178; x <= 194; x++) p.Spawn(p.TouchWallBlue, inter, x, 33f); // Purple Cushion!
        for (int x = 178; x <= 194; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 26f); // Grows UP Y=26..30!
        p.Spawn(p.DashCharge, inter, 186f, 32f);                         // Within 6m of all GP/Cracked blocks!
        SetAtomId(p.Spawn(p.Atom, col, 186f, 30.5f), 4);

        // =====================================================================
        // TIER 2: THE SHIFTING PISTON WORKS (Sweep RIGHT -> LEFT 196 -> 26, Y=22..37)
        // Double-Drop GP + Angled Bounce Ricochet Chute @ Far-West (12..32, Y=22->2)
        // =====================================================================
        p.SpawnGroundSized(env, 160f, 22f, 38);                          // East Piston Floor X=160..198, Y=21..22
        p.SpawnGroundSized(env, 108f, 22f, 44);                          // Mid Piston Floor X=108..152
        p.SpawnGroundSized(env, 32f, 22f, 68);                           // West Piston Floor X=32..100
        p.Spawn(p.OneWayPlatformLarge, env, 152f, 22f);                  // Safety bridge pit 1 (152..160)!
        p.Spawn(p.OneWayPlatformLarge, env, 100f, 22f);                  // Safety bridge pit 2 (100..108)!

        p.Spawn(p.BouncePadMini, inter, 186f, 22f);                      // Handoff right after Breach!
        GameObject db2Start = p.Spawn(p.DashBoostLeft, inter, 174f, 23.5f);
        ConfigureDashZoneBoost(db2Start, new Vector2(3.2f, 3.2f), 34f, 0.30f, 22f); // -> ~X=163.8!
        for (int x = 166; x <= 170; x++)
            for (int y = 23; y <= 28; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject mp2A = p.Spawn(p.MovingPlatform, env, 156f, 23.5f);
        ConfigureMovingPlatform(mp2A, new Vector3(-6f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.DashCharge, inter, 158f, 26f);
        p.Spawn(p.CrumblingBlock, env, 146f, 23.5f);
        GameObject stomp2A = p.Spawn(p.StompBlock, haz, 146f, 30.5f);
        ConfigureStompBlock(stomp2A, 0.28f);
        for (int x = 149; x <= 154; x++)
            for (int y = 27; y <= 31; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 150f, 26.5f), 5);

        // Piston Centerpiece: Barrel -> DashLane_UP Rafters -> Bonus Pocket Atom 6 -> Drop through SpinArm!
        GameObject hold2A = p.Spawn(p.DashHold, inter, 138f, 25.5f);
        ConfigureDashZoneHold(hold2A, 145f, new Vector2(3.4f, 3.4f), 34f, 0.30f, 20f, 0.22f); // Fires UP-LEFT to (129.6, 31.4)!
        p.Spawn(p.DashLaneUp, inter, 134f, 25f);                         // Alt vertical express lane Y=21..29!
        p.Spawn(p.BouncePadMega, inter, 134f, 22f);
        // Rafter Bonus Pocket (OneWay Shelf Y=32, X=128..140 -> Atom 6!)
        p.Spawn(p.OneWayPlatformLarge, env, 128f, 32f);                  // 128..136
        p.Spawn(p.OneWayPlatform, env, 136f, 32f);                       // 136..140
        SetAtomId(p.Spawn(p.Atom, col, 134f, 34f), 6);
        GameObject spin2A = p.Spawn(p.SpinArm, haz, 126f, 27.5f);
        ConfigureSpinArm(spin2A, 95f);
        p.Spawn(p.DashCharge, inter, 123f, 28f);
        GameObject saw2A = p.Spawn(p.SawSlow, haz, 122f, 24f);
        ConfigureSaw(saw2A, new Vector3(0f, 6f, 0f), 3.2f);
        GameObject mp2B = p.Spawn(p.MovingPlatform, env, 118f, 24f);
        ConfigureMovingPlatform(mp2B, new Vector3(0f, 5f, 0f), 3.4f, 0.15f);

        // Mid-Left Sweep -> Yellow Bulkhead #1 (96..101) -> Checkerboard Drift -> Yellow Bulkhead #2 (48..53)
        GameObject hold2B = p.Spawn(p.DashHold, inter, 114f, 28f);
        ConfigureDashZoneHold(hold2B, 192f, new Vector2(3.4f, 3.4f), 36f, 0.44f, 24f, 0.20f); // -> ~(98.5, 24.7)!
        for (int x = 104; x <= 107; x++)
            for (int y = 23; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m clear gap!
        for (int y = 23; y <= 29; y++)
            p.Spawn(p.DashWallChainRow1, inter, 96f, y);                 // Yellow Bulkhead #1 (96..101)!

        p.Spawn(p.CrumblingBlock, env, 92f, 24f);
        GameObject stomp2B = p.Spawn(p.StompBlock, haz, 91f, 30.5f);
        ConfigureStompBlock(stomp2B, 0.30f);
        p.Spawn(p.BouncePadMini, inter, 88f, 22f);
        GameObject mp2C = p.Spawn(p.MovingPlatform, env, 82f, 23.5f);
        ConfigureMovingPlatform(mp2C, new Vector3(-6f, 0f, 0f), 3.4f, 0.15f);
        GameObject spin2B = p.Spawn(p.SpinArm, haz, 78f, 27.5f);
        ConfigureSpinArm(spin2B, -95f);
        for (int x = 71; x <= 84; x++)
            for (int y = 25; y <= 32; y++)                                // Starts at y=25 cleanly above MovingPlatform@23.5!
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y); // Checkerboard Drift!
        for (int x = 67; x <= 70; x++)
            for (int y = 26; y <= 31; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 78f, 33.5f), 7);

        p.Spawn(p.CrumblingBlock, env, 66f, 23.5f);
        GameObject db2End = p.Spawn(p.DashBoostLeft, inter, 63f, 23.5f);
        ConfigureDashZoneBoost(db2End, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f); // -> ~X=47.1!
        p.Spawn(p.BouncePadMini, inter, 46f, 22f);
        for (int x = 56; x <= 58; x++)
            for (int y = 23; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m clear gap!
        for (int y = 23; y <= 29; y++)
            p.Spawn(p.DashWallChainRow1, inter, 48f, y);                 // Yellow Bulkhead #2 (48..53)!

        // Sanctuary 2 (West Shelf X = 32..46, Y = 22..32)
        p.SpawnCeilingSized(env, 32f, 32f, 14);
        p.SpawnBlock(p.BlockMagenta, env, 34f, 31f, 12, 2, 4);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 40f, 23.5f);
        cp2.name = "Checkpoint_2";

        // Tier 2 -> Tier 3 DOUBLE-DROP GP & ANGLED RICOCHET CHUTE (X = 12..32, Y = 22 -> 2)
        p.SpawnBlock(p.BlockCyan, env, 10f, 32f, 2, 60, 4);              // West Shaft Wall X=10..11, Y=2..32
        p.SpawnBlock(p.BlockCyan, env, 32f, 21f, 2, 24, 4);              // Shaft East Divider X=32..33, Y=9..21 (open 2..9 into Tier 3!)
        p.Spawn(p.OneWayPlatformLarge, env, 14f, 22f);                   // Drop Mouth 14..22
        p.Spawn(p.OneWayPlatformLarge, env, 22f, 22f);                   // 22..30
        p.Spawn(p.CrackedBlocks4x4, inter, 15f, 20f);                    // Layer 1 (Y=18..20)
        p.Spawn(p.CrackedBlocks4x4, inter, 19f, 20f);
        p.Spawn(p.CrackedBlocks4x4, inter, 23f, 20f);
        p.Spawn(p.CrackedBlocks4x4, inter, 27f, 20f);
        for (int x = 14; x <= 30; x++) p.Spawn(p.TouchWallBlue, inter, x, 16f); // Cushion
        p.Spawn(p.DashCharge, inter, 22f, 13f);
        // Middle Catch Shelf + BouncePad Ricochet -> Diamond 1 Pocket (28..31, 11..15)!
        p.SpawnGroundSized(env, 12f, 10f, 12);                           // Middle Left Shelf X=12..24, Y=9..10
        p.Spawn(p.BouncePad, inter, 18f, 10f);                           // Ricochets sideways-up toward Atom 8 + Diamond 1!
        p.Spawn(p.CrumblingBlock, env, 27f, 10.5f);                      // Temporary bridge over Drop #2 gap!
        SetAtomId(p.Spawn(p.Atom, col, 26f, 13.5f), 8);
        SetDiamondId(p.Spawn(p.Diamond, col, 30f, 13.5f), 1);            // Ricochet Reward Diamond 1!
        for (int x = 23; x <= 31; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 5f); // Drop #2 Grows UP Y=5..9 (clean below 11!)

        // =====================================================================
        // TIER 3: THE ECHOING CRYSTAL CHASM (Left->Right 12->200, Y = 2..19)
        // High-Altitude Glide + Stalactite Slalom + Diamond 2 Secret Vault -> East Well
        // =====================================================================
        p.SpawnGroundSized(env, 10f, 2f, 36);                            // Tier 3 West Floor X=10..46, Y=1..2
        p.Spawn(p.IceSurface, env, 46f, 2f);                             // Ice Runway 46..52, Y=1..2
        p.SpawnGroundSized(env, 52f, 2f, 148);                           // Tier 3 East Floor X=52..200, Y=1..2

        p.Spawn(p.BouncePadMega, inter, 36f, 2f);
        p.Spawn(p.DashLaneUp, inter, 42f, 7f);                           // Catapults up into Extended Glide Slalom!
        p.Spawn(p.LavaLeaper, haz, 64f, 2f);
        p.Spawn(p.BouncePad, inter, 74f, 2f);
        p.Spawn(p.LavaLeaper, haz, 102f, 2f);
        p.Spawn(p.BouncePadMega, inter, 114f, 2f);
        p.Spawn(p.LavaLeaper, haz, 142f, 2f);
        p.Spawn(p.BouncePad, inter, 152f, 2f);

        // Stalactites + Mid-Air Barrels + MovingPlatforms + Crumbling + Cannon_Fast Crossfire
        for (int x = 52; x <= 58; x++)
            for (int y = 9; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #1
        for (int x = 68; x <= 74; x++)
            for (int y = 14; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Upper Stalactite Cap
        SetAtomId(p.Spawn(p.Atom, col, 55f, 7f), 9);

        p.Spawn(p.DashCharge, inter, 62f, 11f);
        GameObject hold3A = p.Spawn(p.DashHold, inter, 68f, 10.5f);
        ConfigureDashZoneHold(hold3A, 4f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 22f, 0.22f); // -> (80.2, 11.4)!
        GameObject mp3A = p.Spawn(p.MovingPlatform, env, 74f, 6f);
        ConfigureMovingPlatform(mp3A, new Vector3(6f, 0f, 0f), 3.2f, 0.15f);
        for (int x = 82; x <= 88; x++)
            for (int y = 9; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #2
        p.Spawn(p.CrumblingBlock, env, 90f, 6.5f);

        GameObject hold3B = p.Spawn(p.DashHold, inter, 98f, 11f);
        ConfigureDashZoneHold(hold3B, -4f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 22f, 0.22f); // -> (110.2, 10.1)!
        SetAtomId(p.Spawn(p.Atom, col, 98f, 15f), 10);
        for (int x = 102; x <= 108; x++)
            for (int y = 14; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Upper Arch #3
        GameObject mp3B = p.Spawn(p.MovingPlatform, env, 106f, 6.5f);
        ConfigureMovingPlatform(mp3B, new Vector3(0f, 4f, 0f), 3.0f, 0.2f);
        for (int x = 112; x <= 118; x++)
            for (int y = 9; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #3
        GameObject can3A = p.Spawn(p.CannonFast, haz, 124f, 15.5f);
        ConfigureCannon(can3A, new Vector3(0f, 0f, -155f), 11f, 2.2f, 22f);
        p.Spawn(p.DashCharge, inter, 123f, 11f);                         // Mid-air Handoff + Cannon interlock!

        p.Spawn(p.CrumblingBlock, env, 122f, 6.5f);
        GameObject hold3C = p.Spawn(p.DashHold, inter, 130f, 10.5f);
        ConfigureDashZoneHold(hold3C, 8f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 22f, 0.22f); // -> (142.1, 12.2)!
        GameObject mp3C = p.Spawn(p.MovingPlatform, env, 138f, 6.5f);
        ConfigureMovingPlatform(mp3C, new Vector3(6f, 0f, 0f), 3.4f, 0.15f);
        for (int x = 133; x <= 139; x++)
            for (int y = 14; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Upper Arch #4
        for (int x = 144; x <= 150; x++)
            for (int y = 9; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #4

        // Secret Tier 3 Diamond 2 Vault (Upper Right Glide Ledge X=158..176, Y=11..19!)
        p.Spawn(p.DashCharge, inter, 153f, 12.5f);
        p.SpawnGroundSized(env, 158f, 11f, 18);                          // Vault Floor X=158..176, Y=10..11
        p.SpawnCeilingSized(env, 158f, 19f, 18);                         // Vault Roof 158..176, Y=18..19
        p.SpawnBlock(p.BlockCyan, env, 174f, 18f, 4, 14, 4);             // East Seal 174..176, Y=11..18 (flush!)
        for (int y = 12; y <= 18; y++) p.Spawn(p.DashWall, inter, 158f, y); // Yellow Breach Wall X=158..159!
        p.Spawn(p.FireJet, haz, 162f, 11f);
        p.Spawn(p.CrumblingBlock, env, 162f, 14f);                       // Coupled inside Vault!
        SetAtomId(p.Spawn(p.Atom, col, 164f, 12.8f), 11);
        SetDiamondId(p.Spawn(p.Diamond, col, 170f, 13f), 2);             // Secret Glide-Ledge Diamond 2!

        // Tier 3 East Express Push + Sanctuary 3 (176..198, Y=2..10)
        GameObject db3Sanct = p.Spawn(p.DashBoostFast, inter, 162f, 3.5f);
        ConfigureDashZoneBoost(db3Sanct, new Vector2(3.2f, 3.2f), 34f, 0.30f, 22f); // -> ~172.2!
        for (int x = 166; x <= 169; x++)
            for (int y = 3; y <= 8; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        GameObject can3Lower = p.Spawn(p.CannonFast, haz, 174f, 6f);
        ConfigureCannon(can3Lower, new Vector3(0f, 0f, 180f), 11f, 2.3f, 22f);
        p.Spawn(p.BouncePadMini, inter, 176f, 2f);
        p.SpawnCeilingSized(env, 176f, 10f, 22);
        p.SpawnBlock(p.BlockGreen, env, 178f, 9f, 14, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 184f, 3.5f);
        cp3.name = "Checkpoint_3";

        // Tier 3 -> Tier 4 JAGGED EASTERN CRYSTAL WELL (X = 198..218, Y = 2 down to -22)
        p.SpawnBlock(p.BlockCyan, env, 196f, 1f, 4, 26, 4);              // Well West Wall X=196..198, Y=-12..1 (open -22..-12!)
        p.Spawn(p.OneWayPlatformLarge, env, 200f, 2f);                   // 200..208
        p.Spawn(p.OneWayPlatformLarge, env, 208f, 2f);                   // 208..216
        p.Spawn(p.OneWayPlatform, env, 216f, 2f);                        // 216..220
        p.Spawn(p.CrackedBlocks4x4, inter, 201f, 0f);                    // Layer 1 (Y=-2..0)
        p.Spawn(p.CrackedBlocks4x4, inter, 205f, 0f);
        p.Spawn(p.CrackedBlocks4x4, inter, 209f, 0f);
        p.Spawn(p.CrackedBlocks4x4, inter, 213f, 0f);
        for (int x = 200; x <= 217; x++) p.Spawn(p.TouchWallBlue, inter, x, -4f); // Cushion A
        GameObject mp3Well = p.Spawn(p.MovingPlatform, env, 204f, -8f);
        ConfigureMovingPlatform(mp3Well, new Vector3(6f, 0f, 0f), 3.0f, 0.2f);
        GameObject spin3Well = p.Spawn(p.SpinArm, haz, 208f, -12f);
        ConfigureSpinArm(spin3Well, 85f);
        p.Spawn(p.DashCharge, inter, 206f, -9f);
        SetAtomId(p.Spawn(p.Atom, col, 206f, -6.5f), 12);
        for (int x = 200; x <= 216; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -18f); // Bottom Strata Y=-18..-14!

        // =====================================================================
        // TIER 4: THE ABYSSAL CORE FURNACE (Blitz RIGHT->LEFT 218 -> 34, Y=-22..-1)
        // Colossal Furnace Monoliths + Triple Yellow Seal (58..74) -> Abyssal Heart Vault!
        // =====================================================================
        p.SpawnGroundSized(env, 12f, -22f, 168);                         // Split cleanly around IceSurface(180..186)!
        p.Spawn(p.IceSurface, env, 180f, -22f);                          // 180..186
        p.SpawnGroundSized(env, 186f, -22f, 34);                         // 186..220

        // Enclosed Abyssal Vault Roof & Framing (X = 12..196, Y = -22..-1)
        for (int x = 12; x < 74; x += 20)
            p.SpawnCeilingSized(env, x, -3f, Mathf.Min(20, 74 - x));     // Vault Crown Roof Y=-4..-3
        p.SpawnBlock(p.BlockCyan, env, 10f, -3f, 4, 38, 4);              // Abyssal Heart West Wall X=10..12, Y=-22..-3 (flush!)
        p.SpawnBlock(p.BlockMagenta, env, 20f, -4f, 16, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 36f, -4f, 16, 2, 4);

        // Blitz Sequence East -> West (194 -> 76): ZERO Gaps, Dynamic Interlocked Hazards!
        p.Spawn(p.BouncePad, inter, 205f, -22f);
        GameObject db4Start = p.Spawn(p.DashBoostLeft, inter, 193f, -20.5f);
        ConfigureDashZoneBoost(db4Start, new Vector2(3.2f, 3.2f), 34f, 0.32f, 22f); // -> ~182.1 onto Ice!
        for (int x = 186; x <= 190; x++)
            for (int y = -21; y <= -14; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Gate Entry Cushion!

        GameObject hold4A = p.Spawn(p.DashHold, inter, 176f, -18f);
        ConfigureDashZoneHold(hold4A, 164f, new Vector2(3.4f, 3.4f), 36f, 0.38f, 22f, 0.22f); // -> ~(162.9, -14.2)!
        p.Spawn(p.DashCharge, inter, 156f, -13.5f);                      // Handoff right off Hold4A + LavaLeaper!
        p.Spawn(p.LavaLeaper, haz, 166f, -22f);
        GameObject mp4A = p.Spawn(p.MovingPlatform, env, 160f, -16f);
        ConfigureMovingPlatform(mp4A, new Vector3(-6f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.FireJet, haz, 154f, -22f);
        p.Spawn(p.CrumblingBlock, env, 152f, -16f);
        for (int x = 148; x <= 154; x++)
            for (int y = -12; y <= -7; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 166f, -13f), 13);
        p.Spawn(p.BouncePadMega, inter, 144f, -22f);                     // Lower bounce recovery!
        p.Spawn(p.DashLaneUp, inter, 144f, -13f);                        // Vertical catapult to Express Hold4B!

        GameObject hold4B = p.Spawn(p.DashHold, inter, 138f, -9f);
        ConfigureDashZoneHold(hold4B, 194f, new Vector2(3.4f, 3.4f), 36f, 0.40f, 22f, 0.22f); // -> ~(124.0, -12.5)!
        p.Spawn(p.DashCharge, inter, 124f, -13.5f);                      // Smooth handoff after Hold4B!
        GameObject can4Mid = p.Spawn(p.CannonFast, haz, 130f, -6f);
        ConfigureCannon(can4Mid, new Vector3(0f, 0f, -150f), 11f, 2.2f, 22f);
        for (int x = 114; x <= 122; x++)
            for (int y = -18; y <= -11; y++)
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y); // Starts -18 cleanly above Crumbling@-20!
        p.Spawn(p.CrumblingBlock, env, 118f, -20f);                      // Cleanly 1m below -18 drift & above -22!
        p.Spawn(p.BouncePadMini, inter, 112f, -22f);
        SetAtomId(p.Spawn(p.Atom, col, 114f, -14f), 14);

        // Final Dual-Layer Hyper-Blitz Into Colossal Triple Furnace Seal!
        GameObject db4Fin1 = p.Spawn(p.DashBoostLeft, inter, 102f, -20.5f);
        ConfigureDashZoneBoost(db4Fin1, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f); // -> ~86.2!
        for (int x = 90; x <= 98; x++)
            for (int y = -14; y <= -9; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // High Blitz Smash Pod!
        for (int x = 79; x <= 84; x++)
            for (int y = -15; y <= -10; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        p.Spawn(p.LavaLeaper, haz, 92f, -22f);
        p.Spawn(p.CrumblingBlock, env, 92f, -17f);                       // Coupled directly over LavaLeaper!
        p.Spawn(p.BouncePad, inter, 88f, -22f);
        p.Spawn(p.DashCharge, inter, 82f, -20.5f);

        // Terminal Hyper-Barrel + Upper Express Window -> 100% Within 17m of Yellow Fortress Seal!
        GameObject dz4Fin = p.Spawn(p.DashZone, inter, 77f, -20.5f);
        dz4Fin.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        ConfigureDashZoneBoost(dz4Fin, new Vector2(3.6f, 3.6f), 38f, 0.62f, 24f); // -> X=53.4 clean into Heart Vault!

        // Colossal Furnace Purple Front (72..74, -21..-13 -> 3x9) + 2m Air Gap + Double Yellow Chain Wall (60..70 -> all <= 17m of dz4Fin@77!)
        for (int x = 72; x <= 74; x++)
            for (int y = -21; y <= -13; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int y = -21; y <= -13; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 65f, y);                 // 65..70 (5x9 = 45)
            p.Spawn(p.DashWallChainRow1, inter, 60f, y);                 // 60..65 (5x9 = 45, 90 total Yellow chain blocks!)
        }

        // Upper Express Trajectory Window (50..78, Y = -11..-4 -> Atom 16 + Clean Drop onto End_Portal!)
        p.Spawn(p.OneWayPlatformLarge, env, 66f, -11f);                  // Catch 66..74
        GameObject hold4Exp = p.Spawn(p.DashHold, inter, 74f, -8.5f);
        ConfigureDashZoneHold(hold4Exp, 180f, new Vector2(3.4f, 3.4f), 36f, 0.56f, 22f, 0.22f); // Fires LEFT along upper Y=-8.5 to X=53.8!
        for (int x = 67; x <= 69; x++)
            for (int y = -10; y <= -5; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Express Purple Break!
        for (int y = -10; y <= -5; y++) p.Spawn(p.DashWallChainRow1, inter, 60f, y); // Express Yellow Window (60..65)!
        SetAtomId(p.Spawn(p.Atom, col, 80f, -18.5f), 15);
        SetAtomId(p.Spawn(p.Atom, col, 52f, -8.5f), 16);                 // Upper Express Final Atom!
        SetDiamondId(p.Spawn(p.Diamond, col, 48f, -20.2f), 3);           // Heart Vault Crown Diamond 3!

        EnforceZZero(env);
        EnforceZZero(inter);
        EnforceZZero(haz);
        EnforceZZero(col);
        EnforceZZero(cp);
        EnforceZZero(flow);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        int errors = ElementoLevelValidator.Run();
        return $"Level_47 built at {TargetScenePath} (Validator errors: {errors})";
    }

    private static Transform SetupLevelCore()
    {
        GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Managers/LevelCore.prefab");
        GameObject coreObj = (GameObject)PrefabUtility.InstantiatePrefab(corePrefab);
        coreObj.name = "LevelCore";
        coreObj.transform.position = Vector3.zero;

        var vfxList = Object.FindObjectsByType<Vfx>(FindObjectsSortMode.None);
        for (int i = 1; i < vfxList.Length; i++) Object.DestroyImmediate(vfxList[i].gameObject);

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

        var lm = Object.FindFirstObjectByType<LevelManager>();
        if (lm != null)
        {
            lm.ThreeStarTime = 75f;
            lm.TwoStarTime = 115f;
            lm.OneStarTime = 190f;
            EditorUtility.SetDirty(lm);
            PrefabUtility.RecordPrefabInstancePropertyModifications(lm);
        }

        return coreObj.transform;
    }

    private static void WireCoreAndPortals(Transform levelCore, GameObject startPortal, GameObject endPortal)
    {
        var cc = levelCore.GetComponentInChildren<CC_ELEMENTO>(true);
        if (cc != null && startPortal != null)
        {
            cc.transform.position = startPortal.transform.position;
            EditorUtility.SetDirty(cc.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(cc.transform);
        }

        var respawnMgr = Object.FindFirstObjectByType<RespawnManager>();
        if (respawnMgr != null && startPortal != null)
        {
            respawnMgr.SpawnPoint = startPortal.transform;
            respawnMgr.KillY = -34f;
            respawnMgr.KillTopY = 76f;
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
        if (box != null)
        {
            box.size = size;
            EditorUtility.SetDirty(box);
        }
    }

    private static void SetTriggerBoxSize(GameObject go, Vector3 size)
    {
        var box = go.GetComponent<BoxCollider>();
        if (box != null)
        {
            box.isTrigger = true;
            box.size = size;
            EditorUtility.SetDirty(box);
        }
    }

    private static void ConfigureDashZoneBoost(GameObject go, Vector2 size, float speed, float time, float exitSpeed)
    {
        var dz = go.GetComponent<DashZone>();
        if (dz != null)
        {
            dz.Mode = DashZone.ZoneMode.Boost;
            dz.Size = size;
            dz.BoostSpeed = speed;
            dz.BoostTime = time;
            dz.ExitSpeed = exitSpeed;
            dz.Apply();
            EditorUtility.SetDirty(dz);
            PrefabUtility.RecordPrefabInstancePropertyModifications(dz);
        }
    }

    private static void ConfigureDashZoneHold(GameObject go, float angleDeg, Vector2 size, float launchSpeed, float launchTime, float exitSpeed, float maxTime)
    {
        go.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        var dz = go.GetComponent<DashZone>();
        if (dz != null)
        {
            dz.Mode = DashZone.ZoneMode.Hold;
            dz.Size = size;
            dz.HoldLaunchSpeed = launchSpeed;
            dz.HoldLaunchTime = launchTime;
            dz.HoldExitSpeed = exitSpeed;
            dz.HoldMaxTime = maxTime;
            dz.Apply();
            EditorUtility.SetDirty(dz);
            PrefabUtility.RecordPrefabInstancePropertyModifications(dz);
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

    private static void ConfigureMovingPlatform(GameObject plat, Vector3 travel, float speed, float pauseAtEnds)
    {
        var mp = plat.GetComponent<MovingPlatform>();
        if (mp != null)
        {
            mp.Travel = travel;
            mp.Speed = speed;
            mp.PauseAtEnds = pauseAtEnds;
            EditorUtility.SetDirty(mp);
            PrefabUtility.RecordPrefabInstancePropertyModifications(mp);
        }
    }

    private static void ConfigureCannon(GameObject cannonGo, Vector3 eulerRot, float speed, float interval, float range)
    {
        cannonGo.transform.rotation = Quaternion.Euler(eulerRot);
        var c = cannonGo.GetComponent<Cannon>();
        if (c != null)
        {
            c.Speed = speed;
            c.Interval = interval;
            c.Range = range;
            EditorUtility.SetDirty(c);
            PrefabUtility.RecordPrefabInstancePropertyModifications(c);
        }
    }

    private static void ConfigureSaw(GameObject sawGo, Vector3 travel, float speed)
    {
        var s = sawGo.GetComponent<RotatingSaw>();
        if (s != null)
        {
            s.Travel = travel;
            s.Speed = speed;
            EditorUtility.SetDirty(s);
            PrefabUtility.RecordPrefabInstancePropertyModifications(s);
        }
    }

    private static void ConfigureSpinArm(GameObject spinArmGo, float spinSpeed)
    {
        var sa = spinArmGo.GetComponentInChildren<SpinArm>(true);
        if (sa != null)
        {
            sa.SpinSpeed = spinSpeed;
            EditorUtility.SetDirty(sa);
            PrefabUtility.RecordPrefabInstancePropertyModifications(sa);
        }
    }

    private static void ConfigureStompBlock(GameObject stompGo, float delay)
    {
        var sb = stompGo.GetComponentInChildren<StompBlock>(true);
        if (sb != null)
        {
            sb.Delay = delay;
            EditorUtility.SetDirty(sb);
            PrefabUtility.RecordPrefabInstancePropertyModifications(sb);
        }
    }

    private static void EnforceZZero(Transform container)
    {
        foreach (Transform t in container.GetComponentsInChildren<Transform>(true))
        {
            Vector3 pos = t.position;
            if (Mathf.Abs(pos.z) > 0.001f)
                t.position = new Vector3(pos.x, pos.y, 0f);
        }
    }

    private class PrefabLib
    {
        public GameObject Ground20 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ground_20x1.prefab");
        public GameObject Ceiling20 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ceiling_20x1.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject BlockGreen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Green Variant.prefab");
        public GameObject BlockMagenta = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject OneWayPlatformLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject CrumblingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/CrumblingBlock.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject DashWallChainRow1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall_ChainRow 1.prefab");
        public GameObject DashWallCracked = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall_Cracked.prefab");
        public GameObject TouchWallBlue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/TouchWall_Blue.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject GroundPoundChainRow2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/GroundPoundChainRow 2.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMini = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mini.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashBoostLeft = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Left.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashHold = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashHold.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");

        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject CannonFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Cannon_Fast.prefab");
        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject LavaLeaper = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/LavaLeaper.prefab");
        public GameObject StompBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/StompBlock.prefab");
        public GameObject SawSlow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Saw_Slow.prefab");

        public GameObject Atom = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Atom.prefab");
        public GameObject Diamond = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Diamond.prefab");
        public GameObject Checkpoint = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/CheckPoint.prefab");
        public GameObject StartPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/StartPortal.prefab");
        public GameObject EndPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/End_Portal.prefab");
        public GameObject CameraZoneWide = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Wide.prefab");
        public GameObject CameraZoneTight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Tight.prefab");
        public GameObject CameraZoneClimb = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Flow/Variants/CameraZone_Climb.prefab");

        public GameObject Spawn(GameObject prefab, Transform parent, float x, float y)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.position = new Vector3(x, y, 0f);
            return instance;
        }

        public GameObject SpawnBlock(GameObject blockPrefab, Transform parent, float x, float y, int sizeX, int sizeY, int sizeZ)
        {
            GameObject go = Spawn(blockPrefab, parent, x, y);
            NeonBlock nb = go.GetComponent<NeonBlock>();
            if (nb != null)
            {
                nb.Size = new Vector3Int(sizeX, sizeY, sizeZ);
                nb.Apply();
                EditorUtility.SetDirty(nb);
                PrefabUtility.RecordPrefabInstancePropertyModifications(nb);
            }
            return go;
        }

        public GameObject SpawnGroundSized(Transform parent, float x, float y, int width)
        {
            return SpawnBlock(Ground20, parent, x, y, width, 1, 2);
        }

        public GameObject SpawnCeilingSized(Transform parent, float x, float y, int width)
        {
            return SpawnBlock(Ceiling20, parent, x, y, width, 1, 2);
        }
    }
}
#endif
