using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

// The exit animation: when the level is completed, the ball glides to the middle of the portal
// and then DIVES INTO IT along the Z axis (away from the camera), shrinking, while the camera pushes in a little.
// Then the finish panel appears and the next level loads.
//
// SETUP
//   1) Put this script on the portal that has the LevelGoal (or on a child of it).
//   2) Drag it into the "Exit Sequence" slot of the LevelGoal.
//   3) The portal's blue arrow (local +Z, "Move tool" in Local mode) must point INTO the tunnel, away from the camera.
//      This is already the case for the Neon Portal Tunnel and for a Quad that faces the camera.
//   4) Optional: leave "Fade Group" empty and the ScreenFade of the Level UI is used automatically.
//
// It takes about 0.9 seconds before the panel, and it is only played when the level is completed
// (never on retries), so the instant retry of the game is untouched.
public class LevelExitSequence : MonoBehaviour
{
    [Header("Where")]
    // The middle of the portal ring. Empty = this object.
    public Transform PortalCenter;

    [Header("Timing (seconds)")]
    // The ball slides to the middle of the portal
    [Min(0.05f)] public float GlideTime = 0.25f;

    // The dive into the tunnel
    [Min(0.05f)] public float DiveTime = 0.6f;

    // How far into the tunnel the ball goes (world units). A bit less than the tunnel Depth.
    public float DiveDistance = 6f;

    // The ball ends at this fraction of its size (1 = no shrinking)
    [Range(0.1f, 1f)] public float EndScale = 0.35f;

    [Header("Camera")]
    // The camera moves this far toward the portal during the animation (0 = the camera stays put)
    public float CameraPush = 3f;

    [Header("After the dive")]
    // false = the finish panel appears when the ball has gone (recommended: the dive is not covered)
    // true  = the panel is shown at once, during the dive
    public bool ShowPanelDuringExit = false;

    // How long the panel is visible before the next level loads (includes the fade)
    [Min(0.2f)] public float PanelTime = 1.2f;

    // Optional: a CanvasGroup on a full screen black Image. It fades in at the end.
    public CanvasGroup FadeGroup;

    // How long the fade lasts (it ends when the next level loads)
    [Min(0.05f)] public float FadeTime = 0.3f;

    private bool _playing;

    // Called by LevelGoal
    public void Play(CC_ELEMENTO player)
    {
        // only once
        if (_playing) return;
        _playing = true;

        StartCoroutine(Run(player));
    }

    private IEnumerator Run(CC_ELEMENTO player)
    {
        LevelManager level = LevelManager.Instance;
        Transform center = PortalCenter != null ? PortalCenter : transform;

        // the direction INTO the tunnel
        Vector3 inward = center.forward;

        float animationTime = GlideTime + DiveTime;

        if (level != null)
        {
            // the HUD waits, and the next level waits for the animation + the panel
            level.HoldFinishPanel = !ShowPanelDuringExit;
            level.DelayNextLevel(animationTime + PanelTime);
        }

        // the ball is now moved by this script
        player.BeginScriptedExit();

        // the ball mesh is scaled (not the character itself, so the physics stay untouched)
        BallVisuals visuals = player.GetComponentInChildren<BallVisuals>();
        Transform ball = visuals != null ? visuals.BallMesh : null;
        Vector3 startScale = ball != null ? ball.localScale : Vector3.one;

        // the camera stops following the player and moves by script
        CinemachineCamera cinemachineCamera = player.PlayerCamera;
        Transform cameraTransform = null;
        Vector3 cameraStart = Vector3.zero;
        Vector3 cameraDirection = Vector3.forward;

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = null;
            cinemachineCamera.LookAt = null;

            cameraTransform = cinemachineCamera.transform;
            cameraStart = cameraTransform.position;
            cameraDirection = cameraTransform.forward;
        }

        // ---------------- 1) GLIDE to the middle of the portal ----------------
        Vector3 glideStart = player.BallCenterPosition;
        float time = 0f;

        while (time < GlideTime)
        {
            time += Time.deltaTime;
            float k = Mathf.Clamp01(time / GlideTime);

            // slows down at the end (ease out)
            float eased = 1f - (1f - k) * (1f - k);

            player.MoveBallCenterTo(Vector3.Lerp(glideStart, center.position, eased));
            MoveCamera(cameraTransform, cameraStart, cameraDirection, time / animationTime);
            yield return null;
        }

        // ---------------- 2) DIVE into the tunnel ----------------
        Vector3 diveEnd = center.position + inward * DiveDistance;
        time = 0f;

        while (time < DiveTime)
        {
            time += Time.deltaTime;
            float k = Mathf.Clamp01(time / DiveTime);

            // speeds up (ease in): the ball is sucked in
            float eased = k * k;

            player.MoveBallCenterTo(Vector3.Lerp(center.position, diveEnd, eased));

            if (ball != null)
                ball.localScale = Vector3.Lerp(startScale, startScale * EndScale, eased);

            MoveCamera(cameraTransform, cameraStart, cameraDirection, (GlideTime + time) / animationTime);
            yield return null;
        }

        // ---------------- 3) the panel, then the fade ----------------
        if (level != null)
            level.HoldFinishPanel = false;

        float wait = Mathf.Max(PanelTime - FadeTime, 0f);

        while (wait > 0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }

        // no Fade Group assigned: use the ScreenFade of the Level UI, if the level has one
        CanvasGroup fade = FadeGroup != null ? FadeGroup : (ScreenFade.Instance != null ? ScreenFade.Instance.Group : null);

        if (fade != null)
        {
            time = 0f;

            while (time < FadeTime)
            {
                time += Time.deltaTime;
                fade.alpha = Mathf.Clamp01(time / FadeTime);
                yield return null;
            }
        }
    }

    // The camera slowly moves toward the portal (progress 0 to 1 over the whole animation, ease in and out)
    private void MoveCamera(Transform cameraTransform, Vector3 start, Vector3 direction, float progress)
    {
        if (cameraTransform == null) return;

        float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));
        cameraTransform.position = start + direction * (CameraPush * eased);
    }
}
