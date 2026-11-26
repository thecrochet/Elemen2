#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level37Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_37.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 37 (Ancient Sewers & Fractured Shrines)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 180f, -25f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(480f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(480f, 4f, 1f);

        // Continuous Bedrock Base at Y = -18 (X = -10 to 370)
        for (int x = -10; x <= 350; x += 20)
            p.Spawn(p.Ground20, env, x, -18f);

        // =====================================================================
        // FLOW: START PORTAL, END PORTAL, PLAYER SPAWN & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 4f, 0f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 357f, 1.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // ZONE 1: THE DRAINAGE SLUICES (X = 0 to 86, Y = -12 to 7)
        // Tight underground sewer pipework with drop-slams & FireJet/LavaLeaper
        // =====================================================================
        GameObject czTightSluices = p.Spawn(p.CameraZoneTight, flow, 43f, -3f);
        czTightSluices.name = "CameraZone_Tight_Sluices";
        SetTriggerBoxSize(czTightSluices, new Vector3(88f, 22f, 10f));

        // Left Sewer Perimeter Wall (X = -1..0, Y = -18..7)
        p.Spawn(p.Wall12, env, -1f, -6f);                     // Y=-18..-6
        p.Spawn(p.Wall12, env, -1f, 6f);                      // Y=-6..6
        p.SpawnBlock(p.BlockGreen, env, -1f, 7f, 2, 2, 4);    // Top corner Y=6..7

        // Entry Pipe Ceiling (X = 0..42, Y = 6..7) & Pipe Ribs
        p.Spawn(p.Ceiling20, env, 0f, 7f);
        p.Spawn(p.Ceiling20, env, 20f, 7f);
        p.SpawnCeilingSized(env, 40f, 7f, 2);
        foreach (float rx in new[] { 8f, 17f, 38f })
            p.SpawnBlock(p.BlockGreen, env, rx, 6f, 2, 1, 4); // Subtle ceiling brackets

        // Stepping Pipe Ceiling (X = 42..86, Y = -1..0)
        p.SpawnBlock(p.BlockCyan, env, 42f, 7f, 2, 14, 4);    // Vertical drop step X=42..43, Y=0..7
        p.Spawn(p.Ceiling20, env, 42f, 0f);
        p.Spawn(p.Ceiling20, env, 62f, 0f);
        p.SpawnCeilingSized(env, 82f, 0f, 4);

        // 1) Entry Sewer Pipe (X = 0..26, Y = 0)
        p.Spawn(p.Ground20, env, 0f, 0f);                     // X=0..20, Y=-1..0
        p.SpawnGroundSized(env, 20f, 0f, 6);                  // X=20..26 (Drop #1 starts X=26)
        p.Spawn(p.Wall12, env, 12f, -1f);                     // Support pier Y=-13..-1
        p.SpawnBlock(p.BlockCyan, env, 12f, -13f, 2, 10, 4);  // Extends to -18

        // Smashable Grate #1 (X = 11)
        p.SpawnBlock(p.BlockGreen, env, 11f, 6f, 2, 6, 4);    // Header Y=3..6
        p.SpawnTouchWallCol(inter, 11f, 1f, 3f);              // Grate Y=0..3

        p.Spawn(p.FireJet, haz, 17f, 0f);

        // Smashable Grate #2 (X = 22)
        p.SpawnBlock(p.BlockGreen, env, 22f, 6f, 2, 6, 4);    // Header Y=3..6
        p.SpawnTouchWallCol(inter, 22f, 1f, 3f);              // Grate Y=0..3
        SetAtomId(p.Spawn(p.Atom, col, 24.5f, 1.5f), 1);

        // 2) Pipe Drop #1 (X = 26..34, Y = 0 down to -6)
        p.SpawnBlock(p.BlockCyan, env, 25f, -1f, 2, 10, 4);   // Left drop wall X=25..26, Y=-6..-1
        p.SpawnBlock(p.BlockCyan, env, 34f, 6f, 2, 18, 4);    // Right drop baffle X=34..35, Y=-3..6 (3m doorway below!)
        p.Spawn(p.OneWayPlatformLarge, env, 26f, 0f);         // Top grate X=26..34

        // Cracked Pipe Seal (Y = -3)
        p.SpawnBlock(p.BlockGreen, env, 26f, -3f, 4, 4, 4);   // Left shoulder X=26..28, Y=-5..-3
        p.Spawn(p.CrackedBlocks4x4, inter, 28f, -3f);         // X=28..30, Y=-5..-3
        p.Spawn(p.CrackedBlocks4x4, inter, 30f, -3f);         // X=30..32, Y=-5..-3
        p.SpawnBlock(p.BlockGreen, env, 32f, -3f, 4, 4, 4);   // Right shoulder X=32..34, Y=-5..-3

        // 3) Mid Drainage Conduit (X = 26..54, Y = -6)
        p.Spawn(p.Ground20, env, 26f, -6f);                   // X=26..46, Y=-7..-6
        p.SpawnGroundSized(env, 46f, -6f, 8);                 // X=46..54 (Drop #2 starts X=54)
        p.SpawnBlock(p.BlockCyan, env, 38f, -7f, 2, 22, 4);   // Exact 11m support pier Y=-18..-7
        p.Spawn(p.LavaLeaper, haz, 38f, -6f);                 // Crests at Y=-1 (clear under X=35..42 Y=6 roof!)

        // Smashable Grate #3 (X = 49)
        p.SpawnBlock(p.BlockGreen, env, 49f, -1f, 2, 4, 4);   // Header Y=-3..-1
        p.SpawnTouchWallCol(inter, 49f, -5f, -3f);            // Grate Y=-6..-3
        SetAtomId(p.Spawn(p.Atom, col, 51.5f, -4.5f), 2);

        // 4) Pipe Drop #2 (X = 54..62, Y = -6 down to -12)
        p.SpawnBlock(p.BlockCyan, env, 53f, -7f, 2, 10, 4);   // Left wall X=53..54, Y=-12..-7
        p.SpawnBlock(p.BlockCyan, env, 62f, -1f, 2, 16, 4);   // Right baffle X=62..63, Y=-9..-1 (3m doorway below!)
        p.Spawn(p.OneWayPlatformLarge, env, 54f, -6f);        // Drop grate X=54..62

        // Yellow DashWall Seal (Y = -9)
        p.SpawnBlock(p.BlockCyan, env, 54f, -9f, 4, 2, 4);    // Left shoulder X=54..56, Y=-10..-9
        for (int x = 56; x <= 59; x++) p.Spawn(p.DashWall, inter, x, -9f); // Y=-10..-9
        p.SpawnBlock(p.BlockCyan, env, 60f, -9f, 4, 2, 4);    // Right shoulder X=60..62, Y=-10..-9

        // Lower Drainage Tunnel (X = 54..86, Y = -12)
        p.Spawn(p.Ground20, env, 54f, -12f);
        p.SpawnGroundSized(env, 74f, -12f, 12);
        foreach (float px in new[] { 54f, 70f, 84f })
            p.SpawnBlock(p.BlockCyan, env, px, -13f, 2, 10, 4); // Piers Y=-18..-13

        p.Spawn(p.FireJet, haz, 67f, -12f);
        p.Spawn(p.OneWayPlatform, env, 69f, -8.5f);
        p.Spawn(p.LavaLeaper, haz, 76f, -12f);

        // Exit Sluice Grate (X = 81)
        p.SpawnBlock(p.BlockGreen, env, 81f, -1f, 2, 16, 4);  // Header Y=-9..-1
        p.SpawnTouchWallCol(inter, 81f, -11f, -9f);           // Grate Y=-12..-9

        // =====================================================================
        // SANCTUARY 1: SEWER SLUICE CHAMBER (X = 86 to 100, Y = -12 to -3)
        // =====================================================================
        p.SpawnGroundSized(env, 86f, -12f, 14);               // X=86..100, Y=-13..-12
        p.SpawnCeilingSized(env, 86f, -3f, 14);               // X=86..100, Y=-4..-3
        p.SpawnBlock(p.BlockGreen, env, 87f, -4f, 2, 4, 4);
        p.SpawnBlock(p.BlockGreen, env, 98f, -4f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 92f, -13f, 2, 10, 4);  // Foundation Y=-18..-13
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 93f, -10.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE SUBTERRANEAN CISTERN (X = 100 to 184, Y = -12 to 8)
        // Branching Upper Catwalk (Ice/Crumble/SpinArm/Secret) vs Lower Floodway
        // =====================================================================
        GameObject czWideCistern = p.Spawn(p.CameraZoneWide, flow, 142f, -2f);
        czWideCistern.name = "CameraZone_Wide_Cistern";
        SetTriggerBoxSize(czWideCistern, new Vector3(84f, 24f, 10f));

        // Reservoir Floor (X = 100..180, Y = -12) & Vaulted Roof (X = 100..180, Y = 8)
        for (int x = 100; x <= 160; x += 20)
        {
            p.Spawn(p.Ground20, env, x, -12f);
            p.Spawn(p.Ceiling20, env, x, 8f);
        }
        p.SpawnBlock(p.BlockCyan, env, 100f, 7f, 2, 20, 4);   // Left step wall X=100..101, Y=-3..7
        foreach (float px in new[] { 100f, 120f, 140f, 160f, 179f })
            p.SpawnBlock(p.BlockCyan, env, px, -13f, 2, 10, 4); // Reservoir piers Y=-18..-13

        // Ascent Path to Upper Maintenance Catwalk (X = 106..114)
        p.Spawn(p.BouncePad, inter, 106f, -12f);
        p.Spawn(p.OneWayPlatform, env, 109f, -7f);
        p.Spawn(p.BouncePad, inter, 111f, -7f);

        // Upper Maintenance Catwalk (X = 114..158, Y = -1)
        p.Spawn(p.Ledge8, env, 114f, -1f);                    // X=114..118
        p.Spawn(p.IceSurface, env, 118f, -1f);                // X=118..124, Y=-2..-1
        p.Spawn(p.CrumblingBlock, env, 126f, -0.5f);          // X=125..127, top=0
        p.Spawn(p.CrumblingBlock, env, 130f, -0.5f);          // X=129..131, top=0
        GameObject spinCistern = p.Spawn(p.SpinArm, haz, 135f, -0.5f);
        ConfigureSpinArm(spinCistern, 80f);

        p.Spawn(p.IceSurface, env, 138f, -1f);                // X=138..144, Y=-2..-1
        GameObject sawCatwalk = p.Spawn(p.SawSlow, haz, 139f, 0.0f);
        ConfigureSaw(sawCatwalk, new Vector3(4f, 0f, 0f), 2.5f);

        p.Spawn(p.CrumblingBlock, env, 147f, -0.5f);
        p.Spawn(p.CrumblingBlock, env, 151f, -0.5f);
        p.Spawn(p.Ledge8, env, 154f, -1f);                    // X=154..158, top=-1

        // Secret Diamond Vault off Upper Catwalk (X = 158..172, Y = -1 to 5)
        GameObject sawGuard = p.Spawn(p.SawSlow, haz, 156f, 0.2f);
        ConfigureSaw(sawGuard, new Vector3(0f, 2.5f, 0f), 2.4f); // Guards Yellow side-wall cleanly

        p.SpawnGroundSized(env, 158f, -1f, 14);               // Vault floor X=158..172, Y=-2..-1
        p.SpawnBlock(p.BlockMagenta, env, 158f, 4f, 28, 2, 4); // Vault ceiling X=158..172, Y=3..4 (3m headroom above!)
        p.SpawnBlock(p.BlockMagenta, env, 171f, 3f, 2, 8, 4);  // Back wall X=171..172, Y=-1..3
        p.SpawnBlock(p.BlockMagenta, env, 158f, 3f, 2, 2, 4);  // Door lintel Y=2..3
        for (int y = 0; y <= 2; y++) p.Spawn(p.DashWall, inter, 158f, y); // Yellow seal Y=-1..2

        SetAtomId(p.Spawn(p.Atom, col, 161.5f, 0.5f), 3);
        SetDiamondId(p.Spawn(p.Diamond, col, 165f, 0.5f), 1);
        SetAtomId(p.Spawn(p.Atom, col, 168.5f, 0.5f), 4);

        // Lower Floodway Sprint (X = 114..180, Y = -12)
        p.Spawn(p.DashBoostFast, inter, 116f, -10.5f);
        p.SpawnBlock(p.BlockCyan, env, 123f, -6f, 2, 4, 4);   // Sluice arch Y=-8..-6
        p.SpawnTouchWallCol(inter, 123f, -11f, -8f);          // Barricade 1 Y=-12..-8
        SetAtomId(p.Spawn(p.Atom, col, 126.5f, -10.2f), 5);

        p.SpawnBlock(p.BlockMagenta, env, 130f, -6f, 2, 4, 4); // Sluice arch Y=-8..-6
        for (int y = -11; y <= -8; y++) p.Spawn(p.DashWall, inter, 130f, y); // Barricade 2

        GameObject dzFlood = p.Spawn(p.DashZone, inter, 135f, -10.5f);
        ConfigureDashZoneBoost(dzFlood, new Vector2(4f, 3f), 30f, 0.25f, 22f);

        p.SpawnBlock(p.BlockCyan, env, 142f, -6f, 4, 4, 4);   // 2-wide sluice arch
        for (int x = 142; x <= 143; x++)
        {
            for (int y = -11; y <= -8; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }
        SetAtomId(p.Spawn(p.Atom, col, 146.5f, -10.2f), 6);

        // Floodway Rejoin Updraft (X = 176, Y = -12 -> Y = 0 Sanctuary 2)
        p.Spawn(p.BouncePad, inter, 176f, -12f);
        p.Spawn(p.DashLaneUp, inter, 176f, -5.5f);            // Y=-9.5..-1.5 launches cleanly past Y=0!
        p.Spawn(p.OneWayPlatformLarge, env, 172f, 0f);        // Catch platform X=172..180

        // =====================================================================
        // SANCTUARY 2: FLOODGATE SHRINE (X = 180 to 194, Y = 0 to 9)
        // =====================================================================
        p.SpawnGroundSized(env, 180f, 0f, 14);                // X=180..194, Y=-1..0
        p.SpawnCeilingSized(env, 180f, 9f, 14);               // X=180..194, Y=8..9
        p.SpawnBlock(p.BlockGreen, env, 181f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 191f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 187f, -1f, 2, 34, 4);  // Foundation Y=-18..-1
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 187f, 1.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE FRACTURED ASCENT (X = 194 to 272, Y = 0 to 30)
        // Twin-wall chimney -> soaring MovingPlatform shrines, Gliding, Cannons,
        // and continuous terraced lower safety path underneath
        // =====================================================================
        GameObject czClimbAscent = p.Spawn(p.CameraZoneClimb, flow, 200f, 11f);
        czClimbAscent.name = "CameraZone_Climb_Ascent";
        SetTriggerBoxSize(czClimbAscent, new Vector3(14f, 24f, 10f));

        GameObject czWideShrines = p.Spawn(p.CameraZoneWide, flow, 240f, 14f);
        czWideShrines.name = "CameraZone_Wide_Shrines";
        SetTriggerBoxSize(czWideShrines, new Vector3(66f, 32f, 10f));

        // Continuous Terraced Recovery Floor (X = 194..272, Y = 0)
        p.Spawn(p.Ground20, env, 194f, 0f);
        p.Spawn(p.Ground20, env, 214f, 0f);
        p.Spawn(p.Ground20, env, 234f, 0f);
        p.SpawnGroundSized(env, 254f, 0f, 18);

        foreach (float px in new[] { 194f, 214f, 234f, 254f, 271f })
        {
            p.Spawn(p.Wall12, env, px, -1f);
            p.SpawnBlock(p.BlockGreen, env, px, -13f, 2, 10, 4); // To -18
        }

        // Left & Right Shrine Perimeter Walls + Broken-Shrine Crown Roof (Y = 30)
        p.Spawn(p.Wall12, env, 193f, 21f);                    // Y=9..21 (above Sanctuary 2 roof Y=9)
        p.SpawnBlock(p.BlockGreen, env, 193f, 30f, 2, 18, 4); // Y=21..30

        p.Spawn(p.Wall12, env, 271f, 20f);                    // Y=8..20 (leaves Y=0..8 exit arch)
        p.SpawnBlock(p.BlockGreen, env, 271f, 30f, 2, 20, 4); // Y=20..30

        for (int x = 194; x < 271; x += 20)
        {
            int w = Mathf.Min(20, 271 - x);
            p.SpawnCeilingSized(env, x, 30f, w);
        }
        // Shrine crown pillars / ribs along canopy (Y=29)
        foreach (float rx in new[] { 208f, 224f, 240f, 256f })
            p.SpawnBlock(p.BlockGreen, env, rx, 29f, 2, 2, 4);

        // 1) Twin-Wall Shrine Chimney (X = 196..202, 4m interior width)
        p.Spawn(p.Wall12, env, 196f, 16f);                    // Y=4..16 (ground arch Y=0..4)
        p.Spawn(p.Wall12, env, 201f, 12f);                    // Y=0..12
        p.SpawnBlock(p.BlockGreen, env, 201f, 16f, 2, 8, 4);  // Y=12..16
        p.Spawn(p.BouncePadMini, inter, 199f, 0f);
        SetAtomId(p.Spawn(p.Atom, col, 199f, 9f), 7);
        p.Spawn(p.TouchWallBlue, inter, 199f, 13f);
        p.Spawn(p.Ledge8, env, 202f, 16f);                    // Chimney crest X=202..206, Y=16

        // 2) Glide #1 -> Purple Touch Cloud -> MovingPlatform Shrine A
        p.Spawn(p.BouncePad, inter, 205f, 16f);
        p.SpawnTouchWallBlock(inter, 211f, 215f, 18f, 20f, 2f, 1f);
        SetAtomId(p.Spawn(p.Atom, col, 213f, 21f), 8);

        GameObject mpA = p.Spawn(p.MovingPlatform, env, 220f, 17f);
        ConfigureMovingPlatform(mpA, new Vector3(6f, 0f, 0f), 3.0f, 0.4f); // X=220..226

        // Mid-Shrine Cannon + Vertical MovingPlatform Shrine B
        p.SpawnBlock(p.BlockMagenta, env, 231f, 18f, 4, 2, 4); // Pedestal X=231..233, Y=17..18
        GameObject cannonMid = p.Spawn(p.Cannon, haz, 232f, 18.8f);
        ConfigureCannon(cannonMid, new Vector3(0f, 0f, 180f), 8f, 2.2f, 15f);

        GameObject mpB = p.Spawn(p.MovingPlatform, env, 237f, 19f);
        ConfigureMovingPlatform(mpB, new Vector3(0f, 5f, 0f), 2.8f, 0.4f); // X=235..239, Y=19..24 (2m clean gap from cannon pedestal)

        // 3) High-Altitude Glide #2 + Mid-Air Dash Gate -> Diamond 2 Shrine
        SetAtomId(p.Spawn(p.Atom, col, 242f, 24.5f), 9);
        SetAtomId(p.Spawn(p.Atom, col, 246f, 24.0f), 10);

        p.SpawnBlock(p.BlockGreen, env, 250f, 29f, 2, 10, 4); // Shrine lintel Y=24..29
        for (int y = 22; y <= 24; y++) p.Spawn(p.DashWall, inter, 250f, y); // Mid-air gate Y=21..24

        p.Spawn(p.OneWayPlatformLarge, env, 252f, 22f);       // High shrine causeway X=252..260
        p.Spawn(p.OneWayPlatformShort, env, 257f, 24f);       // Pedestal X=257..259
        SetDiamondId(p.Spawn(p.Diamond, col, 258f, 25.0f), 2);
        SetAtomId(p.Spawn(p.Atom, col, 263f, 22.5f), 11);

        p.SpawnBlock(p.BlockGreen, env, 269f, 23f, 4, 1, 4);  // Cannon wall bracket X=269..271, Y=22.5..23
        GameObject cannonHigh = p.Spawn(p.CannonFast, haz, 269.8f, 23.8f);
        ConfigureCannon(cannonHigh, new Vector3(0f, 0f, 180f), 10f, 2.4f, 18f);

        // Descent cushions & lower terraced recovery route (Zero blind deaths!)
        p.Spawn(p.Ledge8, env, 216f, 4f);
        p.Spawn(p.BouncePadMega, inter, 224f, 0f);            // Springs back up to Shrine A
        p.Spawn(p.Ledge8, env, 232f, 6f);
        SetAtomId(p.Spawn(p.Atom, col, 238f, 1.5f), 12);
        p.Spawn(p.BouncePadMega, inter, 246f, 0f);            // Recovery bounce toward upper platforms
        p.Spawn(p.OneWayPlatformLarge, env, 252f, 11f);
        p.Spawn(p.BouncePad, inter, 256f, 11f);               // Springs up to High Shrine Y=22
        p.Spawn(p.OneWayPlatformLarge, env, 262f, 8f);        // Controlled descent cushion

        // =====================================================================
        // SANCTUARY 3: SANCTUM CLOISTER (X = 272 to 286, Y = 0 to 9)
        // =====================================================================
        p.SpawnGroundSized(env, 272f, 0f, 14);                // X=272..286, Y=-1..0
        p.SpawnCeilingSized(env, 272f, 9f, 14);               // X=272..286, Y=8..9
        p.SpawnBlock(p.BlockMagenta, env, 273f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 283f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockCyan, env, 279f, -1f, 2, 34, 4);  // Foundation Y=-18..-1
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 279f, 1.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE SANCTUM GAUNTLET (X = 286 to 362, Y = -10 to 20)
        // StompBlocks -> chained Slope_45_Large ramps + Chevron Boost smashes ->
        // Multi-layer Ground Pound into Altar -> BouncePad_Mega into End_Portal
        // =====================================================================
        GameObject czWideSanctum = p.Spawn(p.CameraZoneWide, flow, 324f, 5f);
        czWideSanctum.name = "CameraZone_Wide_Sanctum";
        SetTriggerBoxSize(czWideSanctum, new Vector3(76f, 32f, 10f));

        // Sanctum Hall Ceiling (X = 286..362, Y = 19..20) & Left Step Wall (X = 286..287, Y = 9..19)
        p.SpawnBlock(p.BlockCyan, env, 286f, 19f, 2, 20, 4);
        for (int x = 286; x < 362; x += 20)
        {
            int w = Mathf.Min(20, 362 - x);
            p.SpawnCeilingSized(env, x, 20f, w);
        }
        // Back Wall sealing Sanctum behind End_Portal (X = 362..363)
        p.SpawnBlock(p.BlockCyan, env, 362f, 20f, 2, 2, 4);
        p.Spawn(p.Wall12, env, 362f, 19f);                    // Y=7..19
        p.Spawn(p.Wall12, env, 362f, 7f);                     // Y=-5..7
        p.SpawnBlock(p.BlockCyan, env, 362f, -5f, 2, 26, 4);  // Y=-18..-5

        // 1) StompBlock Wave Gauntlet (X = 286..307, Y = 0)
        p.Spawn(p.Ground20, env, 286f, 0f);                   // X=286..306
        p.SpawnGroundSized(env, 306f, 0f, 5);                 // X=306..311 (under Ramp 1)
        foreach (float px in new[] { 295f, 306f })
        {
            p.Spawn(p.Wall12, env, px, -1f);
            p.SpawnBlock(p.BlockCyan, env, px, -13f, 2, 10, 4);
        }

        GameObject stomp1 = p.Spawn(p.StompBlock, haz, 291f, 4.5f);
        ConfigureStompBlock(stomp1, 0f);
        GameObject stomp2 = p.Spawn(p.StompBlock, haz, 296f, 4.5f);
        ConfigureStompBlock(stomp2, 0.45f);

        // 2) Chained Slope_45_Large Speed Ramps + Chevron Boost Smashes
        p.Spawn(p.DashBoostFast, inter, 300f, 1.5f);
        p.Spawn(p.Slope45Large, env, 309f, 0f);               // Ramp 1: X=307..311, Y=0..4

        p.SpawnGroundSized(env, 311f, 4f, 11);                // Mid deck X=311..322, Y=3..4
        p.SpawnBlock(p.BlockCyan, env, 311f, 3f, 2, 42, 4);   // Deep pier X=311..312, Y=-18..3
        SetAtomId(p.Spawn(p.Atom, col, 312f, 5.5f), 13);

        GameObject dzRamp = p.Spawn(p.DashZone, inter, 314.5f, 5.5f);
        ConfigureDashZoneBoost(dzRamp, new Vector2(4f, 3f), 32f, 0.18f, 24f);

        p.Spawn(p.Slope45Large, env, 320f, 4f);               // Ramp 2: X=318..322, Y=4..8

        // Upper Sanctum Smash Expressway (X = 322..331, Y = 8)
        p.SpawnGroundSized(env, 322f, 8f, 9);                 // X=322..331, Y=7..8
        p.SpawnBlock(p.BlockCyan, env, 322f, 7f, 2, 50, 4);   // Pier X=322..323, Y=-18..7
        p.SpawnBlock(p.BlockCyan, env, 330f, 7f, 2, 34, 4);   // Left Plunge Wall X=330..331, Y=-10..7
        p.SpawnBlock(p.BlockCyan, env, 330f, -10f, 2, 16, 4); // Extends to -18

        p.Spawn(p.DashBoostFast, inter, 323.5f, 9.5f);
        SetAtomId(p.Spawn(p.Atom, col, 325.5f, 9.8f), 14);

        // Thick 3-column Alternating Yellow + Purple Barricade Wall (X = 327..329, Y = 9..12)
        p.SpawnBlock(p.BlockMagenta, env, 327f, 19f, 6, 14, 4); // Arch header X=327..330, Y=12..19
        for (int x = 327; x <= 329; x++)
        {
            for (int y = 9; y <= 12; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }

        // 3) Final Multi-Layer Ground Pound Drop (X = 331..338, Y = 8 down to -10)
        p.SpawnBlock(p.BlockCyan, env, 338f, 19f, 2, 46, 4);  // Right Plunge Wall X=338..339, Y=-4..19 (leaves -10..-4 arch!)
        SetAtomId(p.Spawn(p.Atom, col, 334.5f, 10.2f), 15);

        for (int x = 331; x <= 337; x++) p.Spawn(p.DashWall, inter, x, 5f);      // Layer 1: Y=4..5
        for (int x = 331; x <= 337; x++) p.Spawn(p.TouchWallBlue, inter, x, 1f); // Layer 2: Y=0..1

        p.Spawn(p.DashWall, inter, 331f, -3f);
        p.Spawn(p.DashWall, inter, 331f, -4f);
        p.Spawn(p.CrackedBlocks4x4, inter, 332f, -3f);        // Layer 3 Core: X=332..334, Y=-5..-3
        p.Spawn(p.CrackedBlocks4x4, inter, 334f, -3f);        // Layer 3 Core: X=334..336, Y=-5..-3
        p.Spawn(p.DashWall, inter, 336f, -3f);
        p.Spawn(p.DashWall, inter, 337f, -3f);
        p.Spawn(p.DashWall, inter, 336f, -4f);
        p.Spawn(p.DashWall, inter, 337f, -4f);

        SetAtomId(p.Spawn(p.Atom, col, 334.5f, -7.5f), 16);

        // 4) Sunken Altar Chamber & Celebratory Launch into End_Portal (X = 331..362)
        p.SpawnGroundSized(env, 331f, -10f, 11);              // X=331..342
        p.Spawn(p.Ground20, env, 342f, -10f);                 // X=342..362
        foreach (float px in new[] { 342f, 355f })
            p.SpawnBlock(p.BlockCyan, env, px, -11f, 2, 14, 4); // Piers Y=-18..-11

        p.Spawn(p.BouncePadMega, inter, 343f, -10f);
        p.Spawn(p.BouncePad, inter, 347f, -10f);              // Backup pad (reaches -5.17 above step!)
        p.Spawn(p.OneWayPlatform, env, 347f, -5.5f);          // Backup step

        // Diamond 3 Crown Jewel Shrine along Victory Bounce Arc (hollow 3x3 halo around Diamond)
        for (int x = 348; x <= 350; x++)
            for (int y = 1; y <= 3; y++)
                if (!(x == 349 && y == 2))
                    p.Spawn(p.TouchWallBlue, inter, x, y);
        SetDiamondId(p.Spawn(p.Diamond, col, 349.5f, 1.5f), 3);

        // Victory Altar Dais (X = 352..362, Y = 0) + Royal Shrine Canopy
        p.SpawnGroundSized(env, 352f, 0f, 10);                // X=352..362, Y=-1..0
        p.SpawnBlock(p.BlockMagenta, env, 352f, -1f, 2, 18, 4); // Altar pillar X=352..353, Y=-10..-1
        p.SpawnBlock(p.BlockGreen, env, 352f, 19f, 20, 1, 4);   // Canopy trim under Y=19 roof
        p.SpawnBlock(p.BlockGreen, env, 352f, 18.5f, 2, 2, 4);  // Left pendant
        p.SpawnBlock(p.BlockGreen, env, 361f, 18.5f, 2, 2, 4);  // Right pendant

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
        return $"Level_37 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 50f;
            lm.TwoStarTime = 80f;
            lm.OneStarTime = 130f;
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
            respawnMgr.KillY = -23f;
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
        public GameObject BlockCyan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Cyan.prefab");
        public GameObject BlockGreen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Green Variant.prefab");
        public GameObject BlockMagenta = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Block_Magenta Variant.prefab");
        public GameObject Slope45Large = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Variants/Slope_45_Large.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject OneWayPlatformLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject OneWayPlatformShort = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject CrumblingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/CrumblingBlock.prefab");
        public GameObject IceSurface = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/IceSurface.prefab");

        public GameObject DashWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashWall.prefab");
        public GameObject TouchWallBlue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/TouchWall_Blue.prefab");
        public GameObject CrackedBlocks4x4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/CrackedBlocks_4x4.prefab");
        public GameObject BouncePad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/BouncePad.prefab");
        public GameObject BouncePadMini = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mini.prefab");
        public GameObject BouncePadMega = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/BouncePad_Mega.prefab");
        public GameObject DashBoostFast = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/Variants/DashBoost_Fast.prefab");
        public GameObject DashLaneUp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashLane_UP.prefab");
        public GameObject DashZone = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Interactables/DashZone.prefab");

        public GameObject SpinArm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/SpinArm.prefab");
        public GameObject Cannon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Cannon.prefab");
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

        public void SpawnTouchWallBlock(Transform parent, float startX, float endX, float startY, float endY, float stepX, float stepY)
        {
            for (float x = startX; x <= endX; x += stepX)
                for (float y = startY; y <= endY; y += stepY)
                    Spawn(TouchWallBlue, parent, x, y);
        }
    }
}
#endif
