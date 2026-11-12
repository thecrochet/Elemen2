using UnityEngine;

// A spring. Put it on a pad with a Collider that has "Is Trigger" checked, sitting on the TOP
// of the pad (about 0.5 units tall). The solid collider of the pad itself stays a normal one.
//
// Touching it launches the character up. With the EARTH element, a ground pound onto the pad
// gives a much bigger launch.
//
// Height of a launch = strength x strength / (2 x 70), so 26 is about 4.8 units and 40 is about 11.4.
// (A normal full jump is about 3.15 units.)
public class BouncePad : MonoBehaviour
{
    // Normal launch
    public float LaunchStrength = 26f;

    // Launch after a ground pound onto the pad
    public float GroundPoundLaunchStrength = 40f;

    // Optional: a ParticleSystem (Play On Awake OFF, Looping OFF) played at each launch
    public ParticleSystem LaunchEffect;

    private void OnTriggerEnter(Collider other)
    {
        // only the player (TryGetComponent doesn't create garbage)
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        if (!player.LaunchFromPad(LaunchStrength, GroundPoundLaunchStrength)) return;

        Sfx.Play(SfxType.PadLaunch);

        if (LaunchEffect != null)
            LaunchEffect.Play();
    }
}