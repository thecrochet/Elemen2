/*using UnityEngine;

public enum PowerUpType
{
    Dash,
    DoubleJump,
    GroundPound,
    WallJump
}

// One pickup script for every power-up. Pick the type in the Inspector.
// Put it on an object with a Collider that has "Is Trigger" checked.
// (This replaces DashPickup.)
public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType Type;

    private void OnTriggerEnter(Collider other)
    {
        CC_ELEMENTO player = other.GetComponent<CC_ELEMENTO>();
        if (player == null) return;

        switch (Type)
        {
            case PowerUpType.Dash:
                player.UnlockDash();
                break;
            case PowerUpType.DoubleJump:
                player.UnlockDoubleJump();
                break;
            case PowerUpType.GroundPound:
                player.UnlockGroundPound();
                break;
            case PowerUpType.WallJump:
                player.UnlockWallJump();
                break;
        }

        Destroy(gameObject);
    }
}
*/