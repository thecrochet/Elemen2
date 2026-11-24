using System.Collections.Generic;
using UnityEngine;

// Every sound event of the game. To add a new sound later, add a name at the END of this list.
public enum SfxType
{
    Jump,
    DoubleJump,
    WallJump,
    Land,
    Dash,
    GroundPoundStart,
    GroundPoundImpact,
    PadLaunch,
    GlideOpen,
    GravityFlip,
    Death,
    Respawn,
    PowerUp,
    Checkpoint,
    BlockBreak,
    LevelFinish,
    CrumbleWarn,
    CrumbleFall,
    StompWarn,
    StompImpact,

    // added in Kit 7 (always add new events at the END of this list)
    FireJetWarn,
    FireJetOn,
    LeaperWarn,
    LeaperLeap,
    LeaperSplash,
    DashBoost,
    LaneEnter,
    ChargeGain,
    GateCross,
    DiamondCollect,
    AtomCollect,
    CannonShoot,
    HazardHit,
    CrackHit,
    StarLost,

    // added with the recharge orbs (always add new events at the END of this list)
    RechargeUse,
    RechargeReturn
}

// The settings of ONE sound event (it shows as one line in the Inspector list)
[System.Serializable]
public class SfxEntry
{
    public string Name;              // filled automatically, just a label
    public SfxType Type;
    public AudioClip[] Clips;        // one is picked at random: 2 or 3 clips make the game feel alive
    [Range(0f, 1f)] public float Volume = 1f;
    [Range(0f, 0.3f)] public float PitchVariation = 0.05f;   // a tiny random pitch, so repeats don't sound identical
    public float MinInterval = 0.05f;   // seconds before the same sound can play again (stops machine-gun repeats)
}

// The sound player. Put it on ONE object in each level (for example an empty object called "Sfx").
//
// Any script plays a sound with one line:
//     Sfx.Play(SfxType.Jump);                              a sound that belongs to the player
//     Sfx.PlayAt(SfxType.StompImpact, transform.position); a sound in the level: quieter and panned
//                                                          when it is far from the camera
// If there is no Sfx object in the scene, these lines simply do nothing.
//
// It makes NO garbage while playing: the AudioSources are created once, and reused in turns.
public class Sfx : MonoBehaviour
{
    public static Sfx Instance { get; private set; }

    public static void Play(SfxType type)
    {
        if (Instance != null)
            Instance.PlayInternal(type, 1f, 0f);
    }

    public static void PlayAt(SfxType type, Vector3 position)
    {
        if (Instance != null)
            Instance.PlayAtInternal(type, position);
    }

    // ------------------------------------------------------------------
    // SETTINGS
    // ------------------------------------------------------------------
    // One entry per sound event. The list fills itself with every event when you add the script.
    public SfxEntry[] Sounds;

    [Range(0f, 1f)] public float MasterVolume = 1f;

    // How many sounds can play at the same time
    public int VoiceCount = 12;

    // A sound placed in the level (PlayAt) can't be heard further than this (in units from the camera)
    public float HearDistance = 25f;

    // How much a sound on the left or right side is moved in the stereo (0 = none, 1 = fully)
    [Range(0f, 1f)] public float MaxPan = 0.6f;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private SfxEntry[] _entries;     // the entries, sorted by SfxType so the lookup is instant
    private float[] _lastPlayTime;   // when each event played last
    private AudioSource[] _sources;
    private int _nextSource;
    private Transform _listener;     // the camera (the AudioListener)

    private void Awake()
    {
        Instance = this;

        // sort the entries by event, once
        int count = System.Enum.GetValues(typeof(SfxType)).Length;
        _entries = new SfxEntry[count];
        _lastPlayTime = new float[count];

        for (int i = 0; i < count; i++)
            _lastPlayTime[i] = -999f;

        if (Sounds != null)
        {
            for (int i = 0; i < Sounds.Length; i++)
            {
                if (Sounds[i] != null)
                    _entries[(int)Sounds[i].Type] = Sounds[i];
            }
        }

        // create the AudioSources once (2D sounds: the distance is handled by PlayAt)
        _sources = new AudioSource[Mathf.Max(VoiceCount, 1)];

        for (int i = 0; i < _sources.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            _sources[i] = source;
        }
    }

    private void Start()
    {
        AudioListener listener = FindFirstObjectByType<AudioListener>();

        if (listener != null)
            _listener = listener.transform;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // A sound in the level: quieter with the distance, and moved left or right like on the screen
    private void PlayAtInternal(SfxType type, Vector3 position)
    {
        if (_listener == null)
        {
            PlayInternal(type, 1f, 0f);
            return;
        }

        float dx = position.x - _listener.position.x;
        float dy = position.y - _listener.position.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // too far: nobody hears it, so don't even start it
        if (distance >= HearDistance) return;

        float closeness = 1f - distance / HearDistance;
        float pan = Mathf.Clamp(dx / (HearDistance * 0.5f), -1f, 1f) * MaxPan;

        PlayInternal(type, closeness * closeness, pan);
    }

    // A new event with no clip yet plays the sound it used before, so nothing goes silent while you add clips
    private static SfxType FallbackOf(SfxType type)
    {
        switch (type)
        {
            case SfxType.GateCross:
            case SfxType.DiamondCollect:
            case SfxType.AtomCollect:
            case SfxType.ChargeGain:
                return SfxType.PowerUp;
            default:
                return type;
        }
    }

    private void PlayInternal(SfxType type, float volumeScale, float pan)
    {
        int index = (int)type;
        if (index < 0 || index >= _entries.Length) return;

        SfxEntry entry = _entries[index];

        if (entry == null || entry.Clips == null || entry.Clips.Length == 0)
        {
            // no clip yet for this event: use the sound of the older event it replaces (one step only)
            SfxType fallback = FallbackOf(type);
            if (fallback != type)
                PlayInternal(fallback, volumeScale, pan);

            return;
        }

        // the same sound can't repeat too fast
        float now = Time.unscaledTime;
        if (now - _lastPlayTime[index] < entry.MinInterval) return;
        _lastPlayTime[index] = now;

        AudioClip clip = entry.Clips[Random.Range(0, entry.Clips.Length)];
        if (clip == null) return;

        // use the AudioSources in turns, so sounds can overlap
        AudioSource source = _sources[_nextSource];
        _nextSource = (_nextSource + 1) % _sources.Length;

        source.clip = clip;
        source.volume = entry.Volume * MasterVolume * volumeScale;
        source.pitch = 1f + Random.Range(-entry.PitchVariation, entry.PitchVariation);
        source.panStereo = pan;
        source.Play();
    }

#if UNITY_EDITOR
    // In the Editor only: fills the Inspector list with one line per event, and adds new events later
    private void Reset()
    {
        FillMissingEntries();
    }

    private void OnValidate()
    {
        FillMissingEntries();
    }

    private void FillMissingEntries()
    {
        List<SfxEntry> list = new List<SfxEntry>();

        if (Sounds != null)
            list.AddRange(Sounds);

        bool changed = false;

        foreach (SfxType type in System.Enum.GetValues(typeof(SfxType)))
        {
            bool found = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].Type == type)
                {
                    found = true;

                    if (list[i].Name != type.ToString())
                    {
                        list[i].Name = type.ToString();
                        changed = true;
                    }
                }
            }

            if (!found)
            {
                SfxEntry entry = new SfxEntry();
                entry.Type = type;
                entry.Name = type.ToString();
                list.Add(entry);
                changed = true;
            }
        }

        if (changed)
            Sounds = list.ToArray();
    }
#endif
}
