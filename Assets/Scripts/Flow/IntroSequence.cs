using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// IntroSequence: the opening of the tutorial level. Plays once when the scene starts:
//   black, the Made with Unity logo, the logo fades into the cube (seen diagonally), the camera orbits to a flat
//   side view while the dark Aether ball fades in on top of the cube, then the gameplay camera takes over and
//   the player gets control.
//
// How it works, in plain words:
//   * The intro camera is an object with a CinemachineCamera on it (higher priority than the gameplay camera).
//     This script only moves its Transform and switches it off at the end. Cinemachine does the blend back.
//   * The ball fade uses a global shader value "_ElementoBallHide" (1 = invisible, 0 = normal). It only affects
//     the Aether ball shader (see the shader patch in the README). Unset = 0, so nothing else is touched.
//   * Player input is locked by disabling the player's input actions, and enabled again at the end.
//   * Any key or button skips to the end. It never replays after a respawn (nothing reloads the scene).
public class IntroSequence : MonoBehaviour
{
    [Header("Scene links (the builder fills these)")]
    [Tooltip("The player. Its input actions are locked during the intro.")]
    public CC_ELEMENTO Player;

    [Tooltip("An object with a CinemachineCamera on it, Priority 20. This script moves it, then switches it off.")]
    public GameObject IntroCamera;

    [Tooltip("What the intro camera looks at and orbits around: the top centre of the cube.")]
    public Transform Pivot;

    [Tooltip("Full-screen black image with a CanvasGroup. Starts at alpha 1.")]
    public CanvasGroup BlackOverlay;

    [Tooltip("The Made with Unity logo with a CanvasGroup. Starts at alpha 0.")]
    public CanvasGroup LogoGroup;

    [Header("Switches")]
    [Tooltip("Turn off to skip the intro while you build the rest of the level.")]
    public bool PlayIntro = true;

    [Tooltip("Any key, mouse button or gamepad button skips to the end.")]
    public bool AllowSkip = true;

    [Header("Timing (seconds)")]
    [Tooltip("Pure black at the start.")]
    public float BlackHold = 1f;
    [Tooltip("The logo fades in.")]
    public float LogoFadeIn = 0.8f;
    [Tooltip("The logo stays on screen.")]
    public float LogoHold = 1.4f;
    [Tooltip("The logo and the black fade out together, so the cube appears.")]
    public float CrossFade = 1.2f;
    [Tooltip("The cube sits there, seen diagonally, before the camera moves.")]
    public float CubeHold = 1f;
    [Tooltip("How long the camera orbit lasts.")]
    public float OrbitTime = 4f;
    [Tooltip("The ball starts to fade in this long after the orbit starts.")]
    public float BallFadeDelay = 2f;
    [Tooltip("How long the ball takes to fade in.")]
    public float BallFadeTime = 4f;
    [Tooltip("The Cinemachine blend back to the gameplay camera. Keep it equal to the Brain's Default Blend (2 s in the Reference). Control returns after it.")]
    public float BlendTime = 2f;

    [Header("Camera orbit")]
    [Tooltip("Distance from the pivot. The gameplay camera uses 32.")]
    public float Distance = 32f;
    [Tooltip("Yaw at the start: 45 shows the cube in the classic diagonal view.")]
    public float StartYaw = 45f;
    [Tooltip("Pitch at the start: 35.264 is the classic isometric angle.")]
    public float StartPitch = 35.264f;
    [Tooltip("Yaw at the end: 0 is the flat side view of the 2.5D game.")]
    public float EndYaw = 0f;
    [Tooltip("Pitch at the end: about 4, like the gameplay camera.")]
    public float EndPitch = 4f;

    private const float SkipGuard = 0.4f;   // seconds after the start before a key can skip

    private static bool s_Played;
    private static readonly int BallHideId = Shader.PropertyToID("_ElementoBallHide");

    private InputActionReference[] _actions;
    private bool[] _wasEnabled;
    private bool _locked;
    private bool _running;
    private float _age;

    // Static values survive "Enter Play Mode without domain reload", so reset them at every start.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        s_Played = false;
        Shader.SetGlobalFloat(BallHideId, 0f);
    }

    private bool ShouldPlay()
    {
        return PlayIntro && !s_Played && Player != null && IntroCamera != null && Pivot != null;
    }

    private void Awake()
    {
        if (PlayIntro && (Player == null || IntroCamera == null || Pivot == null))
        {
            Debug.LogWarning("IntroSequence: Player, Intro Camera or Pivot is not linked, so the intro is skipped.", this);
        }

        if (ShouldPlay())
        {
            if (BlackOverlay != null) BlackOverlay.alpha = 1f;
            if (LogoGroup != null) LogoGroup.alpha = 0f;
            SetBallHide(1f);
        }
        else
        {
            SetOverlays(0f);
            if (IntroCamera != null) IntroCamera.SetActive(false);
            SetBallHide(0f);
        }
    }

    private void Start()
    {
        if (!ShouldPlay()) return;

        s_Played = true;
        LockInput();
        PlaceCamera(StartYaw, StartPitch);
        IntroCamera.SetActive(true);
        _running = true;
        StartCoroutine(Run());
    }

    private void Update()
    {
        if (!_running) return;

        _age += Time.unscaledDeltaTime;
        if (AllowSkip && _age > SkipGuard && AnyPress()) Skip();
    }

    private IEnumerator Run()
    {
        yield return Wait(BlackHold);
        yield return FadeGroup(LogoGroup, 0f, 1f, LogoFadeIn);
        yield return Wait(LogoHold);

        // the logo and the black overlay fade out together: the cube is already there behind them
        float t = 0f;
        float cross = Mathf.Max(CrossFade, 0.01f);
        while (t < cross)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / cross));
            if (BlackOverlay != null) BlackOverlay.alpha = a;
            if (LogoGroup != null) LogoGroup.alpha = a;
            yield return null;
        }

        yield return Wait(CubeHold);

        // orbit to the side view while the ball fades in
        float total = Mathf.Max(OrbitTime, BallFadeDelay + BallFadeTime);
        t = 0f;
        while (t < total)
        {
            t += Time.deltaTime;
            float o = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / Mathf.Max(OrbitTime, 0.01f)));
            PlaceCamera(Mathf.Lerp(StartYaw, EndYaw, o), Mathf.Lerp(StartPitch, EndPitch, o));

            float b = Mathf.Clamp01((t - BallFadeDelay) / Mathf.Max(BallFadeTime, 0.01f));
            SetBallHide(1f - Mathf.SmoothStep(0f, 1f, b));
            yield return null;
        }

        // hand over: switching the intro camera off makes Cinemachine blend to the gameplay camera
        SetBallHide(0f);
        IntroCamera.SetActive(false);
        yield return Wait(BlendTime);

        Finish();
    }

    private void Skip()
    {
        StopAllCoroutines();
        Finish();
    }

    private void Finish()
    {
        _running = false;
        SetOverlays(0f);
        if (IntroCamera != null) IntroCamera.SetActive(false);
        SetBallHide(0f);
        UnlockInput();
    }

    private void PlaceCamera(float yaw, float pitch)
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 position = Pivot.position - rotation * Vector3.forward * Distance;
        IntroCamera.transform.SetPositionAndRotation(position, rotation);
    }

    private static IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    private static IEnumerator FadeGroup(CanvasGroup group, float from, float to, float seconds)
    {
        float t = 0f;
        float length = Mathf.Max(seconds, 0.01f);
        while (t < length)
        {
            t += Time.deltaTime;
            if (group != null) group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / length));
            yield return null;
        }
        if (group != null) group.alpha = to;
    }

    private void SetOverlays(float alpha)
    {
        if (BlackOverlay != null) BlackOverlay.alpha = alpha;
        if (LogoGroup != null) LogoGroup.alpha = alpha;
    }

    private static void SetBallHide(float value)
    {
        Shader.SetGlobalFloat(BallHideId, value);
    }

    // ---- input lock ----

    private void LockInput()
    {
        if (_locked || Player == null) return;

        _actions = new InputActionReference[]
        {
            Player.MoveAction, Player.JumpAction, Player.DashAction, Player.GroundPoundAction,
            Player.RunAction, Player.GravityFlipAction, Player.GlideAction
        };
        _wasEnabled = new bool[_actions.Length];

        for (int i = 0; i < _actions.Length; i++)
        {
            if (_actions[i] == null || _actions[i].action == null) continue;
            _wasEnabled[i] = _actions[i].action.enabled;
            if (_wasEnabled[i]) _actions[i].action.Disable();
        }
        _locked = true;
    }

    private void UnlockInput()
    {
        if (!_locked) return;

        for (int i = 0; i < _actions.Length; i++)
        {
            if (_actions[i] == null || _actions[i].action == null) continue;
            if (_wasEnabled[i]) _actions[i].action.Enable();
        }
        _locked = false;
    }

    // Read the devices directly: the player's actions are disabled during the intro.
    private static bool AnyPress()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.anyKey.wasPressedThisFrame) return true;

        Mouse mouse = Mouse.current;
        if (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)) return true;

        Gamepad pad = Gamepad.current;
        if (pad != null && (pad.buttonSouth.wasPressedThisFrame || pad.buttonEast.wasPressedThisFrame
            || pad.buttonWest.wasPressedThisFrame || pad.buttonNorth.wasPressedThisFrame
            || pad.startButton.wasPressedThisFrame)) return true;

        return false;
    }

    private void OnDestroy()
    {
        SetBallHide(0f);
        UnlockInput();
    }

    private void OnDrawGizmosSelected()
    {
        if (Pivot == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Pivot.position, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Pivot.position, Pivot.position - Quaternion.Euler(StartPitch, StartYaw, 0f) * Vector3.forward * Distance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(Pivot.position, Pivot.position - Quaternion.Euler(EndPitch, EndYaw, 0f) * Vector3.forward * Distance);
    }
}
