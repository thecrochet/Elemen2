using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// A fireball that leaps out of a lava pit, flies up in an arc and falls back in (a world 1 hazard, like the lava
// flames of Mario). It trains TIMING and READING a height, the same way the FireJet does.
//
//   REST (hidden in the lava)  ->  WARN (the lava bubbles at the surface)  ->  LEAP (the fireball kills while it is in the air) -> REST ...
//
// STRUCTURE (the menu GameObject > Elemento > Hazards > Lava Leaper builds it):
//   LavaLeaper        this script. Its position = the lava SURFACE. Its local UP = the direction of the leap.
//    +- Fireball      the picture + a trigger (Sphere Collider, Is Trigger) + Hazard + a kinematic Rigidbody. Moves up and down.
//    +- Flame         Particle System (Looping ON, Play On Awake OFF): follows the fireball while it is in the air
//    +- WarnEffect    Particle System (Looping ON, Play On Awake OFF): bubbles at the surface during the warning
//    +- SplashEffect  Particle System (Looping OFF, Play On Awake OFF): one burst when it leaves and when it falls back
//
// FAIRNESS: it follows the LEVEL CLOCK like every other moving hazard:
//   - it only runs while the level clock runs
//   - after a death it goes back to where it was at your last checkpoint
// So every retry shows exactly the same timing. Use "Time Offset" to put two leapers out of step.
//
// THE NUMBERS: the fireball goes up and down in a smooth arc (slow at the top, fast near the lava).
//   Launch speed = 4 x Height / Air Time. With Height 5 and Air Time 1.2 that is about 16.7 units per second:
//   keep it under about 25 (the speed limit for readable hazards).
//   Jump apex of the player is about 3.15, so a Height of 4 or more means you cannot jump over its top: you must wait.
//
// Rotate the root 180 degrees around Z for a fireball that drops from the ceiling.
[DisallowMultipleComponent]
public class LavaLeaper : MonoBehaviour, IResettable
{
    [Header("Leap")]
    // How high the fireball goes above the surface (world units)
    [Min(0.5f)] public float Height = 5f;

    // How long it stays in the air, up and down (seconds)
    [Min(0.3f)] public float AirTime = 1.2f;

    [Header("Rhythm (seconds)")]
    // How long it waits hidden between two leaps, INCLUDING the warning at the end
    [Min(0.2f)] public float RestTime = 1.6f;

    // The last part of the rest: the lava bubbles to show a leap is coming
    [Min(0f)] public float WarnTime = 0.5f;

    // Shifts the rhythm (so a row of leapers is not in step)
    public float TimeOffset = 0f;

    [Header("Parts")]
    // The fireball (picture + trigger + Hazard). It is hidden while it rests in the lava.
    public Transform Fireball;

    // Looping flame, plays only while the fireball is in the air
    public ParticleSystem Flame;

    // Looping bubbles at the surface during the warning
    public ParticleSystem WarnEffect;

    // One burst at the surface when the fireball leaves and when it falls back (optional)
    public ParticleSystem SplashEffect;

    private enum Phase
    {
        Rest,
        Warn,
        Leap
    }

    private Phase _phase = Phase.Rest;
    private float _time;            // the leaper's own clock
    private float _committedTime;   // the "save": the clock at the last checkpoint
    private float _period;
    private float _warn;

    private void Start()
    {
        _warn = Mathf.Clamp(WarnTime, 0f, RestTime);
        _period = Mathf.Max(RestTime + AirTime, 0.5f);

        // Register also saves the starting state right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);

        Apply(true);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    // the trigger moves, so it moves in the physics step
    private void FixedUpdate()
    {
        // only runs while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;

        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += Time.fixedDeltaTime;

        Apply(false);
    }

    // where we are in the cycle: the phase, and 0..1 through the leap
    private Phase Evaluate(out float leapProgress)
    {
        float t = Mathf.Repeat(_time + TimeOffset, _period);
        leapProgress = 0f;

        if (t < RestTime - _warn) return Phase.Rest;
        if (t < RestTime) return Phase.Warn;

        leapProgress = Mathf.Clamp01((t - RestTime) / Mathf.Max(AirTime, 0.1f));
        return Phase.Leap;
    }

    private void Apply(bool force)
    {
        Phase phase = Evaluate(out float progress);

        if (force || phase != _phase)
        {
            Phase previous = _phase;
            _phase = phase;

            bool leaping = phase == Phase.Leap;

            // sounds: only when the phase really changes (not at the start or after a respawn)
            if (!force)
            {
                if (phase == Phase.Warn) Sfx.PlayAt(SfxType.LeaperWarn, transform.position);
                if (phase == Phase.Leap) Sfx.PlayAt(SfxType.LeaperLeap, transform.position);
                if (phase == Phase.Leap || previous == Phase.Leap) Sfx.PlayAt(SfxType.LeaperSplash, transform.position);
            }

            if (Fireball != null && Fireball.gameObject.activeSelf != leaping)
                Fireball.gameObject.SetActive(leaping);

            SetPlaying(Flame, leaping);
            SetPlaying(WarnEffect, phase == Phase.Warn);

            // a burst when it leaves the lava and when it falls back (not on the first frame or after a reset)
            if (!force && SplashEffect != null && (phase == Phase.Leap || previous == Phase.Leap))
                SplashEffect.Play(true);
        }

        if (_phase == Phase.Leap)
        {
            // a smooth arc: 0 at the surface, Height at the top (progress 0.5), 0 again at the end
            float y = Height * 4f * progress * (1f - progress);

            if (Fireball != null)
                Fireball.localPosition = new Vector3(0f, y, 0f);

            // the flame follows the fireball
            if (Flame != null && Fireball != null)
                Flame.transform.position = Fireball.position;
        }
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

        // nothing flies on after a death
        if (Flame != null)
            Flame.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (WarnEffect != null)
            WarnEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (SplashEffect != null)
            SplashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        Apply(true);
    }

#if UNITY_EDITOR
    // The path the fireball takes, in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0.1f, 1f);
        Vector3 top = transform.position + transform.up * Height;
        Gizmos.DrawLine(transform.position, top);
        Gizmos.DrawWireSphere(top, 0.4f);
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }

    [MenuItem("GameObject/Elemento/Hazards/Lava Leaper", false, 29)]
    private static void CreateLavaLeaper(MenuCommand command)
    {
        GameObject root = new GameObject("LavaLeaper");
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(root, "Create Lava Leaper");

        LavaLeaper leaper = root.AddComponent<LavaLeaper>();

        // the fireball: a sphere picture 0.9 wide, a trigger of about 85 percent of it, a Hazard
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Fireball";
        DestroyImmediate(ball.GetComponent<Collider>());
        ball.transform.SetParent(root.transform, false);
        ball.transform.localScale = Vector3.one * 0.9f;

        MeshRenderer renderer = ball.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        Material material = ElementoMaterials.Find("M_Shape_Orange", "Elemento/NeonShape");

        if (material != null)
            renderer.sharedMaterial = material;

        SphereCollider trigger = ball.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.43f;     // 85 percent of the picture (its radius is 0.5)

        // a kinematic Rigidbody so the trigger works reliably while it moves
        Rigidbody body = ball.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        ball.AddComponent<Hazard>();
        leaper.Fireball = ball.transform;

        leaper.Flame = NewParticles("Flame", root.transform, true, true);
        leaper.WarnEffect = NewParticles("WarnEffect", root.transform, true, false);
        leaper.SplashEffect = NewParticles("SplashEffect", root.transform, false, false);

        ball.SetActive(false);
        Selection.activeGameObject = root;
    }

    // a ready-made particle system in the fire colours (the same look as the Fire Jet)
    private static ParticleSystem NewParticles(string objectName, Transform parent, bool loop, bool flame)
    {
        GameObject go = new GameObject(objectName);
        go.transform.SetParent(parent, false);

        ParticleSystem system = go.AddComponent<ParticleSystem>();
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = system.main;
        main.loop = loop;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(3f, 1.1f, 0.1f, 1f);

        ParticleSystem.EmissionModule emission = system.emission;

        if (flame)
        {
            // a trail of embers behind the fireball
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
            main.maxParticles = 60;
            emission.rateOverTime = 60f;

            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.25f;
        }
        else if (loop)
        {
            // bubbles and sparks at the surface
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
            main.maxParticles = 24;
            emission.rateOverTime = 16f;

            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 20f;
            shape.radius = 0.4f;
            go.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);   // the cone points up
        }
        else
        {
            // one burst of drops
            main.duration = 0.5f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
            main.maxParticles = 24;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 14) });

            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 35f;
            shape.radius = 0.4f;
            go.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.9f, 0.4f), 0f), new GradientColorKey(new Color(1f, 0.25f, 0.02f), 0.5f), new GradientColorKey(new Color(0.4f, 0.02f, 0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.6f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = gradient;

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = system.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

        ParticleSystemRenderer particleRenderer = go.GetComponent<ParticleSystemRenderer>();
        Material particle = ElementoMaterials.Find("P_Circle", "Elemento/NeonParticle");

        if (particle != null)
            particleRenderer.sharedMaterial = particle;

        return system;
    }
#endif
}
