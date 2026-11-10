using UnityEngine;

// Put this on the finish of a level. The object needs a Collider with "Is Trigger" checked.
//
// When the player touches it, the LevelManager freezes the clock, the character stops,
// and the next level loads after a short delay.
public class LevelGoal : MonoBehaviour
{
    // Optional: a ParticleSystem (Play On Awake OFF) that plays when the level is completed
    public ParticleSystem FinishEffect;

    private void OnTriggerEnter(Collider other)
    {
        // only the player can finish the level (TryGetComponent doesn't create garbage)
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;
        if (LevelManager.Instance == null) return;

        // CompleteLevel is true only the first time, so the effect plays once
        if (LevelManager.Instance.CompleteLevel() && FinishEffect != null)
            FinishEffect.Play();
    }
}