using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Stars (all optional)")]
    // 3 icons next to the timer: all lit at the start, they dim as the clock passes each limit
    public Image[] PaceStarIcons;

    // 3 icons on the finish panel: the stars you earned are lit
    public Image[] FinishStarIcons;

    // for example "2 stars: 12.00 or less" (shows the next star to chase)
    public TMP_Text NextStarTargetText;

    public Color LitStarColor = Color.white;
    public Color DimStarColor = new Color(1f, 1f, 1f, 0.2f);

    // created once and reused (16 is more than the 8 characters we ever write)
    private readonly char[] _buffer = new char[16];

    private int _lastCentis = -1;
    private int _lastAttempts = -1;
    private bool _finishShown;
    private int _paceStars = -1;
    private bool _paceHidden;

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

        UpdatePaceStars(level);

        // the exit animation asks the HUD to wait (HoldFinishPanel) until the ball has dived into the portal
        if (level.IsFinished && !_finishShown && !level.HoldFinishPanel)
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

        ShowFinishStars(level);

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

    // Pace stars: the icon colours are only rewritten when the star bracket changes (no per-frame work).
    private void UpdatePaceStars(LevelManager level)
    {
        if (PaceStarIcons == null || PaceStarIcons.Length == 0) return;

        if (!level.EnableStars)
        {
            if (!_paceHidden)
            {
                _paceHidden = true;
                SetIconsActive(PaceStarIcons, false);
            }
            return;
        }

        int stars = level.GetStarsForTime(level.LevelTime);
        if (stars == _paceStars) return;

        // a star was lost (3 to 2, 2 to 1, 1 to 0): a short falling blip. Gaining one back after a respawn is silent.
        if (_paceStars >= 0 && stars < _paceStars)
            Sfx.Play(SfxType.StarLost);

        _paceStars = stars;
        ColorIcons(PaceStarIcons, stars);
    }

    private void ShowFinishStars(LevelManager level)
    {
        if (!level.EnableStars)
        {
            SetIconsActive(FinishStarIcons, false);
            if (NextStarTargetText != null) NextStarTargetText.gameObject.SetActive(false);
            return;
        }

        ColorIcons(FinishStarIcons, level.Stars);

        if (NextStarTargetText == null) return;

        if (level.TryGetNextStarTime(level.Stars, out float target))
        {
            int length = WriteTime(Mathf.RoundToInt(target * 100f));
            NextStarTargetText.text = (level.Stars + 1) + (level.Stars == 0 ? " star: " : " stars: ")
                + new string(_buffer, 0, length) + " or less";
        }
        else
        {
            NextStarTargetText.text = "3 STARS";
        }
    }

    private void ColorIcons(Image[] icons, int lit)
    {
        if (icons == null) return;

        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] != null)
                icons[i].color = i < lit ? LitStarColor : DimStarColor;
        }
    }

    private static void SetIconsActive(Image[] icons, bool active)
    {
        if (icons == null) return;

        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] != null)
                icons[i].gameObject.SetActive(active);
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