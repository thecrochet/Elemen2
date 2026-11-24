using UnityEngine;

// A bar (a "baton") that turns around a pole, like in some Super Meat Boy levels.
// Unlike RotatingSaw, the DANGEROUS PART turns too (not only the picture).
//
// SETUP (like the Super Meat Boy saw on an arm):
//   - an empty parent holds the pivot CUBE (it does not turn)
//   - next to the cube, an empty object "Rotor" sits exactly on the pivot. The Rotor holds this script
//     and a kinematic Rigidbody (added automatically)
//   - under the Rotor: the ARM (a long thin cube, no collider, harmless) and the BLADE at its end
//   - the BLADE has a Sphere Collider (Is Trigger) + the Hazard script, and a child picture (the saw
//     disc) assigned to BladeVisual, so the blade also spins on itself
//   - the cube must NOT be a child of the Rotor, or it would turn too
//
// ARM LENGTH: change "Arm Length" in the Inspector. The script moves the Blade to that distance from
// the pole and stretches the Arm to match. Which side the arm points to (left or right) is read from
// where the Blade currently sits, so rotate the whole SpinArm root if you want another direction.
//
// FAIRNESS: it follows the LEVEL CLOCK like the moving platforms:
//   - it only turns while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
//
// SPEED LIMIT: tip speed (units/s) = Arm Length x Spin Speed x 0.01745. Keep it under about 25,
// or the blade could jump over the player between two physics steps.
[RequireComponent(typeof(Rigidbody))]
public class SpinArm : MonoBehaviour, IResettable
{
    // The most the tip may travel per second before the Editor warns you
    private const float MaxTipSpeed = 25f;

    [Header("Arm")]
    // Distance from the pole to the blade, in world units. Longer arm = bigger circle = faster tip.
    [Tooltip("Distance from the pole to the blade (units). Tip speed = Arm Length x Spin Speed x 0.01745")]
    [Min(0.5f)] public float ArmLength = 6f;

    // The long thin cube. Optional: it is stretched to match Arm Length. (In your prefab it is "Cube".)
    [Tooltip("The long thin cube (harmless, no collider). Stretched to Arm Length.")]
    public Transform Arm;

    // The object with the trigger collider + Hazard. Found by the name "Blade" if left empty.
    [Tooltip("The Blade object (Sphere Collider trigger + Hazard). Placed at Arm Length.")]
    public Transform Blade;

    [Header("Spin")]
    // The turning axis, in world space. Forward (0,0,1) turns in the screen plane of the side camera.
    public Vector3 SpinAxis = Vector3.forward;

    // Degrees per second (negative = the other way). 90 = a full turn every 4 seconds.
    public float SpinSpeed = 90f;

    // Shifts the bar in its turn, in seconds. Use it to make two bars out of step.
    public float TimeOffset = 0f;

    [Header("Blade picture")]
    // Optional: the picture of the saw blade at the end of the arm. It also spins on itself (only a picture).
    public Transform BladeVisual;
    public float BladeSpinSpeed = 360f;

    // How fast the tip travels right now (read only, for the Inspector and other scripts)
    public float TipSpeed => Mathf.Abs(ArmLength * SpinSpeed * Mathf.Deg2Rad);

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

        ApplyArmLength();
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

    // ------------------------------------------------------------------
    // ARM LENGTH
    // ------------------------------------------------------------------
    // Puts the Blade at ArmLength from the pole and stretches the Arm to reach it.
    // It only changes the X position (the arm lies along the Rotor's X axis).
    private void ApplyArmLength()
    {
        if (Blade == null)
            Blade = transform.Find("Blade");

        if (Blade == null) return;

        // which side is the arm on? Read it from the blade (right by default)
        float side = Blade.localPosition.x < -0.001f ? -1f : 1f;

        Vector3 bladePos = Blade.localPosition;
        bladePos.x = side * ArmLength;
        Blade.localPosition = bladePos;

        if (Arm != null)
        {
            // the arm is a unit cube: its scale X is its length, its center is halfway
            Vector3 armPos = Arm.localPosition;
            armPos.x = side * ArmLength * 0.5f;
            Arm.localPosition = armPos;

            Vector3 armScale = Arm.localScale;
            armScale.x = ArmLength;
            Arm.localScale = armScale;
        }
    }

#if UNITY_EDITOR
    // Runs in the Editor when you change a value: the arm resizes live, and you get a speed warning
    private void OnValidate()
    {
        ArmLength = Mathf.Max(0.5f, ArmLength);
        ApplyArmLength();

        if (TipSpeed > MaxTipSpeed)
        {
            float maxSpin = MaxTipSpeed / (ArmLength * Mathf.Deg2Rad);
            Debug.LogWarning(name + ": tip speed is " + TipSpeed.ToString("0.0") +
                             " units/s (limit about " + MaxTipSpeed + "). With this arm, keep Spin Speed under " +
                             maxSpin.ToString("0") + ".", this);
        }
    }

    // A circle showing how far the blade reaches (the danger area)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, ArmLength);
    }
#endif

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