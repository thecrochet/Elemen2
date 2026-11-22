using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// One per level scene. It owns the level clock, detects the finish and loads the next level.
//
// The clock follows the same "save" idea as checkpoints (IResettable):
//   - it only counts from the first input after each respawn
//   - when you die, the time goes back to what it was at the last checkpoint (0 at the start)
// So the final time is the time of your uninterrupted route: no time is lost to deaths,
// and every faster route shows up directly in the number.
public class LevelManager : MonoBehaviour, IResettable
{
    // other scripts (LevelGoal, LevelHUD) find the manager through this
    public static LevelManager Instance { get; private set; }

    // ------------------------------------------------------------------
    // SETTINGS
    // ------------------------------------------------------------------
    public CC_ELEMENTO Player;

    // The same Move and Jump actions as the character. They only start the clock.
    public InputActionReference MoveAction;
    public InputActionReference JumpAction;

    // true  = the clock starts on the first input after a respawn (recommended)
    // false = the clock runs as soon as the level (or the respawn) starts
    public bool StartOnFirstInput = true;

    // Seconds between the finish and the next level, so the player can read his time
    [Min(0f)] public float NextLevelDelay = 1.5f;

    // Name of the next scene. Empty = the next scene in the Build Settings list.
    public string NextSceneName = "";

    // ------------------------------------------------------------------
    // STARS (time only, 0 to 3). Tune the three limits per level after playtests.
    // ------------------------------------------------------------------
    // false = no stars in this level (tutorial): nothing is saved, the HUD hides the star icons
    public bool EnableStars = true;

    // finish in this time or less = 3 stars (the fastest route, secret roads included)
    [Min(0f)] public float ThreeStarTime = 7f;

    // 2 stars
    [Min(0f)] public float TwoStarTime = 12f;

    // 1 star. Slower than this = 0 stars (the level is still finished).
    [Min(0f)] public float OneStarTime = 30f;

    // ------------------------------------------------------------------
    // INFO FOR THE HUD
    // ------------------------------------------------------------------
    public float LevelTime { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsNewBest { get; private set; }

    // true = the HUD must NOT show the finish panel yet (the exit animation is playing)
    public bool HoldFinishPanel { get; set; }

    // true while the clock counts (moving platforms only move while this is true)
    public bool IsClockRunning => _clockRunning;

    // best finished time of this level (-1 = never finished)
    public float BestTime { get; private set; } = -1f;

    // stars of this run (0 to 3, always 0 while EnableStars is off)
    public int Stars { get; private set; }

    // the best star count saved for this level (before this run is counted)
    public int SavedStars { get; private set; }

    // true if this run earned more stars than any run before
    public bool IsNewStarRecord { get; private set; }

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private bool _clockRunning;
    private float _startTime;       // Time.time minus the time already on the clock
    private float _committedTime;   // the "save": the clock value at the last checkpoint
    private float _loadTime;        // when to load the next level
    private bool _loading;

    // each level keeps its own best time, named after its scene
    private string BestTimeKey => "BestTime_" + SceneManager.GetActiveScene().name;

    private static string StarsKey(string sceneName)
    {
        return "Stars_" + sceneName;
    }

    // keep the limits in order while you edit them in the Inspector
    private void OnValidate()
    {
        if (TwoStarTime < ThreeStarTime) TwoStarTime = ThreeStarTime;
        if (OneStarTime < TwoStarTime) OneStarTime = TwoStarTime;
    }

    // 0 to 3 stars for a finish time. It compares the hundredths the HUD shows, so a time that reads
    // 7.00 earns the same stars as a limit of 7.00.
    public int GetStarsForTime(float time)
    {
        int centis = (int)(time * 100f);

        if (centis <= Mathf.RoundToInt(ThreeStarTime * 100f)) return 3;
        if (centis <= Mathf.RoundToInt(TwoStarTime * 100f)) return 2;
        if (centis <= Mathf.RoundToInt(OneStarTime * 100f)) return 1;
        return 0;
    }

    // The time limit for the next star up (0 stars -> the 1 star limit, 1 -> 2, 2 -> 3). False at 3 stars.
    public bool TryGetNextStarTime(int stars, out float time)
    {
        switch (stars)
        {
            case 0: time = OneStarTime; return true;
            case 1: time = TwoStarTime; return true;
            case 2: time = ThreeStarTime; return true;
            default: time = 0f; return false;
        }
    }

    // best saved star count of any level (0 if never finished). For the hub's level portals.
    public static int GetSavedStars(string sceneName)
    {
        return PlayerPrefs.GetInt(StarsKey(sceneName), 0);
    }

    // all saved stars added up over the scenes in the Build Settings (for the hub star counter and world gates)
    public static int GetTotalSavedStars()
    {
        int total = 0;
        int count = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            total += GetSavedStars(sceneName);
        }

        return total;
    }

    // Awake runs before every Start, so the Instance is ready for the HUD and the goal
    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);

        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        BestTime = PlayerPrefs.GetFloat(BestTimeKey, -1f);
        SavedStars = GetSavedStars(SceneManager.GetActiveScene().name);

        // Register also saves the starting state (time 0) right away
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void Update()
    {
        if (IsFinished)
        {
            // the time is frozen, we just wait a moment and move on
            if (Time.time >= _loadTime)
                LoadNextLevel();
            return;
        }

        if (!_clockRunning)
        {
            // waiting for the player to start moving
            if (StartOnFirstInput && !HasStartInput())
                return;

            _clockRunning = true;

            // continue from the time on the clock (0 at the start, or the checkpoint time)
            _startTime = Time.time - LevelTime;
        }

        LevelTime = Time.time - _startTime;
    }

    // true on the frame the player first moves or jumps
    private bool HasStartInput()
    {
        if (JumpAction != null && JumpAction.action.WasPressedThisFrame())
            return true;

        if (MoveAction != null && MoveAction.action.ReadValue<Vector2>().sqrMagnitude > 0.01f)
            return true;

        return false;
    }

    // Called by LevelGoal when the player touches the finish.
    // Returns true only the first time (so the goal can play its effect once).
    public bool CompleteLevel()
    {
        if (IsFinished) return false;

        // a player who just died can't finish
        if (Player.CurrentState == CharacterState.Dead) return false;

        IsFinished = true;
        _clockRunning = false;   // LevelTime stays frozen at its last value

        // best time: keep the faster one
        float previousBest = PlayerPrefs.GetFloat(BestTimeKey, -1f);
        IsNewBest = previousBest < 0f || LevelTime < previousBest;

        if (IsNewBest)
        {
            PlayerPrefs.SetFloat(BestTimeKey, LevelTime);
            PlayerPrefs.Save();
            BestTime = LevelTime;
        }
        else
        {
            BestTime = previousBest;
        }

        // stars: only the best count is kept
        if (EnableStars)
        {
            Stars = GetStarsForTime(LevelTime);
            string starsKey = StarsKey(SceneManager.GetActiveScene().name);
            int savedStars = PlayerPrefs.GetInt(starsKey, 0);
            IsNewStarRecord = Stars > savedStars;

            if (IsNewStarRecord)
            {
                PlayerPrefs.SetInt(starsKey, Stars);
                PlayerPrefs.Save();
                SavedStars = Stars;
            }
            else
            {
                SavedStars = savedStars;
            }
        }

        // no more deaths or respawns, and the character stops
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Lock();

        Player.Finish();

        _loadTime = Time.time + NextLevelDelay;
        return true;
    }

    // The exit animation asks for more time before the next level loads (it never shortens the delay)
    public void DelayNextLevel(float secondsFromNow)
    {
        _loadTime = Mathf.Max(_loadTime, Time.time + secondsFromNow);
    }

    public static string ResolveNextSceneName(string currentSceneName, string explicitNext = "")
    {
        if (!string.IsNullOrEmpty(explicitNext))
        {
            return explicitNext;
        }

        // Check if current scene follows pattern Level_XX
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(currentSceneName, @"Level_(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int currentNum))
        {
            int nextNum = currentNum + 1;
            string targetCandidate = "Level_" + nextNum.ToString("D2");
            string targetCandidateAlt = "Level_" + nextNum.ToString();

            int count = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < count; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (sceneName.Equals(targetCandidate, System.StringComparison.OrdinalIgnoreCase) ||
                    sceneName.Equals(targetCandidateAlt, System.StringComparison.OrdinalIgnoreCase))
                {
                    return sceneName;
                }
            }
        }

        return "";
    }

    private void LoadNextLevel()
    {
        // Update runs every frame, so make sure we only start loading once
        if (_loading) return;
        _loading = true;

        string targetScene = ResolveNextSceneName(SceneManager.GetActiveScene().name, NextSceneName);
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
            return;
        }

        int next = SceneManager.GetActiveScene().buildIndex + 1;

        // after the last level, fall back to Hub/MainMenu or first scene
        if (next < 0 || next >= SceneManager.sceneCountInBuildSettings)
        {
            next = 0;
            int count = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < count; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (sceneName.Equals("Hub", System.StringComparison.OrdinalIgnoreCase) ||
                    sceneName.Equals("MainMenu", System.StringComparison.OrdinalIgnoreCase) ||
                    sceneName.Equals("00_Tutorial", System.StringComparison.OrdinalIgnoreCase))
                {
                    next = i;
                    break;
                }
            }
        }

        SceneManager.LoadScene(next);
    }

    // ------------------------------------------------------------------
    // IResettable: SAVE / LOAD, like pickups and breakable blocks
    // ------------------------------------------------------------------
    // SAVE: at the level start and at each checkpoint
    public void CommitState()
    {
        _committedTime = LevelTime;
    }

    // LOAD: after a death, the clock goes back to the saved time and waits for the first input
    public void ResetObject()
    {
        if (IsFinished) return;

        LevelTime = _committedTime;
        _clockRunning = false;
    }
}
