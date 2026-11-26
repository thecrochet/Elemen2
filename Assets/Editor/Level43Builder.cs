#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level43Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_43.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 43 (Orbit Barrel Citadel)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 108f, -36f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(360f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(360f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -8 to 224)
        for (int x = -8; x < 224; x += 20)
            p.SpawnGroundSized(env, x, -28f, Mathf.Min(20, 224 - x));

        // =====================================================================
        // FLOW: CENTRAL START HUB & SUSPENDED FINALE PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 117f, 4f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 105f, 16.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Orbit mosaic matched to each directional pivot)
        // =====================================================================
        GameObject czZ1Start = p.Spawn(p.CameraZoneTight, flow, 99f, 6.5f);
        czZ1Start.name = "CameraZone_Tight_Z1Vestibule";
        SetTriggerBoxSize(czZ1Start, new Vector3(48f, 17f, 10f));      // X=75..123, Y=-2..15

        GameObject czZ1Gorge = p.Spawn(p.CameraZoneWide, flow, 38f, 1.5f);
        czZ1Gorge.name = "CameraZone_Wide_Z1WestGorge";
        SetTriggerBoxSize(czZ1Gorge, new Vector3(74f, 23f, 10f));      // X=1..75, Y=-10..13

        GameObject czZ2Tunnel = p.Spawn(p.CameraZoneTight, flow, 88f, -15f);
        czZ2Tunnel.name = "CameraZone_Tight_Z2SubTunnel";
        SetTriggerBoxSize(czZ2Tunnel, new Vector3(172f, 12f, 10f));    // X=2..174, Y=-21..-9

        GameObject czZ2Vault = p.Spawn(p.CameraZoneTight, flow, 104.5f, -4.5f);
        czZ2Vault.name = "CameraZone_Tight_Z2SecretRecess";
        SetTriggerBoxSize(czZ2Vault, new Vector3(20f, 9f, 10f));       // X=94.5..114.5, Y=-9..0

        GameObject czZ3Climb = p.Spawn(p.CameraZoneClimb, flow, 189f, 12f);
        czZ3Climb.name = "CameraZone_Climb_Z3EastFoundry";
        SetTriggerBoxSize(czZ3Climb, new Vector3(58f, 66f, 10f));      // X=160..218, Y=-21..45

        GameObject czZ4Skylight = p.Spawn(p.CameraZoneWide, flow, 124f, 36.5f);
        czZ4Skylight.name = "CameraZone_Wide_Z4CrownSkylight";
        SetTriggerBoxSize(czZ4Skylight, new Vector3(100f, 21f, 10f));  // X=74..174, Y=26..47

        GameObject czZ4Finale = p.Spawn(p.CameraZoneTight, flow, 98f, 20f);
        czZ4Finale.name = "CameraZone_Tight_Z4FinaleSanctuary";
        SetTriggerBoxSize(czZ4Finale, new Vector3(28f, 12f, 10f));     // X=84..112, Y=14..26

        // =====================================================================
        // CENTRAL START VESTIBULE (X = 104 to 122, Y = 4 to 13 -> Shoot LEFT!)
        // =====================================================================
        p.SpawnGroundSized(env, 104f, 4f, 18);                         // X=104..122, Y=3..4
        p.SpawnCeilingSized(env, 104f, 13f, 18);                       // X=104..122, Y=12..13
        p.SpawnBlock(p.BlockCyan, env, 121f, 12f, 2, 16, 4);           // Vestibule East Wall X=121..122, Y=4..12
        p.SpawnBlock(p.BlockGreen, env, 113f, 12f, 8, 2, 4);           // Vestibule trim Y=11..12
        p.SpawnBlock(p.BlockCyan, env, 114f, 3f, 16, 6, 4);            // Masonry slab X=114..122, Y=0..3
        p.SpawnBlock(p.BlockCyan, env, 114f, 0f, 16, 18, 4);           // Solid support pier X=114..122, Y=-9..0!

        p.SpawnTouchWallCol(inter, 111f, 5f, 8f);                      // Opening smash doorway going LEFT!
        GameObject dbStartLeft = p.Spawn(p.DashBoostLeft, inter, 107f, 5.5f);
        ConfigureDashZoneBoost(dbStartLeft, new Vector2(3.2f, 3.2f), 34f, 0.36f, 22f);

        // =====================================================================
        // ZONE 1: THE WESTWARD BARREL GORGE (Start Center -> Rocket LEFT & DOWN!)
        // X = 18 to 104, Y = +4 down to -8 -> Far-Left Cliff Alcove & Drop to Sanct 1
        // =====================================================================
        for (int x = 74; x < 104; x += 20)
            p.SpawnCeilingSized(env, x, 13f, Mathf.Min(20, 104 - x));  // Upper Gorge Ceiling Y=12..13
        p.SpawnBlock(p.BlockCyan, env, 73f, 12f, 2, 8, 4);             // Ceiling step X=73..74, Y=8..12
        for (int x = 18; x < 74; x += 20)
            p.SpawnCeilingSized(env, x, 8f, Mathf.Min(20, 74 - x));    // West Gorge Ceiling Y=7..8

        // Step 1: Purple Curtain -> Slope_45_Large (96, 0) sliding DOWN-LEFT -> Deck 1B (76..94, Y=0)
        for (int x = 99; x <= 102; x++)
            for (int y = 5; y <= 10; y++)
                if (x + y >= 105) p.Spawn(p.TouchWallBlue, inter, x, y);

        p.SpawnGroundSized(env, 98f, 4f, 6);                           // X=98..104, Y=3..4
        p.SpawnBlock(p.BlockCyan, env, 98f, 3f, 12, 6, 4);             // Backing X=98..104, Y=0..3
        p.Spawn(p.Slope45Large, env, 96f, 0f);                         // X=94..98, Y=0..4 (slides DOWN-LEFT cleanly!)
        p.SpawnGroundSized(env, 76f, 0f, 18);                          // Deck 1B X=76..94, Y=-1..0 (clean left of Slope!)

        // Playable Under-Deck Smash Pocket beneath Deck 1B (X = 76..94, Y = -8..0)
        p.Spawn(p.BouncePadMini, inter, 78f, -8f);
        for (int x = 81; x <= 90; x++)
            for (int y = -7; y <= -4; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Thick lower smash cache!

        // Jagged Purple Stalactite Curtain (X = 83..89, Y = 2..12) + Saw + Atom 1
        for (int x = 83; x <= 89; x++)
        {
            int bottomY = 2 + Mathf.Abs(x - 86);
            for (int y = bottomY; y <= 12; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        }
        GameObject saw1 = p.Spawn(p.SawSlow, haz, 79f, 2f);
        ConfigureSaw(saw1, new Vector3(7f, 0f, 0f), 2.4f);
        SetAtomId(p.Spawn(p.Atom, col, 77.5f, 3.8f), 1);

        // Step 2: DashBoost_Left -> OneWay -> Barrel 1A (195 deg) -> Stepped Curtain -> Gorge Floor (-8)
        GameObject dbGorgeLeft = p.Spawn(p.DashBoostLeft, inter, 73f, 1.5f);
        ConfigureDashZoneBoost(dbGorgeLeft, new Vector2(3.2f, 3.2f), 34f, 0.36f, 22f);
        p.Spawn(p.OneWayPlatformLarge, env, 68f, 0f);                  // X=68..76 (soft catch!)

        GameObject barrel1A = p.Spawn(p.DashHold, inter, 64f, 2f);
        ConfigureDashZoneHold(barrel1A, 196f, new Vector2(3.4f, 3.4f), 34f, 0.36f, 22f, 0.22f); // Fires DOWN-LEFT!

        // Stepped Curtain & Cracked Core (X = 54..60, Y = -5..+3)
        for (int x = 54; x <= 60; x++)
        {
            for (int y = -5; y <= 3; y++)
            {
                if (x >= 56 && x <= 58 && y >= -2 && y <= 0)
                    p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
                else if (Mathf.Abs(x - 57f) + Mathf.Abs(y + 1f) <= 4.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
            }
        }

        // Gorge Floor Y = -8 (X = 26..94, Y = -9..-8 — flush atop SubTunnel Ceiling Y=-10..-9!)
        p.SpawnGroundSized(env, 26f, -8f, 68);                         // X=26..94, Y=-9..-8
        p.Spawn(p.BouncePad, inter, 66f, -8f);                         // Mid-gorge recovery up to OneWay(68,0)!
        p.Spawn(p.BouncePad, inter, 50f, -8f);                         // Catches Barrel 1A drop & recovers UP!
        p.Spawn(p.FireJet, haz, 44f, -8f);

        // Step 3: Barrel 1B (172 deg) -> Diagonal Purple Waterfall -> Far-Left Cliff Alcove & Drop Hatch!
        p.Spawn(p.OneWayPlatform, env, 39f, -2f);                      // X=39..44
        p.Spawn(p.BouncePad, inter, 37f, -8f);
        GameObject barrel1B = p.Spawn(p.DashHold, inter, 36f, 0f);
        ConfigureDashZoneHold(barrel1B, 174f, new Vector2(3.4f, 3.4f), 34f, 0.38f, 22f, 0.22f); // Fires LEFT!

        for (int x = 27; x <= 32; x++)
            for (int y = -5; y <= 5; y++)
                if ((x + y) % 2 == 0 || (y >= -2 && y <= 2))
                    p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject saw1B = p.Spawn(p.SawSlow, haz, 27f, -6f);
        ConfigureSaw(saw1B, new Vector3(6f, 0f, 0f), 2.4f);

        // West Outer Framing Wall (X = 1..2, Y = -28..8) + Far-Left Secret Cliff Alcove (X = 10..18, Y = -8..0)
        p.SpawnBlock(p.BlockCyan, env, 1f, 8f, 2, 72, 4);              // West Fortress Wall X=1..2, Y=-28..8
        p.SpawnCeilingSized(env, 2f, 8f, 16);                          // Overhang roof X=2..18, Y=7..8

        p.SpawnGroundSized(env, 10f, -8f, 8);                          // Secret Alcove Floor X=10..18, Y=-9..-8
        p.SpawnCeilingSized(env, 10f, 1f, 8);                          // Secret Alcove Roof X=10..18, Y=0..1
        p.SpawnBlock(p.BlockCyan, env, 10f, 0f, 2, 16, 4);             // Alcove Back Wall X=10..11, Y=-8..0
        for (int y = -7; y <= 0; y++) p.Spawn(p.DashWall, inter, 18f, y); // False yellow door X=18, Y=-8..0!
        p.Spawn(p.TouchWallBlue, inter, 15f, -7f);
        p.Spawn(p.TouchWallBlue, inter, 16f, -7f);
        p.Spawn(p.TouchWallBlue, inter, 15f, -6f);
        SetAtomId(p.Spawn(p.Atom, col, 13f, -6.5f), 2);
        SetAtomId(p.Spawn(p.Atom, col, 16f, -3.5f), 3);

        // Bottom-Left Multi-Layer Ground Pound Hatch (X = 18..26, Y = -8 down to -20)
        p.Spawn(p.OneWayPlatformLarge, env, 18f, -8f);                 // Grate X=18..26
        for (int x = 19; x <= 23; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, -10f); // Y=-12..-10
        for (int x = 18; x <= 25; x++) p.Spawn(p.TouchWallBlue, inter, x, -13f);       // Y=-14..-13
        for (int x = 19; x <= 24; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -18f);// Y=-19..-14 flush below TouchWall!

        // =====================================================================
        // SANCTUARY 1: CISTERN ANTECHAMBER (Bottom-Left X = 2 to 18, Y = -20 to -11)
        // =====================================================================
        p.SpawnGroundSized(env, 2f, -20f, 24);                         // Cistern Floor X=2..26, Y=-21..-20
        p.SpawnBlock(p.BlockCyan, env, 10f, -21f, 2, 14, 4);           // Pier Y=-28..-21
        p.SpawnCeilingSized(env, 2f, -11f, 16);                        // Sanct 1 Roof X=2..18, Y=-12..-11
        p.SpawnBlock(p.BlockGreen, env, 3f, -12f, 14, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 10f, -12f, 14, 2, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 9f, -18.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE SUB-HUB HYPER-TUNNEL (Sprint LEFT -> RIGHT under Spawn!)
        // X = 26 to 160, Y = -20 to -10 + Secret Diamond Recess (95..114, Y=-9..0)
        // =====================================================================
        p.SpawnGroundSized(env, 26f, -20f, 20);                        // X=26..46
        p.SpawnGroundSized(env, 46f, -20f, 14);                        // X=46..60 (flush left of IceSurface!)
        p.Spawn(p.IceSurface, env, 60f, -20f);                         // Slippery boost sprint X=60..66!
        p.SpawnGroundSized(env, 66f, -20f, 20);                        // X=66..86
        p.SpawnGroundSized(env, 86f, -20f, 20);                        // X=86..106
        p.SpawnGroundSized(env, 106f, -20f, 20);                       // X=106..126
        p.SpawnGroundSized(env, 126f, -20f, 20);                       // X=126..146
        p.SpawnGroundSized(env, 146f, -20f, 14);                       // X=146..160
        foreach (float px in new[] { 40f, 60f, 80f, 100f, 120f, 140f, 158f })
            p.SpawnBlock(p.BlockCyan, env, px, -21f, 2, 14, 4);        // Piers Y=-28..-21

        // Tunnel Ceiling split around Secret Vault Detour Hatch (95..105)
        for (int x = 26; x < 95; x += 20)
            p.SpawnCeilingSized(env, x, -9f, Mathf.Min(20, 95 - x));   // Tunnel Roof West Y=-10..-9
        p.Spawn(p.OneWayPlatformLarge, env, 95f, -9f);                 // Upward Detour Hatch X=95..103!
        p.Spawn(p.OneWayPlatformShort, env, 103f, -9f);                // X=103..105
        for (int x = 105; x < 160; x += 20)
            p.SpawnCeilingSized(env, x, -9f, Mathf.Min(20, 160 - x));  // Tunnel Roof East Y=-10..-9

        // Tunnel First Half (X = 26..95)
        GameObject dbSub1 = p.Spawn(p.DashBoostFast, inter, 28f, -18.5f);
        ConfigureDashZoneBoost(dbSub1, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f);

        // Bulkhead A (33..39, Y = -19..-13): Purple + DashWall_ChainRow 1 x6 + Purple
        p.SpawnTouchWallCol(inter, 33f, -19f, -14f);
        for (int y = -19; y <= -14; y++) p.Spawn(p.DashWallChainRow1, inter, 34f, y);
        p.SpawnTouchWallCol(inter, 39f, -19f, -14f);

        GameObject dzSub1 = p.Spawn(p.DashZone, inter, 43f, -18.5f);
        ConfigureDashZoneBoost(dzSub1, new Vector2(3.2f, 3.2f), 36f, 0.42f, 24f);

        GameObject stompSub1 = p.Spawn(p.StompBlock, haz, 47f, -12.5f);
        ConfigureStompBlock(stompSub1, 0.25f);

        for (int x = 50; x <= 56; x++)
            for (int y = -19; y <= -13; y++)
                if ((x + y) % 2 == 0 || y >= -17 && y <= -15)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 58f, -17f), 4);

        p.Spawn(p.BouncePadMini, inter, 67f, -20f);
        GameObject barrel2A = p.Spawn(p.DashHold, inter, 72f, -16f);
        ConfigureDashZoneHold(barrel2A, 0f, new Vector2(3.4f, 3.4f), 38f, 0.44f, 24f, 0.22f);

        // Bulkhead B (77..83, Y = -19..-12) + LavaLeaper
        p.SpawnTouchWallCol(inter, 77f, -19f, -12f);
        for (int y = -19; y <= -12; y++) p.Spawn(p.DashWallChainRow1, inter, 78f, y);
        p.SpawnTouchWallCol(inter, 83f, -19f, -12f);
        GameObject leaperSub1 = p.Spawn(p.LavaLeaper, haz, 89f, -20f);
        ConfigureLavaLeaper(leaperSub1, 5.5f);

        // SECRET DIAMOND 1 RECESS DIRECTLY BENEATH SPAWN HUB (X = 94..114, Y = -9..0)
        p.Spawn(p.BouncePad, inter, 100f, -20f);
        p.Spawn(p.DashLaneUp, inter, 100f, -14.5f);                    // Rockets cleanly up through 95..105 Hatch!
        p.SpawnBlock(p.BlockCyan, env, 94f, -1f, 2, 16, 4);            // Recess West Wall X=94..95, Y=-9..-1
        p.SpawnCeilingSized(env, 94f, 0f, 20);                         // Recess Ceiling X=94..114, Y=-1..0
        p.SpawnBlock(p.BlockCyan, env, 113f, -1f, 2, 16, 4);           // Recess East Wall X=113..114, Y=-9..-1
        // Note: Tunnel Roof East (105..160) has top at Y=-9, bottom Y=-10 -> serves directly as Recess Right Floor Y=-9!
        p.Spawn(p.FireJet, haz, 106f, -9f);
        p.SpawnBlock(p.BlockMagenta, env, 108f, -1f, 2, 4, 4);         // Secret door lintel Y=-3..-1
        for (int y = -8; y <= -3; y++) p.Spawn(p.DashWall, inter, 108f, y); // Yellow secret door Y=-9..-3!
        for (int y = -8; y <= -5; y++) p.Spawn(p.TouchWallBlue, inter, 109f, y);
        SetAtomId(p.Spawn(p.Atom, col, 102f, -6.5f), 5);
        SetDiamondId(p.Spawn(p.Diamond, col, 111.3f, -7.5f), 1);

        // Tunnel Second Half (X = 108..160)
        GameObject dbSub2 = p.Spawn(p.DashBoostFast, inter, 110f, -18.5f);
        ConfigureDashZoneBoost(dbSub2, new Vector2(3.2f, 3.2f), 36f, 0.42f, 24f);

        // Bulkhead C (115..121, Y = -19..-13)
        p.SpawnTouchWallCol(inter, 115f, -19f, -13f);
        for (int y = -19; y <= -13; y++) p.Spawn(p.DashWallChainRow1, inter, 116f, y);
        p.SpawnTouchWallCol(inter, 121f, -19f, -13f);

        p.Spawn(p.BouncePadMini, inter, 125f, -20f);
        GameObject barrel2B = p.Spawn(p.DashHold, inter, 130f, -16f);
        ConfigureDashZoneHold(barrel2B, 4f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 24f, 0.22f);

        GameObject stompSub2 = p.Spawn(p.StompBlock, haz, 136f, -12.5f);
        ConfigureStompBlock(stompSub2, 0.30f);
        GameObject leaperSub2 = p.Spawn(p.LavaLeaper, haz, 142f, -20f);
        ConfigureLavaLeaper(leaperSub2, 5.5f);

        for (int x = 146; x <= 152; x++)
            for (int y = -19; y <= -12; y++)
                if (x + y >= 132) p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 155.5f, -18f), 6);

        // =====================================================================
        // SANCTUARY 2: FOUNDRY INLET (Bottom-Right X = 160 to 174, Y = -20 to -11)
        // =====================================================================
        p.SpawnGroundSized(env, 160f, -20f, 14);                       // X=160..174, Y=-21..-20
        p.SpawnCeilingSized(env, 160f, -11f, 14);                      // X=160..174, Y=-12..-11
        p.SpawnBlock(p.BlockGreen, env, 161f, -12f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 167f, -12f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 167f, -21f, 2, 14, 4);          // Pier Y=-28..-21
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 167f, -18.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE JAGGED EASTERN FOUNDRY (Crooked Vertical Ascent + 2 Side Alcoves)
        // X = 174 to 207, Y = -20 up to +32 -> Exit Left into Sanctuary 3 (160..174, 32..43)
        // =====================================================================
        p.SpawnGroundSized(env, 174f, -20f, 33);                       // Chimney Floor X=174..207, Y=-21..-20
        p.SpawnBlock(p.BlockCyan, env, 184f, -21f, 2, 14, 4);
        p.SpawnBlock(p.BlockCyan, env, 205f, -21f, 2, 14, 4);
        p.SpawnCeilingSized(env, 174f, 44f, 33);                       // Crown Foundry Roof Y=43..44

        // West Chimney Wall split only around West Alcove #2 (Y=8..16) and Sanctuary 3 Exit (32..43)
        p.SpawnBlock(p.BlockCyan, env, 173f, 8f, 2, 38, 4);            // X=173..174, Y=-11..8
        p.SpawnBlock(p.BlockCyan, env, 173f, 31f, 2, 30, 4);           // X=173..174, Y=16..31 (open 8..16 & 32..43!)

        // East Chimney Wall split around East Alcove #1 (Y=-5..+3)
        p.SpawnBlock(p.BlockCyan, env, 206f, -5f, 2, 30, 4);           // Lower East Wall Y=-20..-5
        p.SpawnBlock(p.BlockCyan, env, 206f, 43f, 2, 80, 4);           // Upper East Wall Y=3..43 flush below Roof!

        // East Side Alcove #1 (X = 207..217, Y = -5..+3 -> Atom 7!)
        p.SpawnGroundSized(env, 207f, -5f, 10);                        // X=207..217, Y=-6..-5
        p.SpawnCeilingSized(env, 207f, 4f, 10);                        // X=207..217, Y=3..4
        p.SpawnBlock(p.BlockMagenta, env, 216f, 3f, 2, 16, 4);         // Back wall Y=-5..3
        p.SpawnBlock(p.BlockCyan, env, 216f, -6f, 2, 44, 4);           // Pier down to -28
        for (int y = -4; y <= 3; y++) p.Spawn(p.DashWall, inter, 206f, y); // Yellow secret door!
        for (int x = 209; x <= 211; x++)
            for (int y = -4; y <= -2; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 214f, -3.5f), 7);

        // West Side Alcove #2 (X = 160..173, Y = +8..+16 -> Atom 8!)
        p.SpawnGroundSized(env, 160f, 8f, 13);                         // Floor X=160..173, Y=7..8
        p.SpawnCeilingSized(env, 160f, 17f, 13);                       // Roof X=160..173, Y=16..17
        p.SpawnBlock(p.BlockMagenta, env, 160f, 16f, 2, 16, 4);        // West back wall Y=8..16
        for (int y = 9; y <= 16; y++) p.Spawn(p.DashWall, inter, 173f, y); // Yellow secret door!
        for (int x = 165; x <= 167; x++)
            for (int y = 9; y <= 11; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 162.5f, 9.5f), 8);

        // Foundry Ascent Tier 1 (Y = -20 -> -4)
        p.Spawn(p.BouncePadMega, inter, 182f, -20f);
        p.Spawn(p.DashLaneUp, inter, 182f, -11f);                      // Y=-15..-7 shoots UP!
        GameObject mp3A = p.Spawn(p.MovingPlatform, env, 192f, -10f);
        ConfigureMovingPlatform(mp3A, new Vector3(0f, 5f, 0f), 2.8f, 0.3f); // Optional right elevator
        p.SpawnBlock(p.BlockMagenta, env, 202f, -7f, 6, 2, 4);         // Cannon bracket X=202..205
        GameObject cannon3A = p.Spawn(p.CannonFast, haz, 203.5f, -5.8f);
        ConfigureCannon(cannon3A, new Vector3(0f, 0f, 180f), 10f, 2.4f, 24f);
        p.Spawn(p.BouncePadMega, inter, 196f, -20f);

        // Deck 3A (Y = -4): OneWay left hatch (176..184) + Solid Right Floor (188..206)
        p.Spawn(p.OneWayPlatformLarge, env, 176f, -4f);                // X=176..184
        p.SpawnGroundSized(env, 188f, -4f, 18);                        // X=188..206, Y=-5..-4
        for (int x = 177; x <= 184; x++) p.Spawn(p.TouchWallBlue, inter, x, -3f);
        for (int x = 178; x <= 184; x++) p.Spawn(p.DashWall, inter, x, -2f);

        // Foundry Mid Slalom (Y = -4 -> +19): Barrel 3A (40 deg) -> Purple Cluster -> Barrel 3B (145 deg)
        GameObject barrel3A = p.Spawn(p.DashHold, inter, 181f, 0f);
        ConfigureDashZoneHold(barrel3A, 40f, new Vector2(3.4f, 3.4f), 34f, 0.42f, 22f, 0.22f);

        for (int x = 186; x <= 192; x++)
            for (int y = 4; y <= 9; y++)
                if (Mathf.Abs(x - 189f) + Mathf.Abs(y - 6.5f) <= 3.0f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        p.Spawn(p.CrumblingBlock, env, 193f, 6f);
        p.Spawn(p.OneWayPlatformLarge, env, 175f, 8f);                 // Access shelf to West Alcove #2!
        p.Spawn(p.OneWayPlatformLarge, env, 196f, 6f);                 // Recovery shelf right
        p.Spawn(p.BouncePadMega, inter, 200f, -4f);                    // Bottom-right recovery up to 6/12!
        GameObject spin3 = p.Spawn(p.SpinArm, haz, 200f, 1.5f);
        ConfigureSpinArm(spin3, 85f);

        p.Spawn(p.DashCharge, inter, 195f, 10.5f);
        GameObject barrel3B = p.Spawn(p.DashHold, inter, 200f, 12f);
        ConfigureDashZoneHold(barrel3B, 145f, new Vector2(3.6f, 3.6f), 34f, 0.34f, 22f, 0.22f); // Fires UP-LEFT!

        // Foundry Upper Tier (Y = +19 -> +32)
        p.SpawnGroundSized(env, 175f, 19f, 10);                        // West Deck X=175..185, Y=18..19
        p.Spawn(p.OneWayPlatformLarge, env, 185f, 19f);                // East Hatch X=185..193
        for (int x = 186; x <= 193; x++) p.Spawn(p.TouchWallBlue, inter, x, 17f);
        for (int x = 187; x <= 192; x++) p.Spawn(p.DashWallCracked, inter, x + 0.5f, 17.5f);
        SetAtomId(p.Spawn(p.Atom, col, 195f, 15.5f), 9);

        p.SpawnBlock(p.BlockMagenta, env, 202f, 20f, 6, 2, 4);         // Cannon bracket X=202..205
        GameObject cannon3B = p.Spawn(p.CannonFast, haz, 203.5f, 21.2f);
        ConfigureCannon(cannon3B, new Vector3(0f, 0f, 180f), 11f, 2.3f, 25f);

        // Optional High Pinnacle Secret Shelf -> Diamond 2!
        p.Spawn(p.BouncePad, inter, 190f, 19f);
        p.Spawn(p.OneWayPlatformLarge, env, 196f, 26f);                // High Pinnacle X=196..204
        for (int x = 198; x <= 200; x++) p.Spawn(p.TouchWallBlue, inter, x, 27f);
        SetDiamondId(p.Spawn(p.Diamond, col, 202.5f, 27.5f), 2);

        // Summit Updraft (X = 180, Y = 19 -> 32)
        p.Spawn(p.BouncePad, inter, 180f, 19f);
        p.Spawn(p.DashLaneUp, inter, 180f, 26f);                       // Y=22..30 shoots UP cleanly!
        for (int x = 176; x <= 185; x++) p.Spawn(p.TouchWallBlue, inter, x, 31f);
        for (int x = 177; x <= 184; x++) p.Spawn(p.DashWall, inter, x, 30f);

        p.Spawn(p.OneWayPlatformLarge, env, 176f, 32f);                // Summit Grate X=176..184
        p.SpawnGroundSized(env, 184f, 32f, 22);                        // East Summit Deck X=184..206, Y=31..32
        SetAtomId(p.Spawn(p.Atom, col, 179.5f, 33.8f), 10);

        // =====================================================================
        // SANCTUARY 3: SKYWARD BELFRY (Top-Right X = 160 to 174, Y = 32 to 44)
        // =====================================================================
        p.SpawnGroundSized(env, 160f, 32f, 14);                        // X=160..174, Y=31..32
        p.SpawnCeilingSized(env, 160f, 44f, 14);                       // X=160..174, Y=43..44
        p.SpawnBlock(p.BlockGreen, env, 161f, 43f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 167f, 43f, 12, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 167f, 33.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE CROWN SKYLIGHT PLUNGE (Overhead Right->Left Glide -> Triple Breach Finale!)
        // Skylight: X = 75..160, Y = 27..46 | Central Suspended Sanctuary: X = 84..112, Y = 15..25
        // =====================================================================
        for (int x = 75; x < 160; x += 20)
            p.SpawnCeilingSized(env, x, 46f, Mathf.Min(20, 160 - x));  // Skylight Glass Roof Y=45..46
        foreach (float rx in new[] { 86f, 102f, 118f, 134f, 150f })
            p.SpawnBlock(p.BlockCyan, env, rx, 45f, 4, 2, 4);          // Skylight roof mullions Y=44..45
        p.SpawnBlock(p.BlockCyan, env, 74f, 45f, 2, 64, 4);            // West Skylight Outer Wall X=74..75, Y=13..45

        // Continuous Skylight Recovery Floor RIGHT-OF-BREACH (X = 112..160, Y = 27)
        for (int x = 112; x < 160; x += 20)
            p.SpawnGroundSized(env, x, 27f, Mathf.Min(20, 160 - x));   // Clear above Start Hub (Y=13)!
        foreach (float bx in new[] { 120f, 134f, 148f })
            p.Spawn(p.BouncePadMega, inter, bx, 27f);
        p.Spawn(p.OneWayPlatform, env, 107f, 27f);                     // Recovery bridge onto 112!

        // Overhead Glide Chain RIGHT -> LEFT (160 -> 88)
        GameObject dbSkyLeft = p.Spawn(p.DashBoostLeft, inter, 155f, 33.5f);
        ConfigureDashZoneBoost(dbSkyLeft, new Vector2(3.2f, 3.2f), 36f, 0.40f, 22f);
        for (int x = 147; x <= 149; x++)
            for (int y = 33; y <= 39; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 145.5f, 36.5f), 11);

        GameObject barrel4A = p.Spawn(p.DashHold, inter, 142f, 37f);
        ConfigureDashZoneHold(barrel4A, 174f, new Vector2(3.6f, 3.6f), 32f, 0.28f, 20f, 0.22f); // Fires LEFT into GLIDE!

        // Suspended Purple Diamond Cluster #1 (X = 128..134, Y = 33..41 -> Atom 12) + DashCharge #1
        for (int x = 128; x <= 134; x++)
            for (int y = 33; y <= 41; y++)
                if (Mathf.Abs(x - 131f) + Mathf.Abs(y - 37f) <= 3.6f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 131f, 37f), 12);
        p.Spawn(p.DashCharge, inter, 123f, 36f);

        // Suspended Purple Diamond Cluster #2 (X = 110..118, Y = 32..40 -> Diamond 3) + DashCharge #2
        for (int x = 110; x <= 118; x++)
            for (int y = 32; y <= 40; y++)
                if (Mathf.Abs(x - 114f) + Mathf.Abs(y - 36f) <= 4.0f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        SetDiamondId(p.Spawn(p.Diamond, col, 114f, 36f), 3);
        p.Spawn(p.DashCharge, inter, 105f, 35.5f);
        SetAtomId(p.Spawn(p.Atom, col, 100f, 35.5f), 13);
        SetAtomId(p.Spawn(p.Atom, col, 120f, 38.0f), 14);

        // Skylight West Funnel Deck (X = 75..88, Y = 25) -> Plunge Chute (X = 88..97)
        p.SpawnGroundSized(env, 75f, 25f, 9);                          // X=75..84, Y=24..25 flush with Sanct!
        p.Spawn(p.BouncePad, inter, 77f, 25f);                         // Springs straight onto 88..96 Plunge Grate!
        p.Spawn(p.TouchWallBlue, inter, 80f, 26f);
        p.Spawn(p.TouchWallBlue, inter, 81f, 26f);
        p.Spawn(p.OneWayPlatformLarge, env, 88f, 33f);                 // Plunge Grate X=88..96!
        for (int x = 89; x <= 93; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 32f); // Y=30..32 Ground Pound seal!
        SetAtomId(p.Spawn(p.Atom, col, 92.5f, 35.0f), 15);

        // Vertical DashBoost_Fast aimed DOWN (-90 deg) through Triple Yellow DashWall_ChainRow 1 Ceiling!
        GameObject dbPlungeDown = p.Spawn(p.DashBoostFast, inter, 92.5f, 27.5f);
        dbPlungeDown.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        ConfigureDashZoneBoost(dbPlungeDown, new Vector2(3.4f, 6.0f), 38f, 0.45f, 22f);

        // =====================================================================
        // CENTRAL SUSPENDED FINALE SANCTUARY (X = 84 to 112, Y = 15 to 25 -> End_Portal!)
        // =====================================================================
        p.SpawnGroundSized(env, 84f, 15f, 28);                         // Floor X=84..112, Y=14..15
        p.SpawnBlock(p.BlockMagenta, env, 84f, 14f, 56, 2, 4);         // Plinth beam X=84..112, Y=13..14 (flush atop Z1 & Hub!)
        p.SpawnBlock(p.BlockCyan, env, 84f, 25f, 2, 20, 4);            // West Sanctuary Wall X=84..85, Y=15..25
        p.SpawnBlock(p.BlockCyan, env, 111f, 25f, 2, 20, 4);           // East Sanctuary Wall X=111..112, Y=15..25
        p.SpawnCeilingSized(env, 85f, 25f, 4);                         // Roof West X=85..89, Y=24..25
        p.SpawnCeilingSized(env, 96f, 25f, 15);                        // Roof East X=96..111, Y=24..25

        // Triple-Thick Yellow DashWall_ChainRow 1 Ceiling Hatch (X = 89..96, Y = 22..25)!
        p.SpawnTouchWallCol(inter, 89f, 23f, 25f);
        for (int y = 23; y <= 25; y++) p.Spawn(p.DashWallChainRow1, inter, 90f, y); // 3 stacked 5x1 rows = 15 yellow blocks!
        p.SpawnTouchWallCol(inter, 95f, 23f, 25f);

        SetAtomId(p.Spawn(p.Atom, col, 98.5f, 17f), 16);

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
        return $"Level_43 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 66f;
            lm.TwoStarTime = 100f;
            lm.OneStarTime = 170f;
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
            respawnMgr.KillTopY = 70f;
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
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject BlockGreen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Green Variant.prefab");
        public GameObject BlockMagenta = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject OneWayPlatformLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject OneWayPlatformShort = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab");
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
        public GameObject DashBoostUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Up.prefab");
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

        public void SpawnTouchWallCol(Transform parent, float x, float startY, float endY)
        {
            for (float y = startY; y <= endY; y += 1f)
                Spawn(TouchWallBlue, parent, x, y);
        }
    }
}
#endif
