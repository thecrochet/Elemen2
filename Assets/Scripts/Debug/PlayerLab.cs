using System.Collections.Generic;
using System.Reflection;
using System.Text;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

// PlayerLab: a test tool with two jobs. It needs NO test course and NO CSV.
//
//  1) POWER MIXER. Choose any mix of Fire (dash), Air (double jump + glide), Earth (ground pound),
//     Water (wall jump + gravity flip): one, two, three or all four. Keys on the top number row:
//        1 Fire   2 Air   3 Earth   4 Water   (each toggles)
//        5 all four       6 none (base ball)   0 stop mixing (the real power of the level comes back)
//     Or tick the boxes in the Inspector while playing.
//     It sets the private "has..." flags of CC_ELEMENTO from here, so it changes nothing in your code.
//
//  2) STATS SHEET. It reads the numbers in the CC_ELEMENTO Inspector (jump strength, gravity, speeds, dash ...)
//     and the DashZones of the scene, and works out heights, reaches and the gaps you can build for the
//     easy / normal / hard tiers. Change a value on the player while playing and the sheet updates.
//     It also shows the last jumps you made (edge gap, rise, moves used), so you can compare the maths with the real thing
//     just by playing, on any two platforms.
//
// Keys: F5 copies the sheet to the clipboard (paste it to me)   F6 hides / shows the panel
// Only exists in the Editor and in Development Builds, like the DebugOverlay.
public class PlayerLab : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Tooltip("The player. Empty = the first CC_ELEMENTO found in the scene.")]
    public CC_ELEMENTO Player;

    [Header("Power mixer")]
    [Tooltip("On = the boxes below decide the powers. Off = the real power of the level (Starting Power, gates) is used.")]
    public bool MixPowers = false;
    [Tooltip("Dash")] public bool Fire = true;
    [Tooltip("Double jump + glide")] public bool Air = true;
    [Tooltip("Ground pound")] public bool Earth = true;
    [Tooltip("Wall jump + gravity flip")] public bool Water = true;

    [Header("Panel")]
    public bool ShowPanel = false;

    [Tooltip("Ground that counts for the last-jump readout.")]
    public LayerMask GroundMask = ~0;

    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private KinematicCharacterMotor _motor;
    private readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
    private readonly HashSet<string> _warned = new HashSet<string>();
    private MethodInfo _flipMethod;
    private bool _wasMixing;

    // sheet
    private readonly StringBuilder _sheet = new StringBuilder(2048);
    private float _nextSheet;

    // last trips
    private bool _airborne;
    private Vector3 _takeoffPos;
    private Collider _takeoffGround;
    private float _takeoffSpeed;
    private float _apexY;
    private float _airTime;
    private readonly List<string> _moves = new List<string>();
    private readonly List<string> _trips = new List<string>();
    private CharacterState _prevState;
    private bool _prevGlide;
    private bool _prevDouble;
    private float _prevWallLock;

    private GUIStyle _style;
    private GUIStyle _boxStyle;

    private void Start()
    {
        if (Player == null) Player = FindFirstObjectByType<CC_ELEMENTO>();
        if (Player != null) _motor = Player.GetComponent<KinematicCharacterMotor>();

        if (Player == null)
            Debug.LogWarning("PlayerLab: no CC_ELEMENTO found.", this);

        _flipMethod = typeof(CC_ELEMENTO).GetMethod("FlipGravity", Flags);
    }

    private void Update()
    {
        if (Player == null) return;

        HandleKeys();

        if (MixPowers) ApplyMix();
        else if (_wasMixing) Player.SetPower(Player.CurrentPower);   // give the real power back
        _wasMixing = MixPowers;

        TrackTrips();

        if (Time.unscaledTime >= _nextSheet)
        {
            _nextSheet = Time.unscaledTime + 0.5f;
            BuildSheet();
        }
    }

    // ------------------------------------------------------------------
    // POWER MIXER
    // ------------------------------------------------------------------
    private void HandleKeys()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        if (k.digit1Key.wasPressedThisFrame) { MixPowers = true; Fire = !Fire; }
        if (k.digit2Key.wasPressedThisFrame) { MixPowers = true; Air = !Air; }
        if (k.digit3Key.wasPressedThisFrame) { MixPowers = true; Earth = !Earth; }
        if (k.digit4Key.wasPressedThisFrame) { MixPowers = true; Water = !Water; }
        if (k.digit5Key.wasPressedThisFrame) { MixPowers = true; Fire = Air = Earth = Water = true; }
        if (k.digit6Key.wasPressedThisFrame) { MixPowers = true; Fire = Air = Earth = Water = false; }
        if (k.digit0Key.wasPressedThisFrame) MixPowers = false;

        if (k.f6Key.wasPressedThisFrame) ShowPanel = !ShowPanel;
        if (k.f5Key.wasPressedThisFrame)
        {
            BuildSheet();
            GUIUtility.systemCopyBuffer = _sheet.ToString();
            Debug.Log("PlayerLab: the sheet is on the clipboard.");
        }
    }

    private void ApplyMix()
    {
        Set("hasDash", Fire);
        Set("hasDoubleJump", Air);
        Set("hasGlide", Air);
        Set("hasGroundPound", Earth);
        Set("hasWallJump", Water);
        Set("hasGravityFlip", Water);

        // gravity flip taken away while upside down: put the ball back on the floor
        if (!Water && Player.IsGravityFlipped && _flipMethod != null)
            _flipMethod.Invoke(Player, null);
    }

    private FieldInfo Field(string name)
    {
        if (_fields.TryGetValue(name, out FieldInfo f)) return f;

        f = typeof(CC_ELEMENTO).GetField(name, Flags);
        _fields[name] = f;

        if (f == null && _warned.Add(name))
            Debug.LogWarning("PlayerLab: CC_ELEMENTO has no field named " + name + ". Send me this line.", this);

        return f;
    }

    private void Set(string name, bool value)
    {
        FieldInfo f = Field(name);
        if (f != null) f.SetValue(Player, value);
    }

    private T Get<T>(string name, T fallback)
    {
        FieldInfo f = Field(name);
        if (f == null) return fallback;
        object value = f.GetValue(Player);
        return value is T t ? t : fallback;
    }

    // ------------------------------------------------------------------
    // MATHS (the same rules as CC_ELEMENTO.UpdateVelocity)
    // ------------------------------------------------------------------
    private enum Extra { None, Dash, DoubleJump }

    // Horizontal distance of the ball's CENTRE for a jump that lands at "landHeight" above the takeoff height
    // (0 = flat, negative = a drop). It follows the order of CC_ELEMENTO step by step, with the real fixed timestep:
    // the jump frame moves at full jump speed, then every frame gravity, air control, move.
    // startSpeed = speed at takeoff, airTarget = the speed the air control moves toward.
    private float FlatReach(float startSpeed, float airTarget, Extra extra, float landHeight, out float totalTime, out float apex)
    {
        float g = Mathf.Abs(Player.Gravity.y);
        float fallG = g * Player.FallGravityMultiplier;
        float dt = Mathf.Max(Time.fixedDeltaTime, 0.001f);

        float vy = Player.JumpStrength;
        float vx = startSpeed;
        float y = vy * dt;
        float x = vx * dt;
        float t = dt;
        apex = y;
        bool used = false;

        while (t < 8f)
        {
            if (extra != Extra.None && !used && vy <= 0f)
            {
                used = true;

                if (extra == Extra.Dash)
                {
                    // the dash holds the height, then the ball keeps the walking speed (CC_ELEMENTO OnStateExit)
                    x += Player.DashSpeed * Player.DashDuration;
                    t += Player.DashDuration;
                    vy = 0f;
                    vx = Player.MaxGroundSpeed;
                }
                else
                {
                    vy = Player.JumpStrength;
                }
            }

            vy -= (vy > 0f ? g : fallG) * dt;
            if (vy < -Player.MaxFallSpeed) vy = -Player.MaxFallSpeed;

            vx = Mathf.MoveTowards(vx, airTarget, Player.AirAcceleration * dt);

            y += vy * dt;
            x += vx * dt;
            t += dt;
            if (y > apex) apex = y;

            if (y <= landHeight && vy < 0f) break;
        }

        totalTime = t;
        return x;
    }

    private void BuildSheet()
    {
        StringBuilder s = _sheet;
        s.Length = 0;

        float g = Mathf.Abs(Player.Gravity.y);
        float walk = Player.MaxGroundSpeed;
        float run = Player.RunSpeed;
        float radius = _motor != null && _motor.Capsule != null ? _motor.Capsule.radius : 0.5f;
        float coyote = Player.CoyoteTime;

        float ledge = _motor != null ? _motor.MaxStableDistanceFromLedge : 0.5f;

        float t, apexA;
        float walkReach = FlatReach(walk, Player.MaxAirSpeed, Extra.None, 0f, out float walkTime, out float apex);
        float runReach = FlatReach(run, run, Extra.None, 0f, out t, out apexA);
        float walkDash = FlatReach(walk, Player.MaxAirSpeed, Extra.Dash, 0f, out t, out apexA);
        float runDash = FlatReach(run, run, Extra.Dash, 0f, out t, out apexA);
        float walkDouble = FlatReach(walk, Player.MaxAirSpeed, Extra.DoubleJump, 0f, out t, out apexA);
        float runDouble = FlatReach(run, run, Extra.DoubleJump, 0f, out t, out apexA);

        float jumpH = apex;
        float dashLen = Player.DashSpeed * Player.DashDuration;
        float laneUp = jumpH + dashLen + walk * walk / (2f * g);

        // the ledge allowance: the ball stays grounded up to "ledge" past an edge, and also holds on a corner when it lands
        float extraWalk = 2f * ledge + walk * coyote;
        float extraRun = 2f * ledge + run * coyote;

        s.Append("POWERS NOW: ");
        s.Append(MixPowers ? "mixed  " : "level  ");
        s.Append(Get("hasDash", false) ? "Fire " : "- ");
        s.Append((Get("hasDoubleJump", false) ? "Air " : "- "));
        s.Append(Get("hasGroundPound", false) ? "Earth " : "- ");
        s.Append(Get("hasWallJump", false) ? "Water" : "-");
        s.Append("   (keys 1-4 toggle, 5 all, 6 none, 0 level)\n\n");

        s.Append("FROM YOUR INSPECTOR VALUES  (ball radius ").Append(radius.ToString("0.00"));
        s.Append(", coyote ").Append(coyote.ToString("0.00")).Append(" s)\n");
        s.Append("Full jump (simulated with the real timestep): ").Append(jumpH.ToString("0.00")).Append(" high, in the air ").Append(walkTime.ToString("0.00")).Append(" s\n");
        s.Append("Dash: ").Append(dashLen.ToString("0.0")).Append(" long, cooldown ").Append(Player.DashCooldown.ToString("0.00")).Append(" s, super dash x2 = ").Append((dashLen * 2f).ToString("0.0")).Append('\n');
        s.Append("The dash is HORIZONTAL only. Inside a Dash Lane aimed up, a jump + dash rises about ").Append(laneUp.ToString("0.0")).Append('\n');
        s.Append("Fixed timestep ").Append(Time.fixedDeltaTime.ToString("0.0000")).Append(" s, ledge allowance ").Append(ledge.ToString("0.00")).Append(" each side\n");
        s.Append("Double jump: second jump +").Append(jumpH.ToString("0.0")).Append(" high\n");
        s.Append("Glide: falls ").Append(Player.GlideFallSpeed.ToString("0.0")).Append(" u/s, so each unit of drop gives ")
         .Append((walk / Mathf.Max(0.1f, Player.GlideFallSpeed)).ToString("0.0")).Append(" units forward (walking)\n");
        s.Append("Wall jump: ").Append(Player.WallJumpHorizontalSpeed.ToString("0")).Append(" out, ").Append(Player.WallJumpVerticalSpeed.ToString("0"))
         .Append(" up = ").Append((Player.WallJumpVerticalSpeed * Player.WallJumpVerticalSpeed / (2f * g)).ToString("0.0")).Append(" high\n");
        s.Append("Ground pound: ").Append(Player.GroundPoundSpeed.ToString("0")).Append(" u/s, bounce ")
         .Append((Player.GroundPoundBounceStrength * Player.GroundPoundBounceStrength / (2f * g)).ToString("0.0")).Append(" high\n\n");

        s.Append("REACH on flat ground (centre path | edge to edge best case = + ledge allowance both sides + coyote)\n");
        AppendReach(s, "Walking jump", walkReach, extraWalk);
        AppendReach(s, "Running jump", runReach, extraRun);
        AppendReach(s, "Walk jump + dash", walkDash, extraWalk);
        AppendReach(s, "Run jump + dash", runDash, extraRun);
        AppendReach(s, "Walk double jump", walkDouble, extraWalk);
        AppendReach(s, "Run double jump", runDouble, extraRun);
        if (!Player.UseRun) s.Append("(UseRun is OFF on the player, so running is not available)\n");

        s.Append("\nGAPS TO BUILD, edge to edge, flat (best case x tier; easy 65%, normal 85%, hard 95%)\n");
        AppendTiers(s, "Walking jump", walkReach + extraWalk);
        AppendTiers(s, "Running jump", runReach + extraRun);
        AppendTiers(s, "Walk jump + dash", walkDash + extraWalk);
        AppendTiers(s, "Run jump + dash", runDash + extraRun);
        AppendTiers(s, "Run double jump", runDouble + extraRun);

        // what the camera shows
        Camera cam = Camera.main;
        Unity.Cinemachine.CinemachinePositionComposer composer = FindFirstObjectByType<Unity.Cinemachine.CinemachinePositionComposer>();
        if (cam != null && composer != null)
        {
            float h = 2f * composer.CameraDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            s.Append("\nSCREEN at distance ").Append(composer.CameraDistance.ToString("0")).Append(": ")
             .Append((h * cam.aspect).ToString("0.0")).Append(" wide x ").Append(h.ToString("0.0")).Append(" tall (aspect ").Append(cam.aspect.ToString("0.00")).Append(")\n");
        }

        // the dash zones of the scene
        DashZone[] zones = FindObjectsByType<DashZone>(FindObjectsSortMode.None);
        if (zones.Length > 0)
        {
            s.Append("\nDASH ZONES IN THIS SCENE\n");
            for (int i = 0; i < zones.Length; i++)
            {
                DashZone z = zones[i];
                Vector3 p = z.transform.position;
                s.Append(z.name).Append(" (").Append(p.x.ToString("0.0")).Append(", ").Append(p.y.ToString("0.0")).Append(") ").Append(z.Mode);

                if (z.Mode == DashZone.ZoneMode.Boost)
                    s.Append(": carries ").Append((z.BoostSpeed * z.BoostTime).ToString("0.0")).Append(" then keeps ").Append(z.ExitSpeed.ToString("0")).Append(" u/s");
                else if (z.Mode == DashZone.ZoneMode.Hold)
                    s.Append(": throws ").Append((z.HoldLaunchSpeed * z.HoldLaunchTime).ToString("0.0")).Append(" then keeps ").Append(z.HoldExitSpeed.ToString("0")).Append(" u/s");

                s.Append('\n');
            }
        }

        s.Append("\nLAST JUMPS (edge = empty space between the two platforms you used)\n");
        if (_trips.Count == 0) s.Append("none yet, just play\n");
        for (int i = _trips.Count - 1; i >= 0; i--)
            s.Append(_trips[i]).Append('\n');
    }

    private static void AppendReach(StringBuilder s, string label, float centre, float coyoteDistance)
    {
        s.Append(label.PadRight(18)).Append(centre.ToString("0.0")).Append(" | ").Append((centre + coyoteDistance).ToString("0.0")).Append('\n');
    }

    private static void AppendTiers(StringBuilder s, string label, float best)
    {
        s.Append(label.PadRight(18)).Append("easy ").Append((best * 0.65f).ToString("0.0"))
         .Append("   normal ").Append((best * 0.85f).ToString("0.0"))
         .Append("   hard ").Append((best * 0.95f).ToString("0.0")).Append("   max ").Append(best.ToString("0.0")).Append('\n');
    }

    // ------------------------------------------------------------------
    // LAST JUMPS (no course needed: play on any platforms)
    // ------------------------------------------------------------------
    private void TrackTrips()
    {
        if (_motor == null) return;

        bool grounded = _motor.GroundingStatus.IsStableOnGround;
        Vector3 pos = _motor.TransientPosition;

        if (!grounded && !_airborne)
        {
            _airborne = true;
            _takeoffPos = pos;
            _takeoffGround = _motor.GroundingStatus.GroundCollider;
            _takeoffSpeed = Mathf.Abs(_motor.BaseVelocity.x);
            _apexY = pos.y;
            _airTime = 0f;
            _moves.Clear();
        }

        if (_airborne)
        {
            _airTime += Time.deltaTime;
            if (pos.y > _apexY) _apexY = pos.y;
            DetectMoves();
        }

        if (grounded && _airborne)
        {
            _airborne = false;
            Collider landed = _motor.GroundingStatus.GroundCollider;

            // a trip with no real travel (a hop in place) is not worth a line
            float dx = pos.x - _takeoffPos.x;
            if (Mathf.Abs(dx) > 1f || Mathf.Abs(pos.y - _takeoffPos.y) > 1f)
            {
                string edge = "same platform";
                if (landed != null && _takeoffGround != null && landed != _takeoffGround)
                {
                    float gap = dx > 0f
                        ? landed.bounds.min.x - _takeoffGround.bounds.max.x
                        : _takeoffGround.bounds.min.x - landed.bounds.max.x;
                    edge = "edge " + gap.ToString("0.0");
                }

                string moves = _moves.Count == 0 ? "jump" : string.Join("+", _moves);
                string line = (_takeoffSpeed > (Player.MaxGroundSpeed + Player.RunSpeed) * 0.5f ? "run " : "walk ")
                    + moves + ": " + edge
                    + ", centre " + Mathf.Abs(dx).ToString("0.0")
                    + ", rise " + (pos.y - _takeoffPos.y).ToString("0.0")
                    + ", apex " + (_apexY - _takeoffPos.y).ToString("0.0")
                    + ", " + _airTime.ToString("0.00") + " s";

                _trips.Add(line);
                if (_trips.Count > 6) _trips.RemoveAt(0);
            }
        }
    }

    private void AddMove(string m)
    {
        if (_moves.Count == 0 || _moves[_moves.Count - 1] != m) _moves.Add(m);
    }

    private void DetectMoves()
    {
        CharacterState state = Player.CurrentState;
        if (state == CharacterState.Dashing && _prevState != CharacterState.Dashing) AddMove("dash");
        if (state == CharacterState.GroundPound && _prevState != CharacterState.GroundPound) AddMove("pound");
        _prevState = state;

        if (Player.IsGliding && !_prevGlide) AddMove("glide");
        _prevGlide = Player.IsGliding;

        bool dj = Get("_doubleJumpConsumed", false);
        if (dj && !_prevDouble) AddMove("djump");
        _prevDouble = dj;

        float wl = Get("_wallJumpLockTimer", 0f);
        if (wl > _prevWallLock) AddMove("walljump");
        _prevWallLock = wl;
    }

    // ------------------------------------------------------------------
    // PANEL
    // ------------------------------------------------------------------
    private void OnGUI()
    {
        if (!ShowPanel || Player == null) return;

        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = false, wordWrap = false };
            _style.normal.textColor = Color.white;
            _boxStyle = new GUIStyle(GUI.skin.box);
        }

        string text = _sheet.ToString();
        Vector2 size = _style.CalcSize(new GUIContent(text));
        Rect rect = new Rect(10f, 10f, size.x + 16f, size.y + 12f);
        GUI.Box(rect, GUIContent.none, _boxStyle);
        GUI.Label(new Rect(rect.x + 8f, rect.y + 6f, size.x + 4f, size.y + 4f), text, _style);
    }
#endif
}
