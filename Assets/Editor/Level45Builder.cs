#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level45Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_45.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 45 (Observatory Mega-Slingshot)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 114f, -36f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(360f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(360f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -8 to 238)
        for (int x = -8; x < 238; x += 20)
            p.SpawnGroundSized(env, x, -28f, Mathf.Min(20, 238 - x));

        // =====================================================================
        // FLOW: FAR TOP-RIGHT START -> CENTRAL ZENITH VAULT END PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 205f, 16f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 128.5f, 39.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Clean Trigger Lead-In Before Launches, Tight/Wide/Climb Contrast)
        // =====================================================================
        GameObject cz1Start = p.Spawn(p.CameraZoneTight, flow, 199f, 19f);
        cz1Start.name = "CameraZone_Tight_Z1TurretStart";
        SetTriggerBoxSize(cz1Start, new Vector3(20f, 9f, 10f));           // X=189..209, Y=14.5..23.5

        GameObject cz1Dive = p.Spawn(p.CameraZoneWide, flow, 111.5f, 7f);
        cz1Dive.name = "CameraZone_Wide_Z1MeteorDive";
        SetTriggerBoxSize(cz1Dive, new Vector3(155f, 22f, 10f));         // X=34..189, Y=-4..18

        GameObject cz2Prism = p.Spawn(p.CameraZoneClimb, flow, 17f, 9f);
        cz2Prism.name = "CameraZone_Climb_Z2WestPrism";
        SetTriggerBoxSize(cz2Prism, new Vector3(34f, 32f, 10f));         // X=0..34, Y=-7..25

        GameObject cz3Span = p.Spawn(p.CameraZoneWide, flow, 124f, 28.5f);
        cz3Span.name = "CameraZone_Wide_Z3ZenithSpan";
        SetTriggerBoxSize(cz3Span, new Vector3(184f, 11f, 10f));         // X=32..216, Y=23..34

        GameObject cz4Well = p.Spawn(p.CameraZoneTight, flow, 222f, 0f);
        cz4Well.name = "CameraZone_Tight_Z4PlungeWell";
        SetTriggerBoxSize(cz4Well, new Vector3(14f, 50f, 10f));          // X=215..229, Y=-25..25

        GameObject cz4Whip = p.Spawn(p.CameraZoneTight, flow, 171f, -18.5f);
        cz4Whip.name = "CameraZone_Tight_Z4UnderWhip";
        SetTriggerBoxSize(cz4Whip, new Vector3(88f, 13f, 10f));          // X=127..215, Y=-25..-12

        GameObject cz4SlingLow = p.Spawn(p.CameraZoneClimb, flow, 113f, -11f);
        cz4SlingLow.name = "CameraZone_Climb_Z4SlingshotLower";
        SetTriggerBoxSize(cz4SlingLow, new Vector3(26f, 28f, 10f));      // X=100..126, Y=-25..3

        GameObject cz4SlingMid = p.Spawn(p.CameraZoneClimb, flow, 113f, 18f);
        cz4SlingMid.name = "CameraZone_Climb_Z4SlingshotMid";
        SetTriggerBoxSize(cz4SlingMid, new Vector3(24f, 10f, 10f));      // X=101..125, Y=13..23

        GameObject cz4Zenith = p.Spawn(p.CameraZoneWide, flow, 114f, 41f);
        cz4Zenith.name = "CameraZone_Wide_Z4ZenithVault";
        SetTriggerBoxSize(cz4Zenith, new Vector3(44f, 16f, 10f));        // X=92..136, Y=33..49

        // =====================================================================
        // ZONE 1: THE METEOR DECK DIVE (Far Top-Right Start -> Rocket Down-Left!)
        // X = 34 to 209, Y = 23 down to -6
        // =====================================================================
        // Top-Right Observatory Turret (X = 194..209, Y = 16..23)
        p.SpawnGroundSized(env, 194f, 16f, 15);                          // Turret Floor 194..209, Y=15..16
        p.SpawnBlock(p.BlockCyan, env, 208f, 22f, 2, 12, 4);             // Back wall 208..209, Y=16..22 (clean under Z3!)
        p.SpawnBlock(p.BlockCyan, env, 208f, 15f, 2, 12, 4);             // Sub-pier Y=9..15
        p.SpawnTouchWallCol(inter, 199f, 17f, 20f);
        GameObject dbStart = p.Spawn(p.DashBoostLeft, inter, 196f, 17.5f);
        ConfigureDashZoneBoost(dbStart, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);

        // First Left Descent: Slopes + Ice -> Inverted Purple Dome #1 -> Barrel 1A -> Skylight Cracked
        p.Spawn(p.Slope45, env, 193f, 14f);                              // 192..194, 14..16
        p.Spawn(p.Slope45, env, 191f, 12f);                              // 190..192, 12..14
        p.Spawn(p.Slope45, env, 189f, 10f);                              // 188..190, 10..12
        p.SpawnBlock(p.BlockCyan, env, 190f, 12f, 4, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 192f, 14f, 4, 8, 4);

        p.Spawn(p.IceSurface, env, 182f, 10f);                           // 182..188, Y=9..10
        p.SpawnGroundSized(env, 166f, 10f, 16);                          // Deck 1A X=166..182
        p.SpawnBlock(p.BlockCyan, env, 166f, 9f, 2, 16, 4);              // Supports above Alcove

        GameObject db1A = p.Spawn(p.DashBoostLeft, inter, 179f, 11.5f);
        ConfigureDashZoneBoost(db1A, new Vector2(3.2f, 3.2f), 36f, 0.38f, 24f);

        // Inverted Purple Dome #1 (X = 170..176, Y = 11..18)
        for (int x = 170; x <= 176; x++)
            for (int y = 11; y <= 18; y++)
                if (Mathf.Abs(x - 173f) * 0.7f + (18f - y) * 0.5f <= 3.6f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.BouncePadMini, inter, 168f, 10f);
        GameObject barrel1A = p.Spawn(p.DashHold, inter, 163f, 14f);
        ConfigureDashZoneHold(barrel1A, 192f, new Vector2(3.4f, 3.4f), 36f, 0.40f, 24f, 0.22f); // Fires DOWN-LEFT!

        // Diagonal Skylight Breach (X = 152..158, Y = 8..14 — starts flush above Alcove Roof Y=8!) + SpinArm
        for (int x = 152; x <= 158; x++)
            for (int y = 9; y <= 14; y++)
                p.Spawn((x == 155 && y >= 10 && y <= 12) ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x == 155 && y >= 10 && y <= 12) ? x + 0.5f : x,
                    (x == 155 && y >= 10 && y <= 12) ? y - 0.5f : y);
        GameObject spin1 = p.Spawn(p.SpinArm, haz, 148f, 12.5f);
        ConfigureSpinArm(spin1, -90f);

        // Secret Dive Alcove #1 beneath Deck 1A (X = 152..166, Y = 0..8 -> Atom 1!)
        p.SpawnGroundSized(env, 152f, 0f, 15);                           // Floor X=152..167, Y=-1..0
        p.SpawnCeilingSized(env, 152f, 8f, 14);                          // Roof X=152..166, Y=7..8
        p.SpawnBlock(p.BlockMagenta, env, 165f, 7f, 2, 14, 4);           // Back Wall Y=0..7
        for (int y = 1; y <= 7; y++) p.Spawn(p.DashWall, inter, 152f, y); // False yellow door!
        for (int x = 158; x <= 160; x++)
            for (int y = 1; y <= 3; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 163f, 1.8f), 1);

        // Mid Dive Deck 1B (129..151, Y = 4) -> Moving Shuttle -> Center OneWay Crossing (104..120, Y=4)!
        p.Spawn(p.Slope45, env, 149f, 6f);                               // 148..150, 6..8
        p.Spawn(p.Slope45, env, 147f, 4f);                               // 146..148, 4..6
        p.SpawnGroundSized(env, 136f, 4f, 10);                           // 136..146
        p.Spawn(p.IceSurface, env, 130f, 4f);                            // 130..136 slide!
        p.Spawn(p.FireJet, haz, 148f, 4f);
        GameObject db1B = p.Spawn(p.DashBoostLeft, inter, 142f, 5.5f);
        ConfigureDashZoneBoost(db1B, new Vector2(3.2f, 3.2f), 36f, 0.34f, 24f);

        GameObject mp1A = p.Spawn(p.MovingPlatform, env, 125f, 4.5f);
        ConfigureMovingPlatform(mp1A, new Vector3(-4f, 0f, 0f), 3.0f, 0.2f);
        p.Spawn(p.BouncePadMini, inter, 132f, 4f);

        // Center Heart Bridge Crossing (X = 104..120, Y = 4 — open vertically for Zone 4 Mega-Slingshot!)
        p.Spawn(p.OneWayPlatformLarge, env, 104f, 4f);                   // X=104..112
        p.Spawn(p.OneWayPlatformLarge, env, 112f, 4f);                   // X=112..120

        // Zone 1 West Wing (X = 34..104, Y = 4 -> -6)
        p.SpawnGroundSized(env, 82f, 4f, 22);                            // X=82..104, Y=3..4
        GameObject db1C = p.Spawn(p.DashBoostLeft, inter, 101f, 5.5f);
        ConfigureDashZoneBoost(db1C, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        for (int x = 90; x <= 97; x++)
            for (int y = 5; y <= 13; y++)
                if (Mathf.Abs(x - 93.5f) * 0.7f + (13f - y) * 0.5f <= 3.8f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Inverted Dome #2!

        p.SpawnBlock(p.BlockMagenta, env, 79f, 12f, 4, 2, 4);
        GameObject can1 = p.Spawn(p.CannonFast, haz, 80.5f, 13.2f);
        ConfigureCannon(can1, new Vector3(0f, 0f, 0f), 10f, 2.5f, 24f); // Shoots RIGHT across dome!

        GameObject barrel1B = p.Spawn(p.DashHold, inter, 85f, 8f);
        ConfigureDashZoneHold(barrel1B, 196f, new Vector2(3.4f, 3.4f), 36f, 0.36f, 22f, 0.22f); // Fires DOWN-LEFT!

        p.Spawn(p.CrumblingBlock, env, 77f, 4f);
        GameObject mp1B = p.Spawn(p.MovingPlatform, env, 72f, 2f);
        ConfigureMovingPlatform(mp1B, new Vector3(0f, 4f, 0f), 3.0f, 0.2f);
        SetAtomId(p.Spawn(p.Atom, col, 67f, 7f), 2);

        p.SpawnGroundSized(env, 53f, 0f, 17);                            // Deck 1C X=53..70, Y=-1..0
        p.Spawn(p.BouncePadMini, inter, 69f, 0f);
        GameObject db1D = p.Spawn(p.DashBoostLeft, inter, 64f, 1.5f);
        ConfigureDashZoneBoost(db1D, new Vector2(3.2f, 3.2f), 36f, 0.36f, 22f);
        for (int x = 55; x <= 60; x++)
            for (int y = 1; y <= 7; y++)
                p.Spawn((x == 57 || x == 58) && y <= 4 ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x == 57 || x == 58) && y <= 4 ? x + 0.5f : x,
                    (x == 57 || x == 58) && y <= 4 ? y - 0.5f : y);

        // Lower-Left Ground Pound Breach (X = 43..53, Y = 0 down to -6)
        p.Spawn(p.OneWayPlatformLarge, env, 43f, 0f);                    // Grate X=43..51
        p.Spawn(p.OneWayPlatformShort, env, 51f, 0f);                    // 51..53
        for (int x = 44; x <= 52; x++) p.Spawn(p.TouchWallBlue, inter, x, -1f);
        for (int x = 45; x <= 50; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, -2f); // Y=-4..-2

        p.SpawnGroundSized(env, 34f, -6f, 20);                           // Lower-Left Floor X=34..54, Y=-7..-6
        p.SpawnBlock(p.BlockCyan, env, 52f, -7f, 2, 42, 4);              // West anchor pier down to -28
        GameObject saw1B = p.Spawn(p.SawSlow, haz, 40f, -4f);
        ConfigureSaw(saw1B, new Vector3(6f, 0f, 0f), 2.6f);
        p.Spawn(p.BouncePad, inter, 48f, -6f);                           // Easy recovery back UP!
        SetAtomId(p.Spawn(p.Atom, col, 36.5f, -4.2f), 3);

        // =====================================================================
        // SANCTUARY 1: LOWER-LEFT NEXUS (X = 20 to 34, Y = -6 to +4)
        // =====================================================================
        p.SpawnGroundSized(env, 20f, -6f, 14);                           // X=20..34, Y=-7..-6
        p.SpawnCeilingSized(env, 20f, 4f, 14);                           // X=20..34, Y=3..4
        p.SpawnBlock(p.BlockGreen, env, 21f, 3f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 27f, 3f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 27f, -7f, 2, 42, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 27f, -4.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE FAR-LEFT RICOCHET PRISM (Vertical Pinball Ascent X=0..34, Y=-6..+24)
        // Plus High-Speed Side Pocket (X = 34..46, Y = 10..18 -> Atom 4 + Diamond 1!)
        // =====================================================================
        p.SpawnGroundSized(env, 1f, -6f, 19);                            // Prism Base X=1..20, Y=-7..-6
        p.SpawnBlock(p.BlockCyan, env, 0f, 34f, 2, 124, 4);              // Grand West Outer Wall X=0..1, Y=-28..34
        p.Spawn(p.Wall12, env, 1f, 14f);                                 // Structural prism rib X=1..2

        // East Prism Divider Wall split ONLY for Sanct 1 (-6..4), Side Pocket (10..18), and Zone 3 Summit (24..34)
        p.SpawnBlock(p.BlockCyan, env, 33f, 10f, 2, 12, 4);              // Wall 33..34, Y=4..10
        p.SpawnBlock(p.BlockCyan, env, 33f, 23f, 2, 10, 4);              // Wall 33..34, Y=18..23

        // Side Pocket (X = 34..46, Y = 10..18) -> Atom 4 + Diamond 1!
        p.SpawnGroundSized(env, 34f, 10f, 12);                           // X=34..46, Y=9..10
        p.SpawnCeilingSized(env, 34f, 18f, 12);                          // X=34..46, Y=17..18
        p.SpawnBlock(p.BlockMagenta, env, 45f, 17f, 2, 14, 4);           // Back Wall Y=10..17
        for (int y = 11; y <= 17; y++) p.Spawn(p.DashWall, inter, 33f, y); // False yellow door Y=10..17!
        for (int x = 36; x <= 38; x++)
            for (int y = 11; y <= 14; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 40.5f, 11.8f), 4);
        SetDiamondId(p.Spawn(p.Diamond, col, 43.2f, 14.5f), 1);

        // Pinball Prism Vertical Mechanics (Y = -6 up to +24)
        p.Spawn(p.BouncePadMega, inter, 15f, -6f);
        GameObject barrel2A = p.Spawn(p.DashHold, inter, 15f, 3f);
        ConfigureDashZoneHold(barrel2A, 155f, new Vector2(3.4f, 3.4f), 34f, 0.34f, 22f, 0.22f); // Fires UP-LEFT!

        p.Spawn(p.OneWayPlatformLarge, env, 2f, 4f);                     // Left Pinball Shelf X=2..10
        p.Spawn(p.BouncePad, inter, 5f, 4f);
        GameObject mp2A = p.Spawn(p.MovingPlatform, env, 11f, 6f);
        ConfigureMovingPlatform(mp2A, new Vector3(7f, 0f, 0f), 3.4f, 0.2f);
        p.Spawn(p.FireJet, haz, 22f, 4f);
        p.Spawn(p.BouncePad, inter, 26f, 4f);                             // Uses Sanct 1 Roof (20..34) cleanly!

        // Checkerboard Prism Curtain (X = 11..24, Y = 10..15) + Side DashZone toward Side Pocket!
        for (int x = 11; x <= 23; x++)
            for (int y = 10; y <= 15; y++)
                if (x == 17 && y >= 11 && y <= 13)
                    p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
                else if ((x + y) % 2 == 0)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        GameObject dz2Pocket = p.Spawn(p.DashZone, inter, 28f, 12.5f);
        ConfigureDashZoneBoost(dz2Pocket, new Vector2(3.2f, 3.2f), 34f, 0.34f, 22f); // Shoots RIGHT into Side Pocket!

        GameObject saw2 = p.Spawn(p.SawSlow, haz, 23f, 8f);
        ConfigureSaw(saw2, new Vector3(0f, 6f, 0f), 2.8f);

        p.Spawn(p.DashLaneUp, inter, 7f, 15f);                           // Left Climb Lane Y=11..19
        GameObject barrel2B = p.Spawn(p.DashHold, inter, 7f, 20f);
        ConfigureDashZoneHold(barrel2B, 14f, new Vector2(3.4f, 3.4f), 34f, 0.36f, 22f, 0.22f); // Fires UP-RIGHT!
        GameObject spin2 = p.Spawn(p.SpinArm, haz, 17f, 16f);
        ConfigureSpinArm(spin2, 85f);
        GameObject mp2B = p.Spawn(p.MovingPlatform, env, 23f, 18f);
        ConfigureMovingPlatform(mp2B, new Vector3(0f, 4f, 0f), 2.8f, 0.2f);
        p.Spawn(p.BouncePadMega, inter, 28f, 16f);
        p.Spawn(p.OneWayPlatform, env, 25f, 16f);

        // Prism Roof Hatch -> Summit Landing Left of Sanct 2!
        p.Spawn(p.OneWayPlatformLarge, env, 5f, 24f);                    // 5..13
        p.Spawn(p.OneWayPlatformShort, env, 13f, 24f);                   // 13..15
        p.SpawnGroundSized(env, 1f, 24f, 4);                             // 1..5
        SetAtomId(p.Spawn(p.Atom, col, 14f, 25.8f), 5);

        // =====================================================================
        // SANCTUARY 2: PRISM SUMMIT LOOKOUT (X = 16 to 32, Y = 24 to 34)
        // =====================================================================
        p.SpawnGroundSized(env, 16f, 24f, 16);                           // X=16..32, Y=23..24
        p.SpawnCeilingSized(env, 1f, 34f, 31);                           // Roof X=1..32, flush right of West Wall!
        p.SpawnBlock(p.BlockMagenta, env, 17f, 33f, 14, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 24f, 33f, 14, 2, 4);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 24f, 25.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE COLLAPSING ZENITH SPAN (Sweep Far-Left -> Far-Right Above Z1!)
        // X = 32 to 204, Y = +24..+33 + Secret Rafter Vault (178..194, Y=33..40 -> Diamond 2!)
        // =====================================================================
        // Zenith Span Floor Y=24 (Split cleanly around Center Crossing OneWay 104..120 & IceSurface 166..172!)
        p.SpawnGroundSized(env, 32f, 24f, 20);                           // 32..52
        p.SpawnGroundSized(env, 52f, 24f, 20);                           // 52..72
        p.SpawnGroundSized(env, 72f, 24f, 20);                           // 72..92
        p.SpawnGroundSized(env, 92f, 24f, 12);                           // 92..104
        p.Spawn(p.OneWayPlatformLarge, env, 104f, 24f);                  // Center Slingshot Pass-Through 104..112
        p.Spawn(p.OneWayPlatformLarge, env, 112f, 24f);                  // 112..120
        p.SpawnGroundSized(env, 120f, 24f, 20);                          // 120..140
        p.SpawnGroundSized(env, 140f, 24f, 26);                          // 140..166
        p.Spawn(p.IceSurface, env, 166f, 24f);                           // 166..172!
        p.SpawnGroundSized(env, 172f, 24f, 32);                          // 172..204

        // Zenith Span Roof Y=33 (Open 104..120 Center Slingshot Hatch & 180..185 Secret Vault Door!)
        p.SpawnCeilingSized(env, 32f, 33f, 72);                          // 32..104, Y=32..33
        p.SpawnCeilingSized(env, 120f, 33f, 60);                         // 120..180, Y=32..33
        for (int x = 180; x <= 184; x++) p.Spawn(p.DashWall, inter, x, 33f); // False yellow ceiling door 180..185!
        p.SpawnCeilingSized(env, 185f, 33f, 19);                         // 185..204, Y=32..33

        // Secret Rafter Vault (X = 178..194, Y = 33..40) -> Diamond 2 + Atom 10!
        p.SpawnBlock(p.BlockCyan, env, 178f, 39f, 2, 12, 4);             // Y=33..39
        p.SpawnBlock(p.BlockCyan, env, 193f, 39f, 2, 12, 4);             // Y=33..39
        p.SpawnCeilingSized(env, 178f, 40f, 16);                         // Roof Y=39..40
        p.Spawn(p.TouchWallBlue, inter, 186f, 34f);
        p.Spawn(p.TouchWallBlue, inter, 186f, 35f);
        SetAtomId(p.Spawn(p.Atom, col, 182.5f, 36.5f), 10);
        SetDiamondId(p.Spawn(p.Diamond, col, 190f, 36.5f), 2);

        // Collapsing Span Launchers, StompBlocks, Cannons & Massive Stalactites (32 -> 204)
        GameObject db3A = p.Spawn(p.DashBoostFast, inter, 35f, 25.5f);
        ConfigureDashZoneBoost(db3A, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        p.Spawn(p.CrumblingBlock, env, 40f, 25.2f);
        p.Spawn(p.CrumblingBlock, env, 43.5f, 25.2f);
        for (int x = 39; x <= 46; x++)
            for (int y = 27; y <= 32; y++)
                if (y >= 28 || x <= 44) p.Spawn(p.TouchWallBlue, inter, x, y); // Stalactite #1
        SetAtomId(p.Spawn(p.Atom, col, 48.5f, 27f), 6);
        p.Spawn(p.BouncePadMini, inter, 51f, 24f);

        GameObject barrel3A = p.Spawn(p.DashHold, inter, 56f, 28f);
        ConfigureDashZoneHold(barrel3A, 0f, new Vector2(3.4f, 3.4f), 36f, 0.36f, 24f, 0.22f);
        for (int x = 61; x <= 66; x++)
            for (int y = 26; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #2
        p.Spawn(p.DashCharge, inter, 69.5f, 29f);
        GameObject mp3A = p.Spawn(p.MovingPlatform, env, 75f, 26f);
        ConfigureMovingPlatform(mp3A, new Vector3(6f, 0f, 0f), 3.4f, 0.2f);
        p.SpawnBlock(p.BlockMagenta, env, 82f, 29f, 4, 2, 4);
        GameObject can3A = p.Spawn(p.CannonFast, haz, 83f, 30.2f);
        ConfigureCannon(can3A, new Vector3(0f, 0f, 180f), 11f, 2.3f, 26f);
        SetAtomId(p.Spawn(p.Atom, col, 87f, 27f), 7);

        GameObject db3B = p.Spawn(p.DashBoostFast, inter, 91f, 25.5f);
        ConfigureDashZoneBoost(db3B, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        for (int x = 96; x <= 101; x++)
            for (int y = 25; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #3
        SetAtomId(p.Spawn(p.Atom, col, 112f, 26.5f), 8);

        GameObject db3C = p.Spawn(p.DashBoostFast, inter, 123f, 25.5f);
        ConfigureDashZoneBoost(db3C, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        GameObject stomp3A = p.Spawn(p.StompBlock, haz, 130f, 30.5f);
        ConfigureStompBlock(stomp3A, 0.25f);
        p.Spawn(p.CrumblingBlock, env, 135f, 25.2f);
        p.Spawn(p.CrumblingBlock, env, 138.5f, 25.2f);

        GameObject barrel3B = p.Spawn(p.DashHold, inter, 144f, 28f);
        ConfigureDashZoneHold(barrel3B, 6f, new Vector2(3.4f, 3.4f), 36f, 0.38f, 24f, 0.22f);
        for (int x = 149; x <= 154; x++)
            for (int y = 26; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Stalactite #4
        p.Spawn(p.DashCharge, inter, 158f, 29f);
        SetAtomId(p.Spawn(p.Atom, col, 161f, 27f), 9);
        GameObject stomp3B = p.Spawn(p.StompBlock, haz, 165f, 30.5f);
        ConfigureStompBlock(stomp3B, 0.32f);
        for (int x = 169; x <= 174; x++)
            for (int y = 26; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        p.SpawnBlock(p.BlockMagenta, env, 176f, 29f, 4, 2, 4);
        GameObject can3B = p.Spawn(p.CannonFast, haz, 177f, 30.2f);
        ConfigureCannon(can3B, new Vector3(0f, 0f, 180f), 11f, 2.4f, 25f);

        p.Spawn(p.BouncePadMini, inter, 182f, 24f);
        GameObject barrel3Secret = p.Spawn(p.DashHold, inter, 182f, 28.5f);
        ConfigureDashZoneHold(barrel3Secret, 90f, new Vector2(3.0f, 3.0f), 32f, 0.30f, 18f, 0.35f);

        GameObject db3Fin = p.Spawn(p.DashBoostFast, inter, 191f, 25.5f);
        ConfigureDashZoneBoost(db3Fin, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        for (int x = 196; x <= 201; x++)
            for (int y = 25; y <= 32; y++)
                p.Spawn((x == 198 || x == 199) && y <= 29 ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x == 198 || x == 199) && y <= 29 ? x + 0.5f : x,
                    (x == 198 || x == 199) && y <= 29 ? y - 0.5f : y);

        foreach (float bx in new[] { 46f, 67f, 94f, 141f, 158f, 175f, 188f })
            p.Spawn(p.BouncePad, inter, bx, 24f);

        // =====================================================================
        // SANCTUARY 3: FAR-RIGHT APEX WATCH (X = 204 to 216, Y = 24 to 34)
        // =====================================================================
        p.SpawnGroundSized(env, 204f, 24f, 12);                          // X=204..216, Y=23..24
        p.SpawnCeilingSized(env, 204f, 34f, 25);                         // Covers Sanct 3 + Plunge Well 204..229!
        p.SpawnBlock(p.BlockGreen, env, 205f, 33f, 10, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 210f, 33f, 10, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 210f, 25.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: UNDER-CORE SLINGSHOT FINALE (Plunge -> Whip Left -> Dead-Center Slingshot UP!)
        // 4A Well: 216..228, Y=24 -> -24 | 4B Whip: 216 -> 101, Y=-24..-13
        // 4C Center Vertical Mega-Slingshot: X=104..120, Y=-24 -> +38 -> Zenith Vault (End_Portal)
        // =====================================================================
        p.SpawnBlock(p.BlockCyan, env, 228f, 33f, 2, 122, 4);            // East Outer Wall X=228..229, Y=-28..33!
        p.SpawnBlock(p.BlockCyan, env, 215f, 23f, 2, 70, 4);             // Well West Wall X=215..216, Y=-12..23 (open -24..-12!)

        // 4A Plunge Well Strata (X = 216..228, Y = 24 down to -24)
        p.Spawn(p.OneWayPlatformLarge, env, 216f, 24f);                  // 216..224
        p.Spawn(p.OneWayPlatformShort, env, 224f, 24f);                  // 224..226
        for (int x = 217; x <= 225; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 22f);
        for (int x = 217; x <= 227; x++) p.Spawn(p.TouchWallBlue, inter, x, 19f);
        GameObject mp4A = p.Spawn(p.MovingPlatform, env, 220f, 13f);
        ConfigureMovingPlatform(mp4A, new Vector3(4f, 0f, 0f), 3.0f, 0.2f);
        for (int x = 217; x <= 226; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 7f); // Y=6..11
        for (int x = 217; x <= 227; x++) p.Spawn(p.TouchWallBlue, inter, x, 3f);
        for (int x = 217; x <= 225; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, -4f);
        for (int x = 217; x <= 226; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -12f); // Y=-13..-8
        for (int x = 217; x <= 227; x++) p.Spawn(p.TouchWallBlue, inter, x, -15f);

        // 4B Blistering Subterranean Whip-Left Floor (X = 100..228, Y = -24, split cleanly around Ice!)
        p.SpawnGroundSized(env, 100f, -24f, 42);                         // 100..142
        p.Spawn(p.IceSurface, env, 142f, -24f);                          // 142..148
        p.SpawnGroundSized(env, 148f, -24f, 56);                         // 148..204
        p.Spawn(p.IceSurface, env, 204f, -24f);                          // 204..210
        p.SpawnGroundSized(env, 210f, -24f, 18);                         // 210..228
        foreach (float px in new[] { 101f, 121f, 140f, 160f, 180f, 200f, 220f })
            p.SpawnBlock(p.BlockCyan, env, px, -25f, 2, 6, 4);           // Piers Y=-28..-25

        // Whip-Left Tunnel Ceiling Y = -13 (120..216, leaves 101..120 Wide Open UP for Slingshot!)
        for (int x = 120; x < 216; x += 20)
            p.SpawnCeilingSized(env, x, -13f, Mathf.Min(20, 216 - x));
        p.SpawnBlock(p.BlockCyan, env, 100f, 3f, 2, 54, 4);              // West Slingshot Backstop Wall X=100..101, Y=-24..3!

        GameObject leaper4A = p.Spawn(p.LavaLeaper, haz, 221f, -24f);
        ConfigureLavaLeaper(leaper4A, 5.5f);
        SetAtomId(p.Spawn(p.Atom, col, 224.5f, -22f), 11);

        GameObject db4Whip1 = p.Spawn(p.DashBoostLeft, inter, 214f, -22.5f);
        ConfigureDashZoneBoost(db4Whip1, new Vector2(3.2f, 3.2f), 38f, 0.42f, 24f);
        p.SpawnTouchWallCol(inter, 201f, -23f, -15f);
        for (int y = -23; y <= -15; y++) p.Spawn(p.DashWallChainRow1, inter, 196f, y); // Gate #1 (196..201)!
        GameObject leaper4B = p.Spawn(p.LavaLeaper, haz, 191f, -24f);
        ConfigureLavaLeaper(leaper4B, 5.5f);
        SetAtomId(p.Spawn(p.Atom, col, 188f, -22f), 12);

        GameObject db4Whip2 = p.Spawn(p.DashBoostLeft, inter, 185f, -22.5f);
        ConfigureDashZoneBoost(db4Whip2, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        GameObject mp4Whip = p.Spawn(p.MovingPlatform, env, 178f, -20f);
        ConfigureMovingPlatform(mp4Whip, new Vector3(0f, 4f, 0f), 3.0f, 0.2f);
        GameObject stomp4 = p.Spawn(p.StompBlock, haz, 175f, -15.5f);
        ConfigureStompBlock(stomp4, 0.28f);
        GameObject leaper4C = p.Spawn(p.LavaLeaper, haz, 169f, -24f);
        ConfigureLavaLeaper(leaper4C, 5.5f);
        for (int x = 160; x <= 165; x++)
            for (int y = -23; y <= -15; y++)
                p.Spawn((x == 162 || x == 163) && y >= -21 && y <= -18 ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x == 162 || x == 163) && y >= -21 && y <= -18 ? x + 0.5f : x,
                    (x == 162 || x == 163) && y >= -21 && y <= -18 ? y - 0.5f : y);
        SetAtomId(p.Spawn(p.Atom, col, 156f, -22f), 13);

        GameObject db4Whip3 = p.Spawn(p.DashBoostLeft, inter, 152f, -22.5f);
        ConfigureDashZoneBoost(db4Whip3, new Vector2(3.2f, 3.2f), 38f, 0.38f, 24f);
        p.Spawn(p.BouncePadMini, inter, 143f, -24f);
        GameObject barrel4Whip = p.Spawn(p.DashHold, inter, 139f, -19.5f);
        ConfigureDashZoneHold(barrel4Whip, 180f, new Vector2(3.4f, 3.4f), 38f, 0.42f, 24f, 0.20f);
        p.SpawnTouchWallCol(inter, 134f, -23f, -15f);
        for (int y = -23; y <= -15; y++) p.Spawn(p.DashWallChainRow1, inter, 129f, y); // Gate #2 (129..134)!
        SetAtomId(p.Spawn(p.Atom, col, 125f, -22f), 14);

        // 4C: DEAD-CENTER VERTICAL MEGA-SLINGSHOT (X = 104..120, Y = -24 -> +38 -> End_Portal!)
        p.Spawn(p.BouncePadMega, inter, 112f, -24f);
        p.Spawn(p.DashLaneUp, inter, 112f, -14f);                       // Y=-18..-10 launches into Hold 1!
        GameObject slingHold1 = p.Spawn(p.DashHold, inter, 112f, -4f);   // Catches below Zone 1 (Y=4)!
        ConfigureDashZoneHold(slingHold1, 90f, new Vector2(4.0f, 4.0f), 38f, 0.56f, 24f, 0.18f);

        // Lower Monolith A (Y = -1..+2, strictly below Zone 1 OneWay Y=+4!)
        for (int x = 107; x <= 116; x++)
            for (int y = 0; y <= 2; y++)
                p.Spawn((x >= 110 && x <= 113 && y == 1) ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x >= 110 && x <= 113 && y == 1) ? x + 0.5f : x,
                    (x >= 110 && x <= 113 && y == 1) ? y - 0.5f : y);
        SetAtomId(p.Spawn(p.Atom, col, 112f, 1.5f), 15);

        // Slingshot Second Stage between Zone 1 (Y=12) and Zone 3 (Y=24)!
        p.SpawnBlock(p.BlockCyan, env, 102f, 23f, 2, 18, 4);             // Shaft guide wall West X=102..103, Y=14..23
        p.SpawnBlock(p.BlockCyan, env, 121f, 23f, 2, 18, 4);             // Shaft guide wall East X=121..122, Y=14..23
        GameObject slingHold2 = p.Spawn(p.DashHold, inter, 112f, 17.5f); // Safely below Zone 3 OneWay Y=24!
        ConfigureDashZoneHold(slingHold2, 90f, new Vector2(4.0f, 4.0f), 38f, 0.62f, 24f, 0.18f);

        // Middle Monolith B (Y = 20..22, cleanly above Hold 2 and below Zone 3 Y=24!)
        for (int x = 107; x <= 116; x++)
            for (int y = 21; y <= 22; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        // CENTRAL ZENITH VAULT (X = 94..134, Y = 38..47 — directly atop Zone 3 Roof Y=33!)
        p.SpawnBlock(p.BlockCyan, env, 94f, 46f, 2, 26, 4);              // Vault West Wall 94..95, Y=33..46
        p.SpawnBlock(p.BlockCyan, env, 133f, 46f, 2, 26, 4);             // Vault East Wall 133..134, Y=33..46
        for (int x = 94; x < 134; x += 20)
            p.SpawnCeilingSized(env, x, 47f, Mathf.Min(20, 134 - x));    // Zenith Crown Roof Y=46..47
        p.SpawnBlock(p.BlockMagenta, env, 108f, 46f, 16, 2, 4);          // Zenith rib Y=45..46

        p.SpawnGroundSized(env, 95f, 38f, 9);                            // Vault Left Floor 95..104, Y=37..38
        p.Spawn(p.OneWayPlatformLarge, env, 104f, 38f);                  // Breach Catch Grate 104..112
        p.Spawn(p.OneWayPlatformLarge, env, 112f, 38f);                  // 112..120
        p.SpawnGroundSized(env, 120f, 38f, 13);                          // Vault Right End_Portal Floor 120..133
        p.SpawnBlock(p.BlockCyan, env, 102f, 37f, 2, 8, 4);              // Breached Hatch Left Pier Y=33..37
        p.SpawnBlock(p.BlockCyan, env, 120f, 37f, 2, 8, 4);              // Breached Hatch Right Pier Y=33..37

        // Grand Triple-Thick Zenith Ceiling Monolith (X = 107..117, Y = 33..37)
        for (int x = 107; x <= 116; x++) p.Spawn(p.TouchWallBlue, inter, x, 34f); // Y=33..34 purple layer!
        for (int y = 35; y <= 37; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 107f, y);                // 107..112
            p.Spawn(p.DashWallChainRow1, inter, 112f, y);                // 112..117 -> 10 wide x 3 stacked yellow rows!
        }

        SetDiamondId(p.Spawn(p.Diamond, col, 99f, 39.8f), 3);
        SetAtomId(p.Spawn(p.Atom, col, 123f, 39.8f), 16);

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
        return $"Level_45 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 70f;
            lm.TwoStarTime = 108f;
            lm.OneStarTime = 180f;
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
            respawnMgr.KillTopY = 74f;
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
