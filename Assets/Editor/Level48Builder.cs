#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level48Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_48.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 48 (Barrel-Blast Highway)")]
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
        // GLOBAL FRAMING, BEDROCK & KILL BOUNDARIES (Linear Highway X = 0..388)
        // =====================================================================
        GameObject killPit = p.Spawn(p.KillPit, haz, 194f, -26f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(480f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(480f, 4f, 1f);

        // Continuous Bedrock Base at Y = -18 (X = -8 to 396)
        for (int x = -8; x < 396; x += 20)
            p.SpawnGroundSized(env, x, -18f, Mathf.Min(20, 396 - x));

        // West & East End Walls + Act-Specific Framed Canyon Ceilings (Y=30 -> 40 -> 30 -> 40)
        p.SpawnBlock(p.BlockCyan, env, 0f, 30f, 4, 96, 4);                // West Wall X=0..2, Y=-18..30
        p.SpawnBlock(p.BlockCyan, env, 386f, 40f, 4, 116, 4);             // East Wall X=386..388, Y=-18..40

        for (int x = 0; x < 92; x += 20)
            p.SpawnCeilingSized(env, x, 31f, Mathf.Min(20, 92 - x));      // Act 1 Roof X=0..92 flush against 92..94 step!
        p.SpawnBlock(p.BlockCyan, env, 92f, 40f, 4, 20, 4);               // Roof Step Up X=92..94, Y=30..40
        for (int x = 94; x < 192; x += 20)
            p.SpawnCeilingSized(env, x, 41f, Mathf.Min(20, 192 - x));     // Act 2 Roof X=94..192 flush against 192..194 step!
        p.SpawnBlock(p.BlockCyan, env, 192f, 40f, 4, 20, 4);              // Roof Step Down X=192..194, Y=30..40
        for (int x = 194; x < 292; x += 20)
            p.SpawnCeilingSized(env, x, 31f, Mathf.Min(20, 292 - x));     // Act 3 Roof X=194..292 flush against 292..294 step!
        p.SpawnBlock(p.BlockCyan, env, 292f, 40f, 4, 20, 4);              // Roof Step Up X=292..294, Y=30..40
        for (int x = 294; x < 388; x += 20)
            p.SpawnCeilingSized(env, x, 41f, Mathf.Min(20, 388 - x));     // Act 4 Finale Climax Roof Y=40..41
        p.SpawnBlock(p.BlockMagenta, env, 120f, 40f, 24, 4, 4);
        p.SpawnBlock(p.BlockGreen, env, 224f, 30f, 20, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 326f, 40f, 24, 4, 4);

        // =====================================================================
        // FLOW: LEFT HIGHWAY START -> EAST VICTORY MONOLITH END PORTAL
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 7f, 4f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 383f, 1.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Leading Slightly Ahead of Every Shot & Drop)
        // =====================================================================
        GameObject cz1Start = p.Spawn(p.CameraZoneTight, flow, 13f, 15f);
        cz1Start.name = "CameraZone_Tight_Act1Entry";
        SetTriggerBoxSize(cz1Start, new Vector3(26f, 26f, 10f));        // X=0..26, Y=2..28

        GameObject cz1Canyon = p.Spawn(p.CameraZoneWide, flow, 59f, 16f);
        cz1Canyon.name = "CameraZone_Wide_Act1LaunchCanyon";
        SetTriggerBoxSize(cz1Canyon, new Vector3(84f, 32f, 10f));       // X=17..101, Y=0..32

        GameObject cz2Climb = p.Spawn(p.CameraZoneClimb, flow, 114f, 24f);
        cz2Climb.name = "CameraZone_Climb_Act2SkyCannonPeak";
        SetTriggerBoxSize(cz2Climb, new Vector3(36f, 34f, 10f));        // X=96..132, Y=7..41

        GameObject cz2Pinball = p.Spawn(p.CameraZoneWide, flow, 139.5f, 20f);
        cz2Pinball.name = "CameraZone_Wide_Act2AerialPinball";
        SetTriggerBoxSize(cz2Pinball, new Vector3(105f, 44f, 10f));     // X=87..192, ends BEFORE db3Entry@195!

        GameObject cz3Tight = p.Spawn(p.CameraZoneTight, flow, 207f, 14f);
        cz3Tight.name = "CameraZone_Tight_Act3ClockworkEntry";
        SetTriggerBoxSize(cz3Tight, new Vector3(36f, 24f, 10f));        // X=189..225, Y=2..26

        GameObject cz3Range = p.Spawn(p.CameraZoneWide, flow, 255f, 16f);
        cz3Range.name = "CameraZone_Wide_Act3FiringRange";
        SetTriggerBoxSize(cz3Range, new Vector3(88f, 32f, 10f));        // X=211..299, Y=0..32

        GameObject cz4Climb = p.Spawn(p.CameraZoneClimb, flow, 307f, 19.5f);
        cz4Climb.name = "CameraZone_Climb_Act4WaterfallGlide";
        SetTriggerBoxSize(cz4Climb, new Vector3(38f, 43f, 10f));        // X=288..326, Y=-2..41 (cleanly ends BEFORE 334!)

        GameObject cz4Climax = p.Spawn(p.CameraZoneWide, flow, 355f, 20f);
        cz4Climax.name = "CameraZone_Wide_Act4HyperBlitz";
        SetTriggerBoxSize(cz4Climax, new Vector3(72f, 44f, 10f));       // X=319..391, Y=-2..42

        // =====================================================================
        // ACT 1: THE LAUNCH CANYON (X = 2..94, Y = 4..30, Multi-Angle Forward)
        // =====================================================================
        p.SpawnGroundSized(env, 2f, 4f, 92);                             // Continuous Floor X=2..94, Y=3..4
        foreach (float px in new[] { 16f, 34f, 54f, 74f, 90f })
            p.SpawnBlock(p.BlockCyan, env, px, 3f, 4, 42, 4);            // Bedrock piers Y=-18..3
        p.SpawnBlock(p.BlockGreen, env, 18f, 30f, 20, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 56f, 30f, 20, 4, 4);

        // DashBoost -> Ribbon #1 -> Slope45Large Kicker -> BouncePad -> Hold 1A -> Upper Atom 2!
        GameObject db1A = p.Spawn(p.DashBoostFast, inter, 13f, 5.5f);
        ConfigureDashZoneBoost(db1A, new Vector2(3.2f, 3.2f), 32f, 0.28f, 22f); // -> X=22.0 cleanly BEFORE Slope@24!
        p.Spawn(p.DashCharge, inter, 22f, 7f);                           // Closes 13..31 gap cleanly!
        for (int x = 16; x <= 21; x++)
            for (int y = 5; y <= 12; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Taller Purple Ribbon Arch #1

        p.Spawn(p.Slope45Large, env, 26f, 4f);                           // 24..28, 4..8 rises UP-RIGHT!
        p.SpawnGroundSized(env, 28f, 8f, 7);                             // Top of kicker 28..35, Y=7..8
        p.Spawn(p.BouncePad, inter, 31f, 8f);
        SetAtomId(p.Spawn(p.Atom, col, 31f, 11.2f), 1);

        // Hold 1A (24 deg UP-RIGHT) -> Upper Ribbon -> OneWay(19) -> Hold 1B (-30 deg DOWN-RIGHT) -> Cascade!
        GameObject hold1A = p.Spawn(p.DashHold, inter, 35f, 14f);
        ConfigureDashZoneHold(hold1A, 24f, new Vector2(3.4f, 3.4f), 34f, 0.38f, 22f, 0.22f); // -> ~(46.8, 19.3)
        for (int x = 38; x <= 44; x++)
            for (int y = 16; y <= 24; y++)
                if (Mathf.Abs(y - (15.5f + (x - 35f) * 0.44f)) <= 2.4f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Diagonal Flight Ribbon #1!

        p.Spawn(p.OneWayPlatformLarge, env, 45f, 19f);                   // Catch 45..53
        p.Spawn(p.BouncePadMini, inter, 48f, 19f);
        GameObject mp1Mid = p.Spawn(p.MovingPlatform, env, 50f, 12f);
        ConfigureMovingPlatform(mp1Mid, new Vector3(6f, 0f, 0f), 3.2f, 0.2f);
        GameObject spin1Mid = p.Spawn(p.SpinArm, haz, 54f, 16f);
        ConfigureSpinArm(spin1Mid, 90f);
        SetAtomId(p.Spawn(p.Atom, col, 48f, 25f), 2);                    // Upper High-Arc Barrel Trajectory!

        GameObject hold1B = p.Spawn(p.DashHold, inter, 53f, 21f);
        ConfigureDashZoneHold(hold1B, -30f, new Vector2(3.4f, 3.4f), 36f, 0.40f, 22f, 0.22f); // -> ~(65.5, 13.8)!
        for (int x = 56; x <= 63; x++)
            for (int y = 12; y <= 22; y++)
                if (Mathf.Abs(y - (21f - (x - 53f) * 0.58f)) <= 2.5f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Diagonal Cascade #2!

        // Lower Recovery Cushion (40..66, Y=4): BouncePads + LavaLeaper + Crumbling + Saw
        p.Spawn(p.BouncePadMega, inter, 43f, 4f);
        p.Spawn(p.DashCharge, inter, 43f, 12f);
        p.Spawn(p.LavaLeaper, haz, 50f, 4f);
        p.Spawn(p.CrumblingBlock, env, 50f, 9f);                         // Coupled over LavaLeaper!
        p.Spawn(p.CrumblingBlock, env, 59f, 9f);
        GameObject saw1 = p.Spawn(p.SawSlow, haz, 61f, 6f);
        ConfigureSaw(saw1, new Vector3(0f, 4f, 0f), 2.8f);
        p.Spawn(p.BouncePad, inter, 58f, 4f);

        // Act 1 Final Smash Gatehouse (65..82) -> Sanctuary 1 (Checkpoint_1 @ 88, 5.5!)
        p.Spawn(p.OneWayPlatformLarge, env, 63f, 12f);
        SetAtomId(p.Spawn(p.Atom, col, 67f, 15f), 3);
        p.Spawn(p.BouncePadMini, inter, 67f, 12f);
        GameObject db1Gate = p.Spawn(p.DashBoostFast, inter, 66f, 5.5f);
        ConfigureDashZoneBoost(db1Gate, new Vector2(3.4f, 3.4f), 36f, 0.44f, 24f); // -> ~X=81.8!
        for (int x = 70; x <= 73; x++)
            for (int y = 5; y <= 12; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 4x8 Purple Tower (2m gap before Yellow!)
        for (int x = 76; x <= 78; x++)
            for (int y = 5; y <= 11; y++)
                p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);   // Yellow Cracked Gatehouse!
        p.Spawn(p.BouncePadMini, inter, 81f, 4f);                        // Smooth post-gate handoff!

        p.SpawnCeilingSized(env, 82f, 15f, 12);                          // Sanctuary 1 Alcove Roof X=82..94
        p.SpawnBlock(p.BlockGreen, env, 83f, 14f, 16, 2, 4);
        p.Spawn(p.BouncePadMini, inter, 92f, 4f);                        // Closes Sanctuary 1 -> Act 2 X-gap cleanly!
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 88f, 5.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ACT 2: THE SKY-CANNON ZIG-ZAG (X = 94..192, Y = 0..40, Aerial Pinball!)
        // =====================================================================
        p.Spawn(p.Slope45Left, env, 95f, 2f);                            // Step down cleanly 94..96, 4..2
        p.Spawn(p.Slope45Left, env, 97f, 0f);                            // 96..98, 2..0
        p.SpawnGroundSized(env, 98f, 0f, 80);                            // Ends cleanly at 178 flush against 178..180 Pier!
        foreach (float px in new[] { 114f, 134f, 154f, 174f })
            p.SpawnBlock(p.BlockCyan, env, px, -1f, 4, 34, 4);           // Piers down to -18

        // Vertical Rocket Up (BouncePadMega + DashLane_UP @ 102) -> Hold 2A (28 deg UP-RIGHT) -> Aerial Smash-Pod (Diamond 1!)
        p.Spawn(p.BouncePadMega, inter, 102f, 0f);
        p.Spawn(p.DashLaneUp, inter, 102f, 10f);                         // Catapults UP X=102, Y=6..14
        for (int x = 99; x <= 105; x++)
            for (int y = 16; y <= 20; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Sky Pillar Purple Stack!
        SetAtomId(p.Spawn(p.Atom, col, 102f, 29f), 4);

        GameObject hold2A = p.Spawn(p.DashHold, inter, 102f, 24f);
        ConfigureDashZoneHold(hold2A, 28f, new Vector2(3.6f, 3.6f), 36f, 0.42f, 22f, 0.22f); // -> ~(115.3, 31.1)!
        for (int x = 107; x <= 113; x++)
            for (int y = 25; y <= 33; y++)
                if (Mathf.Abs(x - 110f) + Mathf.Abs(y - 29f) <= 4f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Giant Floating Purple Diamond Cloud!

        // Upper Aerial Smash-Pod (114..123, Y = 30..37 -> Diamond 1 + Atom 5!)
        p.Spawn(p.OneWayPlatformLarge, env, 114f, 30f);                  // 114..122
        p.Spawn(p.DashCharge, inter, 116f, 32f);
        for (int y = 31; y <= 36; y++) p.Spawn(p.DashWallCracked, inter, 118.5f, y - 0.5f); // Pod Breach Door!
        SetDiamondId(p.Spawn(p.Diamond, col, 120.5f, 31.8f), 1);         // Aerial Pod Diamond 1!
        SetAtomId(p.Spawn(p.Atom, col, 122.5f, 34f), 5);

        // Hold 2B (-40 deg DOWN-RIGHT) dodging SpinArm -> GP Box Vault (135..147) -> Instant BouncePadMega Rebound UP-RIGHT!
        GameObject hold2B = p.Spawn(p.DashHold, inter, 124f, 30f);
        ConfigureDashZoneHold(hold2B, -40f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 22f, 0.22f); // -> ~(136.1, 19.8)!
        GameObject spin2A = p.Spawn(p.SpinArm, haz, 130f, 25f);
        ConfigureSpinArm(spin2A, 95f);
        p.Spawn(p.CrumblingBlock, env, 130f, 20f);                       // Coupled near SpinArm below Hold2B dive!
        for (int x = 127; x <= 134; x++)
            for (int y = 21; y <= 28; y++)
                if (Mathf.Abs(y - (30f - (x - 124f) * 0.84f)) <= 2.3f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);               // Diagonal Curtain #1!

        // Aerial GP Box Vault (X = 135..148, Y = 6..19)
        p.Spawn(p.OneWayPlatformLarge, env, 135f, 19f);                  // Top Catch Grate 135..143
        p.Spawn(p.CrackedBlocks4x4, inter, 136f, 18f);                   // Y=16..18
        p.Spawn(p.CrackedBlocks4x4, inter, 140f, 18f);
        for (int x = 136; x <= 143; x++) p.Spawn(p.GroundPoundChainRow2, inter, x, 10f); // Grows UP Y=10..14!
        p.SpawnGroundSized(env, 134f, 6f, 14);                           // Vault Floor X=134..148, Y=5..6
        p.Spawn(p.BouncePadMega, inter, 143f, 6f);                       // Shoots straight back UP-RIGHT!
        SetAtomId(p.Spawn(p.Atom, col, 143f, 14f), 6);

        // Hold 2C -> Tower of Purple Touch-Blocks -> Hold 2D (-32 deg) weaving Saw_Slow -> Yellow Gate -> Sanctuary 2!
        GameObject hold2C = p.Spawn(p.DashHold, inter, 143f, 21f);
        ConfigureDashZoneHold(hold2C, 14f, new Vector2(3.6f, 3.6f), 36f, 0.44f, 22f, 0.22f); // -> ~(158.4, 24.8)!
        for (int x = 148; x <= 155; x++)
            for (int y = 20; y <= 32; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Massive Purple Hanging Tower (8x13)!
        GameObject saw2A = p.Spawn(p.SawSlow, haz, 158f, 16f);
        ConfigureSaw(saw2A, new Vector3(0f, 5.5f, 0f), 3.0f);
        GameObject mp2Saw = p.Spawn(p.MovingPlatform, env, 157f, 12f);
        ConfigureMovingPlatform(mp2Saw, new Vector3(4f, 0f, 0f), 3.0f, 0.2f);
        SetAtomId(p.Spawn(p.Atom, col, 158f, 27f), 8);

        GameObject hold2D = p.Spawn(p.DashHold, inter, 162f, 23f);
        ConfigureDashZoneHold(hold2D, -32f, new Vector2(3.6f, 3.6f), 36f, 0.48f, 24f, 0.22f); // -> ~(176.7, 13.8)!
        for (int x = 166; x <= 169; x++)
            for (int y = 14; y <= 24; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m clear gap!
        for (int y = 11; y <= 18; y++)
            p.Spawn(p.DashWallChainRow1, inter, 172f, y);                // Aerial Yellow Bulkhead (172..177)!

        // Act 2 Continuous Recovery Floor & Rebounders (Y = 0)
        p.Spawn(p.BouncePadMega, inter, 114f, 0f);
        p.Spawn(p.DashCharge, inter, 114f, 10f);
        p.Spawn(p.FireJet, haz, 124f, 0f);
        p.Spawn(p.CrumblingBlock, env, 123f, 3.5f);                      // Coupled directly above FireJet!
        p.Spawn(p.BouncePadMega, inter, 128f, 0f);
        SetAtomId(p.Spawn(p.Atom, col, 128f, 5f), 7);
        p.Spawn(p.BouncePad, inter, 152f, 0f);
        p.Spawn(p.FireJet, haz, 164f, 0f);
        p.Spawn(p.CrumblingBlock, env, 164f, 4f);
        p.Spawn(p.BouncePadMega, inter, 168f, 0f);
        p.Spawn(p.DashCharge, inter, 168f, 10f);

        // Step back up to Sanctuary 2 (X = 178..194, Y = 8..18)
        p.SpawnGroundSized(env, 178f, 8f, 16);                           // X=178..194, Y=7..8
        p.SpawnBlock(p.BlockCyan, env, 178f, 7f, 4, 50, 4);
        p.SpawnCeilingSized(env, 178f, 18f, 16);
        p.SpawnBlock(p.BlockMagenta, env, 180f, 17f, 16, 2, 4);
        p.Spawn(p.OneWayPlatformLarge, env, 170f, 8f);                   // Recovery ramp step 170..178
        p.Spawn(p.BouncePadMini, inter, 176f, 0f);
        p.Spawn(p.DashCharge, inter, 191f, 9.5f);                        // Closes Sanctuary 2 -> Act 3 X-gap cleanly!
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 186f, 9.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ACT 3: THE CLOCKWORK FIRING RANGE (X = 194..296, Y = 4..30)
        // Moving Shuttles + Crumbling Kickers + Secret 65deg Ceiling Vault (Diamond 2!)
        // =====================================================================
        p.SpawnGroundSized(env, 194f, 8f, 16);                           // Takeoff Floor X=194..210, Y=7..8
        p.SpawnGroundSized(env, 194f, 4f, 44);                           // Recovery + Secret Base 194..238, Y=3..4
        p.Spawn(p.IceSurface, env, 238f, 4f);                            // Ice Runway 238..244, Y=3..4
        p.SpawnGroundSized(env, 244f, 4f, 50);                           // Ends cleanly at 294 flush before Slope45Left@(295,2)!

        GameObject db3Entry = p.Spawn(p.DashBoostFast, inter, 195f, 9.5f);
        ConfigureDashZoneBoost(db3Entry, new Vector2(3.2f, 3.2f), 34f, 0.30f, 22f); // -> ~X=205.2!
        for (int x = 199; x <= 203; x++)
            for (int y = 9; y <= 17; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Box Tunnel #1
        SetAtomId(p.Spawn(p.Atom, col, 201f, 18.5f), 9);

        // Moving Shuttle + StompBlock + Alternating Barrels + Secret 66 deg Ceiling Vault (Diamond 2!)
        GameObject hold3A = p.Spawn(p.DashHold, inter, 208f, 10.5f);
        ConfigureDashZoneHold(hold3A, 14f, new Vector2(3.4f, 3.4f), 34f, 0.38f, 22f, 0.22f); // -> ~(220.5, 13.6)!
        for (int x = 212; x <= 215; x++)
            for (int y = 10; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Pillar Stack #1 (cleanly left of MovingPlat@217..221!)
        p.Spawn(p.DashCharge, inter, 221f, 14f);                         // Exact Hold3A endpoint handoff!

        GameObject mp3A = p.Spawn(p.MovingPlatform, env, 219f, 10f);
        ConfigureMovingPlatform(mp3A, new Vector3(6f, 0f, 0f), 3.4f, 0.15f);
        GameObject stomp3A = p.Spawn(p.StompBlock, haz, 223f, 18f);
        ConfigureStompBlock(stomp3A, 0.28f);
        SetAtomId(p.Spawn(p.Atom, col, 223f, 14f), 10);
        p.Spawn(p.CrumblingBlock, env, 229f, 11f);

        // Secret Upward 66-deg Barrel Vault (Stub ends X=236 so Hold3Secret@(236,15) shoots CLEAN through DashWall@236..240!)
        p.SpawnGroundSized(env, 232f, 11f, 5);                           // Mini pier X=232..237, Y=10..11
        p.Spawn(p.BouncePadMini, inter, 234f, 11f);
        GameObject hold3Secret = p.Spawn(p.DashHold, inter, 236f, 15f);
        ConfigureDashZoneHold(hold3Secret, 66f, new Vector2(3.2f, 3.2f), 34f, 0.32f, 20f, 0.24f); // Shoots clean UP-RIGHT!
        for (int x = 236; x <= 240; x++) p.Spawn(p.DashWall, inter, x, 19f); // Yellow Ceiling Breach X=236..241!
        p.SpawnGroundSized(env, 241f, 19f, 9);                           // Secret Floor 241..250
        p.SpawnGroundSized(env, 234f, 19f, 2);                           // Vault West Stub 234..236 (zero solid hit!)
        p.SpawnCeilingSized(env, 234f, 28f, 17);
        p.SpawnBlock(p.BlockCyan, env, 233f, 27f, 2, 18, 4);
        p.SpawnBlock(p.BlockCyan, env, 250f, 27f, 2, 18, 4);
        p.Spawn(p.FireJet, haz, 244f, 19f);
        p.Spawn(p.CrumblingBlock, env, 244f, 22.5f);                     // Coupled over Vault FireJet!
        p.Spawn(p.BouncePadMini, inter, 248f, 19f);                      // Handoff right out of Vault!
        SetAtomId(p.Spawn(p.Atom, col, 242f, 21.5f), 11);
        SetDiamondId(p.Spawn(p.Diamond, col, 247f, 21.5f), 2);

        // Low -> High -> Steep-Down Enclosed Box Tunnels (240..282)
        GameObject db3Ice = p.Spawn(p.DashBoostFast, inter, 240f, 5.5f);
        ConfigureDashZoneBoost(db3Ice, new Vector2(3.2f, 3.2f), 36f, 0.42f, 24f); // -> X=255.1!
        for (int x = 245; x <= 247; x++)
            for (int y = 5; y <= 13; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int y = 5; y <= 12; y++)
            p.Spawn(p.DashWallChainRow1, inter, 250f, y);                // Bulkhead #1 (250..255)!

        p.Spawn(p.CrumblingBlock, env, 257f, 6.5f);                      // 1m clear past Bulkhead #1!
        GameObject can3A = p.Spawn(p.CannonFast, haz, 257f, 15.5f);
        ConfigureCannon(can3A, new Vector3(0f, 0f, -155f), 11f, 2.3f, 22f);
        GameObject mp3B = p.Spawn(p.MovingPlatform, env, 262f, 6.5f);
        ConfigureMovingPlatform(mp3B, new Vector3(0f, 5.5f, 0f), 3.4f, 0.15f);
        SetAtomId(p.Spawn(p.Atom, col, 260f, 14f), 12);
        p.Spawn(p.DashCharge, inter, 260f, 11f);

        GameObject hold3B = p.Spawn(p.DashHold, inter, 264f, 12.5f);
        ConfigureDashZoneHold(hold3B, -18f, new Vector2(3.4f, 3.4f), 36f, 0.42f, 24f, 0.22f); // -> ~(278.4, 7.8)!
        GameObject stomp3B = p.Spawn(p.StompBlock, haz, 269f, 17f);
        ConfigureStompBlock(stomp3B, 0.30f);
        p.Spawn(p.CrumblingBlock, env, 270f, 14.2f);                     // Coupled near Stomp3B (above gate top 13!)
        for (int x = 268; x <= 270; x++)
            for (int y = 5; y <= 13; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 2m gap!
        for (int y = 5; y <= 13; y++)
            p.Spawn(p.DashWallChainRow1, inter, 273f, y);                // Bulkhead #2 (273..278)!

        // Act 3 Continuous Recovery Floor BouncePads + Sanctuary 3 (282..294)
        p.Spawn(p.BouncePad, inter, 214f, 4f);
        p.Spawn(p.BouncePad, inter, 228f, 4f);
        p.Spawn(p.BouncePadMega, inter, 258f, 4f);
        p.Spawn(p.BouncePadMini, inter, 280f, 4f);
        p.Spawn(p.BouncePadMini, inter, 291f, 4f);                       // Closes Act 3 -> Act 4 X-gap cleanly!
        p.SpawnCeilingSized(env, 282f, 16f, 12);
        p.SpawnBlock(p.BlockGreen, env, 283f, 15f, 14, 2, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 287f, 5.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ACT 4: THE HYPER-BARREL CLIMAX (X = 294..386, Y = 0..40)
        // Glide Waterfall -> Slope45Large Kicker -> Dual-Line Hyper Finale -> End_Portal!
        // =====================================================================
        p.Spawn(p.Slope45Left, env, 295f, 2f);                           // Drop 294..296, 4..2
        p.Spawn(p.Slope45Left, env, 297f, 0f);                           // Drop 296..298, 2..0
        p.SpawnGroundSized(env, 298f, 0f, 88);                           // Continuous Grand Climax Floor X=298..386, Y=-1..0!
        foreach (float px in new[] { 316f, 340f, 364f, 382f })
            p.SpawnBlock(p.BlockCyan, env, px, -1f, 4, 34, 4);

        // Sweeping Glide Through Giant 200+ Block Purple Waterfall (300..326)!
        p.Spawn(p.BouncePadMega, inter, 300f, 0f);
        p.Spawn(p.DashLaneUp, inter, 300f, 10f);                         // Catapults high to Glide Altitude (Y=26)!
        p.Spawn(p.DashCharge, inter, 307f, 26f);
        p.Spawn(p.DashCharge, inter, 314f, 10f);                         // Lower waterfall catch charge!
        for (int x = 311; x <= 317; x++)
            for (int y = 8; y <= 35; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Colossal 7x28 = 196 Purple Waterfall!
        GameObject mp4Gl = p.Spawn(p.MovingPlatform, env, 322f, 8f);
        ConfigureMovingPlatform(mp4Gl, new Vector3(5f, 0f, 0f), 3.2f, 0.2f);
        p.Spawn(p.CrumblingBlock, env, 325f, 14f);
        SetAtomId(p.Spawn(p.Atom, col, 314f, 37f), 13);
        SetDiamondId(p.Spawn(p.Diamond, col, 321f, 24f), 3);             // Grand Waterfall Glide Diamond 3!
        p.Spawn(p.DashCharge, inter, 321f, 18f);

        // Mid Recovery Pads under Waterfall + Steep Slope45Large Deflection Kicker (328..338)
        p.Spawn(p.BouncePad, inter, 314f, 0f);
        p.Spawn(p.BouncePadMini, inter, 324f, 0f);
        p.Spawn(p.Slope45Large, env, 330f, 0f);                          // 328..332, 0..4 rises UP-RIGHT!
        p.SpawnGroundSized(env, 332f, 4f, 7);                            // Kicker Apex 332..339, Y=3..4
        GameObject db4Slope = p.Spawn(p.DashBoostFast, inter, 334f, 5.5f);
        ConfigureDashZoneBoost(db4Slope, new Vector2(3.2f, 3.2f), 34f, 0.22f, 22f); // -> X=341.5 into Hold4A!

        // Omni-Directional Finale Barrels + Upper Express Line -> Colossal Finale Monolith -> End_Portal!
        GameObject hold4A = p.Spawn(p.DashHold, inter, 342f, 7f);
        ConfigureDashZoneHold(hold4A, 34f, new Vector2(3.6f, 3.6f), 36f, 0.38f, 22f, 0.22f); // -> ~(353.3, 14.6)!
        for (int x = 345; x <= 350; x++)
            for (int y = 9; y <= 17; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // Finale Cluster #1
        SetAtomId(p.Spawn(p.Atom, col, 348f, 19f), 14);
        GameObject spin4 = p.Spawn(p.SpinArm, haz, 346f, 22f);
        ConfigureSpinArm(spin4, 95f);
        p.Spawn(p.CrumblingBlock, env, 346f, 4.5f);                      // Coupled over LavaLeaper@346!
        p.Spawn(p.LavaLeaper, haz, 346f, 0f);
        p.Spawn(p.BouncePad, inter, 352f, 0f);

        GameObject hold4B = p.Spawn(p.DashHold, inter, 354f, 15f);
        ConfigureDashZoneHold(hold4B, -34f, new Vector2(3.6f, 3.6f), 36f, 0.36f, 22f, 0.22f); // -> ~(364.7, 7.8) into Finisher!
        SetAtomId(p.Spawn(p.Atom, col, 359f, 13.5f), 15);
        p.Spawn(p.LavaLeaper, haz, 358f, 0f);
        p.Spawn(p.CrumblingBlock, env, 360f, 4.2f);                      // Cleanly clear of DashLane_UP@356!
        p.Spawn(p.BouncePadMega, inter, 356f, 0f);
        p.Spawn(p.DashLaneUp, inter, 356f, 8f);

        // LOWER & UPPER DUAL FINALE MONOLITHS (ALL Yellow blocks <= 17m from launchers!)
        // Lower Hyper-Line: DashZone@(362, 1.5) -> Purple (366..368, 1..9) -> 2m Gap -> Double Yellow (371..379, 1..9) -> End_Portal@(383, 1.8)!
        GameObject dz4Low = p.Spawn(p.DashZone, inter, 362f, 1.5f);
        ConfigureDashZoneBoost(dz4Low, new Vector2(3.6f, 3.6f), 38f, 0.56f, 24f); // -> X=383.3!
        for (int x = 366; x <= 368; x++)
            for (int y = 1; y <= 9; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                   // 3x9 Purple Front Monolith!
        for (int y = 1; y <= 9; y++)
        {
            p.Spawn(p.DashWallChainRow1, inter, 371f, y);                // 371..376 (5x9 = 45)
            p.Spawn(p.DashWallCracked, inter, 376.5f, y - 0.5f);         // 376..379 (3x9 = 27)
            p.Spawn(p.DashWallCracked, inter, 377.5f, y - 0.5f);
            p.Spawn(p.DashWallCracked, inter, 378.5f, y - 0.5f);         // 8-deep x 9-tall = 72 Yellow wall cleanly <= 17m!
        }

        // Upper Express Line: DashHold@(362, 15) -> Upper Purple + Yellow Gate -> Atom 16@(380, 14.5) -> drops to End_Portal!
        p.Spawn(p.OneWayPlatformLarge, env, 359f, 12f);                  // 359..367 upper catch!
        GameObject hold4Exp = p.Spawn(p.DashHold, inter, 362f, 15f);
        ConfigureDashZoneHold(hold4Exp, 0f, new Vector2(3.4f, 3.4f), 38f, 0.48f, 22f, 0.22f); // -> X=380.2!
        for (int x = 367; x <= 369; x++)
            for (int y = 13; y <= 18; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);
        for (int y = 13; y <= 18; y++)
            p.Spawn(p.DashWallChainRow1, inter, 372f, y);                // 372..377 Upper Yellow Monolith!
        SetAtomId(p.Spawn(p.Atom, col, 380f, 14.5f), 16);                // Upper Express Line Final Atom!

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
        return $"Level_48 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 78f;
            lm.TwoStarTime = 120f;
            lm.OneStarTime = 200f;
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
            respawnMgr.KillY = -24f;
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
