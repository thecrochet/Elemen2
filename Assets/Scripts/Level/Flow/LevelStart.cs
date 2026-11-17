using UnityEngine;

// Put this on the START arch of a level (the arch the ball spawns in).
// It plays one short burst when the level begins: the arch "lights up" as the ball arrives.
// Call Play() yourself if you also want it on every respawn.
public class LevelStart : MonoBehaviour
{
    // Optional: a ParticleSystem that is a CHILD of this arch (Play On Awake OFF, Looping OFF)
    public ParticleSystem StartEffect;

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        if (StartEffect != null)
            StartEffect.Play(true);
    }
}