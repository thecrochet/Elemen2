#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level41Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_41.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 41 (Multi-Direction Barrel Chaos)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 120f, -35f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(360f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(360f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -10 to 260)
        for (int x = -10; x <= 250; x += 20)
            p.Spawn(p.Ground20, env, x, -28f);

        // =====================================================================
        // FLOW: START PORTAL, END PORTAL, PLAYER SPAWN & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 30f, 20f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 231f, 23.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // CAMERA ZONES (Clean non-overlapping 2D mosaic matching each turn!)
        // =====================================================================
        GameObject czZ1Tight = p.Spawn(p.CameraZoneTight, flow, 51f, 15.5f);
        czZ1Tight.name = "CameraZone_Tight_Z1Ramparts";
        SetTriggerBoxSize(czZ1Tight, new Vector3(50f, 23f, 10f));      // X=26..76, Y=4..27

        GameObject czZ1Wide = p.Spawn(p.CameraZoneWide, flow, 109f, 15.5f);
        czZ1Wide.name = "CameraZone_Wide_Z1Gate";
        SetTriggerBoxSize(czZ1Wide, new Vector3(66f, 23f, 10f));       // X=76..142, Y=4..27

        GameObject czZ2Climb = p.Spawn(p.CameraZoneClimb, flow, 151f, 25.5f);
        czZ2Climb.name = "CameraZone_Climb_Z2Tower";
        SetTriggerBoxSize(czZ2Climb, new Vector3(18f, 43f, 10f));      // X=142..160, Y=4..47

        GameObject czZ2Wide = p.Spawn(p.CameraZoneWide, flow, 75f, 37f);
        czZ2Wide.name = "CameraZone_Wide_Z2CathedralLeft";
        SetTriggerBoxSize(czZ2Wide, new Vector3(134f, 20f, 10f));      // X=8..142, Y=27..47

        GameObject czZ3Well = p.Spawn(p.CameraZoneTight, flow, 13f, 3.5f);
        czZ3Well.name = "CameraZone_Tight_Z3BlastWell";
        SetTriggerBoxSize(czZ3Well, new Vector3(26f, 47f, 10f));       // X=0..26, Y=-20..27

        GameObject czZ3Vault = p.Spawn(p.CameraZoneTight, flow, 41f, -0.5f);
        czZ3Vault.name = "CameraZone_Tight_Z3SideVault";
        SetTriggerBoxSize(czZ3Vault, new Vector3(30f, 9f, 10f));       // X=26..56, Y=-5..4

        GameObject czZ4Under = p.Spawn(p.CameraZoneWide, flow, 101f, -12.5f);
        czZ4Under.name = "CameraZone_Wide_Z4UnderPinball";
        SetTriggerBoxSize(czZ4Under, new Vector3(114f, 15f, 10f));     // X=44..158, Y=-20..-5

        GameObject czZ4Spire = p.Spawn(p.CameraZoneWide, flow, 199f, 8f);
        czZ4Spire.name = "CameraZone_Wide_Z4SpireFinale";
        SetTriggerBoxSize(czZ4Spire, new Vector3(82f, 56f, 10f));      // X=158..240, Y=-20..36

        // =====================================================================
        // ZONE 1: THE SHATTERED RAMPART CANNONS (Crooked Down-Right Descent)
        // X = 26 to 128, Y = 8 to 27
        // StartPortal(30,20) -> irregular battlements + false DashWall ->
        // Slope_45_Left + IceSurface -> Slope_45_Large kicker -> DashHold(-25) ->
        // BouncePad -> DashZone smashing giant Purple & DashWall_ChainRow 1 Gate
        // =====================================================================
        p.SpawnBlock(p.BlockCyan, env, 25f, 26f, 2, 44, 4);            // Divides Z1(X=26) from Z3(X=0..25), Y=4..26
        for (int x = 26; x < 128; x += 20)
            p.SpawnCeilingSized(env, x, 27f, Mathf.Min(20, 128 - x));  // Roof Y=26..27
        foreach (float rx in new[] { 34f, 47f, 65f, 82f, 100f, 118f })
            p.SpawnBlock(p.BlockGreen, env, rx, 26f, 2, 2, 4);

        // Stepped Z1 Floors (Y=20 -> Y=16 -> Y=12 -> Y=8)
        p.SpawnGroundSized(env, 26f, 20f, 30);                         // Upper battlements X=26..56, Y=19..20
        p.Spawn(p.Slope45Left, env, 57f, 18f);                         // 56..58, 18..20
        p.Spawn(p.Slope45Left, env, 59f, 16f);                         // 58..60, 16..18
        p.SpawnBlock(p.BlockCyan, env, 56f, 18f, 4, 4, 4);             // Backing 56..58, 16..18
        p.SpawnGroundSized(env, 56f, 16f, 4);                          // 56..60, 15..16
        p.Spawn(p.IceSurface, env, 60f, 16f);                          // 60..66, 15..16
        p.Spawn(p.Slope45Left, env, 67f, 14f);                         // 66..68, 14..16
        p.Spawn(p.Slope45Left, env, 69f, 12f);                         // 68..70, 12..14
        p.SpawnBlock(p.BlockCyan, env, 66f, 14f, 4, 4, 4);             // Backing 66..68, 12..14
        p.SpawnGroundSized(env, 66f, 12f, 14);                         // Mid Runway X=66..80, Y=11..12
        p.Spawn(p.Slope45Large, env, 78f, 12f);                        // Kicker X=76..80, Y=12..16!
        p.Spawn(p.Ground20, env, 80f, 8f);                             // Lower Gate Floor X=80..100, Y=7..8
        p.Spawn(p.Ground20, env, 100f, 8f);                            // X=100..120
        p.SpawnGroundSized(env, 120f, 8f, 8);                          // X=120..128 (meets Sanct 1)

        // Trim walls connecting stepped floors neatly down to Y = 4 ceiling divider of Zone 4!
        p.SpawnBlock(p.BlockCyan, env, 26f, 19f, 2, 30, 4);            // 26..27, Y=4..19
        p.SpawnBlock(p.BlockCyan, env, 55f, 19f, 2, 30, 4);            // 55..56, Y=4..19
        p.SpawnBlock(p.BlockCyan, env, 79f, 11f, 2, 14, 4);            // 79..80, Y=4..11
        p.SpawnBlock(p.BlockCyan, env, 99f, 7f, 2, 6, 4);              // 99..100, Y=4..7
        p.SpawnBlock(p.BlockCyan, env, 119f, 7f, 2, 6, 4);             // 119..120, Y=4..7

        // Z1 Battlement Props, Purple Clusters & Secret Upper Parapet Atom
        p.Spawn(p.BouncePadMini, inter, 33f, 20f);
        p.Spawn(p.OneWayPlatformLarge, env, 34f, 23f);                 // Upper parapet X=34..42
        p.SpawnBlock(p.BlockGreen, env, 36f, 23f, 2, 2, 4);
        p.SpawnTouchWallCol(inter, 36f, 21f, 22f);
        p.Spawn(p.TouchWallBlue, inter, 40f, 21f);
        p.Spawn(p.TouchWallBlue, inter, 40f, 22f);
        p.Spawn(p.TouchWallBlue, inter, 41f, 21f);                     // Irregular L-cluster

        p.Spawn(p.OneWayPlatformLarge, env, 42f, 23f);                 // X=42..50
        p.SpawnBlock(p.BlockMagenta, env, 43f, 26f, 2, 2, 4);          // Upper lintel Y=25..26
        for (int y = 24; y <= 25; y++) p.Spawn(p.DashWall, inter, 43f, y); // False yellow secret door
        p.SpawnBlock(p.BlockMagenta, env, 50f, 26f, 2, 6, 4);          // Vault back wall Y=23..26
        SetAtomId(p.Spawn(p.Atom, col, 46.5f, 24.4f), 1);
        p.Spawn(p.FireJet, haz, 48f, 20f);

        // Rampart Kicker -> Aerial Purple Diamond -> Barrel 1A (-25 deg) -> BouncePad -> DashZone Fortress Gate
        p.Spawn(p.DashBoostFast, inter, 71f, 13.5f);
        // Purple diamond cluster cleanly past kicker (X=81..84, Y=15..19)
        for (int x = 81; x <= 84; x++)
            for (int y = 16; y <= 19; y++)
                if (Mathf.Abs(x - 82.5f) + Mathf.Abs(y - 17.5f) <= 2.1f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject barrel1A = p.Spawn(p.DashHold, inter, 88f, 17.5f);
        ConfigureDashZoneHold(barrel1A, -25f, new Vector2(3.2f, 3.2f), 34f, 0.28f, 22f, 0.22f);

        // Diagonal Purple cascade along Barrel 1A beam
        p.Spawn(p.TouchWallBlue, inter, 91f, 16f);
        p.Spawn(p.TouchWallBlue, inter, 92f, 16f);
        p.Spawn(p.TouchWallBlue, inter, 93f, 15f);
        p.Spawn(p.TouchWallBlue, inter, 94f, 15f);
        p.Spawn(p.TouchWallBlue, inter, 95f, 14f);

        p.Spawn(p.BouncePad, inter, 98f, 8f);                          // Catches Barrel 1A shot cleanly!
        p.Spawn(p.BouncePad, inter, 90f, 8f);                          // Recovery bounce pad

        GameObject dzGate1 = p.Spawn(p.DashZone, inter, 103f, 12f);
        ConfigureDashZoneBoost(dzGate1, new Vector2(3.4f, 3.4f), 36f, 0.44f, 24f);

        p.Spawn(p.TouchWallBlue, inter, 107f, 10f);                    // Hanging Purple curtain before Gate
        p.Spawn(p.TouchWallBlue, inter, 107f, 11f);
        p.Spawn(p.TouchWallBlue, inter, 107f, 12f);
        p.Spawn(p.TouchWallBlue, inter, 107f, 13f);
        p.Spawn(p.TouchWallBlue, inter, 107f, 14f);

        p.SpawnBlock(p.BlockCyan, env, 110f, 26f, 14, 24, 4);          // Fortress Gate header Y=14..26
        p.SpawnTouchWallCol(inter, 110f, 9f, 14f);                     // 6m purple face
        for (int y = 9; y <= 14; y++) p.Spawn(p.DashWallChainRow1, inter, 111f, y); // 5x6 yellow chain gate!!
        p.SpawnTouchWallCol(inter, 116f, 9f, 14f);

        SetAtomId(p.Spawn(p.Atom, col, 120f, 12.5f), 2);
        SetAtomId(p.Spawn(p.Atom, col, 124.5f, 9.5f), 3);

        // =====================================================================
        // SANCTUARY 1: EASTERN BASTION (X = 128 to 142, Y = 8 to 17)
        // =====================================================================
        p.SpawnGroundSized(env, 128f, 8f, 14);                         // X=128..142, Y=7..8
        p.SpawnCeilingSized(env, 128f, 17f, 14);                       // X=128..142, Y=16..17
        p.SpawnBlock(p.BlockCyan, env, 128f, 26f, 2, 18, 4);           // Step Y=17..26
        p.SpawnBlock(p.BlockGreen, env, 129f, 16f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 135f, 16f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 135f, 7f, 2, 6, 4);             // Y=4..7 meets Z4 ceiling
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 135f, 9.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE INVERTED BARREL RAFTERS (Climb UP & Rocket BACKWARDS LEFT!)
        // Tower Ascent (X = 142..158, Y = 8..45, CameraZone_Climb) ->
        // Inverted Gallery RIGHT-TO-LEFT (X = 142 -> 24, Y = 28..45, CameraZone_Wide)
        // =====================================================================
        // Tower Perimeter & Safety Floor (Y=8, X=142..158)
        p.SpawnGroundSized(env, 142f, 8f, 15);                         // 142..157, Y=7..8
        p.SpawnBlock(p.BlockCyan, env, 142f, 7f, 2, 6, 4);             // Meets Z4 ceiling Y=4
        p.SpawnBlock(p.BlockCyan, env, 157f, 44f, 2, 80, 4);           // East Citadel Wall X=157..158, Y=4..44
        p.SpawnCeilingSized(env, 142f, 45f, 16);                       // Tower roof Y=44..45

        // Cramped Twin-Wall Tower Chimney + Vertical DashLane_UP + Barrel 2A (90 deg)
        p.Spawn(p.BouncePad, inter, 149f, 8f);
        p.Spawn(p.DashLaneUp, inter, 149f, 13.5f);                     // Y=9.5..17.5
        p.Spawn(p.Wall12, env, 145f, 28f);                             // Left inner shaft wall Y=16..28
        p.Spawn(p.Wall12, env, 153f, 28f);                             // Right inner shaft wall Y=16..28
        GameObject barrel2A = p.Spawn(p.DashHold, inter, 149f, 20f);
        ConfigureDashZoneHold(barrel2A, 90f, new Vector2(3.2f, 3.2f), 34f, 0.36f, 20f, 0.22f);

        for (int x = 146; x <= 152; x++) p.Spawn(p.TouchWallBlue, inter, x, 24f); // Web 1
        for (int x = 146; x <= 152; x++) p.Spawn(p.DashWall, inter, x, 26f);      // Yellow seal
        for (int x = 146; x <= 152; x++) p.Spawn(p.TouchWallBlue, inter, x, 28f); // Web 2
        p.Spawn(p.OneWayPlatformLarge, env, 145f, 30f);                // Summit catch X=145..153
        SetAtomId(p.Spawn(p.Atom, col, 149f, 32f), 4);
        p.Spawn(p.BouncePadMini, inter, 143f, 28f);

        // Inverted Cathedral Floor (Y = 28, flush atop Z1 roof Y=27!) & Roof (Y = 45)
        for (int x = 22; x < 142; x += 20)
        {
            p.SpawnGroundSized(env, x, 28f, Mathf.Min(20, 142 - x));   // X=22..142, Y=27..28
            p.SpawnCeilingSized(env, x, 45f, Mathf.Min(20, 142 - x));  // Roof Y=44..45
        }
        p.SpawnCeilingSized(env, 8f, 45f, 14);                         // Roof extends over Sanct 2 (8..22)
        p.SpawnBlock(p.BlockCyan, env, 8f, 44f, 2, 14, 4);             // Step down to Sanct 2 roof Y=37..44

        // Backwards Right-to-Left Barrels, Glide Clouds, Hollow Cave, Crumbling & Hazards!
        GameObject dbLeft = p.Spawn(p.DashBoostLeft, inter, 134f, 29.5f);
        ConfigureDashZoneBoost(dbLeft, new Vector2(3.2f, 3.2f), 34f, 0.42f, 22f);

        // Hollow Purple Smash-Cave (X = 117..125, Y = 28..34) enclosing Atom 5!
        for (int x = 117; x <= 125; x++)
        {
            for (int y = 29; y <= 34; y++)
            {
                bool border = x == 117 || x == 125 || y >= 33;
                if (border) p.Spawn(p.TouchWallBlue, inter, x, y);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 121f, 30.2f), 5);

        p.Spawn(p.BouncePadMini, inter, 113f, 28f);
        GameObject barrel2B = p.Spawn(p.DashHold, inter, 107f, 33f);
        ConfigureDashZoneHold(barrel2B, 160f, new Vector2(3.4f, 3.4f), 34f, 0.40f, 22f, 3.5f); // Fires UP-LEFT!

        GameObject sawCathedral = p.Spawn(p.SawSlow, haz, 99f, 33f);
        ConfigureSaw(sawCathedral, new Vector3(0f, 5f, 0f), 2.4f);

        // Dense Hanging Purple Cloud #1 (X = 91..95, Y = 34..40)
        for (int x = 91; x <= 95; x++)
            for (int y = 35; y <= 40; y++)
                if (Mathf.Abs(x - 93f) + Mathf.Abs(y - 37.5f) <= 2.8f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);

        p.Spawn(p.DashCharge, inter, 86f, 38f);                        // Mid-air dash refill heading left!

        // Hanging Purple Cloud #2 (X = 74..79, Y = 33..38) + Diamond 2 High Rafter (Y=41)
        for (int x = 74; x <= 79; x++)
            for (int y = 34; y <= 38; y++)
                if ((x + y) % 2 == 0 || x == 76 || x == 77)
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        p.Spawn(p.OneWayPlatformLarge, env, 73f, 41f);                 // High Cathedral Rafter X=73..81
        SetDiamondId(p.Spawn(p.Diamond, col, 77f, 42.3f), 2);

        GameObject spinCathedral = p.Spawn(p.SpinArm, haz, 66f, 35.5f);
        ConfigureSpinArm(spinCathedral, -85f);

        GameObject barrel2C = p.Spawn(p.DashHold, inter, 58f, 35f);
        ConfigureDashZoneHold(barrel2C, 180f, new Vector2(3.4f, 3.4f), 34f, 0.44f, 22f, 0.25f); // Fires LEFT!

        // Thick Mixed Purple + Cracked Cloud #3 (X = 46..52, Y = 32..38)
        for (int x = 46; x <= 51; x++)
        {
            for (int y = 33; y <= 38; y++)
            {
                if (x >= 48 && x <= 49 && y >= 34 && y <= 36)
                    p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
                else
                    p.Spawn(p.TouchWallBlue, inter, x, y);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 43f, 35f), 6);

        p.Spawn(p.CrumblingBlock, env, 39f, 32f);
        p.Spawn(p.CrumblingBlock, env, 35f, 32f);

        // Far-Left False Yellow DashWall Secret Alcove (X = 24..32, Y = 35..44) -> Atom 7!
        p.Spawn(p.OneWayPlatformLarge, env, 24f, 35f);                 // X=24..32
        p.SpawnBlock(p.BlockMagenta, env, 30f, 44f, 2, 10, 4);         // Lintel Y=39..44
        for (int y = 36; y <= 39; y++) p.Spawn(p.DashWall, inter, 30f, y); // False yellow door!
        p.SpawnBlock(p.BlockMagenta, env, 24f, 44f, 2, 16, 4);         // Back wall Y=36..44
        SetAtomId(p.Spawn(p.Atom, col, 27f, 37.5f), 7);

        // Zone 2 Safety Floor Recovery Pads & Traps (Y = 28)
        p.Spawn(p.FireJet, haz, 108f, 28f);
        p.Spawn(p.BouncePadMega, inter, 102f, 28f);
        p.Spawn(p.FireJet, haz, 80f, 28f);
        p.Spawn(p.BouncePadMega, inter, 66f, 28f);
        p.Spawn(p.FireJet, haz, 44f, 28f);
        p.Spawn(p.BouncePad, inter, 34f, 28f);

        // =====================================================================
        // SANCTUARY 2: WESTERN BALCONY (X = 8 to 22, Y = 28 to 37)
        // =====================================================================
        p.SpawnGroundSized(env, 8f, 28f, 14);                          // X=8..22, Y=27..28
        p.SpawnCeilingSized(env, 8f, 37f, 14);                         // X=8..22, Y=36..37
        p.SpawnBlock(p.BlockMagenta, env, 9f, 36f, 12, 2, 4);
        p.SpawnBlock(p.BlockGreen, env, 15f, 36f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 0f, 44f, 2, 144, 4);            // Grand West Outer Wall X=0..1, Y=-28..44
        p.SpawnCeilingSized(env, 0f, 45f, 8);                          // Completes West Roof over well drop!
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 15f, 29.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE CATACOMB BLAST-WELL (Deep Vertical Ground-Pound & Side Ricochet!)
        // X = 1 to 26 (Well) & X = 26 to 54 (Side-Vault), Y = 28 down to -18
        // =====================================================================
        // Left step-out at Y=28 from Sanct 2 (X=1..8) -> Well mouth
        p.Spawn(p.OneWayPlatformLarge, env, 1f, 28f);                  // X=1..9 (allows drop-through!)

        // Tier A Strata (Y = 21..26 -> Shelf Y = 16)
        for (int x = 1; x <= 10; x++) p.Spawn(p.TouchWallBlue, inter, x, 26f); // Cushion 1
        for (int x = 2; x <= 8; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 24f); // 4x Cracked Y=22..24
        for (int x = 1; x <= 10; x++) p.Spawn(p.TouchWallBlue, inter, x, 21f); // Cushion 2

        p.SpawnGroundSized(env, 1f, 16f, 13);                          // Shelf A X=1..14, Y=15..16
        p.SpawnBlock(p.BlockCyan, env, 13f, 15f, 2, 10, 4);            // Smash-room lintel Y=10..15!
        p.SpawnTouchWallCol(inter, 13f, 5f, 10f);                      // Breakable doorway into smash-pocket!
        p.Spawn(p.BouncePadMini, inter, 4f, 4f);
        for (int x = 5; x <= 10; x++)
            for (int y = 6; y <= 10; y++)
                if (x >= 6 || y <= 8) p.Spawn(p.TouchWallBlue, inter, x, y); // Optional smash crystal cache!
        GameObject stomp3A = p.Spawn(p.StompBlock, haz, 10f, 20.5f);
        ConfigureStompBlock(stomp3A, 0.2f);

        // Tier B Drop (X = 14..25, Y = 16 -> 4): OneWay -> GroundPoundChainRow 2 + Purple cushions -> Y = 4
        p.Spawn(p.OneWayPlatformLarge, env, 14f, 16f);                 // X=14..22
        p.Spawn(p.OneWayPlatformShort, env, 22f, 16f);                 // X=22..24
        for (int x = 15; x <= 23; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, 10f);            // 9m wide x 5m tall crystal vein Y=9..14!
        for (int x = 15; x <= 24; x++) p.Spawn(p.TouchWallBlue, inter, x, 7f);

        // Shelf B (X = 1..25, Y = 4) & Lower Drop (Y = 4 -> -4)
        p.SpawnGroundSized(env, 1f, 4f, 13);                           // 1..14, Y=3..4
        p.Spawn(p.OneWayPlatformLarge, env, 14f, 4f);                  // Drop grate X=14..22
        p.Spawn(p.OneWayPlatformShort, env, 22f, 4f);                  // 22..24
        for (int x = 15; x <= 23; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, 1f); // Cracked stratum Y=-1..1
        for (int x = 15; x <= 24; x++) p.Spawn(p.TouchWallBlue, inter, x, 2f); // Purple layer Y=1..2

        // Tier C Platform (Y = -4) & Sideways Barrel-Ricochet into Secret Diamond Vault (X = 25..54, Y = -4..3)
        p.SpawnGroundSized(env, 12f, -4f, 13);                         // 12..25, Y=-5..-4 (Drop left 1..12!)
        GameObject dbVault = p.Spawn(p.DashBoostFast, inter, 15f, -2.5f);
        ConfigureDashZoneBoost(dbVault, new Vector2(3f, 3f), 34f, 0.40f, 22f);

        p.SpawnGroundSized(env, 25f, -4f, 29);                         // Vault floor 25..54, Y=-5..-4
        p.SpawnBlock(p.BlockMagenta, env, 25f, 4f, 58, 2, 4);          // Vault roof 25..54, Y=3..4 (below Z1 floor!)
        p.SpawnBlock(p.BlockMagenta, env, 53f, 3f, 2, 14, 4);          // Vault back wall Y=-4..3
        p.SpawnBlock(p.BlockMagenta, env, 25f, 3f, 2, 6, 4);           // Lintel Y=0..3
        for (int y = -3; y <= 0; y++) p.Spawn(p.DashWall, inter, 25f, y); // Yellow secret door Y=-4..0!
        for (int x = 32; x <= 35; x++)
            for (int y = -3; y <= 0; y++)
                p.Spawn(p.TouchWallBlue, inter, x, y);                 // Treasure cave purple block fill!

        GameObject stomp3Vault = p.Spawn(p.StompBlock, haz, 29f, 0.5f);
        ConfigureStompBlock(stomp3Vault, 0.3f);
        GameObject leaperVault = p.Spawn(p.LavaLeaper, haz, 39f, -4f); // Crests +4m under Y=3 roof cleanly
        ConfigureLavaLeaper(leaperVault, 4.0f);
        SetAtomId(p.Spawn(p.Atom, col, 43.5f, -2.5f), 8);
        SetAtomId(p.Spawn(p.Atom, col, 47f, -2.5f), 9);
        SetDiamondId(p.Spawn(p.Diamond, col, 50.5f, -2.5f), 1);

        // Final Deep Well Drop (X = 1..12, Y = -4 down to -18)
        p.Spawn(p.OneWayPlatformLarge, env, 1f, -4f);                  // 1..9
        p.Spawn(p.OneWayPlatformShort, env, 9f, -4f);                  // 9..11
        for (int x = 1; x <= 11; x++) p.Spawn(p.TouchWallBlue, inter, x, -6f);
        for (int x = 1; x <= 11; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, -12f);           // 11-wide crystal seam Y=-13..-8!
        for (int x = 1; x <= 11; x++) p.Spawn(p.TouchWallBlue, inter, x, -15f);

        // Catacomb Bottom Floor (X = 1..28, Y = -18)
        p.Spawn(p.Ground20, env, 1f, -18f);
        p.SpawnGroundSized(env, 21f, -18f, 7);                         // Meets Sanct 3 at X=28
        p.SpawnBlock(p.BlockCyan, env, 12f, -19f, 2, 18, 4);
        p.Spawn(p.LavaLeaper, haz, 17f, -18f);
        SetAtomId(p.Spawn(p.Atom, col, 22f, -16.5f), 10);

        // =====================================================================
        // SANCTUARY 3: ABYSSAL FOUNDRY (X = 28 to 42, Y = -18 to -9)
        // =====================================================================
        p.SpawnGroundSized(env, 28f, -18f, 14);                        // X=28..42, Y=-19..-18
        p.SpawnCeilingSized(env, 28f, -9f, 14);                        // X=28..42, Y=-10..-9
        p.SpawnBlock(p.BlockGreen, env, 29f, -10f, 12, 2, 4);
        p.SpawnBlock(p.BlockMagenta, env, 35f, -10f, 12, 2, 4);
        p.SpawnBlock(p.BlockCyan, env, 35f, -19f, 2, 18, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 35f, -16.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE CHAOS CITADEL PINBALL FINALE (Diagonal Up-Right Hyper-Chain!)
        // X = 42 to 238, Y = -18 up to 34
        // Lower Under-Gauntlet (42..156, Y=-18..4) -> Rising Diagonal Spire (156..238, -18..34)
        // =====================================================================
        // Continuous Under-Citadel Ceiling (54..156 at Y=4 -> below Z1 floor!) & Open Spire Roof (158..238 at Y=34)
        p.SpawnBlock(p.BlockCyan, env, 54f, 3f, 2, 14, 4);             // Step wall behind Z3 Vault X=54..55, Y=-4..3
        for (int x = 54; x < 156; x += 20)
            p.SpawnCeilingSized(env, x, 4f, Mathf.Min(20, 156 - x));   // Roof Y=3..4 cleanly below Z1!
        for (int x = 158; x < 238; x += 20)
            p.SpawnCeilingSized(env, x, 34f, Mathf.Min(20, 238 - x));  // Grand Hall Roof Y=33..34
        p.SpawnBlock(p.BlockCyan, env, 238f, 34f, 2, 124, 4);          // East Finale Back Wall Y=-28..34

        // Continuous Recovery Safety Floor across ALL of Zone 4 (X = 42..238, Y = -18)
        for (int x = 42; x < 238; x += 20)
            p.SpawnGroundSized(env, x, -18f, Mathf.Min(20, 238 - x));
        foreach (float px in new[] { 60f, 80f, 100f, 120f, 140f, 160f, 180f, 200f, 220f })
            p.SpawnBlock(p.BlockCyan, env, px, -19f, 2, 18, 4);        // Piers Y=-28..-19

        // 4A: Under-Citadel Pinball Entry (X=42..100, Y=-18..-6)
        GameObject dbZ4Entry = p.Spawn(p.DashBoostFast, inter, 48f, -16.5f);
        ConfigureDashZoneBoost(dbZ4Entry, new Vector2(3.2f, 3.2f), 34f, 0.38f, 22f);

        p.Spawn(p.Slope45Large, env, 57f, -18f);                       // Kicker X=55..59, Y=-18..-14
        // Purple diamond cluster above Slope_45_Large
        for (int x = 60; x <= 63; x++)
            for (int y = -13; y <= -9; y++)
                if (Mathf.Abs(x - 61.5f) + Mathf.Abs(y + 11f) <= 2.2f)
                    p.Spawn(p.TouchWallBlue, inter, x, y);

        GameObject barrel4A = p.Spawn(p.DashHold, inter, 68f, -10f);
        ConfigureDashZoneHold(barrel4A, 20f, new Vector2(3.2f, 3.2f), 34f, 0.42f, 24f, 0.22f);

        // Aerial Monolith 4A (X=73..79, Y=-9..-4): DashWall_ChainRow 1 + TouchWall_Blue curtains
        p.Spawn(p.TouchWallBlue, inter, 73f, -8f);
        p.Spawn(p.TouchWallBlue, inter, 73f, -7f);
        p.Spawn(p.TouchWallBlue, inter, 73f, -6f);
        for (int y = -7; y <= -4; y++) p.Spawn(p.DashWallChainRow1, inter, 74f, y); // 5x4 yellow rows!
        p.Spawn(p.TouchWallBlue, inter, 79f, -7f);
        p.Spawn(p.TouchWallBlue, inter, 79f, -6f);
        SetAtomId(p.Spawn(p.Atom, col, 82f, -5.5f), 11);

        // Mid-Under Pinball Transfer (Y=-12 -> -8): MovingPlatform + Cannon_Fast + DashZone + Monolith 4B
        p.SpawnGroundSized(env, 83f, -12f, 10);                        // X=83..93, Y=-13..-12
        p.SpawnBlock(p.BlockCyan, env, 83f, -13f, 2, 10, 4);
        GameObject mp4 = p.Spawn(p.MovingPlatform, env, 94f, -11f);
        ConfigureMovingPlatform(mp4, new Vector3(6f, 0f, 0f), 3.4f, 0.3f); // X=94..100

        p.SpawnGroundSized(env, 101f, -12f, 25);                       // Under-Track X=101..126, Y=-13..-12
        p.SpawnBlock(p.BlockCyan, env, 101f, -13f, 2, 10, 4);
        p.SpawnBlock(p.BlockCyan, env, 125f, -13f, 2, 10, 4);

        GameObject dzZ4Mid = p.Spawn(p.DashZone, inter, 104f, -10.5f);
        ConfigureDashZoneBoost(dzZ4Mid, new Vector2(3.2f, 3.2f), 36f, 0.44f, 24f);

        p.SpawnTouchWallCol(inter, 108f, -11f, -7f);
        for (int y = -11; y <= -7; y++) p.Spawn(p.DashWallChainRow1, inter, 109f, y); // 5x5 chain monolith!
        p.SpawnTouchWallCol(inter, 114f, -11f, -7f);
        SetAtomId(p.Spawn(p.Atom, col, 117f, -10.5f), 12);

        p.Spawn(p.Slope45Large, env, 124f, -12f);                      // Kicker X=122..126, Y=-12..-8
        p.SpawnGroundSized(env, 126f, -8f, 12);                        // Upper Under-Track X=126..138
        p.SpawnBlock(p.BlockCyan, env, 126f, -9f, 2, 18, 4);

        GameObject mp4Exit = p.Spawn(p.MovingPlatform, env, 141f, -8f);
        ConfigureMovingPlatform(mp4Exit, new Vector3(7f, 0f, 0f), 3.5f, 0.3f); // Transfers cleanly out past 148!
        p.SpawnBlock(p.BlockMagenta, env, 152f, -4f, 4, 1, 4);         // Cannon bracket 152..154, Y=-4.5..-4
        GameObject cannonUnder = p.Spawn(p.CannonFast, haz, 153f, -3.2f);
        ConfigureCannon(cannonUnder, new Vector3(0f, 0f, 180f), 10f, 2.3f, 22f);

        // 4B: Rising Diagonal Up-Right Spire Hyper-Chain (X=154..238, Y=-10 up to +24!)
        p.Spawn(p.OneWayPlatformLarge, env, 149f, -10f);               // Open underside recovery hatch X=149..157!
        p.SpawnGroundSized(env, 157f, -10f, 7);                        // Launch Dais X=157..164, Y=-11..-10
        p.SpawnBlock(p.BlockCyan, env, 157f, -11f, 2, 14, 4);
        GameObject dbSpire = p.Spawn(p.DashBoostFast, inter, 157f, -8.5f);
        ConfigureDashZoneBoost(dbSpire, new Vector2(3.2f, 3.2f), 34f, 0.32f, 24f);

        GameObject barrel4B = p.Spawn(p.DashHold, inter, 165f, -7f);
        ConfigureDashZoneHold(barrel4B, 45f, new Vector2(3.4f, 3.4f), 36f, 0.44f, 24f, 0.22f); // Launches UP-RIGHT 45 deg!

        // Diagonal Purple+Cracked Cluster along Barrel 4B beam
        for (int x = 169; x <= 172; x++)
            for (int y = -3; y <= 2; y++)
                if (Mathf.Abs((x - 169f) - (y + 3f) * 0.8f) <= 1.8f)
                    p.Spawn((x + y) % 2 == 0 ? p.TouchWallBlue : p.DashWallCracked, inter,
                        (x + y) % 2 == 0 ? x : x + 0.5f, (x + y) % 2 == 0 ? y : y - 0.5f);
        SetAtomId(p.Spawn(p.Atom, col, 174f, 2f), 13);

        GameObject barrel4C = p.Spawn(p.DashHold, inter, 177f, 5f);
        ConfigureDashZoneHold(barrel4C, 55f, new Vector2(3.6f, 3.6f), 34f, 0.44f, 24f, 0.20f); // Launches 55 deg UP-RIGHT!

        p.SpawnTouchWallCol(inter, 181f, 10f, 15f);                    // Hanging purple smash curtain

        // Pinnacle DashLane_UP + DashBoost_Up -> Diamond 3 + Barrel 4D (-12 deg)
        p.Spawn(p.DashLaneUp, inter, 186f, 17f);                       // Y=13..21 catches 55 deg beam!
        GameObject boost4Up = p.Spawn(p.DashBoostUp, inter, 186f, 22.5f); // Y=21..24
        ConfigureDashZoneBoost(boost4Up, new Vector2(3.4f, 3.4f), 34f, 0.24f, 22f);
        for (int x = 183; x <= 189; x++) p.Spawn(p.TouchWallBlue, inter, x, 25f);
        SetDiamondId(p.Spawn(p.Diamond, col, 186f, 26.8f), 3);

        GameObject barrel4D = p.Spawn(p.DashHold, inter, 186f, 29.5f);
        ConfigureDashZoneHold(barrel4D, -14f, new Vector2(3.6f, 3.6f), 36f, 0.42f, 26f, 0.20f); // Finale cannon shot!
        SetAtomId(p.Spawn(p.Atom, col, 193f, 27.6f), 14);

        // Towering Grand Finale Monolith 4C (X=198..205, Y=19..28: TouchWall + DashWall_ChainRow 1 x8 + TouchWall!)
        p.Spawn(p.OneWayPlatformLarge, env, 197f, 19f);                // Support X=197..205
        p.SpawnTouchWallCol(inter, 198f, 20f, 28f);
        for (int y = 20; y <= 28; y++) p.Spawn(p.DashWallChainRow1, inter, 199f, y); // 5x9 = 45 yellow chain blocks!!
        p.SpawnTouchWallCol(inter, 204f, 20f, 28f);
        SetAtomId(p.Spawn(p.Atom, col, 207f, 23f), 15);

        // Final Kicker Approach (Y=18 -> Slope_45_Large -> Monolith 4D -> Victory Dais Y=22 -> End_Portal!)
        p.SpawnGroundSized(env, 206f, 18f, 12);                        // X=206..218, Y=17..18
        p.SpawnBlock(p.BlockCyan, env, 206f, 17f, 2, 70, 4);           // Pier down to -18!
        GameObject dzFinalKicker = p.Spawn(p.DashZone, inter, 210f, 19.5f);
        ConfigureDashZoneBoost(dzFinalKicker, new Vector2(3.2f, 3.2f), 34f, 0.38f, 24f);
        p.Spawn(p.Slope45Large, env, 216f, 18f);                       // Kicker X=214..218, Y=18..22

        p.SpawnGroundSized(env, 218f, 22f, 20);                        // Royal Throne Dais X=218..238, Y=21..22
        p.SpawnBlock(p.BlockMagenta, env, 218f, 21f, 2, 78, 4);        // Dais pier down to -18!

        // Monolith 4D right before End_Portal (X = 221..224, Y = 22..27)
        for (int x = 221; x <= 224; x++)
        {
            for (int y = 23; y <= 27; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWallCracked, inter, x + 0.5f, y - 0.5f);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 226.5f, 23.5f), 16);
        p.SpawnBlock(p.BlockGreen, env, 226f, 33f, 24, 2, 4);          // Victory canopy trim

        // Recovery Catch-Lanes across ALL of Zone 4 (Floor Y = -18)
        p.Spawn(p.BouncePadMega, inter, 72f, -18f);
        p.Spawn(p.BouncePadMega, inter, 98f, -18f);
        p.Spawn(p.BouncePadMega, inter, 134f, -18f);
        p.Spawn(p.BouncePad, inter, 152f, -18f);
        p.Spawn(p.DashLaneUp, inter, 152f, -13.5f);                    // Pops straight through 149..157 OneWay onto Dais!
        p.Spawn(p.BouncePadMega, inter, 174f, -18f);
        p.Spawn(p.OneWayPlatform, env, 174f, -5f);
        p.Spawn(p.BouncePadMega, inter, 198f, -18f);
        p.Spawn(p.BouncePad, inter, 201f, -18f);
        p.Spawn(p.DashLaneUp, inter, 201f, -6f);
        GameObject boostRec = p.Spawn(p.DashBoostUp, inter, 201f, 4f);
        ConfigureDashZoneBoost(boostRec, new Vector2(3f, 3f), 34f, 0.30f, 22f);
        p.Spawn(p.OneWayPlatform, env, 199f, 12f);
        p.Spawn(p.BouncePadMini, inter, 201f, 12f);                    // Springs cleanly through 197..205 OneWay -> Final Kicker!

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
        return $"Level_41 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 62f;
            lm.TwoStarTime = 95f;
            lm.OneStarTime = 160f;
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
