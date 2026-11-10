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

    // Optional: a ParticleSystem that is a CHILD of this object (Play On Awake OFF).
    // It plays when the object breaks. No object is created while playing.
    public ParticleSystem BreakEffect;

    private Renderer[] _renderers;
    private Collider[] _colliders;

    private int _hitsTaken;
    private float _lastHitTime = -999f;
    private bool _isBroken;

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

        _renderers = renderers.ToArray();
        _colliders = GetComponentsInChildren<Collider>(true);
    }

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

    public void TakeHit(HitSource source)
    {
        if (_isBroken) return;

        // is this power allowed to break this object?
        if (source == HitSource.Dash && !BreakableByDash) return;
        if (source == HitSource.GroundPound && !BreakableByGroundPound) return;

        // ignore hits that come too fast one after the other
        if (Time.time - _lastHitTime < HitCooldown) return;
        _lastHitTime = Time.time;

        _hitsTaken++;

        if (_hitsTaken >= HitsToBreak)
            Break();
    }

    private void Break()
    {
        _isBroken = true;

        // hidden + no collision right away, so the player passes through
        SetVisibleAndSolid(false);

        Sfx.PlayAt(SfxType.BlockBreak, transform.position);

        if (BreakEffect != null)
            BreakEffect.Play();
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

        SetVisibleAndSolid(!_isBroken);

        if (BreakEffect != null)
            BreakEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void SetVisibleAndSolid(bool value)
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].enabled = value;

        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = value;
    }
}