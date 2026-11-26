#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level39Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_39.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 39 (Barrel-Cannon Hyper-Smash)")]
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
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 192f, -28f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(480f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(480f, 4f, 1f);

        // Continuous Bedrock Base at Y = -22 (X = -10 to 400)
        for (int x = -10; x <= 390; x += 20)
            p.Spawn(p.Ground20, env, x, -22f);

        // =====================================================================
        // FLOW: START PORTAL, END PORTAL, PLAYER SPAWN & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 4f, 0f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 379f, 16.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // ZONE 1: THE BLAST RUNWAY (X = 0 to 92, Y = 0 to 19)
        // High-speed chained Chevron Boosts, BouncePad rebounds, Slope_45_Large,
        // diagonal DashHold barrels & giant Purple + DashWall_ChainRow 1 monoliths
        // =====================================================================
        GameObject czTightRunway = p.Spawn(p.CameraZoneTight, flow, 16.5f, 7.5f);
        czTightRunway.name = "CameraZone_Tight_BlastEntry";
        SetTriggerBoxSize(czTightRunway, new Vector3(35f, 19f, 10f)); // X=-1..34, Y=-2..17

        GameObject czWideBlast = p.Spawn(p.CameraZoneWide, flow, 63f, 9f);
        czWideBlast.name = "CameraZone_Wide_BlastSky";
        SetTriggerBoxSize(czWideBlast, new Vector3(58f, 22f, 10f));   // X=34..92, Y=-2..20

        // Left Perimeter Wall (X = -1..0, Y = -22..14)
        p.SpawnBlock(p.BlockCyan, env, -1f, 14f, 2, 72, 4);

        // Zone 1 Ceilings (Y=14 entry -> Y=19 wide sky)
        p.Spawn(p.Ceiling20, env, 0f, 14f);
        p.SpawnCeilingSized(env, 20f, 14f, 14);                      // X=20..34
        p.SpawnBlock(p.BlockCyan, env, 34f, 18f, 2, 8, 4);           // Step wall X=34..35, Y=14..18 (flush under Y=19 roof)
        p.Spawn(p.Ceiling20, env, 34f, 19f);
        p.Spawn(p.Ceiling20, env, 54f, 19f);
        p.SpawnCeilingSized(env, 74f, 19f, 18);                      // X=74..92

        // Continuous Safety Recovery Floor (X = 0..92, Y = 0) & Piers
        for (int x = 0; x < 80; x += 20)
            p.Spawn(p.Ground20, env, x, 0f);
        p.SpawnGroundSized(env, 80f, 0f, 12);
        foreach (float px in new[] { 0f, 20f, 40f, 60f, 80f })
            p.SpawnBlock(p.BlockCyan, env, px, -1f, 2, 42, 4);       // Y=-22..-1

        // Smash Burst #1: DashBoost -> 7m-deep Monolith (TouchWall_Blue + DashWall_ChainRow 1 x4 + TouchWall_Blue)
        GameObject dbEntry = p.Spawn(p.DashBoost, inter, 10f, 1.5f);
        ConfigureDashZoneBoost(dbEntry, new Vector2(3f, 3f), 34f, 0.40f, 22f);

        p.SpawnBlock(p.BlockCyan, env, 15f, 13f, 14, 18, 4);         // Tunnel arch X=15..22, Y=4..13
        p.SpawnTouchWallCol(inter, 15f, 1f, 4f);
        for (int y = 1; y <= 4; y++) p.Spawn(p.DashWallChainRow1, inter, 16f, y); // 5x4 yellow chain wall!
        p.SpawnTouchWallCol(inter, 21f, 1f, 4f);
        SetAtomId(p.Spawn(p.Atom, col, 24f, 1.5f), 1);

        // Smash Burst #2: DashBoost_Fast -> Slope_45_Large -> Checkerboard Smash Deck -> BouncePad
        p.Spawn(p.DashBoostFast, inter, 28f, 1.5f);
        p.Spawn(p.Slope45Large, env, 37f, 0f);                       // X=35..39, Y=0..4
        p.SpawnGroundSized(env, 39f, 4f, 9);                         // Deck 1 X=39..48, Y=3..4
        p.SpawnBlock(p.BlockCyan, env, 39f, 3f, 2, 6, 4);            // Support pier Y=0..3

        for (int x = 40; x <= 43; x++)
        {
            for (int y = 5; y <= 7; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 44.8f, 5.5f), 2);
        p.Spawn(p.BouncePad, inter, 46f, 4f);

        // Diagonal Barrel Cascade (BouncePad 46,4 -> Barrel 1A 49,8 @ 30 deg -> Sky Cube -> DashZone 59,13.5 -> Monolith 2)
        GameObject barrel1A = p.Spawn(p.DashHold, inter, 49f, 8f);
        ConfigureDashZoneHold(barrel1A, 30f, new Vector2(2.8f, 2.8f), 32f, 0.32f, 22f, 3.5f);

        for (int x = 53; x <= 55; x++)
            for (int y = 10; y <= 13; y++)
                p.Spawn((x + y) % 2 == 0 ? p.TouchWallBlue : p.DashWall, inter, x, y);

        GameObject dzBlast1B = p.Spawn(p.DashZone, inter, 59f, 13.5f);
        ConfigureDashZoneBoost(dzBlast1B, new Vector2(3.4f, 3.4f), 36f, 0.38f, 24f);

        // Sky Monolith 2 (X = 64..71, Y = 11..16) atop OneWayPlatform
        p.Spawn(p.OneWayPlatformLarge, env, 63f, 11f);               // X=63..71, Y=11
        p.SpawnBlock(p.BlockCyan, env, 64f, 18f, 14, 4, 4);          // Header X=64..71, Y=16..18
        p.SpawnTouchWallCol(inter, 64f, 12f, 16f);
        for (int y = 12; y <= 16; y++) p.Spawn(p.DashWallChainRow1, inter, 65f, y); // 5x5 chain monolith!
        p.SpawnTouchWallCol(inter, 70f, 12f, 16f);
        SetAtomId(p.Spawn(p.Atom, col, 73f, 13.5f), 3);

        // Zone 1 Recovery Catch-Lanes (Y = 0) & Exit Gate Approach (X = 75..92, Y = 6)
        p.Spawn(p.BouncePad, inter, 54f, 0f);
        SetAtomId(p.Spawn(p.Atom, col, 62f, 1.5f), 4);
        p.Spawn(p.BouncePad, inter, 70f, 0f);
        p.Spawn(p.OneWayPlatform, env, 71f, 5f);

        p.SpawnGroundSized(env, 75f, 6f, 17);                        // Exit deck X=75..92, Y=5..6
        p.SpawnBlock(p.BlockCyan, env, 75f, 5f, 2, 10, 4);           // Support X=75..76, Y=0..5
        p.SpawnBlock(p.BlockGreen, env, 84f, 18f, 2, 16, 4);         // Arch Y=10..18
        p.SpawnTouchWallCol(inter, 84f, 7f, 10f);                    // Exit curtain Y=6..10

        // =====================================================================
        // SANCTUARY 1: BLASTWAY LANDING (X = 92 to 106, Y = 6 to 15)
        // =====================================================================
        p.SpawnGroundSized(env, 92f, 6f, 14);                        // X=92..106, Y=5..6
        p.SpawnCeilingSized(env, 92f, 15f, 14);                      // X=92..106, Y=14..15
        p.SpawnBlock(p.BlockCyan, env, 92f, 18f, 2, 6, 4);           // Step Y=15..18
        p.SpawnBlock(p.BlockGreen, env, 93f, 14f, 2, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 103f, 14f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 99f, 5f, 2, 54, 4);           // Pier Y=-22..5
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 99f, 7.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE SKY-BARREL SHAFT (X = 106 to 162, Y = 6 to 36)
        // Donkey Kong vertical barrel-cannon climb (CameraZone_Climb):
        // DashLane_UP + timing DashHold barrels + Saw_Slow + FireJet + DashCharge
        // =====================================================================
        GameObject czClimbShaft = p.Spawn(p.CameraZoneClimb, flow, 134f, 21f);
        czClimbShaft.name = "CameraZone_Climb_SkyBarrels";
        SetTriggerBoxSize(czClimbShaft, new Vector3(56f, 34f, 10f)); // X=106..162, Y=4..38

        // Framing: Left Wall (X=106), Roof (Y=36), Right Wall (X=161), Safety Floor (Y=6)
        p.SpawnBlock(p.BlockCyan, env, 106f, 35f, 2, 40, 4);         // X=106..107, Y=15..35
        for (int x = 106; x < 162; x += 20)
            p.SpawnCeilingSized(env, x, 36f, Mathf.Min(20, 162 - x));
        p.SpawnBlock(p.BlockCyan, env, 161f, 35f, 2, 10, 4);         // Upper right wall Y=30..35
        p.SpawnBlock(p.BlockCyan, env, 161f, 21f, 2, 30, 4);         // Lower seal below Sanct 2 Y=6..21

        for (int x = 106; x < 162; x += 20)
            p.SpawnGroundSized(env, x, 6f, Mathf.Min(20, 162 - x));  // Recovery base Y=6
        foreach (float px in new[] { 106f, 126f, 146f, 161f })
            p.SpawnBlock(p.BlockCyan, env, px, 5f, 2, 54, 4);        // Piers Y=-22..5

        // 2A: Vertical DashLane_UP Rocket through 2 Ceiling Layers (Y = 6 -> 17)
        p.Spawn(p.BouncePad, inter, 115f, 6f);
        p.Spawn(p.DashLaneUp, inter, 115f, 11f);                     // Y=7..15
        p.SpawnBlock(p.BlockCyan, env, 111f, 17f, 2, 12, 4);         // Left chute wall Y=11..17
        p.SpawnBlock(p.BlockCyan, env, 119f, 17f, 2, 12, 4);         // Right chute wall Y=11..17
        for (int x = 112; x <= 118; x++) p.Spawn(p.TouchWallBlue, inter, x, 14f); // Ceiling Layer 1
        for (int x = 112; x <= 118; x++) p.Spawn(p.DashWall, inter, x, 16f);      // Ceiling Layer 2
        p.Spawn(p.OneWayPlatformLarge, env, 112f, 17.5f);            // Catch perch X=112..120
        SetAtomId(p.Spawn(p.Atom, col, 115f, 19.5f), 5);

        // 2B: Timing DashHold Barrel Chain climbing Y = 17 -> 30 (Saw_Slow + FireJet_Down)
        GameObject barrel2B = p.Spawn(p.DashHold, inter, 125f, 17f);
        ConfigureDashZoneHold(barrel2B, 45f, new Vector2(2.8f, 2.8f), 30f, 0.35f, 20f, 3.5f);

        GameObject sawShaft2 = p.Spawn(p.SawSlow, haz, 128.5f, 17.5f);
        ConfigureSaw(sawShaft2, new Vector3(0f, 4.5f, 0f), 2.3f);
        for (int x = 128; x <= 130; x++)
            for (int y = 20; y <= 22; y++)
                p.Spawn((x + y) % 2 == 0 ? p.TouchWallBlue : p.DashWallCracked, inter,
                    (x + y) % 2 == 0 ? x : x + 0.5f, (x + y) % 2 == 0 ? y : y - 0.5f);

        GameObject barrel2C = p.Spawn(p.DashHold, inter, 132.5f, 24.5f); // Straight-line zero-g lock from 2B!
        ConfigureDashZoneHold(barrel2C, 0f, new Vector2(3.0f, 3.0f), 34f, 0.38f, 20f, 3.5f);

        p.Spawn(p.FireJetDown, haz, 136f, 35f);
        p.Spawn(p.OneWayPlatformLarge, env, 136f, 22.5f);            // Support shelf under chain wall
        for (int y = 24; y <= 26; y++) p.Spawn(p.DashWallChainRow1, inter, 137f, y); // 5x3 horizontal yellow row!
        p.SpawnBlock(p.BlockMagenta, env, 137f, 35f, 10, 18, 4);     // Ceiling header Y=26..35

        p.Spawn(p.DashCharge, inter, 145f, 25f);                     // Refreshes dash mid-air!
        p.Spawn(p.OneWayPlatformLarge, env, 144f, 22f);              // X=144..152
        SetAtomId(p.Spawn(p.Atom, col, 148f, 24f), 6);

        // Optional Diamond 1 Sky Bonus above BouncePadMega (154, 22)
        p.Spawn(p.OneWayPlatformLarge, env, 152f, 22f);              // X=152..160 flush w/ Sanct 2
        p.Spawn(p.BouncePadMega, inter, 154f, 22f);
        for (int x = 153; x <= 155; x++) p.Spawn(p.TouchWallBlue, inter, x, 28f);
        SetDiamondId(p.Spawn(p.Diamond, col, 154f, 31.5f), 1);

        // Recovery Catch-Lanes on Zone 2 Floor (Y = 6)
        p.Spawn(p.FireJet, haz, 122f, 6f);
        p.Spawn(p.BouncePadMega, inter, 128f, 6f);
        p.Spawn(p.FireJet, haz, 138f, 6f);
        p.Spawn(p.BouncePadMega, inter, 145f, 6f);
        p.Spawn(p.OneWayPlatform, env, 148f, 14f);
        p.Spawn(p.DashLaneUp, inter, 156f, 12f);                     // Y=8..16 lifts back toward 22!
        p.Spawn(p.BouncePad, inter, 156f, 6f);

        // =====================================================================
        // SANCTUARY 2: SKY-BARREL SUMMIT (X = 162 to 176, Y = 22 to 30)
        // =====================================================================
        p.SpawnGroundSized(env, 162f, 22f, 14);                      // X=162..176, Y=21..22
        p.SpawnCeilingSized(env, 162f, 30f, 14);                     // X=162..176, Y=29..30
        p.SpawnBlock(p.BlockGreen, env, 163f, 29f, 2, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 173f, 29f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 169f, 21f, 2, 86, 4);         // Pier Y=-22..21
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 169f, 23.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE RICOCHET COASTER (X = 176 to 278, Y = -12 to 30)
        // Rollercoaster Slope_45_Left + IceSurface tracks, upper barrel-chain
        // shortcut, deep GroundPoundChainRow 2 drops & Secret Diamond 2 Vault
        // =====================================================================
        GameObject czCoasterUpper = p.Spawn(p.CameraZoneWide, flow, 227f, 16f);
        czCoasterUpper.name = "CameraZone_Wide_Coaster";
        SetTriggerBoxSize(czCoasterUpper, new Vector3(102f, 32f, 10f)); // X=176..278, Y=0..32

        GameObject czCoasterLower = p.Spawn(p.CameraZoneTight, flow, 240f, -7f);
        czCoasterLower.name = "CameraZone_Tight_VaultCoaster";
        SetTriggerBoxSize(czCoasterLower, new Vector3(76f, 14f, 10f));  // X=202..278, Y=-14..0

        // Zone 3 Roof (Y=30 across 176..278) & End Step Wall (X=277, Y=9..29)
        for (int x = 176; x < 278; x += 20)
            p.SpawnCeilingSized(env, x, 30f, Mathf.Min(20, 278 - x));
        p.SpawnBlock(p.BlockCyan, env, 277f, 29f, 2, 40, 4);         // Step down to Sanct 3 roof Y=9..29

        // Coaster Drop 1: 4x Slope_45_Left (Y = 22 -> 14) -> IceSurface + DashBoost_Fast -> DashWall_ChainRow 1
        p.Spawn(p.Slope45Left, env, 177f, 20f);                      // 176..178, 20..22
        p.Spawn(p.Slope45Left, env, 179f, 18f);                      // 178..180, 18..20
        p.Spawn(p.Slope45Left, env, 181f, 16f);                      // 180..182, 16..18
        p.Spawn(p.Slope45Left, env, 183f, 14f);                      // 182..184, 14..16
        p.SpawnBlock(p.BlockCyan, env, 176f, 20f, 4, 14, 4);         // 176..178, 13..20
        p.SpawnBlock(p.BlockCyan, env, 178f, 18f, 4, 10, 4);         // 178..180, 13..18
        p.SpawnBlock(p.BlockCyan, env, 180f, 16f, 4, 6, 4);          // 180..182, 13..16
        p.SpawnBlock(p.BlockCyan, env, 182f, 14f, 4, 2, 4);          // 182..184, 13..14

        p.Spawn(p.IceSurface, env, 184f, 14f);                       // X=184..190, Y=13..14
        p.SpawnGroundSized(env, 190f, 14f, 14);                      // Track X=190..204, Y=13..14
        p.SpawnBlock(p.BlockCyan, env, 184f, 13f, 2, 70, 4);         // Deep pier Y=-22..13

        p.Spawn(p.DashBoostFast, inter, 191f, 15.5f);
        p.SpawnBlock(p.BlockCyan, env, 195f, 29f, 10, 24, 4);        // Arch X=195..200, Y=17..29
        for (int y = 15; y <= 17; y++) p.Spawn(p.DashWallChainRow1, inter, 195f, y); // 5x3 yellow row!
        SetAtomId(p.Spawn(p.Atom, col, 201.5f, 15.5f), 7);

        // Optional Upper Barrel-Chain Shortcut (X = 203..238, Y = 17..22) -> 2 bonus Atoms
        p.Spawn(p.OneWayPlatformLarge, env, 203f, 17f);              // X=203..211
        p.Spawn(p.DashCharge, inter, 214f, 19f);
        GameObject barrel3Shortcut = p.Spawn(p.DashHold, inter, 219f, 20f);
        ConfigureDashZoneHold(barrel3Shortcut, 0f, new Vector2(2.8f, 2.8f), 32f, 0.25f, 20f, 0.25f);
        for (int x = 224; x <= 227; x++)
            for (int y = 20; y <= 22; y++)
                p.Spawn((x + y) % 2 == 0 ? p.TouchWallBlue : p.DashWall, inter, x, y);
        p.Spawn(p.OneWayPlatformLarge, env, 229f, 18f);              // X=229..237
        SetAtomId(p.Spawn(p.Atom, col, 232f, 20f), 8);
        SetAtomId(p.Spawn(p.Atom, col, 238.5f, 17f), 9);

        // Coaster Shaft A (X = 204..214, Y = 14 down to 2)
        p.SpawnBlock(p.BlockCyan, env, 203f, 13f, 2, 24, 4);         // Left wall X=203..204, Y=1..13 (flush w/ pier)
        p.SpawnBlock(p.BlockCyan, env, 214f, 16f, 2, 18, 4);         // Right baffle X=214..215, Y=7..16 (leaves Y=16..29 shortcut open!)
        p.Spawn(p.OneWayPlatformLarge, env, 204f, 14f);
        p.Spawn(p.OneWayPlatformShort, env, 212f, 14f);

        p.Spawn(p.GroundPoundChainRow2, inter, 205f, 8f);           // X=205..206, Y=7..12
        p.Spawn(p.GroundPoundChainRow2, inter, 206f, 8f);           // X=206..207, Y=7..12
        p.Spawn(p.GroundPoundChainRow2, inter, 211f, 8f);           // X=211..212, Y=7..12
        p.Spawn(p.GroundPoundChainRow2, inter, 212f, 8f);           // X=212..213, Y=7..12
        p.Spawn(p.CrackedBlocks4x4, inter, 207f, 12f);               // X=207..209, Y=10..12
        p.Spawn(p.CrackedBlocks4x4, inter, 209f, 12f);               // X=209..211, Y=10..12
        for (int x = 207; x <= 210; x++) p.Spawn(p.TouchWallBlue, inter, x, 8f); // Cushion Y=7..8

        // Shelf B & Ice Track (X = 204..233, Y = 2 -> -2)
        p.SpawnGroundSized(env, 204f, 2f, 11);                       // X=204..215, Y=1..2
        p.Spawn(p.IceSurface, env, 215f, 2f);                        // X=215..221, Y=1..2
        p.Spawn(p.Slope45Left, env, 222f, 0f);                       // X=221..223, Y=0..2
        p.Spawn(p.Slope45Left, env, 224f, -2f);                      // X=223..225, Y=-2..0
        p.SpawnBlock(p.BlockCyan, env, 221f, 0f, 4, 4, 4);           // Backing X=221..223, Y=-2..0
        p.SpawnGroundSized(env, 221f, -2f, 12);                      // X=221..233, Y=-3..-2
        p.SpawnBlock(p.BlockCyan, env, 203f, 1f, 2, 46, 4);          // Pier Y=-22..1
        p.SpawnBlock(p.BlockCyan, env, 221f, -3f, 2, 38, 4);         // Pier Y=-22..-3

        // Excavation Shaft B (X = 233..243, Y = -2 down to -12) & Secret Diamond 2 Vault (X = 243..264, Y = -7..0)
        p.SpawnBlock(p.BlockCyan, env, 232f, -3f, 2, 18, 4);         // Left wall X=232..233, Y=-12..-3
        p.SpawnBlock(p.BlockCyan, env, 243f, 29f, 2, 58, 4);         // Upper seal X=243..244, Y=0..29
        p.Spawn(p.OneWayPlatformLarge, env, 233f, -2f);
        p.Spawn(p.OneWayPlatformShort, env, 241f, -2f);
        for (int x = 233; x <= 242; x++) p.Spawn(p.DashWall, inter, x, -4f); // Seal 1 Y=-5..-4

        p.Spawn(p.OneWayPlatformLarge, env, 233f, -7f);              // Mid-shaft landing Y=-7
        p.Spawn(p.OneWayPlatformShort, env, 241f, -7f);

        // Secret Yellow Side-Vault (Diamond 2 + Atom 10 + Atom 11)
        p.SpawnGroundSized(env, 243f, -7f, 21);                      // Floor X=243..264, Y=-8..-7
        p.SpawnBlock(p.BlockMagenta, env, 243f, 0f, 42, 2, 4);       // Vault roof X=243..264, Y=-1..0
        p.SpawnBlock(p.BlockMagenta, env, 263f, -1f, 2, 12, 4);      // Vault back wall X=263..264, Y=-7..-1
        p.SpawnBlock(p.BlockMagenta, env, 243f, -1f, 2, 4, 4);       // Lintel X=243..244, Y=-3..-1
        for (int y = -6; y <= -3; y++) p.Spawn(p.DashWall, inter, 243f, y); // Yellow secret door Y=-7..-3
        p.Spawn(p.FireJet, haz, 250f, -7f);
        GameObject sawZone3 = p.Spawn(p.SawSlow, haz, 253f, -5.8f);
        ConfigureSaw(sawZone3, new Vector3(4f, 0f, 0f), 2.4f);
        SetAtomId(p.Spawn(p.Atom, col, 247.5f, -4.5f), 10);
        SetAtomId(p.Spawn(p.Atom, col, 257f, -4.5f), 11);
        SetDiamondId(p.Spawn(p.Diamond, col, 261f, -5.5f), 2);

        // Lower Shaft B: GroundPoundChainRow 2 full-width vein (Y = -12..-7)
        for (int x = 234; x <= 242; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, -11f);         // Y=-12..-7

        // Deep Coaster Recovery Floor (X = 232..278, Y = -12) -> Launch back up to Sanct 3 (Y = 0)
        for (int x = 232; x < 278; x += 20)
            p.SpawnGroundSized(env, x, -12f, Mathf.Min(20, 278 - x));
        foreach (float px in new[] { 232f, 252f, 272f })
            p.SpawnBlock(p.BlockCyan, env, px, -13f, 2, 18, 4);      // Piers Y=-22..-13

        SetAtomId(p.Spawn(p.Atom, col, 252f, -10.5f), 12);
        p.Spawn(p.BouncePadMega, inter, 264f, -12f);
        p.Spawn(p.DashCharge, inter, 268f, -3f);
        p.Spawn(p.OneWayPlatform, env, 270f, -5f);
        p.Spawn(p.BouncePad, inter, 272f, -12f);
        p.SpawnBlock(p.BlockCyan, env, 278f, -1f, 2, 22, 4);         // Seals below Sanct 3 (Y=-12..-1)

        // =====================================================================
        // SANCTUARY 3: FINALE ARMORY (X = 278 to 292, Y = 0 to 9)
        // =====================================================================
        p.SpawnGroundSized(env, 278f, 0f, 14);                       // X=278..292, Y=-1..0
        p.SpawnCeilingSized(env, 278f, 9f, 14);                      // X=278..292, Y=8..9
        p.SpawnBlock(p.BlockGreen, env, 279f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 289f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 285f, -1f, 2, 42, 4);         // Pier Y=-22..-1
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 285f, 1.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE HYPER-CANNON FINALE (X = 292 to 386, Y = -4 to 28)
        // Uninterrupted rapid-fire chain: DashBoost_Fast -> BouncePad -> DashHold
        // -> DashHold -> DashLane_UP + DashBoost_Up -> DashHold (-15 deg) smashing
        // massive monoliths of Purple & Yellow boxes directly into End_Portal
        // =====================================================================
        GameObject czWideFinale = p.Spawn(p.CameraZoneWide, flow, 340f, 11f);
        czWideFinale.name = "CameraZone_Wide_HyperFinale";
        SetTriggerBoxSize(czWideFinale, new Vector3(96f, 38f, 10f)); // X=292..388, Y=-8..30

        // Sweeping Ceiling (Y=28) & Back Wall behind End_Portal (X=386)
        p.SpawnBlock(p.BlockCyan, env, 292f, 27f, 2, 36, 4);         // Step wall above Sanct 3 roof Y=9..27
        for (int x = 292; x < 386; x += 20)
            p.SpawnCeilingSized(env, x, 28f, Mathf.Min(20, 386 - x));
        p.SpawnBlock(p.BlockCyan, env, 386f, 28f, 2, 100, 4);        // Back wall X=386..387, Y=-22..28

        // Continuous Safety Recovery Floor (X = 292..386, Y = -4)
        for (int x = 292; x < 386; x += 20)
            p.SpawnGroundSized(env, x, -4f, Mathf.Min(20, 386 - x));
        foreach (float px in new[] { 310f, 330f, 350f, 368f })
            p.SpawnBlock(p.BlockCyan, env, px, -5f, 2, 34, 4);       // Piers Y=-22..-5

        // Link 1: DashBoost_Fast -> Horizontal Monolith A (301..308, Y=0..5) -> BouncePad (311, 0)
        p.SpawnGroundSized(env, 292f, 0f, 21);                       // Launch runway X=292..313, Y=-1..0
        GameObject dbFinaleEntry = p.Spawn(p.DashBoostFast, inter, 296f, 1.5f);
        ConfigureDashZoneBoost(dbFinaleEntry, new Vector2(3f, 3f), 36f, 0.42f, 24f);
        p.SpawnBlock(p.BlockMagenta, env, 301f, 27f, 14, 44, 4);     // Header X=301..308, Y=5..27
        p.SpawnTouchWallCol(inter, 301f, 1f, 5f);
        for (int y = 1; y <= 5; y++) p.Spawn(p.DashWallChainRow1, inter, 302f, y); // 5x5 chain monolith!
        p.SpawnTouchWallCol(inter, 307f, 1f, 5f);
        SetAtomId(p.Spawn(p.Atom, col, 309.5f, 1.5f), 13);
        p.Spawn(p.BouncePad, inter, 311f, 0f);

        // Link 2: Barrel 4A (315, 5 @ 35 deg) -> Floating Sky Cube -> Barrel 4B (329, 13.5 @ 0 deg)
        GameObject barrel4A = p.Spawn(p.DashHold, inter, 315f, 5f);
        ConfigureDashZoneHold(barrel4A, 35f, new Vector2(3.0f, 3.0f), 34f, 0.48f, 24f, 0.22f);

        for (int x = 320; x <= 323; x++)
        {
            for (int y = 9; y <= 12; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 326f, 12.5f), 14);

        GameObject barrel4B = p.Spawn(p.DashHold, inter, 329f, 13.5f);
        ConfigureDashZoneHold(barrel4B, 0f, new Vector2(3.2f, 3.2f), 36f, 0.54f, 24f, 0.22f);

        // Link 3: Sky Monolith B (334..341, Y=11..16) -> DashLane_UP + DashBoost_Up -> Ceiling Layer
        p.Spawn(p.OneWayPlatformLarge, env, 334f, 11f);              // X=334..342
        p.SpawnTouchWallCol(inter, 334f, 12f, 16f);
        for (int y = 12; y <= 16; y++) p.Spawn(p.DashWallChainRow1, inter, 335f, y); // 5x5 yellow sky monolith!
        p.SpawnTouchWallCol(inter, 340f, 12f, 16f);
        SetAtomId(p.Spawn(p.Atom, col, 344f, 13.5f), 15);

        p.Spawn(p.DashLaneUp, inter, 349f, 13.5f);                   // Y=9.5..17.5
        GameObject boost4Up = p.Spawn(p.DashBoostUp, inter, 349f, 19f); // Y=17.5..20.5
        ConfigureDashZoneBoost(boost4Up, new Vector2(3.2f, 3.2f), 34f, 0.22f, 20f);
        for (int x = 346; x <= 351; x++) p.Spawn(p.TouchWallBlue, inter, x, 21f); // Sky break curtain Y=20..21

        // Link 4: Barrel 4C (349, 23.5 @ -15 deg) -> Diamond 3 -> Grand Monolith (360..366, Y=14..22) -> End_Portal!
        GameObject barrel4C = p.Spawn(p.DashHold, inter, 349f, 23.5f);
        ConfigureDashZoneHold(barrel4C, -15f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 26f, 0.20f);
        SetDiamondId(p.Spawn(p.Diamond, col, 354f, 22.1f), 3);

        p.Spawn(p.OneWayPlatformLarge, env, 359f, 14f);              // Supports Grand Monolith X=359..367
        p.SpawnBlock(p.BlockCyan, env, 360f, 27f, 14, 10, 4);        // Arch header X=360..367, Y=22..27
        p.SpawnTouchWallCol(inter, 360f, 15f, 22f);                  // 8m tall TouchWall curtain
        for (int y = 15; y <= 22; y++) p.Spawn(p.DashWallChainRow1, inter, 361f, y); // 5x8 = 40 yellow chain blocks!!
        p.SpawnTouchWallCol(inter, 366f, 15f, 22f);                  // 8m exit curtain

        // Victory Dais (X = 368..386, Y = 15) -> Atom 16 -> End_Portal (379, 16.8)
        p.SpawnGroundSized(env, 368f, 15f, 18);                      // X=368..386, Y=14..15
        p.SpawnBlock(p.BlockMagenta, env, 368f, 14f, 2, 36, 4);      // Support pier X=368..369, Y=-4..14
        p.SpawnBlock(p.BlockGreen, env, 372f, 27f, 28, 2, 4);        // Royal shrine canopy trim
        SetAtomId(p.Spawn(p.Atom, col, 372f, 16.5f), 16);

        // Recovery Catch-Lanes beneath Zone 4 Finale (Y = -4)
        p.Spawn(p.BouncePadMega, inter, 320f, -4f);
        p.Spawn(p.BouncePadMega, inter, 340f, -4f);
        p.Spawn(p.OneWayPlatform, env, 355f, 6f);
        p.Spawn(p.BouncePadMega, inter, 360f, -4f);
        p.Spawn(p.DashLaneUp, inter, 365f, 2f);                      // Rockets straight toward Dais top!
        GameObject boostRecover = p.Spawn(p.DashBoostUp, inter, 365f, 8.5f);
        ConfigureDashZoneBoost(boostRecover, new Vector2(3f, 3f), 32f, 0.25f, 20f);
        p.Spawn(p.BouncePad, inter, 365f, -4f);

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
        return $"Level_39 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 48f;
            lm.TwoStarTime = 75f;
            lm.OneStarTime = 125f;
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
            respawnMgr.KillY = -26f;
            respawnMgr.KillTopY = 60f;
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
        public GameObject OneWayPlatformShort = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject DashWallChainRow1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall_ChainRow 1.prefab");
        public GameObject DashWallCracked = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall_Cracked.prefab");
        public GameObject TouchWallBlue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/TouchWall_Blue.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject GroundPoundChainRow2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/GroundPoundChainRow 2.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashBoost = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashBoost.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashBoostUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Up.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashHold = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashHold.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");

        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject FireJetDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Down.prefab");
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

        public void SpawnTouchWallCol(Transform parent, float x, float startY, float endY)
        {
            for (float y = startY; y <= endY; y += 1f)
                Spawn(TouchWallBlue, parent, x, y);
        }
    }
}
#endif
