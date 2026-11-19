using System.Collections.Generic;
using System.Reflection;
using System.Text;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

// JumpProbe v2: measures what the ball REALLY does with every power, so the level design "bible" can be exact.
// Put it on any object in a test scene (for example the DebugOverlay object) and play.
//
// It records every TRIP (from leaving the ground to landing) with the moves used in it, in order:
//   JUMP (nothing else), DASH, DJUMP (double jump), GLIDE, POUND (ground pound), WALLJUMP, CLIMB, FLIP (gravity flip).
// For each combination, walking or running, and each landing height (rise -6 ... +6) it keeps the best:
//   Edge gap   the empty space between the platform you left and the one you landed on (what you BUILD).
//              Real colliders, so coyote time and the ball's width are included.
//   Centre     how far the ball's centre travelled (what the metrics sheet calls the gap).
//   Apex       the highest point above the takeoff height.
// It also keeps the best single numbers of each move (dash length, glide time and fall speed, ...).
//
// ALL POWERS: with the switch on, every ability is unlocked at the same time, whatever the Starting Power is.
// It does this by setting the private "has..." flags of CC_ELEMENTO from here (Editor only), so it changes nothing in your code.
//
// Keys:  F4 clear   F5 copy the records as CSV to the clipboard (paste them to me)   F6 all powers on/off
// It only exists in the Editor and in Development Builds, like the DebugOverlay.
public class JumpProbe : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Tooltip("The player. Empty = the first CC_ELEMENTO found in the scene.")]
    public CC_ELEMENTO Player;

    [Tooltip("On = every power is unlocked at the same time (dash, double jump, glide, ground pound, wall jump, gravity flip).")]
    public bool AllPowers = true;

    [Tooltip("Horizontal speed at takeoff above this counts as RUNNING (walk is 11, run is 16).")]
    public float RunSpeedThreshold = 13.5f;

    [Tooltip("What counts as ground for the edge measurement.")]
    public LayerMask GroundMask = ~0;

    [Tooltip("Also write every trip to the Console.")]
    public bool LogToConsole = true;

    [Tooltip("Show the records on screen.")]
    public bool ShowOnScreen = true;

    // one best-of record per (walk or run, moves used, landing height)
    private class Record
    {
        public string Key;
        public int Count;
        public float BestEdge;      // -1 = never crossed a real gap
        public float BestCentre;
        public float BestApex;
        public float RiseAtBest;
    }

    private static readonly string[] PowerFlags =
    {
        "hasDash", "hasDoubleJump", "hasGlide", "hasGroundPound", "hasWallJump", "hasGravityFlip"
    };

    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private KinematicCharacterMotor _motor;
    private readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
    private readonly HashSet<string> _warned = new HashSet<string>();

    // trip state
    private bool _airborne;
    private Vector3 _lastGroundPos;
    private float _lastGroundSpeed;
    private Collider _takeoffCollider;
    private float _apexY;
    private float _airTime;
    private readonly List<string> _events = new List<string>();

    // move detection
    private CharacterState _prevState;
    private bool _prevGlide;
    private bool _prevFlip;
    private bool _prevClimb;
    private bool _prevDoubleJump;
    private float _prevWallLock;
    private Vector3 _dashStart;
    private float _dashTime;
    private Vector3 _glideStart;
    private float _glideTime;

    // best single numbers of each move
    private float _bestDashX, _bestDashY, _bestGlideTime, _bestGlideDx, _lastGlideFall, _bestPoundDrop;
    private float _poundStartY;

    private readonly Dictionary<string, Record> _records = new Dictionary<string, Record>();
    private readonly List<Record> _sorted = new List<Record>();
    private bool _dirty = true;

    private readonly RaycastHit[] _hits = new RaycastHit[8];
    private string _last = "no trip yet";
    private readonly StringBuilder _text = new StringBuilder(1024);
    private GUIStyle _style;

    private void Start()
    {
        if (Player == null) Player = FindFirstObjectByType<CC_ELEMENTO>();
        if (Player != null) _motor = Player.GetComponent<KinematicCharacterMotor>();

        if (_motor == null)
            Debug.LogWarning("JumpProbe: no player or no KinematicCharacterMotor found, so nothing is measured.", this);
    }

    private void Update()
    {
        if (_motor == null) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.f4Key.wasPressedThisFrame) ClearRecords();
            if (keyboard.f5Key.wasPressedThisFrame) CopyCsv();
            if (keyboard.f6Key.wasPressedThisFrame) AllPowers = !AllPowers;
        }

        if (AllPowers) UnlockEverything();

        DetectMoves();
        TrackTrip();
    }

    // ------------------------------------------------------------------
    // powers
    // ------------------------------------------------------------------
    private void UnlockEverything()
    {
        for (int i = 0; i < PowerFlags.Length; i++)
        {
            FieldInfo field = Field(PowerFlags[i]);
            if (field != null) field.SetValue(Player, true);
        }
    }

    private FieldInfo Field(string name)
    {
        if (_fields.TryGetValue(name, out FieldInfo cached)) return cached;

        FieldInfo field = typeof(CC_ELEMENTO).GetField(name, Flags);
        _fields[name] = field;

        if (field == null && _warned.Add(name))
            Debug.LogWarning("JumpProbe: CC_ELEMENTO has no field named '" + name + "' (renamed?). That move is not detected.", this);

        return field;
    }

    private bool GetBool(string name)
    {
        FieldInfo field = Field(name);
        return field != null && (bool)field.GetValue(Player);
    }

    private float GetFloat(string name)
    {
        FieldInfo field = Field(name);
        return field != null ? (float)field.GetValue(Player) : 0f;
    }

    // ------------------------------------------------------------------
    // move detection: each move adds one entry to the trip, and the single-move numbers are measured here
    // ------------------------------------------------------------------
    private void DetectMoves()
    {
        Vector3 position = _motor.TransientPosition;
        CharacterState state = Player.CurrentState;

        if (state != _prevState)
        {
            if (state == CharacterState.Dashing)
            {
                _dashStart = position;
                _dashTime = Time.time;
                AddEvent("DASH");
            }

            if (_prevState == CharacterState.Dashing)
            {
                float dx = Mathf.Abs(position.x - _dashStart.x);
                float dy = position.y - _dashStart.y;
                if (dx > _bestDashX) { _bestDashX = dx; _bestDashY = dy; }
                Note("DASH   " + dx.ToString("0.00") + " across, " + dy.ToString("+0.00;-0.00;0.00") + " up, " + (Time.time - _dashTime).ToString("0.00") + " s");
            }

            if (state == CharacterState.GroundPound)
            {
                _poundStartY = position.y;
                AddEvent("POUND");
            }

            if (_prevState == CharacterState.GroundPound)
            {
                float drop = _poundStartY - position.y;
                if (drop > _bestPoundDrop) _bestPoundDrop = drop;
            }

            if (state == CharacterState.Dead) AbortTrip();

            _prevState = state;
        }

        bool gliding = Player.IsGliding;
        if (gliding != _prevGlide)
        {
            if (gliding)
            {
                _glideStart = position;
                _glideTime = Time.time;
                AddEvent("GLIDE");
            }
            else
            {
                float time = Time.time - _glideTime;
                float dx = Mathf.Abs(position.x - _glideStart.x);
                float fall = time > 0.05f ? (_glideStart.y - position.y) / time : 0f;
                if (time > _bestGlideTime) { _bestGlideTime = time; _bestGlideDx = dx; }
                _lastGlideFall = fall;
                Note("GLIDE  " + time.ToString("0.00") + " s, " + dx.ToString("0.00") + " across, fall " + fall.ToString("0.00") + " u/s");
            }
            _prevGlide = gliding;
        }

        bool flipped = Player.IsGravityFlipped;
        if (flipped != _prevFlip)
        {
            AddEvent("FLIP");
            _prevFlip = flipped;
        }

        bool climbing = Player.IsClimbing;
        if (climbing && !_prevClimb) AddEvent("CLIMB");
        _prevClimb = climbing;

        bool doubleJump = GetBool("_doubleJumpConsumed");
        if (doubleJump && !_prevDoubleJump && _airborne) AddEvent("DJUMP");
        _prevDoubleJump = doubleJump;

        float wallLock = GetFloat("_wallJumpLockTimer");
        if (wallLock > 0f && _prevWallLock <= 0f) AddEvent("WALLJUMP");
        _prevWallLock = wallLock;
    }

    private void AddEvent(string name)
    {
        if (!_airborne) return;
        if (_events.Count > 0 && _events[_events.Count - 1] == name) return;
        _events.Add(name);
    }

    private void Note(string line)
    {
        _last = line;
        if (LogToConsole) Debug.Log("[JumpProbe] " + line);
    }

    // ------------------------------------------------------------------
    // trips
    // ------------------------------------------------------------------
    private void TrackTrip()
    {
        bool grounded = _motor.GroundingStatus.IsStableOnGround;
        Vector3 position = _motor.TransientPosition;

        if (grounded)
        {
            if (_airborne) Land(position);

            _airborne = false;
            _lastGroundPos = position;
            _lastGroundSpeed = Mathf.Abs(_motor.Velocity.x);
            _takeoffCollider = GroundBelow(position);
        }
        else
        {
            if (!_airborne)
            {
                _airborne = true;
                _airTime = 0f;
                _apexY = position.y;
                _events.Clear();
            }

            _airTime += Time.deltaTime;
            if (position.y > _apexY) _apexY = position.y;
        }
    }

    private void AbortTrip()
    {
        _airborne = false;
        _events.Clear();
    }

    private void Land(Vector3 position)
    {
        // ignore tiny bounces (landing bounce, step hops)
        if (_airTime < 0.15f) return;

        int mode = _lastGroundSpeed > RunSpeedThreshold ? 1 : 0;
        Collider landing = GroundBelow(position);

        float centre = Mathf.Abs(position.x - _lastGroundPos.x);
        float rise = position.y - _lastGroundPos.y;
        float apex = _apexY - _lastGroundPos.y;

        // the empty space between the two platforms, from the real colliders
        float edge = -1f;
        if (landing != null && _takeoffCollider != null && landing != _takeoffCollider)
        {
            bool right = position.x >= _lastGroundPos.x;
            float from = right ? _takeoffCollider.bounds.max.x : _takeoffCollider.bounds.min.x;
            float to = right ? landing.bounds.min.x : landing.bounds.max.x;
            float gap = right ? to - from : from - to;
            if (gap > 0.05f) edge = gap;
        }

        string moves = _events.Count == 0 ? "JUMP" : string.Join(" > ", _events);
        int bucket = Mathf.Clamp(Mathf.RoundToInt(rise), -6, 6);
        string key = (mode == 1 ? "RUN  " : "WALK ") + moves + "  @" + bucket.ToString("+0;-0;0");

        if (!_records.TryGetValue(key, out Record record))
        {
            record = new Record { Key = key, BestEdge = -1f };
            _records[key] = record;
        }

        record.Count++;
        if (edge > record.BestEdge) { record.BestEdge = edge; record.RiseAtBest = rise; }
        if (centre > record.BestCentre) record.BestCentre = centre;
        if (apex > record.BestApex) record.BestApex = apex;
        _dirty = true;

        Note(key + "   edge " + (edge > 0f ? edge.ToString("0.00") : "none")
            + "  centre " + centre.ToString("0.00") + "  apex " + apex.ToString("0.00")
            + "  rise " + rise.ToString("+0.0;-0.0;0.0") + "  air " + _airTime.ToString("0.00") + " s");

        _events.Clear();
    }

    // the collider under a point, ignoring the player and triggers
    private Collider GroundBelow(Vector3 position)
    {
        int count = Physics.RaycastNonAlloc(position + Vector3.up * 0.6f, Vector3.down, _hits, 4f, GroundMask, QueryTriggerInteraction.Ignore);

        Collider best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider hit = _hits[i].collider;
            if (hit == null || hit.transform.IsChildOf(Player.transform)) continue;

            if (_hits[i].distance < bestDistance)
            {
                bestDistance = _hits[i].distance;
                best = hit;
            }
        }

        return best;
    }

    // ------------------------------------------------------------------
    // records, screen, CSV
    // ------------------------------------------------------------------
    private void ClearRecords()
    {
        _records.Clear();
        _sorted.Clear();
        _bestDashX = _bestDashY = _bestGlideTime = _bestGlideDx = _lastGlideFall = _bestPoundDrop = 0f;
        _last = "records cleared";
        _dirty = true;
    }

    private void Sort()
    {
        if (!_dirty) return;
        _dirty = false;

        _sorted.Clear();
        _sorted.AddRange(_records.Values);
        _sorted.Sort((a, b) => b.BestEdge != a.BestEdge ? b.BestEdge.CompareTo(a.BestEdge) : b.BestCentre.CompareTo(a.BestCentre));
    }

    private void CopyCsv()
    {
        Sort();

        StringBuilder csv = new StringBuilder(2048);
        csv.AppendLine("key,count,best_edge_gap,best_centre,best_apex,rise_at_best");

        for (int i = 0; i < _sorted.Count; i++)
        {
            Record r = _sorted[i];
            csv.Append('"').Append(r.Key).Append("\",").Append(r.Count).Append(',')
                .Append(r.BestEdge.ToString("0.00")).Append(',')
                .Append(r.BestCentre.ToString("0.00")).Append(',')
                .Append(r.BestApex.ToString("0.00")).Append(',')
                .Append(r.RiseAtBest.ToString("0.00")).AppendLine();
        }

        csv.AppendLine();
        csv.AppendLine("best dash across," + _bestDashX.ToString("0.00") + ",up," + _bestDashY.ToString("0.00"));
        csv.AppendLine("best glide time," + _bestGlideTime.ToString("0.00") + ",across," + _bestGlideDx.ToString("0.00") + ",fall speed," + _lastGlideFall.ToString("0.00"));
        csv.AppendLine("best ground pound drop," + _bestPoundDrop.ToString("0.00"));
        csv.AppendLine("all powers," + AllPowers);

        GUIUtility.systemCopyBuffer = csv.ToString();
        Note("records copied to the clipboard (" + _sorted.Count + " lines): paste them into the chat");
    }

    private void OnGUI()
    {
        if (!ShowOnScreen) return;

        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label) { fontSize = 13, richText = false };
            _style.normal.textColor = Color.white;
        }

        Sort();

        _text.Length = 0;
        _text.Append("JumpProbe   F4 clear   F5 copy CSV   F6 all powers: ").Append(AllPowers ? "ON" : "off").Append('\n');
        _text.Append("last: ").Append(_last).Append('\n');
        _text.Append("dash ").Append(_bestDashX.ToString("0.00")).Append("   glide ").Append(_bestGlideTime.ToString("0.00"))
            .Append(" s / ").Append(_bestGlideDx.ToString("0.00")).Append(" across / fall ").Append(_lastGlideFall.ToString("0.00"))
            .Append("   pound drop ").Append(_bestPoundDrop.ToString("0.00")).Append('\n');

        int shown = Mathf.Min(_sorted.Count, 14);
        for (int i = 0; i < shown; i++)
        {
            Record r = _sorted[i];
            _text.Append(r.Key).Append("   edge ").Append(r.BestEdge > 0f ? r.BestEdge.ToString("0.00") : "-")
                .Append("  centre ").Append(r.BestCentre.ToString("0.00"))
                .Append("  apex ").Append(r.BestApex.ToString("0.00"))
                .Append("  x").Append(r.Count).Append('\n');
        }

        if (_sorted.Count > shown) _text.Append("... ").Append(_sorted.Count - shown).Append(" more (F5 copies all)\n");

        GUI.Label(new Rect(10f, 120f, 900f, 420f), _text.ToString(), _style);
    }
#endif
}
