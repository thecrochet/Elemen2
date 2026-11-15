using System.Collections.Generic;
using UnityEngine;

// What hit the object
public enum HitSource
{
    Dash,
    GroundPound
}

// Put this on any object with a (non-trigger) Collider that the player can break.
// The player calls TakeHit() from CC_ELEMENTO:
//   - during a dash, in OnMovementHit
//   - on a ground pound landing, in GroundPoundImpact (shockwave)
//
// CHAIN REACTION: when a block breaks, the blocks touching it in the direction of the hit break one
// after the other (ChainDelay apart), like a Mario ground pound through a column or a Sonic dash
// through a row. The chain only goes FORWARD in the hit direction, so a ground pound never eats a
// whole floor sideways.
//
// When broken it is hidden (not destroyed) and comes back when the player respawns,
// unless the player reached a checkpoint after breaking it.
public class DestructibleObject : MonoBehaviour, IResettable
{
    // Which power can break this object?
    public bool BreakableByDash = true;
    public bool BreakableByGroundPound = true;

    // How many hits it takes to break (1 = breaks right away)
    public int HitsToBreak = 1;

    // A dash touches the object on many physics steps, this avoids counting them all as new hits
    public float HitCooldown = 0.2f;

    [Header("Chain reaction")]
    // Turn it off to make a block that never passes the hit on
    public bool ChainReaction = true;

    // Seconds between one block and the next one of the chain (0.05 = a fast ripple)
    public float ChainDelay = 0.05f;

    // How far from this block we look for neighbors (a small gap between blocks is OK)
    public float ChainReach = 0.15f;

    // 1 = only the block exactly in front of the hit, lower = also blocks a bit diagonal
    [Range(0.3f, 1f)] public float ChainMinDot = 0.7f;

    // Only objects on these layers can be reached by the chain
    public LayerMask ChainLayers = ~0;

    // Optional: a ParticleSystem that is a CHILD of this object (Play On Awake OFF).
    // It plays when the object breaks. No object is created while playing.
    public ParticleSystem BreakEffect;

    [Header("Cracks (optional)")]
    // A renderer (usually a child quad "Cracks" with the CrackOverlay material) that shows the damage.
    // Leave empty for a block with no cracks.
    public Renderer CrackRenderer;

    // The block starts already cracked: 0 = clean, 1..3 = the crack stage shown from the start.
    // A block with HitsToBreak 1 and PreCracked 2 looks damaged but still breaks with ONE hit.
    [Range(0, 3)] public int PreCracked = 0;

    // The most advanced crack picture (the overlay has 3 stages)
    private const int MaxStage = 3;
    private static readonly int StageId = Shader.PropertyToID("_Stage");
    private MaterialPropertyBlock _crackBlock;

    // The player asks this to know if the block under it just broke (to keep slamming through)
    public bool IsBroken
    {
        get { return _isBroken; }
    }

    // One shared array for every block: the neighbor search makes no garbage
    private static readonly Collider[] _neighborHits = new Collider[16];

    private Renderer[] _renderers;
    private Collider[] _colliders;
    private Bounds _bounds;

    private int _hitsTaken;
    private float _lastHitTime = -999f;
    private bool _isBroken;

    // the chain hit that is waiting for its turn (while this component is enabled)
    private HitSource _chainSource;
    private Vector3 _chainDirection;
    private float _chainBreakTime;

    // the last saved state (see CommitState)
    private int _committedHits;
    private bool _committedBroken;

    private void Awake()
    {
        // Search everything ONCE at load, so nothing is searched or created while playing
        List<Renderer> renderers = new List<Renderer>();

        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
        {
            // the particle effect must stay on, it plays after the object is hidden
            if (!(r is ParticleSystemRenderer))
                renderers.Add(r);
        }

        // the crack picture must stay out of the "hide when broken" list handling below
        _renderers = renderers.ToArray();
        _colliders = GetComponentsInChildren<Collider>(true);

        // the box that wraps all the colliders (used to find the neighbors)
        if (_colliders.Length > 0)
        {
            _bounds = _colliders[0].bounds;

            for (int i = 1; i < _colliders.Length; i++)
                _bounds.Encapsulate(_colliders[i].bounds);
        }
    }

    private void Start()
    {
        UpdateCracks();

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);

        // Update only runs while a chain hit is waiting (ScheduleChain turns it on)
        enabled = false;

        UpdateCracks();
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // Only runs while a chain hit is waiting for its turn
    private void Update()
    {
        if (Time.time < _chainBreakTime) return;

        enabled = false;
        ApplyHit(_chainSource, _chainDirection);
    }

    // A hit with no direction: it never starts a chain
    public void TakeHit(HitSource source)
    {
        TakeHit(source, Vector3.zero);
    }

    // direction = where the hit goes (the dash direction, or down for a ground pound)
    public void TakeHit(HitSource source, Vector3 direction)
    {
        if (_isBroken) return;

        // is this power allowed to break this object?
        if (!CanBeBrokenBy(source)) return;

        // ignore hits that come too fast one after the other
        if (Time.time - _lastHitTime < HitCooldown) return;
        _lastHitTime = Time.time;

        ApplyHit(source, direction);
    }

    private bool CanBeBrokenBy(HitSource source)
    {
        if (source == HitSource.Dash && !BreakableByDash) return false;
        if (source == HitSource.GroundPound && !BreakableByGroundPound) return false;

        return true;
    }

    // counts one hit, and breaks the object when it has taken enough of them
    private void ApplyHit(HitSource source, Vector3 direction)
    {
        if (_isBroken) return;

        _hitsTaken++;

        UpdateCracks();

        if (_hitsTaken >= HitsToBreak)
            Break(source, direction);
    }

    private void Break(HitSource source, Vector3 direction)
    {
        _isBroken = true;

        // hidden + no collision right away, so the player passes through
        SetVisibleAndSolid(false);

        Sfx.PlayAt(SfxType.BlockBreak, transform.position);

        if (BreakEffect != null)
            BreakEffect.Play();

        // pass the hit on to the blocks in front of this one
        if (ChainReaction && direction.sqrMagnitude > 0.01f)
            HitNeighbors(source, direction.normalized);
    }

    private void HitNeighbors(HitSource source, Vector3 direction)
    {
        Vector3 halfExtents = _bounds.extents + Vector3.one * ChainReach;

        // NonAlloc = fills the shared array instead of creating a new one
        int count = Physics.OverlapBoxNonAlloc(_bounds.center, halfExtents, _neighborHits,
            Quaternion.identity, ChainLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            DestructibleObject other = _neighborHits[i].GetComponentInParent<DestructibleObject>();

            if (other != null && other != this)
                other.ScheduleChain(source, direction, _bounds.center);
        }
    }

    // A neighbor asks this block to break in a moment
    private void ScheduleChain(HitSource source, Vector3 direction, Vector3 fromCenter)
    {
        // already broken, or already waiting for its turn (enabled = waiting)
        if (_isBroken || enabled) return;

        if (!CanBeBrokenBy(source)) return;

        // only the blocks in FRONT of the hit: below for a ground pound, ahead for a dash
        Vector3 toMe = _bounds.center - fromCenter;
        if (toMe.sqrMagnitude < 0.0001f) return;
        if (Vector3.Dot(toMe.normalized, direction) < ChainMinDot) return;

        _chainSource = source;
        _chainDirection = direction;
        _chainBreakTime = Time.time + ChainDelay;
        enabled = true;
    }

    // SAVE: broken before a checkpoint = it stays broken
    public void CommitState()
    {
        _committedBroken = _isBroken;
        _committedHits = _hitsTaken;
    }

    // LOAD: back to the last saved state
    public void ResetObject()
    {
        _isBroken = _committedBroken;
        _hitsTaken = _committedHits;
        _lastHitTime = -999f;

        // forget a chain hit that was still waiting
        enabled = false;

        SetVisibleAndSolid(!_isBroken);
        UpdateCracks();

        if (BreakEffect != null)
            BreakEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // The crack stage: the biggest of "starts cracked" and "how damaged it is now" (3 stages at most)
    private int CurrentStage()
    {
        int fromHits = 0;

        if (_hitsTaken > 0 && HitsToBreak > 0)
            fromHits = Mathf.CeilToInt(MaxStage * (float)_hitsTaken / HitsToBreak);

        return Mathf.Clamp(Mathf.Max(PreCracked, fromHits), 0, MaxStage);
    }

    private void UpdateCracks()
    {
        if (CrackRenderer == null) return;

        if (_crackBlock == null)
            _crackBlock = new MaterialPropertyBlock();

        CrackRenderer.GetPropertyBlock(_crackBlock);
        _crackBlock.SetFloat(StageId, CurrentStage());
        CrackRenderer.SetPropertyBlock(_crackBlock);
    }

    private void SetVisibleAndSolid(bool value)
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].enabled = value;

        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = value;
    }
}
