#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level38Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_38.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 38 (Slumbering Sanctuary & Crystal Core)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 184f, -35f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(480f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(480f, 4f, 1f);

        // Continuous Bedrock Base at Y = -28 (X = -10 to 390)
        for (int x = -10; x <= 370; x += 20)
            p.Spawn(p.Ground20, env, x, -28f);

        // =====================================================================
        // FLOW: START PORTAL, END PORTAL, PLAYER SPAWN & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 4f, 18f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 363f, 13.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // ZONE 1: THE SLUMBERING ANTECHAMBER (X = 0 to 76, Y = 18 to 32)
        // Symmetrical two-story temple halls, secret Yellow DashWall side-walls,
        // early Atom alcove, and upper DashCharge double-dash shortcut
        // =====================================================================
        GameObject czWideEntry = p.Spawn(p.CameraZoneWide, flow, 15.5f, 24f);
        czWideEntry.name = "CameraZone_Wide_Antechamber";
        SetTriggerBoxSize(czWideEntry, new Vector3(33f, 20f, 10f));

        GameObject czTightPassage = p.Spawn(p.CameraZoneTight, flow, 54f, 24f);
        czTightPassage.name = "CameraZone_Tight_Temple";
        SetTriggerBoxSize(czTightPassage, new Vector3(44f, 20f, 10f));

        // Left Temple Perimeter Wall (X = -1..0, Y = -28..32)
        p.SpawnBlock(p.BlockCyan, env, -1f, 32f, 2, 120, 4);

        // Upper Temple Roof (X = 0..76, Y = 31..32) & Symmetrical Ceiling Ribs
        p.Spawn(p.Ceiling20, env, 0f, 32f);
        p.Spawn(p.Ceiling20, env, 20f, 32f);
        p.Spawn(p.Ceiling20, env, 40f, 32f);
        p.SpawnCeilingSized(env, 60f, 32f, 16);
        foreach (float rx in new[] { 8f, 16f, 28f, 36f, 44f, 56f, 68f })
            p.SpawnBlock(p.BlockGreen, env, rx, 31f, 2, 2, 4);  // Y=30..31 trim ribs

        // Main Nave Floor (X = 0..76, Y = 17..18) + Deep Temple Columns down to Y = -28
        p.Spawn(p.Ground20, env, 0f, 18f);
        p.Spawn(p.Ground20, env, 20f, 18f);
        p.Spawn(p.Ground20, env, 40f, 18f);
        p.SpawnGroundSized(env, 60f, 18f, 16);
        foreach (float px in new[] { 0f, 18f, 36f, 54f, 75f })
            p.SpawnBlock(p.BlockCyan, env, px, 17f, 2, 90, 4); // Y=-28..17

        // Doorway #1 (X = 12): Purple touch-block doorway under choir balcony
        p.Spawn(p.OneWayPlatformLarge, env, 8f, 24f);          // Balcony X=8..16
        p.Spawn(p.BouncePadMini, inter, 8f, 18f);
        p.SpawnBlock(p.BlockGreen, env, 12f, 24f, 2, 6, 4);    // Lintel Y=21..24
        p.SpawnTouchWallCol(inter, 12f, 19f, 21f);             // Purple door Y=18..21

        // Secret Reliquary Alcove (X = 18..26, Y = 24..31): DashWall entrance -> FireJet_Down -> 2 Atoms
        p.Spawn(p.OneWayPlatformLarge, env, 16f, 24f);         // X=16..24
        p.Spawn(p.OneWayPlatformLarge, env, 24f, 24f);         // X=24..32
        p.SpawnBlock(p.BlockMagenta, env, 18f, 31f, 2, 8, 4);  // Left lintel X=18..19, Y=27..31
        for (int y = 25; y <= 27; y++) p.Spawn(p.DashWall, inter, 18f, y); // False Yellow wall Y=24..27
        p.Spawn(p.FireJetDown, haz, 22f, 31f);
        SetAtomId(p.Spawn(p.Atom, col, 20.5f, 25.5f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 24f, 25.5f), 2);
        p.SpawnBlock(p.BlockMagenta, env, 26f, 31f, 2, 8, 4);  // Right lintel X=26..27, Y=27..31
        p.SpawnTouchWallCol(inter, 26f, 25f, 27f);             // Exit purple curtain Y=24..27

        // Lower Nave Gauntlet (X = 22..72, Y = 18): FireJets, Purple Doors & Saw_Slow patrols
        p.Spawn(p.FireJet, haz, 23f, 18f);
        p.SpawnBlock(p.BlockGreen, env, 28f, 24f, 2, 6, 4);    // Lintel Y=21..24
        p.SpawnTouchWallCol(inter, 28f, 19f, 21f);             // Door #2 Y=18..21
        GameObject sawZone1A = p.Spawn(p.SawSlow, haz, 35f, 19.2f);
        ConfigureSaw(sawZone1A, new Vector3(6f, 0f, 0f), 2.4f);
        p.Spawn(p.FireJet, haz, 48f, 18f);
        GameObject sawZone1B = p.Spawn(p.SawSlow, haz, 56f, 19.2f);
        ConfigureSaw(sawZone1B, new Vector3(6f, 0f, 0f), 2.6f);
        p.Spawn(p.OneWayPlatform, env, 64f, 21f);
        p.SpawnBlock(p.BlockGreen, env, 69f, 24f, 2, 6, 4);    // Lintel Y=21..24 (flush below upper lane)
        p.SpawnTouchWallCol(inter, 69f, 19f, 21f);             // Exit Door #3 Y=18..21

        // Upper DashCharge Shortcut (X = 32..70, Y = 24..31)
        p.Spawn(p.Ledge8, env, 32f, 24f);                      // X=32..36
        p.Spawn(p.FireJet, haz, 34f, 24f);
        p.Spawn(p.Ledge8, env, 38f, 24f);                      // X=38..42
        p.Spawn(p.BouncePadMini, inter, 39f, 24f);
        p.Spawn(p.DashCharge, inter, 45f, 26f);                // Refreshes extra mid-air dash
        p.SpawnBlock(p.BlockCyan, env, 50f, 31f, 2, 8, 4);     // Veil 1 header Y=27..31
        for (int y = 25; y <= 27; y++) p.Spawn(p.DashWall, inter, 50f, y);
        SetAtomId(p.Spawn(p.Atom, col, 54f, 26f), 3);
        p.Spawn(p.Ledge8, env, 54f, 24f);                      // X=54..58
        p.SpawnBlock(p.BlockCyan, env, 60f, 31f, 2, 8, 4);     // Veil 2 header Y=27..31
        for (int y = 25; y <= 27; y++) p.Spawn(p.DashWall, inter, 60f, y);
        SetAtomId(p.Spawn(p.Atom, col, 64f, 26f), 4);
        p.Spawn(p.OneWayPlatformLarge, env, 62f, 24f);         // X=62..70

        // =====================================================================
        // SANCTUARY 1: SANCTUM GALLERY (X = 76 to 90, Y = 18 to 26)
        // =====================================================================
        p.SpawnGroundSized(env, 76f, 18f, 14);                 // X=76..90, Y=17..18
        p.SpawnCeilingSized(env, 76f, 26f, 14);                // X=76..90, Y=25..26
        p.SpawnBlock(p.BlockCyan, env, 76f, 31f, 2, 10, 4);    // Vertical step X=76..77, Y=26..31
        p.SpawnBlock(p.BlockGreen, env, 77f, 25f, 2, 4, 4);
        p.SpawnBlock(p.BlockGreen, env, 87f, 25f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 83f, 17f, 2, 90, 4);    // Pier Y=-28..17
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 83f, 19.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE CRYSTAL QUARRY DESCENT (X = 90 to 176, Y = -18 to 26)
        // Multi-tiered Ground Pound plunge through GroundPoundChainRow 2,
        // CrackedBlocks_4x4, Slope_45_Left chutes, IceSurface, StompBlock & Secret Diamond 1
        // =====================================================================
        GameObject czQuarryUpper = p.Spawn(p.CameraZoneWide, flow, 134f, 17.5f);
        czQuarryUpper.name = "CameraZone_Wide_QuarryUpper";
        SetTriggerBoxSize(czQuarryUpper, new Vector3(88f, 19f, 10f)); // X=90..178, Y=8..27

        GameObject czQuarryMid = p.Spawn(p.CameraZoneWide, flow, 113f, 0f);
        czQuarryMid.name = "CameraZone_Wide_QuarryMid";
        SetTriggerBoxSize(czQuarryMid, new Vector3(46f, 16f, 10f));   // X=90..136, Y=-8..8

        GameObject czTightVault = p.Spawn(p.CameraZoneTight, flow, 157f, 1f);
        czTightVault.name = "CameraZone_Tight_CrystalVault";
        SetTriggerBoxSize(czTightVault, new Vector3(42f, 14f, 10f));  // X=136..178, Y=-6..8

        GameObject czQuarryDeep = p.Spawn(p.CameraZoneWide, flow, 157f, -13f);
        czQuarryDeep.name = "CameraZone_Wide_QuarryDeep";
        SetTriggerBoxSize(czQuarryDeep, new Vector3(42f, 14f, 10f));  // X=136..178, Y=-20..-6

        // Quarry Ceiling (X = 90..176, Y = 26) & Right Containment Wall (X = 175)
        for (int x = 90; x < 176; x += 20)
            p.SpawnCeilingSized(env, x, 26f, Mathf.Min(20, 176 - x));
        p.SpawnBlock(p.BlockCyan, env, 175f, 25f, 2, 68, 4);   // Right wall X=175..176, Y=-9..25

        // Tier A: Slope_45_Left x2 -> IceSurface + StompBlock -> Excavation Shaft #1 (Y = 18 -> 6)
        p.SpawnGroundSized(env, 90f, 18f, 1);                  // X=90..91, Y=17..18
        p.Spawn(p.Slope45Left, env, 92f, 16f);                 // X=91..93, Y=16..18
        p.Spawn(p.Slope45Left, env, 94f, 14f);                 // X=93..95, Y=14..16
        p.SpawnBlock(p.BlockCyan, env, 90f, 17f, 2, 22, 4);    // X=90..91, Y=6..17
        p.SpawnBlock(p.BlockCyan, env, 91f, 16f, 4, 20, 4);    // X=91..93, Y=6..16
        p.SpawnBlock(p.BlockCyan, env, 93f, 14f, 4, 16, 4);    // X=93..95, Y=6..14

        p.Spawn(p.IceSurface, env, 95f, 14f);                  // X=95..101, Y=13..14
        p.Spawn(p.Ledge8, env, 101f, 14f);                     // X=101..105, Y=13..14
        GameObject stomp2A = p.Spawn(p.StompBlock, haz, 99f, 18.5f);
        ConfigureStompBlock(stomp2A, 0.1f);

        // Shaft #1 (X = 105..115, Y = 14 down to 6)
        p.SpawnBlock(p.BlockCyan, env, 104f, 13f, 2, 14, 4);   // Left wall X=104..105, Y=6..13
        p.SpawnBlock(p.BlockCyan, env, 115f, 25f, 2, 30, 4);   // Right baffle X=115..116, Y=10..25 (4m doorway below)
        p.Spawn(p.OneWayPlatformLarge, env, 105f, 14f);        // X=105..113
        p.Spawn(p.OneWayPlatformShort, env, 113f, 14f);        // X=113..115

        // Strata inside Shaft #1: GroundPoundChainRow 2 flanks + CrackedBlocks_4x4 core + TouchWall cushion
        p.Spawn(p.GroundPoundChainRow2, inter, 105f, 7f);      // X=105..106, Y=6..11
        p.Spawn(p.GroundPoundChainRow2, inter, 114f, 7f);      // X=114..115, Y=6..11
        p.Spawn(p.CrackedBlocks4x4, inter, 106f, 12f);         // X=106..108, Y=10..12
        p.Spawn(p.CrackedBlocks4x4, inter, 108f, 12f);         // X=108..110, Y=10..12
        p.Spawn(p.CrackedBlocks4x4, inter, 110f, 12f);         // X=110..112, Y=10..12
        p.Spawn(p.CrackedBlocks4x4, inter, 112f, 12f);         // X=112..114, Y=10..12
        for (int x = 106; x <= 113; x++) p.Spawn(p.TouchWallBlue, inter, x, 9f); // Shock cushion Y=8..9

        // Quarry Shelf B Floor (X = 90..120, Y = 5..6) & Piers
        p.Spawn(p.Ground20, env, 90f, 6f);
        p.SpawnGroundSized(env, 110f, 6f, 10);                 // Ends cleanly at X=120
        foreach (float px in new[] { 90f, 104f, 119f })
            p.SpawnBlock(p.BlockCyan, env, px, 5f, 2, 66, 4);  // Piers Y=-28..5
        SetAtomId(p.Spawn(p.Atom, col, 117.5f, 7.5f), 5);

        // Tier B Slide: Slope_45_Left x2 (Y=6->2) -> IceSurface + StompBlock -> Shaft #2 (Y = 2 -> -8)
        p.Spawn(p.Slope45Left, env, 121f, 4f);                 // X=120..122, Y=4..6
        p.Spawn(p.Slope45Left, env, 123f, 2f);                 // X=122..124, Y=2..4
        p.SpawnBlock(p.BlockCyan, env, 120f, 4f, 4, 4, 4);     // Solid backing X=120..122, Y=2..4
        p.SpawnGroundSized(env, 120f, 2f, 4);                  // Sub-ramp floor X=120..124, Y=1..2
        p.SpawnBlock(p.BlockCyan, env, 120f, 1f, 2, 58, 4);    // Pier X=120..121, Y=-28..1

        p.Spawn(p.IceSurface, env, 124f, 2f);                  // X=124..130, Y=1..2
        p.Spawn(p.Ledge8, env, 130f, 2f);                      // X=130..134, Y=1..2
        GameObject stomp2B = p.Spawn(p.StompBlock, haz, 127f, 6.5f);
        ConfigureStompBlock(stomp2B, 0.35f);

        // Shaft #2 (X = 134..144, Y = 2 down to -8) & Secret Diamond 1 Vault (X = 144..164, Y = -3..4)
        p.Spawn(p.OneWayPlatformLarge, env, 134f, 2f);         // X=134..142
        p.Spawn(p.OneWayPlatformShort, env, 142f, 2f);         // X=142..144
        p.SpawnBlock(p.BlockCyan, env, 133f, 1f, 2, 20, 4);    // Left shaft wall X=133..134, Y=-9..1 (flush w/ pier)
        p.SpawnBlock(p.BlockCyan, env, 144f, 25f, 2, 42, 4);   // Upper quarry seal X=144..145, Y=4..25
        for (int x = 134; x <= 143; x++) p.Spawn(p.DashWall, inter, x, 1f); // Layer 1 seal Y=0..1

        // Mid-Shaft Shelf (Y = -3) leading to Secret Crystal Vault behind DashWall!
        p.Spawn(p.OneWayPlatformLarge, env, 134f, -3f);        // X=134..142
        p.Spawn(p.OneWayPlatformShort, env, 142f, -3f);        // X=142..144

        // Secret Diamond 1 Crystal Vault (X = 144..164, Y = -3..4)
        p.SpawnGroundSized(env, 144f, -3f, 20);                // Vault floor X=144..164, Y=-4..-3
        p.SpawnBlock(p.BlockMagenta, env, 144f, 4f, 40, 2, 4); // Vault ceiling X=144..164, Y=3..4
        p.SpawnBlock(p.BlockMagenta, env, 163f, 3f, 2, 12, 4); // Vault back wall X=163..164, Y=-3..3
        p.SpawnBlock(p.BlockMagenta, env, 144f, 3f, 2, 6, 4);  // Vault door lintel X=144..145, Y=0..3
        for (int y = -2; y <= 0; y++) p.Spawn(p.DashWall, inter, 144f, y); // Yellow secret door Y=-3..0!
        GameObject sawVault = p.Spawn(p.SawSlow, haz, 150f, -1.8f);
        ConfigureSaw(sawVault, new Vector3(5f, 0f, 0f), 2.5f);
        p.Spawn(p.FireJet, haz, 157f, -3f);
        SetAtomId(p.Spawn(p.Atom, col, 154f, 0.5f), 6);
        SetDiamondId(p.Spawn(p.Diamond, col, 161f, -1.0f), 1);

        // Lower half of Shaft #2 (Y = -3 down to -8)
        for (int x = 134; x <= 143; x++) p.Spawn(p.TouchWallBlue, inter, x, -4f); // Cushion Y=-5..-4
        for (int x = 136; x <= 140; x += 2) p.Spawn(p.CrackedBlocks4x4, inter, x, -5f); // Core X=136..142, Y=-7..-5
        for (int y = -6; y <= -5; y++)
        {
            p.Spawn(p.DashWallPoundOnly, inter, 134f, y);
            p.Spawn(p.DashWallPoundOnly, inter, 135f, y);
            p.Spawn(p.DashWallPoundOnly, inter, 142f, y);
            p.Spawn(p.DashWallPoundOnly, inter, 143f, y);
        }

        // Tier C Floor (Y = -8) & Downhill Chute -> Final Excavation Shaft #3 (Y = -12 -> -18)
        p.SpawnGroundSized(env, 134f, -8f, 12);                // X=134..146, Y=-9..-8
        p.SpawnBlock(p.BlockCyan, env, 133f, -9f, 2, 38, 4);   // Pier X=133..134, Y=-28..-9
        p.SpawnBlock(p.BlockCyan, env, 145f, -9f, 2, 38, 4);   // Pier X=145..146, Y=-28..-9
        SetAtomId(p.Spawn(p.Atom, col, 143f, -6.5f), 7);

        p.Spawn(p.Slope45Left, env, 147f, -10f);               // X=146..148, Y=-10..-8
        p.Spawn(p.Slope45Left, env, 149f, -12f);               // X=148..150, Y=-12..-10
        p.SpawnBlock(p.BlockCyan, env, 146f, -10f, 4, 4, 4);   // Backing X=146..148, Y=-12..-10
        p.SpawnGroundSized(env, 146f, -12f, 4);                // X=146..150, Y=-13..-12

        p.Spawn(p.IceSurface, env, 150f, -12f);                // X=150..156, Y=-13..-12
        p.SpawnGroundSized(env, 156f, -12f, 2);                // X=156..158, Y=-13..-12
        GameObject stomp2C = p.Spawn(p.StompBlock, haz, 153f, -7.5f);
        ConfigureStompBlock(stomp2C, 0.25f);

        // Shaft #3 (X = 158..168, Y = -12 down to -18)
        p.SpawnBlock(p.BlockCyan, env, 157f, -13f, 2, 10, 4);  // Left wall X=157..158, Y=-18..-13
        p.SpawnBlock(p.BlockCyan, env, 168f, 25f, 2, 78, 4);   // Right quarry shaft wall X=168..169, Y=-14..25 (4m doorway -18..-14)
        p.Spawn(p.OneWayPlatformLarge, env, 158f, -12f);       // X=158..166
        p.Spawn(p.OneWayPlatformShort, env, 166f, -12f);       // X=166..168

        // Full-width GroundPoundChainRow 2 crystal seam (Y = -17..-12)
        for (int x = 158; x <= 167; x++)
            p.Spawn(p.GroundPoundChainRow2, inter, x, -16f);

        // Quarry Core Floor (X = 157..176, Y = -18)
        p.SpawnGroundSized(env, 157f, -18f, 19);               // X=157..176, Y=-19..-18
        foreach (float px in new[] { 157f, 174f })
            p.SpawnBlock(p.BlockCyan, env, px, -19f, 2, 18, 4); // Piers Y=-28..-19
        SetAtomId(p.Spawn(p.Atom, col, 171f, -16.5f), 8);

        // =====================================================================
        // SANCTUARY 2: DEEP CORE CHAMBER (X = 176 to 190, Y = -18 to -9)
        // =====================================================================
        p.SpawnGroundSized(env, 176f, -18f, 14);               // X=176..190, Y=-19..-18
        p.SpawnCeilingSized(env, 176f, -9f, 14);               // X=176..190, Y=-10..-9
        p.SpawnBlock(p.BlockMagenta, env, 177f, -10f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 187f, -10f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 183f, -19f, 2, 18, 4);  // Pier Y=-28..-19
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 183f, -16.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE AWAKENING BREAKOUT (X = 190 to 268, Y = -18 to 24)
        // Fast vertical climb (CameraZone_Climb): LavaLeaper_Fast -> DashLane_UP +
        // DashBoost_Up blast through ceiling layers -> MovingPlatform + CrumblingBlocks ->
        // Twin-wall Wall_1x12 chimney + Secret Diamond 2 -> SpinArm & Cannon_Fast upper deck
        // =====================================================================
        GameObject czClimb1 = p.Spawn(p.CameraZoneClimb, flow, 210f, 3f);
        czClimb1.name = "CameraZone_Climb_AwakeningLower";
        SetTriggerBoxSize(czClimb1, new Vector3(40f, 44f, 10f)); // X=190..230, Y=-19..25

        GameObject czClimb2 = p.Spawn(p.CameraZoneClimb, flow, 249f, 3f);
        czClimb2.name = "CameraZone_Climb_AwakeningUpper";
        SetTriggerBoxSize(czClimb2, new Vector3(38f, 44f, 10f)); // X=230..268, Y=-19..25

        // Zone 3 Perimeter Walls, Roof & Recovery Floors
        p.SpawnBlock(p.BlockCyan, env, 190f, 23f, 2, 64, 4);   // Left wall above Sanct 2 X=190..191, Y=-9..23
        for (int x = 190; x < 268; x += 20)
            p.SpawnCeilingSized(env, x, 24f, Mathf.Min(20, 268 - x));
        p.SpawnBlock(p.BlockCyan, env, 267f, 13f, 2, 62, 4);   // Right seal below Sanct 3 Y=-18..13

        // Continuous Quarry Awakening Floor (X = 190..240, Y = -18) & Lower Piers
        p.Spawn(p.Ground20, env, 190f, -18f);
        p.Spawn(p.Ground20, env, 210f, -18f);
        p.SpawnGroundSized(env, 230f, -18f, 10);
        foreach (float px in new[] { 196f, 214f, 234f, 267f })
            p.SpawnBlock(p.BlockCyan, env, px, -19f, 2, 18, 4);

        // 3A: LavaLeaper_Fast -> BouncePad -> DashLane_UP + DashBoost_Up blasting ceiling layers!
        p.Spawn(p.LavaLeaperFast, haz, 196f, -18f);
        p.Spawn(p.BouncePad, inter, 203f, -18f);
        p.Spawn(p.DashLaneUp, inter, 203f, -11f);             // Y=-15..-7
        GameObject boostUp = p.Spawn(p.DashBoostUp, inter, 203f, -5f); // Y=-6.5..-3.5
        ConfigureDashZoneBoost(boostUp, new Vector2(3f, 3f), 34f, 0.25f, 24f);

        p.SpawnBlock(p.BlockCyan, env, 199f, 3f, 2, 22, 4);    // Vertical chute left X=199..200, Y=-8..3
        p.SpawnBlock(p.BlockCyan, env, 207f, 3f, 2, 22, 4);    // Vertical chute right X=207..208, Y=-8..3
        for (int x = 200; x <= 206; x++) p.Spawn(p.TouchWallBlue, inter, x, -2f); // Ceiling Layer 1
        for (int x = 200; x <= 206; x++) p.Spawn(p.DashWall, inter, x, 0f);       // Ceiling Layer 2
        for (int x = 200; x <= 206; x++) p.Spawn(p.TouchWallBlue, inter, x, 2f);  // Ceiling Layer 3

        p.Spawn(p.OneWayPlatformLarge, env, 200f, 3.5f);       // Catch deck X=200..208
        SetAtomId(p.Spawn(p.Atom, col, 203.5f, 5.5f), 9);

        // Recovery cushions for 3A
        p.Spawn(p.OneWayPlatformLarge, env, 210f, -6f);
        p.Spawn(p.BouncePadMega, inter, 213f, -18f);
        p.Spawn(p.BouncePadMega, inter, 226f, -18f);

        // 3B: MovingPlatform Lift -> CrumblingBlock escape ledges -> Twin-wall Wall_1x12 Chimney
        GameObject mp3Lift = p.Spawn(p.MovingPlatform, env, 212f, 4f);
        ConfigureMovingPlatform(mp3Lift, new Vector3(0f, 5.5f, 0f), 3.0f, 0.4f); // Y=4..9.5

        p.Spawn(p.CrumblingBlock, env, 217f, 9.5f);
        SetAtomId(p.Spawn(p.Atom, col, 219f, 11.8f), 10);
        p.Spawn(p.CrumblingBlock, env, 221f, 9.5f);
        p.Spawn(p.Ledge8, env, 225f, 6.5f);                    // Step into chimney mouth X=225..229

        // Twin-Wall Breakout Chimney (X = 232..238, Y = 4..22)
        p.SpawnGroundSized(env, 228f, 4f, 10);                 // Base X=228..238, Y=3..4 (flush w/ 238!)
        p.Spawn(p.BouncePadMini, inter, 235f, 4f);
        p.Spawn(p.Wall12, env, 232f, 22f);                     // Left wall Y=10..22 (mouth Y=4..10)
        p.SpawnBlock(p.BlockCyan, env, 238f, 14f, 2, 20, 4);   // Right chimney wall Y=4..14 (flush on floor!)
        SetAtomId(p.Spawn(p.Atom, col, 235.5f, 10.5f), 11);
        for (int x = 233; x <= 237; x++) p.Spawn(p.TouchWallBlue, inter, x, 13f); // Breakable cap Y=12..13

        // Secret Diamond 2 Upper Chimney Rafter (X = 238..247, Y = 19)
        p.Spawn(p.OneWayPlatformLarge, env, 238f, 19f);        // X=238..246
        p.SpawnBlock(p.BlockGreen, env, 241f, 23f, 2, 4, 4);   // Lintel Y=21..23
        for (int y = 20; y <= 21; y++) p.Spawn(p.DashWall, inter, 241f, y); // Yellow secret barrier Y=19..21
        SetDiamondId(p.Spawn(p.Diamond, col, 244.5f, 20.5f), 2);

        // 3C: Upper Escape Deck (MovingPlatform + SpinArm + CrumblingBlock + Cannon_Fast) & Recovery Floor
        p.Spawn(p.Ledge8, env, 239f, 14f);                     // Chimney crest landing X=239..243
        GameObject mp3Horiz = p.Spawn(p.MovingPlatform, env, 245.5f, 14f);
        ConfigureMovingPlatform(mp3Horiz, new Vector3(3.5f, 0f, 0f), 3.0f, 0.4f); // X=243.5..251
        GameObject spinBreakout = p.Spawn(p.SpinArm, haz, 252f, 16.6f);
        ConfigureSpinArm(spinBreakout, 85f);
        p.Spawn(p.CrumblingBlock, env, 256.5f, 14.5f);
        p.SpawnGroundSized(env, 259f, 14f, 9);                 // X=259..268, Y=13..14
        SetAtomId(p.Spawn(p.Atom, col, 261.5f, 16f), 12);

        p.SpawnBlock(p.BlockMagenta, env, 265f, 18f, 4, 1, 4); // Bracket X=265..267, Y=17.5..18
        GameObject cannon3 = p.Spawn(p.CannonFast, haz, 266f, 18.8f);
        ConfigureCannon(cannon3, new Vector3(0f, 0f, 180f), 11f, 2.2f, 20f);

        // Lower Recovery Floor under Upper Escape (X = 238..267, Y = 4)
        p.SpawnGroundSized(env, 238f, 4f, 29);                 // Seamless X=238..267, Y=3..4
        p.SpawnBlock(p.BlockCyan, env, 238f, 3f, 2, 42, 4);    // Pier Y=-18..3
        p.SpawnBlock(p.BlockCyan, env, 255f, 3f, 2, 42, 4);    // Pier Y=-18..3
        p.Spawn(p.BouncePadMega, inter, 244f, 4f);
        p.Spawn(p.LavaLeaperFast, haz, 252f, 4f);
        p.Spawn(p.BouncePadMega, inter, 262f, 4f);

        // =====================================================================
        // SANCTUARY 3: CITADEL BELFRY (X = 268 to 282, Y = 14 to 22)
        // =====================================================================
        p.SpawnGroundSized(env, 268f, 14f, 14);                // X=268..282, Y=13..14
        p.SpawnCeilingSized(env, 268f, 22f, 14);               // X=268..282, Y=21..22
        p.SpawnBlock(p.BlockGreen, env, 269f, 21f, 2, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 279f, 21f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 275f, 13f, 2, 82, 4);   // Pier Y=-28..13
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 275f, 15.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE HIGH THRONE RUN (X = 282 to 368, Y = 4 to 26)
        // BouncePad_Mega launch -> extended Glide through Purple TouchWall curtains
        // + Diamond 3 high rafter -> DashBoost_Fast / DashZone runways smashing
        // triple-layered Yellow DashWall barricades -> leap into End_Portal
        // =====================================================================
        GameObject czWideThrone = p.Spawn(p.CameraZoneWide, flow, 325f, 15f);
        czWideThrone.name = "CameraZone_Wide_ThroneRun";
        SetTriggerBoxSize(czWideThrone, new Vector3(86f, 26f, 10f)); // X=282..368, Y=2..28

        // Sweeping Throne Canopy Roof (Y = 26) & Right Back Wall (X = 368)
        for (int x = 282; x < 368; x += 20)
            p.SpawnCeilingSized(env, x, 26f, Mathf.Min(20, 368 - x));
        p.SpawnBlock(p.BlockCyan, env, 282f, 25f, 2, 6, 4);    // Left step X=282..283, Y=22..25
        p.SpawnBlock(p.BlockCyan, env, 368f, 26f, 2, 108, 4);  // Back wall X=368..369, Y=-28..26

        // Continuous Throne Safety Floor under Descent & Glide (X = 282..326, Y = 4)
        p.Spawn(p.Ground20, env, 282f, 4f);
        p.Spawn(p.Ground20, env, 302f, 4f);
        p.SpawnGroundSized(env, 322f, 4f, 4);                  // Meets Runway pier at 326
        foreach (float px in new[] { 290f, 310f })
            p.SpawnBlock(p.BlockCyan, env, px, 3f, 2, 62, 4);  // Piers Y=-28..3

        // Crystal Entry Steps (Slope_45_Left x4 from Y=14 down to Y=6 Launch Dais)
        p.Spawn(p.Slope45Left, env, 283f, 12f);                // 282..284, 12..14
        p.Spawn(p.Slope45Left, env, 285f, 10f);                // 284..286, 10..12
        p.Spawn(p.Slope45Left, env, 287f, 8f);                 // 286..288, 8..10
        p.Spawn(p.Slope45Left, env, 289f, 6f);                 // 288..290, 6..8
        p.SpawnBlock(p.BlockCyan, env, 282f, 12f, 4, 16, 4);   // 282..284, Y=4..12
        p.SpawnBlock(p.BlockCyan, env, 284f, 10f, 4, 12, 4);   // 284..286, Y=4..10
        p.SpawnBlock(p.BlockCyan, env, 286f, 8f, 4, 8, 4);     // 286..288, Y=4..8
        p.SpawnBlock(p.BlockCyan, env, 288f, 6f, 4, 4, 4);     // 288..290, Y=4..6

        // Launch Dais (X = 290..296, Y = 6)
        p.SpawnBlock(p.BlockMagenta, env, 290f, 6f, 12, 4, 4); // X=290..296, Y=4..6
        p.Spawn(p.BouncePadMega, inter, 293f, 6f);             // Launches skyward into Glide arc

        // Extended Glide Span: 3 Suspended Purple Throne Curtains + Diamond 3 Upper Rafter
        p.SpawnBlock(p.BlockCyan, env, 301f, 25f, 2, 8, 4);    // Curtain 1 hanger Y=21..25 (allows high-glide route!)
        p.SpawnTouchWallCol(inter, 301f, 13f, 21f);            // Curtain 1 Y=12..21
        SetAtomId(p.Spawn(p.Atom, col, 305.5f, 15.0f), 13);

        p.Spawn(p.OneWayPlatformLarge, env, 307f, 19.5f);      // High glide rafter X=307..315
        SetDiamondId(p.Spawn(p.Diamond, col, 311f, 20.8f), 3);

        p.SpawnBlock(p.BlockMagenta, env, 310f, 17f, 2, 2, 4); // Floating pendant Y=16..17 (keeps high rafter open)
        p.SpawnTouchWallCol(inter, 310f, 12f, 16f);            // Curtain 2 Y=11..16
        SetAtomId(p.Spawn(p.Atom, col, 314.5f, 14.0f), 14);

        p.SpawnBlock(p.BlockCyan, env, 319f, 25f, 2, 12, 4);   // Curtain 3 hanger Y=19..25 (keeps high rafter open)
        p.SpawnTouchWallCol(inter, 319f, 11f, 19f);            // Curtain 3 Y=10..19
        SetAtomId(p.Spawn(p.Atom, col, 323.5f, 13.0f), 15);

        // Recovery Mega BouncePads on Lower Throne Safety Floor (Y = 4)
        p.Spawn(p.BouncePadMega, inter, 305f, 4f);
        p.Spawn(p.BouncePadMega, inter, 316f, 4f);

        // Flow-State Boost-Smash Runway (X = 326..358, Y = 10)
        p.Spawn(p.Ground20, env, 326f, 10f);                   // X=326..346, Y=9..10
        p.SpawnGroundSized(env, 346f, 10f, 12);                // X=346..358, Y=9..10
        foreach (float px in new[] { 326f, 342f, 357f })
            p.SpawnBlock(p.BlockCyan, env, px, 9f, 2, 74, 4);  // Piers Y=-28..9

        p.Spawn(p.DashBoostFast, inter, 329f, 11.5f);
        p.SpawnBlock(p.BlockMagenta, env, 334f, 25f, 6, 22, 4); // Header X=334..337, Y=14..25
        for (int x = 334; x <= 336; x++)
            for (int y = 11; y <= 14; y++)
                p.Spawn(p.DashWall, inter, x, y);              // Triple-wide Yellow Barricade #1

        SetAtomId(p.Spawn(p.Atom, col, 339.5f, 11.5f), 16);

        GameObject dzRunway = p.Spawn(p.DashZone, inter, 342.5f, 11.5f);
        ConfigureDashZoneBoost(dzRunway, new Vector2(4f, 3f), 34f, 0.25f, 24f);

        p.SpawnBlock(p.BlockCyan, env, 346f, 25f, 8, 22, 4);   // Header X=346..350, Y=14..25
        p.SpawnTouchWallCol(inter, 346f, 11f, 14f);            // Purple shock face
        for (int x = 347; x <= 349; x++)
            for (int y = 11; y <= 14; y++)
                p.Spawn(p.DashWall, inter, x, y);              // Triple-wide Yellow Barricade #2

        // Celebratory Slope_45 Leap onto Upper Throne Dais (X = 356..368, Y = 12) -> End_Portal
        p.Spawn(p.Slope45, env, 357f, 10f);                    // X=356..358, Y=10..12
        p.SpawnGroundSized(env, 358f, 12f, 10);                // Dais floor X=358..368, Y=11..12
        p.SpawnBlock(p.BlockMagenta, env, 358f, 11f, 2, 78, 4); // Dais pier X=358..359, Y=-28..11
        p.SpawnBlock(p.BlockGreen, env, 358f, 25f, 20, 2, 4);  // Royal canopy trim X=358..368, Y=24..25
        p.SpawnBlock(p.BlockGreen, env, 358f, 24f, 2, 2, 4);   // Left pendant
        p.SpawnBlock(p.BlockGreen, env, 367f, 24f, 2, 2, 4);   // Right pendant

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
        return $"Level_38 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 55f;
            lm.TwoStarTime = 85f;
            lm.OneStarTime = 140f;
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
            respawnMgr.KillTopY = 58f;
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
        public GameObject Ledge8 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Ledge_8x1.prefab");
        public GameObject Wall12 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Wall_1x12.prefab");
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject Slope45Left = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Left.prefab");
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
        public GameObject DashWallPoundOnly = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashWall_PoundOnly.prefab");
        public GameObject TouchWallBlue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/TouchWall_Blue.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject GroundPoundChainRow2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/GroundPoundChainRow 2.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMini = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mini.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashCharge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashCharge.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashBoostUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Up.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");

        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject CannonFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/Cannon_Fast.prefab");
        public GameObject FireJet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/FireJet.prefab");
        public GameObject FireJetDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Down.prefab");
        public GameObject LavaLeaperFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/LavaLeaper_Fast.prefab");
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
