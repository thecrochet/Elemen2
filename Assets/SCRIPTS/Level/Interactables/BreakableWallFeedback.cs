using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Cinemachine;

/// <summary>
/// Modular shatter and fiery explosion feedback component for breakable yellow wall blocks.
/// Provides:
///   1. 2-3 tumbling miniature yellow block chunks per cell that fly outward with angular velocity,
///      gravity, and explosive force before smoothly scaling down to 0.
///   2. Stretched golden-orange billboard sparks (~75% reduced count, shortened length, crisp non-blooming HDR)
///      that bounce along the floor.
///   3. Expanding glowing orange/gold shockwave ring (P_Ring).
///   4. Warm orange-red fireball/smoke puff burst (P_Circle) with bright gold -> fiery orange -> dark ember gradient.
///   5. Volatile electric neon-blue pinpoint sparks (P_Circle) that pop asynchronously in staggered bursts
///      across 0.05s - 0.45s with high-frequency erratic noise as the fireball dissipates.
///   6. Cinemachine 3 camera shake (Bump shape, ~0.25s duration, strong horizontal + vertical kick) with a 0.15s
///      static shared cooldown guard so rapid multi-block dashes fire one heavy, punchy kick.
///   7. Unscaled hit-stop freeze frame for visceral impact punch.
/// </summary>
[DisallowMultipleComponent]
public class BreakableWallFeedback : MonoBehaviour, IResettable
{
    [Header("Physical Non-Colliding Debris Squares")]
    [Tooltip("Reusable mini debris square prefab with DebrisKickable component. Spawns flat glowing white squares that bounce on the floor while player passes freely through them.")]
    [SerializeField] private GameObject debrisCubePrefab;

    [Tooltip("Number of debris pieces to spawn per broken block (~25% increase to 2-3).")]
    [SerializeField] private Vector2Int debrisCountRange = new Vector2Int(2, 3);

    [Tooltip("Gentle outward burst speed along dash direction (~2.5 to 5.0 m/s).")]
    [SerializeField] private float debrisBurstForce = 4.0f;

    [Tooltip("Gentle upward pop speed (~2.0 to 4.5 m/s) so cubes arc smoothly onto the floor.")]
    [SerializeField] private float debrisUpwardPopForce = 3.2f;

    [Tooltip("Explosion force applied to debris cubes via AddExplosionForce (0 disables explosion in favor of gentle pop).")]
    [SerializeField] private float debrisExplosionForce = 0f;

    [Tooltip("Radius of explosion sphere behind the wall.")]
    [SerializeField] private float debrisExplosionRadius = 2.0f;

    [Tooltip("Distance behind wall where explosion originates along dash direction.")]
    [SerializeField] private float debrisBackwardOffset = 0.5f;

    [Tooltip("Upward modifier for AddExplosionForce to give an organic arc.")]
    [SerializeField] private float debrisUpwardModifier = 0.3f;

    [Tooltip("Angular velocity / spin torque spun onto each debris cube on spawn.")]
    [SerializeField] private float debrisInitialTorque = 15f;

    [Tooltip("Scale variation range for spawned debris squares (scaled up 25% to ~0.125 to 0.20 world units).")]
    [SerializeField] private Vector2 debrisScaleRange = new Vector2(0.125f, 0.20f);

    [Tooltip("Material for the debris squares (matching the wall's glowing yellow material, M_Level_Orange).")]
    [SerializeField] private Material debrisMaterial;

    [Tooltip("Physics material for bouncy, low-friction floor sliding.")]
    [SerializeField] private PhysicsMaterial debrisPhysicsMaterial;

    [Header("Sparks Burst (Crisp Golden-Orange)")]
    [Tooltip("Particle system emitting stretched billboard sparks that bounce along floor.")]
    [SerializeField] private ParticleSystem sparkParticles;

    [Tooltip("Material for sparks (defaults to P_Streak).")]
    [SerializeField] private Material sparkMaterial;

    [Tooltip("Number of spark particles to emit (reduced by ~75% for readable, non-bloomed visuals).")]
    [SerializeField] private int sparkBurstCount = 8;

    [Tooltip("Stretched billboard length scale (shortened so sparks are crisp needles).")]
    [SerializeField] private float sparkLengthScale = 0.8f;

    [Tooltip("Stretched billboard velocity scale.")]
    [SerializeField] private float sparkVelocityScale = 0.08f;

    [Tooltip("Speed range for spark particles.")]
    [SerializeField] private Vector2 sparkSpeedRange = new Vector2(8f, 18f);

    [Tooltip("Lifetime range in seconds for spark particles.")]
    [SerializeField] private Vector2 sparkLifetimeRange = new Vector2(0.4f, 0.8f);

    [Tooltip("HDR golden-orange start color (min) - crisp and avoids solid white blowout.")]
    [SerializeField] [ColorUsage(true, true)] private Color sparkHdrColorMin = new Color(1.8f, 0.9f, 0.1f, 1f);

    [Tooltip("HDR golden-orange start color (max).")]
    [SerializeField] [ColorUsage(true, true)] private Color sparkHdrColorMax = new Color(2.0f, 0.6f, 0.05f, 1f);

    [Tooltip("Floor bounce coefficient for sparks (0 = no bounce, 1 = full bounce).")]
    [SerializeField] [Range(0f, 1f)] private float sparkFloorBounce = 0.65f;

    [Tooltip("Floor dampen coefficient when sparks bounce.")]
    [SerializeField] [Range(0f, 1f)] private float sparkFloorDampen = 0.25f;

    [Tooltip("Gravity multiplier for spark particles.")]
    [SerializeField] private float sparkGravity = 2.5f;

    [Tooltip("Layers that sparks bounce off (e.g. Default / Floor).")]
    [SerializeField] private LayerMask sparkCollisionLayers = 1; // Layer 0: Default

    [Header("Shockwave Ring")]
    [Tooltip("Particle system emitting expanding glowing ring shockwave.")]
    [SerializeField] private ParticleSystem shockwaveParticles;

    [Tooltip("Material for shockwave ring (defaults to P_Ring).")]
    [SerializeField] private Material shockwaveMaterial;

    [Tooltip("Number of shockwaves emitted per explosion.")]
    [SerializeField] private int shockwaveBurstCount = 1;

    [Tooltip("Lifetime of expanding shockwave ring.")]
    [SerializeField] private float shockwaveLifetime = 0.22f;

    [Tooltip("Initial start size of shockwave ring.")]
    [SerializeField] private float shockwaveStartSize = 0.35f;

    [Tooltip("Maximum expanded size of shockwave ring.")]
    [SerializeField] private float shockwaveEndSize = 3.2f;

    [Tooltip("HDR glowing gold/orange tint for shockwave ring.")]
    [SerializeField] [ColorUsage(true, true)] private Color shockwaveColor = new Color(2.4f, 1.3f, 0.2f, 1.0f);

    [Header("Fireball / Smoke Puff Burst")]
    [Tooltip("Particle system emitting warm orange-red fireball and smoke puffs.")]
    [SerializeField] private ParticleSystem fireballParticles;

    [Tooltip("Material for fireball puffs (defaults to P_Circle).")]
    [SerializeField] private Material fireballMaterial;

    [Tooltip("Number of fireball smoke puff particles emitted.")]
    [SerializeField] private int fireballBurstCount = 6;

    [Tooltip("Speed range for fireball smoke puffs.")]
    [SerializeField] private Vector2 fireballSpeedRange = new Vector2(2f, 5.5f);

    [Tooltip("Lifetime range for fireball smoke puffs.")]
    [SerializeField] private Vector2 fireballLifetimeRange = new Vector2(0.28f, 0.42f);

    [Tooltip("Initial size range for fireball puffs.")]
    [SerializeField] private Vector2 fireballStartSizeRange = new Vector2(0.45f, 0.8f);

    [Tooltip("Expansion multiplier over lifetime.")]
    [SerializeField] private float fireballEndSizeMultiplier = 2.0f;

    [Tooltip("Drag / deceleration applied to fireball puffs.")]
    [SerializeField] private float fireballDrag = 0.45f;

    [Tooltip("Color over lifetime: bright gold -> fiery orange -> dark fading ember.")]
    [SerializeField] private Gradient fireballGradient;

    [Header("Lingering Spirit Wisps (Ori-Inspired Cyan/Blue)")]
    [Tooltip("Particle system emitting soft-glowing, lingering neon-blue/cyan spirit motes.")]
    [SerializeField] private ParticleSystem blueSparkParticles;

    [Tooltip("Material for spirit wisps (soft radial glow circle, defaults to P_Circle).")]
    [SerializeField] private Material blueSparkMaterial;

    [Tooltip("Number of staggered burst events across spawn delay.")]
    [SerializeField] private int blueSparkBurstCount = 4;

    [Tooltip("Motes emitted per staggered burst (min/max).")]
    [SerializeField] private Vector2Int blueSparkCountPerBurst = new Vector2Int(1, 3);

    [Tooltip("Probability of each staggered burst triggering (< 1.0 for organic asynchronous popping).")]
    [SerializeField] [Range(0f, 1f)] private float blueSparkBurstProbability = 0.85f;

    [Tooltip("Slight delayed spawn timing range (seconds) so wisps appear as fireball clears.")]
    [SerializeField] private Vector2 blueSparkSpawnDelayRange = new Vector2(0.10f, 0.35f);

    [Tooltip("Long lingering lifetime range in seconds (~1.6s to 3.2s).")]
    [SerializeField] private Vector2 blueSparkLifetimeRange = new Vector2(1.6f, 3.2f);

    [Tooltip("Varied particle size range using Random Between Two Constants (mix of small pinpoint sparks and medium soft-halo orbs, ~3x-5x larger).")]
    [SerializeField] private Vector2 blueSparkSizeRange = new Vector2(0.10f, 0.35f);

    [Tooltip("Initial gentle outward burst speed before drag settles particles into a hover.")]
    [SerializeField] private Vector2 blueSparkSpeedRange = new Vector2(1.5f, 3.5f);

    [Tooltip("Vivid electric cyan-blue HDR color min (#00E5FF base with zero red so it stays vivid saturated blue).")]
    [SerializeField] [ColorUsage(true, true)] private Color blueSparkHdrColorMin = new Color(0.0f, 1.796f, 2.0f, 1.0f);

    [Tooltip("Vivid electric cyan-blue HDR color max (#2979FF base with low red).")]
    [SerializeField] [ColorUsage(true, true)] private Color blueSparkHdrColorMax = new Color(0.354f, 1.045f, 2.2f, 1.0f);

    [Tooltip("Velocity damping/drag for weightless hovering motion.")]
    [SerializeField] private float blueSparkDrag = 0.85f;

    [Tooltip("Slight upward drift force so motes gently float upwards.")]
    [SerializeField] private float blueSparkUpwardDrift = 0.3f;

    [Tooltip("Subtle negative gravity for weightless floating floatiness.")]
    [SerializeField] private float blueSparkGravity = -0.05f;

    [Tooltip("Organic 3D turbulence noise strength.")]
    [SerializeField] private float blueSparkNoiseStrength = 1.2f;

    [Tooltip("Smooth turbulence frequency (lower for smooth swirling currents).")]
    [SerializeField] private float blueSparkNoiseFrequency = 0.8f;

    [Tooltip("Smooth turbulence scroll speed.")]
    [SerializeField] private float blueSparkNoiseScrollSpeed = 0.6f;

    [Tooltip("Spawn sphere radius around the block center.")]
    [SerializeField] private float blueSparkRadius = 0.4f;

    [Tooltip("Multi-peak pulsing size curve for asynchronous shimmering/twinkling.")]
    [SerializeField] private AnimationCurve blueSparkTwinkleCurve;

    [Tooltip("Color over lifetime gradient for twinkling/flickering alpha shimmer.")]
    [SerializeField] private Gradient blueSparkTwinkleGradient;

    [Header("Camera Shake (Cinemachine 3)")]
    [Tooltip("Cinemachine 3 Impulse Source for camera shake. If null, tries GetComponent.")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Tooltip("Strong horizontal shake kick force.")]
    [SerializeField] private float shakeForceHorizontal = 1.2f;

    [Tooltip("Strong vertical shake kick force.")]
    [SerializeField] private float shakeForceVertical = 0.8f;

    [Tooltip("Impulse shake duration.")]
    [SerializeField] private float shakeDuration = 0.25f;

    [Tooltip("Static shared cooldown guard so breaking 10+ blocks in one dash fires one clean, heavy shake.")]
    [SerializeField] private float cameraShakeCooldown = 0.15f;

    [Header("Hit-Stop Freeze Frame")]
    [Tooltip("Duration of freeze frame in unscaled realtime seconds (~0.015s - 0.025s for snappy punch).")]
    [SerializeField] private float hitStopDuration = 0.02f;

    [Tooltip("Time scale during hit-stop (0 = complete freeze frame).")]
    [SerializeField] [Range(0f, 1f)] private float hitStopTimeScale = 0f;

    [Tooltip("Shared cooldown guard for hit-stop.")]
    [SerializeField] private float hitStopCooldown = 0.15f;

    // Static timers shared across all blocks to prevent impulse / freeze-frame stacking on multi-block dashes
    private static float _lastCameraShakeTime = -999f;
    private static float _lastHitStopTime = -999f;
    private static Mesh _cachedCubeMesh;

    private DestructibleObject _destructible;
    private BoxCollider _boxCollider;
    private Renderer _wallRenderer;
    private Coroutine _hitStopCoroutine;
    private readonly List<GameObject> _activeDebris = new List<GameObject>(16);

    private void Awake()
    {
        _destructible = GetComponent<DestructibleObject>();
        _boxCollider = GetComponent<BoxCollider>();
        _wallRenderer = GetComponentInChildren<MeshRenderer>();

        if (impulseSource == null)
            impulseSource = GetComponent<CinemachineImpulseSource>();

        if (_destructible != null)
        {
            _destructible.OnBreak += HandleBreak;
            _destructible.OnReset += HandleReset;
        }

        EnsureDefaultFireballGradient();
        EnsureDefaultTwinkleCurves();
        EnsureVfxHierarchies();
        ConfigureBlueSparksEmission();
    }

    private void Start()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (_destructible != null)
        {
            _destructible.OnBreak -= HandleBreak;
            _destructible.OnReset -= HandleReset;
        }

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);

        CleanUpActiveDebris();
        RestoreTimeScaleIfFrozen();
    }

    private void OnDisable()
    {
        RestoreTimeScaleIfFrozen();
    }

    private void HandleBreak(HitSource source, Vector3 direction)
    {
        PlayFeedback(direction);
    }

    private void HandleReset()
    {
        ResetObject();
    }

    /// <summary>
    /// Executes the full fiery shatter feedback suite including volatile blue sparks.
    /// </summary>
    /// <param name="dashDirection">Direction the player dashed into the wall.</param>
    public void PlayFeedback(Vector3 dashDirection)
    {
        Vector3 dir = dashDirection.sqrMagnitude > 0.001f ? dashDirection.normalized : Vector3.right;

        SpawnPhysicalDebris(dir);
        EmitParticles(dir);
        TriggerHitStop();
        TriggerCameraShake(dir);
    }

    private void SpawnPhysicalDebris(Vector3 dashDir)
    {
        Vector3 colCenter = _boxCollider != null ? _boxCollider.center : Vector3.zero;
        Vector3 wallWorldCenter = transform.TransformPoint(colCenter);
        Vector3 explosionOrigin = wallWorldCenter - dashDir * debrisBackwardOffset;

        int count = Random.Range(debrisCountRange.x, debrisCountRange.y + 1);

        Collider[] playerColliders = null;
        var player = FindFirstObjectByType<CC_ELEMENTO>();
        if (player != null)
        {
            playerColliders = player.GetComponentsInChildren<Collider>(true);
        }

        int debrisLayer = LayerMask.NameToLayer("Debris");

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.25f, 0.25f),
                Random.Range(-0.25f, 0.25f),
                0f
            );

            Vector3 spawnPos = wallWorldCenter + offset;
            Quaternion spawnRot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            GameObject debrisObj = null;
            if (debrisCubePrefab != null)
            {
                debrisObj = Instantiate(debrisCubePrefab, spawnPos, spawnRot);
            }
            else
            {
                debrisObj = CreateProceduralDebris(spawnPos, spawnRot);
            }

            if (debrisObj != null)
            {
                // Ensure debris root is on dedicated Debris layer while preserving child trigger layer on Interactable
                if (debrisLayer != -1)
                {
                    debrisObj.layer = debrisLayer;
                    int interactableLayer = LayerMask.NameToLayer("Interactable");
                    foreach (Transform child in debrisObj.transform)
                    {
                        var col = child.GetComponent<Collider>();
                        if (col != null && col.isTrigger)
                        {
                            if (interactableLayer != -1)
                                child.gameObject.layer = interactableLayer;
                        }
                        else
                        {
                            child.gameObject.layer = debrisLayer;
                        }
                    }
                }

                // Immediately ignore solid collision with all player colliders on spawn frame
                if (playerColliders != null)
                {
                    Collider[] debrisCols = debrisObj.GetComponentsInChildren<Collider>(true);
                    for (int d = 0; d < debrisCols.Length; d++)
                    {
                        if (debrisCols[d] == null || debrisCols[d].isTrigger) continue;
                        for (int p = 0; p < playerColliders.Length; p++)
                        {
                            if (playerColliders[p] == null) continue;
                            Physics.IgnoreCollision(debrisCols[d], playerColliders[p], true);
                        }
                    }
                }

                float randomScale = Random.Range(debrisScaleRange.x, debrisScaleRange.y);
                // Flat glowing square: confetti/tile size (~0.10 to 0.16) in X/Y, flattened in Z
                debrisObj.transform.localScale = new Vector3(randomScale, randomScale, 0.02f);

                Rigidbody rb = debrisObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Constrain Rigidbody to 2D plane: freeze Z position and freeze X/Y rotation so square spins cleanly in 2D
                    rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

                    // Gentle outward pop along dash direction + upward arc (velocity ~3-6 m/s)
                    float burstX = dashDir.x * (debrisBurstForce * Random.Range(0.85f, 1.25f)) + Random.Range(-0.35f, 0.35f);
                    float popY = debrisUpwardPopForce * Random.Range(0.85f, 1.25f);
                    Vector3 launchVelocity = new Vector3(burstX, popY, 0f);

                    rb.linearVelocity = launchVelocity;

                    if (debrisExplosionForce > 0.01f)
                    {
                        rb.AddExplosionForce(debrisExplosionForce, explosionOrigin, debrisExplosionRadius, debrisUpwardModifier, ForceMode.Impulse);
                    }

                    // Spin cleanly on the Z axis like 2D squares without edge-disappearing in 3D
                    rb.angularVelocity = new Vector3(
                        0f,
                        0f,
                        Random.Range(-debrisInitialTorque, debrisInitialTorque)
                    );
                }

                _activeDebris.Add(debrisObj);
            }
        }
    }

    private GameObject CreateProceduralDebris(Vector3 position, Quaternion rotation)
    {
        // Flat quad facing camera
        GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debris.name = "MiniDebrisSquare";
        debris.transform.position = position;
        debris.transform.rotation = rotation;

        int debrisLayer = LayerMask.NameToLayer("Debris");
        if (debrisLayer != -1)
            debris.layer = debrisLayer;

        // Clean up default MeshCollider from PrimitiveType.Quad and use BoxCollider
        MeshCollider mc = debris.GetComponent<MeshCollider>();
        if (mc != null) Destroy(mc);

        Material resolvedMat = debrisMaterial;
        if (resolvedMat == null && _wallRenderer != null)
            resolvedMat = _wallRenderer.sharedMaterial;
        if (resolvedMat == null)
            resolvedMat = Resources.Load<Material>("M_Level_Orange");

        MeshRenderer mr = debris.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sharedMaterial = resolvedMat;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }

        BoxCollider bc = debris.GetComponent<BoxCollider>();
        if (bc == null) bc = debris.AddComponent<BoxCollider>();
        bc.size = new Vector3(1f, 1f, 25f);
        if (debrisPhysicsMaterial != null)
        {
            bc.sharedMaterial = debrisPhysicsMaterial;
        }

        Rigidbody rb = debris.GetComponent<Rigidbody>();
        if (rb == null) rb = debris.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.linearDamping = 1.5f;
        rb.angularDamping = 1.2f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        if (!debris.TryGetComponent<DebrisKickable>(out _))
        {
            debris.AddComponent<DebrisKickable>();
        }

        return debris;
    }

    private void EmitParticles(Vector3 dashDir)
    {
        Vector3 center = _boxCollider != null ? _boxCollider.center : Vector3.zero;
        Vector3 worldCenter = transform.TransformPoint(center);
        Quaternion dashRot = Quaternion.LookRotation(new Vector3(dashDir.x, 0.25f, 0f));

        // 1. Crisp Sparks
        if (sparkParticles != null)
        {
            sparkParticles.transform.position = worldCenter;
            sparkParticles.transform.rotation = dashRot;
            sparkParticles.Emit(sparkBurstCount);
        }

        // 2. Shockwave Ring
        if (shockwaveParticles != null)
        {
            shockwaveParticles.transform.position = worldCenter;
            shockwaveParticles.transform.rotation = Quaternion.identity;
            shockwaveParticles.Emit(shockwaveBurstCount);
        }

        // 3. Fireball Smoke Puffs
        if (fireballParticles != null)
        {
            fireballParticles.transform.position = worldCenter;
            fireballParticles.transform.rotation = dashRot;
            fireballParticles.Emit(fireballBurstCount);
        }

        // 4. Volatile Blue Sparks (staggered pops scheduled across 0.05s - 0.45s)
        if (blueSparkParticles != null)
        {
            blueSparkParticles.transform.position = worldCenter;
            blueSparkParticles.transform.rotation = Quaternion.identity;
            blueSparkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            blueSparkParticles.Play();
        }
    }

    private void TriggerCameraShake(Vector3 dashDir)
    {
        // Static shared cooldown guard so multi-block dashes fire one heavy, clean impulse
        if (Time.unscaledTime - _lastCameraShakeTime < cameraShakeCooldown) return;
        _lastCameraShakeTime = Time.unscaledTime;

        float dirSign = dashDir.x >= 0f ? 1f : -1f;
        Vector3 impulseVelocity = new Vector3(dirSign * shakeForceHorizontal, shakeForceVertical, 0f);

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithVelocity(impulseVelocity);
        }
        else
        {
            CinemachineImpulseSource sceneSource = FindFirstObjectByType<CinemachineImpulseSource>();
            if (sceneSource != null)
            {
                sceneSource.GenerateImpulseWithVelocity(impulseVelocity);
            }
        }
    }

    private void TriggerHitStop()
    {
        if (hitStopDuration <= 0f) return;
        if (Time.unscaledTime - _lastHitStopTime < hitStopCooldown) return;
        if (Time.timeScale <= 0.001f) return;

        _lastHitStopTime = Time.unscaledTime;

        if (_hitStopCoroutine != null)
            StopCoroutine(_hitStopCoroutine);

        _hitStopCoroutine = StartCoroutine(HitStopRoutine());
    }

    private IEnumerator HitStopRoutine()
    {
        float previousScale = Time.timeScale;
        Time.timeScale = hitStopTimeScale;

        yield return new WaitForSecondsRealtime(hitStopDuration);

        if (Mathf.Approximately(Time.timeScale, hitStopTimeScale))
        {
            Time.timeScale = previousScale;
        }

        _hitStopCoroutine = null;
    }

    private void EnsureDefaultFireballGradient()
    {
        if (fireballGradient != null && fireballGradient.colorKeys != null && fireballGradient.colorKeys.Length > 0)
            return;

        fireballGradient = new Gradient();
        fireballGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(2.4f, 1.6f, 0.25f), 0.0f),
                new GradientColorKey(new Color(2.0f, 0.55f, 0.05f), 0.35f),
                new GradientColorKey(new Color(1.2f, 0.15f, 0.02f), 0.70f),
                new GradientColorKey(new Color(0.2f, 0.02f, 0.0f), 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.85f, 0.6f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
    }

    private void EnsureDefaultTwinkleCurves()
    {
        if (blueSparkTwinkleCurve == null || blueSparkTwinkleCurve.length == 0)
        {
            blueSparkTwinkleCurve = new AnimationCurve(
                new Keyframe(0f, 0f, 4f, 4f),
                new Keyframe(0.08f, 1f, 0.5f, -0.5f),
                new Keyframe(0.25f, 0.65f, -0.3f, 0.3f),
                new Keyframe(0.42f, 1.15f, 0.4f, -0.4f),
                new Keyframe(0.60f, 0.55f, -0.3f, 0.3f),
                new Keyframe(0.78f, 1.0f, 0.3f, -0.3f),
                new Keyframe(0.90f, 0.45f, -0.5f, -0.5f),
                new Keyframe(1f, 0f, -4f, -4f)
            );
        }

        if (blueSparkTwinkleGradient == null || blueSparkTwinkleGradient.colorKeys == null || blueSparkTwinkleGradient.colorKeys.Length == 0)
        {
            blueSparkTwinkleGradient = new Gradient();
            blueSparkTwinkleGradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(new Color(0.0f, 0.9f, 1.0f), 0.0f),
                    new GradientColorKey(new Color(0.16f, 0.47f, 1.0f), 0.5f),
                    new GradientColorKey(new Color(0.0f, 0.85f, 1.0f), 1.0f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 0.08f),
                    new GradientAlphaKey(0.65f, 0.25f),
                    new GradientAlphaKey(1.0f, 0.42f),
                    new GradientAlphaKey(0.55f, 0.60f),
                    new GradientAlphaKey(0.95f, 0.78f),
                    new GradientAlphaKey(0.50f, 0.90f),
                    new GradientAlphaKey(0.0f, 1.0f)
                }
            );
        }
    }

    public void ConfigureBlueSparksEmission()
    {
        if (blueSparkParticles == null) return;

        var emission = blueSparkParticles.emission;
        emission.rateOverTime = 0;

        int burstsCount = Mathf.Max(1, blueSparkBurstCount);
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[burstsCount];

        float minT = blueSparkSpawnDelayRange.x;
        float maxT = blueSparkSpawnDelayRange.y;
        float dt = (maxT - minT) / Mathf.Max(1, burstsCount);

        for (int i = 0; i < burstsCount; i++)
        {
            float t = minT + (i + 0.5f) * dt;
            short minCount = (short)Mathf.Max(1, blueSparkCountPerBurst.x);
            short maxCount = (short)Mathf.Max(minCount, blueSparkCountPerBurst.y);

            bursts[i] = new ParticleSystem.Burst(t, minCount, maxCount, 1, 0f)
            {
                probability = blueSparkBurstProbability
            };
        }

        emission.SetBursts(bursts);
    }

    private void EnsureVfxHierarchies()
    {
        Transform explosionRoot = transform.Find("FX_Explosion");
        if (explosionRoot == null)
        {
            GameObject expObj = new GameObject("FX_Explosion");
            expObj.transform.SetParent(transform, false);
            expObj.transform.localPosition = _boxCollider != null ? _boxCollider.center : Vector3.zero;
            explosionRoot = expObj.transform;
        }

        // 1. Sparks
        if (sparkParticles == null)
        {
            Transform t = explosionRoot.Find("FX_Sparks") ?? transform.Find("FX_BreakSparks");
            if (t != null && t.TryGetComponent(out ParticleSystem ps))
                sparkParticles = ps;
            else
                sparkParticles = CreateSparkParticleSystem(explosionRoot);
        }

        // 2. Shockwave
        if (shockwaveParticles == null)
        {
            Transform t = explosionRoot.Find("FX_Shockwave");
            if (t != null && t.TryGetComponent(out ParticleSystem ps))
                shockwaveParticles = ps;
            else
                shockwaveParticles = CreateShockwaveParticleSystem(explosionRoot);
        }

        // 3. Fireball
        if (fireballParticles == null)
        {
            Transform t = explosionRoot.Find("FX_Fireball");
            if (t != null && t.TryGetComponent(out ParticleSystem ps))
                fireballParticles = ps;
            else
                fireballParticles = CreateFireballParticleSystem(explosionRoot);
        }

        // 4. Volatile Blue Sparks
        if (blueSparkParticles == null)
        {
            Transform t = explosionRoot.Find("FX_VolatileBlueSparks");
            if (t != null && t.TryGetComponent(out ParticleSystem ps))
                blueSparkParticles = ps;
            else
                blueSparkParticles = CreateBlueSparkParticleSystem(explosionRoot);
        }
    }

    private ParticleSystem CreateSparkParticleSystem(Transform parent)
    {
        GameObject obj = new GameObject("FX_Sparks");
        obj.transform.SetParent(parent, false);

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();

        psr.renderMode = ParticleSystemRenderMode.Stretch;
        psr.cameraVelocityScale = 0f;
        psr.velocityScale = sparkVelocityScale;
        psr.lengthScale = sparkLengthScale;

        if (sparkMaterial != null)
            psr.sharedMaterial = sparkMaterial;
        else
            psr.sharedMaterial = Resources.Load<Material>("P_Streak");

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = sparkGravity;
        main.startSpeed = new ParticleSystem.MinMaxCurve(sparkSpeedRange.x, sparkSpeedRange.y);
        main.startLifetime = new ParticleSystem.MinMaxCurve(sparkLifetimeRange.x, sparkLifetimeRange.y);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.14f);
        main.startColor = new ParticleSystem.MinMaxGradient(sparkHdrColorMin, sparkHdrColorMax);

        var emission = ps.emission;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.15f;

        var col = ps.collision;
        col.enabled = true;
        col.type = ParticleSystemCollisionType.World;
        col.mode = ParticleSystemCollisionMode.Collision3D;
        col.collidesWith = sparkCollisionLayers;
        col.bounce = sparkFloorBounce;
        col.dampen = sparkFloorDampen;
        col.lifetimeLoss = 0.05f;
        col.radiusScale = 0.1f;

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

        return ps;
    }

    private ParticleSystem CreateShockwaveParticleSystem(Transform parent)
    {
        GameObject obj = new GameObject("FX_Shockwave");
        obj.transform.SetParent(parent, false);

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();

        psr.renderMode = ParticleSystemRenderMode.Billboard;
        if (shockwaveMaterial != null)
            psr.sharedMaterial = shockwaveMaterial;
        else
            psr.sharedMaterial = Resources.Load<Material>("P_Ring");

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = 0f;
        main.startLifetime = shockwaveLifetime;
        main.startSize = shockwaveStartSize;
        main.startColor = shockwaveColor;

        var emission = ps.emission;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.enabled = false;

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve expandCurve = new AnimationCurve(
            new Keyframe(0f, 1f, 2.5f, 2.5f),
            new Keyframe(1f, shockwaveEndSize / Mathf.Max(0.01f, shockwaveStartSize), 0.2f, 0f)
        );
        sol.size = new ParticleSystem.MinMaxCurve(1f, expandCurve);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient fadeGrad = new Gradient();
        fadeGrad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.4f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(fadeGrad);

        return ps;
    }

    private ParticleSystem CreateFireballParticleSystem(Transform parent)
    {
        GameObject obj = new GameObject("FX_Fireball");
        obj.transform.SetParent(parent, false);

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();

        psr.renderMode = ParticleSystemRenderMode.Billboard;
        if (fireballMaterial != null)
            psr.sharedMaterial = fireballMaterial;
        else
            psr.sharedMaterial = Resources.Load<Material>("P_Circle");

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = new ParticleSystem.MinMaxCurve(fireballSpeedRange.x, fireballSpeedRange.y);
        main.startLifetime = new ParticleSystem.MinMaxCurve(fireballLifetimeRange.x, fireballLifetimeRange.y);
        main.startSize = new ParticleSystem.MinMaxCurve(fireballStartSizeRange.x, fireballStartSizeRange.y);

        var emission = ps.emission;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, fireballEndSizeMultiplier));

        var limitVelocity = ps.limitVelocityOverLifetime;
        limitVelocity.enabled = true;
        limitVelocity.drag = fireballDrag;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = new ParticleSystem.MinMaxGradient(fireballGradient);

        return ps;
    }

    private ParticleSystem CreateBlueSparkParticleSystem(Transform parent)
    {
        GameObject obj = new GameObject("FX_VolatileBlueSparks");
        obj.transform.SetParent(parent, false);

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();

        psr.renderMode = ParticleSystemRenderMode.Billboard;
        if (blueSparkMaterial != null)
            psr.sharedMaterial = blueSparkMaterial;
        else
            psr.sharedMaterial = Resources.Load<Material>("P_Circle");

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = Mathf.Max(0.5f, blueSparkSpawnDelayRange.y + 0.15f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = new ParticleSystem.MinMaxCurve(blueSparkSpeedRange.x, blueSparkSpeedRange.y);
        main.startLifetime = new ParticleSystem.MinMaxCurve(blueSparkLifetimeRange.x, blueSparkLifetimeRange.y);
        main.startSize = new ParticleSystem.MinMaxCurve(blueSparkSizeRange.x, blueSparkSizeRange.y);
        main.startColor = new ParticleSystem.MinMaxGradient(blueSparkHdrColorMin, blueSparkHdrColorMax);
        main.gravityModifier = blueSparkGravity;

        // Weightless floating hover: drag / velocity limit
        var limitVelocity = ps.limitVelocityOverLifetime;
        limitVelocity.enabled = true;
        limitVelocity.drag = blueSparkDrag;

        // Gentle upward drift force
        var force = ps.forceOverLifetime;
        force.enabled = true;
        force.space = ParticleSystemSimulationSpace.World;
        force.y = new ParticleSystem.MinMaxCurve(blueSparkUpwardDrift * 0.5f, blueSparkUpwardDrift * 1.5f);

        // Smooth 3D Noise turbulence
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = blueSparkNoiseStrength;
        noise.frequency = blueSparkNoiseFrequency;
        noise.scrollSpeed = blueSparkNoiseScrollSpeed;
        noise.damping = true;
        noise.octaveCount = 2;
        noise.quality = ParticleSystemNoiseQuality.High;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = blueSparkRadius;

        // Multi-peak pulse curves for twinkling / shimmering
        EnsureDefaultTwinkleCurves();

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, blueSparkTwinkleCurve);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = new ParticleSystem.MinMaxGradient(blueSparkTwinkleGradient);

        return ps;
    }

    private static Mesh GetOrCreateCubeMesh()
    {
        if (_cachedCubeMesh == null)
        {
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cachedCubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tempCube);
        }
        return _cachedCubeMesh;
    }

    private void CleanUpActiveDebris()
    {
        for (int i = 0; i < _activeDebris.Count; i++)
        {
            if (_activeDebris[i] != null)
                Destroy(_activeDebris[i]);
        }
        _activeDebris.Clear();
    }

    private void RestoreTimeScaleIfFrozen()
    {
        if (_hitStopCoroutine != null)
        {
            StopCoroutine(_hitStopCoroutine);
            _hitStopCoroutine = null;
            if (Mathf.Approximately(Time.timeScale, hitStopTimeScale) && hitStopTimeScale <= 0.001f)
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void CommitState()
    {
        // Ephemeral VFX
    }

    public void ResetObject()
    {
        CleanUpActiveDebris();
        RestoreTimeScaleIfFrozen();

        if (sparkParticles != null)
            sparkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (shockwaveParticles != null)
            shockwaveParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (fireballParticles != null)
            fireballParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (blueSparkParticles != null)
            blueSparkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
