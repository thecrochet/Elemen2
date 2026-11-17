using UnityEngine;

// A control hint in the world ("SPACE  Jump"). It fades in when the player enters the trigger box
// and fades out when the player leaves. Use it only when a NEW input appears in the level.
//
// The builder (GameObject > Elemento > UI > Control Prompt) makes the whole object:
// a Box Collider (Is Trigger) + a small world space canvas with a text. Change the text in the
// "Label" slot's TextMeshPro, and resize the Box Collider to decide where the hint shows.
[RequireComponent(typeof(Collider))]
public class ControlPrompt : MonoBehaviour
{
    // The CanvasGroup on the world space canvas (alpha 0 at the start)
    public CanvasGroup Group;

    // Seconds to fade in or out
    [Min(0.01f)] public float FadeTime = 0.2f;

    // true = once the player has seen it and leaves, it never shows again (good for the first tutorial hints)
    public bool ShowOnce = false;

    private float _target;
    private bool _used;

    private void Start()
    {
        if (Group != null) Group.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.TryGetComponent(out CC_ELEMENTO _)) return;

        _target = 1f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO _)) return;

        _target = 0f;
        if (ShowOnce) _used = true;
    }

    private void Update()
    {
        if (Group == null || Mathf.Approximately(Group.alpha, _target)) return;

        Group.alpha = Mathf.MoveTowards(Group.alpha, _target, Time.deltaTime / FadeTime);
    }
}
