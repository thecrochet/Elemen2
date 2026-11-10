using UnityEngine;

// A bar (a "baton") that turns around a pole, like in some Super Meat Boy levels.
// Unlike RotatingSaw, the DANGEROUS PART turns too (not only the picture).
//
// SETUP:
//   - the root object sits exactly on the POLE (the point the bar turns around)
//     it holds this script, the Hazard script, and a kinematic Rigidbody (added automatically)
//   - the bar is one or more CHILD objects, each with a mesh and a trigger collider (Is Trigger),
//     placed away from the pole. A bar through the pole = one long child centered on the pole.
//     Saws at the ends of the bar = a child with a Sphere Collider (trigger) at each end.
//   - the trigger colliders should be a bit SMALLER than the pictures (about 90%)
//   - if the bar does not kill, also put the Hazard script on the child that has the trigger
//
// FAIRNESS: it follows the LEVEL CLOCK like the moving platforms:
//   - it only turns while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
//
// SPEED LIMIT: keep the tip speed (distance from the pole to the tip, in units, x turns per second x 6.3)
// under about 25 units/s. A faster tip could jump over the player between two physics steps.
//   Example: a 5 unit bar at 90 degrees per second has a tip speed of 5 x 1.57 = 7.9 units/s (calm).
[RequireComponent(typeof(Rigidbody))]
public class SpinningBar1 : MonoBehaviour, IResettable
{
    // The turning axis, in world space. Forward (0,0,1) turns in the screen plane of the side camera.
    public Vector3 SpinAxis = Vector3.forward;

    // Degrees per second (negative = the other way). 90 = a full turn every 4 seconds.
    public float SpinSpeed = 90f;

    // Shifts the bar in its turn, in seconds. Use it to make two bars out of step.
    public float TimeOffset = 0f;

    private Rigidbody _body;
    private Quaternion _startRotation;
    private Vector3 _axis;

    private float _time;            // the bar's own clock
    private float _committedTime;   // the "save": the clock at the last checkpoint

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
        // the rotation you gave the bar in the editor is its position at time 0
        _startRotation = transform.rotation;
        _axis = SpinAxis.normalized;

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // The turning runs in the physics step (FixedUpdate), like every moving object that can hurt
    private void FixedUpdate()
    {
        // only turns while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;
        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += Time.fixedDeltaTime;

        float angle = SpinSpeed * (_time + TimeOffset);

        _body.MoveRotation(Quaternion.AngleAxis(angle, _axis) * _startRotation);
    }

    // SAVE: at the level start and at each checkpoint
    public void CommitState()
    {
        _committedTime = _time;
    }

    // LOAD: after a death, the bar goes back to its saved moment
    public void ResetObject()
    {
        _time = _committedTime;
    }
}