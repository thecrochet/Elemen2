using UnityEngine;

// A platform you can jump UP through from below and stand on from above.
// Press DOWN + JUMP to drop through it.
//
// HOW IT WORKS: the player script (CC_ELEMENTO) asks this script on every collision: "are you solid for me right now?"
//   - the feet are above the top face -> yes, you stand on it
//   - the feet are below the top face (coming from under, or dropping) -> no, you pass through
// The "top" is the local up side. Rotate the object 180 degrees around Z to make it solid from below instead.
//
// SETUP: a NeonBlock + this script. It needs a Box Collider that is NOT a trigger.
[RequireComponent(typeof(BoxCollider))]
public class OneWayPlatform : MonoBehaviour
{
    // How far under the top face the feet may be and still count as "on top" (world units)
    public float Tolerance = 0.05f;

    // Can the player drop through with DOWN + JUMP?
    public bool AllowDropThrough = true;

    private BoxCollider _box;

    private void Awake()
    {
        _box = GetComponent<BoxCollider>();
    }

    // Called by CC_ELEMENTO: true = solid, false = the ball passes through
    public bool IsSolidFor(Vector3 feetPosition, bool dropping)
    {
        if (AllowDropThrough && dropping)
            return false;

        // the middle of the top face, in world space
        Vector3 top = transform.TransformPoint(_box.center + new Vector3(0f, _box.size.y * 0.5f, 0f));

        return Vector3.Dot(feetPosition - top, transform.up) >= -Tolerance;
    }
}
