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
            // where the hazard touched the ball: the point of THIS hazard's collider closest to the ball.
            // On a row of 6 spikes this is the spike that was touched, not the middle of the row.
            Collider own = GetComponent<Collider>();
            Vector3 ballCenter = other.bounds.center;
            HitEffect.transform.position = own != null ? own.ClosestPoint(ballCenter) : ballCenter;
            HitEffect.Play(true);
        }

        Sfx.PlayAt(SfxType.HazardHit, transform.position);

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.KillPlayer();
    }
}