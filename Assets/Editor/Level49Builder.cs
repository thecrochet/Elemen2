#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level49Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_49.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 49 (Sky-Shatter Rollercoaster)")]
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
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES (Rollercoaster X = 0..388)
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 194f, -22f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(480f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(480f, 4f, 1f);

        // Continuous Bedrock Base at Y = -16 (X = -8 to 396)
        for (int x = -8; x < 396; x += 20)
            p.SpawnGroundSized(env, x, -16f, Mathf.Min(20, 396 - x));

        // West & East End Walls + Extreme Rollercoaster Stepped Roof (38 -> 46 -> 12 -> 44)
        p.SpawnBlock(p.BlockCyan, env, 0f, 38f, 4, 108, 4);                // West Citadel Wall X=0..2, Y=-16..38
        p.SpawnBlock(p.BlockCyan, env, 386f, 44f, 4, 120, 4);             // East Citadel Wall X=386..388, Y=-16..44

        for (int x = 0; x < 92; x += 20)
            p.SpawnCeilingSized(env, x, 39f, Mathf.Min(20, 92 - x));      // Act 1 Climbing Roof X=0..92, Y=38..39
        p.SpawnBlock(p.BlockCyan, env, 92f, 46f, 4, 16, 4);               // Step Up X=92..94, Y=38..46
        for (int x = 94; x < 204; x += 20)
            p.SpawnCeilingSized(env, x, 47f, Mathf.Min(20, 204 - x));     // Act 2 High Cathedral Roof X=94..204, Y=46..47
        p.SpawnBlock(p.BlockCyan, env, 204f, 46f, 4, 28, 4);              // Step Down X=204..206, Y=32..46
        for (int x = 206; x < 220; x += 14)
            p.SpawnCeilingSized(env, x, 33f, Mathf.Min(14, 220 - x));     // Plunge Mouth Cap X=206..220, Y=32..33
        p.SpawnBlock(p.BlockCyan, env, 219f, 32f, 2, 40, 4);              // East Plunge Wall X=219..220, Y=12..32
        for (int x = 220; x < 296; x += 20)
            p.SpawnCeilingSized(env, x, 13f, Mathf.Min(20, 296 - x));     // Act 3 Pressurized Trench Roof X=220..296, Y=12..13!
        p.SpawnBlock(p.BlockCyan, env, 296f, 44f, 4, 64, 4);              // Zenith Eruption Step Up X=296..298, Y=12..44
        for (int x = 298; x < 388; x += 20)
            p.SpawnCeilingSized(env, x, 45f, Mathf.Min(20, 388 - x));     // Act 4 Zenith Roof X=298..388, Y=44..45

        // Architectural Neon Trim Ribs
        p.SpawnBlock(p.BlockGreen, env, 24f, 38f, 20, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 128f, 46f, 24, 4, 4);
        p.SpawnBlock(p.BlockGreen, env, 232f, 12f, 18, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 328f, 44f, 24, 4, 4);

        // =====================================================================
        // FLOW: LOW IGNITION START -> ZENITH FINALE BASTION END PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 7f, 2f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 382f, 13.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (8m+ Early Lead Ahead of Steep Climbs & Dive-Bombs)
        // =====================================================================
        GameObject cz1Tight = p.Spawn(p.CameraZoneTight, flow, 13f, 8f);
        cz1Tight.name = "CameraZone_Tight_Act1IgnitionBay";
        SetTriggerBoxSize(cz1Tight, new Vector3(26f, 16f, 10f));        // X=0..26, Y=0..16

        GameObject cz1Climb = p.Spawn(p.CameraZoneClimb, flow, 59f, 19f);
        cz1Climb.name = "CameraZone_Climb_Act1SkyClimb";
        SetTriggerBoxSize(cz1Climb, new Vector3(84f, 38f, 10f));        // X=17..101, Y=0..38

        GameObject cz2Wide = p.Spawn(p.CameraZoneWide, flow, 146f, 30f);
        cz2Wide.name = "CameraZone_Wide_Act2CathedralAttic";
        SetTriggerBoxSize(cz2Wide, new Vector3(114f, 34f, 10f));        // X=89..203, Y=13..47

        GameObject cz3Climb = p.Spawn(p.CameraZoneClimb, flow, 211f, 9f);
        cz3Climb.name = "CameraZone_Climb_Act3DiveChute";
        SetTriggerBoxSize(cz3Climb, new Vector3(30f, 46f, 10f));         // X=196..226, Y=-14..32 (10m early plunge lead!)

        GameObject cz3Tight = p.Spawn(p.CameraZoneTight, flow, 255f, 0f);
        cz3Tight.name = "CameraZone_Tight_Act3SubTrench";
        SetTriggerBoxSize(cz3Tight, new Vector3(82f, 28f, 10f));         // X=214..296, Y=-14..14

        GameObject cz4Climb = p.Spawn(p.CameraZoneClimb, flow, 310f, 11f);
        cz4Climb.name = "CameraZone_Climb_Act4ZenithEruption";
        SetTriggerBoxSize(cz4Climb, new Vector3(44f, 50f, 10f));         // X=288..332, Y=-14..36

        GameObject cz4Wide = p.Spawn(p.CameraZoneWide, flow, 353f, 22f);
        cz4Wide.name = "CameraZone_Wide_Act4ZenithFinale";
        SetTriggerBoxSize(cz4Wide, new Vector3(76f, 46f, 10f));         // X=315..391, Y=-1..45

        // =====================================================================
        // ACT 1: THE UPDRAFT LAUNCH BAY (X = 2..94, Y = 2 -> 24)
        // Low Start -> Slope_45_Large -> Upper-Left Atom 1 -> Skyward Climb!
        // =====================================================================
        p.SpawnGroundSized(env, 2f, 2f, 54);                             // Act 1 Low Safety Floor X=2..56, Y=1..2
        p.SpawnGroundSized(env, 56f, 12f, 22);                           // Mid Step-Up Cushion X=56..78, Y=11..12
        p.SpawnGroundSized(env, 78f, 24f, 18);                           // Sanctuary 1 Terrace X=78..96, Y=23..24
        p.SpawnCeilingSized(env, 2f, 12f, 16);                           // Ignition Overhang X=2..18, Y=11..12
        p.SpawnBlock(p.BlockGreen, env, 4f, 11f, 16, 2, 4);
        foreach (float px in new[] { 18f, 38f, 54f, 76f, 92f })
            p.SpawnBlock(p.BlockCyan, env, px, 1f, 4, 34, 4);            // Bedrock piers Y=-16..1 (and clean under step-ups)

        // Launch #1 -> Purple Wave -> Slope45Large -> Mega Bounce -> Early Upper-Left Alcove Atom 1!
        GameObject db1A = p.Spawn(p.DashBoostFast, inter, 13f, 3.5f);
        ConfigureDashZoneBoost(db1A, new Vector2(3.2f, 3.2f), 32f, 0.24f, 22f); // -> ~20.7 before Slope@22!
        for (int x = 16; x <= 20; x++)
            for (int y = 3; y <= 7; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.Slope45Large, env, 24f, 2f);                           // 22..26, Y=2..6 rises UP-RIGHT!
        p.Spawn(p.DashCharge, inter, 26f, 7.5f);                         // Mid-ramp air dash bridge -> BouncePadMega!
        p.SpawnGroundSized(env, 26f, 6f, 8);                             // 26..34, Y=5..6
        p.Spawn(p.BouncePadMega, inter, 30f, 6f);
        // Upper-Left Secret Alcove off Bounce Apex (17..25, Y=15)
        p.Spawn(p.OneWayPlatformLarge, env, 17f, 15f);                   // 17..25
        for (int x = 18; x <= 20; x++)
            for (int y = 16; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 22f, 17f), 1);                    // Early Upper-Left Alcove Atom 1!

        // Updraft Climb Chain (30 -> 80) weaving FireJets -> Yellow Cracked Sky Hatch -> Sanctuary 1
        p.Spawn(p.DashLaneUp, inter, 30f, 12f);
        GameObject hold1A = p.Spawn(p.DashHold, inter, 34f, 16f);
        ConfigureDashZoneHold(hold1A, 40f, new Vector2(3.6f, 3.6f), 34f, 0.36f, 22f, 0.22f); // -> ~(43.4, 23.9)!
        for (int x = 37; x <= 42; x++)
            for (int y = 17; y <= 24; y++)
                if (Mathf.Abs(y - (16f + (x - 34f) * 0.84f)) <= 2.2f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Diagonal Purple Arch #1

        p.Spawn(p.OneWayPlatformLarge, env, 41f, 22f);                   // Catch 41..49
        p.Spawn(p.FireJet, haz, 48f, 22f);
        p.Spawn(p.DashCharge, inter, 46f, 24.5f);
        p.Spawn(p.CrumblingBlock, env, 48f, 25.5f);                      // Coupled directly over FireJet!

        GameObject hold1B = p.Spawn(p.DashHold, inter, 52f, 23.5f);
        ConfigureDashZoneHold(hold1B, 14f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 22f, 0.22f); // -> ~(67.4, 27.3)!
        for (int x = 56; x <= 59; x++)
            for (int y = 23; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m air gap before Yellow Hatch!
        for (int x = 62; x <= 64; x++)
            for (int y = 24; y <= 30; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);   // Yellow Cracked Sky Hatch!
        p.Spawn(p.OneWayPlatformLarge, env, 65f, 26f);                   // Catch 65..73
        p.Spawn(p.BouncePadMini, inter, 68f, 26f);
        SetAtomId(p.Spawn(p.Atom, col, 67f, 29f), 2);

        GameObject db1Fin = p.Spawn(p.DashBoostFast, inter, 71f, 27.5f);
        ConfigureDashZoneBoost(db1Fin, new Vector2(3.2f, 3.2f), 32f, 0.30f, 22f); // -> ~80.6 cleanly onto Sanctuary 1!
        for (int x = 74; x <= 77; x++)
            for (int y = 25; y <= 31; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 75.5f, 32.5f), 3);

        // Act 1 Terraced Recovery Cushions (Y=2 & Y=12)
        p.Spawn(p.BouncePadMega, inter, 40f, 2f);
        p.Spawn(p.BouncePadMega, inter, 52f, 2f);
        p.Spawn(p.FireJet, haz, 46f, 2f);
        p.Spawn(p.CrumblingBlock, env, 46f, 5.5f);
        p.Spawn(p.BouncePadMega, inter, 62f, 12f);
        p.Spawn(p.FireJet, haz, 68f, 12f);
        p.Spawn(p.BouncePadMega, inter, 74f, 12f);
        p.Spawn(p.DashCharge, inter, 74f, 20f);
        p.Spawn(p.DashCharge, inter, 81.5f, 27f);

        p.SpawnCeilingSized(env, 79f, 34f, 14);                          // Sanctuary 1 Alcove Roof
        p.SpawnBlock(p.BlockMagenta, env, 80f, 33f, 14, 2, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 86f, 25.5f);
        cp1.name = "Checkpoint_1";
        GameObject db1To2 = p.Spawn(p.DashBoostFast, inter, 91f, 25.5f);
        ConfigureDashZoneBoost(db1To2, new Vector2(2.8f, 2.8f), 32f, 0.14f, 20f); // -> ~95.5 right into Act 2 cascade!

        // =====================================================================
        // ACT 2: THE RAFTER RICOCHET WAVE (X = 94..204, Y = 18..46)
        // High-Altitude Aerial Coaster, Glide Slalom & Secret Rafter Pocket (Diamond 1!)
        // =====================================================================
        p.Spawn(p.Slope45Left, env, 97f, 22f);                           // 96..98, 24..22
        p.Spawn(p.Slope45Left, env, 99f, 20f);                           // 98..100, 22..20
        p.Spawn(p.Slope45Left, env, 101f, 18f);                          // 100..102, 20..18
        p.SpawnGroundSized(env, 102f, 18f, 92);                          // Continuous Attic Safety Floor X=102..194, Y=17..18!
        p.SpawnBlock(p.BlockCyan, env, 102f, 17f, 4, 66, 4);
        p.SpawnBlock(p.BlockCyan, env, 148f, 17f, 4, 66, 4);

        p.Spawn(p.BouncePadMini, inter, 94f, 24f);                       // Closes Act 1 -> Act 2 X-gap!
        GameObject db2Start = p.Spawn(p.DashBoostFast, inter, 104f, 20f);
        ConfigureDashZoneBoost(db2Start, new Vector2(3.2f, 3.2f), 34f, 0.28f, 22f); // -> ~113.5 into Hold2A!
        for (int x = 107; x <= 111; x++)
            for (int y = 20; y <= 26; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Hanging Chandelier #1

        // Hold 2A (32 deg UP-RIGHT) -> Checkerboard -> Moving Shuttle mp2A under SpinArm -> Hold 2B Glide Dive!
        GameObject hold2A = p.Spawn(p.DashHold, inter, 114f, 23f);
        ConfigureDashZoneHold(hold2A, 32f, new Vector2(3.6f, 3.6f), 36f, 0.40f, 22f, 0.22f); // -> ~(126.2, 30.6)!
        for (int x = 118; x <= 124; x++)
            for (int y = 24; y <= 33; y++)
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y); // Checkerboard Cloud!

        GameObject mp2A = p.Spawn(p.MovingPlatform, env, 128f, 29f);
        ConfigureMovingPlatform(mp2A, new Vector3(5f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.DashCharge, inter, 131f, 31f);
        GameObject spin2A = p.Spawn(p.SpinArm, haz, 130f, 35.5f);
        ConfigureSpinArm(spin2A, 95f);
        SetAtomId(p.Spawn(p.Atom, col, 130f, 38.5f), 4);
        p.SpawnGroundSized(env, 136f, 29f, 5);                           // Take-off Pier 136..141, Y=28..29
        p.Spawn(p.BouncePad, inter, 138f, 29f);

        // Hold 2B (-22 deg Glide Dive) through Giant Hanging Curtain & Cannon_Fast -> mp2B!
        GameObject hold2B = p.Spawn(p.DashHold, inter, 139f, 34f);
        ConfigureDashZoneHold(hold2B, -22f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 22f, 0.22f); // -> ~(153.7, 28.1)!
        for (int x = 143; x <= 148; x++)
            for (int y = 26; y <= 36; y++)
                if (Mathf.Abs(y - (34f - (x - 139f) * 0.40f)) <= 2.8f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Wave Curtain along dive cone!
        p.Spawn(p.DashCharge, inter, 148f, 30f);
        GameObject can2A = p.Spawn(p.CannonFast, haz, 155f, 38f);
        ConfigureCannon(can2A, new Vector3(0f, 0f, -145f), 11f, 2.2f, 22f);
        GameObject mp2B = p.Spawn(p.MovingPlatform, env, 154f, 26f);
        ConfigureMovingPlatform(mp2B, new Vector3(0f, 5f, 0f), 3.2f, 0.15f);

        // Optional Upper Rafter Pocket (Secret Atom 5 + Diamond 1 behind Yellow DashWall @ 166 -> Express Hold Exit!)
        GameObject hold2Rafter = p.Spawn(p.DashHold, inter, 159f, 33f);
        ConfigureDashZoneHold(hold2Rafter, 14f, new Vector2(3.4f, 3.4f), 36f, 0.40f, 22f, 0.22f); // -> ~(173.0, 36.5)!
        p.Spawn(p.OneWayPlatformLarge, env, 166f, 34f);                  // Secret Rafter Floor 166..174
        for (int y = 35; y <= 40; y++) p.Spawn(p.DashWall, inter, 166f, y); // Yellow Breach Wall X=166..167!
        SetDiamondId(p.Spawn(p.Diamond, col, 170f, 36f), 1);             // Secret Rafter Diamond 1!
        SetAtomId(p.Spawn(p.Atom, col, 173.5f, 36f), 5);
        GameObject hold2RafterExit = p.Spawn(p.DashHold, inter, 176f, 34.5f);
        ConfigureDashZoneHold(hold2RafterExit, -22f, new Vector2(3.2f, 3.2f), 34f, 0.42f, 22f, 0.22f); // -> ~(189.2, 29.2) through top of Bulkhead onto Sanct 2!

        // Main Attic Coaster Wave (159 -> 195) -> Yellow Bulkhead -> Sanctuary 2 (194..206, Y=24)
        GameObject hold2C = p.Spawn(p.DashHold, inter, 159f, 24.5f);
        ConfigureDashZoneHold(hold2C, -8f, new Vector2(3.6f, 3.6f), 36f, 0.42f, 22f, 0.22f); // -> ~(174.0, 22.4)!
        for (int x = 163; x <= 167; x++)
            for (int y = 21; y <= 28; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Forest Pillar #1!
        p.Spawn(p.CrumblingBlock, env, 178f, 23.5f);
        GameObject spin2B = p.Spawn(p.SpinArm, haz, 182f, 29f);
        ConfigureSpinArm(spin2B, -95f);

        GameObject db2End = p.Spawn(p.DashBoostFast, inter, 181f, 25.5f);
        ConfigureDashZoneBoost(db2End, new Vector2(3.2f, 3.2f), 36f, 0.46f, 24f); // -> ~197.6 cleanly onto Sanct 2!
        for (int x = 185; x <= 187; x++)
            for (int y = 24; y <= 30; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m clear gap!
        for (int y = 24; y <= 30; y++)
            p.Spawn(p.DashWallChainRow1, inter, 189f, y);                // Yellow Attic Bulkhead (189..194)!

        // Attic Safety BouncePads (Y=18)
        p.Spawn(p.BouncePadMega, inter, 118f, 18f);
        p.Spawn(p.CrumblingBlock, env, 126f, 21f);
        p.Spawn(p.BouncePadMega, inter, 134f, 18f);
        p.Spawn(p.FireJet, haz, 144f, 18f);
        p.Spawn(p.BouncePadMega, inter, 152f, 18f);
        SetAtomId(p.Spawn(p.Atom, col, 144f, 22f), 6);
        p.Spawn(p.BouncePadMega, inter, 175f, 18f);
        p.Spawn(p.DashCharge, inter, 175f, 24f);

        // Sanctuary 2 Terrace (194..206, Y=24) right before Dive-Bomb Plunge Mouth!
        p.SpawnGroundSized(env, 194f, 24f, 12);                          // Floor 194..206, Y=23..24
        p.SpawnBlock(p.BlockCyan, env, 194f, 23f, 4, 78, 4);             // West Wall of Act 3 Shaft!
        p.SpawnCeilingSized(env, 194f, 34f, 10);                         // 194..204 (flush with BlockCyan@204..206!)
        p.SpawnBlock(p.BlockGreen, env, 195f, 33f, 14, 2, 4);
        p.Spawn(p.DashCharge, inter, 195.5f, 26.5f);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 199f, 25.5f);
        cp2.name = "Checkpoint_2";
        p.Spawn(p.BouncePadMini, inter, 203.5f, 24f);

        // =====================================================================
        // ACT 3: THE SUPERSONIC DIVE-BOMB TRENCH (Plunge 206..220 -> Blitz 220..296)
        // 36m Ground Pound Dive -> Pressurized Y=-12..+12 Speed Trench + Secret Vault (Diamond 2!)
        // =====================================================================
        p.Spawn(p.OneWayPlatformLarge, env, 206f, 24f);                  // Drop Mouth 206..214
        p.Spawn(p.OneWayPlatform, env, 214f, 24f);                       // 214..218
        p.Spawn(p.DashCharge, inter, 209f, 23f);                         // Plunge Mouth Air-Dash Recharge!
        p.Spawn(p.CrackedBlocks4x4, inter, 207f, 22f);                   // Layer 1 (20..22)
        p.Spawn(p.CrackedBlocks4x4, inter, 211f, 22f);
        p.Spawn(p.CrackedBlocks4x4, inter, 215f, 22f);
        for (int x = 207; x <= 218; x++) p.Spawn(p.TouchWallBlue, inter, x, 17f); // Cushion A
        for (int x = 207; x <= 218; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 9f); // Strata 9..13!
        for (int x = 207; x <= 218; x++) p.Spawn(p.TouchWallBlue, inter, x, 5f);  // Cushion B
        p.Spawn(p.CrackedBlocks4x4, inter, 207f, 1f);                    // Layer 2 (-1..1)
        p.Spawn(p.CrackedBlocks4x4, inter, 211f, 1f);
        p.Spawn(p.CrackedBlocks4x4, inter, 215f, 1f);
        for (int x = 207; x <= 218; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -8f); // Bottom Seal -8..-4!
        p.Spawn(p.DashCharge, inter, 213f, 2f);
        SetAtomId(p.Spawn(p.Atom, col, 213f, -10.5f), 7);

        // Pressurized Sub-Trench Floor (Split cleanly around IceSurface 228..234 @ Y=-12!)
        p.SpawnGroundSized(env, 196f, -12f, 32);                         // X=196..228, Y=-13..-12
        p.Spawn(p.IceSurface, env, 228f, -12f);                          // Ice Runway 228..234
        p.SpawnGroundSized(env, 234f, -12f, 64);                         // X=234..298, Y=-13..-12

        p.Spawn(p.BouncePad, inter, 222f, -12f);                         // Smooth transition out of plunge!
        GameObject db3Entry = p.Spawn(p.DashBoostFast, inter, 230f, -10.5f);
        ConfigureDashZoneBoost(db3Entry, new Vector2(3.2f, 3.2f), 36f, 0.34f, 22f); // -> X=242.2!
        for (int x = 234; x <= 238; x++)
            for (int y = -11; y <= -4; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Trench Thicket #1!

        p.Spawn(p.BouncePadMini, inter, 243f, -12f);                     // Exact endpoint handoff!
        p.Spawn(p.CrumblingBlock, env, 246f, -8.5f);                     // Coupled under Stomp3A!
        GameObject stomp3A = p.Spawn(p.StompBlock, haz, 246f, 1f);
        ConfigureStompBlock(stomp3A, 0.28f);
        SetAtomId(p.Spawn(p.Atom, col, 246f, -5.5f), 8);

        // High-Speed Trench Blast #2 -> Bulkhead #1 (256..261) + Secret Overhead Ceiling Vault (Diamond 2!)
        GameObject dz3Mid = p.Spawn(p.DashZone, inter, 248f, -10.5f);
        ConfigureDashZoneBoost(dz3Mid, new Vector2(3.2f, 3.2f), 36f, 0.40f, 24f); // -> X=262.4!
        p.Spawn(p.LavaLeaper, haz, 252f, -12f);
        for (int x = 252; x <= 253; x++)
            for (int y = -11; y <= -4; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m clear gap!
        for (int y = -11; y <= -4; y++)
            p.Spawn(p.DashWallChainRow1, inter, 256f, y);                // Trench Yellow Bulkhead #1 (256..261)!

        GameObject saw3A = p.Spawn(p.SawSlow, haz, 263f, -8f);
        ConfigureSaw(saw3A, new Vector3(0f, 4.5f, 0f), 3.0f);
        p.Spawn(p.BouncePadMini, inter, 264f, -12f);

        // Secret Overhead Diamond 2 Chamber (255..273, Y=1..12) via Upward DashHold @ (264, -5)!
        GameObject hold3Secret = p.Spawn(p.DashHold, inter, 264f, -5f);
        ConfigureDashZoneHold(hold3Secret, 82f, new Vector2(3.2f, 3.2f), 34f, 0.30f, 20f, 0.24f); // Shoots clean UP through DashWall@262..265!
        for (int x = 262; x <= 265; x++) p.Spawn(p.DashWall, inter, x, 2f); // Yellow Ceiling Breach X=262..266!
        p.SpawnGroundSized(env, 255f, 2f, 7);                            // Left Secret Floor 255..262
        p.SpawnGroundSized(env, 266f, 2f, 6);                            // Right Secret Floor 266..272 (flush with BlockCyan@272..273!)
        p.SpawnBlock(p.BlockCyan, env, 254f, 11f, 2, 20, 4);             // West Wall 254..255, Y=1..11
        p.SpawnBlock(p.BlockCyan, env, 272f, 11f, 2, 20, 4);             // East Wall 272..273, Y=1..11
        p.Spawn(p.DashCharge, inter, 265f, 4.5f);
        p.Spawn(p.FireJet, haz, 268f, 2f);
        p.Spawn(p.CrumblingBlock, env, 268f, 5.5f);                      // Coupled over FireJet!
        SetAtomId(p.Spawn(p.Atom, col, 258f, 4.5f), 9);
        SetDiamondId(p.Spawn(p.Diamond, col, 270f, 4.5f), 2);            // Secret Overhead Trench Diamond 2!

        // Hold 3B -> Bulkhead #2 (276..281) -> Sanctuary 3 (284..298, Y=-12..-2)
        GameObject hold3B = p.Spawn(p.DashHold, inter, 268f, -7.5f);
        ConfigureDashZoneHold(hold3B, 6f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 24f, 0.22f); // -> ~(283.0, -5.9)!
        for (int x = 272; x <= 273; x++)
            for (int y = -11; y <= -3; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m gap!
        for (int y = -11; y <= -3; y++)
            p.Spawn(p.DashWallChainRow1, inter, 276f, y);                // Trench Yellow Bulkhead #2 (276..281)!
        GameObject stomp3B = p.Spawn(p.StompBlock, haz, 275f, 0f);
        ConfigureStompBlock(stomp3B, 0.30f);
        p.Spawn(p.LavaLeaper, haz, 279f, -12f);
        p.Spawn(p.CrumblingBlock, env, 283f, -9f);
        SetAtomId(p.Spawn(p.Atom, col, 283f, -5.5f), 10);

        p.Spawn(p.BouncePadMini, inter, 285f, -12f);
        p.SpawnCeilingSized(env, 284f, -2f, 12);
        p.SpawnBlock(p.BlockMagenta, env, 285f, -3f, 12, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 290f, -10.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ACT 4: THE ZENITH PINBALL SLINGSHOT (X = 296..386, Y = -12 -> +44)
        // Multi-Vector Crumbling Island Finale -> Triple Bastion (End_Portal@382, 13.8!)
        // =====================================================================
        p.SpawnGroundSized(env, 298f, -12f, 26);                         // Eruption Base X=298..324, Y=-13..-12
        p.SpawnGroundSized(env, 324f, 0f, 24);                           // Mid Zenith Step X=324..348, Y=-1..0
        p.SpawnBlock(p.BlockCyan, env, 324f, -1f, 4, 30, 4);
        p.SpawnGroundSized(env, 348f, 12f, 38);                          // Finale Sanctuary Deck X=348..386, Y=11..12!
        p.SpawnBlock(p.BlockCyan, env, 348f, 11f, 4, 54, 4);
        p.SpawnBlock(p.BlockCyan, env, 370f, 11f, 4, 54, 4);

        // Vector #1 (UP -> 40 deg UP-RIGHT): BouncePadMega -> DashLane_UP -> Hold 4A -> Crumbling + MovingPlat!
        p.Spawn(p.BouncePadMini, inter, 294f, -12f);
        p.Spawn(p.BouncePadMega, inter, 302f, -12f);
        p.Spawn(p.DashLaneUp, inter, 302f, -2f);
        p.Spawn(p.LavaLeaper, haz, 308f, -12f);
        p.Spawn(p.BouncePadMega, inter, 314f, -12f);

        GameObject hold4A = p.Spawn(p.DashHold, inter, 302f, 10f);
        ConfigureDashZoneHold(hold4A, 40f, new Vector2(3.6f, 3.6f), 36f, 0.42f, 22f, 0.22f); // -> ~(313.6, 19.7)!
        for (int x = 306; x <= 311; x++)
            for (int y = 12; y <= 20; y++)
                if (Mathf.Abs(y - (10f + (x - 302f) * 0.84f)) <= 2.4f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Up-Right Purple Arch!
        SetAtomId(p.Spawn(p.Atom, col, 309f, 21.5f), 11);
        p.Spawn(p.CrumblingBlock, env, 315f, 19f);
        p.Spawn(p.DashCharge, inter, 319f, 21.5f);
        GameObject mp4A = p.Spawn(p.MovingPlatform, env, 321f, 19f);
        ConfigureMovingPlatform(mp4A, new Vector3(4f, 0f, 0f), 3.4f, 0.15f);

        // Vector #2 (70 deg STEEP-UP -> Diamond 3 Sky Peak -> Vector #3 (-36 deg DIVE-RIGHT)!)
        GameObject hold4B = p.Spawn(p.DashHold, inter, 325f, 22f);
        ConfigureDashZoneHold(hold4B, 70f, new Vector2(3.6f, 3.6f), 36f, 0.34f, 22f, 0.22f); // -> ~(329.2, 33.5)!
        GameObject spin4 = p.Spawn(p.SpinArm, haz, 329f, 27.5f);
        ConfigureSpinArm(spin4, 95f);
        for (int x = 326; x <= 329; x++)
            for (int y = 26; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Sky Pillar Smash!
        SetDiamondId(p.Spawn(p.Diamond, col, 326f, 35.5f), 3);           // Zenith Rollercoaster Peak Diamond 3!
        SetAtomId(p.Spawn(p.Atom, col, 332f, 36.5f), 12);

        GameObject hold4C = p.Spawn(p.DashHold, inter, 333f, 33f);
        ConfigureDashZoneHold(hold4C, -36f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 22f, 0.22f); // -> ~(345.8, 23.7)!
        for (int x = 337; x <= 342; x++)
            for (int y = 24; y <= 32; y++)
                if (Mathf.Abs(y - (33f - (x - 333f) * 0.72f)) <= 2.3f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Dive Cascade #3!
        SetAtomId(p.Spawn(p.Atom, col, 340f, 31f), 13);
        GameObject can4A = p.Spawn(p.CannonFast, haz, 344f, 35f);
        ConfigureCannon(can4A, new Vector3(0f, 0f, -150f), 11f, 2.2f, 22f);
        p.Spawn(p.CrumblingBlock, env, 346f, 23f);                       // Collapsing Sky Island!

        // Recovery below Zenith Waves (330..346, Y=0)
        p.Spawn(p.BouncePadMega, inter, 330f, 0f);
        p.Spawn(p.LavaLeaper, haz, 336f, 0f);
        p.Spawn(p.BouncePadMega, inter, 342f, 0f);
        p.Spawn(p.DashLaneUp, inter, 342f, 8f);
        SetAtomId(p.Spawn(p.Atom, col, 346f, 17.5f), 14);

        // Vector #4 (Diagonal UP Express + Low Hyper Blitz) -> COLOSSAL FINALE BASTION -> End_Portal!
        p.Spawn(p.BouncePad, inter, 352f, 12f);
        GameObject hold4Up = p.Spawn(p.DashHold, inter, 353f, 22f);
        ConfigureDashZoneHold(hold4Up, 26f, new Vector2(3.4f, 3.4f), 34f, 0.34f, 22f, 0.22f); // -> ~(363.4, 27.1)!
        GameObject hold4Exp = p.Spawn(p.DashHold, inter, 364f, 27.5f);
        ConfigureDashZoneHold(hold4Exp, 4f, new Vector2(3.6f, 3.6f), 38f, 0.48f, 22f, 0.22f); // -> ~(382.2, 28.8)!
        for (int x = 368; x <= 370; x++)
            for (int y = 25; y <= 31; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int y = 25; y <= 30; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 372f, y);                // Upper Express Bastion (372..379, 7-deep)!
            p.Spawn(p.DashWallCracked, inter, 377.5f, y - 0.5f);
            p.Spawn(p.DashWallCracked, inter, 378.5f, y - 0.5f);
        }
        SetAtomId(p.Spawn(p.Atom, col, 381.5f, 28.5f), 16);              // Top Express Vector Reward Atom 16!
        p.Spawn(p.DashCharge, inter, 381.5f, 25.5f);

        // Main Finale Hyper Blitz (Lower Zenith Deck Y=12 -> DashZone@(362, 13.5) -> Triple Bastion -> End_Portal@(382, 13.8))
        p.Spawn(p.CrumblingBlock, env, 358f, 18.5f);                     // Step-up above Hyper Blitz beam -> zero clip!
        GameObject db4Low = p.Spawn(p.DashBoostFast, inter, 351f, 13.5f);
        ConfigureDashZoneBoost(db4Low, new Vector2(3.0f, 3.0f), 34f, 0.30f, 22f); // -> 361.2 into dz4Low!
        SetAtomId(p.Spawn(p.Atom, col, 359f, 16.5f), 15);
        GameObject dz4Low = p.Spawn(p.DashZone, inter, 362f, 13.5f);
        ConfigureDashZoneBoost(dz4Low, new Vector2(3.6f, 3.6f), 38f, 0.56f, 24f); // -> X=383.3!
        for (int x = 366; x <= 368; x++)
            for (int y = 13; y <= 19; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 3x7 Colossal Purple Front!
        for (int y = 13; y <= 19; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 371f, y);                // Flush at Y=12..19 + 8-deep Bastion (371..379)!
            p.Spawn(p.DashWallCracked, inter, 376.5f, y - 0.5f);
            p.Spawn(p.DashWallCracked, inter, 377.5f, y - 0.5f);
            p.Spawn(p.DashWallCracked, inter, 378.5f, y - 0.5f);
        }

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
        return $"Level_49 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 80f;
            lm.TwoStarTime = 122f;
            lm.OneStarTime = 205f;
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
            respawnMgr.KillY = -20f;
            respawnMgr.KillTopY = 66f;
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
