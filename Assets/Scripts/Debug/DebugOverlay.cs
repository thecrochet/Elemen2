using System.Text;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

// Small debug overlay for testing levels. Put it on any object in the scene (for example "Debug").
//
//   F3            show / hide the overlay
//   ]  and  [     warp to the next / previous checkpoint (sorted left to right)
//
// It only exists in the Editor and in Development Builds. In a final build the whole
// script compiles to nothing, so it can never cost performance or ship by mistake.
public class DebugOverlay : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public CC_ELEMENTO Player;
    public bool StartVisible = false;

    private KinematicCharacterMotor _motor;
    private Checkpoint[] _checkpoints;
    private int _warpIndex = -1;

    private bool _visible;
    private float _smoothDelta = 0.016f;    // smoothed frame time, so the fps number is readable
    private float _worstDelta;              // slowest recent frame (shows stutters)
    private float _worstTimer;

    private readonly StringBuilder _text = new StringBuilder(256);   // reused, so no new string every frame
    private GUIStyle _style;

    private void Start()
    {
        _visible = StartVisible;

        if (Player != null)
            _motor = Player.GetComponent<KinematicCharacterMotor>();

        // find every checkpoint once, then sort them left to right (a side scroller goes along X)
        _checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        System.Array.Sort(_checkpoints, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
    }

    private void Update()
    {
        // frame time: smoothed average + the worst frame of the last 2 seconds
        float dt = Time.unscaledDeltaTime;
        _smoothDelta += (dt - _smoothDelta) * 0.1f;

        _worstTimer -= dt;
        if (dt > _worstDelta || _worstTimer <= 0f)
        {
            _worstDelta = dt;
            _worstTimer = 2f;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.f3Key.wasPressedThisFrame)
            _visible = !_visible;

        if (keyboard.rightBracketKey.wasPressedThisFrame) Warp(+1);
        if (keyboard.leftBracketKey.wasPressedThisFrame) Warp(-1);
    }

    // Makes the next/previous checkpoint the active one, then respawns there.
    // It goes through the RespawnManager, so the clock, pickups and power follow the normal rules.
    private void Warp(int direction)
    {
        if (_checkpoints == null || _checkpoints.Length == 0) return;
        if (RespawnManager.Instance == null) return;

        _warpIndex = Mathf.Clamp(_warpIndex + direction, 0, _checkpoints.Length - 1);

        RespawnManager.Instance.SetCheckpoint(_checkpoints[_warpIndex]);
        RespawnManager.Instance.KillPlayer();
    }

    private void OnGUI()
    {
        if (!_visible || Player == null) return;

        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label) { fontSize = 16 };
            _style.normal.textColor = Color.white;
        }

        float speedX = 0f, speedY = 0f;
        if (_motor != null)
        {
            speedX = _motor.Velocity.x;
            speedY = _motor.Velocity.y;
        }

        _text.Clear();
        _text.Append("FPS ").Append(Mathf.RoundToInt(1f / _smoothDelta));
        _text.Append("   worst frame ").Append((_worstDelta * 1000f).ToString("0.0")).Append(" ms\n");
        _text.Append("State ").Append(Player.CurrentState.ToString());
        _text.Append("   Power ").Append(Player.CurrentPower.ToString()).Append('\n');
        _text.Append("Speed X ").Append(speedX.ToString("0.0"));
        _text.Append("   Y ").Append(speedY.ToString("0.0")).Append('\n');

        if (_motor != null)
            _text.Append("Grounded ").Append(_motor.GroundingStatus.IsStableOnGround).Append('\n');

        if (RespawnManager.Instance != null)
            _text.Append("Attempt ").Append(RespawnManager.Instance.Attempts).Append('\n');

        if (LevelManager.Instance != null)
            _text.Append("Level time ").Append(LevelManager.Instance.LevelTime.ToString("0.00")).Append('\n');

        _text.Append("[F3] hide   [ ] warp checkpoint");

        GUI.Label(new Rect(10, 10, 600, 200), _text.ToString(), _style);
    }
#endif
}