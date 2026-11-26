#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level36Builder
{
    private const string TargetScenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_36.unity";

    [MenuItem("Elemento/Build FINAL_PURPLE/Level 36 (Dead Cells Showcase)")]
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
        GameObject killPit = p.Spawn(p.KillPit, haz, 165f, -24f);
        killPit.name = "KillPit_GlobalBottom";
        SetBoxSize(killPit, new Vector3(440f, 4f, 10f));
        Transform kpVis = killPit.transform.Find("Visual");
        if (kpVis != null) kpVis.localScale = new Vector3(440f, 4f, 1f);

        // Continuous Bedrock Base at Y = -16 (spans X = -10 to 350)
        for (int x = -10; x <= 330; x += 20)
        {
            p.Spawn(p.Ground20, env, x, -16f);
        }

        // =====================================================================
        // FLOW: START PORTAL, END PORTAL, PLAYER SPAWN & MANAGERS
        // =====================================================================
        GameObject startPortal = p.Spawn(p.StartPortal, flow, 4f, 0f);
        startPortal.name = "StartPortal";

        GameObject endPortal = p.Spawn(p.EndPortal, flow, 327f, 1.8f);
        endPortal.name = "End_Portal";

        WireCoreAndPortals(levelCore, startPortal, endPortal);

        // =====================================================================
        // ZONE 1: THE OUTER RAMPARTS (X = 0 to 76, Y = 0 to 18)
        // =====================================================================
        // Left Fortress Perimeter Wall (X = -1..0, Y = -16..18)
        p.Spawn(p.Wall12, env, -1f, 0f);          // Y = -12..0
        p.SpawnBlock(p.BlockCyan, env, -1f, -12f, 2, 8, 4); // Y = -16..-12
        p.Spawn(p.Wall12, env, -1f, 12f);         // Y = 0..12
        p.SpawnBlock(p.BlockCyan, env, -1f, 18f, 2, 12, 4); // Y = 12..18

        // Main Ramparts Floor (X = 0..76, Y = -1..0)
        p.Spawn(p.Ground20, env, 0f, 0f);
        p.Spawn(p.Ground20, env, 20f, 0f);
        p.Spawn(p.Ground20, env, 40f, 0f);
        p.SpawnGroundSized(env, 60f, 0f, 16);

        // Sub-Rampart Structural Piers down to Bedrock (-16..-1)
        // Kept left of Secret Alcove #1 (X=68..90, Y=-8..-2)
        foreach (float px in new[] { 18f, 38f, 58f, 66f })
        {
            p.Spawn(p.Wall12, env, px, -1f);               // Y = -13..-1
            p.SpawnBlock(p.BlockCyan, env, px, -13f, 2, 6, 4); // Y = -16..-13
        }
        // Decorative under-arch corbels
        foreach (float cx in new[] { 1f, 17f, 19f, 37f, 39f, 57f, 59f, 65f })
        {
            p.SpawnBlock(p.BlockCyan, env, cx, -1f, 2, 2, 4);
        }

        // Ramparts Castle Roofs & Battlement Merlons
        p.Spawn(p.Ceiling20, env, 0f, 12f);                // Entry Hall Roof Y=11..12
        for (int mx = 1; mx <= 17; mx += 4)
            p.SpawnBlock(p.BlockCyan, env, mx, 13f, 2, 2, 4);

        p.SpawnBlock(p.BlockCyan, env, 19f, 17f, 2, 10, 4); // Turret step X=19..20, Y=12..17
        p.Spawn(p.Ceiling20, env, 20f, 18f);               // Skyway Roof Y=17..18
        p.Spawn(p.Ceiling20, env, 40f, 18f);
        for (int mx = 21; mx <= 57; mx += 4)
            p.SpawnBlock(p.BlockCyan, env, mx, 19f, 2, 2, 4);

        p.SpawnBlock(p.BlockCyan, env, 59f, 17f, 2, 6, 4); // Gatehouse turret step Y=14..17
        p.SpawnCeilingSized(env, 60f, 14f, 16);            // Gatehouse Roof X=60..76, Y=13..14
        for (int mx = 61; mx <= 73; mx += 4)
            p.SpawnBlock(p.BlockCyan, env, mx, 15f, 2, 2, 4);

        // Doorway #1 (X = 12): Stone arch header + 3-tall Purple smashable door
        p.SpawnBlock(p.BlockCyan, env, 12f, 11f, 2, 16, 4); // Y = 3..11
        p.SpawnTouchWallCol(inter, 12f, 1f, 3f);            // Y = 0..3
        SetAtomId(p.Spawn(p.Atom, col, 15.5f, 1.5f), 1);

        // Courtyard Entry Ramp & Lower Level
        p.Spawn(p.Slope45, env, 20f, 0f);                  // X=19..21, Y=0..2
        p.Spawn(p.FireJet, haz, 28f, 0f);

        // Doorway #2 (X = 34): Stone lintel under Tier 1 + Purple door
        p.SpawnBlock(p.BlockCyan, env, 34f, 3.5f, 2, 1, 4); // Y=3..3.5 under Tier 1
        p.SpawnTouchWallCol(inter, 34f, 1f, 3f);            // Y=0..3
        SetAtomId(p.Spawn(p.Atom, col, 36.5f, 1.5f), 2);

        p.Spawn(p.Slope45, env, 40f, 0f);                  // Parapet ramp X=39..41, Y=0..2
        p.Spawn(p.BouncePadMini, inter, 44f, 0f);          // Easy hop back up to Tier 1/2

        // Battlement Tier 1 (Y = 4)
        p.Spawn(p.OneWayPlatformLarge, env, 22f, 4f);      // X=22..30
        p.Spawn(p.Ledge8, env, 30f, 4f);                   // X=30..34
        p.Spawn(p.OneWayPlatform, env, 34f, 4f);           // X=34..38
        SetAtomId(p.Spawn(p.Atom, col, 25.5f, 5.2f), 3);

        // Doorway #3 on Tier 1 (X = 30)
        p.SpawnBlock(p.BlockCyan, env, 30f, 7.5f, 2, 1, 4); // Y=7..7.5 under Tier 2
        p.SpawnTouchWallCol(inter, 30f, 5f, 7f);            // Y=4..7
        p.Spawn(p.BouncePadMini, inter, 37f, 4f);          // Springs up to Tier 2 (Y=8)

        // Battlement Tier 2 (Y = 8) & Watchtower (Y = 12)
        p.Spawn(p.Ledge8, env, 26f, 8f);                   // X=26..30
        p.Spawn(p.Ledge8, env, 30f, 8f);                   // X=30..34
        p.Spawn(p.OneWayPlatform, env, 34f, 8f);           // X=34..38
        p.Spawn(p.OneWayPlatform, env, 31f, 12f);          // X=31..35 (step up to Watchtower)
        p.Spawn(p.Ledge8, env, 35f, 12f);                  // Watchtower Launch X=35..39, Y=12

        // Glide Path Banner & Hidden Upper Parapet (X = 51..55, Y = 10)
        p.SpawnTouchWallCol(inter, 45f, 12f, 14f);         // Floating banner on glide trajectory
        p.Spawn(p.Ledge8, env, 51f, 10f);                  // Hidden Parapet X=51..55, Y=10
        p.SpawnBlock(p.BlockGreen, env, 52.5f, 17f, 2, 5, 4); // Canopy hanger from Y=17 roof down to Y=14.5
        p.SpawnBlock(p.BlockGreen, env, 51f, 14.5f, 8, 1, 4); // Decorative Green canopy over Parapet
        SetAtomId(p.Spawn(p.Atom, col, 53f, 11.2f), 4);

        // Guarded Gatehouse (X = 57..66)
        p.Spawn(p.FireJet, haz, 56f, 0f);
        GameObject saw1 = p.Spawn(p.SawSlow, haz, 59f, 1.0f);
        ConfigureSaw(saw1, new Vector3(3.5f, 0f, 0f), 2.5f);

        // Yellow DashWall Gate at X = 65 (Y = 0..4) under Gatehouse Arch
        p.SpawnBlock(p.BlockCyan, env, 65f, 13f, 2, 18, 4); // Y=4..13 arch wall
        for (int y = 1; y <= 4; y++) p.Spawn(p.DashWall, inter, 65f, y); // Y=0..4
        SetAtomId(p.Spawn(p.Atom, col, 69f, 1.5f), 5);

        // =====================================================================
        // SANCTUARY 1: RAMPART CLOISTER (X = 76 to 88, Y = 0 to 9)
        // =====================================================================
        p.SpawnGroundSized(env, 76f, 0f, 12);              // X=76..88, Y=-1..0
        p.SpawnCeilingSized(env, 76f, 9f, 12);             // X=76..88, Y=8..9
        p.SpawnBlock(p.BlockGreen, env, 77f, 8f, 2, 4, 4); // Sanctuary arch capital left
        p.SpawnBlock(p.BlockGreen, env, 86f, 8f, 2, 4, 4); // Sanctuary arch capital right
        // Solid stone masonry separating Ramparts/Sanctuary (Y=-1) from Alcove #1 ceiling (Y=-2)
        p.SpawnBlock(p.BlockCyan, env, 68f, -1f, 44, 2, 4); // X=68..90, Y=-2..-1
        GameObject cp1 = p.Spawn(p.Checkpoint, cp, 82f, 1.5f);
        cp1.name = "Checkpoint_1";

        // =====================================================================
        // ZONE 2: THE SUNKEN CATACOMBS (X = 88 to 165, Y = -16 to 9)
        // =====================================================================
        // Camera Zones (Tight underground corridors, zero overlap into Zone 1 Courtyard)
        GameObject czTightAlcove = p.Spawn(p.CameraZoneTight, flow, 79f, -5.5f);
        czTightAlcove.name = "CameraZone_Tight_Alcove1";
        SetTriggerBoxSize(czTightAlcove, new Vector3(22f, 5.5f, 10f));

        GameObject czTightMain = p.Spawn(p.CameraZoneTight, flow, 128f, -4.5f);
        czTightMain.name = "CameraZone_Tight_Catacombs";
        SetTriggerBoxSize(czTightMain, new Vector3(76f, 23f, 10f));

        // Catacombs Ceiling Cap (X = 88..165, Y = 8..9)
        p.Spawn(p.Ceiling20, env, 88f, 9f);
        p.Spawn(p.Ceiling20, env, 108f, 9f);
        p.Spawn(p.Ceiling20, env, 128f, 9f);
        p.SpawnCeilingSized(env, 148f, 9f, 17);

        // Descent Shaft Framing (X = 90..103)
        p.SpawnBlock(p.BlockCyan, env, 88f, 0f, 4, 1, 4);    // Entry bridge left X=88..90, Y=-0.5..0
        p.SpawnBlock(p.BlockCyan, env, 88f, -0.5f, 4, 1, 4);  // Support X=88..90, Y=-1..-0.5
        p.SpawnBlock(p.BlockCyan, env, 91f, 0f, 6, 1, 4);    // Entry bridge right X=91..94, Y=-0.5..0

        // Left Shaft Wall (X=90..91) with Alcove #1 doorway at Y=-8..-4
        p.SpawnBlock(p.BlockMagenta, env, 90f, 0f, 2, 6, 4);   // Y=-3..0
        p.SpawnBlock(p.BlockMagenta, env, 90f, -3f, 2, 2, 4);  // Lintel Y=-4..-3
        for (int y = -4; y >= -7; y--) p.Spawn(p.DashWall, inter, 90f, y); // Alcove #1 Yellow seal Y=-8..-4
        p.SpawnBlock(p.BlockMagenta, env, 90f, -8f, 2, 16, 4); // Lower left wall Y=-16..-8

        // Right Shaft Wall (X=102..103) with Alcove #2 doorway at Y=-5..-1
        p.SpawnBlock(p.BlockCyan, env, 102f, 0f, 2, 2, 4);     // Lintel Y=-1..0
        for (int y = -1; y >= -4; y--) p.Spawn(p.DashWall, inter, 102f, y); // Alcove #2 Yellow seal Y=-5..-1
        p.SpawnBlock(p.BlockCyan, env, 102f, -5f, 2, 8, 4);    // Mid right wall Y=-9..-5 (leaves Y=-15..-9 open!)

        // Shaft Stacked Floors & Ground Pound Decks (X = 91..102)
        p.Spawn(p.OneWayPlatformLarge, env, 94f, 0f);          // Drop-through Entry X=94..102, Y=0

        // Breakable Deck 1 (Y = -3): Solid shoulders + 6m DashWall floor
        p.SpawnBlock(p.BlockCyan, env, 91f, -3f, 6, 2, 4);     // X=91..94, Y=-4..-3
        for (int x = 94; x <= 99; x++) p.Spawn(p.DashWall, inter, x, -3f); // Y=-4..-3
        p.SpawnBlock(p.BlockCyan, env, 100f, -3f, 4, 2, 4);    // X=100..102, Y=-4..-3
        SetAtomId(p.Spawn(p.Atom, col, 97f, -2f), 6);

        // Alcove Access Ledges inside Shaft
        p.Spawn(p.OneWayPlatformShort, env, 91f, -8f);         // Left step to Alcove #1 (X=91..93)
        p.Spawn(p.OneWayPlatformShort, env, 100f, -5f);        // Right step to Alcove #2 (X=100..102)

        // Breakable Deck 2 (Y = -8 OneWay -> Y = -11 CrackedBlocks_4x4 Vault Cap)
        p.Spawn(p.OneWayPlatform, env, 95f, -8f);              // X=95..99, Y=-8
        p.SpawnBlock(p.BlockMagenta, env, 91f, -11f, 8, 4, 4); // X=91..95, Y=-13..-11
        p.Spawn(p.CrackedBlocks4x4, inter, 95f, -11f);         // X=95..97, Y=-13..-11
        p.Spawn(p.CrackedBlocks4x4, inter, 97f, -11f);         // X=97..99, Y=-13..-11
        p.SpawnBlock(p.BlockMagenta, env, 99f, -11f, 6, 4, 4); // X=99..102, Y=-13..-11

        // ---------------------------------------------------------------------
        // SECRET ALCOVE #1: THE CURSED VAULT (X = 68..90, Y = -8..-2)
        // ---------------------------------------------------------------------
        p.SpawnGroundSized(env, 68f, -8f, 22);                 // Floor Y=-9..-8
        p.SpawnCeilingSized(env, 68f, -2f, 22);                // Ceiling Y=-3..-2 (5m headroom)
        p.SpawnBlock(p.BlockMagenta, env, 67f, -2f, 2, 14, 4); // Left end wall X=67..68, Y=-9..-2
        // Clean structural piers beneath Alcove #1 down to bedrock (Y=-16..-9)
        foreach (float ax in new[] { 67f, 77f, 88f })
            p.SpawnBlock(p.BlockMagenta, env, ax, -9f, 2, 14, 4);

        // Timed Traps & Rewards inside Alcove #1
        p.Spawn(p.FireJetDown, haz, 84f, -3f);
        GameObject sawAlcove = p.Spawn(p.SawSlow, haz, 80f, -7.2f);
        ConfigureSaw(sawAlcove, new Vector3(-5f, 0f, 0f), 2.6f);
        SetAtomId(p.Spawn(p.Atom, col, 76f, -5.0f), 7);
        p.SpawnBlock(p.BlockMagenta, env, 69.5f, -7.5f, 4, 1, 4); // Shrine pedestal X=69.5..71.5
        SetDiamondId(p.Spawn(p.Diamond, col, 70.5f, -6.5f), 1);

        // ---------------------------------------------------------------------
        // SECRET ALCOVE #2: TOUCH-BLOCK SPRINT GALLERY (X = 103..140, Y = -5..1)
        // ---------------------------------------------------------------------
        p.SpawnGroundSized(env, 103f, -5f, 31);                // Floor X=103..134, Y=-6..-5 (chute X=134..140)
        p.Spawn(p.Ceiling20, env, 103f, 1f);                   // Ceiling X=103..123, Y=0..1
        p.Spawn(p.Ceiling20, env, 123f, 1f);                   // Ceiling X=123..143, Y=0..1
        p.SpawnBlock(p.BlockCyan, env, 140f, 0f, 2, 12, 4);    // Right chute wall X=140..141, Y=-6..0

        p.Spawn(p.DashBoostFast, inter, 107f, -3.5f);
        p.SpawnTouchWallCol(inter, 113f, -4f, -1f);
        SetAtomId(p.Spawn(p.Atom, col, 115.5f, -3.5f), 8);
        p.SpawnTouchWallCol(inter, 118f, -4f, -1f);
        SetAtomId(p.Spawn(p.Atom, col, 121f, -3.5f), 9);
        p.SpawnTouchWallCol(inter, 123f, -4f, -1f);
        p.SpawnTouchWallCol(inter, 124f, -4f, -1f);
        SetAtomId(p.Spawn(p.Atom, col, 126.5f, -3.5f), 10);
        p.SpawnTouchWallCol(inter, 129f, -4f, -1f);

        // ---------------------------------------------------------------------
        // MAIN LOWER CATACOMB HALL & EXIT CHIMNEY (X = 91..165, Y = -15..-6)
        // ---------------------------------------------------------------------
        p.SpawnGroundSized(env, 91f, -15f, 11);                // Floor under shaft X=91..102
        p.Spawn(p.Ground20, env, 102f, -15f);                  // X=102..122
        p.Spawn(p.Ground20, env, 122f, -15f);                  // X=122..142
        p.Spawn(p.Ground20, env, 142f, -15f);                  // X=142..162
        p.SpawnGroundSized(env, 162f, -15f, 4);                // X=162..166

        // Ceiling separating Upper Gallery & Lower Hall (leaves X=134..140 chute & X=160..165 chimney)
        p.SpawnCeilingSized(env, 103f, -6f, 31);               // X=103..134, Y=-7..-6
        p.Spawn(p.Ceiling20, env, 140f, -6f);                  // X=140..160, Y=-7..-6

        // Catacomb Hall Hazards & Door
        p.Spawn(p.StompBlock, haz, 115f, -10.5f);              // Bottom slams to Y=-15.0
        p.Spawn(p.FireJet, haz, 123f, -15f);
        p.Spawn(p.OneWayPlatform, env, 126f, -11.5f);
        GameObject sawCat = p.Spawn(p.SawSlow, haz, 145f, -14.1f);
        ConfigureSaw(sawCat, new Vector3(6f, 0f, 0f), 2.8f);

        p.SpawnBlock(p.BlockMagenta, env, 156f, -7f, 2, 8, 4); // Doorway lintel Y=-11..-7
        p.SpawnTouchWallCol(inter, 156f, -14f, -11f);          // Smashable door Y=-15..-11

        // Exit Chimney (X=160..165, Y=-15..0)
        p.SpawnBlock(p.BlockCyan, env, 159f, 0f, 2, 12, 4);    // Left chimney wall Y=-6..0
        p.SpawnBlock(p.BlockCyan, env, 165f, -1f, 2, 28, 4);   // Right chimney wall Y=-15..-1 (flush below Sanctuary 2 floor)
        p.Spawn(p.BouncePadMini, inter, 162.5f, -15f);
        p.Spawn(p.DashLaneUp, inter, 162.5f, -10f);
        GameObject czClimbCat = p.Spawn(p.CameraZoneClimb, flow, 162.5f, -6f);
        czClimbCat.name = "CameraZone_Climb_CatacombExit";
        SetTriggerBoxSize(czClimbCat, new Vector3(10f, 16f, 10f));

        // =====================================================================
        // SANCTUARY 2: CLOCKWORK VESTIBULE (X = 165 to 179, Y = 0 to 9)
        // =====================================================================
        p.SpawnGroundSized(env, 165f, 0f, 14);                 // X=165..179, Y=-1..0
        p.SpawnCeilingSized(env, 165f, 9f, 13);                // X=165..178, Y=8..9 (flush at X=178 wall)
        p.SpawnBlock(p.BlockGreen, env, 166f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockGreen, env, 176f, 8f, 2, 4, 4);
        // Sanctuary 2 center foundation pier (X=178 is built by Zone 3)
        p.Spawn(p.Wall12, env, 171f, -1f);
        p.SpawnBlock(p.BlockCyan, env, 171f, -13f, 2, 6, 4);
        GameObject cp2 = p.Spawn(p.Checkpoint, cp, 172f, 1.5f);
        cp2.name = "Checkpoint_2";

        // =====================================================================
        // ZONE 3: THE CLOCKWORK SPIRE (X = 179 to 248, Y = 0 to 32)
        // =====================================================================
        // Camera Zones: Climb transitioning cleanly to Wide at X = 206
        GameObject czClimbSpire = p.Spawn(p.CameraZoneClimb, flow, 193f, 14f);
        czClimbSpire.name = "CameraZone_Climb_Spire";
        SetTriggerBoxSize(czClimbSpire, new Vector3(26f, 28f, 10f));

        GameObject czWideSpire = p.Spawn(p.CameraZoneWide, flow, 227f, 16f);
        czWideSpire.name = "CameraZone_Wide_Spire";
        SetTriggerBoxSize(czWideSpire, new Vector3(42f, 32f, 10f));

        // Outer Tower Masonry & Foundations
        p.Spawn(p.Ground20, env, 179f, 0f);
        p.Spawn(p.Ground20, env, 199f, 0f);
        p.Spawn(p.Ground20, env, 219f, 0f);
        p.SpawnGroundSized(env, 239f, 0f, 9);                  // X=179..248, Y=-1..0

        foreach (float px in new[] { 178f, 198f, 218f, 238f, 247f })
        {
            p.Spawn(p.Wall12, env, px, -1f);
            p.SpawnBlock(p.BlockGreen, env, px, -13f, 2, 6, 4);
        }

        // Left & Right Tower Walls (leaving Y=0..6 arches open)
        p.Spawn(p.Wall12, env, 178f, 18f);                     // Y=6..18
        p.Spawn(p.Wall12, env, 178f, 30f);                     // Y=18..30
        p.SpawnBlock(p.BlockGreen, env, 178f, 32f, 2, 4, 4);   // Y=30..32

        p.Spawn(p.Wall12, env, 247f, 18f);                     // Y=6..18
        p.Spawn(p.Wall12, env, 247f, 30f);                     // Y=18..30
        p.SpawnBlock(p.BlockGreen, env, 247f, 32f, 2, 4, 4);   // Y=30..32

        // Tower Crown Roof (Y=31..32) + Decorative Clockwork Battlements & Rafter Ribs
        p.Spawn(p.Ceiling20, env, 179f, 32f);
        p.Spawn(p.Ceiling20, env, 199f, 32f);
        p.Spawn(p.Ceiling20, env, 219f, 32f);
        p.SpawnCeilingSized(env, 239f, 32f, 8);                // X=239..247 (flush at X=247 wall)
        for (int mx = 181; mx <= 245; mx += 4)
            p.SpawnBlock(p.BlockGreen, env, mx, 33f, 2, 2, 4);

        // 1) Twin-Wall Jump Shaft (X=186..192, 4m interior width)
        p.Spawn(p.Wall12, env, 186f, 16f);                     // X=186..187, Y=4..16 (leaves ground arch 0..4)
        p.Spawn(p.Wall12, env, 191f, 12f);                     // X=191..192, Y=0..12
        p.Spawn(p.BouncePadMini, inter, 189f, 0f);
        p.Spawn(p.TouchWallBlue, inter, 189f, 7f);
        p.Spawn(p.TouchWallBlue, inter, 189f, 10f);
        SetAtomId(p.Spawn(p.Atom, col, 189f, 11.5f), 11);

        // Shaft Top Landing & Elevator
        p.Spawn(p.Ledge8, env, 192f, 12f);                     // X=192..196, Y=11.5..12
        GameObject elevator = p.Spawn(p.MovingPlatform, env, 200f, 12.5f);
        ConfigureMovingPlatform(elevator, new Vector3(0f, 9.5f, 0f), 3.5f, 0.5f); // Y=12.5..22.0

        // 2) Crumbling Bridge + SpinArm Gauntlet (Y = 22)
        p.Spawn(p.CrumblingBlock, env, 206f, 22f);
        p.Spawn(p.CrumblingBlock, env, 210f, 22f);
        p.Spawn(p.CrumblingBlock, env, 214f, 22f);
        GameObject spinArm = p.Spawn(p.SpinArm, haz, 211f, 17.2f);
        ConfigureSpinArm(spinArm, 75f);

        p.Spawn(p.Ledge8, env, 218f, 23f);                     // Upper Launch Perch X=218..222, Y=23

        // 3) Lower Safety Mezzanine & Ground Trap Gauntlet (Softlock-proof recovery!)
        p.Spawn(p.OneWayPlatformLarge, env, 204f, 10f);
        p.Spawn(p.OneWayPlatformLarge, env, 220f, 10f);
        p.Spawn(p.BouncePad, inter, 224f, 10f);                // Springs back toward upper tiers
        p.Spawn(p.StompBlock, haz, 210f, 4.5f);                // Slams down to Y=0

        p.SpawnBlock(p.BlockGreen, env, 237.5f, 1.5f, 4, 3, 4); // Pedestal X=237.5..239.5, Y=0..1.5
        GameObject cannonLow = p.Spawn(p.Cannon, haz, 238.5f, 2.2f);
        ConfigureCannon(cannonLow, new Vector3(0f, 0f, 180f), 8f, 2.2f, 16f); // Fires Left
        p.Spawn(p.BouncePadMega, inter, 243f, 0f);             // High recovery spring

        // 4) Optional High-Altitude Glide + Mid-Air Dash Shortcut (X = 222..244, Y = 23..30)
        SetAtomId(p.Spawn(p.Atom, col, 226f, 25.5f), 12);
        SetAtomId(p.Spawn(p.Atom, col, 230.5f, 25.5f), 13);

        // Suspended Clockwork Rafter Arch & Mid-Air Dash Gate (X=234)
        p.SpawnBlock(p.BlockGreen, env, 234f, 31f, 2, 8, 4);   // Rafter truss Y=27..31
        p.Spawn(p.TouchWallBlue, inter, 234f, 27f);            // Top banner
        for (int y = 24; y <= 26; y++) p.Spawn(p.DashWall, inter, 234f, y); // Mid-air dash barrier Y=23..26

        p.Spawn(p.OneWayPlatformLarge, env, 236f, 24f);        // Reward Rafter X=236..244
        SetDiamondId(p.Spawn(p.Diamond, col, 240f, 26.2f), 2);

        p.SpawnBlock(p.BlockGreen, env, 245f, 24.5f, 4, 1, 4); // Cannon wall bracket X=245..247, Y=24..24.5
        GameObject cannonHigh = p.Spawn(p.CannonFast, haz, 245.8f, 25.2f);
        ConfigureCannon(cannonHigh, new Vector3(0f, 0f, 180f), 10f, 2.4f, 16f); // Fires Left across upper rafter

        // Descent cushions & return pads down right tower side
        p.Spawn(p.OneWayPlatformLarge, env, 237f, 15f);
        p.Spawn(p.OneWayPlatformLarge, env, 237f, 8f);
        p.Spawn(p.BouncePad, inter, 240f, 8f);

        // =====================================================================
        // SANCTUARY 3: CITADEL ANTECHAMBER (X = 248 to 262, Y = 0 to 9)
        // =====================================================================
        p.SpawnGroundSized(env, 248f, 0f, 14);                 // X=248..262, Y=-1..0
        p.SpawnCeilingSized(env, 248f, 9f, 14);                // X=248..262, Y=8..9
        p.SpawnBlock(p.BlockCyan, env, 249f, 8f, 2, 4, 4);
        p.SpawnBlock(p.BlockMagenta, env, 259f, 8f, 2, 4, 4);
        p.Spawn(p.Wall12, env, 254f, -1f);
        p.SpawnBlock(p.BlockCyan, env, 254f, -13f, 2, 6, 4);
        GameObject cp3 = p.Spawn(p.Checkpoint, cp, 255f, 1.5f);
        cp3.name = "Checkpoint_3";

        // =====================================================================
        // ZONE 4: THE CITADEL GAUNTLET (X = 262 to 334, Y = -10 to 15)
        // =====================================================================
        GameObject czWideFin = p.Spawn(p.CameraZoneWide, flow, 300f, 2f);
        czWideFin.name = "CameraZone_Wide_Citadel";
        SetTriggerBoxSize(czWideFin, new Vector3(76f, 28f, 10f));

        // Upper Supersonic Hall (X = 262..294, Y = 0..12)
        p.Spawn(p.Ground20, env, 262f, 0f);                    // X=262..282
        p.SpawnGroundSized(env, 282f, 0f, 12);                 // X=282..294
        p.Spawn(p.Ceiling20, env, 262f, 12f);                  // X=262..282, Y=11..12
        p.Spawn(p.Ceiling20, env, 282f, 12f);                  // X=282..302, Y=11..12

        // Supporting Citadel Piers beneath Upper Hall
        p.Spawn(p.Wall12, env, 262f, -1f);
        p.SpawnBlock(p.BlockCyan, env, 262f, -13f, 2, 6, 4);
        p.Spawn(p.Wall12, env, 278f, -1f);
        p.SpawnBlock(p.BlockMagenta, env, 278f, -13f, 2, 6, 4);

        // Supersonic Chevron Boost Pad Smashes
        p.Spawn(p.DashBoostFast, inter, 266f, 1.5f);

        // Barricade #1 (X = 272): Purple TouchWall door + Citadel arch
        p.SpawnBlock(p.BlockCyan, env, 272f, 11f, 2, 14, 4);   // Y=4..11
        p.SpawnTouchWallCol(inter, 272f, 1f, 4f);              // Y=0..4
        SetAtomId(p.Spawn(p.Atom, col, 274.5f, 1.8f), 14);

        // Barricade #2 (X = 277): Yellow DashWall door + Royal Magenta arch
        p.SpawnBlock(p.BlockMagenta, env, 277f, 11f, 2, 14, 4); // Y=4..11
        for (int y = 1; y <= 4; y++) p.Spawn(p.DashWall, inter, 277f, y); // Y=0..4

        GameObject dz2 = p.Spawn(p.DashZone, inter, 281f, 1.5f);
        var dzComp = dz2.GetComponent<DashZone>();
        if (dzComp != null)
        {
            dzComp.Mode = DashZone.ZoneMode.Boost;
            dzComp.BoostSpeed = 30f;
            dzComp.BoostTime = 0.25f;
            dzComp.ExitSpeed = 20f;
            dzComp.Size = new Vector2(4f, 3f);
            dzComp.Apply();
            EditorUtility.SetDirty(dzComp);
        }
        SetAtomId(p.Spawn(p.Atom, col, 284.5f, 1.8f), 15);

        // Barricade #3 (X = 287..288): Alternating 2-thick Purple & Yellow wall
        p.SpawnBlock(p.BlockCyan, env, 287f, 11f, 4, 14, 4);   // Y=4..11 arch
        for (int x = 287; x <= 288; x++)
        {
            for (int y = 1; y <= 4; y++)
            {
                if ((x + y) % 2 == 0) p.Spawn(p.TouchWallBlue, inter, x, y);
                else p.Spawn(p.DashWall, inter, x, y);
            }
        }

        // Plunge Shaft (X = 294..302, Y = 0 down to -10)
        p.SpawnBlock(p.BlockCyan, env, 293f, -1f, 2, 18, 4);   // Left shaft wall X=293..294, Y=-10..-1
        p.SpawnBlock(p.BlockCyan, env, 293f, -10f, 2, 12, 4);  // Extends to -16
        p.SpawnBlock(p.BlockCyan, env, 302f, 11f, 2, 32, 4);   // Right shaft wall X=302..303, Y=-5..11 (leaves -10..-5 arch!)
        SetAtomId(p.Spawn(p.Atom, col, 298f, 2.2f), 16);

        // Multi-Layer Ground Pound Drop
        for (int x = 294; x <= 301; x++) p.Spawn(p.DashWall, inter, x, -1f);      // Layer 1: Y=-2..-1
        for (int x = 294; x <= 301; x++) p.Spawn(p.TouchWallBlue, inter, x, -4f); // Layer 2: Y=-5..-4
        p.Spawn(p.DashWall, inter, 294f, -7f);
        p.Spawn(p.DashWall, inter, 295f, -7f);
        p.Spawn(p.DashWall, inter, 294f, -8f);
        p.Spawn(p.DashWall, inter, 295f, -8f);
        p.Spawn(p.CrackedBlocks4x4, inter, 296f, -7f);         // Layer 3 Heavy Core: X=296..298, Y=-9..-7
        p.Spawn(p.CrackedBlocks4x4, inter, 298f, -7f);         // Layer 3 Heavy Core: X=298..300, Y=-9..-7
        p.Spawn(p.DashWall, inter, 300f, -7f);
        p.Spawn(p.DashWall, inter, 301f, -7f);
        p.Spawn(p.DashWall, inter, 300f, -8f);
        p.Spawn(p.DashWall, inter, 301f, -8f);

        // Safe Throne Arena (X = 294..334, Y = -10..15)
        p.SpawnGroundSized(env, 294f, -10f, 8);                // Plunge floor X=294..302
        p.Spawn(p.Ground20, env, 302f, -10f);                  // Arena floor X=302..322
        p.SpawnGroundSized(env, 322f, -10f, 12);               // Arena floor X=322..334
        p.SpawnBlock(p.BlockMagenta, env, 302f, 14f, 2, 6, 4); // Connects X=302..303 Y=11..14 cleanly to Arena Vault
        p.Spawn(p.Ceiling20, env, 302f, 15f);                  // Vault Roof X=302..322, Y=14..15
        p.SpawnCeilingSized(env, 322f, 15f, 12);               // Vault Roof X=322..334, Y=14..15

        // Back Wall sealing Citadel behind End_Portal (X = 334..335)
        p.SpawnBlock(p.BlockCyan, env, 334f, 15f, 2, 2, 4);    // Top corner cap Y=14..15
        p.Spawn(p.Wall12, env, 334f, 14f);                     // Y=2..14
        p.Spawn(p.Wall12, env, 334f, 2f);                      // Y=-10..2
        p.SpawnBlock(p.BlockCyan, env, 334f, -10f, 2, 12, 4);  // Y=-16..-10

        // Arena Foundations (-16..-11)
        foreach (float px in new[] { 302f, 317f, 325f })
            p.SpawnBlock(p.BlockCyan, env, px, -11f, 2, 10, 4);

        // Celebratory Launch & Victory Dais (Y = 0)
        p.Spawn(p.BouncePadMega, inter, 308f, -10f);
        p.Spawn(p.BouncePad, inter, 313f, -10f);               // Backup pad
        p.Spawn(p.OneWayPlatform, env, 313f, -5f);             // Backup step

        // Diamond 3 Crown Jewel Cluster along Victory Flight Arc
        p.SpawnTouchWallBlock(inter, 313f, 315f, 1f, 3f, 1f, 1f);
        SetDiamondId(p.Spawn(p.Diamond, col, 314f, 1.5f), 3);

        // Victory Dais (X = 318..334, Y = 0) + Royal Colonnade
        p.SpawnGroundSized(env, 318f, 0f, 16);                 // X=318..334, Y=-1..0
        p.SpawnBlock(p.BlockCyan, env, 318f, -1f, 2, 18, 4);   // Dais front pillar X=318..319, Y=-10..-1
        p.SpawnBlock(p.BlockMagenta, env, 326f, -1f, 2, 18, 4); // Dais mid pillar X=326..327, Y=-10..-1
        p.SpawnBlock(p.BlockGreen, env, 321f, 14f, 24, 1, 4);  // Flush canopy under vault roof Y=13.5..14
        p.SpawnBlock(p.BlockGreen, env, 321f, 13.5f, 2, 2, 4); // Canopy left pendant
        p.SpawnBlock(p.BlockGreen, env, 332f, 13.5f, 2, 2, 4); // Canopy right pendant

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
        return $"Level_36 built at {TargetScenePath} (Validator errors: {errors})";
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
            lm.ThreeStarTime = 45f;
            lm.TwoStarTime = 75f;
            lm.OneStarTime = 120f;
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
            respawnMgr.KillY = -22f;
            respawnMgr.KillTopY = 55f;
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

    private static void EnforceZZero(Transform container)
    {
        foreach (Transform t in container.GetComponentsInChildren<Transform>(true))
        {
            Vector3 pos = t.position;
            if (Mathf.Abs(pos.z) > 0.001f)
            {
                t.position = new Vector3(pos.x, pos.y, 0f);
            }
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
        public GameObject Slope45 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/Slope_45.prefab");
        public GameObject KillPit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Foundation/KillPit.prefab");

        public GameObject OneWayPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/OneWayPlatform.prefab");
        public GameObject OneWayPlatformLarge = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Large.prefab");
        public GameObject OneWayPlatformShort = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/Variants/OneWayPlatform_Short.prefab");
        public GameObject MovingPlatform = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/MovingPlatform 1.prefab");
        public GameObject CrumblingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Platforms/CrumblingBlock.prefab");

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
        public GameObject FireJetDown = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/LEVEL/Hazards/Variants/FireJet_Down.prefab");
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
            // Preserve prefab's authored rotation (critical for FireJet_Down, DashLane_UP, etc.)
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
