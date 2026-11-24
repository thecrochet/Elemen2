using UnityEngine;
using UnityEngine.Serialization;

// Makes the sphere LOOK like it rolls, shows the BASE SKIN, and MORPHS into an element's skin while a move lasts.
//
// The physics are still done by the Kinematic Character Motor (a capsule that slides).
// This script only rotates and colors the VISUAL sphere, so it never touches the collision.
//
// The two looks:
//   BASE SKIN  = the ball's material (gold Base Spark, dark Aether, or the element of the last NEW gate you crossed).
//   MORPH SHELL = a second sphere, a little bigger, child of the ball mesh. During a move (dash, ground pound,
//                 glide, double jump, wall jump, gravity flip) it grows from the core in the element's material,
//                 stays while the move lasts, then shrinks back. If the move's element is the base skin, nothing happens.
//
// Put it on the same object as CC_ELEMENTO.
public class BallVisuals : MonoBehaviour
{
    // ------------------------------------------------------------------
    // REFERENCES
    // ------------------------------------------------------------------
    public CC_ELEMENTO Player;

    // The sphere mesh. It must be a CHILD of the character and have NO collider on it.
    public Transform BallMesh;
    public Renderer BallRenderer;

    // ------------------------------------------------------------------
    // ROLLING SETTINGS
    // ------------------------------------------------------------------
    // Radius of the sphere mesh (a Unity sphere with scale 1 has a radius of 0.5)
    public float BallRadius = 0.5f;

    // 1 = the texture exactly follows the ground. Higher = spins faster (cartoon feel)
    public float RollSpeedMultiplier = 1f;

    // ------------------------------------------------------------------
    // MATERIALS (one per power)
    // ------------------------------------------------------------------
    public Material NoPowerMaterial;
    // (FormerlySerializedAs keeps the materials you already assigned in the Inspector)
    [FormerlySerializedAs("DashMaterial")] public Material FireMaterial;
    [FormerlySerializedAs("DoubleJumpMaterial")] public Material AirMaterial;
    [FormerlySerializedAs("GroundPoundMaterial")] public Material EarthMaterial;
    [FormerlySerializedAs("WallJumpMaterial")] public Material WaterMaterial;
    public Material AetherMaterial;

    // ------------------------------------------------------------------
    // MORPH (the element skin during a move)
    // ------------------------------------------------------------------
    [Header("Morph shell")]
    // A sphere child of the Ball Mesh (no collider). Leave empty = no morph (the ball only changes with the gate).
    public Renderer MorphShell;

    // Size of the shell compared to the ball (1.03 = 3 percent bigger, so it never flickers with the ball)
    public float ShellScale = 1.03f;

    // Seconds to grow in, and to shrink out (the spec: about 0.1 in, 0.25 to 0.3 out)
    public float MorphInTime = 0.10f;
    public float MorphOutTime = 0.28f;

    [Header("Switch flash (a small pop of the ball)")]
    // 0.15 = the ball swells 15 percent for a split second when the skin switches. 0 = no pop.
    public float PopAmount = 0.15f;
    public float PopTime = 0.12f;

    // ------------------------------------------------------------------
    // CHARGE PULSE: the ball pulses while it holds extra dashes (from a Dash Charge zone)
    // ------------------------------------------------------------------
    // How much the ball grows and shrinks (0.12 = 12 percent). 0 = no pulse.
    public float PulseAmount = 0.12f;

    // Pulses per second (about 2 = calm, 4 = urgent)
    public float PulseSpeed = 3f;

    // ------------------------------------------------------------------
    // OPTIONAL: a parachute object (child of the character) shown while gliding
    // ------------------------------------------------------------------
    public GameObject ParachuteVisual;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private Vector3 _lastPosition;
    private Quaternion _rollRotation;
    private PowerUpType _shownPower;
    private bool _wasDead;
    private Vector3 _baseScale;

    // morph
    private Transform _shellTransform;
    private PowerUpType _shellElement = PowerUpType.None;   // None = no shell material chosen yet
    private float _shellAmount;                             // 0 = hidden, 1 = fully shown
    private PowerUpType _pulseElement = PowerUpType.None;   // a short move (double jump, wall jump, flip)
    private float _pulseTimer;
    private float _popTimer;

    private void OnEnable()
    {
        if (Player != null)
            Player.MoveStarted += OnMoveStarted;
    }

    private void OnDisable()
    {
        if (Player != null)
            Player.MoveStarted -= OnMoveStarted;
    }

    private void Start()
    {
        _lastPosition = transform.position;
        _rollRotation = BallMesh.rotation;
        _baseScale = BallMesh.localScale;

        if (MorphShell != null)
        {
            _shellTransform = MorphShell.transform;
            MorphShell.enabled = false;
        }

        _shownPower = Player.CurrentPower;
        ApplyMaterial(_shownPower);
    }

    // a short move wants the morph for a moment
    private void OnMoveStarted(PowerUpType element, float seconds)
    {
        _pulseElement = element;
        _pulseTimer = seconds;
    }

    // LateUpdate runs after the motor moved the character, so we use the final position
    private void LateUpdate()
    {
        // dead = the ball is hidden (the respawn is instant, so there is nothing to wait for)
        if (Player.CurrentState == CharacterState.Dead)
        {
            if (BallRenderer.enabled)
                BallRenderer.enabled = false;

            HideShellNow();

            if (ParachuteVisual != null && ParachuteVisual.activeSelf)
                ParachuteVisual.SetActive(false);

            _wasDead = true;
            return;
        }

        // we just respawned: show the ball again, and the teleport must not count as rolling
        if (_wasDead)
        {
            _wasDead = false;
            BallRenderer.enabled = true;
            _lastPosition = transform.position;

            // the checkpoint skin comes back (no pop: it is not a "switch")
            _pulseTimer = 0f;
            _popTimer = 0f;
            _shownPower = Player.CurrentPower;
            ApplyMaterial(_shownPower);
            return;
        }

        UpdateRolling();
        UpdateMaterial();
        UpdateMorph();
        UpdateParachute();
        UpdateScale();
    }

    // ------------------------------------------------------------------
    // SCALE: charge heartbeat + the small pop of a switch (only the visual sphere, never the collision)
    // ------------------------------------------------------------------
    private void UpdateScale()
    {
        float factor = 1f;

        // a soft heartbeat while the ball holds extra dashes
        if (PulseAmount > 0f && Player.DashCharges > 0)
        {
            // 0..1 with a quick rise and a slower fall, like a heartbeat
            float wave = Mathf.Abs(Mathf.Sin(Time.time * PulseSpeed * Mathf.PI));
            factor += PulseAmount * wave;
        }

        // the switch flash: starts big, shrinks back fast
        if (_popTimer > 0f)
        {
            _popTimer -= Time.deltaTime;
            float k = Mathf.Clamp01(_popTimer / Mathf.Max(PopTime, 0.01f));
            factor += PopAmount * k * k;
        }

        Vector3 scale = _baseScale * factor;
        if (BallMesh.localScale != scale)
            BallMesh.localScale = scale;
    }

    private void StartPop()
    {
        if (PopAmount > 0f)
            _popTimer = PopTime;
    }

    // show the parachute only while the character glides
    private void UpdateParachute()
    {
        if (ParachuteVisual == null) return;

        bool show = Player.IsGliding;

        if (ParachuteVisual.activeSelf != show)
            ParachuteVisual.SetActive(show);
    }

    // ------------------------------------------------------------------
    // ROLLING
    // ------------------------------------------------------------------
    private void UpdateRolling()
    {
        // how far did we move since last frame?
        Vector3 moved = transform.position - _lastPosition;
        _lastPosition = transform.position;

        // a teleport (respawn) is not a roll
        if (moved.magnitude > 5f) return;

        // only the movement along the ground makes the ball roll (jumping up/down doesn't)
        // the character's own up (it is upside down while the gravity is flipped, so the ball rolls the right way)
        Vector3 up = transform.up;
        Vector3 flatMove = Vector3.ProjectOnPlane(moved, up);
        float distance = flatMove.magnitude;

        if (distance > 0.0001f)
        {
            // A ball turns by (distance / radius) radians. That is exactly why it looks like it
            // rolls without slipping.
            float angle = (distance / BallRadius) * Mathf.Rad2Deg * RollSpeedMultiplier;

            // The ball turns around the axis that is perpendicular to the move direction and up.
            // Moving right (+X) gives the axis -Z, so the top of the ball goes forward.
            Vector3 rollAxis = Vector3.Cross(up, flatMove.normalized);

            // add this small turn on top of the rotation we already have (world space)
            _rollRotation = Quaternion.AngleAxis(angle, rollAxis) * _rollRotation;
        }

        // We set the WORLD rotation so the character's own turning (UpdateRotation) doesn't spin the ball
        BallMesh.rotation = _rollRotation;
    }

    // ------------------------------------------------------------------
    // BASE SKIN (changes only when a NEW gate is crossed, or at a respawn)
    // ------------------------------------------------------------------
    private void UpdateMaterial()
    {
        // only change the material when the power changed
        if (Player.CurrentPower == _shownPower) return;

        _shownPower = Player.CurrentPower;
        ApplyMaterial(_shownPower);
        StartPop();
    }

    private void ApplyMaterial(PowerUpType power)
    {
        Material material = GetMaterial(power);

        if (material != null)
            BallRenderer.material = material;
    }

    private Material GetMaterial(PowerUpType power)
    {
        switch (power)
        {
            case PowerUpType.Fire: return FireMaterial;
            case PowerUpType.Air: return AirMaterial;
            case PowerUpType.Earth: return EarthMaterial;
            case PowerUpType.Water: return WaterMaterial;
            case PowerUpType.Aether: return AetherMaterial;
            default: return NoPowerMaterial;
        }
    }

    // ------------------------------------------------------------------
    // MORPH
    // ------------------------------------------------------------------
    // Which element is the ball using RIGHT NOW? None = just the base skin.
    private PowerUpType CurrentMoveElement()
    {
        // the long moves are read from the character, so the skin lasts exactly as long as the move
        if (Player.CurrentState == CharacterState.Dashing) return PowerUpType.Fire;
        if (Player.CurrentState == CharacterState.GroundPound) return PowerUpType.Earth;
        if (Player.IsGliding) return PowerUpType.Air;

        // the short moves last a fixed time
        if (_pulseTimer > 0f)
        {
            _pulseTimer -= Time.deltaTime;
            return _pulseElement;
        }

        return PowerUpType.None;
    }

    private void UpdateMorph()
    {
        if (_shellTransform == null) return;

        PowerUpType wanted = CurrentMoveElement();

        // the ball already wears this element as its base skin: nothing to show
        if (wanted == Player.CurrentPower)
            wanted = PowerUpType.None;

        if (wanted != PowerUpType.None)
        {
            // a new element takes the shell (if another one was showing, it is swapped on the spot)
            if (wanted != _shellElement)
            {
                Material material = GetMaterial(wanted);
                if (material == null) return;

                bool wasHidden = _shellAmount <= 0f;

                MorphShell.sharedMaterial = material;
                _shellElement = wanted;

                // the flash cue only when the shell appears (not when it just changes color)
                if (wasHidden)
                    StartPop();
            }

            _shellAmount = Mathf.MoveTowards(_shellAmount, 1f, Time.deltaTime / Mathf.Max(MorphInTime, 0.01f));
        }
        else
        {
            _shellAmount = Mathf.MoveTowards(_shellAmount, 0f, Time.deltaTime / Mathf.Max(MorphOutTime, 0.01f));
        }

        if (_shellAmount <= 0f)
        {
            if (MorphShell.enabled)
                MorphShell.enabled = false;
            _shellElement = PowerUpType.None;
            return;
        }

        if (!MorphShell.enabled)
            MorphShell.enabled = true;

        // it grows from the core (smooth start and end)
        float eased = _shellAmount * _shellAmount * (3f - 2f * _shellAmount);
        _shellTransform.localScale = Vector3.one * (ShellScale * eased);
    }

    private void HideShellNow()
    {
        _shellAmount = 0f;
        _shellElement = PowerUpType.None;
        _pulseTimer = 0f;

        if (MorphShell != null && MorphShell.enabled)
            MorphShell.enabled = false;
    }
}
