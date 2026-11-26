#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level40Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_40.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 40 (Purple Crown Grand Finale)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 203f, -35f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(520f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(520f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -10 to 420)
        for (int x = -10; x <= 410; x += 20)
            p.Spawn(p.Ground20, env, x, -28f);

        // =====================================================================
        // FLOW: START PORTAL, END PORTAL, PLAYER SPAWN & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 4f, 10f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 399f, 17.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // ACT 1: THE CITADEL RAMPARTS & IGNITION GATE (X = 0 to 86, Y = 10 to 26)
        // Multi-story Dead Cells battlements -> hidden DashWall Atom alcove ->
        // Slope_45_Large runway + Chevron Boost Pads smashing DashWall_ChainRow 1 gate
        // =====================================================================
        GameObject czTightAct1 = p.Spawn(p.CameraZoneTight, flow, 23.5f, 18f);
        czTightAct1.name = "CameraZone_Tight_Ramparts";
        SetTriggerBoxSize(czTightAct1, new Vector3(49f, 20f, 10f)); // X=-1..48, Y=8..28

        GameObject czWideAct1 = p.Spawn(p.CameraZoneWide, flow, 67f, 18f);
        czWideAct1.name = "CameraZone_Wide_IgnitionGate";
        SetTriggerBoxSize(czWideAct1, new Vector3(38f, 20f, 10f));  // X=48..86, Y=8..28

        // Left Perimeter Wall & Roof (X = 0..86, Y = 26)
        p.SpawnBlock(p.BlockCyan, env, -1f, 26f, 2, 108, 4);        // X=-1..0, Y=-28..26
        for (int x = 0; x < 86; x += 20)
            p.SpawnCeilingSized(env, x, 26f, Mathf.Min(20, 86 - x));
        foreach (float rx in new[] { 9f, 19f, 35f, 46f, 58f, 80f })
            p.SpawnBlock(p.BlockGreen, env, rx, 25f, 2, 2, 4);

        // Lower Battlement Floor (X = 0..57, Y = 10) & Piers down to -28
        p.Spawn(p.Ground20, env, 0f, 10f);
        p.Spawn(p.Ground20, env, 20f, 10f);
        p.SpawnGroundSized(env, 40f, 10f, 17);                      // Meets Slope_45_Large at X=57
        foreach (float px in new[] { 0f, 20f, 40f, 56f })
            p.SpawnBlock(p.BlockCyan, env, px, 9f, 2, 74, 4);       // Y=-28..9

        // Battlement Upper OneWay Walkways & Purple TouchWall Doorways
        p.Spawn(p.BouncePadMini, inter, 9f, 10f);
        p.Spawn(p.OneWayPlatformLarge, env, 10f, 16f);              // X=10..18
        p.SpawnBlock(p.BlockGreen, env, 14f, 16f, 2, 6, 4);         // Lintel Y=13..16
        p.SpawnTouchWallCol(inter, 14f, 11f, 13f);                  // Door 1 Y=10..13

        p.Spawn(p.FireJet, haz, 21f, 10f);
        GameObject sawAct1 = p.Spawn(p.SawSlow, haz, 29f, 11.2f);
        ConfigureSaw(sawAct1, new Vector3(5f, 0f, 0f), 2.4f);
        p.SpawnBlock(p.BlockGreen, env, 38f, 16f, 2, 6, 4);
        p.SpawnTouchWallCol(inter, 38f, 11f, 13f);                  // Door 2 Y=10..13

        // Upper Secret Wall Alcove (X = 20..39, Y = 16..25): DashWall door -> FireJet_Down -> 2 Atoms
        p.Spawn(p.Ledge8, env, 20f, 16f);                           // X=20..24
        p.Spawn(p.OneWayPlatformLarge, env, 24f, 16f);              // X=24..32
        p.SpawnBlock(p.BlockMagenta, env, 25f, 25f, 2, 12, 4);      // Left lintel Y=19..25
        for (int y = 17; y <= 19; y++) p.Spawn(p.DashWall, inter, 25f, y); // Yellow secret wall!
        p.Spawn(p.FireJetDown, haz, 28f, 25f);
        SetAtomId(p.Spawn(p.Atom, col, 27f, 17.8f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 29.5f, 17.8f), 2);
        p.SpawnBlock(p.BlockMagenta, env, 31f, 25f, 2, 12, 4);      // Right lintel Y=19..25
        p.SpawnTouchWallCol(inter, 31f, 17f, 19f);
        p.Spawn(p.OneWayPlatformLarge, env, 32f, 16f);              // X=32..40
        SetAtomId(p.Spawn(p.Atom, col, 35.5f, 17.8f), 3);

        // Ignition Gate Breakout: DashBoost_Fast -> Slope_45_Large -> DashZone -> DashWall_ChainRow 1 Gate
        p.Spawn(p.DashBoostFast, inter, 50f, 11.5f);
        p.Spawn(p.Slope45Large, env, 59f, 10f);                     // X=57..61, Y=10..14
        p.SpawnGroundSized(env, 57f, 10f, 4);                       // Sub-slope backing flush w/ Y=10
        p.SpawnGroundSized(env, 61f, 14f, 25);                      // Gate deck X=61..86, Y=13..14
        p.SpawnBlock(p.BlockCyan, env, 61f, 13f, 2, 82, 4);         // Pier Y=-28..13

        GameObject dzAct1Gate = p.Spawn(p.DashZone, inter, 63.5f, 15.5f);
        ConfigureDashZoneBoost(dzAct1Gate, new Vector2(3.4f, 3.4f), 36f, 0.42f, 24f);

        p.SpawnBlock(p.BlockCyan, env, 69f, 25f, 14, 14, 4);        // Gate arch header X=69..76, Y=18..25
        p.SpawnTouchWallCol(inter, 69f, 15f, 18f);
        for (int y = 15; y <= 18; y++) p.Spawn(p.DashWallChainRow1, inter, 70f, y); // 5x4 fortress gate!
        p.SpawnTouchWallCol(inter, 75f, 15f, 18f);
        SetAtomId(p.Spawn(p.Atom, col, 79f, 15.5f), 4);

        // =====================================================================
        // SANCTUARY 1: IGNITION WATCHTOWER (X = 86 to 100, Y = 14 to 23)
        // =====================================================================
        p.SpawnGroundSized(env, 86f, 14f, 14);                      // X=86..100, Y=13..14
        p.SpawnCeilingSized(env, 86f, 23f, 14);                     // X=86..100, Y=22..23
        p.SpawnBlock(p.BlockCyan, env, 86f, 25f, 2, 4, 4);          // Step Y=23..25
        p.SpawnBlock(p.BlockGreen, env, 87f, 22f, 2, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 97f, 22f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 93f, 13f, 2, 82, 4);         // Pier Y=-28..13
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 93f, 15.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ACT 2: THE SKY-BARREL FOUNDRY (X = 100 to 180, Y = 10 to 38)
        // Towering Donkey Kong vertical/diagonal barrel chains (CameraZone_Climb):
        // DashLane_UP -> DashHold diagonal locks -> DashWall_ChainRow 1 + DashWall_Cracked
        // + DashCharge + Saw_Slow + FireJet_Down + Cannon_Fast + Diamond 1 Vault
        // =====================================================================
        GameObject czClimbAct2 = p.Spawn(p.CameraZoneClimb, flow, 140f, 24f);
        czClimbAct2.name = "CameraZone_Climb_BarrelFoundry";
        SetTriggerBoxSize(czClimbAct2, new Vector3(80f, 32f, 10f)); // X=100..180, Y=8..40

        // Foundry Framing: Roof (Y=38), Left/Right Perimeter Walls, Safety Floor (Y=10)
        p.SpawnBlock(p.BlockCyan, env, 100f, 37f, 2, 28, 4);        // X=100..101, Y=23..37
        for (int x = 100; x < 180; x += 20)
            p.SpawnCeilingSized(env, x, 38f, Mathf.Min(20, 180 - x));
        p.SpawnBlock(p.BlockCyan, env, 179f, 37f, 2, 16, 4);        // Upper right wall Y=29..37
        p.SpawnBlock(p.BlockCyan, env, 179f, 19f, 2, 18, 4);        // Seals Y=10..19 under Sanct 2

        // Entry Step Down to Foundry Floor Y=10 (X=100..104) & Continuous Floor (100..156)
        p.Spawn(p.Slope45Left, env, 101f, 12f);                     // 100..102, 12..14
        p.Spawn(p.Slope45Left, env, 103f, 10f);                     // 102..104, 10..12
        p.SpawnBlock(p.BlockCyan, env, 100f, 12f, 4, 4, 4);         // Backing Y=10..12
        p.Spawn(p.Ground20, env, 100f, 10f);
        p.Spawn(p.Ground20, env, 120f, 10f);
        p.Spawn(p.Ground20, env, 140f, 10f);
        p.SpawnGroundSized(env, 160f, 10f, 20);                     // Extends cleanly to X=180
        foreach (float px in new[] { 104f, 124f, 144f, 164f, 179f })
            p.SpawnBlock(p.BlockCyan, env, px, 9f, 2, 74, 4);       // Y=-28..9

        // 2A: Vertical DashLane_UP Rocket through 2 Ceiling Layers (Y = 10 -> 21.5)
        p.Spawn(p.BouncePad, inter, 111.5f, 10f);
        p.Spawn(p.DashLaneUp, inter, 111.5f, 15.5f);                // Y=11.5..19.5
        p.SpawnBlock(p.BlockCyan, env, 107f, 21f, 2, 12, 4);        // Left chute Y=15..21
        p.SpawnBlock(p.BlockCyan, env, 115f, 21f, 2, 12, 4);        // Right chute Y=15..21
        for (int x = 108; x <= 114; x++) p.Spawn(p.TouchWallBlue, inter, x, 18f);
        for (int x = 108; x <= 114; x++) p.Spawn(p.DashWall, inter, x, 20f);
        p.Spawn(p.OneWayPlatformLarge, env, 108f, 21.5f);           // Catch perch X=108..116
        SetAtomId(p.Spawn(p.Atom, col, 111.5f, 23.5f), 5);

        // 2B: Diagonal Barrel 2A (120, 21 @ 35 deg) -> Straight Lock into Barrel 2B (129.5, 27.6 @ 0 deg)
        GameObject barrel2A = p.Spawn(p.DashHold, inter, 120f, 21f);
        ConfigureDashZoneHold(barrel2A, 35f, new Vector2(3.0f, 3.0f), 32f, 0.36f, 22f, 3.5f);

        GameObject sawFoundry = p.Spawn(p.SawSlow, haz, 124f, 21.5f);
        ConfigureSaw(sawFoundry, new Vector3(0f, 4.5f, 0f), 2.4f);
        for (int x = 123; x <= 125; x++)
        {
            for (int y = 24; y <= 26; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
            }
        }

        GameObject barrel2B = p.Spawn(p.DashHold, inter, 129.5f, 27.6f); // Exact zero-g lock!
        ConfigureDashZoneHold(barrel2B, 0f, new Vector2(3.2f, 3.2f), 35f, 0.44f, 22f, 3.5f);

        p.Spawn(p.FireJetDown, haz, 133f, 37f);
        p.Spawn(p.OneWayPlatformLarge, env, 134f, 25.5f);           // Supports Sky Gate X=134..142
        p.SpawnBlock(p.BlockMagenta, env, 135f, 37f, 14, 14, 4);    // Header X=135..142, Y=30..37
        p.SpawnTouchWallCol(inter, 135f, 27f, 30f);
        for (int y = 27; y <= 30; y++) p.Spawn(p.DashWallChainRow1, inter, 136f, y); // 5x4 yellow row!
        p.SpawnTouchWallCol(inter, 141f, 27f, 30f);

        p.Spawn(p.DashCharge, inter, 145f, 28f);                    // Mid-air extra dash
        p.Spawn(p.OneWayPlatformLarge, env, 146f, 25f);             // X=146..154
        SetAtomId(p.Spawn(p.Atom, col, 150f, 27f), 6);

        // 2C: Exit Approach Deck (Y=20) w/ Cannon_Fast + High Diamond 1 Secret Alcove (Y=30)
        p.SpawnGroundSized(env, 156f, 20f, 24);                     // X=156..180, Y=19..20 (flush Sanct 2)
        p.SpawnBlock(p.BlockCyan, env, 156f, 19f, 2, 18, 4);        // Support leg Y=10..19

        p.SpawnBlock(p.BlockMagenta, env, 175f, 24f, 4, 1, 4);      // Bracket X=175..177, Y=23.5..24
        GameObject cannonAct2 = p.Spawn(p.CannonFast, haz, 176f, 24.8f);
        ConfigureCannon(cannonAct2, new Vector3(0f, 0f, 180f), 11f, 2.2f, 20f);

        p.Spawn(p.BouncePadMega, inter, 158f, 20f);
        p.Spawn(p.OneWayPlatformLarge, env, 161f, 30f);             // Secret rafter X=161..169
        p.SpawnBlock(p.BlockGreen, env, 161f, 37f, 2, 8, 4);        // Lintel Y=33..37
        for (int y = 31; y <= 33; y++) p.Spawn(p.DashWall, inter, 161f, y); // Yellow seal Y=30..33
        SetDiamondId(p.Spawn(p.Diamond, col, 165.5f, 31.8f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 168.5f, 31.8f), 7);

        // Foundry Recovery Catch-Lanes (Y = 10)
        p.Spawn(p.FireJet, haz, 120f, 10f);
        p.Spawn(p.BouncePadMega, inter, 128f, 10f);
        p.Spawn(p.FireJet, haz, 138f, 10f);
        p.Spawn(p.BouncePadMega, inter, 145f, 10f);
        p.Spawn(p.OneWayPlatformShort, env, 147f, 16f);
        p.Spawn(p.DashLaneUp, inter, 152f, 16f);                    // Y=12..20 boosts up to 156,20!
        p.Spawn(p.BouncePad, inter, 152f, 10f);

        // =====================================================================
        // SANCTUARY 2: FOUNDRY CROWN SANCTUM (X = 180 to 194, Y = 20 to 29)
        // =====================================================================
        p.SpawnGroundSized(env, 180f, 20f, 14);                     // X=180..194, Y=19..20
        p.SpawnCeilingSized(env, 180f, 29f, 14);                    // X=180..194, Y=28..29
        p.SpawnBlock(p.BlockMagenta, env, 181f, 28f, 2, 4, 4);
        p.SpawnBlock(p.BlockGreen, env, 191f, 28f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 187f, 19f, 2, 94, 4);        // Pier Y=-28..19
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 187f, 21.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ACT 3: THE SUNKEN EXCAVATION VAULTS (X = 194 to 288, Y = -8 to 26)
        // Tight underground catacombs (CameraZone_Tight): Ground Pound drops through
        // GroundPoundChainRow 2 & CrackedBlocks_4x4 -> Secret Diamond 2 Vault ->
        // Split Upper Trap Rafter (CrumblingBlock/SpinArm/StompBlock) vs Lower Ice Slide
        // =====================================================================
        GameObject czTightCatacombs = p.Spawn(p.CameraZoneTight, flow, 214f, 12f);
        czTightCatacombs.name = "CameraZone_Tight_Excavation";
        SetTriggerBoxSize(czTightCatacombs, new Vector3(40f, 32f, 10f)); // X=194..234, Y=-4..28

        GameObject czTightSplit = p.Spawn(p.CameraZoneTight, flow, 261f, 0f);
        czTightSplit.name = "CameraZone_Tight_SplitVaults";
        SetTriggerBoxSize(czTightSplit, new Vector3(54f, 28f, 10f));     // X=234..288, Y=-14..14

        // Roof stepping from Y=26 (194..234) down to Y=13 (234..288)
        for (int x = 194; x < 234; x += 20)
            p.SpawnCeilingSized(env, x, 26f, Mathf.Min(20, 234 - x));
        p.SpawnBlock(p.BlockCyan, env, 234f, 25f, 2, 24, 4);        // Step wall X=234..235, Y=13..25
        for (int x = 234; x < 288; x += 20)
            p.SpawnCeilingSized(env, x, 13f, Mathf.Min(20, 288 - x));

        // 3A: Shaft 1 (X = 198..208, Y = 20 -> 10)
        p.SpawnGroundSized(env, 194f, 20f, 4);                      // Threshold X=194..198
        p.SpawnBlock(p.BlockCyan, env, 197f, 19f, 2, 18, 4);        // Left wall X=197..198, Y=10..19
        p.SpawnBlock(p.BlockCyan, env, 208f, 25f, 2, 20, 4);        // Right baffle X=208..209, Y=15..25 (5m arch below)
        p.Spawn(p.OneWayPlatformLarge, env, 198f, 20f);
        p.Spawn(p.OneWayPlatformShort, env, 206f, 20f);

        p.Spawn(p.GroundPoundChainRow2, inter, 199f, 14f);          // X=199..200, Y=13..18
        p.Spawn(p.GroundPoundChainRow2, inter, 200f, 14f);          // X=200..201, Y=13..18
        p.Spawn(p.GroundPoundChainRow2, inter, 205f, 14f);          // X=205..206, Y=13..18
        p.Spawn(p.GroundPoundChainRow2, inter, 206f, 14f);          // X=206..207, Y=13..18
        p.Spawn(p.CrackedBlocks4x4, inter, 201f, 18f);              // X=201..203, Y=16..18
        p.Spawn(p.CrackedBlocks4x4, inter, 203f, 18f);              // X=203..205, Y=16..18
        for (int x = 201; x <= 204; x++) p.Spawn(p.TouchWallBlue, inter, x, 14f); // Cushion Y=13..14

        // Mid Catacomb Deck (Y = 10, X = 197..216) & Piers
        p.SpawnGroundSized(env, 197f, 10f, 19);                     // Ends X=216
        p.SpawnBlock(p.BlockCyan, env, 197f, 9f, 2, 74, 4);         // Pier Y=-28..9
        SetAtomId(p.Spawn(p.Atom, col, 211.5f, 11.5f), 8);

        // 3B: Shaft 2 (X = 216..226, Y = 10 -> -4) & Secret Diamond 2 Vault (X = 226..248, Y = 2..9)
        p.SpawnBlock(p.BlockCyan, env, 215f, 9f, 2, 26, 4);         // Left wall X=215..216, Y=-4..9
        p.SpawnBlock(p.BlockCyan, env, 226f, 25f, 2, 32, 4);        // Upper shaft seal X=226..227, Y=9..25
        p.Spawn(p.OneWayPlatformLarge, env, 216f, 10f);
        p.Spawn(p.OneWayPlatformShort, env, 224f, 10f);
        for (int x = 216; x <= 225; x++) p.Spawn(p.DashWall, inter, x, 8f); // Seal 1 Y=7..8

        p.Spawn(p.OneWayPlatformLarge, env, 216f, 2f);              // Mid-shaft shelf Y=2
        p.Spawn(p.OneWayPlatformShort, env, 224f, 2f);

        // Secret Sunken Vault (X = 226..248, Y = 2..9) behind Yellow DashWall (226, Y=2..5)
        p.SpawnGroundSized(env, 226f, 2f, 22);                      // Vault floor X=226..248, Y=1..2
        p.SpawnBlock(p.BlockMagenta, env, 226f, 9f, 44, 2, 4);      // Vault ceiling X=226..248, Y=8..9
        p.SpawnBlock(p.BlockMagenta, env, 247f, 8f, 2, 12, 4);      // Back wall X=247..248, Y=2..8
        p.SpawnBlock(p.BlockMagenta, env, 226f, 8f, 2, 6, 4);       // Lintel X=226..227, Y=5..8
        for (int y = 3; y <= 5; y++) p.Spawn(p.DashWall, inter, 226f, y); // Yellow secret door Y=2..5!
        p.Spawn(p.FireJet, haz, 233f, 2f);
        GameObject sawVault3 = p.Spawn(p.SawSlow, haz, 236f, 3.2f);
        ConfigureSaw(sawVault3, new Vector3(5f, 0f, 0f), 2.5f);
        SetAtomId(p.Spawn(p.Atom, col, 230.5f, 4.5f), 9);
        SetDiamondId(p.Spawn(p.Diamond, col, 244.5f, 3.5f), 2);

        // Lower Shaft 2: GroundPoundChainRow 2 crystal core (Y = -4..1)
        for (int x = 217; x <= 225; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, -3f);         // Y=-4..1 (below Y=2 OneWayPlatform!)

        // Underpass beneath Vault (X = 215..250, Y = -4) & Split Dual-Path Entry
        p.Spawn(p.Ground20, env, 215f, -4f);                        // 215..235
        p.SpawnGroundSized(env, 235f, -4f, 15);                     // 235..250 (Drop starts at X=250)
        p.SpawnBlock(p.BlockCyan, env, 215f, -5f, 2, 46, 4);        // Pier Y=-28..-5
        p.SpawnBlock(p.BlockCyan, env, 235f, -5f, 2, 46, 4);        // Pier Y=-28..-5
        SetAtomId(p.Spawn(p.Atom, col, 240f, -2.5f), 10);

        // Split Path A – Upper Trap Rafter (X = 250..282, Y = 4)
        p.Spawn(p.BouncePad, inter, 250f, -4f);
        p.Spawn(p.OneWayPlatformLarge, env, 251f, 4f);              // X=251..259
        p.Spawn(p.CrumblingBlock, env, 261f, 4.5f);
        GameObject spinCatacomb = p.Spawn(p.SpinArm, haz, 265f, 6.2f);
        ConfigureSpinArm(spinCatacomb, 85f);
        p.Spawn(p.CrumblingBlock, env, 269f, 4.5f);
        p.Spawn(p.OneWayPlatform, env, 272f, 4f);                   // X=272..276
        GameObject stompUpper = p.Spawn(p.StompBlock, haz, 274f, 8.5f);
        ConfigureStompBlock(stompUpper, 0.2f);
        SetAtomId(p.Spawn(p.Atom, col, 278f, 5.5f), 11);

        // Split Path B – Lower High-Speed Ice Slide (X = 250..288, Y = -4 -> -8)
        p.Spawn(p.Slope45Left, env, 251f, -6f);                     // 250..252, -6..-4
        p.Spawn(p.Slope45Left, env, 253f, -8f);                     // 252..254, -8..-6
        p.SpawnBlock(p.BlockCyan, env, 250f, -6f, 4, 4, 4);         // Backing 250..252, -8..-6
        p.SpawnGroundSized(env, 250f, -8f, 4);                      // 250..254, -9..-8
        p.SpawnBlock(p.BlockCyan, env, 250f, -9f, 2, 38, 4);        // Pier Y=-28..-9

        p.Spawn(p.IceSurface, env, 254f, -8f);                      // 254..260, -9..-8
        p.SpawnGroundSized(env, 260f, -8f, 2);                      // 260..262
        p.Spawn(p.IceSurface, env, 262f, -8f);                      // 262..268, -9..-8
        p.Spawn(p.Ground20, env, 268f, -8f);                        // 268..288, -9..-8
        p.SpawnBlock(p.BlockCyan, env, 268f, -9f, 2, 38, 4);        // Pier Y=-28..-9
        p.SpawnBlock(p.BlockCyan, env, 287f, -9f, 2, 38, 4);        // Pier Y=-28..-9

        GameObject dbIceSlide = p.Spawn(p.DashBoost, inter, 256f, -6.5f);
        ConfigureDashZoneBoost(dbIceSlide, new Vector2(3f, 3f), 32f, 0.35f, 22f);
        GameObject stompLower = p.Spawn(p.StompBlock, haz, 263f, -3.5f);
        ConfigureStompBlock(stompLower, 0.45f);
        p.Spawn(p.DashWallChainRow1, inter, 271f, -7f);             // Smashable 2-high yellow row along ice slide!
        p.Spawn(p.DashWallChainRow1, inter, 271f, -6f);
        p.SpawnTouchWallCol(inter, 276f, -7f, -5f);

        // Merge Launch up to Sanctuary 3 (Y = -8 -> Y = 4)
        p.Spawn(p.BouncePadMega, inter, 282f, -8f);
        p.Spawn(p.DashLaneUp, inter, 282f, -1f);                    // Y=-5..3
        p.SpawnBlock(p.BlockCyan, env, 287f, 3f, 2, 22, 4);         // Wall under Sanct 3 X=287..288, Y=-8..3

        // =====================================================================
        // SANCTUARY 3: ROYAL THRESHOLD (X = 288 to 302, Y = 4 to 13)
        // =====================================================================
        p.SpawnGroundSized(env, 288f, 4f, 14);                      // X=288..302, Y=3..4
        p.SpawnCeilingSized(env, 288f, 13f, 14);                    // X=288..302, Y=12..13
        p.SpawnBlock(p.BlockGreen, env, 289f, 12f, 2, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 299f, 12f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 295f, 3f, 2, 62, 4);         // Pier Y=-28..3
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 295f, 5.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ACT 4: THE PURPLE CROWN HYPER-FINALE (X = 302 to 406, Y = -4 to 32)
        // High-ceilinged Crown Throne Hall: BouncePad_Mega -> sweeping Glide slicing
        // suspended Purple curtains (Diamond 3 high rafter) -> DashBoost_Fast ->
        // DashHold (35 deg) -> Sky Cube -> DashLane_UP + DashBoost_Up -> DashHold (-18 deg)
        // shattering 6x8 Grand Crown Monolith straight into End_Portal!
        // =====================================================================
        GameObject czWideCrown = p.Spawn(p.CameraZoneWide, flow, 355f, 14f);
        czWideCrown.name = "CameraZone_Wide_CrownFinale";
        SetTriggerBoxSize(czWideCrown, new Vector3(106f, 40f, 10f)); // X=302..408, Y=-6..34

        // Sweeping Crown Roof (Y=32) & Right Citadel Back Wall (X=406)
        p.SpawnBlock(p.BlockCyan, env, 302f, 31f, 2, 36, 4);        // Step above Sanct 3 roof Y=13..31
        for (int x = 302; x < 406; x += 20)
            p.SpawnCeilingSized(env, x, 32f, Mathf.Min(20, 406 - x));
        p.SpawnBlock(p.BlockCyan, env, 406f, 32f, 2, 120, 4);       // Back wall X=406..407, Y=-28..32

        // Continuous Recovery Safety Floor (X = 302..406, Y = -4)
        for (int x = 302; x < 406; x += 20)
            p.SpawnGroundSized(env, x, -4f, Mathf.Min(20, 406 - x));
        foreach (float px in new[] { 318f, 338f, 358f, 378f, 398f })
            p.SpawnBlock(p.BlockCyan, env, px, -5f, 2, 46, 4);      // Piers Y=-28..-5

        // Launch Terrace (Y = 4) -> BouncePad_Mega -> Extended Glide Curtains + Diamond 3 Rafter
        p.SpawnGroundSized(env, 302f, 4f, 8);                       // X=302..310, Y=3..4
        p.SpawnBlock(p.BlockCyan, env, 309f, 3f, 2, 14, 4);         // Pier down to -4
        p.Spawn(p.BouncePadMega, inter, 307f, 4f);

        p.SpawnBlock(p.BlockCyan, env, 315f, 31f, 2, 22, 4);        // Curtain 1 hanger Y=20..31
        p.SpawnTouchWallCol(inter, 315f, 11f, 20f);                 // 10m tall purple glass curtain!
        SetAtomId(p.Spawn(p.Atom, col, 319.5f, 14.5f), 12);

        p.Spawn(p.OneWayPlatformLarge, env, 321f, 18.5f);           // High Crown Glide Rafter X=321..329
        SetDiamondId(p.Spawn(p.Diamond, col, 325f, 19.8f), 3);

        p.SpawnBlock(p.BlockMagenta, env, 325f, 16f, 2, 2, 4);      // Pendant below rafter Y=15..16
        p.SpawnTouchWallCol(inter, 325f, 10f, 15f);
        SetAtomId(p.Spawn(p.Atom, col, 330f, 13.0f), 13);

        p.SpawnBlock(p.BlockCyan, env, 335f, 31f, 2, 26, 4);        // Curtain 3 hanger Y=18..31
        p.SpawnTouchWallCol(inter, 335f, 9f, 18f);
        SetAtomId(p.Spawn(p.Atom, col, 339f, 12.0f), 14);

        // Glide Landing Runway (Y = 8, X = 342..358) -> Uninterrupted 4-Link Final Chain
        p.SpawnGroundSized(env, 342f, 8f, 16);                      // X=342..358, Y=7..8
        p.SpawnBlock(p.BlockCyan, env, 342f, 7f, 2, 22, 4);         // Pier Y=-4..7
        p.SpawnBlock(p.BlockCyan, env, 357f, 7f, 2, 22, 4);         // Pier Y=-4..7

        GameObject dbCrownEntry = p.Spawn(p.DashBoostFast, inter, 345f, 9.5f);
        ConfigureDashZoneBoost(dbCrownEntry, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f);

        p.SpawnBlock(p.BlockMagenta, env, 350f, 31f, 14, 36, 4);    // Header X=350..357, Y=13..31
        p.SpawnTouchWallCol(inter, 350f, 9f, 13f);
        for (int y = 9; y <= 13; y++) p.Spawn(p.DashWallChainRow1, inter, 351f, y); // 5x5 Crown Monolith A!
        p.SpawnTouchWallCol(inter, 356f, 9f, 13f);
        SetAtomId(p.Spawn(p.Atom, col, 359f, 9.5f), 15);

        // Barrel 4A (361.5, 9.5 @ 35 deg) -> Sky Cube -> DashLane_UP (374, 17) + DashBoost_Up (374, 21.5)
        GameObject barrel4A = p.Spawn(p.DashHold, inter, 361.5f, 9.5f);
        ConfigureDashZoneHold(barrel4A, 35f, new Vector2(3.4f, 3.4f), 34f, 0.42f, 24f, 0.20f);

        for (int x = 366; x <= 368; x++)
        {
            for (int y = 13; y <= 16; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
            }
        }

        p.Spawn(p.DashLaneUp, inter, 374f, 17f);                    // Y=13..21 catches 35-deg beam cleanly
        GameObject boost4Up = p.Spawn(p.DashBoostUp, inter, 374f, 22f); // Y=20.5..23.5
        ConfigureDashZoneBoost(boost4Up, new Vector2(3.2f, 3.2f), 34f, 0.22f, 20f);
        for (int x = 371; x <= 376; x++) p.Spawn(p.TouchWallBlue, inter, x, 24f); // High curtain Y=23..24

        // Finale Cannon Barrel 4C (374, 26.5 @ -18 deg) -> Grand Monolith (382..388, Y=16..24) -> End_Portal!
        GameObject barrel4C = p.Spawn(p.DashHold, inter, 374f, 26.5f);
        ConfigureDashZoneHold(barrel4C, -18f, new Vector2(3.4f, 3.4f), 36f, 0.36f, 26f, 0.20f);
        SetAtomId(p.Spawn(p.Atom, col, 379.5f, 24.7f), 16);

        p.Spawn(p.OneWayPlatformLarge, env, 381f, 16f);             // Supports Grand Monolith X=381..389
        p.SpawnBlock(p.BlockCyan, env, 382f, 31f, 14, 14, 4);       // Header X=382..389, Y=24..31
        p.SpawnTouchWallCol(inter, 382f, 17f, 24f);
        for (int y = 17; y <= 24; y++) p.Spawn(p.DashWallChainRow1, inter, 383f, y); // 5x8 = 40 yellow blocks!!
        p.SpawnTouchWallCol(inter, 388f, 17f, 24f);

        // Purple Crown Finale Throne Dais (X = 390..406, Y = 16) -> End_Portal (399, 17.8)
        p.SpawnGroundSized(env, 390f, 16f, 16);                     // X=390..406, Y=15..16
        p.SpawnBlock(p.BlockMagenta, env, 390f, 15f, 2, 38, 4);     // Dais pier X=390..391, Y=-4..15
        p.SpawnBlock(p.BlockGreen, env, 392f, 31f, 28, 2, 4);       // Crown canopy trim
        p.SpawnBlock(p.BlockMagenta, env, 392f, 30f, 2, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 404f, 30f, 2, 2, 4);

        // Act 4 Continuous Recovery Catch-Lanes (Y = -4)
        p.Spawn(p.BouncePadMega, inter, 322f, -4f);
        p.Spawn(p.BouncePadMega, inter, 338f, -4f);
        p.Spawn(p.BouncePadMega, inter, 362f, -4f);
        p.Spawn(p.OneWayPlatform, env, 372f, 5f);
        p.Spawn(p.BouncePad, inter, 384f, -4f);
        p.Spawn(p.DashLaneUp, inter, 384f, 3f);                     // Y=-1..7
        GameObject boostCrownRecover = p.Spawn(p.DashBoostUp, inter, 384f, 9.5f);
        ConfigureDashZoneBoost(boostCrownRecover, new Vector2(3f, 3f), 32f, 0.25f, 20f);

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
        return $"Level_40 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 58f;
            lm.TwoStarTime = 90f;
            lm.OneStarTime = 150f;
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
            respawnMgr.KillY = -33f;
            respawnMgr.KillTopY = 64f;
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
        public GameObject Ledge8 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ledge_8x1.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject BlockGreen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Green Variant.prefab");
        public GameObject BlockMagenta = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject OneWayPlatformLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject OneWayPlatformShort = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab");
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
        public GameObject DashBoost = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashBoost.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashBoostUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Up.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashHold = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashHold.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");

        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject CannonFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Cannon_Fast.prefab");
        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject FireJetDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Down.prefab");
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

        public void SpawnTouchWallCol(Transform parent, float x, float startY, float endY)
        {
            for (float y = startY; y <= endY; y += 1f)
                Spawn(TouchWallBlue, parent, x, y);
        }
    }
}
#endif
