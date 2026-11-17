using UnityEngine;

// A bar (a "baton") that turns around a pole, like in some Super Meat Boy levels.
// Unlike RotatingSaw, the DANGEROUS PART turns too (not only the picture).
//
// SETUP (like the Super Meat Boy saw on an arm):
//   - an empty parent holds the pivot CUBE (it does not turn)
//   - next to the cube, an empty object "Rotor" sits exactly on the pivot. The Rotor holds this script,
//     the Hazard script, and a kinematic Rigidbody (added automatically)
//   - under the Rotor: the ARM (a long thin mesh, no collider, harmless) and the BLADE at its end
//   - the BLADE has a Sphere Collider (Is Trigger) + the Hazard script, and a child picture (the saw
//     disc) assigned to BladeVisual, so the blade also spins on itself
//   - the trigger should be a bit SMALLER than the blade picture (about 85%)
//   - the cube must NOT be a child of the Rotor, or it would turn too
//
// FAIRNESS: it follows the LEVEL CLOCK like the moving platforms:
//   - it only turns while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
//
// SPEED LIMIT: keep the tip speed (distance from the pole to the tip, in units, x turns per second x 6.3)
// under about 25 units/s. A faster tip could jump over the player between two physics steps.
//   Example: a 5 unit bar at 90 degrees per second has a tip speed of 5 x 1.57 = 7.9 units/s (calm).
[RequireComponent(typeof(Rigidbody))]
public class SpinArm : MonoBehaviour, IResettable
{
    // The turning axis, in world space. Forward (0,0,1) turns in the screen plane of the side camera.
    public Vector3 SpinAxis = Vector3.forward;

    // Degrees per second (negative = the other way). 90 = a full turn every 4 seconds.
    public float SpinSpeed = 90f;

    // Shifts the bar in its turn, in seconds. Use it to make two bars out of step.
    public float TimeOffset = 0f;

    // Optional: the picture of the saw blade at the end of the arm. It also spins on itself (only a picture).
    public Transform BladeVisual;
    public float BladeSpinSpeed = 360f;

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

    // The blade's own spin is only a picture, so it runs every frame (and stops with the pause)
    private void Update()
    {
        if (BladeVisual != null)
            BladeVisual.Rotate(Vector3.forward, BladeSpinSpeed * Time.deltaTime, Space.Self);
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

#if UNITY_EDITOR
    // Fairness limit for the tip speed, in units per second. Placeholder: confirm the number you want.
    private const float TipSpeedWarn = 25f;

    // Scene view (select the arm): the circle the blade sweeps and its tip speed (red above the limit)
    private void OnDrawGizmosSelected()
    {
        Transform blade = BladeVisual != null && BladeVisual.parent != null ? BladeVisual.parent : BladeVisual;

        if (blade == null) return;

        Vector3 center = transform.position;
        float radius = Vector2.Distance(center, blade.position);
        float tipSpeed = radius * SpinSpeed * Mathf.Deg2Rad;
        Vector3 axis = transform.TransformDirection(SpinAxis).normalized;

        Color color = tipSpeed > TipSpeedWarn ? new Color(1f, 0.15f, 0.15f, 1f) : new Color(0.3f, 1f, 0.4f, 1f);

        Color oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawWireDisc(center, axis, radius);
        UnityEditor.Handles.Label(center + Vector3.up * (radius + 0.5f),
            "tip speed " + tipSpeed.ToString("0.0") + " u/s" + (tipSpeed > TipSpeedWarn ? "  TOO FAST (limit " + TipSpeedWarn + ")" : "")
            + "\nmax spin for this arm: " + (TipSpeedWarn / Mathf.Max(radius, 0.01f) * Mathf.Rad2Deg).ToString("0") + " deg/s");
        UnityEditor.Handles.color = oldColor;
    }
#endif
}