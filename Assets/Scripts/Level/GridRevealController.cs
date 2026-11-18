using UnityEngine;

/// <summary>
/// Broadcasts an expanding radial reveal wave to all materials that use the
/// _GridRevealEnabled / _GridRevealOrigin / _GridRevealRadius global shader properties.
/// Place on any scene GameObject (e.g. the LevelCore). Wire GridRevealTrigger to call StartReveal().
/// Implements IResettable so a checkpoint correctly restores revealed state on respawn.
/// </summary>
public class GridRevealController : MonoBehaviour, IResettable
{
    // ------------------------------------------------------------------
    // INSPECTOR
    // ------------------------------------------------------------------
    [Header("Reveal Wave")]
    [Tooltip("Units per second the reveal radius grows.")]
    [SerializeField] private float expandSpeed = 28f;

    [Tooltip("Maximum radius at which the reveal is considered complete.")]
    [SerializeField] private float maxRadius = 160f;

    [Tooltip("Soft-edge feather width passed to the shader (shader must read _GridRevealSoftEdge).")]
    [SerializeField] private float softEdgeWidth = 4f;

    // ------------------------------------------------------------------
    // CACHED SHADER PROPERTY IDS  (set once in Awake, never per-frame)
    // ------------------------------------------------------------------
    private static readonly int PropEnabled   = Shader.PropertyToID("_GridRevealEnabled");
    private static readonly int PropOrigin    = Shader.PropertyToID("_GridRevealOrigin");
    private static readonly int PropRadius    = Shader.PropertyToID("_GridRevealRadius");
    private static readonly int PropSoftEdge  = Shader.PropertyToID("_GridRevealSoftEdge");

    // ------------------------------------------------------------------
    // RUNTIME STATE
    // ------------------------------------------------------------------
    private bool  _revealing;
    private float _currentRadius;
    private Vector3 _origin;

    // IResettable checkpoint state
    private bool  _committedRevealing;
    private float _committedRadius;
    private Vector3 _committedOrigin;

    // ------------------------------------------------------------------
    // UNITY LIFECYCLE
    // ------------------------------------------------------------------
    private bool _registered;

    private void Awake()
    {
        // Start dark: reveal not active, radius 0
        _revealing     = false;
        _currentRadius = 0f;
        _origin        = Vector3.zero;

        Shader.SetGlobalFloat(PropEnabled, 1f);   // feature on so the shader can read radius
        Shader.SetGlobalFloat(PropRadius, 0f);
        Shader.SetGlobalFloat(PropSoftEdge, softEdgeWidth);
    }

    private void Start()
    {
        // Register in Start so RespawnManager.Instance is guaranteed to be set
        // (all Awakes run before any Start).
        if (!_registered && RespawnManager.Instance != null)
        {
            RespawnManager.Instance.Register(this); // also calls CommitState() immediately
            _registered = true;
        }
        else if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[GridRevealController] RespawnManager not found — IResettable not registered.", this);
        }
    }

    private void OnDestroy()
    {
        // Unregister and clear globals so the Scene view is never left dark after Play mode
        if (_registered && RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);

        Shader.SetGlobalFloat(PropEnabled, 0f);
        Shader.SetGlobalFloat(PropRadius,  0f);
    }

    private void Update()
    {
        if (!_revealing) return;

        _currentRadius += expandSpeed * Time.deltaTime;

        if (_currentRadius >= maxRadius)
        {
            _currentRadius = maxRadius;
            _revealing = false;
        }

        Shader.SetGlobalVector(PropOrigin, _origin);
        Shader.SetGlobalFloat(PropRadius,  _currentRadius);
    }

    // ------------------------------------------------------------------
    // PUBLIC API
    // ------------------------------------------------------------------
    /// <summary>
    /// Begin the expanding wave from the given world-space position.
    /// Safe to call multiple times; subsequent calls move the origin and restart
    /// the expansion from the current radius (never shrinks).
    /// </summary>
    public void StartReveal(Vector3 worldOrigin)
    {
        _origin    = worldOrigin;
        _revealing = true;

        // Push origin immediately so the first frame is correct
        Shader.SetGlobalVector(PropOrigin, _origin);
    }

    // ------------------------------------------------------------------
    // IResettable
    // ------------------------------------------------------------------
    public void CommitState()
    {
        _committedRevealing = _revealing;
        _committedRadius    = _currentRadius;
        _committedOrigin    = _origin;
    }

    public void ResetObject()
    {
        _revealing     = _committedRevealing;
        _currentRadius = _committedRadius;
        _origin        = _committedOrigin;

        // Push restored state to shaders immediately
        Shader.SetGlobalFloat(PropEnabled,  1f);
        Shader.SetGlobalVector(PropOrigin,  _origin);
        Shader.SetGlobalFloat(PropRadius,   _currentRadius);
        Shader.SetGlobalFloat(PropSoftEdge, softEdgeWidth);
    }
}
