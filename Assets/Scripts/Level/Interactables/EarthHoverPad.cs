using UnityEngine;

// EARTH: a hexagon pad that floats in the air. You can stand on it, but a GROUND POUND onto it
// shatters it and launches the ball high, so you can fly to the next pad and slam that one too.
// (A chain of these is a ladder you climb by slamming.)
//
// A normal landing does nothing: the pad is just a platform. Only a ground pound breaks it.
//
// Height of the launch = strength x strength / (2 x 70): 40 gives about 11.4 units.
// Put the next pad 6 to 9 units higher (or the same height, up to about 10 units away) so the
// ball arrives ABOVE it and can slam down onto it. The yellow gizmo shows the launch height.
//
// SETUP (all on ONE object):
//   - a solid collider (Box or convex Mesh Collider) that the ball stands on
//   - a SECOND collider with "Is Trigger" checked, taller than the solid one on the TOP side only,
//     so a slam is noticed even if the ball is already touching it
//   - this script, on the SAME object as the trigger (Unity sends the trigger events to that object)
//   - the mesh: on the same object, or a CHILD assigned to "Visual" (no collider on it) if you want it to bob,
//     so the solid collider doesn't move under the player's feet
//   - layer Default (like the CrumblingBlock)
//   Easiest: GameObject > Elemento > Level > Earth Hover Pad builds all of this and asks first.
//
// A death always puts every pad back (see ResetObject), so each retry starts the same way.
public class EarthHoverPad : MonoBehaviour, IResettable
{
    // Launch speed after the slam (40 = about 11.4 units high, like the bounce pad's slam)
    public float SlamLaunchStrength = 40f;

    // Seconds before the pad comes back. 0 or less = it stays gone until the player respawns.
    public float ReturnDelay = 2f;

    // The mesh (a child) that bobs up and down while the pad waits. Optional.
    public Transform Visual;

    // How far the mesh bobs (units) and how fast (cycles per second). 0 = no bob.
    public float BobHeight = 0.12f;
    public float BobSpeed = 0.6f;

    // Optional: a ParticleSystem (Play On Awake OFF, Looping OFF) played when the slam breaks the pad
    // (FX_PadLaunchPound). It can be a child of the pad: the particles are not hidden with the mesh.
    public ParticleSystem BreakEffect;

    // Optional: debris (Play On Awake OFF, Looping OFF) played at the same moment (FX_Crumble)
    public ParticleSystem DebrisEffect;

    // Optional: a SMALL flash played when the pad comes back
    public ParticleSystem ReturnEffect;

    // Tick this to see in the Console what the ball is doing when it touches the pad (to find why a slam doesn't work)
    public bool LogContacts = false;

    private bool _gone;
    private float _timer;
    private float _phase;
    private Vector3 _visualStart;

    // found ONCE, so nothing is searched or created while playing
    private Collider[] _colliders;
    private Renderer[] _renderers;

    private void Start()
    {
        // every collider of the pad: the solid one, the trigger, and any collider on a child
        _colliders = GetComponentsInChildren<Collider>(true);

        System.Collections.Generic.List<Renderer> found = new System.Collections.Generic.List<Renderer>();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (!(r is ParticleSystemRenderer))
                found.Add(r);
        }
        _renderers = found.ToArray();

        if (Visual != null)
            _visualStart = Visual.localPosition;

        // each pad bobs out of step with its neighbours
        _phase = (transform.position.x * 0.37f + transform.position.y * 0.53f) * 6.2831f;

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // Enter AND Stay: if the ball is already inside the trigger when it slams, Enter would not fire again
    private void OnTriggerEnter(Collider other)
    {
        if (LogContacts && other.TryGetComponent(out CC_ELEMENTO p))
            Debug.Log("EarthHoverPad '" + name + "': the ball touched the trigger. State = " + p.CurrentState + ", Power = " + p.CurrentPower
                      + (p.CurrentPower != PowerUpType.Earth ? "  <-- needs EARTH to ground pound" : ""), this);

        TrySlam(other);
    }
    private void OnTriggerStay(Collider other) { TrySlam(other); }

    private void TrySlam(Collider other)
    {
        if (_gone) return;
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        // only a ground pound counts: a normal landing returns false and the pad stays
        if (!player.LaunchFromPadSlamOnly(SlamLaunchStrength)) return;

        Break();
    }

    private void Break()
    {
        _gone = true;
        _timer = ReturnDelay;
        SetPadActive(false);

        if (Visual != null)
            Visual.localPosition = _visualStart;

        Sfx.PlayAt(SfxType.PadLaunch, transform.position);
        Sfx.PlayAt(SfxType.CrumbleFall, transform.position);

        if (BreakEffect != null) BreakEffect.Play(true);
        if (DebrisEffect != null) DebrisEffect.Play(true);
    }

    private void Update()
    {
        if (!_gone)
        {
            // the mesh floats (the solid collider never moves)
            if (Visual != null && BobHeight > 0f)
            {
                float y = Mathf.Sin(Time.time * BobSpeed * 6.2831f + _phase) * BobHeight;
                Visual.localPosition = _visualStart + new Vector3(0f, y, 0f);
            }
            return;
        }

        // it comes back by itself, unless ReturnDelay is 0 or less
        if (ReturnDelay > 0f)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                Restore();

                if (ReturnEffect != null)
                    ReturnEffect.Play(true);
            }
        }
    }

    private void Restore()
    {
        _gone = false;
        SetPadActive(true);

        if (Visual != null)
            Visual.localPosition = _visualStart;
    }

    private void SetPadActive(bool active)
    {
        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = active;

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].enabled = active;
    }

    // SAVE: nothing to save. A broken pad is never kept after a death or a checkpoint.
    public void CommitState()
    {
    }

    // LOAD: after a death the pad is always back
    public void ResetObject()
    {
        if (_colliders == null) return;   // not started yet
        Restore();
    }

    // The launch height: a line and a small box where the top of the arc is
    private void OnDrawGizmos()
    {
        float height = (SlamLaunchStrength * SlamLaunchStrength) / (2f * 70f);
        Vector3 start = transform.position + Vector3.up * 0.5f;
        Vector3 top = start + Vector3.up * height;

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.9f);
        Gizmos.DrawLine(start, top);
        Gizmos.DrawWireCube(top, new Vector3(1.2f, 0.1f, 0.1f));
    }
}
