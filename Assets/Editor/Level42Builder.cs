#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level42Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_42.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 42 (Figure-8 Barrel Citadel)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 130f, -36f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(380f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(380f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -10 to 270)
        for (int x = -10; x < 270; x += 20)
            p.SpawnGroundSized(env, x, -28f, Mathf.Min(20, 270 - x));

        // =====================================================================
        // FLOW: START PORTAL (LOWEST POINT Y=-22!), END PORTAL & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 80f, -22f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 260.5f, 17.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Dynamic mosaic matched to each Figure-8 directional pivot)
        // =====================================================================
        GameObject czZ1Lower = p.Spawn(p.CameraZoneTight, flow, 92f, -13f);
        czZ1Lower.name = "CameraZone_Tight_Z1IgnitionLower";
        SetTriggerBoxSize(czZ1Lower, new Vector3(32f, 22f, 10f));      // X=76..108, Y=-24..-2

        GameObject czZ1Alcove = p.Spawn(p.CameraZoneTight, flow, 114f, 0f);
        czZ1Alcove.name = "CameraZone_Tight_Z1SecretAlcove";
        SetTriggerBoxSize(czZ1Alcove, new Vector3(12f, 10f, 10f));     // X=108..120, Y=-5..5

        GameObject czZ1Climb = p.Spawn(p.CameraZoneClimb, flow, 92f, 13f);
        czZ1Climb.name = "CameraZone_Climb_Z1ReactorUpper";
        SetTriggerBoxSize(czZ1Climb, new Vector3(32f, 30f, 10f));      // X=76..108, Y=-2..28

        GameObject czZ2Left = p.Spawn(p.CameraZoneWide, flow, 44f, 23.5f);
        czZ2Left.name = "CameraZone_Wide_Z2GalleryLeft";
        SetTriggerBoxSize(czZ2Left, new Vector3(68f, 13f, 10f));       // X=10..78, Y=17..30

        GameObject czZ2Turn = p.Spawn(p.CameraZoneClimb, flow, 5f, 30f);
        czZ2Turn.name = "CameraZone_Climb_Z2Turnaround";
        SetTriggerBoxSize(czZ2Turn, new Vector3(10f, 26f, 10f));       // X=0..10, Y=17..43

        GameObject czZ2Right = p.Spawn(p.CameraZoneWide, flow, 66f, 36.5f);
        czZ2Right.name = "CameraZone_Wide_Z2UpperLoopRight";
        SetTriggerBoxSize(czZ2Right, new Vector3(112f, 13f, 10f));     // X=10..122, Y=30..43

        GameObject czZ3Coaster = p.Spawn(p.CameraZoneTight, flow, 147f, 26f);
        czZ3Coaster.name = "CameraZone_Tight_Z3AqueductCoaster";
        SetTriggerBoxSize(czZ3Coaster, new Vector3(50f, 28f, 10f));    // X=122..172, Y=12..40

        GameObject czZ3Wells = p.Spawn(p.CameraZoneTight, flow, 150f, -1f);
        czZ3Wells.name = "CameraZone_Tight_Z3CrystalWells";
        SetTriggerBoxSize(czZ3Wells, new Vector3(48f, 26f, 10f));      // X=126..174, Y=-14..12

        GameObject czZ4Dome = p.Spawn(p.CameraZoneWide, flow, 220f, 12f);
        czZ4Dome.name = "CameraZone_Wide_Z4GrandDomeFinale";
        SetTriggerBoxSize(czZ4Dome, new Vector3(92f, 54f, 10f));       // X=174..266, Y=-15..39

        // =====================================================================
        // ZONE 1: THE PRESSURIZED IGNITION CORE (Bottom-Up Vertical Barrel Climb)
        // X = 76 to 108, Y = -22 up to +18 -> Exit Left into Sanctuary 1
        // =====================================================================
        p.SpawnGroundSized(env, 76f, -22f, 33);                        // Base Floor X=76..109, Y=-23..-22
        p.SpawnBlock(p.BlockCyan, env, 76f, -23f, 2, 10, 4);           // Pier X=76..77, Y=-28..-23
        p.SpawnBlock(p.BlockCyan, env, 92f, -23f, 2, 10, 4);           // Pier X=92..93, Y=-28..-23
        p.SpawnBlock(p.BlockCyan, env, 108f, -23f, 2, 10, 4);          // Pier X=108..109, Y=-28..-23

        p.SpawnBlock(p.BlockCyan, env, 76f, 17f, 2, 78, 4);            // West Core Wall X=76..77, Y=-22..17 (open 18..28 above!)
        p.Spawn(p.Wall12, env, 77f, 10f);                              // Inner Wall_1x12 ribbing X=77..78, Y=-2..10
        p.SpawnCeilingSized(env, 78f, 29f, 30);                        // Core Ceiling X=78..108, Y=28..29 (kisses Z2B Y=30!)
        foreach (float rx in new[] { 82f, 94f, 104f })
            p.SpawnBlock(p.BlockGreen, env, rx, 28f, 2, 2, 4);         // Ceiling trim Y=27..28

        // East Core Wall + Secret Ignition Alcove Doorway (Y=-4..0)
        p.SpawnBlock(p.BlockCyan, env, 108f, -4f, 2, 36, 4);           // Lower East Wall Y=-22..-4
        p.SpawnBlock(p.BlockMagenta, env, 108f, 4f, 2, 8, 4);          // Alcove Door Lintel Y=0..4
        for (int y = -3; y <= 0; y++) p.Spawn(p.DashWall, inter, 108f, y); // False yellow seal Y=-4..0
        p.SpawnBlock(p.BlockCyan, env, 108f, 29f, 2, 50, 4);           // Upper East Wall Y=4..29

        // Secret Ignition Alcove (X = 109..119, Y = -4..+4) -> Atoms 2 & 3!
        p.SpawnGroundSized(env, 109f, -4f, 10);                        // X=109..119, Y=-5..-4
        p.SpawnCeilingSized(env, 109f, 4f, 10);                        // X=109..119, Y=3..4
        p.SpawnBlock(p.BlockMagenta, env, 118f, 3f, 2, 14, 4);         // Back Wall X=118..119, Y=-4..3
        p.SpawnBlock(p.BlockCyan, env, 118f, -5f, 2, 46, 4);           // Backing pier X=118..119, Y=-28..-5
        p.Spawn(p.FireJet, haz, 111.5f, -4f);
        for (int x = 113; x <= 115; x++)
            for (int y = -3; y <= 0; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // 3x4 Purple treasure cache!
        SetAtomId(p.Spawn(p.Atom, col, 116.5f, -2.5f), 2);
        SetAtomId(p.Spawn(p.Atom, col, 116.5f, 0.5f), 3);

        // Tier 1 Ignition Chamber (Y = -22 -> -11): Boost -> Purple Core -> DashLane_UP -> Ceiling Breach
        p.SpawnTouchWallCol(inter, 84f, -21f, -19f);                   // Opening smash door
        GameObject db1 = p.Spawn(p.DashBoostFast, inter, 88f, -20.5f);
        ConfigureDashZoneBoost(db1, new Vector2(3.2f, 3.2f), 34f, 0.36f, 22f);
        for (int x = 92; x <= 96; x++)
            for (int y = -21; y <= -18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // 5x4 Purple ignition cluster!

        p.Spawn(p.BouncePadMini, inter, 98.5f, -22f);
        p.Spawn(p.BouncePad, inter, 103f, -22f);
        p.Spawn(p.DashLaneUp, inter, 103f, -16f);                      // Y=-20..-12 rockets UP!

        // Deck 1 (Y = -11): Solid left + OneWay right hatch (X=99..107)
        p.SpawnGroundSized(env, 78f, -11f, 21);                        // X=78..99, Y=-12..-11
        p.Spawn(p.OneWayPlatformLarge, env, 99f, -11f);                // X=99..107 (pass-through upward!)
        for (int x = 99; x <= 107; x++) p.Spawn(p.TouchWallBlue, inter, x, -10f);
        for (int x = 100; x <= 106; x++) p.Spawn(p.DashWallCracked, inter, x + 0.5f, -9.5f); // Y=-10..-9

        GameObject leaper1 = p.Spawn(p.LavaLeaper, haz, 85f, -11f);
        ConfigureLavaLeaper(leaper1, 5.5f);
        p.Spawn(p.BouncePad, inter, 80f, -11f);                        // Left recovery bounce to -4!

        GameObject barrel1A = p.Spawn(p.DashHold, inter, 103f, -7f);
        ConfigureDashZoneHold(barrel1A, 152f, new Vector2(3.4f, 3.4f), 34f, 0.38f, 22f, 0.22f); // Fires UP-LEFT!

        // Deck 2 / Mid Reactor (Y = -4): Solid Left + OneWay Hatch (91..99) + Right FireJet to Alcove
        p.SpawnGroundSized(env, 78f, -4f, 13);                         // X=78..91, Y=-5..-4
        p.Spawn(p.OneWayPlatformLarge, env, 91f, -4f);                 // X=91..99
        p.SpawnGroundSized(env, 99f, -4f, 9);                          // X=99..108, Y=-5..-4

        // Hanging Ignition Cluster shredded by Barrel 1A (X = 87..93, Y = -3..+1)
        for (int x = 88; x <= 93; x++)
            for (int y = -3; y <= 1; y++)
                if (Mathf.Abs(x - 90.5f) + Mathf.Abs(y + 1f) <= 2.7f)
                    p.Spawn((x == 90 && y == -1) ? p.DashWall : p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 85.5f, 0.5f), 1);

        GameObject leaper2 = p.Spawn(p.LavaLeaper, haz, 85f, -4f);
        ConfigureLavaLeaper(leaper2, 5.0f);
        p.Spawn(p.FireJet, haz, 104f, -4f);
        p.Spawn(p.BouncePad, inter, 80f, -4f);                         // Springs cleanly into Barrel 1B!
        p.Spawn(p.BouncePad, inter, 102f, -4f);                        // Right recovery bounce to Y=+4!

        // Tier 3 Upper Climb (Y = +3 -> +18): Barrel 1B (42 deg) -> Diagonal Staircase -> DashCharge -> Barrel 1C (135 deg)
        GameObject barrel1B = p.Spawn(p.DashHold, inter, 80f, 3f);
        ConfigureDashZoneHold(barrel1B, 42f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 22f, 0.22f); // Fires UP-RIGHT!

        for (int i = 0; i < 6; i++)
        {
            float bx = 85f + i * 1.5f;
            float by = 5f + i * 1.1f;
            p.Spawn(p.TouchWallBlue, inter, Mathf.Round(bx), Mathf.Round(by));
            p.Spawn(p.DashWallCracked, inter, Mathf.Round(bx) + 0.5f, Mathf.Round(by) + 0.5f);
        }
        p.Spawn(p.DashCharge, inter, 96f, 11f);
        p.Spawn(p.OneWayPlatformLarge, env, 94f, 5f);                  // Catch platform X=94..102
        p.Spawn(p.BouncePadMega, inter, 99f, 5f);                      // Recovery into Barrel 1C!

        GameObject barrel1C = p.Spawn(p.DashHold, inter, 102f, 11.5f);
        ConfigureDashZoneHold(barrel1C, 138f, new Vector2(3.6f, 3.6f), 34f, 0.36f, 22f, 0.22f); // Fires UP-LEFT!

        // Core Summit Ceiling Breach (X = 87..100, Y = 18) -> Smash through to Y=18 Summit!
        p.SpawnGroundSized(env, 78f, 18f, 9);                          // Summit Floor X=78..87, Y=17..18
        p.Spawn(p.OneWayPlatformLarge, env, 87f, 18f);                 // X=87..95
        p.Spawn(p.OneWayPlatform, env, 95f, 18f);                      // X=95..100
        for (int x = 88; x <= 95; x++) p.Spawn(p.TouchWallBlue, inter, x, 16f);
        for (int x = 88; x <= 95; x++) p.Spawn(p.DashWallCracked, inter, x + 0.5f, 16.5f); // Y=16..17
        for (int x = 88; x <= 95; x++) p.Spawn(p.TouchWallBlue, inter, x, 19f); // Upper break burst above grate!
        SetAtomId(p.Spawn(p.Atom, col, 84f, 19.8f), 4);
        p.Spawn(p.BouncePadMini, inter, 98f, 18f);                     // Rests cleanly on OneWay(95..100)!

        // =====================================================================
        // SANCTUARY 1: PIVOT BALCONY (X = 64 to 78, Y = 18 to 29)
        // =====================================================================
        p.SpawnGroundSized(env, 64f, 18f, 14);                         // X=64..78, Y=17..18
        p.SpawnBlock(p.BlockCyan, env, 64f, 17f, 2, 90, 4);            // West Core Pier Y=-28..17
        p.SpawnCeilingSized(env, 64f, 29f, 14);                        // X=64..78, Y=28..29 (flush below Z2B!)
        p.SpawnBlock(p.BlockGreen, env, 65f, 28f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 71f, 28f, 12, 2, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 71f, 19.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE CLOCKWORK LOOPBACK GALLERY (Figure-8 Westbound -> Eastbound Overhead!)
        // 2A Lower Westbound: X = 14..64, Y = 18..29 | Turnaround & Vault: X = 0..14, Y = 18..42
        // 2B Upper Eastbound: X = 14..108, Y = 30..42 (directly OVER 2A, Sanct 1 & Z1!)
        // =====================================================================
        // Shared Figure-8 Slab (2A Ceiling Y=28..29, 2B Floor Y=29..30) + West Outer Tower Walls
        p.SpawnGroundSized(env, 14f, 18f, 50);                         // 2A Floor X=14..64, Y=17..18
        foreach (float px in new[] { 18f, 34f, 50f })
            p.SpawnBlock(p.BlockCyan, env, px, 17f, 2, 90, 4);         // Architectural support piers Y=-28..17
        for (int x = 14; x < 64; x += 20)
            p.SpawnCeilingSized(env, x, 29f, Mathf.Min(20, 64 - x));   // 2A Ceiling Y=28..29

        // 2A Westbound Barrel & Chandelier Run (64 -> 14)
        GameObject dbLeft = p.Spawn(p.DashBoostLeft, inter, 59f, 19.5f);
        ConfigureDashZoneBoost(dbLeft, new Vector2(3.2f, 3.2f), 34f, 0.42f, 22f);

        // Hanging Purple Chandelier #1 (X = 48..54, Y = 22..28)
        for (int x = 48; x <= 54; x++) p.Spawn(p.TouchWallBlue, inter, x, 28f); // Top crown
        for (int x = 49; x <= 53; x++) p.Spawn(p.TouchWallBlue, inter, x, 27f);
        for (int x = 50; x <= 52; x++) p.Spawn(p.TouchWallBlue, inter, x, 26f);
        for (int y = 22; y <= 25; y++) p.Spawn(p.TouchWallBlue, inter, 51f, y); // Staggered spire down!
        p.Spawn(p.TouchWallBlue, inter, 49f, 24f);
        p.Spawn(p.TouchWallBlue, inter, 53f, 24f);
        SetAtomId(p.Spawn(p.Atom, col, 45.5f, 22f), 5);

        p.Spawn(p.CrumblingBlock, env, 43f, 20f);
        p.Spawn(p.BouncePadMini, inter, 40f, 18f);

        GameObject barrel2A = p.Spawn(p.DashHold, inter, 37f, 22.5f);
        ConfigureDashZoneHold(barrel2A, 170f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 22f, 0.22f); // Fires UP-LEFT!

        // Hanging Purple + Cracked Chandelier #2 (X = 26..32, Y = 21..28)
        for (int x = 26; x <= 32; x++) p.Spawn(p.TouchWallBlue, inter, x, 28f);
        for (int x = 27; x <= 31; x++)
            for (int y = 24; y <= 27; y++)
                p.Spawn((x == 29 && y <= 25) ? p.DashWallCracked : p.TouchWallBlue, inter,
                    (x == 29 && y <= 25) ? x + 0.5f : x, (x == 29 && y <= 25) ? y - 0.5f : y);
        p.Spawn(p.TouchWallBlue, inter, 28f, 22f);
        p.Spawn(p.TouchWallBlue, inter, 30f, 22f);

        p.Spawn(p.DashCharge, inter, 23f, 23.5f);
        GameObject spin2A = p.Spawn(p.SpinArm, haz, 22f, 24.5f);
        ConfigureSpinArm(spin2A, 85f);
        GameObject mp2A = p.Spawn(p.MovingPlatform, env, 15f, 20.5f);
        ConfigureMovingPlatform(mp2A, new Vector3(0f, 3f, 0f), 2.8f, 0.3f);

        // 2A Continuous Recovery Pads & Hazards on Safety Floor (Y = 18)
        p.Spawn(p.BouncePad, inter, 51f, 18f);
        p.Spawn(p.FireJet, haz, 46f, 18f);
        p.Spawn(p.BouncePad, inter, 33f, 18f);
        p.Spawn(p.FireJet, haz, 27f, 18f);
        p.Spawn(p.BouncePad, inter, 19f, 18f);

        // Far-Left Turnaround Chimney & Secret Diamond 1 Vault (X = 0..14, Y = 18..42)
        p.SpawnBlock(p.BlockCyan, env, 0f, 41f, 2, 138, 4);            // Grand West Outer Wall X=0..1, Y=-28..41
        p.SpawnCeilingSized(env, 0f, 42f, 14);                         // Roof X=0..14, Y=41..42
        p.SpawnGroundSized(env, 1f, 18f, 13);                          // Floor X=1..14, Y=17..18
        p.SpawnBlock(p.BlockCyan, env, 13f, 17f, 2, 90, 4);            // Support pier Y=-28..17

        // Far-Left Secret Diamond 1 Vault (X = 1..6, Y = 18..26) behind False Yellow DashWall (X = 6)
        p.SpawnBlock(p.BlockMagenta, env, 1f, 26f, 10, 2, 4);          // Vault Roof X=1..6, Y=25..26
        for (int y = 19; y <= 25; y++) p.Spawn(p.DashWall, inter, 6f, y); // False door X=6, Y=18..25!
        for (int x = 3; x <= 4; x++)
            for (int y = 19; y <= 21; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Purple treasure stash inside!
        SetDiamondId(p.Spawn(p.Diamond, col, 2.0f, 19.5f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 4.5f, 23f), 6);

        // Turnaround Ascent Chimney (X = 7..14, Y = 18..34) -> Barrel 2_Turn (0 deg RIGHT!)
        p.Spawn(p.BouncePadMega, inter, 10f, 18f);
        p.Spawn(p.DashLaneUp, inter, 10f, 25f);                        // Y=21..29
        p.Spawn(p.OneWayPlatformShort, env, 6f, 30f);                  // 6..8
        p.Spawn(p.OneWayPlatform, env, 8f, 30f);                       // 8..12
        p.Spawn(p.OneWayPlatformShort, env, 12f, 30f);                 // 12..14
        GameObject barrel2Turn = p.Spawn(p.DashHold, inter, 10f, 34f);
        ConfigureDashZoneHold(barrel2Turn, 0f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 24f, 0.25f); // Fires RIGHT!

        // 2B: Upper Clockwork Truss Loopback (X = 14..108, Y = 30..42 — ROCKET BACK RIGHT!)
        for (int x = 14; x < 108; x += 20)
        {
            p.SpawnGroundSized(env, x, 30f, Mathf.Min(20, 108 - x));   // Truss Floor Y=29..30
            p.SpawnCeilingSized(env, x, 42f, Mathf.Min(20, 108 - x));  // Truss Roof Y=41..42
        }
        foreach (float tx in new[] { 22f, 42f, 62f, 82f, 100f })
            p.SpawnBlock(p.BlockMagenta, env, tx, 41f, 4, 2, 4);       // Upper neon beam trim Y=40..41

        // 6m Upper Truss Ceiling Curtain right out of Turnaround (X=15..20, Y=33..39)
        for (int x = 15; x <= 20; x++)
            for (int y = 33; y <= 39; y++)
                if (x <= 18 || y >= 35) p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject db2Right = p.Spawn(p.DashBoostFast, inter, 24f, 31.5f);
        ConfigureDashZoneBoost(db2Right, new Vector2(3.2f, 3.2f), 36f, 0.42f, 24f);

        // 7m Checkerboard Smash Tunnel (X=29..36, Y=31..38) -> Atom 7
        for (int x = 29; x <= 35; x++)
        {
            for (int y = 31; y <= 37; y++)
            {
                if ((x == 31 || x == 33) && y >= 32 && y <= 35)
                    p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
                else
                    p.Spawn(p.TouchWallBlue, inter, x, y);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 38.5f, 32f), 7);

        p.Spawn(p.BouncePad, inter, 43f, 30f);
        GameObject barrel2B = p.Spawn(p.DashHold, inter, 48f, 35.5f);
        ConfigureDashZoneHold(barrel2B, 8f, new Vector2(3.4f, 3.4f), 36f, 0.40f, 24f, 0.22f); // Fires RIGHT!

        // Truss Archway Smash (X=53..57, Y=34..40) + Saw + MovingPlatform
        for (int x = 53; x <= 57; x++)
            for (int y = 34; y <= 40; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject saw2B = p.Spawn(p.SawSlow, haz, 60f, 32f);
        ConfigureSaw(saw2B, new Vector3(7f, 0f, 0f), 2.8f);
        GameObject mp2B = p.Spawn(p.MovingPlatform, env, 65f, 34f);
        ConfigureMovingPlatform(mp2B, new Vector3(4f, 0f, 0f), 3.0f, 0.3f);

        GameObject db2B2 = p.Spawn(p.DashBoostFast, inter, 74f, 31.5f);
        ConfigureDashZoneBoost(db2B2, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f);

        // Overhead Purple Waterfall directly above Zone 1 Roof! (X = 79..87, Y = 32..40)
        for (int x = 79; x <= 87; x++)
            for (int y = 32; y <= 40; y++)
                if (y >= 38 - (x - 79) || x == 82 || x == 85)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        p.Spawn(p.DashCharge, inter, 90f, 35f);
        p.Spawn(p.OneWayPlatformLarge, env, 88f, 37f);                 // High truss X=88..96
        SetAtomId(p.Spawn(p.Atom, col, 92f, 38.5f), 8);

        // Final Upper Loopback Bastion Gate (X = 97..102, Y = 31..37) -> Atom 9 -> Sanctuary 2!
        p.SpawnTouchWallCol(inter, 97f, 31f, 37f);
        for (int y = 31; y <= 37; y++) p.Spawn(p.DashWallChainRow1, inter, 98f, y); // 5x7 yellow chains!
        p.SpawnTouchWallCol(inter, 103f, 31f, 37f);
        SetAtomId(p.Spawn(p.Atom, col, 105.5f, 32.5f), 9);

        // =====================================================================
        // SANCTUARY 2: AQUEDUCT LOCKHOUSE (X = 108 to 122, Y = 30 to 42)
        // =====================================================================
        p.SpawnGroundSized(env, 108f, 30f, 14);                        // X=108..122, Y=29..30
        p.SpawnCeilingSized(env, 108f, 42f, 14);                       // X=108..122, Y=41..42
        p.SpawnBlock(p.BlockMagenta, env, 109f, 41f, 12, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 115f, 41f, 12, 2, 4);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 115f, 31.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE DIAGONAL CRYSTAL AQUEDUCT (Coaster & Multi-Layer Ground-Pound Wells)
        // X = 122 to 162, Y = 30 down to -12 (Clear of Z1 Alcove 109..119!)
        // =====================================================================
        // Enclosed Chute Roof (122..140 at 42 -> step -> 140..173 at 33 -> East Backwall X=172)
        p.SpawnCeilingSized(env, 122f, 42f, 18);                       // X=122..140, Y=41..42
        p.SpawnBlock(p.BlockCyan, env, 139f, 41f, 2, 18, 4);           // Ceiling step down Y=32..41 flush against 140 roof!
        for (int x = 140; x < 173; x += 20)
            p.SpawnCeilingSized(env, x, 33f, Mathf.Min(20, 173 - x));  // Roof Y=32..33
        p.SpawnBlock(p.BlockCyan, env, 172f, 32f, 2, 66, 4);           // East Aqueduct Wall X=172..173, Y=-1..32!

        // 3A Steep Slope + Ice Coaster (X=122..150, Y=30 -> 18)
        p.SpawnGroundSized(env, 122f, 30f, 12);                        // X=122..134, Y=29..30
        p.SpawnBlock(p.BlockCyan, env, 121f, 29f, 2, 114, 4);          // Aqueduct West Rock Pillar X=121..122, Y=-28..29!
        GameObject db3 = p.Spawn(p.DashBoostFast, inter, 125f, 31.5f);
        ConfigureDashZoneBoost(db3, new Vector2(3f, 3f), 32f, 0.34f, 20f);
        for (int x = 129; x <= 131; x++)
            for (int y = 31; y <= 34; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Entry crystal curtain

        p.Spawn(p.Slope45Left, env, 135f, 28f);                        // 134..136, Y=28..30
        p.Spawn(p.Slope45Left, env, 137f, 26f);                        // 136..138, Y=26..28
        p.SpawnBlock(p.BlockCyan, env, 134f, 28f, 4, 4, 4);            // Sub-slope backing
        p.Spawn(p.IceSurface, env, 138f, 26f);                         // X=138..144, Y=25..26!
        p.SpawnGroundSized(env, 134f, 26f, 4);                         // 134..138, Y=25..26
        GameObject stomp3A = p.Spawn(p.StompBlock, haz, 141f, 30f);
        ConfigureStompBlock(stomp3A, 0.25f);

        p.Spawn(p.Slope45Left, env, 145f, 24f);                        // 144..146, Y=24..26
        p.Spawn(p.Slope45Left, env, 147f, 22f);                        // 146..148, Y=22..24
        p.Spawn(p.Slope45Left, env, 149f, 20f);                        // 148..150, Y=20..22
        p.SpawnGroundSized(env, 144f, 20f, 6);                         // Landing deck X=144..150, Y=19..20
        p.SpawnBlock(p.BlockCyan, env, 149f, 19f, 2, 32, 4);           // Shaft West Divider X=149..150, Y=3..19!

        // Off-Angle Upper Aqueduct Barrel Detour (X = 160..172, Y = 22..31 -> Atom 10!)
        GameObject barrel3Off = p.Spawn(p.DashHold, inter, 154f, 22f);
        ConfigureDashZoneHold(barrel3Off, 18f, new Vector2(3.2f, 3.2f), 32f, 0.34f, 20f, 0.30f);
        p.SpawnGroundSized(env, 160f, 22f, 12);                        // Pocket Floor X=160..172, Y=21..22
        GameObject saw3P = p.Spawn(p.SawSlow, haz, 162f, 25f);
        ConfigureSaw(saw3P, new Vector3(4f, 0f, 0f), 2.4f);
        for (int x = 165; x <= 167; x++)
            for (int y = 23; y <= 26; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Purple pocket blocks!
        SetAtomId(p.Spawn(p.Atom, col, 169.5f, 23.5f), 10);

        // 3B: Plunge Well #1 (X = 150..160, Y = 18 -> +2) + Side-Ricochet Pocket (X = 160..172, Y = 2..11)
        p.Spawn(p.OneWayPlatformLarge, env, 150f, 18f);                // Grate X=150..158
        p.Spawn(p.OneWayPlatformShort, env, 158f, 18f);                // 158..160
        p.SpawnBlock(p.BlockCyan, env, 160f, 21f, 2, 20, 4);           // Pocket/Shaft East Wall Y=11..21 (open 2..10!)

        for (int x = 150; x <= 159; x++) p.Spawn(p.TouchWallBlue, inter, x, 16f);
        for (int x = 151; x <= 158; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 11f); // Y=10..15!
        for (int x = 151; x <= 157; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 9f);   // Y=7..9
        for (int x = 150; x <= 159; x++) p.Spawn(p.TouchWallBlue, inter, x, 6f);

        p.SpawnGroundSized(env, 150f, 2f, 10);                         // Shaft 1 Catch Floor X=150..160, Y=1..2
        p.SpawnBlock(p.BlockCyan, env, 159f, 1f, 2, 8, 4);             // Support X=159..160, Y=-3..1
        p.Spawn(p.BouncePad, inter, 152f, 2f);                         // Easy recovery UP!
        GameObject dz3Side = p.Spawn(p.DashZone, inter, 156f, 4f);
        ConfigureDashZoneBoost(dz3Side, new Vector2(3f, 3f), 32f, 0.32f, 20f); // Shoots RIGHT into Atom 11 Pocket!

        // Mid Side-Ricochet Pocket (X = 160..172, Y = 2..10 -> Atom 11!)
        p.SpawnGroundSized(env, 160f, 2f, 12);                         // X=160..172, Y=1..2
        p.SpawnCeilingSized(env, 160f, 11f, 12);                       // X=160..172, Y=10..11
        p.SpawnBlock(p.BlockCyan, env, 160f, 10f, 2, 6, 4);            // Lintel Y=7..10
        for (int y = 3; y <= 7; y++) p.Spawn(p.DashWall, inter, 160f, y); // Yellow door Y=2..7
        GameObject stomp3B = p.Spawn(p.StompBlock, haz, 164f, 8.0f);
        ConfigureStompBlock(stomp3B, 0.3f);
        for (int x = 166; x <= 167; x++)
            for (int y = 3; y <= 5; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 169.5f, 3.5f), 11);

        // 3C: Switchback Plunge Well #2 (X = 138..150, Y = 2 -> -12) + West Basin (122..139, Y=2) + Secret Vault #2 (122..138, Y=-12..-4)
        p.Spawn(p.OneWayPlatformLarge, env, 139f, 2f);                 // Grate X=139..147
        p.Spawn(p.OneWayPlatformShort, env, 147f, 2f);                 // 147..149
        p.SpawnGroundSized(env, 122f, 2f, 17);                         // Open West Smash Basin Floor X=122..139, Y=1..2
        p.SpawnBlock(p.BlockCyan, env, 122f, 1f, 34, 8, 4);            // Solid masonry slab X=122..139, Y=-3..1!
        GameObject db3West = p.Spawn(p.DashBoostLeft, inter, 136f, 3.5f);
        ConfigureDashZoneBoost(db3West, new Vector2(3f, 3f), 32f, 0.30f, 18f);
        for (int x = 127; x <= 132; x++)
            for (int y = 3; y <= 7; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Giant 6x5 Purple basin crystal cluster!
        p.Spawn(p.BouncePad, inter, 124f, 2f);

        for (int x = 139; x <= 148; x++) p.Spawn(p.TouchWallBlue, inter, x, 0f);
        for (int x = 140; x <= 147; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -5f); // Y=-6..-1
        for (int x = 139; x <= 148; x++) p.Spawn(p.TouchWallBlue, inter, x, -8f);

        // Secret Vault #2 (X = 123..138, Y = -12..-4) -> Diamond 2!
        p.SpawnGroundSized(env, 123f, -12f, 15);                       // Floor X=123..138, Y=-13..-12
        p.SpawnBlock(p.BlockCyan, env, 123f, -3f, 2, 18, 4);           // Vault West Wall X=123..124, Y=-12..-3
        p.SpawnCeilingSized(env, 124f, -3f, 14);                       // Vault Roof Y=-4..-3
        p.SpawnBlock(p.BlockMagenta, env, 138f, -3f, 2, 4, 4);         // Door lintel Y=-5..-3
        for (int y = -11; y <= -5; y++) p.Spawn(p.DashWall, inter, 138f, y); // Yellow secret door Y=-12..-5
        GameObject leaper3 = p.Spawn(p.LavaLeaper, haz, 133f, -12f);
        ConfigureLavaLeaper(leaper3, 4.5f);
        for (int x = 130; x <= 131; x++)
            for (int y = -11; y <= -9; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetDiamondId(p.Spawn(p.Diamond, col, 126.5f, -10.5f), 2);

        // Zone 3 Bottom Floor -> Sanctuary 3 (X = 138..162, Y = -12)
        p.SpawnGroundSized(env, 138f, -12f, 24);                       // X=138..162, Y=-13..-12
        p.SpawnCeilingSized(env, 150f, -3f, 12);                       // Covers exit hall X=150..162, Y=-4..-3
        foreach (float px in new[] { 126f, 144f, 158f })
            p.SpawnBlock(p.BlockCyan, env, px, -13f, 2, 30, 4);        // Piers Y=-28..-13
        GameObject saw3B = p.Spawn(p.SawSlow, haz, 151f, -10f);
        ConfigureSaw(saw3B, new Vector3(5f, 0f, 0f), 2.5f);
        SetAtomId(p.Spawn(p.Atom, col, 158.5f, -10.5f), 12);

        // =====================================================================
        // SANCTUARY 3: DOME THRESHOLD (X = 162 to 176, Y = -12 to -3)
        // =====================================================================
        p.SpawnGroundSized(env, 162f, -12f, 14);                       // X=162..176, Y=-13..-12
        p.SpawnCeilingSized(env, 162f, -3f, 14);                       // X=162..176, Y=-4..-3
        p.SpawnBlock(p.BlockGreen, env, 163f, -4f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 169f, -4f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 175f, -13f, 2, 30, 4);          // Pier X=175..176, Y=-28..-13 flush with Z4 floor!
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 169f, -10.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE GRAND DOME BARREL LABYRINTH (Pinball -> High Glide Waterfall -> Finale!)
        // X = 176 to 264, Y = -14 up to +38
        // =====================================================================
        // Stepped Dome Shell
        p.SpawnBlock(p.BlockCyan, env, 174f, 37f, 2, 80, 4);           // Dome West Step Wall Y=-3..37 (above Sanct 3!)
        for (int x = 174; x < 264; x += 20)
            p.SpawnCeilingSized(env, x, 38f, Mathf.Min(20, 264 - x));  // Grand Dome Roof Y=37..38 flush on 174 wall!
        foreach (float dx in new[] { 186f, 206f, 226f, 246f })
            p.SpawnBlock(p.BlockMagenta, env, dx, 37f, 6, 2, 4);       // Vaulted rib accents
        p.SpawnBlock(p.BlockCyan, env, 264f, 38f, 2, 132, 4);          // East Citadel Backwall X=264..265, Y=-28..38

        // Continuous Dome Floor & Recovery Piers (Y = -14, X = 176..264)
        for (int x = 176; x < 264; x += 20)
            p.SpawnGroundSized(env, x, -14f, Mathf.Min(20, 264 - x));
        foreach (float px in new[] { 184f, 202f, 220f, 238f, 256f })
            p.SpawnBlock(p.BlockCyan, env, px, -15f, 2, 26, 4);        // Piers Y=-28..-15

        // 4A: Dome Entry Launch -> Vertical Pinball -> Extended High-Altitude GLIDE Waterfall!
        GameObject db4Entry = p.Spawn(p.DashBoostFast, inter, 180f, -12.5f);
        ConfigureDashZoneBoost(db4Entry, new Vector2(3.2f, 3.2f), 34f, 0.34f, 22f);
        p.Spawn(p.Slope45Large, env, 187f, -14f);                      // Kicker X=185..189, Y=-14..-10
        p.SpawnGroundSized(env, 189f, -10f, 7);                        // Upper Dais X=189..196, Y=-11..-10
        p.SpawnBlock(p.BlockCyan, env, 189f, -11f, 2, 6, 4);           // Pier -14..-11
        p.Spawn(p.BouncePad, inter, 193f, -10f);

        GameObject barrel4A = p.Spawn(p.DashHold, inter, 193f, -3f);
        ConfigureDashZoneHold(barrel4A, 65f, new Vector2(3.4f, 3.4f), 36f, 0.38f, 24f, 0.22f); // Fires 65 deg UP-RIGHT!

        p.Spawn(p.DashLaneUp, inter, 200f, 12f);                       // Y=8..16 catches Barrel 4A cleanly!
        GameObject db4Up = p.Spawn(p.DashBoostUp, inter, 200f, 18f);   // Y=16.5..19.5 shoots straight UP!
        ConfigureDashZoneBoost(db4Up, new Vector2(3.4f, 3.4f), 34f, 0.28f, 22f);
        for (int x = 197; x <= 203; x++) p.Spawn(p.TouchWallBlue, inter, x, 21f); // Overhead ring!

        GameObject barrel4B = p.Spawn(p.DashHold, inter, 200f, 26f);
        ConfigureDashZoneHold(barrel4B, -8f, new Vector2(3.6f, 3.6f), 30f, 0.26f, 20f, 0.28f); // Launches RIGHT into GLIDE!

        // COLOSSAL PURPLE TOUCH-BLOCK WATERFALL (X = 207..226, cascading diagonally Y=29 down to Y=16!)
        for (int x = 207; x <= 226; x++)
        {
            float baseCenter = 27.5f - (x - 207f) * 0.45f;
            for (int y = Mathf.FloorToInt(baseCenter - 3f); y <= Mathf.CeilToInt(baseCenter + 3f); y++)
            {
                if ((x + y) % 2 == 0 || Mathf.Abs(y - baseCenter) <= 1.2f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 212f, 25f), 13);
        SetDiamondId(p.Spawn(p.Diamond, col, 218f, 22.5f), 3);
        SetAtomId(p.Spawn(p.Atom, col, 225f, 19.5f), 14);

        // Mid-Dome Catapult Landing & Recovery Updraft (X = 222..241, Y = 12)
        p.Spawn(p.OneWayPlatformLarge, env, 222f, 12f);                // X=222..230 (allows recovery updraft!)
        p.SpawnGroundSized(env, 230f, 12f, 11);                        // X=230..241, Y=11..12
        p.SpawnBlock(p.BlockCyan, env, 230f, 11f, 2, 50, 4);           // Tall pier X=230..231, Y=-14..11

        // Overhead Cannon_Fast Bracket firing LEFT over the Glide Landing!
        p.SpawnBlock(p.BlockMagenta, env, 238f, 23f, 4, 2, 4);         // Bracket X=238..240, Y=22..23
        GameObject cannon4 = p.Spawn(p.CannonFast, haz, 239f, 24.0f);
        ConfigureCannon(cannon4, new Vector3(0f, 0f, 180f), 11f, 2.2f, 26f);

        // 4B: Finale Cannon Runway -> Slope_45_Large -> DashZone -> 7x6 Grand Bastion -> End_Portal!
        GameObject db4Fin = p.Spawn(p.DashBoostFast, inter, 233f, 13.5f);
        ConfigureDashZoneBoost(db4Fin, new Vector2(3.2f, 3.2f), 34f, 0.34f, 22f);
        p.Spawn(p.Slope45Large, env, 239f, 12f);                       // Kicker X=237..241, Y=12..16

        p.SpawnGroundSized(env, 241f, 16f, 23);                        // Finale Throne Deck X=241..264, Y=15..16
        p.SpawnBlock(p.BlockMagenta, env, 241f, 15f, 2, 58, 4);        // Dais front pier X=241..242, Y=-14..15

        GameObject dz4Finale = p.Spawn(p.DashZone, inter, 244f, 17.8f);
        ConfigureDashZoneBoost(dz4Finale, new Vector2(3.4f, 3.4f), 38f, 0.46f, 24f);
        SetAtomId(p.Spawn(p.Atom, col, 247.5f, 17.8f), 15);

        // 7m-deep x 6m-tall Grand Bastion (TouchWall + DashWall_ChainRow 1 x6 + TouchWall!)
        p.SpawnTouchWallCol(inter, 249f, 17f, 22f);
        for (int y = 17; y <= 22; y++) p.Spawn(p.DashWallChainRow1, inter, 250f, y); // 5x6 = 30 yellow chain blocks!
        p.SpawnTouchWallCol(inter, 255f, 17f, 22f);
        SetAtomId(p.Spawn(p.Atom, col, 257.5f, 17.8f), 16);

        // Continuous Dome Recovery Bounce Pads & Updrafts (Floor Y = -14)
        p.Spawn(p.BouncePadMega, inter, 198f, -14f);
        p.Spawn(p.BouncePadMega, inter, 212f, -14f);
        p.Spawn(p.BouncePad, inter, 225f, -14f);
        p.Spawn(p.DashLaneUp, inter, 225f, -4f);                       // Y=-8..0 -> straight through 222..230 OneWay!
        GameObject dbRec = p.Spawn(p.DashBoostUp, inter, 225f, 5f);
        ConfigureDashZoneBoost(dbRec, new Vector2(3f, 3f), 34f, 0.28f, 22f);

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
        return $"Level_42 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 64f;
            lm.TwoStarTime = 98f;
            lm.OneStarTime = 165f;
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
