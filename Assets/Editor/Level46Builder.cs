#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level46Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_46.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 46 (Twin-Tower Pendulum Engine)")]
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
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES (X = 0..224, Y = -28..57)
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 112f, -36f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(360f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(360f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -8 to 232)
        for (int x = -8; x < 232; x += 20)
            p.SpawnGroundSized(env, x, -28f, Mathf.Min(20, 232 - x));

        // West & East Outer Twin-Tower Shells + Zenith Crown Roof (Y = 57)
        p.SpawnBlock(p.BlockCyan, env, 0f, 56f, 2, 168, 4);                // X=0..1, Y=-28..56
        p.SpawnBlock(p.BlockCyan, env, 223f, 56f, 2, 168, 4);              // X=223..224, Y=-28..56
        for (int x = 0; x < 224; x += 20)
            p.SpawnCeilingSized(env, x, 57f, Mathf.Min(20, 224 - x));      // Roof Y=56..57

        // =====================================================================
        // FLOW: BOTTOM-CENTER START -> SUB-SPAWN CROWN VAULT END PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 112f, -8f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 88f, -22.2f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (100% Coverage + Extended Lead-In Across Crossings)
        // =====================================================================
        GameObject cz1Sluice = p.Spawn(p.CameraZoneTight, flow, 113f, -4f);
        cz1Sluice.name = "CameraZone_Tight_Z1StartSluice";
        SetTriggerBoxSize(cz1Sluice, new Vector3(44f, 12f, 10f));        // X=91..135, Y=-10..2

        GameObject cz1Ascent = p.Spawn(p.CameraZoneClimb, flow, 66f, 5f);
        cz1Ascent.name = "CameraZone_Climb_Z1UpLeftCatapult";
        SetTriggerBoxSize(cz1Ascent, new Vector3(70f, 32f, 10f));        // X=31..101, Y=-11..21

        GameObject czWestSecret = p.Spawn(p.CameraZoneTight, flow, 17f, 22f);
        czWestSecret.name = "CameraZone_Tight_WestAlcoveAndSanct1";
        SetTriggerBoxSize(czWestSecret, new Vector3(34f, 18f, 10f));     // X=0..34, Y=13..31

        GameObject cz2Bridge = p.Spawn(p.CameraZoneWide, flow, 112f, 22f);
        cz2Bridge.name = "CameraZone_Wide_Z2CrossfireBridge";
        SetTriggerBoxSize(cz2Bridge, new Vector3(172f, 26f, 10f));       // X=26..198, Y=9..35

        GameObject cz2EastDive = p.Spawn(p.CameraZoneClimb, flow, 202f, 14f);
        cz2EastDive.name = "CameraZone_Climb_Z2EastDiveAndVault";
        SetTriggerBoxSize(cz2EastDive, new Vector3(46f, 44f, 10f));      // X=179..225, Y=-8..36

        GameObject cz2Return = p.Spawn(p.CameraZoneWide, flow, 118f, 12f);
        cz2Return.name = "CameraZone_Wide_Z2DiagonalReturn";
        SetTriggerBoxSize(cz2Return, new Vector3(168f, 38f, 10f));      // X=34..202, Y=-7..31

        GameObject cz3Belfry = p.Spawn(p.CameraZoneClimb, flow, 24f, 38f);
        cz3Belfry.name = "CameraZone_Climb_Z3WestBelfry";
        SetTriggerBoxSize(cz3Belfry, new Vector3(48f, 28f, 10f));        // X=0..48, Y=24..52

        GameObject cz3Skylight = p.Spawn(p.CameraZoneWide, flow, 126f, 49f);
        cz3Skylight.name = "CameraZone_Wide_Z3ZenithSlalom";
        SetTriggerBoxSize(cz3Skylight, new Vector3(178f, 16f, 10f));    // X=37..215, Y=41..57

        GameObject cz4HeartSlash = p.Spawn(p.CameraZoneWide, flow, 174f, 17f);
        cz4HeartSlash.name = "CameraZone_Wide_Z4DiagonalHeartSlash";
        SetTriggerBoxSize(cz4HeartSlash, new Vector3(96f, 66f, 10f));   // X=126..222, Y=-16..50

        GameObject cz4CrownVault = p.Spawn(p.CameraZoneTight, flow, 112f, -19.5f);
        cz4CrownVault.name = "CameraZone_Tight_Z4SubSpawnCrownVault";
        SetTriggerBoxSize(cz4CrownVault, new Vector3(84f, 15f, 10f));   // X=70..154, Y=-27..-12

        // =====================================================================
        // ZONE 1: THE SUNKEN SLUICE CATAPULT (Start BOTTOM-CENTER -> Up-Left!)
        // X = 96..132 (Start Sluice) -> Up-Left 94 -> 34, Y = -8 -> 18
        // =====================================================================
        p.SpawnGroundSized(env, 96f, -8f, 36);                           // Start Floor X=96..132, Y=-9..-8
        p.SpawnCeilingSized(env, 98f, 0f, 34);                           // Start Roof X=98..132, Y=-1..0
        p.SpawnBlock(p.BlockCyan, env, 131f, -1f, 2, 14, 4);             // East Start Seal X=131..132, Y=-8..-1 (flush!)
        p.SpawnBlock(p.BlockGreen, env, 104f, -1f, 16, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 96f, -9f, 4, 10, 4);           // Reactor Pier Y=-14..-9 (flush!)

        // First Boost Left + Mid-Flight Charge -> Slope45Left Staircase
        GameObject db1Sluice = p.Spawn(p.DashBoostLeft, inter, 101f, -6.5f);
        ConfigureDashZoneBoost(db1Sluice, new Vector2(3.2f, 3.2f), 30f, 0.22f, 20f); // Ends ~X=94.4!
        p.Spawn(p.DashCharge, inter, 91f, -5.5f);
        for (int x = 94; x <= 97; x++)
            for (int y = -7; y <= -2; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Sluice Break Archway!

        p.Spawn(p.Slope45Left, env, 89f, -8f);                           // 88..90, -8..-6
        p.Spawn(p.Slope45Left, env, 87f, -6f);                           // 86..88, -6..-4
        p.Spawn(p.Slope45Left, env, 85f, -4f);                           // 84..86, -4..-2
        p.SpawnBlock(p.BlockCyan, env, 84f, -6f, 4, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 86f, -6f, 4, 4, 4);
        p.SpawnGroundSized(env, 74f, -2f, 10);                           // Upper Sluice Pad X=74..84, Y=-3..-2
        p.SpawnBlock(p.BlockCyan, env, 74f, -3f, 2, 10, 4);              // Pier Y=-8..-3

        p.Spawn(p.LavaLeaper, haz, 77f, -2f);
        p.Spawn(p.CrumblingBlock, env, 72f, -1.8f);                      // Coupled near LavaLeaper!
        p.Spawn(p.BouncePad, inter, 81f, -2f);
        SetAtomId(p.Spawn(p.Atom, col, 81f, 0.2f), 1);

        // Diagonal Barrel #1 (148 deg UP-LEFT -> Purple Cascade -> 1.8m Clear Gap -> Yellow Gate)
        GameObject hold1A = p.Spawn(p.DashHold, inter, 74f, 4.5f);
        ConfigureDashZoneHold(hold1A, 148f, new Vector2(3.4f, 3.4f), 34f, 0.36f, 22f, 0.20f); // -> ~(63.6, 11.0)!

        for (int x = 64; x <= 70; x++)
            for (int y = 5; y <= 13; y++)
                if (y >= 12 - (x - 64) - 2 && y <= 13)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Inverted Cascade
        for (int y = 8; y <= 14; y++)                                    // Cleanly sits ON TOP of Y=7!
        {
            p.Spawn(p.DashWallCracked, inter, 61.5f, y - 0.5f);
            p.Spawn(p.DashWallCracked, inter, 62.5f, y - 0.5f);
        }
        SetAtomId(p.Spawn(p.Atom, col, 67f, 10f), 2);

        // West Catch Deck + Interlocked MovingPlatform/Crumbling + FireJet -> Sanctuary 1
        p.SpawnGroundSized(env, 46f, 7f, 17);                            // Catch Deck X=46..63, Y=6..7
        p.SpawnBlock(p.BlockCyan, env, 46f, 6f, 2, 28, 4);               // Pier down to -8!
        p.Spawn(p.FireJet, haz, 56f, 7f);
        GameObject mp1Catch = p.Spawn(p.MovingPlatform, env, 53f, 9f);
        ConfigureMovingPlatform(mp1Catch, new Vector3(4f, 0f, 0f), 3.0f, 0.2f);
        p.Spawn(p.BouncePadMini, inter, 51f, 7f);
        p.Spawn(p.CrumblingBlock, env, 42f, 8.5f);
        GameObject hold1B = p.Spawn(p.DashHold, inter, 46f, 11.5f);
        ConfigureDashZoneHold(hold1B, 115f, new Vector2(3.4f, 3.4f), 32f, 0.28f, 18f, 0.20f); // -> ~(42.2, 19.6) UP through OneWay!

        // Zone 1 Continuous Lower Safety Floor (X = 44..96, Y = -8) with Instant Rebounders + Crumbling Step
        p.SpawnGroundSized(env, 44f, -8f, 52);                           // X=44..96, Y=-9..-8
        p.SpawnBlock(p.BlockCyan, env, 44f, -9f, 2, 38, 4);
        p.Spawn(p.BouncePadMega, inter, 52f, -8f);
        p.Spawn(p.BouncePadMega, inter, 63f, -8f);
        p.Spawn(p.BouncePad, inter, 73f, -8f);
        p.Spawn(p.BouncePadMini, inter, 86f, -4f);
        p.Spawn(p.LavaLeaper, haz, 68f, -8f);
        p.Spawn(p.CrumblingBlock, env, 68f, -5.5f);                      // Dynamic bridge over LavaLeaper!
        p.Spawn(p.DashCharge, inter, 58f, 0f);

        // =====================================================================
        // SANCTUARY 1 & FAR-LEFT SECRET ALCOVE (X = 1..49, Y = 18..29)
        // =====================================================================
        p.SpawnGroundSized(env, 20f, 18f, 21);                           // Sanct 1 Floor X=20..41, Y=17..18
        p.Spawn(p.OneWayPlatformLarge, env, 41f, 18f);                   // Hold1B Hatch X=41..49, Y=18!
        p.SpawnCeilingSized(env, 20f, 29f, 27);                          // Roof X=20..47, Y=28..29
        p.SpawnBlock(p.BlockGreen, env, 23f, 28f, 16, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 34f, 28f, 16, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 34f, 17f, 2, 4, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 38f, 19.5f);
        cp1.name = "Checkpoint_1";

        // Far-Left Secret Vault (X = 1..20, Y = 18..29 -> Atom 3!)
        p.SpawnGroundSized(env, 1f, 18f, 19);
        p.SpawnCeilingSized(env, 1f, 29f, 19);
        p.SpawnBlock(p.BlockCyan, env, 1f, 17f, 4, 90, 4);
        p.Spawn(p.BouncePadMini, inter, 34f, 18f);                         // Eliminates 22m X-gap across West Alcove!
        GameObject dbWestSec = p.Spawn(p.DashBoostLeft, inter, 24f, 19.5f);
        ConfigureDashZoneBoost(dbWestSec, new Vector2(3.0f, 3.0f), 32f, 0.24f, 18f); // Ends X=16.3, clean before Crumbling@12!
        for (int y = 19; y <= 28; y++) p.Spawn(p.DashWall, inter, 19f, y);
        GameObject stompW = p.Spawn(p.StompBlock, haz, 14f, 26.5f);
        ConfigureStompBlock(stompW, 0.30f);
        p.Spawn(p.CrumblingBlock, env, 12f, 21.8f);                      // Coupled cleanly near StompBlock, out of boost line!
        for (int x = 7; x <= 10; x++)
            for (int y = 19; y <= 24; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 4.5f, 20f), 3);

        // =====================================================================
        // ZONE 2: LIVING CROSSFIRE BRIDGE (UPPER Y=23..30) & RETURN (LOWER Y=1..22)
        // East Dive Shaft: X = 202..223, Y = 23 -> -4 (Diamond 1 Vault @ 179..200)
        // =====================================================================
        GameObject db2Start = p.Spawn(p.DashBoostFast, inter, 49f, 19.5f);
        ConfigureDashZoneBoost(db2Start, new Vector2(3.2f, 3.2f), 34f, 0.28f, 22f); // -> ~X=58.5!
        for (int x = 53; x <= 57; x++)
            for (int y = 19; y <= 26; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.CrumblingBlock, env, 61f, 19.5f);
        GameObject mp2A = p.Spawn(p.MovingPlatform, env, 68f, 20.5f);
        ConfigureMovingPlatform(mp2A, new Vector3(8f, 0f, 0f), 3.6f, 0.15f); // 68..76, Y=20.5
        GameObject spin2A = p.Spawn(p.SpinArm, haz, 72f, 26f);
        ConfigureSpinArm(spin2A, 95f);
        SetAtomId(p.Spawn(p.Atom, col, 72f, 29.5f), 4);
        for (int x = 70; x <= 77; x++)
            for (int y = 27; y <= 31; y++)
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y); // Cleanly Y>=27!

        // Upper Crossfire Chasm Barrels (Y=24.5 — cleanly 7m ABOVE Lower Return Lane!)
        GameObject hold2A = p.Spawn(p.DashHold, inter, 83f, 24.5f);
        ConfigureDashZoneHold(hold2A, 8f, new Vector2(3.4f, 3.4f), 34f, 0.32f, 22f, 0.22f); // -> (93.8, 26.0)
        p.Spawn(p.DashCharge, inter, 92f, 26f);
        for (int x = 95; x <= 104; x++)
            for (int y = 23; y <= 30; y++)
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 99f, 27f), 5);

        GameObject mp2B = p.Spawn(p.MovingPlatform, env, 108f, 22f);
        ConfigureMovingPlatform(mp2B, new Vector3(0f, 5f, 0f), 3.4f, 0.15f); // 22..27
        GameObject saw2A = p.Spawn(p.SawSlow, haz, 114f, 25f);
        ConfigureSaw(saw2A, new Vector3(0f, 5.5f, 0f), 3.2f);
        p.Spawn(p.BouncePadMini, inter, 108f, 12f);                        // On safety deck below!

        GameObject hold2B = p.Spawn(p.DashHold, inter, 120f, 24.5f);
        ConfigureDashZoneHold(hold2B, 4f, new Vector2(3.4f, 3.4f), 34f, 0.30f, 22f, 0.22f); // -> (130.2, 25.2)
        for (int x = 124; x <= 128; x++)
            for (int y = 23; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.CrumblingBlock, env, 134f, 23.2f);
        p.Spawn(p.CrumblingBlock, env, 139f, 23.2f);
        GameObject spin2B = p.Spawn(p.SpinArm, haz, 136.5f, 28f);
        ConfigureSpinArm(spin2B, -95f);
        SetAtomId(p.Spawn(p.Atom, col, 136.5f, 31f), 6);
        for (int x = 141; x <= 145; x++)
            for (int y = 26; y <= 31; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Upper Crossfire Purple Pod!

        // Final Bridge -> East Tower Gate (Y = 23..30)
        p.SpawnGroundSized(env, 145f, 23f, 12);                          // Bridge Deck X=145..157, Y=22..23
        p.Spawn(p.OneWayPlatformLarge, env, 171f, 23f);                  // Open entry X=171..179
        p.SpawnBlock(p.BlockMagenta, env, 150f, 33f, 10, 2, 4);
        GameObject can2A = p.Spawn(p.CannonFast, haz, 150f, 31.2f);
        ConfigureCannon(can2A, new Vector3(0f, 0f, 180f), 11f, 2.3f, 22f);
        p.Spawn(p.BouncePadMini, inter, 147f, 23f);
        GameObject db2Gate = p.Spawn(p.DashBoostFast, inter, 155f, 24.5f);
        ConfigureDashZoneBoost(db2Gate, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f); // -> X=170.8!
        for (int x = 160; x <= 162; x++)
            for (int y = 24; y <= 30; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m clear gap!
        for (int y = 24; y <= 30; y++)
            p.Spawn(p.DashWallChainRow1, inter, 165f, y);                // 5x7 Yellow Chain Gate!

        // Zone 2 Lower Recovery Catwalk (OneWay Y = 12, X = 52..148 cleanly under Chasm!)
        for (int x = 52; x < 148; x += 8)
            p.Spawn(p.OneWayPlatformLarge, env, x, 12f);
        foreach (float px in new[] { 62f, 94f, 124f, 144f })
            p.Spawn(p.BouncePadMega, inter, px, 12f);
        foreach (float lx in new[] { 78f, 136f })
            p.Spawn(p.LavaLeaper, haz, lx, 12f);

        // EAST TOWER MID-ROOM & GP PLUNGE SHAFT (X = 180..223, Y = 23 -> -4)
        p.SpawnGroundSized(env, 180f, 23f, 6);                           // Solid X=180..186
        p.Spawn(p.OneWayPlatformLarge, env, 186f, 23f);                  // Zone 4 Diagonal Catch 186..194!
        p.Spawn(p.OneWayPlatformLarge, env, 194f, 23f);                  // 194..202!
        p.Spawn(p.OneWayPlatformLarge, env, 202f, 23f);                  // Plunge Mouth 202..210
        p.Spawn(p.OneWayPlatformLarge, env, 210f, 23f);                  // 210..218
        p.SpawnGroundSized(env, 218f, 23f, 5);
        GameObject mp2East = p.Spawn(p.MovingPlatform, env, 188f, 24f);
        ConfigureMovingPlatform(mp2East, new Vector3(5f, 0f, 0f), 3.2f, 0.2f);
        GameObject can2East = p.Spawn(p.CannonFast, haz, 200f, 28f);
        ConfigureCannon(can2East, new Vector3(0f, 0f, 180f), 11f, 2.2f, 22f);
        p.Spawn(p.DashCharge, inter, 211f, 25f);                            // Eliminates YellowDashOutside18m & powers dive!

        // Plunge Shaft Seal Layers (Fixed GroundPoundChainRow2 upward anchor + zero D_OVERLAP!)
        p.SpawnBlock(p.BlockCyan, env, 200f, 22f, 4, 24, 4);             // X=200..202, Y=10..22
        p.Spawn(p.CrackedBlocks4x4, inter, 204f, 21f);                   // Y=19..21
        p.Spawn(p.CrackedBlocks4x4, inter, 208f, 21f);
        p.Spawn(p.CrackedBlocks4x4, inter, 212f, 21f);
        p.Spawn(p.CrackedBlocks4x4, inter, 216f, 21f);
        for (int x = 204; x <= 219; x++) p.Spawn(p.TouchWallBlue, inter, x, 15f); // Y=14..15
        for (int x = 204; x <= 218; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 7f); // Grows UP Y=7..11! Clear below 14!
        p.Spawn(p.CrackedBlocks4x4, inter, 205f, 2f);                    // Y=0..2 (clear below 7!)
        p.Spawn(p.CrackedBlocks4x4, inter, 211f, 2f);
        p.Spawn(p.CrackedBlocks4x4, inter, 215f, 2f);

        // East Tower Secret Diamond 1 Vault (X = 178..201, Y = 1..10)
        p.SpawnGroundSized(env, 178f, 10f, 22);                          // Roof 178..200, Y=9..10
        p.SpawnBlock(p.BlockCyan, env, 178f, 9f, 2, 16, 4);              // West Vault Back Wall 178..179, Y=1..9 (flush!)
        p.SpawnGroundSized(env, 178f, 1f, 24);                           // Vault Floor X=178..202, Y=0..1
        for (int y = 2; y <= 9; y++) p.Spawn(p.DashWall, inter, 200f, y);
        GameObject db2Vault = p.Spawn(p.DashBoostLeft, inter, 203f, 2.5f);
        ConfigureDashZoneBoost(db2Vault, new Vector2(3.0f, 3.0f), 32f, 0.28f, 18f);
        GameObject saw2Vault = p.Spawn(p.SawSlow, haz, 193f, 6f);
        ConfigureSaw(saw2Vault, new Vector3(0f, -3.5f, 0f), 2.8f);
        p.Spawn(p.FireJet, haz, 189f, 1f);
        p.Spawn(p.CrumblingBlock, env, 186f, 4.5f);                      // Coupled inside vault near FireJet & Saw!
        SetDiamondId(p.Spawn(p.Diamond, col, 183f, 3.2f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 186.5f, 3.2f), 7);

        // East Bottom Trench (Ground SPLIT cleanly around IceSurface!)
        p.SpawnGroundSized(env, 172f, -4f, 24);                          // 172..196
        p.Spawn(p.IceSurface, env, 196f, -4f);                           // 196..202
        p.SpawnGroundSized(env, 202f, -4f, 21);                          // 202..223
        p.Spawn(p.LavaLeaper, haz, 210f, -4f);
        p.Spawn(p.CrumblingBlock, env, 207f, -1.5f);                     // Coupled over LavaLeaper!
        GameObject db2Ret = p.Spawn(p.DashBoostLeft, inter, 192f, -2.5f);
        ConfigureDashZoneBoost(db2Ret, new Vector2(3.2f, 3.2f), 34f, 0.30f, 22f);
        p.Spawn(p.BouncePadMega, inter, 178f, -4f);
        p.Spawn(p.BouncePad, inter, 216f, -4f);
        SetAtomId(p.Spawn(p.Atom, col, 180f, 0f), 8);

        // Return Express Lower Diagonal Chute (ZERO conflict with Zone 4 Slide@Y=6..10 & Zone 2 Bridge@Y=23!)
        GameObject holdRet1 = p.Spawn(p.DashHold, inter, 173f, 1.5f);
        ConfigureDashZoneHold(holdRet1, 168f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 24f, 0.20f); // -> (157.5, 4.8) BELOW Y=6!
        GameObject holdRet1B = p.Spawn(p.DashHold, inter, 154f, 4f);
        ConfigureDashZoneHold(holdRet1B, 144f, new Vector2(3.6f, 3.6f), 36f, 0.36f, 24f, 0.20f); // -> (143.5, 11.6) UP through OneWay!
        p.Spawn(p.DashCharge, inter, 142f, 13f);
        GameObject holdRet2 = p.Spawn(p.DashHold, inter, 135f, 16.5f);
        ConfigureDashZoneHold(holdRet2, 168f, new Vector2(3.6f, 3.6f), 36f, 0.40f, 24f, 0.20f); // -> (120.9, 19.5)
        GameObject mpRet = p.Spawn(p.MovingPlatform, env, 116f, 15.5f);
        ConfigureMovingPlatform(mpRet, new Vector3(-6f, 2f, 0f), 3.4f, 0.15f); // Sweeps Y=15.5..17.5 (5m below mp2B!)
        GameObject holdRet3 = p.Spawn(p.DashHold, inter, 92f, 17.5f);
        ConfigureDashZoneHold(holdRet3, 156f, new Vector2(3.6f, 3.6f), 38f, 0.44f, 24f, 0.20f); // -> (76.7, 24.3)!
        for (int x = 70; x <= 76; x++)
            for (int y = 21; y <= 24; y++)
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y); // Clean below Y=27!
        p.Spawn(p.OneWayPlatformLarge, env, 50f, 22f);                   // 50..58
        p.Spawn(p.OneWayPlatformLarge, env, 60f, 22f);                   // 60..68
        p.Spawn(p.BouncePad, inter, 54f, 22f);
        p.Spawn(p.BouncePadMini, inter, 64f, 22f);

        // =====================================================================
        // SANCTUARY 2 & ZONE 3: WEST BELFRY CLIMB & HIGH ZENITH SLALOM
        // Belfry: X = 1..48, Y = 30..44 | Zenith: X = 48..176, Y = 44..56
        // =====================================================================
        p.SpawnGroundSized(env, 26f, 30f, 16);                           // Sanct 2 Floor X=26..42, Y=29..30
        p.Spawn(p.OneWayPlatformLarge, env, 42f, 30f);                   // Entry Grate 42..50
        p.SpawnCeilingSized(env, 26f, 44f, 22);                          // Belfry Roof X=26..48, Y=43..44
        p.SpawnBlock(p.BlockGreen, env, 27f, 43f, 16, 2, 4);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 34f, 31.5f);
        cp2.name = "Checkpoint_2";

        // West Belfry Vertical Shaft
        p.SpawnGroundSized(env, 1f, 30f, 25);
        p.SpawnGroundSized(env, 1f, 44f, 9);
        p.Spawn(p.OneWayPlatformLarge, env, 10f, 44f);
        p.Spawn(p.OneWayPlatformLarge, env, 18f, 44f);
        p.Spawn(p.Wall12, env, 6f, 43f);
        p.Spawn(p.Wall12, env, 20f, 43f);
        p.Spawn(p.DashLaneUp, inter, 13.5f, 36f);
        p.Spawn(p.BouncePadMega, inter, 13.5f, 30f);
        GameObject mp3Belfry = p.Spawn(p.MovingPlatform, env, 12f, 39f);
        ConfigureMovingPlatform(mp3Belfry, new Vector3(4f, 0f, 0f), 3.0f, 0.2f);
        GameObject stomp3B = p.Spawn(p.StompBlock, haz, 23f, 42f);
        ConfigureStompBlock(stomp3B, 0.30f);
        GameObject saw3B = p.Spawn(p.SawSlow, haz, 4f, 38f);
        ConfigureSaw(saw3B, new Vector3(0f, -5f, 0f), 3.0f);
        for (int x = 9; x <= 18; x++) p.Spawn(p.TouchWallBlue, inter, x, 34f);
        for (int x = 9; x <= 18; x++) p.Spawn(p.TouchWallBlue, inter, x, 41f);
        SetAtomId(p.Spawn(p.Atom, col, 4.5f, 46f), 9);

        // ZONE 3 HIGH ZENITH SLALOM
        p.SpawnGroundSized(env, 48f, 44f, 12);
        GameObject db3Launch = p.Spawn(p.DashBoostFast, inter, 50f, 45.5f);
        ConfigureDashZoneBoost(db3Launch, new Vector2(3.2f, 3.2f), 34f, 0.34f, 22f); // -> ~X=61.5!
        for (int x = 55; x <= 59; x++)
            for (int y = 45; y <= 53; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.CrumblingBlock, env, 64f, 43.8f);
        p.Spawn(p.CrumblingBlock, env, 69f, 43.8f);
        p.SpawnGroundSized(env, 74f, 44f, 10);
        p.Spawn(p.BouncePadMega, inter, 79f, 44f);
        p.Spawn(p.FireJet, haz, 75f, 44f);
        p.Spawn(p.CrumblingBlock, env, 75f, 45.2f);                      // Coupled near FireJet, cleanly below Stalactite (Y>=46)!

        GameObject mp3A = p.Spawn(p.MovingPlatform, env, 88f, 44.5f);
        ConfigureMovingPlatform(mp3A, new Vector3(8f, 0f, 0f), 3.6f, 0.15f);
        GameObject stomp3Mid = p.Spawn(p.StompBlock, haz, 92f, 52.5f);
        ConfigureStompBlock(stomp3Mid, 0.28f);

        p.SpawnGroundSized(env, 100f, 44f, 12);
        p.Spawn(p.BouncePad, inter, 106f, 44f);
        p.Spawn(p.CrumblingBlock, env, 116f, 44.2f);
        GameObject mp3B = p.Spawn(p.MovingPlatform, env, 124f, 45f);
        ConfigureMovingPlatform(mp3B, new Vector3(8f, 0f, 0f), 3.6f, 0.15f);
        GameObject spin3Sky = p.Spawn(p.SpinArm, haz, 128f, 49.5f);
        ConfigureSpinArm(spin3Sky, 95f);

        p.SpawnGroundSized(env, 138f, 44f, 16);
        p.Spawn(p.BouncePad, inter, 141f, 44f);
        p.Spawn(p.CrumblingBlock, env, 158f, 43.8f);

        // Skylight Stalactites (Added denser Purple waves!)
        p.Spawn(p.DashCharge, inter, 66f, 50f);
        for (int x = 68; x <= 74; x++)
            for (int y = 47; y <= 55; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        GameObject hold3A = p.Spawn(p.DashHold, inter, 79f, 49.5f);
        ConfigureDashZoneHold(hold3A, 4f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 22f, 0.22f);
        for (int x = 84; x <= 90; x++)
            for (int y = 47; y <= 55; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.DashCharge, inter, 96f, 50f);
        SetAtomId(p.Spawn(p.Atom, col, 96f, 53.5f), 10);
        for (int x = 100; x <= 107; x++)
            for (int y = 47; y <= 55; y++)
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
        GameObject can3Sky1 = p.Spawn(p.CannonFast, haz, 114f, 54.5f);
        ConfigureCannon(can3Sky1, new Vector3(0f, 0f, -155f), 11f, 2.2f, 20f);

        GameObject hold3B = p.Spawn(p.DashHold, inter, 120f, 50f);
        ConfigureDashZoneHold(hold3B, -4f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 22f, 0.22f);
        for (int x = 135; x <= 142; x++)
            for (int y = 47; y <= 55; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int x = 146; x <= 150; x++)
            for (int y = 48; y <= 54; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Upper Rafter Purple Cascade!
        SetAtomId(p.Spawn(p.Atom, col, 138f, 53f), 11);
        SetDiamondId(p.Spawn(p.Diamond, col, 118f, 53.8f), 2);

        // Zenith End Gate -> Sanctuary 3 (Curtain starts y=46 cleanly above CrumblingBlock@158, 43.8!)
        GameObject db3End = p.Spawn(p.DashBoostFast, inter, 152f, 45.5f);
        ConfigureDashZoneBoost(db3End, new Vector2(3.2f, 3.2f), 36f, 0.42f, 24f); // -> ~X=167.1!
        for (int x = 158; x <= 160; x++)
            for (int y = 46; y <= 53; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int y = 45; y <= 52; y++)
            p.Spawn(p.DashWallChainRow1, inter, 163f, y);
        p.Spawn(p.OneWayPlatformLarge, env, 168f, 44f);
        p.SpawnGroundSized(env, 176f, 44f, 24);
        p.SpawnBlock(p.BlockMagenta, env, 177f, 55f, 16, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 184f, 45.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE DIAGONAL HEART-SLASH FINALE
        // East Summit Plunge -> Diagonal Slash (Y=28 -> 23 -> 10 -> 6)
        // -> Core Plunge (136..148) -> Under-Spawn Crown Vault (74..134, Y=-24..-15)
        // =====================================================================
        p.Spawn(p.OneWayPlatformLarge, env, 200f, 44f);
        p.Spawn(p.OneWayPlatformLarge, env, 208f, 44f);
        p.SpawnGroundSized(env, 216f, 44f, 7);
        GameObject db4Entry = p.Spawn(p.DashBoostFast, inter, 191f, 45.5f);
        ConfigureDashZoneBoost(db4Entry, new Vector2(3.0f, 3.0f), 32f, 0.25f, 18f);
        for (int x = 195; x <= 198; x++)
            for (int y = 45; y <= 52; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        p.Spawn(p.BouncePadMini, inter, 200f, 44f);
        p.Spawn(p.DashCharge, inter, 206f, 38f);                            // Eliminates Zenith->Summit NN gap & powers GP!

        p.Spawn(p.CrackedBlocks4x4, inter, 201f, 42f);                   // Y=40..42
        p.Spawn(p.CrackedBlocks4x4, inter, 205f, 42f);
        p.Spawn(p.CrackedBlocks4x4, inter, 209f, 42f);
        for (int x = 201; x <= 212; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 34f); // Grows UP Y=34..38! 2m clear below 40!
        SetAtomId(p.Spawn(p.Atom, col, 208f, 39.5f), 12);

        // Diagonal Barrel #1: East Summit (206, 28) -> (195.6, 21.5) onto OneWay@194..202,23!
        GameObject hold4A = p.Spawn(p.DashHold, inter, 206f, 28f);
        ConfigureDashZoneHold(hold4A, 212f, new Vector2(3.6f, 3.6f), 36f, 0.34f, 22f, 0.20f); // Cleanly stops at (195.6, 21.5) (Zero solid hit!)
        p.Spawn(p.DashCharge, inter, 196f, 24f);                            // Closes Hold4A->Hold4B gap cleanly!
        for (int x = 195; x <= 200; x++)
            for (int y = 24; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        // Diagonal Barrel #2: (186, 18) -> (175.2, 10.8) onto Slope Deck Y=10!
        GameObject hold4B = p.Spawn(p.DashHold, inter, 186f, 18f);
        ConfigureDashZoneHold(hold4B, 214f, new Vector2(3.6f, 3.6f), 36f, 0.36f, 22f, 0.20f);
        for (int x = 175; x <= 181; x++)
            for (int y = 11; y <= 17; y++)                                // Bottom Y>=10 (zero Vault Roof 178..200 overlap!)
                if (Mathf.Abs(x - 178f) + Mathf.Abs(y - 14f) <= 3.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int x = 163; x <= 167; x++)
            for (int y = 11; y <= 16; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Heart Slide Purple Smash Cloud!

        // Elevated Sloped Heart Slide (X = 148..178, Y = 10 -> 6 — keeps Y=-4..5 100% open for Return!)
        p.SpawnGroundSized(env, 170f, 10f, 8);                           // Connects Vault Roof 178..200 seamlessly!
        p.Spawn(p.Slope45, env, 169f, 8f);                               // 168..170, 8..10
        p.Spawn(p.Slope45, env, 167f, 6f);                               // 166..168, 6..8
        p.SpawnBlock(p.BlockCyan, env, 168f, 8f, 4, 4, 4);               // Support 168..170, 6..8
        p.Spawn(p.IceSurface, env, 160f, 6f);                            // 160..166, Y=5..6
        p.Spawn(p.OneWayPlatform, env, 156f, 6f);                        // 156..160
        p.Spawn(p.OneWayPlatformLarge, env, 148f, 6f);                   // 148..156 (Return shoots UP cleanly!)
        GameObject mp4Slide = p.Spawn(p.MovingPlatform, env, 161f, 7.5f);
        ConfigureMovingPlatform(mp4Slide, new Vector3(0f, 3f, 0f), 2.8f, 0.2f);
        GameObject saw4Slide = p.Spawn(p.SawSlow, haz, 155f, 9f);
        ConfigureSaw(saw4Slide, new Vector3(0f, 4f, 0f), 3.0f);
        GameObject db4Slash = p.Spawn(p.DashBoostLeft, inter, 159f, 7.5f);
        ConfigureDashZoneBoost(db4Slash, new Vector2(3.2f, 3.2f), 34f, 0.34f, 22f); // -> ~X=147.4!
        for (int y = 7; y <= 13; y++) p.Spawn(p.DashWallCracked, inter, 151.5f, y - 0.5f);
        SetAtomId(p.Spawn(p.Atom, col, 154f, 9f), 13);

        // HEART CORE PLUNGE WELL (X = 134..148, Y = 6 down to -24!)
        p.SpawnBlock(p.BlockCyan, env, 134f, 5f, 2, 38, 4);              // West Well Wall X=134..135, Y=-14..5 (open -24..-14!)
        p.SpawnBlock(p.BlockCyan, env, 148f, 5f, 2, 38, 4);              // East Well Wall X=148..149, Y=-14..5
        p.Spawn(p.OneWayPlatformLarge, env, 136f, 6f);
        p.Spawn(p.OneWayPlatform, env, 144f, 6f);
        p.Spawn(p.DashCharge, inter, 142f, -2f);
        for (int x = 136; x <= 147; x++) p.Spawn(p.TouchWallBlue, inter, x, 3f); // Y=2..3
        p.Spawn(p.CrackedBlocks4x4, inter, 137f, 0f);                    // Y=-2..0
        p.Spawn(p.CrackedBlocks4x4, inter, 141f, 0f);
        p.Spawn(p.CrackedBlocks4x4, inter, 145f, 0f);
        for (int x = 136; x <= 147; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -9f); // Grows UP Y=-9..-5! Clear below -2!
        SetAtomId(p.Spawn(p.Atom, col, 142f, -3.5f), 14);
        p.Spawn(p.FireJet, haz, 145f, -24f);
        p.Spawn(p.CrumblingBlock, env, 145f, -20.5f);                    // Coupled above bottom FireJet!

        // SUB-SPAWN CROWN VAULT & COLOSSAL HYPER-MONOLITH (Ground SPLIT around IceSurface!)
        p.SpawnGroundSized(env, 74f, -24f, 58);                          // Floor X=74..132
        p.Spawn(p.IceSurface, env, 132f, -24f);                          // Ice 132..138
        p.SpawnGroundSized(env, 138f, -24f, 34);                         // Floor X=138..172
        p.SpawnBlock(p.BlockCyan, env, 74f, -9f, 2, 30, 4);              // Vault West Wall 74..75, Y=-24..-9
        for (int x = 75; x < 134; x += 20)
            p.SpawnCeilingSized(env, x, -14f, Mathf.Min(20, 134 - x));   // Flush with 74..75 right edge!
        p.SpawnBlock(p.BlockMagenta, env, 80f, -15f, 16, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 96f, -15f, 16, 2, 4);

        // Final Hyper-Sling Left from Heart Plunge Bottom -> Monolith -> Crown Vault!
        p.Spawn(p.BouncePadMini, inter, 141f, -24f);
        GameObject db4Fin1 = p.Spawn(p.DashBoostLeft, inter, 135f, -22.5f);
        ConfigureDashZoneBoost(db4Fin1, new Vector2(3.2f, 3.2f), 38f, 0.42f, 24f);
        GameObject dz4Fin2 = p.Spawn(p.DashZone, inter, 128f, -22.5f);
        dz4Fin2.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        ConfigureDashZoneBoost(dz4Fin2, new Vector2(3.4f, 3.4f), 38f, 0.58f, 24f);

        // Colossal Purple Monolith (121..124, -23..-16) + 2m Air Gap + Double Yellow DashWall_ChainRow 1 Door (108..118)
        for (int x = 121; x <= 124; x++)
            for (int y = -23; y <= -16; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int y = -23; y <= -16; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 113f, y);
            p.Spawn(p.DashWallChainRow1, inter, 108f, y);
        }

        SetAtomId(p.Spawn(p.Atom, col, 126f, -22.2f), 15);
        SetDiamondId(p.Spawn(p.Diamond, col, 102f, -22.2f), 3);
        SetAtomId(p.Spawn(p.Atom, col, 96f, -22.2f), 16);

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
        return $"Level_46 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 72f;
            lm.TwoStarTime = 112f;
            lm.OneStarTime = 185f;
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

    private static void ConfigureLavaLeaper(GameObject go, float height)
    {
        var ll = go.GetComponent<LavaLeaper>();
        if (ll != null)
        {
            ll.Height = height;
            EditorUtility.SetDirty(ll);
            PrefabUtility.RecordPrefabInstancePropertyModifications(ll);
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
        public GameObject Wall12 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Wall_1x12.prefab");
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
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
