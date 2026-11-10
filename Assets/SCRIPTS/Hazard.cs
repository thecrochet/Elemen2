using UnityEngine;

// Put this on spikes, saws, lava...: anything that kills the player on touch.
// The object needs a Collider with "Is Trigger" checked.
public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // only the player can die (TryGetComponent doesn't create garbage)
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.KillPlayer();
    }
}
