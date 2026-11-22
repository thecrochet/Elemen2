using UnityEngine;

// Put this on the finish of a level. The object needs a Collider with "Is Trigger" checked.
//
// When the player touches it, the LevelManager freezes the clock, the character stops,
// and the next level loads after a short delay.
public class LevelGoal : MonoBehaviour
{
    // Optional: a ParticleSystem (Play On Awake OFF) that plays when the level is completed
    public ParticleSystem FinishEffect;

    // Optional: the ball dives into the portal when the level is completed (LevelExitSequence on this object or a child)
    public LevelExitSequence ExitSequence;

    [Header("Next Level Linking")]
    [Tooltip("Explicit next scene name (e.g. Level_02). If empty, auto-resolves Level_(XX+1)")]
    public string NextSceneName = "";

    private void OnTriggerEnter(Collider other)
    {
        // only the player can finish the level (TryGetComponent doesn't create garbage)
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;
        if (LevelManager.Instance == null) return;

        if (!string.IsNullOrEmpty(NextSceneName))
        {
            LevelManager.Instance.NextSceneName = NextSceneName;
        }
        else if (ExitSequence != null && !string.IsNullOrEmpty(ExitSequence.NextSceneName))
        {
            LevelManager.Instance.NextSceneName = ExitSequence.NextSceneName;
        }

        // CompleteLevel is true only the first time, so the effect plays once
        if (!LevelManager.Instance.CompleteLevel()) return;

        if (FinishEffect != null)
            FinishEffect.Play();

        if (ExitSequence != null)
            ExitSequence.Play(player);
    }
}
