#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level52Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_52.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 52 (Shatter-Lock Viaduct)")]
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
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES (Viaduct X = 0..414, Y = -16..52)
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 207f, -24f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(500f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(500f, 4f, 1f);

        // Continuous Bedrock Base at Y = -16 (X = -8 to 424)
        for (int x = -8; x < 424; x += 20)
            p.SpawnGroundSized(env, x, -16f, Mathf.Min(20, 424 - x));

        // Enclosing West & East Viaduct Walls + Stepped Aqueduct Skyline Rooflines
        p.SpawnBlock(p.BlockCyan, env, 0f, 41f, 4, 114, 4);                // West Citadel Wall X=0..2, Y=-16..41
        p.Spawn(p.Wall1x12, env, 2f, 25f);                                // West Lock Fortification X=2..3, Y=13..25
        SpawnPierToY(p, env, 2f, 14f, -16f, 2);                           // Flush below Wall_1x12: Y=-16..13
        p.SpawnBlock(p.BlockCyan, env, 412f, 51f, 4, 134, 4);             // East Terminus Wall X=412..414, Y=-16..51

        for (int x = 0; x < 108; x += 20)
            p.SpawnCeilingSized(env, x, 42f, Mathf.Min(20, 108 - x));     // Act 1 Lock & Arch Roof X=0..108, Y=41..42
        p.SpawnBlock(p.BlockCyan, env, 108f, 41f, 4, 14, 4);              // Step down 41->34 (108..110)
        for (int x = 110; x < 224; x += 20)
            p.SpawnCeilingSized(env, x, 35f, Mathf.Min(20, 224 - x));     // Act 2 Bridge Deck Skyline Roof X=110..224, Y=34..35
        p.SpawnBlock(p.BlockCyan, env, 222f, 52f, 4, 34, 4);              // Step up to Twin-Arch Sky Gallery X=222..224, Y=35..52
        for (int x = 224; x < 414; x += 20)
            p.SpawnCeilingSized(env, x, 53f, Mathf.Min(20, 414 - x));     // Act 3+4 Soaring Cathedral Roof X=224..414, Y=52..53

        // Architectural Neon Viaduct Trim Ribs (Flush below rooflines)
        p.SpawnBlock(p.BlockGreen, env, 44f, 41f, 28, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 132f, 34f, 32, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 248f, 52f, 32, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 348f, 52f, 32, 2, 4);

        // =====================================================================
        // FLOW: PORTALS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 7f, 26f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 406f, 31.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Calibration Note #3: >= 8m Early Lead & >= 6m Runway)
        // =====================================================================
        GameObject cz1Tight = p.Spawn(p.CameraZoneTight, flow, 13f, 22f);
        cz1Tight.name = "CameraZone_Tight_Act1FloodgateLock";
        SetTriggerBoxSize(cz1Tight, new Vector3(26f, 26f, 10f));        // X=0..26, Y=9..35

        GameObject cz1Wide = p.Spawn(p.CameraZoneWide, flow, 66f, 21f);
        cz1Wide.name = "CameraZone_Wide_Act1ViaductArch1";
        SetTriggerBoxSize(cz1Wide, new Vector3(104f, 42f, 10f));        // X=14..118, Y=0..42 (12m early lead!)

        GameObject cz2Tight = p.Spawn(p.CameraZoneTight, flow, 141.5f, 8.5f);
        cz2Tight.name = "CameraZone_Tight_Act2SubPierSluice";
        SetTriggerBoxSize(cz2Tight, new Vector3(63f, 21f, 10f));        // X=110..173 (15.5m past dz2Bulk@157.5, ends before 175!)

        GameObject cz2Wide = p.Spawn(p.CameraZoneWide, flow, 199.5f, 16f);
        cz2Wide.name = "CameraZone_Wide_Act2VaultAndExit";
        SetTriggerBoxSize(cz2Wide, new Vector3(67f, 40f, 10f));         // X=166..233, Y=-4..36 (ends before hold3GlideStart@235!)

        GameObject cz3Climb = p.Spawn(p.CameraZoneClimb, flow, 234f, 26f);
        cz3Climb.name = "CameraZone_Climb_Act3SkyAscent";
        SetTriggerBoxSize(cz3Climb, new Vector3(44f, 52f, 10f));        // X=212..256, Y=0..52 (21m runway past 235!)

        GameObject cz3Wide = p.Spawn(p.CameraZoneWide, flow, 290f, 26f);
        cz3Wide.name = "CameraZone_Wide_Act3TwinArchGallery";
        SetTriggerBoxSize(cz3Wide, new Vector3(104f, 60f, 10f));        // X=238..342, Y=-4..56 (18m overlap with Climb)

        GameObject cz4Wide = p.Spawn(p.CameraZoneWide, flow, 342f, 25f);
        cz4Wide.name = "CameraZone_Wide_Act4PyramidsEntry";
        SetTriggerBoxSize(cz4Wide, new Vector3(48f, 50f, 10f));         // X=318..366, Y=0..50 (30m runway past db4Pyr1@336)

        GameObject cz4Climb = p.Spawn(p.CameraZoneClimb, flow, 384f, 31f);
        cz4Climb.name = "CameraZone_Climb_Act4SummitBlitz";
        SetTriggerBoxSize(cz4Climb, new Vector3(72f, 44f, 10f));        // X=348..420, Y=9..53 (18m overlap at 348..366!)

        // =====================================================================
        // ACT 1: THE FLOODGATE LOCK BREACH (X = 2..108, Y = 2..41)
        // Drop Hatch -> IceSurface + Slope_45_Large -> Over-Arch #1 Vault + Recessed Pier Alcove Atom 2!
        // =====================================================================
        p.SpawnGroundSized(env, 2f, 26f, 16);                             // Floodgate Lock Floor X=2..18, Y=25..26
        SpawnPierToY(p, env, 15f, 26f, 10f, 4);                           // Left Drop Shaft Bulkhead X=15..17, Y=10..25
        p.SpawnCeilingSized(env, 2f, 34f, 26);                            // Floodgate Lock Roof X=2..28, Y=33..34
        p.SpawnBlock(p.BlockGreen, env, 3f, 33f, 30, 2, 4);
        p.Spawn(p.Wall1x12, env, 27f, 33f);                               // East Hood of Drop Hatch X=27..28, Y=21..33
        for (int x = 11; x <= 13; x++)
            for (int y = 27; y <= 31; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Initial Pressurized Purple Door!
        p.Spawn(p.DashCharge, inter, 17f, 27.5f);                         // Starter Hatch Dash Charge!

        // Starter Earth Ground Pound Drop Hatch (X=18..26, Y=26 -> 10)
        for (int x = 18; x <= 24; x += 2)
            SpawnSingleHitCracked4x4(p, inter, x, 26f);                   // Layer 1: Y=24..26 (1-hit starter GP hatch!)
        for (int x = 18; x <= 25; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 21f);                      // Layer 2: Purple cushion Y=20..21
        SetAtomId(p.Spawn(p.Atom, col, 22f, 18.5f), 1);
        for (int x = 18; x <= 25; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 16f);                      // Layer 3: Purple cushion Y=15..16

        // IceSurface + Slope_45_Large Sluice Accelerator (Calibration Note #1: Boost ends 4.0m before ramp!)
        p.SpawnGroundSized(env, 14f, 10f, 12);                            // Bowl entry base X=14..26, Y=9..10
        SpawnPierToBedrock(p, env, 14f, 10f, 4);
        p.Spawn(p.IceSurface, env, 26f, 10f);                             // Ice #1 (26..32)
        p.Spawn(p.IceSurface, env, 32f, 10f);                             // Ice #2 (32..38)
        p.SpawnGroundSized(env, 38f, 10f, 6);                             // Run-up (38..44)
        p.Spawn(p.Slope45Large, env, 46f, 10f);                           // Ramp Crest UP-RIGHT X=44..48, Y=10..14!
        p.SpawnBlock(p.BlockCyan, env, 44f, 10f, 8, 2, 4);                // Base under Slope_45_Large
        SpawnPierToY(p, env, 44f, 10f, 2f, 4);                            // Terminates cleanly on Safety Floor Y=2!

        p.Spawn(p.FireJet, haz, 27f, 10f);                                // Timed sluice vent at bowl entry
        p.Spawn(p.CrumblingBlock, env, 29f, 16.5f);                       // Coupled cleanly above FireJet@27!
        for (int x = 29; x <= 31; x++)
            for (int y = 11; y <= 12; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Ice Carpet A
        GameObject db1Bowl = p.Spawn(p.DashBoostFast, inter, 32.5f, 11.5f);
        ConfigureDashZoneBoost(db1Bowl, new Vector2(3.2f, 3.2f), 34f, 0.24f, 24f); // -> X=40.66 (3.3m before Slope_45_Large@44!)
        for (int x = 35; x <= 38; x++)
            for (int y = 11; y <= 13; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Ice Carpet B
        SetAtomId(p.Spawn(p.Atom, col, 40f, 14.5f), 3);
        p.Spawn(p.DashCharge, inter, 46.5f, 15.5f);                       // Ramp crest air dash (7.0m from db1Bowl end!)

        // Act 1 Continuous Recovery Safety Floor (X=44..92, Y=2) + Recessed Pier Alcove (X=68..88, Y=10)
        p.SpawnGroundSized(env, 44f, 2f, 48);                             // Safety floor X=44..92, Y=1..2
        SpawnPierToBedrock(p, env, 44f, 2f, 4);
        SpawnPierToBedrock(p, env, 68f, 2f, 4);
        SpawnPierToBedrock(p, env, 86f, 2f, 4);
        GameObject mp1Rec = p.Spawn(p.MovingPlatform, env, 50f, 7f);
        ConfigureMovingPlatform(mp1Rec, new Vector3(5f, 0f, 0f), 3.4f, 0.15f); // Clear of piers (44..46 & 68..70) & pads!
        p.Spawn(p.BouncePadMega, inter, 58.5f, 2f);                       // Unobstructed above (58.5 clear of Arch Deck@68!)
        p.Spawn(p.FireJet, haz, 63f, 2f);
        p.Spawn(p.CrumblingBlock, env, 63f, 5.5f);                        // Coupled over recovery FireJet!
        p.SpawnGroundSized(env, 60f, 10f, 28);                            // Alcove Floor Deck X=60..88, Y=9..10
        SpawnPierToY(p, env, 60f, 10f, 2f, 4);                            // Terminates cleanly on Safety Floor Y=2!
        p.Spawn(p.DashLaneUp, inter, 89.5f, 10f);                         // Recovery updraft directly east of Alcove!
        p.Spawn(p.BouncePadMega, inter, 90f, 2f);                         // Offset cleanly outside X=88!

        // Recessed Pier Alcove beneath Viaduct Arch #1 (X=68..88, Y=10..23 -> Secret Atom 2!)
        p.Spawn(p.DashCharge, inter, 65.5f, 11.5f);
        p.Spawn(p.BouncePad, inter, 62f, 10f);                            // Unobstructed left of Arch Deck@68!
        for (int y = 11; y <= 15; y++)
            p.Spawn(p.DashWall, inter, 69f, y);                           // False Yellow Gate into Pier Alcove (69..70)!
        p.Spawn(p.Wall1x12, env, 86f, 22f);                               // Alcove East Back Wall X=86..87, Y=10..22!
        p.SpawnBlock(p.BlockCyan, env, 87f, 22f, 2, 24, 4);               // Thickens pier to X=86..88, Y=10..22!
        for (int x = 72; x <= 76; x++)
            for (int y = 11; y <= 14; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Alcove Purple Stash!
        SetAtomId(p.Spawn(p.Atom, col, 81f, 12.5f), 2);                   // Early Secret Pier Alcove Atom 2!

        // Over-Arch #1 Barrel Chain & Keystone Smash (X=50..94, Y=16 -> 34 -> 22)
        GameObject db1Arch = p.Spawn(p.DashBoostFast, inter, 50f, 16.5f);
        db1Arch.transform.rotation = Quaternion.Euler(0f, 0f, 28f);
        ConfigureDashZoneBoost(db1Arch, new Vector2(3.4f, 3.4f), 34f, 0.34f, 24f); // -> ~(60.2, 21.9)!
        for (int x = 53; x <= 57; x++)
            for (int y = 17; y <= 23; y++)
                if (Mathf.Abs(y - (16.5f + (x - 50f) * 0.532f)) <= 2.0f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Purple Approach Ribbon!

        GameObject hold1Arch = p.Spawn(p.DashHold, inter, 62f, 23f);
        ConfigureDashZoneHold(hold1Arch, 24f, new Vector2(3.4f, 3.4f), 36f, 0.38f, 24f, 0.20f); // -> ~(74.5, 28.6)!
        for (int x = 65; x <= 72; x++)
            for (int y = 25; y <= 32; y++)
                if (Mathf.Abs(y - (23f + (x - 62f) * 0.445f)) <= 2.2f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Sweeping Rainbow Arch Ribbon!
        SetAtomId(p.Spawn(p.Atom, col, 69f, 30.5f), 4);

        // Viaduct Arch #1 Vault Deck (X=68..88, Y=24 -> BouncePadMega@(77,24) -> DashHold@(80, 33.5)!)
        p.SpawnGroundSized(env, 68f, 24f, 20);                            // Arch #1 Crown Deck X=68..88, Y=23..24
        p.SpawnBlock(p.BlockMagenta, env, 69f, 23f, 34, 2, 4);            // Arch Trim cleanly at X=69..86, Y=22..23!
        p.Spawn(p.FireJet, haz, 72.5f, 24f);                              // Timed jet before BouncePadMega (clean >4.5m from pad)
        p.Spawn(p.BouncePadMega, inter, 77f, 24f);                        // Unobstructed up to Ceiling@42 -> PAD_CEIL=0!
        p.Spawn(p.FireJet, haz, 82f, 24f);
        GameObject mp1Arch = p.Spawn(p.MovingPlatform, env, 86f, 25f);
        ConfigureMovingPlatform(mp1Arch, new Vector3(4f, 0f, 0f), 3.4f, 0.15f); // Coupled near FireJet@82, clear below hold1Key!

        GameObject hold1Key = p.Spawn(p.DashHold, inter, 80f, 33.5f);
        ConfigureDashZoneHold(hold1Key, -24f, new Vector2(3.6f, 3.6f), 38f, 0.40f, 24f, 0.20f); // -> ~(93.9, 27.3)!
        for (int x = 83; x <= 84; x++)
            for (int y = 29; y <= 36; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Keystone Purple Front!
        for (int x = 86; x <= 88; x++)
            for (int y = 28; y <= 35; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // Yellow Keystone Gate (86..89)!
        SetAtomId(p.Spawn(p.Atom, col, 90.5f, 28.5f), 5);
        p.Spawn(p.DashCharge, inter, 94f, 25.5f);                         // Catches hold1Key@(93.9, 27.3) cleanly!

        // Sanctuary 1 (X=92..108, Y=22)
        p.SpawnGroundSized(env, 92f, 22f, 16);
        SpawnPierToBedrock(p, env, 92f, 22f, 4);
        SpawnPierToBedrock(p, env, 104f, 22f, 4);
        p.SpawnCeilingSized(env, 92f, 32f, 16);
        p.SpawnBlock(p.BlockGreen, env, 93f, 31f, 28, 2, 4);
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 99f, 23.5f);
        cp1.name = "Checkpoint_1";
        GameObject db1To2 = p.Spawn(p.DashBoostFast, inter, 103.5f, 23.5f);
        ConfigureDashZoneBoost(db1To2, new Vector2(3.0f, 3.0f), 30f, 0.20f, 22f); // -> X=109.5 squarely over Act 2 GP Hatch!

        // =====================================================================
        // ACT 2: THE SUB-PIER UNDERCUT GAUNTLET (X = 108..228, Sluice Y = 0..18)
        // Dive Under -> Rapid Horizontal/Diagonal Sluice Pinball + Pier #2 Vault Diamond 1!
        // =====================================================================
        // Plunge Shaft #1 Behind Pier #1 (X=108..116, Y=22 -> 0)
        for (int x = 108; x <= 114; x += 2)
            SpawnSingleHitCracked4x4(p, inter, x, 22f);                   // Layer 1: Y=20..22
        p.Spawn(p.DashCharge, inter, 112f, 18.2f);                        // Mid-plunge charge (5.9m from db1To2 end, GAP<=8m!)
        for (int x = 108; x <= 115; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 16f);                      // Cushion Y=15..16
        SetAtomId(p.Spawn(p.Atom, col, 112f, 13.6f), 6);                  // Clear pocket Y=12..15 between strata!
        for (int x = 108; x <= 115; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, 8f);                // Layer 3: GP Crystal Strata Y=7..12!

        // Sub-Pier Continuous Sluiceway Floor (X=106..212, Y=0)
        for (int x = 106; x < 212; x += 20)
            p.SpawnGroundSized(env, x, 0f, Mathf.Min(20, 212 - x));
        foreach (float px in new[] { 108f, 134f, 166f, 194f, 208f })
            SpawnPierToBedrock(p, env, px, 0f, 4);

        // Overhead Viaduct Bridge Deck Framing Sub-Pier Ceiling (Y=19, open cleanly at Vault Breach 170..180!)
        for (int x = 116; x < 170; x += 20)
            p.SpawnGroundSized(env, x, 19f, Mathf.Min(20, 170 - x));      // West Bridge Span X=116..170, Y=18..19
        p.SpawnBlock(p.BlockGreen, env, 118f, 18f, 48, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 126f, 34f, 4, 30, 4);              // Massive Viaduct Masonry Buttress A (126..128, 19..34)
        p.SpawnBlock(p.BlockCyan, env, 166f, 34f, 4, 30, 4);              // Pier #2 West Cheek X=166..168, Y=19..34
        // -> Vault Breach X=170..180 (10m wide clear shaft!)
        p.SpawnGroundSized(env, 180f, 19f, 24);                           // East Bridge Span X=180..204, Y=18..19
        p.SpawnBlock(p.BlockGreen, env, 182f, 18f, 40, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 184f, 34f, 4, 14, 4);              // Pier #2 East Upper Header X=184..186, Y=27..34 (leaves Y=19..27 doorway!)

        // Sub-Pier Sluice Barrel Chain (X=118..210)
        GameObject db2Start = p.Spawn(p.DashBoostFast, inter, 119f, 1.5f);
        ConfigureDashZoneBoost(db2Start, new Vector2(3.2f, 3.2f), 34f, 0.28f, 24f); // -> X=128.5 into hold2Ric1!
        for (int x = 122; x <= 124; x++)
            for (int y = 1; y <= 6; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Sluice Purple Gate #1 (ends X=125)!
        GameObject saw2SluiceA = p.Spawn(p.SawSlow, haz, 123.5f, 10.5f);
        ConfigureSaw(saw2SluiceA, new Vector3(0f, 3.5f, 0f), 2.8f);       // Hovering safely above boost beam!

        GameObject mp2A = p.Spawn(p.MovingPlatform, env, 128f, 7.5f);
        ConfigureMovingPlatform(mp2A, new Vector3(3.5f, 0f, 0f), 3.4f, 0.15f); // Clear 126..133.5, Y=7..8 above floor & TouchWall!
        GameObject hold2Ric1 = p.Spawn(p.DashHold, inter, 132f, 3f);
        ConfigureDashZoneHold(hold2Ric1, 28f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 24f, 0.20f); // -> ~(142.8, 8.8)!
        for (int x = 135; x <= 140; x++)
            for (int y = 3; y <= 10; y++)
                if (Mathf.Abs(y - (3f + (x - 132f) * 0.532f)) <= 2.1f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Diagonal Purple Ribbon!
        SetAtomId(p.Spawn(p.Atom, col, 138f, 9f), 7);

        GameObject hold2Ric2 = p.Spawn(p.DashHold, inter, 144f, 9.5f);
        ConfigureDashZoneHold(hold2Ric2, -24f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 24f, 0.20f); // -> ~(155.2, 4.5)!
        for (int y = 4; y <= 10; y++)
            p.Spawn(p.DashWallCracked, inter, 149.5f, y - 0.5f);          // Yellow Sluice Lock #1 (149..150)!
        GameObject stomp2A = p.Spawn(p.StompBlock, haz, 147f, 13.5f);
        ConfigureStompBlock(stomp2A, 0.25f);
        p.Spawn(p.CrumblingBlock, env, 150f, 2.5f);                       // Coupled skip ledge safely below dive cone!

        GameObject dz2Bulk = p.Spawn(p.DashZone, inter, 157.5f, 2.5f);
        ConfigureDashZoneBoost(dz2Bulk, new Vector2(3.4f, 3.4f), 38f, 0.42f, 24f); // -> X=173.5 (175-157.5=17.5m<=18m)!
        for (int x = 160; x <= 162; x++)
            for (int y = 1; y <= 8; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Sluice Thicket #2!
        for (int y = 1; y <= 8; y++)
            p.Spawn(p.DashWallChainRow1, inter, 164f, y);                 // Yellow Sluice Bulkhead #2 (164..169)!
        GameObject stomp2B = p.Spawn(p.StompBlock, haz, 161f, 13.5f);
        ConfigureStompBlock(stomp2B, 0.50f);
        p.Spawn(p.LavaLeaper, haz, 170f, 0f);
        p.Spawn(p.CrumblingBlock, env, 170f, 5.5f);                       // Coupled over LavaLeaper & clear of Vault UP!
        SetAtomId(p.Spawn(p.Atom, col, 170f, 8.5f), 8);

        // --- High-Risk Secret Diamond 1 Vault inside Pier #2 (X=168..186, Y=19..32) ---
        p.Spawn(p.BouncePad, inter, 175f, 0f);                            // Unobstructed right up into Open Breach 170..180!
        p.Spawn(p.DashLaneUp, inter, 175f, 9f);
        GameObject hold2VaultUp = p.Spawn(p.DashHold, inter, 175f, 15.5f);
        ConfigureDashZoneHold(hold2VaultUp, 86f, new Vector2(3.4f, 3.0f), 34f, 0.30f, 22f, 0.08f); // -> ~(175.7, 25.7)!
        for (int x = 172; x <= 177; x++)
            for (int y = 20; y <= 21; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // Yellow Vault Ceiling Seal Y=19..21!
        p.Spawn(p.OneWayPlatformLarge, env, 171f, 22f);                   // Catch flush at Y=22!
        for (int x = 170; x <= 172; x++)
            for (int y = 23; y <= 26; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Vault Purple Stash!
        SetAtomId(p.Spawn(p.Atom, col, 172f, 27.5f), 9);
        SetDiamondId(p.Spawn(p.Diamond, col, 177f, 26f), 1);              // Pier #2 Secret Vault Diamond 1!
        GameObject saw2Vault = p.Spawn(p.SawSlow, haz, 181f, 28.5f);
        ConfigureSaw(saw2Vault, new Vector3(0f, -3.5f, 0f), 2.8f);        // Patrols Y=25..28.5 clear above Vault Floor@22!
        p.Spawn(p.CrumblingBlock, env, 180.5f, 21.5f);                     // Coupled near Saw2Vault & flush below db2VaultOut!
        GameObject db2VaultOut = p.Spawn(p.DashBoostFast, inter, 182.5f, 23.5f);
        db2VaultOut.transform.rotation = Quaternion.Euler(0f, 0f, -12f);
        ConfigureDashZoneBoost(db2VaultOut, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f); // -> ~(195.2, 20.8) clearing Pier Cheek@184 cleanly overhead onto Act 2 East!

        // Lower Sluice Finale (180..212, Y=0..18)
        p.Spawn(p.BouncePadMini, inter, 182f, 0f);
        p.Spawn(p.CrumblingBlock, env, 186f, 3.5f);                       // Clear of Mini Pad@182 & MP@190!
        GameObject mp2B = p.Spawn(p.MovingPlatform, env, 188f, 6.5f);
        ConfigureMovingPlatform(mp2B, new Vector3(5f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.LavaLeaper, haz, 190f, 0f);
        p.Spawn(p.DashCharge, inter, 194f, 15.5f);                        // Catches db2VaultOut@(195.2, 20.8) cleanly (5.4m)!
        GameObject saw2End = p.Spawn(p.SawSlow, haz, 195f, 1.5f);
        ConfigureSaw(saw2End, new Vector3(0f, 4.5f, 0f), 2.6f);           // Clear 4m left of db2End@199!
        GameObject db2End = p.Spawn(p.DashBoostFast, inter, 199f, 2.5f);
        ConfigureDashZoneBoost(db2End, new Vector2(3.2f, 3.2f), 36f, 0.36f, 24f); // -> X=212.0 onto Sanct 2!
        for (int y = 1; y <= 8; y++)
            p.Spawn(p.DashWallChainRow1, inter, 203f, y);                 // Yellow Sluice Exit Bulkhead (203..208)!
        SetAtomId(p.Spawn(p.Atom, col, 210f, 5f), 10);

        // Sanctuary 2 (X=212..228, Y=4)
        p.SpawnGroundSized(env, 212f, 4f, 16);
        SpawnPierToBedrock(p, env, 212f, 4f, 4);
        SpawnPierToBedrock(p, env, 224f, 4f, 4);
        p.SpawnCeilingSized(env, 212f, 16f, 10);                          // Sanct 2 Hood 212..222, clear of Ascent@227!
        p.SpawnBlock(p.BlockMagenta, env, 213f, 15f, 16, 2, 4);
        p.Spawn(p.DashCharge, inter, 214f, 6f);                           // Closes Sluice Exit -> Sanct 2 handoff!
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 218f, 5.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ACT 3: THE TWIN-ARCH SKY SLALOM (X = 228..336, Y = 0..52)
        // Climb -> Double-Arch Sky Gallery (Under Truss A + Over Pier #3 + Upper Express Line!)
        // =====================================================================
        // Terraced Safety Recovery Floors (Y=0 & Y=10) + Open Recovery Launchers (PAD_CEIL = 0!)
        for (int x = 228; x < 308; x += 20)
            p.SpawnGroundSized(env, x, 0f, Mathf.Min(20, 308 - x));       // West Gallery Basin X=228..308, Y=-1..0
        SpawnPierToBedrock(p, env, 236f, 0f, 4);
        SpawnPierToBedrock(p, env, 266f, 0f, 4);
        SpawnPierToBedrock(p, env, 296f, 0f, 4);
        p.SpawnGroundSized(env, 308f, 10f, 14);                           // East Gallery Step X=308..322, Y=9..10
        p.SpawnBlock(p.BlockCyan, env, 308f, 9f, 4, 50, 4);               // Step closing -16..9

        p.Spawn(p.BouncePadMega, inter, 238f, 0f);                        // Clean unobstructed 0..9.3m!
        p.Spawn(p.LavaLeaper, haz, 246f, 0f);
        p.Spawn(p.CrumblingBlock, env, 246f, 4.5f);                       // Coupled cleanly over LavaLeaper@246!
        p.Spawn(p.BouncePadMega, inter, 254f, 0f);
        p.Spawn(p.DashLaneUp, inter, 254f, 9f);
        p.Spawn(p.LavaLeaper, haz, 262f, 0f);
        p.Spawn(p.CrumblingBlock, env, 262f, 4.5f);                       // Coupled cleanly over LavaLeaper@262!
        p.Spawn(p.BouncePadMega, inter, 270f, 0f);
        p.Spawn(p.BouncePadMega, inter, 298f, 0f);                        // Offset cleanly past Pier#3@(278..292)!
        p.Spawn(p.DashLaneUp, inter, 298f, 9f);
        p.Spawn(p.BouncePadMega, inter, 314f, 10f);

        // Eruption Climb out of Sanctuary 2 (X=227..248)
        p.Spawn(p.BouncePadMega, inter, 226f, 4f);
        p.Spawn(p.DashLaneUp, inter, 228f, 12.5f);
        GameObject hold3Climb = p.Spawn(p.DashHold, inter, 228f, 21f);
        ConfigureDashZoneHold(hold3Climb, 74f, new Vector2(3.6f, 3.6f), 36f, 0.36f, 24f, 0.14f); // -> ~(231.6, 33.5)!
        for (int x = 226; x <= 231; x++)
            for (int y = 26; y <= 28; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Sky Gallery Purple Halo!
        p.Spawn(p.OneWayPlatform, env, 230f, 31f);

        // Twin-Arch Sky Gallery (Arch Truss A @ 250..270, Y=38..40; Pier #3 @ 278..292, Y=0..18)
        p.SpawnBlock(p.BlockCyan, env, 250f, 40f, 40, 4, 4);              // Arch Truss A Baffle: X=250..270, Y=38..40!
        p.SpawnBlock(p.BlockGreen, env, 252f, 38f, 32, 2, 4);             // Neon Rib under Truss A: X=252..268, Y=37..38!
        p.SpawnGroundSized(env, 278f, 18f, 14);                           // Pier #3 Crown Deck X=278..292, Y=17..18
        SpawnPierToY(p, env, 278f, 17f, 0f, 4);                           // Terminates cleanly on Basin Floor Y=0!
        SpawnPierToY(p, env, 288f, 17f, 0f, 4);
        p.Spawn(p.BouncePadMega, inter, 283f, 18f);                       // Over Pier #3: Y=18..27.3 << Roof@52!
        p.Spawn(p.Slope45, env, 290f, 18f);                               // Wall Kicker X=289..291, Y=18..20!

        // Main Under-Truss & Over-Pier Sweeping Glide Slalom (X=235..318, Y=20..36)
        GameObject hold3GlideStart = p.Spawn(p.DashHold, inter, 235f, 33.5f);
        ConfigureDashZoneHold(hold3GlideStart, -14f, new Vector2(3.6f, 3.6f), 36f, 0.36f, 24f, 0.22f); // -> ~(247.6, 30.4) under Truss A!
        p.Spawn(p.DashCharge, inter, 248f, 30.5f);                        // Closes hold3GlideStart -> Curtain #1 handoff!
        for (int x = 250; x <= 253; x++)
            for (int y = 22; y <= 36; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Hanging Curtain #1 beneath Truss A (60 blocks)!
        p.Spawn(p.DashCharge, inter, 256f, 27f);
        p.Spawn(p.CrumblingBlock, env, 258f, 21f);
        GameObject mp3Gallery = p.Spawn(p.MovingPlatform, env, 260f, 17.5f);
        ConfigureMovingPlatform(mp3Gallery, new Vector3(6f, 0f, 0f), 3.4f, 0.15f); // Clear of BouncePad@270 (17.5 > 9.3!) & Pier@278!
        GameObject spin3Mid = p.Spawn(p.SpinArm, haz, 261f, 27.5f);
        ConfigureSpinArm(spin3Mid, 95f);

        for (int x = 270; x <= 276; x++)
            for (int y = 24; y <= 32; y++)
                if (Mathf.Abs(x - 273f) + Mathf.Abs(y - 28f) <= 3.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Checkerboard Diamond Cloud!
        SetAtomId(p.Spawn(p.Atom, col, 273f, 28f), 11);
        GameObject can3Upper = p.Spawn(p.CannonFast, haz, 273f, 38.5f);
        ConfigureCannon(can3Upper, new Vector3(0f, 0f, -135f), 11f, 2.2f, 22f);
        p.Spawn(p.CrumblingBlock, env, 277.5f, 35f);                      // Coupled cleanly near CannonUpper!
        GameObject can3Lower = p.Spawn(p.CannonFast, haz, 294f, 13f);
        ConfigureCannon(can3Lower, new Vector3(0f, 0f, 140f), 11f, 2.3f, 22f);
        p.Spawn(p.DashCharge, inter, 284f, 28.5f);
        p.Spawn(p.CrumblingBlock, env, 293f, 19f);

        // Curtain #2 + Gallery Exit Bulkhead -> Sanctuary 3 (318..336, Y=16)
        for (int x = 295; x <= 297; x++)
            for (int y = 20; y <= 34; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Hanging Curtain #2 (45 Purple blocks)!
        p.Spawn(p.BouncePadMini, inter, 299f, 12f);                       // Auxiliary catch below Curtain #2!
        p.SpawnGroundSized(env, 297f, 12f, 5);                            // Catch deck 297..302, Y=11..12 (clean above 9.3m!)
        GameObject hold3End = p.Spawn(p.DashHold, inter, 302f, 21f);
        ConfigureDashZoneHold(hold3End, -4f, new Vector2(3.6f, 3.6f), 38f, 0.42f, 24f, 0.20f); // -> ~(317.9, 19.9) onto Sanct 3!
        for (int y = 17; y <= 26; y++)
            p.Spawn(p.DashWallChainRow1, inter, 307f, y);                 // Yellow Twin-Arch Exit Gate (307..312)!

        // --- Upper Ceiling-Rafter Barrel Express Line (Y=40..50 -> Bonus Atoms 12, 13 & Diamond 2!) ---
        p.Spawn(p.OneWayPlatform, env, 241f, 37f);
        p.Spawn(p.BouncePad, inter, 243f, 37f);                           // X=243 cleanly west of Truss A@250 -> PAD_CEIL=0!
        GameObject hold3Raf1 = p.Spawn(p.DashHold, inter, 245f, 43.5f);
        ConfigureDashZoneHold(hold3Raf1, 4f, new Vector2(3.4f, 3.4f), 36f, 0.36f, 22f, 0.20f); // -> ~(257.9, 44.4) above Truss A!
        for (int x = 250; x <= 254; x++)
            for (int y = 42; y <= 47; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Upper Rafter Purple Cloud #1!
        SetAtomId(p.Spawn(p.Atom, col, 256.5f, 45f), 12);                 // Bonus Upper Rafter Atom 12!
        p.Spawn(p.CrumblingBlock, env, 258f, 41.5f);                      // Clean above Truss@40!
        GameObject mp3Raf = p.Spawn(p.MovingPlatform, env, 261f, 42.5f);
        ConfigureMovingPlatform(mp3Raf, new Vector3(4f, 0f, 0f), 3.4f, 0.15f); // Clear 259..267, Y=42..43 above Truss!
        p.Spawn(p.DashCharge, inter, 263f, 45f);                          // Closes hold3Raf1 -> hold3Raf2!

        GameObject hold3Raf2 = p.Spawn(p.DashHold, inter, 270f, 44f);
        ConfigureDashZoneHold(hold3Raf2, 2f, new Vector2(3.4f, 3.4f), 38f, 0.44f, 22f, 0.20f); // -> ~(286.7, 44.6)!
        for (int x = 275; x <= 278; x++)
            for (int y = 42; y <= 47; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Upper Rafter Purple Cloud #2!
        SetAtomId(p.Spawn(p.Atom, col, 281f, 45f), 13);                   // Bonus Upper Rafter Atom 13!
        p.Spawn(p.DashCharge, inter, 287f, 45f);

        GameObject hold3Raf3 = p.Spawn(p.DashHold, inter, 291f, 44.5f);
        ConfigureDashZoneHold(hold3Raf3, -4f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 22f, 0.20f); // -> ~(306.1, 43.4)!
        for (int y = 42; y <= 47; y++)
            p.Spawn(p.DashWallCracked, inter, 297.5f, y - 0.5f);          // Yellow Rafter Lock!
        p.Spawn(p.OneWayPlatformLarge, env, 298f, 41f);
        SetDiamondId(p.Spawn(p.Diamond, col, 304f, 44.5f), 2);            // Upper Rafter Diamond 2!
        GameObject hold3RafDive = p.Spawn(p.DashHold, inter, 308f, 43f);
        ConfigureDashZoneHold(hold3RafDive, -52f, new Vector2(3.4f, 3.4f), 38f, 0.48f, 24f, 0.18f); // -> ~(319.2, 28.6)!
        p.Spawn(p.OneWayPlatform, env, 317f, 23f);                        // Soft mid-catch above Sanctuary 3!
        p.Spawn(p.DashCharge, inter, 318f, 25f);

        // Sanctuary 3 (X=318..336, Y=16)
        p.SpawnGroundSized(env, 318f, 16f, 18);
        SpawnPierToBedrock(p, env, 322f, 16f, 4);
        SpawnPierToBedrock(p, env, 332f, 16f, 4);
        p.SpawnCeilingSized(env, 320f, 27f, 14);
        p.SpawnBlock(p.BlockGreen, env, 321f, 26f, 24, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 327f, 17.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ACT 4: THE SHATTER-PYRAMID STAIRWAY (X = 336..412, Y = 8..52)
        // Escalating 3 Stepped Shatter-Pyramids (Purple + Yellow Bastions) -> End_Portal@(406, 31.8)
        // =====================================================================
        // Continuous Terraced Recovery Floor (Y=8, X=334..380)
        p.SpawnGroundSized(env, 334f, 8f, 20);
        p.SpawnGroundSized(env, 354f, 8f, 20);
        p.SpawnGroundSized(env, 374f, 8f, 6);                             // Stops cleanly at X=380 before Summit Pier@380!
        SpawnPierToBedrock(p, env, 344f, 8f, 4);
        SpawnPierToBedrock(p, env, 366f, 8f, 4);
        p.Spawn(p.BouncePadMega, inter, 340.0f, 8f);                      // Centered in 4m open chute (338..342) -> PAD_CEIL=0, XGap<=15.5m!
        p.Spawn(p.FireJet, haz, 362f, 8f);
        p.Spawn(p.CrumblingBlock, env, 362f, 11.5f);                      // Coupled over FireJet!
        p.Spawn(p.BouncePadMega, inter, 368f, 8f);
        p.Spawn(p.DashLaneUp, inter, 376f, 18f);

        // Pyramid #1 Base Deck (X=342..358, Y=16) -> Link 1: db4Pyr1 (+20 deg) -> Stepped Pyramid 1!
        p.SpawnGroundSized(env, 342f, 16f, 16);
        SpawnPierToY(p, env, 342f, 16f, 8f, 4);                           // Terminates cleanly on Recovery Y=8!
        SpawnPierToY(p, env, 354f, 16f, 8f, 4);
        GameObject db4Pyr1 = p.Spawn(p.DashBoostFast, inter, 336f, 17.5f);
        db4Pyr1.transform.rotation = Quaternion.Euler(0f, 0f, 20f);
        ConfigureDashZoneBoost(db4Pyr1, new Vector2(3.4f, 3.4f), 38f, 0.44f, 24f); // -> ~(351.7, 23.2)!
        // Stepped Pyramid #1 (Purple front 342..345 + Yellow DashWall_ChainRow 1 tapered stairs 347..352)
        for (int x = 342; x <= 345; x++)
        {
            int topY = 23 + (x - 342);
            for (int y = 17; y <= topY; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Stepped Purple Front!
        }
        for (int y = 17; y <= 24; y++)
            p.Spawn(p.DashWallChainRow1, inter, 347f, y);                 // Yellow Stair Core (347..352)!
        SetAtomId(p.Spawn(p.Atom, col, 353f, 24.5f), 14);
        p.Spawn(p.BouncePadMega, inter, 355.5f, 16f);                     // Clear right of Pyramid 1!

        // Link 2 & 3: BouncePadMega@(355.5, 16) -> DashLane_UP@(357, 23.5) -> hold4Pyr2@(357, 30) (+10 deg) -> Stepped Pyramid 2 -> Diamond 3!
        p.Spawn(p.DashLaneUp, inter, 357f, 23.5f);
        GameObject hold4Pyr2 = p.Spawn(p.DashHold, inter, 357f, 30f);
        ConfigureDashZoneHold(hold4Pyr2, 10f, new Vector2(3.6f, 3.6f), 38f, 0.44f, 24f, 0.20f); // -> ~(373.5, 32.9), exits ~Y=31.4!
        for (int x = 361; x <= 365; x++)
        {
            int topY = 33 + (x - 361);
            for (int y = 26; y <= topY; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Stepped Pyramid #2 Purple Front (45 blocks)!
        }
        for (int x = 367; x <= 369; x++)
        {
            int topY = 35 - (x - 367);
            for (int y = 26; y <= topY; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // Yellow Stepped Core (27 blocks)!
        }
        SetDiamondId(p.Spawn(p.Diamond, col, 373f, 31.4f), 3);            // Summit Pyramid Diamond 3 on exact barrel exit vector!
        SetAtomId(p.Spawn(p.Atom, col, 375f, 32.0f), 15);

        // Coupled MovingPlatform + SpinArm + Cannon guarding middle stairway
        GameObject mp4Step = p.Spawn(p.MovingPlatform, env, 363f, 19f);
        ConfigureMovingPlatform(mp4Step, new Vector3(5f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.CrumblingBlock, env, 360f, 21f);
        GameObject spin4Mid = p.Spawn(p.SpinArm, haz, 362f, 23f);
        ConfigureSpinArm(spin4Mid, -90f);
        GameObject can4Summit = p.Spawn(p.CannonFast, haz, 381f, 44f);
        ConfigureCannon(can4Summit, new Vector3(0f, 0f, -135f), 11f, 2.2f, 22f);
        p.Spawn(p.CrumblingBlock, env, 377f, 38f);                        // Coupled near Cannon!

        // Summit Terminus Deck (X=380..412, Y=30) -> Pyramid #3 Containment Bastion -> End_Portal@(406, 31.8)
        for (int x = 380; x < 412; x += 20)
            p.SpawnGroundSized(env, x, 30f, Mathf.Min(20, 412 - x));      // Summit Floor X=380..412, Y=29..30
        SpawnPierToBedrock(p, env, 380f, 30f, 4);                         // Terminus West Wall closing 380..382 down to -16!
        SpawnPierToBedrock(p, env, 402f, 30f, 4);
        p.SpawnCeilingSized(env, 402f, 42f, 10);                          // Summit Crown Hood 402..412, Y=41..42
        p.SpawnBlock(p.BlockCyan, env, 403f, 41f, 16, 2, 4);

        GameObject hold4Pre = p.Spawn(p.DashHold, inter, 377f, 32.5f);
        ConfigureDashZoneHold(hold4Pre, -6f, new Vector2(3.6f, 3.6f), 36f, 0.16f, 24f, 0.18f); // -> ~(382.7, 31.9)!

        GameObject dz4Summit = p.Spawn(p.DashZone, inter, 383f, 32.5f);
        ConfigureDashZoneBoost(dz4Summit, new Vector2(3.8f, 5.2f), 40f, 0.64f, 26f); // -> 408.6 nominal (< East Wall@412)!
        for (int x = 386; x <= 389; x++)
        {
            int topY = 39 - (x - 386);
            for (int y = 31; y <= topY; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Stepped Pyramid #3 Purple Stepfront!
        }
        for (int y = 31; y <= 38; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 391f, y);                 // Yellow Containment Shield (391..396)!
            p.Spawn(p.DashWallCracked, inter, 396.5f, y - 0.5f);          // Triple-Thick Summit Layer 6!
            if (y <= 37) p.Spawn(p.DashWallCracked, inter, 397.5f, y - 0.5f); // Tapered Pyramid Summit Layer 7!
            if (y <= 36) p.Spawn(p.DashWallCracked, inter, 398.5f, y - 0.5f); // Layer 8!
        }
        p.Spawn(p.DashCharge, inter, 401f, 32.5f);                        // Finale post-bastion DashCharge (7.6m from 408.6, 18.0m from 383)!
        SetAtomId(p.Spawn(p.Atom, col, 403f, 32f), 16);

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
        return $"Level_52 built at {TargetScenePath} (Validator errors: {errors})";
    }

    private static void SpawnSingleHitCracked4x4(PrefabLib p, Transform parent, float x, float y)
    {
        GameObject go = p.Spawn(p.CrackedBlocks4x4, parent, x, y);
        var d = go.GetComponent<DestructibleObject>();
        if (d != null)
        {
            d.HitsToBreak = 1;
            d.PreCracked = 2;
            EditorUtility.SetDirty(d);
            PrefabUtility.RecordPrefabInstancePropertyModifications(d);
        }
    }

    private static void SpawnPierToBedrock(PrefabLib p, Transform env, float x, float floorTopY, int widthCellsX)
    {
        SpawnPierToY(p, env, x, floorTopY, -16f, widthCellsX);
    }

    private static void SpawnPierToY(PrefabLib p, Transform env, float x, float topFloorY, float bottomStopY, int widthCellsX)
    {
        float pierTop = topFloorY - 1f;
        float h = pierTop - bottomStopY;
        if (h > 0.4f)
            p.SpawnBlock(p.BlockCyan, env, x, pierTop, widthCellsX, Mathf.RoundToInt(h * 2f), 4);
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
            lm.ThreeStarTime = 85f;
            lm.TwoStarTime = 130f;
            lm.OneStarTime = 215f;
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
        public GameObject Wall1x12 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Wall_1x12.prefab");
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
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
