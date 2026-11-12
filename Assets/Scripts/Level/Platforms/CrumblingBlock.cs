using System.Collections.Generic;
using UnityEngine;

// A platform that falls apart soon after you touch it, then comes back (Geometry Dash style).
//
// SETUP (all on ONE object):
//   - a normal Box Collider (the solid block the player stands on)
//   - a SECOND Box Collider with "Is Trigger" checked, taller on the TOP side only
//     (size Y + 0.4 and center Y + 0.2): touching it starts the countdown
//   - this script
//   - a CHILD object with the mesh, assigned to "Visual" (no collider on it): only the mesh shakes,
//     so the solid collider doesn't move under the player's feet
//
// A death always puts every block back (see ResetObject), so each retry starts the same way.
public class CrumblingBlock : MonoBehaviour, IResettable
{
    // Seconds between the first touch and the fall (the block shakes during this time)
    public float CrumbleDelay = 0.4f;

    // Seconds before the block comes back. 0 or less = it stays gone until the player respawns.
    public float ReappearDelay = 2.5f;

    // The mesh that shakes (a child). Optional.
    public Transform Visual;

    // How far the mesh shakes, in units
    public float ShakeAmount = 0.05f;

    // Optional: a ParticleSystem (Play On Awake OFF, Looping OFF) played when the block falls apart.
    // It can be a child of the block: the particles are not hidden with the mesh.
    public ParticleSystem CrumbleEffect;

    private enum BlockState
    {
        Solid,
        Crumbling,
        Gone
    }

    private BlockState _state = BlockState.Solid;
    private float _timer;

    // found ONCE, so nothing is searched or created while playing
    private Collider[] _colliders;
    private Renderer[] _renderers;
    private Vector3 _visualStart;

    private void Start()
    {
        // all the colliders of this object (the solid one AND the trigger)
        _colliders = GetComponents<Collider>();

        // all the meshes, except particle systems (they must stay visible when the block disappears)
        List<Renderer> found = new List<Renderer>();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (!(r is ParticleSystemRenderer))
                found.Add(r);
        }
        _renderers = found.ToArray();

        if (Visual != null)
            _visualStart = Visual.localPosition;

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // The player touched the trigger on top of the block
    private void OnTriggerEnter(Collider other)
    {
        if (_state != BlockState.Solid) return;
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        _state = BlockState.Crumbling;
        _timer = CrumbleDelay;

        Sfx.PlayAt(SfxType.CrumbleWarn, transform.position);
    }

    private void Update()
    {
        switch (_state)
        {
            case BlockState.Crumbling:
                {
                    _timer -= Time.deltaTime;

                    // shake the mesh only (random numbers make no garbage)
                    if (Visual != null)
                    {
                        Vector3 shake = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * ShakeAmount;
                        Visual.localPosition = _visualStart + shake;
                    }

                    if (_timer <= 0f)
                        Crumble();
                    break;
                }

            case BlockState.Gone:
                {
                    // it comes back by itself, unless ReappearDelay is 0 or less
                    if (ReappearDelay > 0f)
                    {
                        _timer -= Time.deltaTime;

                        if (_timer <= 0f)
                            Restore();
                    }
                    break;
                }
        }
    }

    // The block falls apart: no mesh, no collision
    private void Crumble()
    {
        _state = BlockState.Gone;
        _timer = ReappearDelay;

        SetBlockActive(false);
        Sfx.PlayAt(SfxType.CrumbleFall, transform.position);

        if (Visual != null)
            Visual.localPosition = _visualStart;

        if (CrumbleEffect != null)
            CrumbleEffect.Play();
    }

    // The block is back, solid and ready for the next touch
    private void Restore()
    {
        _state = BlockState.Solid;

        SetBlockActive(true);

        if (Visual != null)
            Visual.localPosition = _visualStart;
    }

    private void SetBlockActive(bool active)
    {
        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = active;

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].enabled = active;
    }

    // SAVE: nothing to save. A crumbled block is never kept after a death or a checkpoint.
    public void CommitState()
    {
    }

    // LOAD: after a death the block is always back and solid
    public void ResetObject()
    {
        Restore();
    }
}