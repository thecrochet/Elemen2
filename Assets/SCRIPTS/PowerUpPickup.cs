using UnityEngine;

public enum PowerUpType
{
    Fire,     // dash (breaks blocks)
    Air,      // double jump + glide
    Earth,    // ground pound (+ super jump from a bounce pad)
    Water,    // wall jump + vapor (gravity flip)
    None      // keep the order: Unity saves the numbers, so pickups you already placed keep their element
}

// One pickup script for every power-up. Pick the type in the Inspector.
// Put it on an object with a Collider that has "Is Trigger" checked.
// It is hidden (not destroyed) when picked up, and comes back when the player respawns,
// unless the player reached a checkpoint after picking it up.
public class PowerUpPickup : MonoBehaviour, IResettable
{
    public PowerUpType Type;

    private bool _committedActive = true;

    private void Start()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        // replaces the old power with this one
        player.SetPower(Type);
        Sfx.Play(SfxType.PowerUp);
        Vfx.PlayTinted(VfxType.PowerUpCollect, transform.position, Quaternion.identity, Type);

        // hide it instead of Destroy, so it can come back after a respawn
        gameObject.SetActive(false);
    }

    // SAVE: is the pickup still there? (collected before a checkpoint = it stays gone)
    public void CommitState()
    {
        _committedActive = gameObject.activeSelf;
    }

    // LOAD: back to the last saved state
    public void ResetObject()
    {
        gameObject.SetActive(_committedActive);
    }
}