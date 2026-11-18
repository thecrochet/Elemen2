using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// One-shot volume trigger that starts the GridRevealController wave when the player
/// (identified by CC_ELEMENTO component, no tags or layers) enters it.
/// Draws a wire-box gizmo so the designer can see the trigger volume in the Scene view.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GridRevealTrigger : MonoBehaviour, IResettable
{
    // ------------------------------------------------------------------
    // INSPECTOR
    // ------------------------------------------------------------------
    [Header("Target")]
    [Tooltip("GridRevealController that owns the wave. Assign in the Inspector.")]
    [SerializeField] private GridRevealController revealController;

    [Header("Wave Origin")]
    [Tooltip("Use the trigger's own position as the wave origin. " +
             "Disable to use the player's position instead.")]
    [SerializeField] private bool useTriggerPosition = true;

    [Header("Gizmo")]
    [Tooltip("Color of the Scene-view gizmo.")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 1f, 0.25f);

    // ------------------------------------------------------------------
    // RUNTIME STATE
    // ------------------------------------------------------------------
    private bool _fired;
    private bool _committedFired;

    // ------------------------------------------------------------------
    // UNITY LIFECYCLE
    // ------------------------------------------------------------------
    private bool _registered;

    private void Awake()
    {
        if (revealController == null)
            Debug.LogWarning("[GridRevealTrigger] revealController is not assigned on " + name, this);
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
            Debug.LogWarning("[GridRevealTrigger] RespawnManager not found — IResettable not registered.", this);
        }
    }

    private void OnDestroy()
    {
        if (_registered && RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_fired) return;
        if (revealController == null) return;

        // Identify player by component, not by tag or layer
        if (other.GetComponent<CC_ELEMENTO>() == null) return;

        _fired = true;

        Vector3 origin = useTriggerPosition
            ? transform.position
            : other.transform.position;

        revealController.StartReveal(origin);
    }

    // ------------------------------------------------------------------
    // IResettable
    // ------------------------------------------------------------------
    public void CommitState()
    {
        _committedFired = _fired;
    }

    public void ResetObject()
    {
        _fired = _committedFired;
    }

    // ------------------------------------------------------------------
    // GIZMOS
    // ------------------------------------------------------------------
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawTriggerGizmo(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawTriggerGizmo(true);
    }

    private void DrawTriggerGizmo(bool selected)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Color fill  = gizmoColor;
        Color wire  = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, selected ? 0.9f : 0.6f);

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        // Filled semi-transparent
        Gizmos.color = fill;
        if (col is BoxCollider box)
            Gizmos.DrawCube(box.center, box.size);
        else
            Gizmos.DrawSphere(Vector3.zero, 0.5f);

        // Wire outline
        Gizmos.color = wire;
        if (col is BoxCollider box2)
            Gizmos.DrawWireCube(box2.center, box2.size);
        else
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);

        Gizmos.matrix = Matrix4x4.identity;

        // Label
        Handles.Label(transform.position + Vector3.up * 0.6f,
            _fired ? "[GridReveal FIRED]" : "[GridReveal Trigger]");
    }
#endif

    // ------------------------------------------------------------------
    // CREATE-MENU HELPER
    // ------------------------------------------------------------------
#if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/Elemento/Level/Grid Reveal Trigger", false, 10)]
    private static void CreateFromMenu(UnityEditor.MenuCommand cmd)
    {
        GameObject go = new GameObject("GridRevealTrigger");
        GameObjectUtility.SetParentAndAlign(go, cmd.context as GameObject);
        BoxCollider bc = go.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(4f, 6f, 4f);
        go.AddComponent<GridRevealTrigger>();
        go.layer = LayerMask.NameToLayer("Interactable"); // layer 8
        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create GridRevealTrigger");
        UnityEditor.Selection.activeGameObject = go;
    }
#endif
}
