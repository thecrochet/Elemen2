using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Plays a one-shot isometric-to-2.5D cold open at the start of the tutorial level.
//
// What it does:
//   1. At scene start it sets LevelManager.IsIntroPending = true, which holds the level clock.
//   2. After holdStartDelay seconds it morphs the cube (isometric Euler to flat 2.5D),
//      fades the player visuals in, and blends the intro camera to the 2.5D play camera.
//   3. When done it clears IsIntroPending and re-enables the Gameplay action map so the
//      player can move. The sequence never replays on a RespawnManager respawn.
//
// Setup:
//   - Attach to any GameObject in the tutorial scene (e.g. a TutorialIntro object).
//   - Fill in cubeTransform, playerVisualsRoot, and optionally introCamera in the Inspector.
//   - LevelManager.IsTutorialLevel must be checked on the LevelManager of the same scene.
//
// Inspector links outside a prefab are lost when placed in another scene – see the report.
public class TutorialIntroSequence : MonoBehaviour
{
    // ------------------------------------------------------------------
    // INSPECTOR
    // ------------------------------------------------------------------
    [Header("Timing")]
    [Tooltip("Seconds to hold on the isometric view before the morph begins.")]
    [Min(0f)] public float holdStartDelay = 0.4f;

    [Tooltip("Total duration of the morph (cube flattening + fade-in).")]
    [Min(0.1f)] public float introDuration = 1.8f;

    [Header("References")]
    [Tooltip("The Unity cube that morphs from isometric to flat. Required.")]
    public Transform cubeTransform;

    [Tooltip("Root of the player's visual objects. Its renderers will be faded in. Required.")]
    public Transform playerVisualsRoot;

    [Tooltip("Optional Cinemachine camera shown during the hold phase. When set it blends to " +
             "the player's camera at the start of the morph.")]
    public CinemachineCamera introCamera;

    // ------------------------------------------------------------------
    // PRIVATE
    // ------------------------------------------------------------------
    // Isometric start and 2.5D end euler angles (from the task spec).
    private static readonly Vector3 _startEuler = new Vector3(35.264f, 45f, 0f);
    private static readonly Vector3 _endEuler   = new Vector3(0f,      0f,  0f);

    // Cached renderers under playerVisualsRoot (set once, zero allocations per frame).
    private Renderer[] _playerRenderers;
    private RendererFadeData[] _fadeData;
    private bool _hasStarted;   // true once StartIntro runs (so respawns skip it)

    // MaterialPropertyBlock lets us set alpha per-renderer with no material instances.
    private MaterialPropertyBlock _mpb;

    // Standard shader property IDs
    private static readonly int _baseColorId   = Shader.PropertyToID("_BaseColor");
    private static readonly int _colorPropId   = Shader.PropertyToID("_Color");

    // Elemento Ball shader property IDs (M_Ball_Aether and element variants)
    private static readonly int _tintColorId   = Shader.PropertyToID("_TintColor");
    private static readonly int _glassAlphaId  = Shader.PropertyToID("_GlassAlpha");
    private static readonly int _rimColorId    = Shader.PropertyToID("_RimColor");

    // Ball_Aether specific
    private static readonly int _starColorId   = Shader.PropertyToID("_StarColor");
    private static readonly int _lineColorId   = Shader.PropertyToID("_LineColor");
    private static readonly int _nebulaColorId = Shader.PropertyToID("_NebulaColor");

    // Other ball element variants
    private static readonly int _swirlColorId  = Shader.PropertyToID("_SwirlColor");
    private static readonly int _coreColorId   = Shader.PropertyToID("_CoreColor");
    private static readonly int _boltColorId   = Shader.PropertyToID("_BoltColor");
    private static readonly int _ringColorId   = Shader.PropertyToID("_RingColor");
    private static readonly int _crackGlowId   = Shader.PropertyToID("_CrackGlow");
    private static readonly int _rockLightId   = Shader.PropertyToID("_RockLight");
    private static readonly int _rockDarkId    = Shader.PropertyToID("_RockDark");
    private static readonly int _hotColorId    = Shader.PropertyToID("_HotColor");
    private static readonly int _lavaColorId   = Shader.PropertyToID("_LavaColor");
    private static readonly int _facetLightId  = Shader.PropertyToID("_FacetLight");
    private static readonly int _facetDarkId   = Shader.PropertyToID("_FacetDark");
    private static readonly int _edgeColorId   = Shader.PropertyToID("_EdgeColor");

    // Cache structure to hold property presence and original values per renderer
    private struct RendererFadeData
    {
        public Renderer renderer;
        public Material lastSharedMaterial;

        // Standard properties
        public bool hasBaseColor;
        public Color originalBaseColor;
        public bool hasColor;
        public Color originalColor;

        // Ball shader common properties (M_Ball_Aether and others)
        public bool hasTintColor;
        public Color originalTintColor;
        public bool hasGlassAlpha;
        public float originalGlassAlpha;
        public bool hasRimColor;
        public Color originalRimColor;

        // Ball_Aether specific
        public bool hasStarColor;
        public Color originalStarColor;
        public bool hasLineColor;
        public Color originalLineColor;
        public bool hasNebulaColor;
        public Color originalNebulaColor;

        // Other ball element variants
        public bool hasSwirlColor;
        public Color originalSwirlColor;
        public bool hasCoreColor;
        public Color originalCoreColor;
        public bool hasBoltColor;
        public Color originalBoltColor;
        public bool hasRingColor;
        public Color originalRingColor;
        public bool hasCrackGlow;
        public Color originalCrackGlow;
        public bool hasRockLight;
        public Color originalRockLight;
        public bool hasRockDark;
        public Color originalRockDark;
        public bool hasHotColor;
        public Color originalHotColor;
        public bool hasLavaColor;
        public Color originalLavaColor;
        public bool hasFacetLight;
        public Color originalFacetLight;
        public bool hasFacetDark;
        public Color originalFacetDark;
        public bool hasEdgeColor;
        public Color originalEdgeColor;
    }

    // We hold references to the action map we disable during the intro.
    private InputActionMap _gameplayMap;

    // ------------------------------------------------------------------
    // UNITY
    // ------------------------------------------------------------------
    private void Awake()
    {
        // Validation: log clear warnings if required slots are empty.
        if (cubeTransform == null)
            Debug.LogWarning("[TutorialIntroSequence] cubeTransform is not set.", this);

        if (playerVisualsRoot == null)
            Debug.LogWarning("[TutorialIntroSequence] playerVisualsRoot is not set.", this);
    }

    private void Start()
    {
        // Block the level clock immediately (before the player can touch anything).
        if (LevelManager.Instance != null)
            LevelManager.Instance.IsIntroPending = true;

        // Cache renderers once (no per-frame allocations).
        if (playerVisualsRoot != null)
        {
            _playerRenderers = playerVisualsRoot.GetComponentsInChildren<Renderer>(true);
            _fadeData = new RendererFadeData[_playerRenderers.Length];
            for (int i = 0; i < _playerRenderers.Length; i++)
            {
                _fadeData[i] = BuildFadeData(_playerRenderers[i]);
            }
            _mpb = new MaterialPropertyBlock();
            SetPlayerAlpha(0f);     // start fully invisible
        }

        // Start the cube at its isometric orientation.
        if (cubeTransform != null)
            cubeTransform.localEulerAngles = _startEuler;

        // Raise the priority of the intro camera so Cinemachine blends FROM it.
        // The player camera (CC_ELEMENTO.PlayerCamera) must have a lower priority.
        if (introCamera != null)
            introCamera.Priority = 20;

        // Find the Gameplay action map through LevelManager's input references.
        // LevelManager always has at least MoveAction wired; we use that to reach the asset.
        if (LevelManager.Instance != null && LevelManager.Instance.MoveAction != null)
        {
            InputAction moveAction = LevelManager.Instance.MoveAction.action;
            _gameplayMap = moveAction.actionMap;
        }

        // Lock input immediately so the player cannot skip the sequence.
        LockInput(true);

        // Kick off the coroutine. It will NOT replay on respawn because _hasStarted guards it.
        if (!_hasStarted)
        {
            _hasStarted = true;
            StartCoroutine(PlayIntro());
        }
    }

    // RespawnManager.ResetObject is NOT implemented here on purpose.
    // The sequence must not replay after a respawn, so it does NOT implement IResettable
    // and does NOT register with RespawnManager.

    // ------------------------------------------------------------------
    // INTRO COROUTINE
    // ------------------------------------------------------------------
    private IEnumerator PlayIntro()
    {
        // ---- Phase 1: Hold on the isometric view ----
        if (holdStartDelay > 0f)
            yield return new WaitForSeconds(holdStartDelay);

        // ---- Phase 2: Blend intro camera away ----
        // Dropping the priority makes Cinemachine blend back to the player camera.
        if (introCamera != null)
            introCamera.Priority = 0;

        // ---- Phase 3: Morph cube and fade player in ----
        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / introDuration);
            float smoothT = t * t * (3f - 2f * t);   // smoothstep

            // Rotate the cube from isometric to flat.
            if (cubeTransform != null)
                cubeTransform.localEulerAngles = Vector3.LerpUnclamped(_startEuler, _endEuler, smoothT);

            // Fade the player in.
            SetPlayerAlpha(smoothT);

            yield return null;
        }

        // ---- Finalise ----
        if (cubeTransform != null)
            cubeTransform.localEulerAngles = _endEuler;

        SetPlayerAlpha(1f);

        // Release the level clock and input.
        if (LevelManager.Instance != null)
            LevelManager.Instance.IsIntroPending = false;

        LockInput(false);
    }

    // ------------------------------------------------------------------
    // HELPERS
    // ------------------------------------------------------------------
    // Disables or re-enables the Gameplay action map.
    // Disabling the map blocks all InputActionReferences in the map (Move, Jump, Dash…)
    // so the player character ignores input while the intro plays.
    private void LockInput(bool locked)
    {
        if (_gameplayMap == null) return;

        if (locked)
            _gameplayMap.Disable();
        else
            _gameplayMap.Enable();
    }

    // Sets the alpha of every renderer under playerVisualsRoot.
    // Uses a MaterialPropertyBlock so no new material instances are created.
    // Supports standard URP shaders (_BaseColor, _Color) and custom Elemento ball shaders
    // (such as M_Ball_Aether with _TintColor, _GlassAlpha, _RimColor, _StarColor, etc.).
    // Restores original unoverridden state cleanly at alpha >= 1f.
    private void SetPlayerAlpha(float alpha)
    {
        if (_fadeData == null) return;

        for (int i = 0; i < _fadeData.Length; i++)
        {
            Renderer r = _fadeData[i].renderer;
            if (r == null) continue;

            if (alpha >= 1f)
            {
                // Restore original state cleanly by clearing the property block
                r.SetPropertyBlock(null);
                continue;
            }

            // If the shared material changed (e.g. BallVisuals assigned on Start), re-cache original values
            if (r.sharedMaterial != _fadeData[i].lastSharedMaterial)
                _fadeData[i] = BuildFadeData(r);

            ref RendererFadeData data = ref _fadeData[i];

            r.GetPropertyBlock(_mpb);

            if (data.hasBaseColor)
            {
                Color c = data.originalBaseColor;
                c.a *= alpha;
                _mpb.SetColor(_baseColorId, c);
            }

            if (data.hasColor)
            {
                Color c = data.originalColor;
                c.a *= alpha;
                _mpb.SetColor(_colorPropId, c);
            }

            if (data.hasTintColor)
            {
                Color c = data.originalTintColor * alpha;
                _mpb.SetColor(_tintColorId, c);
            }

            if (data.hasGlassAlpha)
            {
                _mpb.SetFloat(_glassAlphaId, data.originalGlassAlpha * alpha);
            }

            if (data.hasRimColor)
            {
                _mpb.SetColor(_rimColorId, data.originalRimColor * alpha);
            }

            if (data.hasStarColor)
            {
                _mpb.SetColor(_starColorId, data.originalStarColor * alpha);
            }

            if (data.hasLineColor)
            {
                _mpb.SetColor(_lineColorId, data.originalLineColor * alpha);
            }

            if (data.hasNebulaColor)
            {
                _mpb.SetColor(_nebulaColorId, data.originalNebulaColor * alpha);
            }

            if (data.hasSwirlColor)
            {
                _mpb.SetColor(_swirlColorId, data.originalSwirlColor * alpha);
            }

            if (data.hasCoreColor)
            {
                _mpb.SetColor(_coreColorId, data.originalCoreColor * alpha);
            }

            if (data.hasBoltColor)
            {
                _mpb.SetColor(_boltColorId, data.originalBoltColor * alpha);
            }

            if (data.hasRingColor)
            {
                _mpb.SetColor(_ringColorId, data.originalRingColor * alpha);
            }

            if (data.hasCrackGlow)
            {
                _mpb.SetColor(_crackGlowId, data.originalCrackGlow * alpha);
            }

            if (data.hasRockLight)
            {
                _mpb.SetColor(_rockLightId, data.originalRockLight * alpha);
            }

            if (data.hasRockDark)
            {
                _mpb.SetColor(_rockDarkId, data.originalRockDark * alpha);
            }

            if (data.hasHotColor)
            {
                _mpb.SetColor(_hotColorId, data.originalHotColor * alpha);
            }

            if (data.hasLavaColor)
            {
                _mpb.SetColor(_lavaColorId, data.originalLavaColor * alpha);
            }

            if (data.hasFacetLight)
            {
                _mpb.SetColor(_facetLightId, data.originalFacetLight * alpha);
            }

            if (data.hasFacetDark)
            {
                _mpb.SetColor(_facetDarkId, data.originalFacetDark * alpha);
            }

            if (data.hasEdgeColor)
            {
                _mpb.SetColor(_edgeColorId, data.originalEdgeColor * alpha);
            }

            r.SetPropertyBlock(_mpb);
        }
    }

    private static RendererFadeData BuildFadeData(Renderer r)
    {
        var data = new RendererFadeData();
        data.renderer = r;
        if (r == null) return data;

        Material mat = r.sharedMaterial;
        data.lastSharedMaterial = mat;
        if (mat == null) return data;

        if (mat.HasProperty(_baseColorId))
        {
            data.hasBaseColor = true;
            data.originalBaseColor = mat.GetColor(_baseColorId);
        }

        if (mat.HasProperty(_colorPropId))
        {
            data.hasColor = true;
            data.originalColor = mat.GetColor(_colorPropId);
        }

        if (mat.HasProperty(_tintColorId))
        {
            data.hasTintColor = true;
            data.originalTintColor = mat.GetColor(_tintColorId);
        }

        if (mat.HasProperty(_glassAlphaId))
        {
            data.hasGlassAlpha = true;
            data.originalGlassAlpha = mat.GetFloat(_glassAlphaId);
        }

        if (mat.HasProperty(_rimColorId))
        {
            data.hasRimColor = true;
            data.originalRimColor = mat.GetColor(_rimColorId);
        }

        if (mat.HasProperty(_starColorId))
        {
            data.hasStarColor = true;
            data.originalStarColor = mat.GetColor(_starColorId);
        }

        if (mat.HasProperty(_lineColorId))
        {
            data.hasLineColor = true;
            data.originalLineColor = mat.GetColor(_lineColorId);
        }

        if (mat.HasProperty(_nebulaColorId))
        {
            data.hasNebulaColor = true;
            data.originalNebulaColor = mat.GetColor(_nebulaColorId);
        }

        if (mat.HasProperty(_swirlColorId))
        {
            data.hasSwirlColor = true;
            data.originalSwirlColor = mat.GetColor(_swirlColorId);
        }

        if (mat.HasProperty(_coreColorId))
        {
            data.hasCoreColor = true;
            data.originalCoreColor = mat.GetColor(_coreColorId);
        }

        if (mat.HasProperty(_boltColorId))
        {
            data.hasBoltColor = true;
            data.originalBoltColor = mat.GetColor(_boltColorId);
        }

        if (mat.HasProperty(_ringColorId))
        {
            data.hasRingColor = true;
            data.originalRingColor = mat.GetColor(_ringColorId);
        }

        if (mat.HasProperty(_crackGlowId))
        {
            data.hasCrackGlow = true;
            data.originalCrackGlow = mat.GetColor(_crackGlowId);
        }

        if (mat.HasProperty(_rockLightId))
        {
            data.hasRockLight = true;
            data.originalRockLight = mat.GetColor(_rockLightId);
        }

        if (mat.HasProperty(_rockDarkId))
        {
            data.hasRockDark = true;
            data.originalRockDark = mat.GetColor(_rockDarkId);
        }

        if (mat.HasProperty(_hotColorId))
        {
            data.hasHotColor = true;
            data.originalHotColor = mat.GetColor(_hotColorId);
        }

        if (mat.HasProperty(_lavaColorId))
        {
            data.hasLavaColor = true;
            data.originalLavaColor = mat.GetColor(_lavaColorId);
        }

        if (mat.HasProperty(_facetLightId))
        {
            data.hasFacetLight = true;
            data.originalFacetLight = mat.GetColor(_facetLightId);
        }

        if (mat.HasProperty(_facetDarkId))
        {
            data.hasFacetDark = true;
            data.originalFacetDark = mat.GetColor(_facetDarkId);
        }

        if (mat.HasProperty(_edgeColorId))
        {
            data.hasEdgeColor = true;
            data.originalEdgeColor = mat.GetColor(_edgeColorId);
        }

        return data;
    }
}
