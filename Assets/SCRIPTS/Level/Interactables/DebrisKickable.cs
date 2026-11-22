using System.Collections;
using UnityEngine;
using KinematicCharacterController;

/// <summary>
/// Physical debris square spawned when breakable blocks explode.
/// Features:
///   1. Bounces and slides across the floor realistically via bouncy, low-friction physics.
///   2. Solid physics collision 100% disabled against the Player (via Physics.IgnoreCollision and layer exclusion)
///      so the player ball's velocity and dash momentum are never blocked or slowed down.
///   3. Non-blocking Trigger sensor that detects the player ball touching the piece and applies a subtle,
///      natural outward horizontal push + tiny upward hop + Z-axis spin torque (scaled lightly by player speed).
///   4. Smoothly shrinks to scale 0 before destroying after 2.5s lifetime.
///   5. Registers with RespawnManager to clean up deterministically on player death.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class DebrisKickable : MonoBehaviour, IResettable
{
    [Header("Lifecycle & Shrink")]
    [Tooltip("Total lifetime in seconds before debris shrinks and is destroyed (3.5s).")]
    [SerializeField] private float lifetime = 3.5f;

    [Tooltip("Duration of smooth shrink-out to scale 0 before destruction (~0.5s).")]
    [SerializeField] private float shrinkDuration = 0.5f;

    [Header("Player Nudge Interaction")]
    [Tooltip("Horizontal push force applied to this debris square when touched by the player.")]
    [SerializeField] private float nudgeForce = 0.28f;

    [Tooltip("Upward hop force applied to this debris square when touched by the player.")]
    [SerializeField] private float upwardHopForce = 0.14f;

    [Tooltip("Spin torque applied around the Z axis when nudged.")]
    [SerializeField] private float spinTorque = 0.03f;

    [Tooltip("Radius of the trigger sensor detecting the player ball (tightly sized to square visual footprint).")]
    [SerializeField] private float triggerRadius = 0.65f;

    [Tooltip("Minimum time in seconds between consecutive nudges on this piece to prevent force accumulation.")]
    [SerializeField] private float nudgeCooldown = 0.08f;

    [Tooltip("Influence factor scaling nudge force with player movement/dash velocity.")]
    [SerializeField] private float speedInfluence = 1.0f;

    [Header("Collision & Constraints")]
    [Tooltip("Constrain Rigidbody to the 2D gameplay plane (Z = 0) and constrain rotation to Z axis (FreezeRotationX | FreezeRotationY).")]
    [SerializeField] private bool lockToZPlane = true;

    [Tooltip("Automatically ignore solid PhysX collision with the Player so the player passes straight through with zero push.")]
    [SerializeField] private bool ignorePlayerSolidCollision = true;

    [Header("Trigger References")]
    [Tooltip("The trigger collider detecting player proximity without blocking or affecting the ball.")]
    [SerializeField] private SphereCollider triggerCollider;

    private Rigidbody _rb;
    private Collider _solidCollider;
    private Vector3 _initialScale;
    private Coroutine _lifetimeCoroutine;
    private bool _isDestroyed;
    private float _lastNudgeTime = -999f;
    private CC_ELEMENTO _cachedPlayer;
    private KinematicCharacterMotor _cachedPlayerMotor;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _initialScale = transform.localScale;

        if (lockToZPlane && _rb != null)
        {
            _rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }

        if (_rb != null)
        {
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        // Cache solid collider for floor collision
        _solidCollider = GetComponent<BoxCollider>();
        if (_solidCollider == null)
            _solidCollider = GetComponent<Collider>();

        // Ensure trigger collider is configured and lives on an interactable layer
        EnsureTriggerCollider();

        CachePlayerReferences();
        IgnorePlayerCollision();
    }

    private void Start()
    {
        IgnorePlayerCollision();

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);

        _lifetimeCoroutine = StartCoroutine(LifetimeRoutine());
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    private void OnValidate()
    {
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
            triggerCollider.radius = triggerRadius;
        }
    }

    private void EnsureTriggerCollider()
    {
        if (triggerCollider == null)
        {
            // Search in children or self
            SphereCollider[] sphereCols = GetComponentsInChildren<SphereCollider>(true);
            for (int i = 0; i < sphereCols.Length; i++)
            {
                if (sphereCols[i] != null && sphereCols[i].isTrigger)
                {
                    triggerCollider = sphereCols[i];
                    break;
                }
            }

            if (triggerCollider == null && sphereCols.Length > 0)
            {
                triggerCollider = sphereCols[0];
            }
        }

        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer == -1) interactableLayer = 0;

        // If still null, create child GameObject for the trigger sensor on the Interactable layer
        if (triggerCollider == null)
        {
            Transform existingChild = transform.Find("NudgeTrigger");
            GameObject triggerGo;
            if (existingChild != null)
            {
                triggerGo = existingChild.gameObject;
            }
            else
            {
                triggerGo = new GameObject("NudgeTrigger");
                triggerGo.transform.SetParent(transform, false);
                triggerGo.transform.localPosition = Vector3.zero;
                triggerGo.transform.localRotation = Quaternion.identity;
                triggerGo.transform.localScale = Vector3.one;
            }

            triggerGo.layer = interactableLayer;
            triggerCollider = triggerGo.GetComponent<SphereCollider>();
            if (triggerCollider == null)
                triggerCollider = triggerGo.AddComponent<SphereCollider>();
        }

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
            triggerCollider.radius = triggerRadius;

            // If the trigger collider is on a layer that ignores player collision in the matrix (such as Debris),
            // move the trigger to the Interactable layer so PhysX trigger events fire reliably
            int playerLayer = _cachedPlayer != null ? _cachedPlayer.gameObject.layer : 6;
            if (Physics.GetIgnoreLayerCollision(playerLayer, triggerCollider.gameObject.layer))
            {
                if (triggerCollider.gameObject == gameObject)
                {
                    // Move to dedicated child GameObject so root solid BoxCollider stays on Debris layer
                    GameObject childGo = new GameObject("NudgeTrigger");
                    childGo.transform.SetParent(transform, false);
                    childGo.transform.localPosition = Vector3.zero;
                    childGo.transform.localRotation = Quaternion.identity;
                    childGo.transform.localScale = Vector3.one;
                    childGo.layer = interactableLayer;

                    SphereCollider newCol = childGo.AddComponent<SphereCollider>();
                    newCol.isTrigger = true;
                    newCol.radius = triggerRadius;

                    Destroy(triggerCollider);
                    triggerCollider = newCol;
                }
                else
                {
                    triggerCollider.gameObject.layer = interactableLayer;
                }
            }
        }
    }

    private void CachePlayerReferences()
    {
        if (_cachedPlayer == null)
            _cachedPlayer = FindFirstObjectByType<CC_ELEMENTO>();

        if (_cachedPlayer != null && _cachedPlayerMotor == null)
            _cachedPlayerMotor = _cachedPlayer.GetComponent<KinematicCharacterMotor>();
    }

    public void IgnorePlayerCollision()
    {
        if (!ignorePlayerSolidCollision) return;

        CachePlayerReferences();

        if (_cachedPlayer != null)
        {
            Collider[] playerColliders = _cachedPlayer.GetComponentsInChildren<Collider>(true);
            Collider[] debrisColliders = GetComponentsInChildren<Collider>(true);

            for (int i = 0; i < debrisColliders.Length; i++)
            {
                if (debrisColliders[i] == null) continue;
                // CRUCIAL: Only ignore solid non-trigger colliders!
                // Trigger colliders must NOT be ignored so OnTriggerEnter / OnTriggerStay fire!
                if (debrisColliders[i].isTrigger) continue;

                for (int j = 0; j < playerColliders.Length; j++)
                {
                    if (playerColliders[j] == null) continue;
                    Physics.IgnoreCollision(debrisColliders[i], playerColliders[j], true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryNudge(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryNudge(other);
    }

    private void TryNudge(Collider other)
    {
        if (_isDestroyed || _rb == null) return;
        if (Time.time < _lastNudgeTime + nudgeCooldown) return;

        CachePlayerReferences();
        if (_cachedPlayer == null) return;

        // Check if other is player
        if (other.transform.root != _cachedPlayer.transform.root && !other.TryGetComponent(out CC_ELEMENTO _))
            return;

        _lastNudgeTime = Time.time;

        // Calculate horizontal direction away from the player
        Vector3 playerPos = _cachedPlayer.transform.position;
        Vector3 debrisPos = transform.position;
        float dx = debrisPos.x - playerPos.x;

        float horizontalDir = Mathf.Abs(dx) > 0.02f ? Mathf.Sign(dx) : (Random.value > 0.5f ? 1f : -1f);

        // Factor in player velocity from motor if moving
        Vector3 playerVel = _cachedPlayerMotor != null ? _cachedPlayerMotor.Velocity : Vector3.zero;
        float playerSpeedX = Mathf.Abs(playerVel.x);

        if (playerSpeedX > 0.2f)
        {
            float moveDir = Mathf.Sign(playerVel.x);
            float blend = dx * 0.35f + moveDir * 0.65f;
            if (Mathf.Abs(blend) > 0.01f)
                horizontalDir = Mathf.Sign(blend);
        }

        // Scale impulse gently with player speed
        float speedScale = 1f;
        if (playerSpeedX > 1f)
        {
            speedScale += Mathf.Clamp((playerSpeedX - 1f) / 12f, 0f, 1.2f) * speedInfluence;
        }

        // Damp existing horizontal velocity so repeated touches don't launch debris wildly
        Vector3 currentVel = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(currentVel.x * 0.2f, Mathf.Max(0f, currentVel.y * 0.2f), 0f);

        Vector3 impulse = new Vector3(horizontalDir * nudgeForce * speedScale, upwardHopForce, 0f);
        _rb.AddForce(impulse, ForceMode.Impulse);

        // Spin torque around Z axis away from push direction
        float torqueZ = -horizontalDir * spinTorque * Random.Range(0.85f, 1.15f);
        _rb.AddTorque(new Vector3(0f, 0f, torqueZ), ForceMode.Impulse);
    }

    private IEnumerator LifetimeRoutine()
    {
        float elapsed = 0f;
        float shrinkStart = Mathf.Max(0.1f, lifetime - shrinkDuration);

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= shrinkStart)
            {
                float t = Mathf.Clamp01((elapsed - shrinkStart) / Mathf.Max(0.001f, shrinkDuration));
                float scaleFactor = Mathf.SmoothStep(1f, 0f, t);
                transform.localScale = _initialScale * scaleFactor;
            }

            yield return null;
        }

        _isDestroyed = true;
        Destroy(gameObject);
    }

    public void CommitState()
    {
        // Debris is dynamic and transient
    }

    public void ResetObject()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        if (_lifetimeCoroutine != null)
            StopCoroutine(_lifetimeCoroutine);

        Destroy(gameObject);
    }
}
