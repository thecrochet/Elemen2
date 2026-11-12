using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// A short tutorial text in the world ("Press Shift to dash"). It fades in when the ball comes near and out when it leaves.
//
// SETUP (the menu GameObject > Elemento > Flow > Tutorial Sign does all of this):
//   root object = this script + a Box Collider (Is Trigger) that covers the zone where the text should show
//   child "Text" = a TextMeshPro (3D, not UI) component, placed where you want to read it
//
// It only changes the alpha of the text, so nothing is created or destroyed while playing.
// Needs TextMeshPro (it comes with Unity 6: the first time, Unity asks to import the TMP Essentials).
[DisallowMultipleComponent]
public class TutorialSign : MonoBehaviour
{
    // The text to fade (found by itself in the children if empty)
    public TMP_Text Label;

    // Write the message here, or keep the text you typed in the TextMeshPro. Empty = leave the label as it is.
    [TextArea] public string Message;

    [Header("Fade")]
    public float FadeInTime = 0.25f;
    public float FadeOutTime = 0.4f;

    // Show it only until the player has seen it once in this attempt of the game (no repeat after a death)
    public bool OnlyFirstTime = false;

    private float _alpha;
    private bool _inside;
    private bool _seen;
    private Color _baseColor;

    private void Awake()
    {
        if (Label == null)
            Label = GetComponentInChildren<TMP_Text>();

        if (Label == null)
        {
            enabled = false;
            return;
        }

        if (!string.IsNullOrEmpty(Message))
            Label.text = Message;

        _baseColor = Label.color;
        _baseColor.a = 1f;
        Apply(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO _)) return;
        if (OnlyFirstTime && _seen) return;

        _inside = true;
        _seen = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO _)) return;

        _inside = false;
    }

    private void Update()
    {
        float target = _inside ? 1f : 0f;
        if (Mathf.Approximately(_alpha, target)) return;

        float time = _inside ? FadeInTime : FadeOutTime;
        float step = time > 0.0001f ? Time.deltaTime / time : 1f;

        _alpha = Mathf.MoveTowards(_alpha, target, step);
        Apply(_alpha);
    }

    private void Apply(float alpha)
    {
        Color color = _baseColor;
        color.a = alpha;
        Label.color = color;
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Elemento/Flow/Tutorial Sign", false, 17)]
    private static void CreateTutorialSign(MenuCommand command)
    {
        GameObject go = new GameObject("TutorialSign");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Tutorial Sign");

        BoxCollider zone = go.AddComponent<BoxCollider>();
        zone.isTrigger = true;
        zone.size = new Vector3(8f, 4f, 3f);

        GameObject text = new GameObject("Text");
        text.transform.SetParent(go.transform, false);
        text.transform.localPosition = new Vector3(0f, 2.5f, 0f);

        TextMeshPro label = text.AddComponent<TextMeshPro>();
        label.text = "Press ... to ...";
        label.fontSize = 6f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.7f, 1f, 1f, 1f);
        label.rectTransform.sizeDelta = new Vector2(10f, 2f);

        TutorialSign sign = go.AddComponent<TutorialSign>();
        sign.Label = label;

        Selection.activeGameObject = go;
    }
#endif
}
