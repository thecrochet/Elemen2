using UnityEngine;

// A full screen black fade. Put it on the "ScreenFade" object of the Level UI (the builder does this).
//
//  - At the start of the level the screen is black and fades IN (0.3 s), so a level never "pops" in.
//  - LevelExitSequence uses this same fade for the exit, when its own Fade Group slot is empty.
//
// The fade only happens when the scene loads, never on a retry (retries are instant, no scene load).
// It uses unscaled time, so it also works if the game is paused.
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance { get; private set; }

    // How long the fade in at the start of the level lasts. 0 = no fade in.
    [Min(0f)] public float FadeInTime = 0.3f;

    public CanvasGroup Group { get; private set; }

    private float _time;

    private void Awake()
    {
        Instance = this;
        Group = GetComponent<CanvasGroup>();
        Group.blocksRaycasts = false;
        Group.interactable = false;

        // start black, unless there is no fade in
        Group.alpha = FadeInTime > 0f ? 1f : 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (_time >= FadeInTime) return;

        _time += Time.unscaledDeltaTime;
        Group.alpha = 1f - Mathf.Clamp01(_time / FadeInTime);
    }
}
