using UnityEngine;

// A flame that switches on and off on a rhythm (a world 1 hazard that trains timing).
//
//   OFF  ->  WARN (a few sparks, the player sees it is about to light)  ->  ON (the flame kills)  ->  OFF ...
//
// STRUCTURE (the menu GameObject > Elemento > Hazards > Fire Jet builds it):
//   FireJet          this script + a solid Box Collider (the nozzle the ball can stand next to)
//    +- Nozzle       the picture of the base, no collider
//    +- KillZone     Box Collider [trigger] + Hazard (only enabled while the flame is ON)
//    +- Flame        Particle System (Looping ON, Play On Awake OFF): plays only while the flame is ON
//    +- WarnEffect   Particle System (Looping ON, Play On Awake OFF): plays only during the warning
//
// FAIRNESS: it follows the LEVEL CLOCK like every other moving hazard:
//   - it only runs while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
// So every retry shows exactly the same timing. Use "Time Offset" to put two jets out of step.
[DisallowMultipleComponent]
public class FireJet : MonoBehaviour, IResettable
{
    [Header("Rhythm (seconds)")]
    // How long the flame burns (it kills during this time)
    public float OnTime = 1.2f;

    // How long it is off, INCLUDING the warning at the end
    public float OffTime = 1.6f;

    // The last part of the off time: sparks show the flame is about to light
    public float WarnTime = 0.4f;

    // Shifts the rhythm (so a row of jets is not in step)
    public float TimeOffset = 0f;

    [Header("Parts")]
    // The trigger that kills (it carries the Hazard script). It is switched on and off.
    public Collider KillZone;

    // Looping flame particles, played while ON
    public ParticleSystem Flame;

    // Looping sparks, played during the warning
    public ParticleSystem WarnEffect;

    private enum Phase
    {
        Off,
        Warn,
        On
    }

    private Phase _phase = Phase.Off;
    private float _time;            // the jet's own clock
    private float _committedTime;   // the "save": the clock at the last checkpoint
    private float _period;
    private float _warn;

    private void Start()
    {
        _warn = Mathf.Clamp(WarnTime, 0f, OffTime);
        _period = Mathf.Max(OnTime + OffTime, 0.1f);

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);

        Apply(Evaluate(), true);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // Colliders change in the physics step
    private void FixedUpdate()
    {
        // only runs while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;

        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += Time.fixedDeltaTime;

        Apply(Evaluate(), false);
    }

    private Phase Evaluate()
    {
        float t = Mathf.Repeat(_time + TimeOffset, _period);

        if (t < OffTime - _warn) return Phase.Off;
        if (t < OffTime) return Phase.Warn;
        return Phase.On;
    }

    private void Apply(Phase phase, bool force)
    {
        if (!force && phase == _phase) return;

        _phase = phase;

        if (KillZone != null)
            KillZone.enabled = phase == Phase.On;

        SetPlaying(Flame, phase == Phase.On);
        SetPlaying(WarnEffect, phase == Phase.Warn);
    }

    private static void SetPlaying(ParticleSystem system, bool play)
    {
        if (system == null) return;

        if (play)
        {
            if (!system.isPlaying)
                system.Play(true);
        }
        else if (system.isEmitting)
        {
            // the particles already in the air finish by themselves
            system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    // SAVE (start of the level and at each checkpoint)
    public void CommitState()
    {
        _committedTime = _time;
    }

    // LOAD (at each respawn)
    public void ResetObject()
    {
        _time = _committedTime;
        Apply(Evaluate(), true);

        // nothing flies on after a death
        if (Flame != null)
            Flame.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (WarnEffect != null)
            WarnEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        Apply(Evaluate(), true);
    }
}
