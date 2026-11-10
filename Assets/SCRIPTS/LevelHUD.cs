using TMPro;
using UnityEngine;

// Shows the level time, the attempt number and the finish panel.
// Put it on the Canvas and fill the slots. All the texts are optional.
//
// The timer changes many times per second, so it is written into a character array that is
// created ONCE. This makes no garbage (no memory to clean up), which keeps the game smooth.
public class LevelHUD : MonoBehaviour
{
    public TMP_Text TimerText;        // for example "0:12.34", top of the screen
    public TMP_Text AttemptsText;     // for example "Attempt 3"

    public GameObject FinishPanel;    // shown when the level is completed (starts hidden)
    public TMP_Text FinishTimeText;   // your final time
    public TMP_Text BestTimeText;     // "NEW BEST!" or your best time

    // created once and reused (16 is more than the 8 characters we ever write)
    private readonly char[] _buffer = new char[16];

    private int _lastCentis = -1;
    private int _lastAttempts = -1;
    private bool _finishShown;

    private void Start()
    {
        if (FinishPanel != null)
            FinishPanel.SetActive(false);
    }

    private void Update()
    {
        LevelManager level = LevelManager.Instance;
        if (level == null) return;

        // The timer: only rewrite the text when the visible hundredths change
        int centis = (int)(level.LevelTime * 100f);

        if (centis != _lastCentis && TimerText != null)
        {
            _lastCentis = centis;

            int length = WriteTime(centis);
            TimerText.SetCharArray(_buffer, 0, length);
        }

        // The attempt number changes rarely, so a normal string is fine here
        RespawnManager respawn = RespawnManager.Instance;

        if (respawn != null && respawn.Attempts != _lastAttempts && AttemptsText != null)
        {
            _lastAttempts = respawn.Attempts;
            AttemptsText.text = "Attempt " + _lastAttempts;
        }

        if (level.IsFinished && !_finishShown)
            ShowFinish(level);
    }

    private void ShowFinish(LevelManager level)
    {
        _finishShown = true;

        if (FinishPanel != null)
            FinishPanel.SetActive(true);

        if (FinishTimeText != null)
        {
            int length = WriteTime((int)(level.LevelTime * 100f));
            FinishTimeText.SetCharArray(_buffer, 0, length);
        }

        if (BestTimeText != null)
        {
            if (level.IsNewBest)
            {
                BestTimeText.text = "NEW BEST!";
            }
            else
            {
                // happens once, so one small string is fine
                int length = WriteTime((int)(level.BestTime * 100f));
                BestTimeText.text = "Best " + new string(_buffer, 0, length);
            }
        }
    }

    // Writes a time like "0:12.34" (or "12:05.07") into the buffer and returns its length.
    // It builds the digits by hand: no string formatting, so no garbage.
    private int WriteTime(int totalCentis)
    {
        if (totalCentis < 0) totalCentis = 0;

        int minutes = totalCentis / 6000;
        int seconds = (totalCentis / 100) % 60;
        int centis = totalCentis % 100;

        if (minutes > 99) minutes = 99;

        int i = 0;

        if (minutes >= 10)
            _buffer[i++] = (char)('0' + minutes / 10);

        _buffer[i++] = (char)('0' + minutes % 10);
        _buffer[i++] = ':';
        _buffer[i++] = (char)('0' + seconds / 10);
        _buffer[i++] = (char)('0' + seconds % 10);
        _buffer[i++] = '.';
        _buffer[i++] = (char)('0' + centis / 10);
        _buffer[i++] = (char)('0' + centis % 10);

        return i;
    }
}