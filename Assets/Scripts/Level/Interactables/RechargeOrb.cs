using UnityEngine;

// A floating orb that gives ONE extra use of the ball's ability, until it lands (or dies).
//
//   Fire  orb = one extra DASH
//   Air   orb = one extra JUMP in the air (the double jump becomes a triple jump)
//   Water orb = one extra GRAVITY FLIP (flip, then flip back in mid air)
//
// There is no Earth orb: Earth uses the EarthHoverPad instead.
//
// The orb only works if the ball has that element right now. If it doesn't, the ball just passes through
// and the orb stays (it never gets wasted). After a pickup it disappears, then comes back by itself.
//
// SETUP (all on ONE object):
//   - a Sphere Collider with "Is Trigger" checked, Radius 0.8
//   - this script, with "Power" set to Fire, Air or Water
//   - a CHILD object with the mesh (no collider on it), assigned to "Visual"
//   - layer Default (like the other level objects)
//   Easiest: GameObject > Elemento > Level > Recharge Orb (Fire / Air / Water) builds all of this and asks first.
//
// A death always puts the orb back (see ResetObject), so each retry starts the same way.
public class RechargeOrb : MonoBehaviour, IResettable
{
    // Which element this orb belongs to (Fire, Air or Water)
    public PowerUpType Power = PowerUpType.Fire;

    // Seconds before the orb comes back. 0 or less = it stays gone until the player respawns.
    public float ReturnDelay = 2f;

    // The mesh (a child). It is hidden after a pickup and spins slowly while waiting. Optional.
    public Transform Visual;

    // Spin of the mesh in degrees per second (0 = no spin)
    public float SpinSpeed = 60f;

    // Optional: a ParticleSystem (Play On Awake OFF, Looping OFF) played when the ball takes the orb.
    // It can be a child of the orb: the particles are not hidden with the mesh.
    public ParticleSystem UseEffect;

    // Optional: a SMALL flash (Play On Awake OFF, Looping OFF) played when the orb comes back
    public ParticleSystem ReturnEffect;

    private bool _used;
    private float _timer;

    // found ONCE, so nothing is searched or created while playing
    private Collider[] _colliders;
    private Renderer[] _renderers;

    private void Start()
    {
        _colliders = GetComponents<Collider>();

        // all the meshes, except particle systems (they must stay visible when the orb disappears)
        System.Collections.Generic.List<Renderer> found = new System.Collections.Generic.List<Renderer>();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (!(r is ParticleSystemRenderer))
                found.Add(r);
        }
        _renderers = found.ToArray();

        if (Power != PowerUpType.Fire && Power != PowerUpType.Air && Power != PowerUpType.Water)
            Debug.LogWarning("RechargeOrb '" + name + "': Power must be Fire, Air or Water.", this);

        // Register also saves the starting state right away
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
        if (_used) return;
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        // the ball doesn't have this element (or is dead): the orb stays
        if (!player.GiveRecharge(Power)) return;

        _used = true;
        _timer = ReturnDelay;
        SetOrbActive(false);

        Sfx.PlayAt(SfxType.RechargeUse, transform.position);

        if (UseEffect != null)
            UseEffect.Play(true);
    }

    private void Update()
    {
        if (!_used)
        {
            // a slow spin, only on the mesh
            if (Visual != null && SpinSpeed != 0f)
                Visual.Rotate(0f, SpinSpeed * Time.deltaTime, 0f, Space.Self);
            return;
        }

        // it comes back by itself, unless ReturnDelay is 0 or less
        if (ReturnDelay > 0f)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                Restore();
                Sfx.PlayAt(SfxType.RechargeReturn, transform.position);

                if (ReturnEffect != null)
                    ReturnEffect.Play(true);
            }
        }
    }

    private void Restore()
    {
        _used = false;
        SetOrbActive(true);
    }

    private void SetOrbActive(bool active)
    {
        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = active;

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].enabled = active;
    }

    // SAVE: nothing to save. A used orb is never kept after a death or a checkpoint.
    public void CommitState()
    {
    }

    // LOAD: after a death the orb is always back (no sound, no flash)
    public void ResetObject()
    {
        if (_colliders == null) return;   // not started yet
        Restore();
    }

    // The trigger radius, in the color of the element
    private void OnDrawGizmos()
    {
        switch (Power)
        {
            case PowerUpType.Fire: Gizmos.color = new Color(1f, 0.45f, 0.1f, 0.9f); break;
            case PowerUpType.Air: Gizmos.color = new Color(0.7f, 1f, 0.9f, 0.9f); break;
            case PowerUpType.Water: Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.9f); break;
            default: Gizmos.color = Color.white; break;
        }

        if (TryGetComponent(out SphereCollider sphere))
            Gizmos.DrawWireSphere(transform.TransformPoint(sphere.center), sphere.radius * transform.lossyScale.x);
        else
            Gizmos.DrawWireSphere(transform.position, 0.8f);
    }
}
