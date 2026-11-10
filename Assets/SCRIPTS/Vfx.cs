using UnityEngine;

// Every particle effect of the BALL, by name (like Sfx does for sounds).
// Any script plays one with a single line:
//     Vfx.Play(VfxType.Jump, position, rotation);                  a neutral burst
//     Vfx.PlayTinted(VfxType.Respawn, position, rotation, power);  a burst in the color of an element
//     Vfx.SetLoop(VfxType.DashTrail, true);                        a continuous effect, follows the ball
// If there is no Vfx object in the scene, these lines simply do nothing.
//
// SETUP (no wiring at all):
//   1) make an empty object "Vfx" with this script (put it at position 0,0,0, rotation 0,0,0)
//   2) under it, create one Particle System per effect and NAME it exactly like the list below
//      (Jump, Land, DashTrail, AmbientFire...). The script finds them by name when the game starts.
//   3) drag the Ball (the mesh) into "Follow"
//   A name that is missing is just skipped, so you can build the effects one at a time.
//   If an effect has several layers (a ring + sparks), make the extra layers CHILDREN of the named
//   Particle System: they play, stop and move together with it.
//
// No garbage while playing: everything is found once in Awake.
public enum VfxType
{
    // one-shot effects: played at a position
    Jump,
    DoubleJump,
    WallJump,
    Land,
    DashStart,
    GroundPoundStart,
    GroundPoundImpact,
    GlideOpen,
    GravityFlip,
    PowerUpCollect,
    Respawn,
    Death,

    // looping effects, in the color of the current element: ON/OFF, follow the ball
    DashTrail,
    RunTrail,       // a soft light trace while running fast on the ground
    WallSlide,
    GlideStream,

    // looping effects with their own color: one is ON at a time, depending on the power
    AmbientSpark,    // no power
    AmbientFire,
    AmbientWater,
    AmbientAir,
    AmbientEarth
}

public class Vfx : MonoBehaviour
{
    public static Vfx Instance { get; private set; }

    // ------------------------------------------------------------------
    // THE ONE-LINE API
    // ------------------------------------------------------------------
    public static void Play(VfxType type, Vector3 position)
    {
        if (Instance != null)
            Instance.PlayInternal(type, position, Quaternion.identity, false, PowerUpType.None);
    }

    public static void Play(VfxType type, Vector3 position, Quaternion rotation)
    {
        if (Instance != null)
            Instance.PlayInternal(type, position, rotation, false, PowerUpType.None);
    }

    // Same, but the particles get the color of the element
    public static void PlayTinted(VfxType type, Vector3 position, Quaternion rotation, PowerUpType element)
    {
        if (Instance != null)
            Instance.PlayInternal(type, position, rotation, true, element);
    }

    // Starts or stops a looping effect
    public static void SetLoop(VfxType type, bool on)
    {
        if (Instance != null)
            Instance.SetLoopInternal(type, on);
    }

    // The power changed: the right ambient effect turns on, and the movement loops turn off
    public static void SetPower(PowerUpType power)
    {
        if (Instance != null)
            Instance.SetPowerInternal(power);
    }

    // Stops every looping effect (the particles already in the air finish by themselves)
    public static void StopLoops()
    {
        if (Instance != null)
            Instance.StopLoopsInternal();
    }

    // ------------------------------------------------------------------
    // SETTINGS
    // ------------------------------------------------------------------
    // The ball (mesh). The looping effects follow its position.
    public Transform Follow;

    [Header("Element colors (for the tinted effects)")]
    public Color SparkColor = new Color(1f, 0.9f, 0.3f, 1f);   // no power
    public Color FireColor = new Color(1f, 0.45f, 0.1f, 1f);
    public Color WaterColor = new Color(0.3f, 0.85f, 1f, 1f);
    public Color AirColor = new Color(0.8f, 0.95f, 1f, 1f);
    public Color EarthColor = new Color(0.8f, 0.5f, 0.25f, 1f);

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private const int FirstLoop = (int)VfxType.DashTrail;
    private const int LastTintedLoop = (int)VfxType.GlideStream;

    private ParticleSystem[] _roots;        // the system with the exact name, one per effect
    private ParticleSystem[][] _groups;     // the root + its children (all of them get the color)
    private Quaternion[] _baseRotation;     // the rotation we gave each system in the scene
    private bool[] _loopOn;

    private void Awake()
    {
        Instance = this;

        int count = System.Enum.GetValues(typeof(VfxType)).Length;
        _roots = new ParticleSystem[count];
        _groups = new ParticleSystem[count][];
        _baseRotation = new Quaternion[count];
        _loopOn = new bool[count];

        // find every Particle System under this object whose name is a VfxType
        // (this runs once, so the garbage of the search doesn't matter)
        ParticleSystem[] all = GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < all.Length; i++)
        {
            if (!System.Enum.TryParse(all[i].name, true, out VfxType type)) continue;

            int index = (int)type;
            if (_roots[index] != null) continue;

            _roots[index] = all[i];
            _baseRotation[index] = all[i].transform.rotation;
            _groups[index] = all[i].GetComponentsInChildren<ParticleSystem>(true);
        }
    }

    private void Start()
    {
        // at the start of the level: the spark of the "no power" ball
        // (unless the player already told us its power, for example a Starting Power set for testing)
        if (!_powerApplied)
            SetPowerInternal(PowerUpType.None);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // The looping effects follow the ball (position only: their own rotation stays)
    private void LateUpdate()
    {
        if (Follow == null) return;

        for (int i = FirstLoop; i < _roots.Length; i++)
        {
            if (_loopOn[i] && _roots[i] != null)
                _roots[i].transform.position = Follow.position;
        }
    }

    // ------------------------------------------------------------------
    // PLAYING
    // ------------------------------------------------------------------
    private void PlayInternal(VfxType type, Vector3 position, Quaternion rotation, bool tinted, PowerUpType element)
    {
        int index = (int)type;
        ParticleSystem root = _roots[index];
        if (root == null) return;

        // the effect is placed at the position, and turned with the ball (so it follows a gravity flip)
        root.transform.SetPositionAndRotation(position, rotation * _baseRotation[index]);

        if (tinted)
            SetColor(index, GetColor(element));

        // stop emitting first, so Play() restarts even if the last play is not over yet.
        // (the particles already flying are not cleared)
        root.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        root.Play(true);
    }

    private void SetLoopInternal(VfxType type, bool on)
    {
        int index = (int)type;
        if (_loopOn[index] == on) return;       // nothing changed: nothing to do
        _loopOn[index] = on;

        ParticleSystem root = _roots[index];
        if (root == null) return;

        if (on)
        {
            // the movement loops take the color of the current element, the ambient ones keep their own
            if (index <= LastTintedLoop)
                SetColor(index, GetColor(_currentPower));

            if (Follow != null)
                root.transform.position = Follow.position;

            root.Play(true);
        }
        else
        {
            root.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private PowerUpType _currentPower = PowerUpType.None;
    private bool _powerApplied;     // true once SetPower was called (so Start doesn't overwrite it)

    private void SetPowerInternal(PowerUpType power)
    {
        _currentPower = power;
        _powerApplied = true;

        // the movement effects stop (a new power = a clean start)
        SetLoopInternal(VfxType.DashTrail, false);
        SetLoopInternal(VfxType.RunTrail, false);
        SetLoopInternal(VfxType.WallSlide, false);
        SetLoopInternal(VfxType.GlideStream, false);

        // only the ambient effect of this element is on
        SetLoopInternal(VfxType.AmbientSpark, power == PowerUpType.None);
        SetLoopInternal(VfxType.AmbientFire, power == PowerUpType.Fire);
        SetLoopInternal(VfxType.AmbientWater, power == PowerUpType.Water);
        SetLoopInternal(VfxType.AmbientAir, power == PowerUpType.Air);
        SetLoopInternal(VfxType.AmbientEarth, power == PowerUpType.Earth);
    }

    private void StopLoopsInternal()
    {
        for (int i = FirstLoop; i < _roots.Length; i++)
            SetLoopInternal((VfxType)i, false);
    }

    // ------------------------------------------------------------------
    // COLORS
    // ------------------------------------------------------------------
    private void SetColor(int index, Color color)
    {
        ParticleSystem[] group = _groups[index];
        if (group == null) return;

        for (int i = 0; i < group.Length; i++)
        {
            ParticleSystem.MainModule main = group[i].main;
            main.startColor = color;
        }
    }

    private Color GetColor(PowerUpType element)
    {
        switch (element)
        {
            case PowerUpType.Fire: return FireColor;
            case PowerUpType.Water: return WaterColor;
            case PowerUpType.Air: return AirColor;
            case PowerUpType.Earth: return EarthColor;
            default: return SparkColor;
        }
    }
}