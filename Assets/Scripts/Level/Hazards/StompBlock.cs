using KinematicCharacterController;
using UnityEngine;

// A crusher (like a Mario Thwomp): it waits, shakes as a warning, slams down, rests, then rises.
// It repeats forever. Several of them in a row make a "wave" that a dash can outrun.
//
// It does NOT kill by itself: put the Hazard script on the same object, with a trigger collider.
//
// SETUP:
//   - the block object: a trigger collider (Is Trigger) + Hazard + this script + the PhysicsMover component (KCC asset)
//     (the Rigidbody stays; PhysicsMover makes it kinematic by itself)
//   - the ball can STAND on the top of the block: add a thin child object with a normal (NOT trigger) Box Collider
//     on the top face, without Hazard, outside the kill trigger. Because the block is a PhysicsMover, the ball is
//     carried up and down with it (a plain moving Rigidbody does not carry the ball)
//   - a CHILD object with the mesh, assigned to "Visual" (only the mesh shakes during the warning)
//   - make the trigger a bit SMALLER than the picture (about 90%): deaths feel fair
//   - an optional ParticleSystem as a child, placed at the bottom face of the block, for the impact
//
// FAIRNESS: it follows the LEVEL CLOCK like the moving platforms and saws:
//   - it only moves while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
// So every retry shows exactly the same timing. Don't place a checkpoint under a stomp block.
//
// THE WAVE: put blocks in a row and give each one a bigger Delay. The slams then travel along the row at
//   wave speed = distance between blocks / delay step
// The player must go faster than that speed to stay ahead of the slams:
//   under 11 = walking is enough, 11 to 16 = needs running or a dash, 16 to 28 = needs the dash
[RequireComponent(typeof(PhysicsMover))]
public class StompBlock : MonoBehaviour, IMoverController, IResettable
{
    // ------------------------------------------------------------------
    // MOVEMENT
    // ------------------------------------------------------------------
    // The direction of the slam, in world space (down by default)
    public Vector3 SlamDirection = Vector3.down;

    // How far the block travels from its waiting position to its lowest position
    public float Drop = 3f;

    // ------------------------------------------------------------------
    // TIMING (one cycle = all of these added up)
    // ------------------------------------------------------------------
    // Seconds waiting up: the safe moment to pass under
    public float WaitTime = 0.8f;

    // Seconds of warning: the mesh shakes, the block has not moved yet
    public float WarnTime = 0.3f;

    // Speed of the slam in units per second (30 = a 3 unit drop in about 0.1 s)
    public float SlamSpeed = 30f;

    // Seconds spent at the bottom
    public float RestTime = 0.5f;

    // Speed of the rise in units per second
    public float RiseSpeed = 6f;

    // Seconds before this block starts its first cycle. Increase it block by block to make a wave.
    public float Delay = 0f;

    // ------------------------------------------------------------------
    // LOOK
    // ------------------------------------------------------------------
    // The mesh that shakes during the warning (a child). Optional.
    public Transform Visual;
    public float ShakeAmount = 0.06f;

    // Optional: a ParticleSystem (Play On Awake OFF, Looping OFF) played when the block hits the bottom.
    // Make it a child of the block so it plays at the right place.
    public ParticleSystem ImpactEffect;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private enum Phase
    {
        Wait,
        Warn,
        Slam,
        Rest,
        Rise
    }

    private PhysicsMover _mover;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _direction;
    private Vector3 _visualStart;

    private float _time;            // the block's own clock
    private float _committedTime;   // the "save": the clock at the last checkpoint

    private float _slamTime;        // seconds the slam lasts
    private float _riseTime;        // seconds the rise lasts
    private float _period;          // seconds of a full cycle

    private bool _warning;          // true during the warning (the mesh shakes)
    private bool _shaking;          // true while the mesh is away from its place
    private bool _impactPlayed;     // so the impact effect plays once per slam
    private bool _warnPlayed;       // so the warning sound plays once per cycle

    private void Start()
    {
        // tell the mover that this script decides its movement (the KCC asset then also carries the ball)
        _mover = GetComponent<PhysicsMover>();
        _mover.MoverController = this;

        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _direction = SlamDirection.normalized;

        if (Visual != null)
            _visualStart = Visual.localPosition;

        // the minimum avoids a division by zero if a speed is set to 0
        _slamTime = Mathf.Max(Drop / Mathf.Max(SlamSpeed, 0.01f), 0.01f);
        _riseTime = Mathf.Max(Drop / Mathf.Max(RiseSpeed, 0.01f), 0.01f);
        _period = WaitTime + WarnTime + _slamTime + RestTime + _riseTime;

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // Called by the KCC asset on every simulation step: "where should the block be now?"
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalRotation = _startRotation;

        // only moves while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;
        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += deltaTime;

        Phase phase;
        float offset = GetOffset(out phase);

        goalPosition = _startPosition + _direction * offset;

        _warning = phase == Phase.Warn;

        // the warning sound plays once, when the warning starts
        if (_warning)
        {
            if (!_warnPlayed)
            {
                _warnPlayed = true;
                Sfx.PlayAt(SfxType.StompWarn, transform.position);
            }
        }
        else
        {
            _warnPlayed = false;
        }

        // the impact effect plays once, when the block arrives at the bottom
        if (phase == Phase.Rest)
        {
            if (!_impactPlayed)
            {
                _impactPlayed = true;

                Sfx.PlayAt(SfxType.StompImpact, transform.position);

                if (ImpactEffect != null)
                    ImpactEffect.Play();
            }
        }
        else
        {
            _impactPlayed = false;
        }
    }

    // The shake is only a picture, so it runs every frame
    private void Update()
    {
        if (Visual == null) return;

        if (_warning)
        {
            _shaking = true;
            Visual.localPosition = _visualStart + new Vector3(Random.Range(-1f, 1f), 0f, 0f) * ShakeAmount;
        }
        else if (_shaking)
        {
            _shaking = false;
            Visual.localPosition = _visualStart;
        }
    }

    // How far the block is from its waiting position right now, and in which phase of the cycle
    private float GetOffset(out Phase phase)
    {
        // before its Delay the block simply waits at the top
        float t = Mathf.Max(0f, _time - Delay);
        t = Mathf.Repeat(t, _period);

        if (t < WaitTime)
        {
            phase = Phase.Wait;
            return 0f;
        }
        t -= WaitTime;

        if (t < WarnTime)
        {
            phase = Phase.Warn;
            return 0f;
        }
        t -= WarnTime;

        if (t < _slamTime)
        {
            // p * p = it starts slowly and speeds up, like something heavy falling
            phase = Phase.Slam;
            float p = t / _slamTime;
            return Drop * p * p;
        }
        t -= _slamTime;

        if (t < RestTime)
        {
            phase = Phase.Rest;
            return Drop;
        }
        t -= RestTime;

        phase = Phase.Rise;
        return Drop * (1f - t / _riseTime);
    }

    // SAVE: at the level start and at each checkpoint
    public void CommitState()
    {
        _committedTime = _time;
    }

    // LOAD: after a death, the block goes back to its saved moment
    public void ResetObject()
    {
        _time = _committedTime;
    }
}
