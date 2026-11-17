using UnityEngine;

// A saw that spins, and can also go back and forth along a path.
//
// It does NOT kill by itself: put the Hazard script on the same object (it kills on touch).
//
// SETUP:
//   - the saw object: a trigger collider (Is Trigger) + Hazard + this script
//     (a kinematic Rigidbody is added automatically: leave it, it lets the saw move safely)
//   - a CHILD object with the saw mesh, assigned to "Visual" (only the mesh spins)
//   - make the trigger a bit SMALLER than the picture (about 85%): deaths feel fair
//
// FAIRNESS: when the saw travels, it follows the LEVEL CLOCK like the moving platforms:
//   - it only moves while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
// So every retry shows exactly the same saw timing.
[RequireComponent(typeof(Rigidbody))]
public class RotatingSaw : MonoBehaviour, IResettable
{
    // ------------------------------------------------------------------
    // SPIN
    // ------------------------------------------------------------------
    // The mesh that spins (a child). Empty = this object spins (fine for a saw that does not travel)
    public Transform Visual;

    // The spin axis, in the Visual's own space. Forward (0,0,1) is a disc facing the side camera.
    public Vector3 SpinAxis = Vector3.forward;

    // Degrees per second (negative = the other way)
    public float SpinSpeed = 360f;

    // ------------------------------------------------------------------
    // TRAVEL (optional)
    // ------------------------------------------------------------------
    // Where the saw goes, relative to where you placed it (world units). (0,0,0) = stays in place.
    public Vector3 Travel = Vector3.zero;

    // Speed in units per second
    public float Speed = 4f;

    // Seconds spent waiting at each end
    public float PauseAtEnds = 0f;

    // Slow down gently at the ends (off = constant speed, easier to read)
    public bool SmoothEnds = false;

    // Shifts the saw in its cycle, in seconds (use it to make two saws go in turns)
    public float TimeOffset = 0f;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private Rigidbody _body;
    private Vector3 _startPosition;

    private float _time;            // the saw's own clock
    private float _committedTime;   // the "save": the clock at the last checkpoint

    private float _travelTime;      // seconds to go from one end to the other
    private float _period;          // seconds for a full round trip

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();

        // a kinematic body moves only when WE tell it to (no gravity, no pushing)
        _body.isKinematic = true;
        _body.useGravity = false;

        // smooth movement between physics steps
        _body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (Visual == null)
            Visual = transform;

        _startPosition = transform.position;

        float distance = Travel.magnitude;
        _travelTime = (Speed > 0.01f && distance > 0.001f) ? distance / Speed : 0f;
        _period = 2f * (_travelTime + PauseAtEnds);

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // The spin is only a picture, so it runs every frame (and stops with the pause)
    private void Update()
    {
        Visual.Rotate(SpinAxis, SpinSpeed * Time.deltaTime, Space.Self);
    }

    // The movement runs in the physics step (FixedUpdate), like every moving object that can hurt
    private void FixedUpdate()
    {
        // a saw without a path just spins
        if (_period <= 0.0001f) return;

        // only moves while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;
        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += Time.fixedDeltaTime;

        _body.MovePosition(GetPosition());
    }

    // Where the saw should be right now (same round trip as the MovingPlatform)
    private Vector3 GetPosition()
    {
        float t = Mathf.Repeat(_time + TimeOffset, _period);
        float progress;   // 0 = start, 1 = end

        if (t < _travelTime)
            progress = t / _travelTime;                                     // going to the end
        else if (t < _travelTime + PauseAtEnds)
            progress = 1f;                                                  // waiting at the end
        else if (t < 2f * _travelTime + PauseAtEnds)
            progress = 1f - (t - _travelTime - PauseAtEnds) / _travelTime;   // coming back
        else
            progress = 0f;                                                  // waiting at the start

        if (SmoothEnds)
            progress = Mathf.SmoothStep(0f, 1f, progress);

        return _startPosition + Travel * progress;
    }

    // SAVE: at the level start and at each checkpoint
    public void CommitState()
    {
        _committedTime = _time;
    }

    // LOAD: after a death, the saw goes back to its saved moment
    public void ResetObject()
    {
        _time = _committedTime;
    }

#if UNITY_EDITOR
    // Scene view (select the saw): the path (if it travels) and the size of the blade
    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? _startPosition : transform.position;

        float radius = 0.5f;
        Renderer picture = Visual != null ? Visual.GetComponentInChildren<Renderer>() : null;

        if (picture != null)
            radius = picture.bounds.extents.x;

        Gizmos.color = new Color(1f, 0.25f, 0.25f, 0.9f);
        Gizmos.DrawWireSphere(start, radius);

        if (Travel.sqrMagnitude > 0.0001f)
        {
            Gizmos.DrawLine(start, start + Travel);
            Gizmos.DrawWireSphere(start + Travel, radius);
        }

        UnityEditor.Handles.Label(start + Vector3.up * (radius + 0.4f),
            "spin " + SpinSpeed.ToString("0") + " deg/s" + (Travel.sqrMagnitude > 0.0001f ? ", travel " + Travel.magnitude.ToString("0.0") + " u at " + Speed.ToString("0.0") + " u/s" : ""));
    }
#endif
}