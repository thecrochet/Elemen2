#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level51Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_51.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 51 (Ricochet Reactor)")]
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
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES (X = 0..416, Y = -16..52)
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 208f, -24f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(500f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(500f, 4f, 1f);

        // Continuous Bedrock Base at Y = -16 (X = -8 to 424)
        for (int x = -8; x < 424; x += 20)
            p.SpawnGroundSized(env, x, -16f, Mathf.Min(20, 424 - x));

        // Enclosing West & East Reactor Walls + Fortified Wall_1x12 Piers + Stepped Dead Cells Rooflines
        p.SpawnBlock(p.BlockCyan, env, 0f, 34f, 4, 100, 4);                // West Wall X=0..2, Y=-16..34
        p.Spawn(p.Wall1x12, env, 2f, 19f);                                // Fortified West Intake Wall X=2..3, Y=7..19
        p.SpawnBlock(p.BlockCyan, env, 414f, 41f, 4, 114, 4);             // East Wall X=414..416, Y=-16..41
        p.Spawn(p.Wall1x12, env, 413f, 38f);                              // Fortified East Core Wall X=413..414, Y=26..38

        for (int x = 0; x < 112; x += 20)
            p.SpawnCeilingSized(env, x, 35f, Mathf.Min(20, 112 - x));     // Act 1 Cryo Intake Roof X=0..112, Y=34..35
        p.SpawnBlock(p.BlockCyan, env, 110f, 36f, 4, 2, 4);               // Roof Step 35->36 X=110..112
        for (int x = 112; x < 218; x += 20)
            p.SpawnCeilingSized(env, x, 36f, Mathf.Min(20, 218 - x));     // Act 2 Twin-Conduit Roof X=112..218, Y=35..36
        p.SpawnBlock(p.BlockCyan, env, 216f, 51f, 4, 30, 4);              // Step Up into Turbine Dome X=216..218, Y=36..51
        for (int x = 218; x < 322; x += 20)
            p.SpawnCeilingSized(env, x, 52f, Mathf.Min(20, 322 - x));     // Act 3 Turbine Dome Roof X=218..322, Y=51..52
        p.SpawnBlock(p.BlockCyan, env, 320f, 51f, 4, 18, 4);              // Step Down to Overdrive Core X=320..322, Y=42..51
        for (int x = 322; x < 416; x += 20)
            p.SpawnCeilingSized(env, x, 42f, Mathf.Min(20, 416 - x));     // Act 4 Overdrive Core Roof X=322..416, Y=41..42

        // Neon Reactor Trim Ribs (Flush below ceilings, zero overlap)
        p.SpawnBlock(p.BlockGreen, env, 50f, 34f, 24, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 134f, 35f, 24, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 246f, 51f, 28, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 344f, 41f, 28, 2, 4);

        // =====================================================================
        // FLOW: PORTALS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 7f, 20f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 408f, 15.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Calibration Note #3: 8m+ Early Lead & >= 6m Runway)
        // =====================================================================
        GameObject cz1Tight = p.Spawn(p.CameraZoneTight, flow, 12f, 22f);
        cz1Tight.name = "CameraZone_Tight_Act1IntakeChamber";
        SetTriggerBoxSize(cz1Tight, new Vector3(24f, 18f, 10f));        // X=0..24, Y=13..31

        GameObject cz1Wide = p.Spawn(p.CameraZoneWide, flow, 68f, 18f);
        cz1Wide.name = "CameraZone_Wide_Act1CryoBowl";
        SetTriggerBoxSize(cz1Wide, new Vector3(104f, 36f, 10f));        // X=16..120, Y=0..36 (12.5m past db1To2@107.5)

        GameObject cz2WideW = p.Spawn(p.CameraZoneWide, flow, 143.75f, 19f);
        cz2WideW.name = "CameraZone_Wide_Act2CorkscrewWest";
        SetTriggerBoxSize(cz2WideW, new Vector3(75.5f, 38f, 10f));      // X=106..181.5 (6.5m past db2Low1@175, before dz2Low2@183)

        GameObject cz2Tight = p.Spawn(p.CameraZoneTight, flow, 189f, 10f);
        cz2Tight.name = "CameraZone_Tight_Act2LowerBurst";
        SetTriggerBoxSize(cz2Tight, new Vector3(34f, 16f, 10f));        // X=172..206, Y=2..18

        GameObject cz2WideE = p.Spawn(p.CameraZoneWide, flow, 214f, 19f);
        cz2WideE.name = "CameraZone_Wide_Act2EastClimb";
        SetTriggerBoxSize(cz2WideE, new Vector3(28f, 38f, 10f));        // X=200..228, Y=0..38 (ends before db2To3@229)

        GameObject cz3Climb = p.Spawn(p.CameraZoneClimb, flow, 237f, 24f);
        cz3Climb.name = "CameraZone_Climb_Act3TurbineEntry";
        SetTriggerBoxSize(cz3Climb, new Vector3(38f, 52f, 10f));        // X=218..256, Y=-2..50 (27m & 21m runway!)

        GameObject cz3Wide = p.Spawn(p.CameraZoneWide, flow, 284f, 24f);
        cz3Wide.name = "CameraZone_Wide_Act3PinballDome";
        SetTriggerBoxSize(cz3Wide, new Vector3(96f, 62f, 10f));         // X=236..332, Y=-7..55

        GameObject cz4Wide = p.Spawn(p.CameraZoneWide, flow, 370f, 22f);
        cz4Wide.name = "CameraZone_Wide_Act4OverdriveCore";
        SetTriggerBoxSize(cz4Wide, new Vector3(104f, 52f, 10f));        // X=318..422, Y=-4..48

        // =====================================================================
        // ACT 1: THE CRYO-SLINGSHOT INTAKE (X = 2..112, Y = 2..34)
        // Steep Slope_45_Left -> IceSurface Acceleration Bowl -> Slope_45_Large -> Multi-Angle Barrels!
        // =====================================================================
        p.SpawnGroundSized(env, 2f, 20f, 16);                             // Spawn Deck X=2..18, Y=19..20
        SpawnPierToY(p, env, 2f, 8f, -16f, 4);                            // Flush below Wall_1x12(7..19): top 7 -> -16!
        SpawnPierToBedrock(p, env, 14f, 20f, 4);
        p.SpawnCeilingSized(env, 2f, 28f, 16);                            // Pressurized Spawn Hood Y=27..28
        p.SpawnBlock(p.BlockGreen, env, 4f, 27f, 24, 2, 4);

        for (int x = 12; x <= 14; x++)
            for (int y = 21; y <= 25; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Initial Purple Smash Door!
        SetAtomId(p.Spawn(p.Atom, col, 16.5f, 21.5f), 1);
        p.Spawn(p.BouncePadMini, inter, 17f, 20f);                        // Optional springboard up to Secret Vault!

        // Early Secret Upper Intake Chamber (X=26..42, Y=24..31 -> Atom 2 behind False Yellow DashWall!)
        p.Spawn(p.OneWayPlatform, env, 19f, 23f);                         // Entry OneWay X=19..23
        p.Spawn(p.DashCharge, inter, 24.5f, 25f);                         // Mid-air Dash straight RIGHT into False Wall!
        p.SpawnGroundSized(env, 26f, 24f, 16);                            // Upper Secret Floor X=26..42, Y=23..24
        p.SpawnCeilingSized(env, 26f, 31f, 16);                           // Upper Secret Roof X=26..42, Y=30..31
        for (int y = 25; y <= 29; y++)
            p.Spawn(p.DashWall, inter, 27f, y);                           // False Yellow Upper Intake Gate (X=27..28, Y=24..29)
        for (int x = 30; x <= 33; x++)
            for (int y = 25; y <= 28; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Secret Purple Stash!
        SetAtomId(p.Spawn(p.Atom, col, 35.5f, 26f), 2);                   // Early Secret Atom 2!

        // Steep Slope_45_Left Descent Chute (X=18..26, Y=20 -> 12) + Solid Masonry Backing
        p.Spawn(p.Slope45Left, env, 19f, 18f);                            // X=18..20, Y=18..20
        p.SpawnBlock(p.BlockCyan, env, 18f, 18f, 4, 14, 4);               // X=18..20, Y=11..18
        p.Spawn(p.Slope45Left, env, 21f, 16f);                            // X=20..22, Y=16..18
        p.SpawnBlock(p.BlockCyan, env, 20f, 16f, 4, 10, 4);               // X=20..22, Y=11..16
        p.Spawn(p.Slope45Left, env, 23f, 14f);                            // X=22..24, Y=14..16
        p.SpawnBlock(p.BlockCyan, env, 22f, 14f, 4, 6, 4);                // X=22..24, Y=11..14
        p.Spawn(p.Slope45Left, env, 25f, 12f);                            // X=24..26, Y=12..14
        p.SpawnBlock(p.BlockCyan, env, 24f, 12f, 4, 2, 4);                // X=24..26, Y=11..12

        // Cryo-Ice Acceleration Bowl -> Slope_45_Large (Calibration Note #1: Boost ends 2.4m before ramp!)
        p.SpawnGroundSized(env, 26f, 12f, 2);                             // 26..28
        p.Spawn(p.IceSurface, env, 28f, 12f);                             // Ice #1 (28..34)
        p.Spawn(p.IceSurface, env, 34f, 12f);                             // Ice #2 (34..40)
        p.SpawnGroundSized(env, 40f, 12f, 2);                             // 40..42
        p.Spawn(p.Slope45Large, env, 44f, 12f);                           // Ramp Crest UP-RIGHT X=42..46, Y=12..16!
        p.SpawnBlock(p.BlockCyan, env, 42f, 12f, 8, 2, 4);                // Solid base under Slope45Large Y=11..12
        SpawnPierToBedrock(p, env, 26f, 12f, 4);
        SpawnPierToBedrock(p, env, 42f, 12f, 4);

        p.Spawn(p.FireJet, haz, 28.5f, 12f);                              // Timed cryo vent #1 at bowl entry
        for (int x = 30; x <= 32; x++)
            for (int y = 13; y <= 14; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Ice Carpet A
        GameObject db1Bowl = p.Spawn(p.DashBoostFast, inter, 33.5f, 13.5f);
        ConfigureDashZoneBoost(db1Bowl, new Vector2(3.2f, 3.2f), 34f, 0.23f, 24f); // -> X=41.32 (< 42, punches Purple carpet onto Slope45Large!)
        for (int x = 36; x <= 39; x++)
            for (int y = 13; y <= 15; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Ice Carpet B
        SetAtomId(p.Spawn(p.Atom, col, 38f, 16.5f), 3);
        p.Spawn(p.DashCharge, inter, 45f, 17f);                           // Ramp crest charge -> closes 40..49 gap cleanly!

        // Act 1 Multi-Angle Barrel Chain (db1_Launch +24deg -> hold1_Apex -30deg -> BouncePadMega -> db1_Blast -> Sanctuary 1)
        GameObject db1Launch = p.Spawn(p.DashBoostFast, inter, 49f, 18.5f);
        db1Launch.transform.rotation = Quaternion.Euler(0f, 0f, 24f);
        ConfigureDashZoneBoost(db1Launch, new Vector2(3.4f, 3.4f), 34f, 0.28f, 24f); // -> ~(57.7, 22.4)!
        for (int x = 51; x <= 56; x++)
            for (int y = 18; y <= 24; y++)
                if (Mathf.Abs(y - (18.5f + (x - 49f) * 0.445f)) <= 2.1f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Purple Diagonal Arch #1
        SetAtomId(p.Spawn(p.Atom, col, 55f, 22f), 4);

        GameObject hold1Apex = p.Spawn(p.DashHold, inter, 59f, 23f);
        ConfigureDashZoneHold(hold1Apex, -30f, new Vector2(3.4f, 3.4f), 36f, 0.38f, 24f, 0.20f); // -> ~(70.9, 16.2)!
        for (int x = 62; x <= 68; x++)
            for (int y = 15; y <= 24; y++)
                if (Mathf.Abs(y - (23f - (x - 59f) * 0.577f)) <= 2.1f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Purple Diagonal Arch #2
        p.SpawnBlock(p.BlockGreen, env, 63f, 29f, 10, 2, 4);              // Reactor Gantry (63..68, Y=28..29)
        p.Spawn(p.FireJetDown, haz, 65.5f, 28f);                          // Timed downward jet above dive!

        // Mega-Pad Catapult Deck (68..76, Y=12) + Air DashCharge -> Blast Door Runway (80..92, Y=16)
        p.SpawnGroundSized(env, 68f, 12f, 8);
        SpawnPierToY(p, env, 68f, 12f, 2f, 4);                            // Terminates cleanly on Safety Floor Y=2!
        p.Spawn(p.BouncePadMega, inter, 72f, 12f);                        // Catches ~(70.9, 16.2) -> shoots up 9.3m!
        p.Spawn(p.DashCharge, inter, 74.5f, 16.5f);

        p.SpawnGroundSized(env, 80f, 16f, 12);                            // Blast Aperture Deck 80..92, Y=15..16
        SpawnPierToY(p, env, 80f, 16f, 2f, 4);                            // Terminates cleanly on Safety Floor Y=2!
        p.SpawnCeilingSized(env, 80f, 26f, 12);                           // Blast Aperture Hood 80..92, Y=25..26
        GameObject db1Blast = p.Spawn(p.DashBoostFast, inter, 77f, 19.5f);
        ConfigureDashZoneBoost(db1Blast, new Vector2(3.6f, 4.2f), 38f, 0.40f, 24f); // -> ~92.2 cleanly into Sanct 1!
        for (int x = 81; x <= 83; x++)
            for (int y = 17; y <= 23; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Purple Arch Curtain!
        for (int x = 86; x <= 88; x++)
            for (int y = 17; y <= 23; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // Yellow Cracked Blast Door (86..89)!
        p.Spawn(p.FireJet, haz, 90.5f, 16f);                              // Post-door vent
        p.Spawn(p.CrumblingBlock, env, 90.5f, 23f);                       // Coupled clear above blast path Y=19.5!
        SetAtomId(p.Spawn(p.Atom, col, 90.5f, 19.5f), 5);

        // Act 1 Continuous Recovery Safety Floor (X=46..94, Y=2)
        p.SpawnGroundSized(env, 46f, 2f, 48);
        SpawnPierToBedrock(p, env, 46f, 2f, 4);
        SpawnPierToBedrock(p, env, 68f, 2f, 4);
        SpawnPierToBedrock(p, env, 80f, 2f, 4);
        SpawnPierToBedrock(p, env, 90f, 2f, 4);
        p.Spawn(p.BouncePadMega, inter, 54f, 2f);
        p.Spawn(p.CrumblingBlock, env, 50f, 5.5f);
        p.Spawn(p.FireJet, haz, 61f, 2f);
        p.Spawn(p.CrumblingBlock, env, 61f, 5.5f);                        // Coupled above recovery FireJet!
        GameObject mp1Rec = p.Spawn(p.MovingPlatform, env, 65f, 8.5f);
        ConfigureMovingPlatform(mp1Rec, new Vector3(5f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.BouncePadMega, inter, 68f, 2f);
        p.Spawn(p.FireJet, haz, 75f, 2f);
        p.Spawn(p.CrumblingBlock, env, 75f, 5.5f);
        p.Spawn(p.BouncePadMega, inter, 83f, 2f);
        p.Spawn(p.DashLaneUp, inter, 90f, 9.5f);

        // Sanctuary 1 (X=92..112, Y=16)
        p.SpawnGroundSized(env, 92f, 16f, 20);
        SpawnPierToBedrock(p, env, 94f, 16f, 4);
        SpawnPierToBedrock(p, env, 108f, 16f, 4);
        p.SpawnCeilingSized(env, 94f, 26f, 16);
        p.SpawnBlock(p.BlockMagenta, env, 95f, 25f, 28, 2, 4);
        p.Spawn(p.DashCharge, inter, 95f, 18.5f);                         // Closes Blast Door -> Sanct 1 handoff!
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 102f, 17.5f);
        cp1.name = "Checkpoint_1";
        GameObject db1To2 = p.Spawn(p.DashBoostFast, inter, 107.5f, 17.5f);
        ConfigureDashZoneBoost(db1To2, new Vector2(3.0f, 3.0f), 30f, 0.14f, 22f); // -> X=111.7 cleanly onto Act 2 Chute!

        // =====================================================================
        // ACT 2: THE TWIN-CONDUIT CORKSCREW (X = 112..232, Lower Y=4, Upper Y=19)
        // =====================================================================
        // Step Chute Down from Sanctuary 1 (112..124, Y=16 -> 4)
        for (int s = 0; s < 6; s++)
        {
            float sx = 113f + s * 2f;
            float sy = 14f - s * 2f;
            p.Spawn(p.Slope45Left, env, sx, sy);                          // 112..124 descending cleanly 16->4!
            int fillH = 10 - s * 2;
            if (fillH > 0) p.SpawnBlock(p.BlockCyan, env, sx - 1f, sy, 4, fillH * 2, 4);
        }
        p.Spawn(p.DashCharge, inter, 117f, 13.5f);                        // Mid-chute boost dash charge (closes 111.7->128)!
        p.SpawnGroundSized(env, 112f, 4f, 12);                            // Under-chute base 112..124, Y=3..4

        // Lower Conduit Floor (Continuous X=124..216, Y=4)
        for (int x = 124; x < 216; x += 20)
            p.SpawnGroundSized(env, x, 4f, Mathf.Min(20, 216 - x));
        foreach (float px in new[] { 126f, 146f, 174f, 198f, 212f })
            SpawnPierToBedrock(p, env, px, 4f, 4);

        // Mid-Pipe Separator Deck (Y=19, i.e., Y=18..19) with 8m Corkscrew Breaches!
        p.SpawnGroundSized(env, 124f, 19f, 8);                            // Separator Segment A: X=124..132
        p.SpawnBlock(p.BlockGreen, env, 124f, 18f, 16, 2, 4);
        // -> Corkscrew #1 Upward Shaft A: X=132..140 (8m clear width!)
        p.SpawnGroundSized(env, 140f, 19f, 26);                           // Separator Segment B: X=140..166
        p.SpawnBlock(p.BlockGreen, env, 144f, 18f, 36, 2, 4);             // Starts cleanly at 144 (clear of BlockCyan@142..144!)
        // -> Corkscrew #2 Downward Dive Shaft B: X=166..174 (8m clear width!)
        for (int x = 174; x < 206; x += 20)
            p.SpawnGroundSized(env, x, 19f, Mathf.Min(20, 206 - x));      // Separator Segment C: X=174..206
        p.SpawnBlock(p.BlockGreen, env, 176f, 18f, 56, 2, 4);
        // -> Corkscrew #3 East Escape Shaft C: X=206..214 (8m clear width!)

        // --- Corkscrew #1: Lower Entry -> BLAST STRAIGHT UP Shaft A (132..140) ---
        for (int x = 125; x <= 127; x++)
            for (int y = 5; y <= 9; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Lower Conduit Entry Doorway
        GameObject db2Entry = p.Spawn(p.DashBoostFast, inter, 128f, 5.5f);
        ConfigureDashZoneBoost(db2Entry, new Vector2(3.2f, 3.2f), 34f, 0.14f, 22f); // -> X=132.8 onto BouncePad!
        p.Spawn(p.BouncePad, inter, 136f, 4f);
        p.Spawn(p.DashLaneUp, inter, 136f, 10f);
        GameObject hold2Up1 = p.Spawn(p.DashHold, inter, 136f, 15.5f);
        ConfigureDashZoneHold(hold2Up1, 86f, new Vector2(3.4f, 3.0f), 36f, 0.34f, 22f, 0.08f); // -> ~(136.9, 27.7) straight UP!
        for (int x = 133; x <= 138; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 18f);                      // Hatch A Purple layer Y=17..18
        p.Spawn(p.OneWayPlatformLarge, env, 132f, 19f);                   // Catch platform 132..140 flush at Y=19!
        for (int x = 133; x <= 138; x++)
            for (int y = 21; y <= 22; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // Hatch A Yellow Ceiling Y=20..22!
        SetAtomId(p.Spawn(p.Atom, col, 136.5f, 26f), 6);
        p.Spawn(p.DashCharge, inter, 139.5f, 25f);                        // Closes Shaft A -> Upper Traverse gap!

        // --- Upper Conduit Traverse (140..166, Y=19..35) ---
        for (int x = 141; x <= 142; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 20f);                      // Upper Carpet Split A (clear of BouncePad@145!)
        for (int x = 147; x <= 152; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 20f);                      // Upper Carpet Split B (clear below MovingPlat@22.5!)
        p.Spawn(p.BouncePad, inter, 145f, 19f);
        GameObject mp2Shuttle = p.Spawn(p.MovingPlatform, env, 144f, 22.5f);
        ConfigureMovingPlatform(mp2Shuttle, new Vector3(8f, 0f, 0f), 3.6f, 0.15f);
        p.Spawn(p.CrumblingBlock, env, 151.5f, 21.5f);                    // Coupled near SpinArm!
        GameObject spin2Upper = p.Spawn(p.SpinArm, haz, 150f, 27.5f);
        ConfigureSpinArm(spin2Upper, 90f);

        GameObject db2Upper = p.Spawn(p.DashBoostFast, inter, 155f, 20.5f);
        ConfigureDashZoneBoost(db2Upper, new Vector2(3.2f, 3.2f), 34f, 0.22f, 22f); // -> X=162.5 into BouncePadMini!
        for (int x = 158; x <= 160; x++)
            for (int y = 20; y <= 25; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Upper Curtain!
        p.Spawn(p.CrumblingBlock, env, 163f, 24.5f);
        p.Spawn(p.BouncePadMini, inter, 163f, 19f);                       // Hops cleanly into Dive B (166..174)!
        SetAtomId(p.Spawn(p.Atom, col, 169f, 27f), 7);

        // --- Corkscrew #2: Earth Ground Pound Dive Shaft B (166..174, 8m wide!) ---
        for (int x = 166; x <= 172; x += 2)
            SpawnSingleHitCracked4x4(p, inter, x, 19f);                   // Layer 1: Y=17..19 (1-hit GP hatch!)
        for (int x = 166; x <= 173; x++)
            p.Spawn(p.TouchWallBlue, inter, x, 15f);                      // Layer 2: Purple cushion Y=14..15
        SetAtomId(p.Spawn(p.Atom, col, 170f, 16f), 8);
        for (int x = 166; x <= 173; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, 8f);                // Layer 3: GP Crystal Strata Y=7..12 (clean above DashCharge@5)!
        p.Spawn(p.DashCharge, inter, 168.5f, 5f);                         // Instant Dash Charge at Dive B floor for Side-Conduit!

        // --- High-Risk Secret Diamond 1 Vault (142..165, Y=4..18) off Dive B! ---
        p.SpawnBlock(p.BlockCyan, env, 142f, 17f, 4, 26, 4);              // Side-Conduit West Bulkhead X=142..144, Y=4..17
        p.SpawnBlock(p.BlockMagenta, env, 162f, 18f, 8, 16, 4);           // Vault Header X=162..166, Y=10..18
        for (int y = 5; y <= 10; y++)
            p.Spawn(p.DashWallCracked, inter, 164.5f, y - 0.5f);          // Sealed Yellow Side-Conduit Door (X=164..165, Y=4..10)!
        GameObject saw2Vault = p.Spawn(p.SawSlow, haz, 157f, 12.5f);
        ConfigureSaw(saw2Vault, new Vector3(0f, -4.5f, 0f), 2.8f);        // Patrols Y=8..12.5 guarding vault jump & exit!
        p.Spawn(p.FireJet, haz, 152f, 4f);
        p.Spawn(p.CrumblingBlock, env, 152f, 8.5f);                       // Coupled above vault FireJet!
        p.Spawn(p.OneWayPlatform, env, 144f, 11f);
        for (int x = 145; x <= 149; x++)
            for (int y = 5; y <= 8; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Vault Purple Treasure Pile!
        SetDiamondId(p.Spawn(p.Diamond, col, 145.5f, 6.5f), 1);           // Secret Side-Conduit Diamond 1!
        SetAtomId(p.Spawn(p.Atom, col, 147.5f, 13f), 9);
        GameObject db2VaultOut = p.Spawn(p.DashBoostFast, inter, 152f, 5.5f);
        ConfigureDashZoneBoost(db2VaultOut, new Vector2(3.2f, 3.2f), 36f, 0.52f, 24f); // -> X=170.7 blasting straight out RIGHT!

        // --- Lower Pipe Runway (X=174..206, Y=4..18 inside CameraZone_Tight) ---
        GameObject db2Low1 = p.Spawn(p.DashBoostFast, inter, 175f, 5.5f);
        ConfigureDashZoneBoost(db2Low1, new Vector2(3.2f, 3.2f), 34f, 0.22f, 24f); // -> X=182.5 into dz2Low2!
        for (int x = 178; x <= 180; x++)
            for (int y = 5; y <= 9; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Lower Purple Thicket #1!

        GameObject dz2Low2 = p.Spawn(p.DashZone, inter, 183f, 5.5f);
        ConfigureDashZoneBoost(dz2Low2, new Vector2(3.4f, 3.2f), 38f, 0.40f, 24f); // -> X=198.2!
        for (int x = 186; x <= 188; x++)
            for (int y = 5; y <= 11; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // 2m clear gap before Yellow!
        for (int y = 5; y <= 11; y++)
            p.Spawn(p.DashWallChainRow1, inter, 191f, y);                 // Yellow Containment Bulkhead (191..196)!
        GameObject stomp2A = p.Spawn(p.StompBlock, haz, 185f, 13.5f);
        ConfigureStompBlock(stomp2A, 0.25f);
        GameObject stomp2B = p.Spawn(p.StompBlock, haz, 198f, 13.5f);
        ConfigureStompBlock(stomp2B, 0.50f);
        p.Spawn(p.CrumblingBlock, env, 199f, 8.5f);                       // Coupled skip ledge!
        p.Spawn(p.DashCharge, inter, 201f, 11f);                          // Closes dz2Low2 -> BouncePadMega gap!
        GameObject mp2LowLift = p.Spawn(p.MovingPlatform, env, 203.5f, 9.5f);
        ConfigureMovingPlatform(mp2LowLift, new Vector3(0f, 4.5f, 0f), 3.4f, 0.15f);
        GameObject saw2Low = p.Spawn(p.SawSlow, haz, 201f, 5f);
        ConfigureSaw(saw2Low, new Vector3(4f, 0f, 0f), 2.8f);

        // --- Corkscrew #3: East Escape Shaft C (206..215, Y=4 -> 26) -> Sanctuary 2! ---
        p.Spawn(p.Wall1x12, env, 215f, 16f);                              // Encloses Lower Conduit East End X=215..216, Y=4..16!
        p.SpawnBlock(p.BlockCyan, env, 214f, 25f, 4, 18, 4);              // Supports Sanct 2 X=214..216, Y=16..25
        p.SpawnBlock(p.BlockCyan, env, 214f, 16f, 2, 24, 4);              // Backfill X=214..215, Y=4..16 beside Wall_1x12!
        p.Spawn(p.BouncePadMega, inter, 208f, 4f);
        p.Spawn(p.DashLaneUp, inter, 210f, 12f);
        GameObject hold2Esc = p.Spawn(p.DashHold, inter, 210f, 19f);
        ConfigureDashZoneHold(hold2Esc, 80f, new Vector2(3.4f, 3.2f), 36f, 0.34f, 22f, 0.08f); // -> ~(212.1, 31.0)!
        for (int x = 207; x <= 212; x++)
            for (int y = 22; y <= 23; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // East Hatch Yellow seal Y=21..23!
        for (int x = 207; x <= 212; x++)
            for (int y = 24; y <= 25; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // East Hatch Purple top Y=23..25!
        p.Spawn(p.OneWayPlatformLarge, env, 206f, 26f);                   // Catch 206..214 flush with Sanctuary 2!
        SetAtomId(p.Spawn(p.Atom, col, 211f, 29.5f), 10);

        // Sanctuary 2 (X=214..232, Y=26)
        p.SpawnGroundSized(env, 214f, 26f, 18);
        SpawnPierToY(p, env, 228f, 26f, 4f, 4);                           // Terminates cleanly on Lower Floor Y=4!
        p.SpawnBlock(p.BlockMagenta, env, 215f, 35f, 32, 2, 4);           // Sanctuary trim flush under Ceiling@36
        p.Spawn(p.DashCharge, inter, 216f, 28f);                          // Closes hold2Esc -> Sanct 2 handoff!
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 223f, 27.5f);
        cp2.name = "Checkpoint_2";
        GameObject db2To3 = p.Spawn(p.DashBoostFast, inter, 229f, 27.5f);
        ConfigureDashZoneBoost(db2To3, new Vector2(3.0f, 3.0f), 30f, 0.14f, 22f); // -> X=233.2 into hold3Entry@235!

        // =====================================================================
        // ACT 3: THE TURBINE PINBALL DOME (X = 232..334, Y = -4..52)
        // =====================================================================
        // Continuous Terraced Basin Recovery Floors (Y=-4 & Y=4) + BouncePadMega/DashLane_UP/LavaLeapers!
        for (int x = 232; x < 302; x += 20)
            p.SpawnGroundSized(env, x, -4f, Mathf.Min(20, 302 - x));      // West Basin Floor X=232..302, Y=-5..-4
        for (int x = 302; x < 334; x += 20)
            p.SpawnGroundSized(env, x, 4f, Mathf.Min(20, 334 - x));       // East Basin Step X=302..334, Y=3..4
        SpawnPierToBedrock(p, env, 232f, -4f, 4);
        SpawnPierToBedrock(p, env, 268f, -4f, 4);
        p.SpawnBlock(p.BlockCyan, env, 302f, 3f, 4, 38, 4);               // Clean step wall closing Y=-16..3!
        SpawnPierToBedrock(p, env, 324f, 4f, 4);

        p.Spawn(p.BouncePadMega, inter, 240f, -4f);
        p.Spawn(p.LavaLeaper, haz, 247f, -4f);
        GameObject mp3B1 = p.Spawn(p.MovingPlatform, env, 249f, 4f);
        ConfigureMovingPlatform(mp3B1, new Vector3(4f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.BouncePadMega, inter, 254f, -4f);
        p.Spawn(p.DashLaneUp, inter, 254f, 4.5f);
        p.Spawn(p.LavaLeaper, haz, 262f, -4f);
        GameObject mp3B2 = p.Spawn(p.MovingPlatform, env, 262f, 4f);
        ConfigureMovingPlatform(mp3B2, new Vector3(5f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.BouncePadMega, inter, 270f, -4f);
        p.Spawn(p.LavaLeaper, haz, 279f, -4f);
        GameObject mp3B3 = p.Spawn(p.MovingPlatform, env, 280f, 11f);
        ConfigureMovingPlatform(mp3B3, new Vector3(6f, 0f, 0f), 3.5f, 0.15f);
        p.Spawn(p.BouncePadMega, inter, 288f, -4f);
        p.Spawn(p.DashLaneUp, inter, 288f, 4.5f);
        p.Spawn(p.BouncePadMega, inter, 298f, -4f);
        p.Spawn(p.BouncePadMega, inter, 310f, 4f);
        p.Spawn(p.DashLaneUp, inter, 312f, 12f);

        // Pinball Entry -> Deflection Bumper + Slope_45 Wall Kicker -> Fork
        GameObject hold3Entry = p.Spawn(p.DashHold, inter, 235f, 26f);
        ConfigureDashZoneHold(hold3Entry, -20f, new Vector2(3.4f, 3.4f), 34f, 0.30f, 22f, 0.22f); // -> ~(244.6, 22.5)!
        for (int x = 238; x <= 241; x++)
            for (int y = 21; y <= 28; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Hanging Curtain #1!

        p.SpawnGroundSized(env, 243f, 17f, 7);                            // Pinball Pedestal 243..250, Y=16..17
        p.SpawnBlock(p.BlockCyan, env, 244f, 16f, 4, 40, 4);              // Pier Y=-4..16
        p.Spawn(p.BouncePad, inter, 245f, 17f);
        p.Spawn(p.Slope45, env, 249f, 17f);                               // Angled Slope_45 Wall Kicker (248..250, 17..19)!
        p.Spawn(p.DashCharge, inter, 252f, 23f);

        p.SpawnGroundSized(env, 253f, 14f, 5);                            // Mid Bumper Island 253..258, Y=13..14
        p.Spawn(p.BouncePadMini, inter, 255f, 14f);
        p.Spawn(p.CrumblingBlock, env, 256f, 21f);
        p.Spawn(p.OneWayPlatform, env, 253f, 25f);                        // Upper Line Steppable Pad 253..257
        p.Spawn(p.BouncePad, inter, 255f, 25f);                           // Bounces cleanly up to Upper Pinball Barrel!

        // Main Turbine Glide & Diamond Cloud Pinball Line (X=259..322, Y=15..34)
        GameObject hold3Mid = p.Spawn(p.DashHold, inter, 259f, 22.5f);
        ConfigureDashZoneHold(hold3Mid, 36f, new Vector2(3.4f, 3.4f), 36f, 0.38f, 24f, 0.22f); // -> ~(270.1, 30.5)!
        for (int x = 262; x <= 268; x++)
            for (int y = 22; y <= 31; y++)
                if (Mathf.Abs(x - 265f) + Mathf.Abs(y - 26.5f) <= 3.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Purple Diamond Cloud #1!
        SetAtomId(p.Spawn(p.Atom, col, 265f, 27f), 11);

        p.Spawn(p.OneWayPlatform, env, 269f, 28f);                        // Catch 269..273
        p.Spawn(p.BouncePad, inter, 271f, 28f);
        p.Spawn(p.CrumblingBlock, env, 275f, 28.5f);
        p.Spawn(p.DashCharge, inter, 278f, 31f);                          // Initiates Long Sweeping Dome Glide!

        // Hanging Curtain #2 + Diamond Cloud #2 + Cannon Crossfire + Mid Slope Kicker
        for (int x = 282; x <= 284; x++)
            for (int y = 22; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Hanging Curtain #2 (33 Purple blocks, clean above SpinArm@14!)
        GameObject spin3Mid = p.Spawn(p.SpinArm, haz, 282f, 14f);
        ConfigureSpinArm(spin3Mid, 95f);
        p.Spawn(p.DashCharge, inter, 286f, 24f);
        p.Spawn(p.CrumblingBlock, env, 287f, 31.5f);                      // Coupled near CannonUpper, clear of Curtain@284..285!
        p.Spawn(p.CrumblingBlock, env, 296f, 12.5f);                      // Coupled near CannonLower!

        GameObject can3Upper = p.Spawn(p.CannonFast, haz, 287f, 36f);
        ConfigureCannon(can3Upper, new Vector3(0f, 0f, -135f), 11f, 2.2f, 22f);
        GameObject can3Lower = p.Spawn(p.CannonFast, haz, 298f, 9f);
        ConfigureCannon(can3Lower, new Vector3(0f, 0f, 140f), 11f, 2.4f, 22f);

        p.SpawnGroundSized(env, 290f, 15f, 6);                            // Mid-Dome Ricochet Kicker Deck 290..296, Y=14..15
        p.Spawn(p.BouncePadMini, inter, 292f, 15f);
        p.Spawn(p.Slope45, env, 295f, 15f);                               // Angled Slope_45 Kicker!
        for (int x = 292; x <= 298; x++)
            for (int y = 19; y <= 27; y++)
                if (Mathf.Abs(x - 295f) + Mathf.Abs(y - 23f) <= 3.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Purple Diamond Cloud #2!
        p.Spawn(p.DashCharge, inter, 300f, 21f);

        // Upper Aerial Pinball Bonus Line (2 Bonus Atoms 12, 13 + Secret Diamond 2 Vault!)
        GameObject hold3Top1 = p.Spawn(p.DashHold, inter, 264f, 35.5f);
        ConfigureDashZoneHold(hold3Top1, 12f, new Vector2(3.4f, 3.4f), 34f, 0.34f, 22f, 0.22f); // -> ~(275.3, 37.9)!
        p.Spawn(p.CrumblingBlock, env, 276f, 36f);
        p.Spawn(p.DashCharge, inter, 280f, 39f);
        for (int x = 282; x <= 285; x++)
            for (int y = 38; y <= 42; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Upper Purple Cloud
        SetAtomId(p.Spawn(p.Atom, col, 284f, 40f), 12);                   // Bonus Atom 12!

        GameObject mp3Top = p.Spawn(p.MovingPlatform, env, 289f, 38f);
        ConfigureMovingPlatform(mp3Top, new Vector3(4f, 0f, 0f), 3.6f, 0.15f);
        GameObject hold3Top2 = p.Spawn(p.DashHold, inter, 294f, 40f);
        ConfigureDashZoneHold(hold3Top2, 6f, new Vector2(3.4f, 3.4f), 34f, 0.32f, 22f, 0.22f); // -> ~(304.8, 41.1)!
        for (int x = 296; x <= 298; x++)
            for (int y = 39; y <= 43; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        SetAtomId(p.Spawn(p.Atom, col, 297f, 41f), 13);                   // Bonus Atom 13!
        for (int y = 39; y <= 43; y++)
            p.Spawn(p.DashWallCracked, inter, 300.5f, y - 0.5f);          // Upper Pinball Yellow Barrier!
        p.Spawn(p.OneWayPlatformLarge, env, 298f, 37f);
        SetDiamondId(p.Spawn(p.Diamond, col, 304f, 41f), 2);              // Upper Pinball Diamond 2!
        GameObject hold3TopOut = p.Spawn(p.DashHold, inter, 307f, 40f);
        ConfigureDashZoneHold(hold3TopOut, -46f, new Vector2(3.4f, 3.4f), 36f, 0.44f, 24f, 0.20f); // -> ~(318.0, 28.6)!

        // Dome Exit Barrel -> Curtain #3 -> Yellow Turbine Gate -> Sanctuary 3
        GameObject hold3End = p.Spawn(p.DashHold, inter, 304f, 19f);
        ConfigureDashZoneHold(hold3End, 26f, new Vector2(3.4f, 3.4f), 38f, 0.40f, 24f, 0.20f); // -> ~(317.7, 25.7)!
        for (int x = 307; x <= 310; x++)
            for (int y = 18; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Hanging Curtain #3 (48 blocks, catches BOTH paths)!
        for (int y = 20; y <= 29; y++)
            p.Spawn(p.DashWallChainRow1, inter, 313f, y);                 // Yellow Turbine Gate (313..318, catches BOTH paths)!

        // Sanctuary 3 (X=320..334, Y=22)
        p.Spawn(p.Wall1x12, env, 319f, 44f);                              // Fortified Dome East Buttress X=319..320, Y=32..44
        p.SpawnGroundSized(env, 320f, 22f, 14);
        SpawnPierToY(p, env, 320f, 22f, 4f, 4);                           // Terminates cleanly on East Basin Y=4!
        SpawnPierToY(p, env, 330f, 22f, 4f, 4);
        p.SpawnCeilingSized(env, 320f, 32f, 14);                          // Sanct 3 Roof Y=31..32
        p.SpawnBlock(p.BlockMagenta, env, 321f, 31f, 24, 2, 4);
        p.Spawn(p.DashCharge, inter, 320f, 26f);                          // Closes Dome exit -> Sanct 3 handoff!
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 327f, 23.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ACT 4: THE OVERDRIVE CORE BREACH (X = 334..416, Y = 4..41)
        // Non-Stop 6-Link Closing Chain: Boost -> Hold -> BouncePadMega -> DashLane_UP -> Hold -> Hold -> DashZone -> EndPortal!
        // =====================================================================
        for (int x = 334; x < 380; x += 20)
            p.SpawnGroundSized(env, x, 4f, Mathf.Min(20, 380 - x));       // Act 4 Safety Floor X=334..380, Y=3..4
        SpawnPierToBedrock(p, env, 344f, 4f, 4);
        SpawnPierToBedrock(p, env, 364f, 4f, 4);
        p.Spawn(p.BouncePadMega, inter, 344f, 4f);
        p.Spawn(p.FireJet, haz, 351f, 4f);
        p.Spawn(p.CrumblingBlock, env, 351f, 8f);                         // Coupled over FireJet!
        p.Spawn(p.BouncePadMega, inter, 358f, 4f);
        p.Spawn(p.LavaLeaper, haz, 367f, 4f);
        GameObject mp4Rec = p.Spawn(p.MovingPlatform, env, 365f, 8.5f);
        ConfigureMovingPlatform(mp4Rec, new Vector3(6f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.BouncePadMega, inter, 375f, 4f);
        p.Spawn(p.DashLaneUp, inter, 376f, 11.5f);

        // Victory Exit Sanctuary Deck (X=380..414, Y=14)
        for (int x = 380; x < 414; x += 20)
            p.SpawnGroundSized(env, x, 14f, Mathf.Min(20, 414 - x));      // X=380..414, Y=13..14
        p.SpawnBlock(p.BlockCyan, env, 380f, 13f, 4, 20, 4);              // Step pier 3->13
        SpawnPierToBedrock(p, env, 380f, 4f, 4);
        SpawnPierToBedrock(p, env, 404f, 14f, 4);
        p.SpawnCeilingSized(env, 404f, 26f, 10);                          // Crown Sanctuary Hood 404..414, Y=25..26
        p.SpawnBlock(p.BlockCyan, env, 405f, 25f, 16, 2, 4);

        // Link 1: DashBoost_Fast (+16 deg) -> Monolith 4A
        GameObject db4_1 = p.Spawn(p.DashBoostFast, inter, 334f, 23.5f);
        db4_1.transform.rotation = Quaternion.Euler(0f, 0f, 16f);
        ConfigureDashZoneBoost(db4_1, new Vector2(3.2f, 3.2f), 36f, 0.34f, 24f); // -> ~(345.7, 26.9)!
        for (int x = 337; x <= 340; x++)
            for (int y = 22; y <= 28; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Monolith 4A (28 Purple blocks)!
        SetAtomId(p.Spawn(p.Atom, col, 344f, 27f), 14);

        // Link 2: DashHold (-32 deg) -> Monolith 4B -> BouncePad_Mega Deck!
        GameObject hold4Dive = p.Spawn(p.DashHold, inter, 347f, 27f);
        ConfigureDashZoneHold(hold4Dive, -32f, new Vector2(3.4f, 3.4f), 36f, 0.34f, 24f, 0.20f); // -> ~(357.4, 20.5)!
        for (int x = 350; x <= 355; x++)
            for (int y = 19; y <= 28; y++)
                if (Mathf.Abs(y - (27f - (x - 347f) * 0.625f)) <= 2.2f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);                // Monolith 4B Diagonal!
        GameObject spin4Upper = p.Spawn(p.SpinArm, haz, 351f, 29.5f);
        ConfigureSpinArm(spin4Upper, -95f);

        p.SpawnGroundSized(env, 355f, 16f, 8);                            // Mega Pad Core Deck 355..363, Y=15..16
        p.SpawnBlock(p.BlockCyan, env, 358f, 15f, 4, 22, 4);              // Pier Y=4..15
        p.Spawn(p.BouncePadMega, inter, 359f, 16f);

        // Link 3: BouncePad_Mega -> DashLane_UP (Vertical!) -> Purple Core Halo -> hold4Zenith
        p.Spawn(p.DashLaneUp, inter, 360f, 22.5f);
        for (int x = 358; x <= 362; x++)
            for (int y = 28; y <= 29; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Core Vertical Halo (10 Purple blocks, zero chevron overlap)!

        // Link 4: hold4Zenith (-22 deg) -> Colossal Monolith 4C (Purple + Yellow Core Shield) -> Diamond 3!
        GameObject hold4Zenith = p.Spawn(p.DashHold, inter, 360f, 32f);
        ConfigureDashZoneHold(hold4Zenith, -22f, new Vector2(3.6f, 3.6f), 38f, 0.40f, 24f, 0.20f); // -> ~(374.1, 26.3)!
        for (int x = 364; x <= 367; x++)
            for (int y = 25; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // 4C Purple Vanguard (32 blocks)!
        for (int x = 369; x <= 371; x++)
            for (int y = 24; y <= 31; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);    // 4C Yellow Core Shield (24 blocks)!
        SetDiamondId(p.Spawn(p.Diamond, col, 373f, 27.5f), 3);            // Core Crown Diamond 3!
        SetAtomId(p.Spawn(p.Atom, col, 375f, 25.5f), 15);

        // Coupled MovingPlatform + Crumbling near Link 4/5 + Upper Crossfire Cannon
        GameObject mp4Core = p.Spawn(p.MovingPlatform, env, 367f, 15f);
        ConfigureMovingPlatform(mp4Core, new Vector3(5f, 0f, 0f), 3.4f, 0.15f);
        p.Spawn(p.CrumblingBlock, env, 364f, 17f);
        p.Spawn(p.CrumblingBlock, env, 382f, 27.5f);                      // Coupled near CannonFast!
        GameObject can4Fin = p.Spawn(p.CannonFast, haz, 386f, 32f);
        ConfigureCannon(can4Fin, new Vector3(0f, 0f, -135f), 11f, 2.3f, 22f);

        // Link 5: hold4Fin (-26 deg) -> Link 6: dz4Climax (DashZone Boost) -> Colossal Triple Containment Shield -> End_Portal!
        GameObject hold4Fin = p.Spawn(p.DashHold, inter, 377f, 23f);
        ConfigureDashZoneHold(hold4Fin, -26f, new Vector2(3.6f, 3.6f), 36f, 0.28f, 24f, 0.18f); // -> ~(386.1, 18.6)!

        GameObject dz4Climax = p.Spawn(p.DashZone, inter, 387f, 16.5f);
        ConfigureDashZoneBoost(dz4Climax, new Vector2(3.8f, 5.2f), 40f, 0.64f, 26f); // -> 412.6 nominal (< East Wall@414!), punches all 8 Yellow layers cleanly into End_Portal@408!
        for (int x = 390; x <= 393; x++)
            for (int y = 15; y <= 23; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                    // Finale Purple Vanguard (36 blocks)!
        for (int y = 15; y <= 22; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 395f, y);                 // Yellow Containment Shield (395..400)!
            p.Spawn(p.DashWallCracked, inter, 400.5f, y - 0.5f);          // Triple-Thick Yellow Core Layer 6!
            p.Spawn(p.DashWallCracked, inter, 401.5f, y - 0.5f);          // Layer 7!
            p.Spawn(p.DashWallCracked, inter, 402.5f, y - 0.5f);          // Layer 8! (64 Yellow blocks!)
        }
        p.Spawn(p.DashCharge, inter, 404f, 16.5f);                        // Finale post-bastion DashCharge (X-gap 17.0m)!
        SetAtomId(p.Spawn(p.Atom, col, 405.5f, 16f), 16);

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
        return $"Level_51 built at {TargetScenePath} (Validator errors: {errors})";
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
        public GameObject FireJetDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Down.prefab");
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
