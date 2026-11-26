#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level44Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_44.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 44 (Kinetic Sawtooth Pendulum)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 140f, -36f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(380f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(380f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -8 to 292)
        for (int x = -8; x < 292; x += 20)
            p.SpawnGroundSized(env, x, -28f, Mathf.Min(20, 292 - x));

        // =====================================================================
        // FLOW: HIGH BELFRY START PORTAL & PENDULUM APEX END PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 5f, 32f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 278.5f, 20.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Sharply Contrasting Non-Overlapping Mosaic!)
        // =====================================================================
        GameObject cz1A = p.Spawn(p.CameraZoneTight, flow, 14f, 34f);
        cz1A.name = "CameraZone_Tight_Z1Belfry";
        SetTriggerBoxSize(cz1A, new Vector3(28f, 22f, 10f));          // X=0..28, Y=23..45

        GameObject cz1B = p.Spawn(p.CameraZoneWide, flow, 52f, 13f);
        cz1B.name = "CameraZone_Wide_Z1DiveReveal";
        SetTriggerBoxSize(cz1B, new Vector3(48f, 62f, 10f));          // X=28..76, Y=-18..44

        GameObject cz2A = p.Spawn(p.CameraZoneTight, flow, 113f, -9f);
        cz2A.name = "CameraZone_Tight_Z2PistonTrench";
        SetTriggerBoxSize(cz2A, new Vector3(74f, 16f, 10f));          // X=76..150, Y=-17..-1

        GameObject cz2B = p.Spawn(p.CameraZoneClimb, flow, 169f, 2f);
        cz2B.name = "CameraZone_Climb_Z2EastSurge";
        SetTriggerBoxSize(cz2B, new Vector3(38f, 40f, 10f));          // X=150..188, Y=-18..22

        GameObject cz3 = p.Spawn(p.CameraZoneTight, flow, 120f, 11f);
        cz3.name = "CameraZone_Tight_Z3TurbineUndertow";
        SetTriggerBoxSize(cz3, new Vector3(92f, 20f, 10f));           // X=74..166, Y=1..21

        GameObject cz4A = p.Spawn(p.CameraZoneWide, flow, 131f, 30.5f);
        cz4A.name = "CameraZone_Wide_Z4Overpass";
        SetTriggerBoxSize(cz4A, new Vector3(114f, 17f, 10f));         // X=74..188, Y=22..39

        GameObject cz4B = p.Spawn(p.CameraZoneClimb, flow, 197f, 10f);
        cz4B.name = "CameraZone_Climb_Z4Quarry";
        SetTriggerBoxSize(cz4B, new Vector3(18f, 58f, 10f));          // X=188..206, Y=-19..39

        GameObject cz4C = p.Spawn(p.CameraZoneWide, flow, 243f, 10f);
        cz4C.name = "CameraZone_Wide_Z4ApexDome";
        SetTriggerBoxSize(cz4C, new Vector3(74f, 58f, 10f));          // X=206..280, Y=-19..39

        // =====================================================================
        // ZONE 1: THE COLLAPSING BELFRY PLUNGE (High Start Y=32 -> Steep Dive Y=-16)
        // X = 0 to 76, Y = 44 down to -16
        // =====================================================================
        p.SpawnBlock(p.BlockCyan, env, 0f, 43f, 2, 142, 4);            // Grand West Wall X=0..1, Y=-28..43
        for (int x = 0; x < 74; x += 20)
            p.SpawnCeilingSized(env, x, 44f, Mathf.Min(20, 74 - x));   // High Belfry Roof Y=43..44
        foreach (float rx in new[] { 12f, 28f, 44f, 60f })
            p.SpawnBlock(p.BlockGreen, env, rx, 43f, 4, 2, 4);

        // 1A: Belfry Opening & Collapsing Plank Bridge (X = 1..52, Y = 32 -> 18)
        p.SpawnGroundSized(env, 1f, 32f, 13);                          // Start Deck X=1..14, Y=31..32
        p.SpawnBlock(p.BlockCyan, env, 13f, 31f, 2, 12, 4);            // Deck pier Y=25..31 flush atop Ground!
        p.SpawnTouchWallCol(inter, 10f, 33f, 36f);                     // Belfry door smash
        GameObject db1 = p.Spawn(p.DashBoostFast, inter, 13f, 33.5f);
        ConfigureDashZoneBoost(db1, new Vector2(3.2f, 3.2f), 34f, 0.36f, 22f);

        // Collapsing CrumblingBlock Belfry Bridge shredding Overhead Purple Wave
        p.Spawn(p.CrumblingBlock, env, 16f, 32f);
        p.Spawn(p.CrumblingBlock, env, 19.5f, 32f);
        p.Spawn(p.CrumblingBlock, env, 23f, 32f);
        for (int x = 16; x <= 25; x++)
            for (int y = 34; y <= 37; y++)
                if ((x + y) % 2 == 0 || y <= 35) p.Spawn(p.TouchWallBlue, inter, x, y);

        // Lower Recovery & Kinetic Slope/Ice Line beneath Belfry Bridge (Y = 25 -> 19)
        p.SpawnGroundSized(env, 1f, 25f, 15);                          // X=1..16, Y=24..25
        p.Spawn(p.IceSurface, env, 16f, 25f);                          // X=16..22 slide!
        p.SpawnGroundSized(env, 22f, 25f, 6);                          // X=22..28
        p.SpawnBlock(p.BlockCyan, env, 1f, 24f, 54, 2, 4);             // Cantilevered stone truss X=1..28, Y=23..24
        p.Spawn(p.BouncePad, inter, 26f, 25f);                         // Springs straight into Barrel 1A!

        // Upper Express Line: Barrel 1A (-22 deg) -> Diagonal Wave -> Crumbling -> MovingPlatform -> DashCharge -> Atom 1!
        GameObject barrel1A = p.Spawn(p.DashHold, inter, 29f, 33.5f);
        ConfigureDashZoneHold(barrel1A, -20f, new Vector2(3.4f, 3.4f), 34f, 0.38f, 22f, 0.22f);
        for (int x = 33; x <= 42; x++)
            for (int y = 27; y <= 33; y++)
                if (Mathf.Abs((x - 33) * 0.4f + (y - 31f)) <= 1.8f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        GameObject saw1 = p.Spawn(p.SawSlow, haz, 38f, 33.5f);
        ConfigureSaw(saw1, new Vector3(6f, 0f, 0f), 2.8f);
        p.Spawn(p.CrumblingBlock, env, 35f, 26f);
        GameObject mp1A = p.Spawn(p.MovingPlatform, env, 43f, 25f);
        ConfigureMovingPlatform(mp1A, new Vector3(0f, 5f, 0f), 3.0f, 0.25f);
        p.Spawn(p.DashCharge, inter, 48f, 28f);
        SetAtomId(p.Spawn(p.Atom, col, 45.5f, 31.5f), 1);

        // Lower Kinetic Slopes (28..52, Y=25 -> 19) feeding Dive Quarry
        p.Spawn(p.Slope45Left, env, 29f, 23f);                         // 28..30, 23..25
        p.Spawn(p.Slope45Left, env, 31f, 21f);                         // 30..32, 21..23
        p.Spawn(p.Slope45Left, env, 33f, 19f);                         // 32..34, 19..21
        p.SpawnBlock(p.BlockCyan, env, 28f, 23f, 4, 8, 4);             // Clean backing 28..30, Y=19..23
        p.SpawnBlock(p.BlockCyan, env, 30f, 21f, 4, 4, 4);             // Clean backing 30..32, Y=19..21
        p.Spawn(p.IceSurface, env, 34f, 19f);                          // High-speed slide X=34..40, Y=18..19
        p.SpawnGroundSized(env, 40f, 19f, 12);                         // X=40..52, Y=18..19
        GameObject db1B = p.Spawn(p.DashBoostFast, inter, 44f, 20.5f);
        ConfigureDashZoneBoost(db1B, new Vector2(3.2f, 3.2f), 34f, 0.32f, 22f);

        // 1B: Steep Kinetic Dive Shaft (X = 52..74, Y = +24 -> -16) & Yellow Side-Wall Secret Alcove (38..52, Y = 8..17)
        p.SpawnBlock(p.BlockCyan, env, 73f, 43f, 2, 78, 4);            // Dive East Wall X=73..74, Y=4..43 (flush atop Sanct 3!)
        p.SpawnBlock(p.BlockMagenta, env, 52f, 18f, 2, 6, 4);          // Alcove Door Lintel Y=15..18
        for (int y = 9; y <= 15; y++) p.Spawn(p.DashWall, inter, 52f, y); // Secret Door X=52, Y=8..15!
        p.SpawnBlock(p.BlockCyan, env, 52f, 8f, 2, 48, 4);             // Shaft West Wall Y=-16..8

        // Secret Dive Alcove (X = 38..52, Y = 8..17) -> Atom 2!
        p.SpawnGroundSized(env, 38f, 8f, 14);                          // X=38..52, Y=7..8
        p.SpawnCeilingSized(env, 38f, 17f, 14);                        // X=38..52, Y=16..17
        p.SpawnBlock(p.BlockMagenta, env, 38f, 16f, 2, 16, 4);         // Back Wall Y=8..16
        p.Spawn(p.FireJet, haz, 45f, 8f);
        for (int x = 47; x <= 49; x++)
            for (int y = 9; y <= 12; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 40.5f, 9.5f), 2);

        // Dive Quarry Strata (X = 53..73)
        p.Spawn(p.OneWayPlatformLarge, env, 55f, 19f);                 // Grate X=55..63
        p.Spawn(p.OneWayPlatformLarge, env, 63f, 19f);                 // Grate X=63..71
        for (int x = 57; x <= 67; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 17f); // Y=15..17
        for (int x = 54; x <= 72; x++) p.Spawn(p.TouchWallBlue, inter, x, 14f);
        for (int x = 55; x <= 71; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 9f); // Y=8..13

        GameObject mp1B = p.Spawn(p.MovingPlatform, env, 64f, 4f);
        ConfigureMovingPlatform(mp1B, new Vector3(-7f, 0f, 0f), 3.2f, 0.25f);
        GameObject stomp1 = p.Spawn(p.StompBlock, haz, 58f, 3.5f);
        ConfigureStompBlock(stomp1, 0.28f);

        p.Spawn(p.OneWayPlatformLarge, env, 55f, 0f);
        p.Spawn(p.OneWayPlatformLarge, env, 63f, 0f);
        for (int x = 54; x <= 72; x++) p.Spawn(p.TouchWallBlue, inter, x, -2f);
        for (int x = 59; x <= 71; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, -8f); // Clears X=54..58 DashLane_UP!
        for (int x = 54; x <= 72; x++) p.Spawn(p.TouchWallBlue, inter, x, -11f);

        // Zone 1 Base Trench Floor (X = 52..76, Y = -16) -> Recovery into Dive/Alcove or Enter Sanct 1!
        p.SpawnGroundSized(env, 52f, -16f, 24);                        // X=52..76, Y=-17..-16
        p.SpawnBlock(p.BlockCyan, env, 52f, -17f, 2, 22, 4);
        GameObject leaper1 = p.Spawn(p.LavaLeaper, haz, 61f, -16f);
        ConfigureLavaLeaper(leaper1, 5.0f);
        p.Spawn(p.BouncePad, inter, 56f, -16f);
        p.Spawn(p.DashLaneUp, inter, 56f, -8f);                        // Recovery right back up to Deck B/Alcove!
        SetAtomId(p.Spawn(p.Atom, col, 69f, -14.2f), 3);

        // =====================================================================
        // SANCTUARY 1: LOWER FORGE LOCKHOUSE (X = 76 to 90, Y = -16 to -7)
        // =====================================================================
        p.SpawnGroundSized(env, 76f, -16f, 14);                        // X=76..90, Y=-17..-16
        p.SpawnCeilingSized(env, 76f, -7f, 14);                        // X=76..90, Y=-8..-7
        p.SpawnBlock(p.BlockGreen, env, 77f, -8f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 83f, -8f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 83f, -17f, 2, 22, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 83f, -14.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE LIVING PISTON FORGE (Low Trench -16..-2 -> Up-Right Surge -> Peak +14)
        // 2A Trench: X = 90..150, Y = -16..-2 | 2B Surge: X = 150..188, Y = -16..+14
        // =====================================================================
        // 2A Living Piston Trench Floor (split cleanly around IceSurface 112..118!)
        p.SpawnGroundSized(env, 90f, -16f, 22);                        // X=90..112
        p.Spawn(p.IceSurface, env, 112f, -16f);                        // X=112..118!
        p.SpawnGroundSized(env, 118f, -16f, 32);                       // X=118..150
        foreach (float px in new[] { 98f, 116f, 134f, 150f })
            p.SpawnBlock(p.BlockCyan, env, px, -17f, 2, 22, 4);        // Piers Y=-28..-17

        for (int x = 90; x < 150; x += 20)
            p.SpawnCeilingSized(env, x, -2f, Mathf.Min(20, 150 - x));  // Trench Roof Y=-3..-2 (clean below Z3 Y=4!)

        GameObject db2A = p.Spawn(p.DashBoostFast, inter, 92f, -14.5f);
        ConfigureDashZoneBoost(db2A, new Vector2(3.2f, 3.2f), 36f, 0.38f, 22f);
        for (int x = 96; x <= 99; x++)
            for (int y = -15; y <= -10; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Piston Wall #1 (96..100)

        // Synced Moving Shuttle Pair + StompBlock + FireJet in Piston Trench
        GameObject mp2A1 = p.Spawn(p.MovingPlatform, env, 104f, -12f); // X=102..106 cleanly right of Wall #1!
        ConfigureMovingPlatform(mp2A1, new Vector3(4f, 0f, 0f), 3.4f, 0.2f);
        GameObject stomp2A = p.Spawn(p.StompBlock, haz, 107f, -4.5f);
        ConfigureStompBlock(stomp2A, 0.25f);
        p.Spawn(p.FireJet, haz, 110f, -16f);

        GameObject db2Ice = p.Spawn(p.DashBoostFast, inter, 114f, -14.5f);
        ConfigureDashZoneBoost(db2Ice, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        p.Spawn(p.BouncePadMini, inter, 121f, -16f);

        GameObject barrel2A = p.Spawn(p.DashHold, inter, 126f, -11f);
        ConfigureDashZoneHold(barrel2A, 0f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 24f, 0.22f);
        for (int x = 131; x <= 135; x++)
            for (int y = -15; y <= -9; y++)
                p.Spawn((x == 133 && y >= -13 && y <= -10) ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x == 133 && y >= -13 && y <= -10) ? x + 0.5f : x,
                    (x == 133 && y >= -13 && y <= -10) ? y - 0.5f : y);
        SetAtomId(p.Spawn(p.Atom, col, 138f, -11f), 4);
        GameObject stomp2B = p.Spawn(p.StompBlock, haz, 142f, -4.5f);
        ConfigureStompBlock(stomp2B, 0.32f);
        p.Spawn(p.FireJet, haz, 146f, -16f);
        p.Spawn(p.BouncePad, inter, 103f, -16f);
        p.Spawn(p.BouncePad, inter, 140f, -16f);

        // 2B: Diagonal Up-Right Surge (X = 150..188, stays <= Y=+3 under Z3 Slopes, then climbs X=176..188 -> Peak +14!)
        p.SpawnGroundSized(env, 150f, -16f, 38);                       // Surge Base X=150..188, Y=-17..-16
        p.SpawnBlock(p.BlockCyan, env, 170f, -17f, 2, 22, 4);
        p.SpawnBlock(p.BlockCyan, env, 188f, 22f, 2, 100, 4);          // East Surge Wall X=188..189, Y=-28..22 (under Z4 Quarry!)

        p.Spawn(p.BouncePad, inter, 153f, -16f);
        p.Spawn(p.DashLaneUp, inter, 153f, -11f);                      // Y=-15..-7 shoots up into Barrel 2B!
        GameObject barrel2B = p.Spawn(p.DashHold, inter, 153f, -4f);
        ConfigureDashZoneHold(barrel2B, 22f, new Vector2(3.4f, 3.4f), 34f, 0.40f, 22f, 0.22f); // Fires UP-RIGHT below Z3!

        // Giant Checkerboard Drift (X = 158..164, Y = -6..-1 — clean below Z3 Floor Y=3..4!) -> DashCharge -> Atom 5!
        for (int x = 158; x <= 164; x++)
        {
            for (int y = -6; y <= -1; y++)
            {
                if (x >= 160 && x <= 162 && y >= -4 && y <= -2)
                    p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
                else if ((x + y) % 2 == 0)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
            }
        }
        p.Spawn(p.DashCharge, inter, 168f, -0.5f);                     // Express Air-Dash toward Express Shelf!
        p.Spawn(p.OneWayPlatformLarge, env, 169f, 2f);                 // Express Route Shelf X=169..177
        SetAtomId(p.Spawn(p.Atom, col, 173f, 3.8f), 5);                // Shines cleanly below Z3/Sanct 2!
        GameObject mp2BTop = p.Spawn(p.MovingPlatform, env, 178f, 6f);
        ConfigureMovingPlatform(mp2BTop, new Vector3(0f, 5f, 0f), 3.0f, 0.25f);

        // Lower Surge Kinetic Recovery Shuttles & Bounce Chain (162..188)
        p.Spawn(p.BouncePadMega, inter, 162f, -16f);
        GameObject mp2BMid = p.Spawn(p.MovingPlatform, env, 165f, -9f);
        ConfigureMovingPlatform(mp2BMid, new Vector3(6f, 0f, 0f), 3.4f, 0.2f);
        p.Spawn(p.OneWayPlatformLarge, env, 173f, -6f);                // X=173..181
        p.Spawn(p.BouncePadMega, inter, 177f, -6f);
        p.SpawnGroundSized(env, 181f, 2f, 7);
        p.Spawn(p.BouncePadMega, inter, 184f, 2f);
        p.Spawn(p.BouncePad, inter, 184f, -16f);
        p.Spawn(p.DashLaneUp, inter, 184f, 8f);                        // Y=4..12 -> climbs X=180..188 cleanly right of Sanct 2!
        for (int x = 180; x <= 187; x++) p.Spawn(p.TouchWallBlue, inter, x, 12f);
        for (int x = 181; x <= 187; x++) p.Spawn(p.DashWall, inter, x, 13f);

        p.Spawn(p.OneWayPlatformLarge, env, 180f, 14f);                // Peak Summit Landing X=180..188
        SetAtomId(p.Spawn(p.Atom, col, 185f, 15.8f), 6);

        // =====================================================================
        // SANCTUARY 2: EASTERN PEAK SWITCHBACK (X = 166 to 180, Y = 14 to 22)
        // =====================================================================
        p.SpawnGroundSized(env, 166f, 14f, 14);                        // X=166..180, Y=13..14
        p.SpawnCeilingSized(env, 166f, 22f, 14);                       // X=166..180, Y=21..22 (clean below Z4 Overpass Y=24!)
        p.SpawnBlock(p.BlockMagenta, env, 167f, 21f, 12, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 173f, 21f, 12, 2, 4);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 173f, 15.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE REVERSE TURBINE UNDERTOW (Rocket LEFT Between Z1 & Z2!)
        // X = 166 -> 84, Y = +4..+13 + False Yellow Ceiling Vault (108..122, Y=14..21 -> Diamond 1!)
        // =====================================================================
        // 5-step Left-Descending Slopes off Sanctuary 2 (166..156, Y=14 -> 4 — 0 overlaps!)
        p.Spawn(p.Slope45, env, 165f, 12f);                            // 164..166, 12..14 (flush with Sanct 2!)
        p.Spawn(p.Slope45, env, 163f, 10f);                            // 162..164, 10..12
        p.Spawn(p.Slope45, env, 161f, 8f);                             // 160..162, 8..10
        p.Spawn(p.Slope45, env, 159f, 6f);                             // 158..160, 6..8
        p.Spawn(p.Slope45, env, 157f, 4f);                             // 156..158, 4..6

        // Continuous Turbine Corridor Floor Y=4 (split cleanly around IceSurface 128..134!)
        p.SpawnGroundSized(env, 86f, 4f, 42);                          // X=86..128
        p.Spawn(p.IceSurface, env, 128f, 4f);                          // X=128..134!
        p.SpawnGroundSized(env, 134f, 4f, 22);                         // X=134..156 (stops cleanly left of Slope45!)
        foreach (float px in new[] { 96f, 114f, 136f, 156f })
            p.SpawnBlock(p.BlockCyan, env, px, 3f, 2, 10, 4);          // Support piers down to Z2 Ceiling Y=-2!

        // Turbine Corridor Ceiling Y=13 (split cleanly around Secret Diamond Vault Hatch 114..119!)
        p.SpawnCeilingSized(env, 86f, 13f, 27);                        // X=86..113, Y=12..13
        for (int x = 113; x <= 118; x++) p.Spawn(p.DashWall, inter, x, 13f); // False Yellow Ceiling Door Y=12..13!
        p.SpawnCeilingSized(env, 119f, 13f, 45);                       // X=119..164, Y=12..13

        // Secret Ceiling Vault (X = 108..123, Y = 14..21) -> Diamond 1 + Atom 8!
        p.SpawnBlock(p.BlockCyan, env, 108f, 20f, 2, 14, 4);           // Vault West Wall Y=13..20
        p.SpawnBlock(p.BlockCyan, env, 122f, 20f, 2, 14, 4);           // Vault East Wall Y=13..20
        p.SpawnCeilingSized(env, 108f, 21f, 16);                       // Vault Roof X=108..124, Y=20..21 (below Z4 Y=24!)
        for (int x = 110; x <= 111; x++) p.Spawn(p.TouchWallBlue, inter, x, 15f);
        SetDiamondId(p.Spawn(p.Diamond, col, 116f, 17.5f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 112f, 17.5f), 8);

        // Pressurized Turbine Boost Chain & Moving Hazards (154 -> 86)
        GameObject db3A = p.Spawn(p.DashBoostLeft, inter, 154f, 5.5f);
        ConfigureDashZoneBoost(db3A, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f);
        for (int x = 145; x <= 150; x++)
            for (int y = 5; y <= 11; y++)
                if (Mathf.Abs(x - 147.5f) + Mathf.Abs(y - 8f) <= 3.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);             // Turbine Thicket #1

        GameObject mp3A = p.Spawn(p.MovingPlatform, env, 141f, 6.5f);
        ConfigureMovingPlatform(mp3A, new Vector3(-4f, 0f, 0f), 3.0f, 0.2f);
        GameObject spin3 = p.Spawn(p.SpinArm, haz, 140f, 9.5f);
        ConfigureSpinArm(spin3, -90f);

        GameObject db3B = p.Spawn(p.DashBoostLeft, inter, 135f, 5.5f);
        ConfigureDashZoneBoost(db3B, new Vector2(3.2f, 3.2f), 38f, 0.44f, 24f); // Rockets across Ice into Gate #1!
        p.SpawnTouchWallCol(inter, 127f, 5f, 11f);
        for (int y = 5; y <= 11; y++) p.Spawn(p.DashWallChainRow1, inter, 122f, y); // 5x7 yellow gate (122..127)!
        SetAtomId(p.Spawn(p.Atom, col, 120f, 6f), 7);

        // Upward Secret Barrel Shot (DashHold@116,8.5 aiming 90 deg UP toward Diamond 1!)
        p.Spawn(p.BouncePadMini, inter, 116f, 4f);
        GameObject barrel3Secret = p.Spawn(p.DashHold, inter, 116f, 8.5f);
        ConfigureDashZoneHold(barrel3Secret, 90f, new Vector2(3.0f, 3.0f), 30f, 0.30f, 18f, 0.35f);
        p.Spawn(p.BouncePadMini, inter, 111f, 4f);

        GameObject db3C = p.Spawn(p.DashBoostLeft, inter, 107f, 5.5f);
        ConfigureDashZoneBoost(db3C, new Vector2(3.2f, 3.2f), 36f, 0.38f, 22f);
        for (int x = 97; x <= 102; x++)
            for (int y = 5; y <= 11; y++)
                if ((x + y) % 2 == 0 || y <= 8) p.Spawn(p.TouchWallBlue, inter, x, y); // Thicket #2
        GameObject saw3 = p.Spawn(p.SawSlow, haz, 93f, 6f);
        ConfigureSaw(saw3, new Vector3(0f, 4f, 0f), 2.8f);
        p.Spawn(p.CrumblingBlock, env, 90f, 6f);
        SetAtomId(p.Spawn(p.Atom, col, 88f, 5.8f), 9);

        // =====================================================================
        // SANCTUARY 3: TURBINE PIVOT LOCK (X = 74 to 86, Y = 4 to 13 -> Launch UP to Overpass!)
        // =====================================================================
        p.SpawnGroundSized(env, 74f, 4f, 12);                          // X=74..86, Y=3..4
        p.SpawnBlock(p.BlockCyan, env, 74f, 3f, 2, 10, 4);             // Pier down to -2
        p.Spawn(p.OneWayPlatformLarge, env, 74f, 13f);                 // Upward pass-through ceiling X=74..82!
        p.SpawnCeilingSized(env, 82f, 13f, 4);                         // Right cap X=82..86
        p.SpawnBlock(p.BlockGreen, env, 82f, 12f, 6, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 82f, 5.5f);
        cp3.name = "Checkpoint_3";

        // Slingshot Exit UP to Zone 4A Overpass (X = 78, Y = 4 -> 24)
        p.Spawn(p.BouncePad, inter, 78f, 4f);
        p.Spawn(p.DashLaneUp, inter, 78f, 15f);                        // Y=11..19 rockets UP cleanly!
        p.Spawn(p.OneWayPlatformLarge, env, 74f, 24f);                 // Overpass Catch Grate X=74..82!
        p.SpawnBlock(p.BlockCyan, env, 74f, 37f, 2, 26, 4);            // Overpass West Wall Y=24..37

        // =====================================================================
        // ZONE 4: THE PENDULUM APEX VAULT (Overpass -> Deep Quarry Plunge -> Moving Apex Dome!)
        // 4A Overpass: X = 74..188, Y = +24..+38 | 4B Quarry: X = 188..206, Y = +24 -> -18
        // 4C Moving Glide & Rapid-Fire Barrel Finale: X = 206..279, Y = -18..+38
        // =====================================================================
        for (int x = 74; x < 281; x += 20)
            p.SpawnCeilingSized(env, x, 38f, Mathf.Min(20, 281 - x));  // Continuous Grand Apex Roof Y=37..38
        foreach (float dx in new[] { 96f, 128f, 160f, 198f, 234f, 266f })
            p.SpawnBlock(p.BlockMagenta, env, dx, 37f, 6, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 281f, 38f, 2, 132, 4);          // East Citadel Backwall X=281..282, Y=-28..38

        // 4A: High Overpass (X = 82..188, Y = 24..38)
        for (int x = 82; x < 188; x += 20)
            p.SpawnGroundSized(env, x, 24f, Mathf.Min(20, 188 - x));   // Clear 2m ABOVE Z3 & Sanct 2!
        GameObject barrel4Launch = p.Spawn(p.DashHold, inter, 78f, 28f);
        ConfigureDashZoneHold(barrel4Launch, 0f, new Vector2(3.4f, 3.4f), 36f, 0.36f, 22f, 0.22f);

        for (int x = 85; x <= 93; x++)
            for (int y = 26; y <= 33; y++)
                if (Mathf.Abs(x - 89f) + Mathf.Abs(y - 29.5f) <= 3.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.CrumblingBlock, env, 97f, 26f);
        GameObject mp4A1 = p.Spawn(p.MovingPlatform, env, 103f, 27f);
        ConfigureMovingPlatform(mp4A1, new Vector3(7f, 0f, 0f), 3.4f, 0.2f);
        p.Spawn(p.DashBoostFast, inter, 117f, 25.5f);

        for (int x = 123; x <= 129; x++)
            for (int y = 25; y <= 31; y++)
                p.Spawn((x == 125 || x == 127) && y <= 29 ? p.DashWallCracked : p.TouchWallBlue,
                    inter, (x == 125 || x == 127) && y <= 29 ? x + 0.5f : x,
                    (x == 125 || x == 127) && y <= 29 ? y - 0.5f : y);
        SetAtomId(p.Spawn(p.Atom, col, 133f, 26f), 10);

        // Optional High Sky Niche for Diamond 2! (Above Overpass X = 148..162, Y = 33)
        p.Spawn(p.BouncePad, inter, 140f, 24f);
        GameObject barrel4A = p.Spawn(p.DashHold, inter, 145f, 29f);
        ConfigureDashZoneHold(barrel4A, 8f, new Vector2(3.4f, 3.4f), 36f, 0.40f, 24f, 0.22f);
        p.Spawn(p.OneWayPlatformLarge, env, 152f, 32.5f);
        SetDiamondId(p.Spawn(p.Diamond, col, 156f, 34f), 2);
        GameObject spin4A = p.Spawn(p.SpinArm, haz, 155f, 27f);
        ConfigureSpinArm(spin4A, 85f);

        GameObject mp4A2 = p.Spawn(p.MovingPlatform, env, 162f, 26f);
        ConfigureMovingPlatform(mp4A2, new Vector3(0f, 4f, 0f), 3.0f, 0.2f);
        p.Spawn(p.DashCharge, inter, 170f, 30f);
        for (int x = 173; x <= 178; x++)
            for (int y = 26; y <= 33; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 182f, 27.5f), 11);
        foreach (float bx in new[] { 98f, 150f, 172f }) p.Spawn(p.BouncePad, inter, bx, 24f);

        // 4B: Deep Quarry Plunge (X = 188..206, Y = +24 down to -18)
        p.Spawn(p.OneWayPlatformLarge, env, 189f, 24f);                // X=189..197
        p.Spawn(p.OneWayPlatformLarge, env, 197f, 24f);                // X=197..205
        for (int x = 191; x <= 201; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 22f); // Y=20..22
        for (int x = 189; x <= 205; x++) p.Spawn(p.TouchWallBlue, inter, x, 18f);

        GameObject mp4Q = p.Spawn(p.MovingPlatform, env, 192f, 12f);
        ConfigureMovingPlatform(mp4Q, new Vector3(7f, 0f, 0f), 3.6f, 0.2f);
        for (int x = 189; x <= 205; x++) p.Spawn(p.TouchWallBlue, inter, x, 8f);
        for (int x = 191; x <= 203; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 2f); // Y=1..6
        for (int x = 189; x <= 205; x++) p.Spawn(p.TouchWallBlue, inter, x, -3f);
        for (int x = 191; x <= 201; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, -6f); // Y=-8..-6

        // 4C: Continuous Arena Floor (X = 189..281, Y = -18, clean split around IceSurface 225..231!)
        p.SpawnGroundSized(env, 189f, -18f, 20);                       // 189..209
        p.SpawnGroundSized(env, 209f, -18f, 16);                       // 209..225
        p.Spawn(p.IceSurface, env, 225f, -18f);                        // Fast slide recovery 225..231!
        p.SpawnGroundSized(env, 231f, -18f, 20);                       // 231..251
        p.SpawnGroundSized(env, 251f, -18f, 20);                       // 251..271
        p.SpawnGroundSized(env, 271f, -18f, 10);                       // 271..281
        foreach (float px in new[] { 196f, 214f, 232f, 250f, 268f })
            p.SpawnBlock(p.BlockCyan, env, px, -19f, 2, 18, 4);        // Piers Y=-28..-19

        SetAtomId(p.Spawn(p.Atom, col, 196.5f, -15.5f), 12);
        p.Spawn(p.BouncePadMega, inter, 192f, -18f);                   // Quarry recovery!

        GameObject db4C1 = p.Spawn(p.DashBoostFast, inter, 203f, -16.5f);
        ConfigureDashZoneBoost(db4C1, new Vector2(3.2f, 3.2f), 34f, 0.32f, 22f);
        p.Spawn(p.Slope45Large, env, 211f, -18f);                      // Kicker 209..213, Y=-18..-14
        p.SpawnGroundSized(env, 213f, -14f, 8);                        // Launch dais 213..221, Y=-15..-14
        p.SpawnBlock(p.BlockCyan, env, 213f, -15f, 2, 6, 4);
        p.Spawn(p.BouncePadMega, inter, 217f, -14f);
        p.Spawn(p.DashLaneUp, inter, 217f, -4f);                       // Y=-8..0

        GameObject barrel4B = p.Spawn(p.DashHold, inter, 217f, 8f);
        ConfigureDashZoneHold(barrel4B, 14f, new Vector2(3.6f, 3.6f), 30f, 0.28f, 20f, 0.22f); // Fires RIGHT into GLIDE!

        // Moving Island #1 + Waterfall #1 + Atom 13 + DashCharge
        GameObject mpGlide1 = p.Spawn(p.MovingPlatform, env, 223f, 3f);
        ConfigureMovingPlatform(mpGlide1, new Vector3(0f, 6f, 0f), 3.0f, 0.2f);
        for (int x = 223; x <= 230; x++)
            for (int y = 9; y <= 21; y++)
                if (Vector2.Distance(new Vector2(x + 0.5f, y - 0.5f), new Vector2(227f, 15f)) >= 1.3f && ((x + y) % 2 == 0 || (y >= 13 && y <= 17)))
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 227f, 15f), 13);
        p.Spawn(p.DashCharge, inter, 233f, 14.5f);

        // Moving Island #2 + Cannon_Fast Crossfire + Waterfall #2 + Diamond 3
        GameObject mpGlide2 = p.Spawn(p.MovingPlatform, env, 235f, 5f);
        ConfigureMovingPlatform(mpGlide2, new Vector3(5f, 0f, 0f), 3.2f, 0.2f);
        for (int x = 236; x <= 245; x++)
            for (int y = 8; y <= 20; y++)
                if (Vector2.Distance(new Vector2(x + 0.5f, y - 0.5f), new Vector2(240.5f, 14f)) >= 1.3f && ((x + y) % 2 == 0 || Mathf.Abs(y - 14f) <= 1.5f))
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        SetDiamondId(p.Spawn(p.Diamond, col, 240.5f, 14f), 3);

        p.SpawnBlock(p.BlockMagenta, env, 250f, 17f, 6, 2, 4);         // Cannon bracket 250..253
        GameObject cannon4 = p.Spawn(p.CannonFast, haz, 251.5f, 18.2f);
        ConfigureCannon(cannon4, new Vector3(0f, 0f, 180f), 11f, 2.4f, 28f);

        // Arena Lower Recovery Catch-Lanes (Y = -18 -> +2)
        p.Spawn(p.BouncePadMega, inter, 234f, -18f);
        p.Spawn(p.BouncePadMega, inter, 243f, -18f);
        GameObject mpRec4 = p.Spawn(p.MovingPlatform, env, 246f, -9f);
        ConfigureMovingPlatform(mpRec4, new Vector3(0f, 8f, 0f), 3.2f, 0.2f);
        p.Spawn(p.BouncePad, inter, 251f, -18f);
        p.Spawn(p.DashLaneUp, inter, 251f, -7f);                       // Returns up through OneWay(249,2)!

        // 4D: Rapid-Fire DashBoost -> DashHold -> DashLane_UP -> DashBoostUp -> Barrel Final -> Triple Monolith -> End_Portal!
        p.Spawn(p.OneWayPlatformLarge, env, 249f, 2f);                 // Landing Grate X=249..257
        p.SpawnGroundSized(env, 257f, 2f, 24);                         // Lower Deck X=257..281
        SetAtomId(p.Spawn(p.Atom, col, 251f, 3.8f), 14);

        GameObject dbFinEntry = p.Spawn(p.DashBoostFast, inter, 255f, 3.5f);
        ConfigureDashZoneBoost(dbFinEntry, new Vector2(3.2f, 3.2f), 34f, 0.30f, 22f);

        GameObject barrel4FinHold = p.Spawn(p.DashHold, inter, 260f, 4.5f);
        ConfigureDashZoneHold(barrel4FinHold, 72f, new Vector2(3.2f, 3.2f), 34f, 0.28f, 22f, 0.18f); // Fires straight up into Lane!

        p.Spawn(p.DashLaneUp, inter, 262.5f, 12f);                     // Y=8..16 -> completely clear left of X=266 Throne!
        GameObject db4Up = p.Spawn(p.DashBoostUp, inter, 262.5f, 17f);
        ConfigureDashZoneBoost(db4Up, new Vector2(3.2f, 3.2f), 34f, 0.22f, 20f);

        GameObject barrel4Apex = p.Spawn(p.DashHold, inter, 262.5f, 21.5f);
        ConfigureDashZoneHold(barrel4Apex, 0f, new Vector2(3.6f, 3.6f), 38f, 0.45f, 24f, 0.18f); // Rockets RIGHT!

        // Royal Finale Throne Dais (X = 266..281, Y = 19)
        p.SpawnGroundSized(env, 266f, 19f, 15);                        // X=266..281, Y=18..19
        p.SpawnBlock(p.BlockMagenta, env, 266f, 18f, 2, 32, 4);        // Dais support column Y=2..18
        SetAtomId(p.Spawn(p.Atom, col, 266.2f, 21.5f), 15);

        // Grand Triple Monolith (Purple + DashWall_ChainRow 1 x5 rows = 25 yellow + Purple!)
        p.SpawnTouchWallCol(inter, 268f, 20f, 24f);
        for (int y = 20; y <= 24; y++) p.Spawn(p.DashWallChainRow1, inter, 269f, y); // 269..274!
        SetAtomId(p.Spawn(p.Atom, col, 275.2f, 20.8f), 16);

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
        return $"Level_44 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 68f;
            lm.TwoStarTime = 104f;
            lm.OneStarTime = 175f;
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
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject BlockGreen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Green Variant.prefab");
        public GameObject BlockMagenta = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");

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
