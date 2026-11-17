using KinematicCharacterController;
using UnityEngine;

// A platform that goes back and forth. The character rides on it (the KCC asset carries him).
//
// SETUP: put the PhysicsMover component (from the KCC asset) AND this script on the platform.
// The platform needs a normal (non-trigger) Collider. Do NOT put a Rigidbody yourself:
// PhysicsMover adds it.
//
// FAIRNESS: the platform follows the LEVEL CLOCK, like every other saved object:
//   - it only moves while the level clock runs (so it waits for your first input after a respawn)
//   - after a death it goes back to where it was when you reached the last checkpoint
// So every retry shows exactly the same platform timing (pillar 2: instant, consistent retries).
//
// Don't put a checkpoint or a spawn point on a moving platform.
[RequireComponent(typeof(PhysicsMover))]
public class MovingPlatform : MonoBehaviour, IMoverController, IResettable
{
    // Where the platform goes, relative to where you placed it (in world units)
    public Vector3 Travel = new Vector3(6f, 0f, 0f);

    // Speed in units per second
    public float Speed = 4f;

    // Seconds spent waiting at each end
    public float PauseAtEnds = 0.3f;

    // Slow down gently at the ends (off = constant speed, easier to read for precise jumps)
    public bool SmoothEnds = false;

    // Shifts the platform in its cycle (use it to make two platforms go in turns), in seconds
    public float TimeOffset = 0f;

    private PhysicsMover _mover;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private float _time;            // the platform's own clock
    private float _committedTime;   // the "save": the clock at the last checkpoint

    private float _travelTime;      // seconds to go from one end to the other
    private float _period;          // seconds for a full round trip

    private void Start()
    {
        _mover = GetComponent<PhysicsMover>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        float distance = Travel.magnitude;
        _travelTime = (Speed > 0.01f && distance > 0.001f) ? distance / Speed : 0f;
        _period = 2f * (_travelTime + PauseAtEnds);

        // tell the mover that this script decides its movement
        _mover.MoverController = this;

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // Called by the KCC asset on every simulation step: "where should the platform be now?"
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalRotation = _startRotation;

        // the platform only moves while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;
        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += deltaTime;

        if (_period <= 0.0001f)
        {
            goalPosition = _startPosition;
            return;
        }

        // where are we in the round trip?
        float t = Mathf.Repeat(_time + TimeOffset, _period);
        float progress;   // 0 = start, 1 = end

        if (t < _travelTime)
            progress = t / _travelTime;                                   // going to the end
        else if (t < _travelTime + PauseAtEnds)
            progress = 1f;                                                // waiting at the end
        else if (t < 2f * _travelTime + PauseAtEnds)
            progress = 1f - (t - _travelTime - PauseAtEnds) / _travelTime; // coming back
        else
            progress = 0f;                                                // waiting at the start

        if (SmoothEnds)
            progress = Mathf.SmoothStep(0f, 1f, progress);

        goalPosition = _startPosition + Travel * progress;
    }

    // SAVE: at the level start and at each checkpoint
    public void CommitState()
    {
        _committedTime = _time;
    }

    // LOAD: after a death, the platform goes back to its saved moment
    public void ResetObject()
    {
        _time = _committedTime;
    }

#if UNITY_EDITOR
    // Scene view (select the platform): the path, both end points, the end footprint and the trip time
    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? _startPosition : transform.position;
        Vector3 end = start + Travel;

        Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.9f);
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.25f);
        Gizmos.DrawWireSphere(end, 0.25f);

        // where the platform ends up (a box the size of its collider)
        Collider body = GetComponentInChildren<Collider>();

        if (body != null)
            Gizmos.DrawWireCube(body.bounds.center + (start - transform.position) + Travel, body.bounds.size);

        float trip = Speed > 0.01f ? Travel.magnitude / Speed : 0f;
        UnityEditor.Handles.Label(end + Vector3.up * 0.6f,
            "travel " + Travel.magnitude.ToString("0.0") + " u, " + trip.ToString("0.0") + " s one way");
    }
#endif
}