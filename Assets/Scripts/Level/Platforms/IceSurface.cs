using UnityEngine;

// Put this on a block (or on its parent) to make its top slippery.
// The character controller reads these two numbers while he stands on it.
//
// For comparison, normal ground is 120 (speeding up) and 150 (slowing down). Lower = more slippery.
//   Acceleration 40 and Deceleration 20: stopping from walking takes about 3 units, from running about 6.
//   Deceleration 8 is very slippery: stopping from walking takes about 7.5 units.
public class IceSurface : MonoBehaviour
{
    // How fast the speed builds up while pushing (also while turning around)
    public float Acceleration = 40f;

    // How fast the character slows down when he stops pushing
    public float Deceleration = 20f;
}