using UnityEngine;

// Put this on spikes, saws, lava...: anything that kills the player on touch.
// The object needs a Collider with "Is Trigger" checked.
public class Hazard : MonoBehaviour
{
    // Optional: a ParticleSystem that is a CHILD of this object (Play On Awake OFF, Looping OFF).
    // It plays at the spot where the ball touched the hazard. The ball's own Death effect plays too.
    public ParticleSystem HitEffect;

    private void OnTriggerEnter(Collider other)
    {
        // only the player can die (TryGetComponent doesn't create garbage)
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        if (HitEffect != null)
        {
            // the closest point of the hazard to the ball = where the spike touched it
            HitEffect.transform.position = other.ClosestPoint(transform.position);
            HitEffect.Play(true);
        }

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.KillPlayer();
    }
}