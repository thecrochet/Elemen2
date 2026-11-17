using UnityEngine;

// Put this on a checkpoint object with a Collider (Is Trigger checked).
//
// When the player touches it:
//   - he comes back HERE the next time he dies
//   - everything he did so far is kept (collected pickups, broken objects, current power)
//   - everything he does AFTER it comes back after a death
public class Checkpoint : MonoBehaviour
{
    // Where the player comes back. If empty, the checkpoint's own position is used.
    // Tip: use a child object placed just above the floor.
    public Transform RespawnPoint;

    // Optional: a ParticleSystem (Play On Awake OFF) that plays when the checkpoint is reached
    public ParticleSystem ActivateEffect;

    public Vector3 Position => RespawnPoint != null ? RespawnPoint.position : transform.position;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;
        if (RespawnManager.Instance == null) return;

        // SetCheckpoint is true only for a new checkpoint, so touching the same one twice does nothing
        if (RespawnManager.Instance.SetCheckpoint(this))
        {
            Sfx.Play(SfxType.Checkpoint);

            if (ActivateEffect != null)
                ActivateEffect.Play();
        }
    }
}