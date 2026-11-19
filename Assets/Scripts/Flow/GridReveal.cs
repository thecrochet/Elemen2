using System.Collections;
using UnityEngine;

// GridReveal: a trigger that fades the neon grid in. The grid is hidden when the scene starts; when the ball
// enters this trigger the grid fades in once, and stays visible (respawns do not hide it again).
//
// How it works: it sets the global shader value "_ElementoGridHide" (1 = grid invisible, 0 = normal), which the
// patched NeonGrid shader reads (see the README). Unset = 0, so every other level is untouched.
[RequireComponent(typeof(BoxCollider))]
public class GridReveal : MonoBehaviour
{
    [Header("Reveal")]
    [Tooltip("Seconds the grid takes to fade in once the ball enters the trigger. Keep the trigger about 26 units before the first thing the player must see (2.4 s of walking).")]
    [Range(0.5f, 8f)] public float FadeTime = 2.5f;

    [Tooltip("Hide the grid when the scene starts. Turn this off while you are building to see the level in Play mode.")]
    public bool HideGridAtStart = true;

    private static bool s_Revealed;
    private static readonly int HideId = Shader.PropertyToID("_ElementoGridHide");

    private bool _running;

    // Static values survive "Enter Play Mode without domain reload", so reset them at every start.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        s_Revealed = false;
        Shader.SetGlobalFloat(HideId, 0f);
    }

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        Shader.SetGlobalFloat(HideId, (HideGridAtStart && !s_Revealed) ? 1f : 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (s_Revealed || _running) return;
        if (!other.TryGetComponent(out CC_ELEMENTO _)) return;

        StartCoroutine(Reveal());
    }

    private IEnumerator Reveal()
    {
        _running = true;

        float t = 0f;
        float length = Mathf.Max(FadeTime, 0.01f);
        while (t < length)
        {
            t += Time.deltaTime;
            Shader.SetGlobalFloat(HideId, 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / length)));
            yield return null;
        }

        Shader.SetGlobalFloat(HideId, 0f);
        s_Revealed = true;
        _running = false;
    }

    private void OnDestroy()
    {
        Shader.SetGlobalFloat(HideId, 0f);
    }

    private void OnDrawGizmos()
    {
        if (!TryGetComponent(out BoxCollider box)) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
