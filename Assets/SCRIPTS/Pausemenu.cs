using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Pause menu: Esc (or gamepad Start) pauses, with Resume / Restart level / Quit.
//
// Put this script on the CANVAS (or an empty object), NOT on the pause panel:
// the panel is hidden at the start, and a hidden object's Update never runs.
//
// How the pause works: Time.timeScale = 0 freezes everything that depends on time
// (the KCC motor, the level clock, the moving platforms...). Update still runs, so we
// can still read the pause button and the menu keeps working.
public class PauseMenu : MonoBehaviour
{
    // The panel that holds the title and the buttons (starts hidden)
    public GameObject PausePanel;

    // The button that is selected when the menu opens, so keyboard and gamepad work right away
    public Button ResumeButton;

    // Optional: a button action for the pause (Escape + gamepad Start).
    // Empty = the script simply listens to Esc and the gamepad Start button.
    public InputActionReference PauseAction;

    public bool IsPaused { get; private set; }

    private bool _resuming;   // the game starts again at the END of the frame (see LateUpdate)
    private CursorLockMode _previousLockMode;
    private bool _previousCursorVisible;

    private void Start()
    {
        // a scene can be loaded while the game was paused: always start unfrozen
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (PausePanel != null)
            PausePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // never leave the game frozen when this scene goes away
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Update()
    {
        if (!PausePressed()) return;

        if (IsPaused)
            Resume();
        else
            Pause();
    }

    // The game restarts at the end of the frame on purpose: if you click Resume with Space or Enter,
    // that same key press must not also become a jump in the game.
    private void LateUpdate()
    {
        if (!_resuming) return;

        _resuming = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private bool PausePressed()
    {
        if (PauseAction != null)
            return PauseAction.action.WasPressedThisFrame();

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) return true;

        Gamepad gamepad = Gamepad.current;
        return gamepad != null && gamepad.startButton.wasPressedThisFrame;
    }

    // ------------------------------------------------------------------
    // These three are called by the buttons (On Click in the Inspector)
    // ------------------------------------------------------------------
    public void Pause()
    {
        if (IsPaused || _resuming) return;

        // no pause during the finish screen
        if (LevelManager.Instance != null && LevelManager.Instance.IsFinished) return;

        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // show the mouse for the buttons, and remember how it was
        _previousLockMode = Cursor.lockState;
        _previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PausePanel != null)
            PausePanel.SetActive(true);

        if (ResumeButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(ResumeButton.gameObject);
    }

    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused = false;
        _resuming = true;   // Time.timeScale goes back to 1 in LateUpdate

        if (PausePanel != null)
            PausePanel.SetActive(false);

        Cursor.lockState = _previousLockMode;
        Cursor.visible = _previousCursorVisible;
    }

    // Starts the level again from the very beginning (the scene reloads).
    // For an instant retry from the last checkpoint, use your normal restart key instead.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        // in the Editor, Application.Quit does nothing: stop the Play mode instead
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}