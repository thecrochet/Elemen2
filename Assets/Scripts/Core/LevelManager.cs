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
    [Header("Tutorial")]
    [Tooltip("When true the HUD hides the timer and star ratings, and the clock does not start until the intro finishes.")]
    public bool IsTutorialLevel;

    // Set by TutorialIntroSequence while the cold-open animation runs.
    // The clock ignores StartOnFirstInput while this is true.
    public bool IsIntroPending { get; set; }

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
            // The tutorial intro must finish before the clock can start
            if (IsIntroPending)
                return;

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

    private void LoadNextLevel()
    {
        // Update runs every frame, so make sure we only start loading once
        if (_loading) return;
        _loading = true;

        if (!string.IsNullOrEmpty(NextSceneName))
        {
            SceneManager.LoadScene(NextSceneName);
            return;
        }

        int next = SceneManager.GetActiveScene().buildIndex + 1;

        // after the last level, go back to the first scene (a hub or menu later)
        if (next >= SceneManager.sceneCountInBuildSettings)
            next = 0;

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
