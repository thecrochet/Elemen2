using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Modular UI component for glowing neon buttons matching Elemento's neon cyber aesthetic.
/// Clones its material instance to allow independent hover/selection animations,
/// drives perimeter wave ripples and HDR glow intensity using unscaled time (animating while paused),
/// and smoothly interpolates all visual transitions with zero per-frame allocations.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public class NeonWaveButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("Colors (HDR)")]
    [Tooltip("Dark semi-transparent glass fill in idle state.")]
    [SerializeField] private Color idleFillColor = new Color(0.02f, 0.05f, 0.09f, 0.88f);

    [Tooltip("Dark semi-transparent glass fill in selected/hovered state.")]
    [SerializeField] private Color selectedFillColor = new Color(0.04f, 0.09f, 0.16f, 0.94f);

    [Tooltip("Subtle cyan/teal border color in idle state.")]
    [SerializeField, ColorUsage(true, true)] private Color idleBorderColor = new Color(0.08f, 0.55f, 0.65f, 0.60f);

    [Tooltip("Bright HDR electric cyan glow color when selected or hovered.")]
    [SerializeField, ColorUsage(true, true)] private Color selectedBorderColor = new Color(0.0f, 2.4f, 2.8f, 1.0f);

    [Tooltip("White-hot boost applied to the center of the outline border.")]
    [Range(0f, 1f)]
    [SerializeField] private float coreWhiteAmount = 0.5f;

    [Header("Border & Geometry")]
    [Tooltip("Width of the crisp neon border in pixels.")]
    [Range(1f, 10f)]
    [SerializeField] private float borderThickness = 3.0f;

    [Tooltip("Corner rounding radius in pixels.")]
    [Range(0f, 40f)]
    [SerializeField] private float cornerRadius = 14.0f;

    [Tooltip("Inner inset padding in pixels to ensure outer bloom does not clip against the quad boundary.")]
    [Range(0f, 30f)]
    [SerializeField] private float glowPadding = 8.0f;

    [Header("Glow Falloff")]
    [Tooltip("Spread distance of the soft outer bloom falloff in pixels.")]
    [Range(1f, 30f)]
    [SerializeField] private float glowSpread = 8.0f;

    [Tooltip("Outer bloom intensity in idle state.")]
    [Range(0f, 3f)]
    [SerializeField] private float idleGlowStrength = 0.35f;

    [Tooltip("Outer bloom intensity in selected/hovered state.")]
    [Range(0f, 3f)]
    [SerializeField] private float selectedGlowStrength = 1.25f;

    [Header("Perimeter Wave Animation")]
    [Tooltip("Amplitude of the travelling wave ripples in idle state.")]
    [Range(0f, 1f)]
    [SerializeField] private float idleWaveAmplitude = 0.08f;

    [Tooltip("Amplitude of the travelling wave ripples when selected or hovered.")]
    [Range(0f, 1f)]
    [SerializeField] private float selectedWaveAmplitude = 0.55f;

    [Tooltip("Spatial frequency of the perimeter wave ripples.")]
    [Range(0.001f, 0.05f)]
    [SerializeField] private float waveFrequency = 0.015f;

    [Tooltip("Travelling speed of perimeter ripples around the border.")]
    [Range(0f, 20f)]
    [SerializeField] private float waveSpeed = 4.5f;

    [Header("Transition")]
    [Tooltip("Speed of visual state transition between idle and selected/hovered.")]
    [Range(1f, 30f)]
    [SerializeField] private float transitionSpeed = 12.0f;

    [Header("Optional Label")]
    [Tooltip("Button label text component to brighten when selected/hovered.")]
    [SerializeField] private TMP_Text buttonLabel;

    [Tooltip("Label color in idle state.")]
    [SerializeField] private Color labelIdleColor = new Color(0.35f, 0.85f, 0.95f, 0.85f);

    [Tooltip("Label color in selected/hovered state.")]
    [SerializeField] private Color labelSelectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    // Cached shader property IDs (zero string allocation in Update)
    private static readonly int PropBorderColor = Shader.PropertyToID("_BorderColor");
    private static readonly int PropFillColor = Shader.PropertyToID("_FillColor");
    private static readonly int PropBorderThickness = Shader.PropertyToID("_BorderThickness");
    private static readonly int PropCornerRadius = Shader.PropertyToID("_CornerRadius");
    private static readonly int PropCoreWhite = Shader.PropertyToID("_CoreWhite");
    private static readonly int PropGlowSpread = Shader.PropertyToID("_GlowSpread");
    private static readonly int PropGlowStrength = Shader.PropertyToID("_GlowStrength");
    private static readonly int PropWaveAmplitude = Shader.PropertyToID("_WaveAmplitude");
    private static readonly int PropWaveFrequency = Shader.PropertyToID("_WaveFrequency");
    private static readonly int PropWaveSpeed = Shader.PropertyToID("_WaveSpeed");
    private static readonly int PropDimensions = Shader.PropertyToID("_Dimensions");
    private static readonly int PropPadding = Shader.PropertyToID("_Padding");
    private static readonly int PropUnscaledTime = Shader.PropertyToID("_UnscaledTime");

    // Component references
    private Image _image;
    private RectTransform _rectTransform;
    private Button _button;
    private Material _materialInstance;

    // State tracking
    private bool _isPointerOver;
    private bool _isSelected;
    private float _currentTransition;
    private float _accumulatedUnscaledTime;
    private Vector2 _lastSize;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();

        if (buttonLabel == null)
        {
            buttonLabel = GetComponentInChildren<TMP_Text>();
        }

        // Prevent default Unity Button color tints from overriding custom shader glow
        if (_button != null)
        {
            _button.transition = Selectable.Transition.None;
        }

        InitializeMaterialInstance();
    }

    private void OnEnable()
    {
        // Re-check EventSystem current selection in case this button was pre-selected on open
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            _isSelected = true;
        }
        else
        {
            _isSelected = false;
        }

        _isPointerOver = false;
        _currentTransition = _isSelected ? 1.0f : 0.0f;

        ApplyPropertiesToMaterial(forceUpdateStatic: true);
    }

    private void OnDisable()
    {
        _isPointerOver = false;
        _isSelected = false;
        _currentTransition = 0.0f;
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(_materialInstance);
            }
            else
            {
                Destroy(_materialInstance);
            }
#else
            Destroy(_materialInstance);
#endif
            _materialInstance = null;
        }
    }

    private void Update()
    {
        if (_materialInstance == null) return;

        float dt = Time.unscaledDeltaTime;
        _accumulatedUnscaledTime += dt;

        bool targetActive = _isPointerOver || _isSelected;
        float targetT = targetActive ? 1.0f : 0.0f;

        // Smooth exponential interpolation
        _currentTransition = Mathf.MoveTowards(_currentTransition, targetT, dt * transitionSpeed);

        // Check if RectTransform size changed
        Vector2 currentSize = _rectTransform.rect.size;
        if (currentSize != _lastSize)
        {
            _lastSize = currentSize;
            _materialInstance.SetVector(PropDimensions, new Vector4(currentSize.x, currentSize.y, 0f, 0f));
        }

        // Drive unscaled time uniform
        _materialInstance.SetFloat(PropUnscaledTime, _accumulatedUnscaledTime);

        // Drive dynamic animated properties
        Color currentBorder = Color.Lerp(idleBorderColor, selectedBorderColor, _currentTransition);
        Color currentFill = Color.Lerp(idleFillColor, selectedFillColor, _currentTransition);
        float currentGlow = Mathf.Lerp(idleGlowStrength, selectedGlowStrength, _currentTransition);
        float currentWaveAmp = Mathf.Lerp(idleWaveAmplitude, selectedWaveAmplitude, _currentTransition);

        _materialInstance.SetColor(PropBorderColor, currentBorder);
        _materialInstance.SetColor(PropFillColor, currentFill);
        _materialInstance.SetFloat(PropGlowStrength, currentGlow);
        _materialInstance.SetFloat(PropWaveAmplitude, currentWaveAmp);

        if (buttonLabel != null)
        {
            buttonLabel.color = Color.Lerp(labelIdleColor, labelSelectedColor, _currentTransition);
        }
    }

    private void InitializeMaterialInstance()
    {
        if (_image == null || _image.material == null) return;

        // Clone base material so each button has its own independent uniforms
        _materialInstance = Instantiate(_image.material);
        _materialInstance.name = $"{_image.material.name}_{gameObject.name}_Instance";
        _image.material = _materialInstance;

        _lastSize = _rectTransform != null ? _rectTransform.rect.size : new Vector2(420f, 84f);
        ApplyPropertiesToMaterial(forceUpdateStatic: true);
    }

    private void ApplyPropertiesToMaterial(bool forceUpdateStatic)
    {
        if (_materialInstance == null) return;

        if (forceUpdateStatic)
        {
            _materialInstance.SetFloat(PropBorderThickness, borderThickness);
            _materialInstance.SetFloat(PropCornerRadius, cornerRadius);
            _materialInstance.SetFloat(PropCoreWhite, coreWhiteAmount);
            _materialInstance.SetFloat(PropGlowSpread, glowSpread);
            _materialInstance.SetFloat(PropPadding, glowPadding);
            _materialInstance.SetFloat(PropWaveFrequency, waveFrequency);
            _materialInstance.SetFloat(PropWaveSpeed, waveSpeed);
            _materialInstance.SetVector(PropDimensions, new Vector4(_lastSize.x, _lastSize.y, 0f, 0f));
        }

        Color currentBorder = Color.Lerp(idleBorderColor, selectedBorderColor, _currentTransition);
        Color currentFill = Color.Lerp(idleFillColor, selectedFillColor, _currentTransition);
        float currentGlow = Mathf.Lerp(idleGlowStrength, selectedGlowStrength, _currentTransition);
        float currentWaveAmp = Mathf.Lerp(idleWaveAmplitude, selectedWaveAmplitude, _currentTransition);

        _materialInstance.SetColor(PropBorderColor, currentBorder);
        _materialInstance.SetColor(PropFillColor, currentFill);
        _materialInstance.SetFloat(PropGlowStrength, currentGlow);
        _materialInstance.SetFloat(PropWaveAmplitude, currentWaveAmp);
    }

    // UI Event Handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
    }
}
