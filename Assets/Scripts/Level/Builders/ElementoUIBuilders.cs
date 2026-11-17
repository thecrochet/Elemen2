#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// EDITOR ONLY. One click UI prefabs.
//   GameObject > Elemento > UI > Level UI        Canvas + HUD + Pause menu + Finish panel + Screen fade (+ EventSystem if the scene has none)
//   GameObject > Elemento > UI > Control Prompt  a hint that shows when the player enters a box
// After building, drag the result into Assets/Prefabs/UI and use the PREFAB in your levels.
public static class ElementoUIBuilders
{
    // palette (the same neon as the game)
    private static readonly Color Dim = new Color(0.02f, 0.03f, 0.06f, 0.78f);
    private static readonly Color Panel = new Color(0.04f, 0.07f, 0.13f, 0.95f);
    private static readonly Color Neon = new Color(0.35f, 0.95f, 1f, 1f);
    private static readonly Color White = new Color(0.92f, 0.97f, 1f, 1f);

    // ------------------------------------------------------------------
    // helpers
    // ------------------------------------------------------------------
    private static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static RectTransform Stretch(GameObject go)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static RectTransform Place(GameObject go, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static Image NewImage(string name, Transform parent, Color color)
    {
        GameObject go = NewUI(name, parent);
        Image image = go.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI NewText(string name, Transform parent, string text, float size, Color color,
                                           TextAlignmentOptions alignment, FontStyles style = FontStyles.Normal)
    {
        GameObject go = NewUI(name, parent);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.fontStyle = style;
        tmp.raycastTarget = false;
        return tmp;
    }

    private static Button NewButton(string name, Transform parent, string label)
    {
        GameObject go = NewUI(name, parent);
        Image image = go.AddComponent<Image>();
        image.color = Panel;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;

        // visible highlight for mouse AND for keyboard / gamepad selection
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 1f);
        colors.highlightedColor = new Color(0.55f, 1f, 1f, 1f);
        colors.selectedColor = new Color(0.35f, 0.95f, 1f, 1f);
        colors.pressedColor = new Color(0.2f, 0.7f, 0.8f, 1f);
        colors.fadeDuration = 0.05f;
        button.colors = colors;

        LayoutElement layout = go.AddComponent<LayoutElement>();
        layout.preferredWidth = 420f;
        layout.preferredHeight = 84f;

        TextMeshProUGUI text = NewText("Label", go.transform, label, 40f, Neon, TextAlignmentOptions.Center, FontStyles.Bold);
        Stretch(text.gameObject);

        return button;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;

        GameObject go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
    }

    // ------------------------------------------------------------------
    // LEVEL UI
    // ------------------------------------------------------------------
    [MenuItem("GameObject/Elemento/UI/Level UI", false, 60)]
    private static void BuildLevelUI(MenuCommand command)
    {
        GameObject root = new GameObject("LevelUI", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(root, "Create LevelUI");

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        root.AddComponent<GraphicRaycaster>();

        // ---- HUD ----
        GameObject hud = NewUI("HUD", root.transform);
        Stretch(hud);

        TextMeshProUGUI timer = NewText("TimerText", hud.transform, "0:00.00", 72f, White, TextAlignmentOptions.Center, FontStyles.Bold);
        Place(timer.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(500f, 96f));

        TextMeshProUGUI attempts = NewText("AttemptsText", hud.transform, "Attempt 1", 36f, Neon, TextAlignmentOptions.Right);
        Place(attempts.gameObject, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(400f, 56f));

        // ---- Pause panel (starts hidden, PauseMenu hides it in Start) ----
        GameObject pausePanel = NewUI("PausePanel", root.transform);
        Stretch(pausePanel);
        Image pauseDim = pausePanel.AddComponent<Image>();
        pauseDim.color = Dim;

        TextMeshProUGUI pauseTitle = NewText("Title", pausePanel.transform, "PAUSED", 96f, Neon, TextAlignmentOptions.Center, FontStyles.Bold);
        Place(pauseTitle.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 280f), new Vector2(900f, 130f));

        GameObject buttons = NewUI("Buttons", pausePanel.transform);
        Place(buttons, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(460f, 400f));
        VerticalLayoutGroup list = buttons.AddComponent<VerticalLayoutGroup>();
        list.spacing = 24f;
        list.childAlignment = TextAnchor.MiddleCenter;
        list.childControlWidth = true;
        list.childControlHeight = true;
        list.childForceExpandWidth = false;
        list.childForceExpandHeight = false;

        Button resume = NewButton("ResumeButton", buttons.transform, "RESUME");
        Button restart = NewButton("RestartButton", buttons.transform, "RESTART LEVEL");
        Button quit = NewButton("QuitButton", buttons.transform, "QUIT");

        // ---- Finish panel (starts hidden, LevelHUD hides it in Start) ----
        GameObject finishPanel = NewUI("FinishPanel", root.transform);
        Stretch(finishPanel);
        Image finishDim = finishPanel.AddComponent<Image>();
        finishDim.color = Dim;

        TextMeshProUGUI finishTitle = NewText("Title", finishPanel.transform, "LEVEL COMPLETE", 96f, Neon, TextAlignmentOptions.Center, FontStyles.Bold);
        Place(finishTitle.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 150f), new Vector2(1400f, 130f));

        TextMeshProUGUI finishTime = NewText("FinishTimeText", finishPanel.transform, "0:12.34", 110f, White, TextAlignmentOptions.Center, FontStyles.Bold);
        Place(finishTime.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(900f, 150f));

        TextMeshProUGUI bestTime = NewText("BestTimeText", finishPanel.transform, "NEW BEST!", 56f, Neon, TextAlignmentOptions.Center);
        Place(bestTime.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(900f, 80f));

        // ---- Screen fade (always the LAST child so it is drawn on top of everything) ----
        GameObject fade = NewUI("ScreenFade", root.transform);
        Stretch(fade);
        Image fadeImage = fade.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;
        CanvasGroup fadeGroup = fade.AddComponent<CanvasGroup>();
        fadeGroup.blocksRaycasts = false;
        fadeGroup.alpha = 0f;   // invisible while you work, ScreenFade turns it black at Play
        fade.AddComponent<ScreenFade>();

        // ---- the scripts ----
        LevelHUD levelHud = root.AddComponent<LevelHUD>();
        levelHud.TimerText = timer;
        levelHud.AttemptsText = attempts;
        levelHud.FinishPanel = finishPanel;
        levelHud.FinishTimeText = finishTime;
        levelHud.BestTimeText = bestTime;

        PauseMenu pause = root.AddComponent<PauseMenu>();
        pause.PausePanel = pausePanel;
        pause.ResumeButton = resume;

        UnityEventTools.AddPersistentListener(resume.onClick, pause.Resume);
        UnityEventTools.AddPersistentListener(restart.onClick, pause.RestartLevel);
        UnityEventTools.AddPersistentListener(quit.onClick, pause.QuitGame);

        EnsureEventSystem();

        // panels start hidden in play mode (the scripts do it), but hide them now too so the scene view is clean
        pausePanel.SetActive(false);
        finishPanel.SetActive(false);

        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // CONTROL PROMPT
    // ------------------------------------------------------------------
    [MenuItem("GameObject/Elemento/UI/Control Prompt", false, 61)]
    private static void BuildControlPrompt(MenuCommand command)
    {
        GameObject root = new GameObject("ControlPrompt");
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(root, "Create ControlPrompt");

        // the zone: where the hint shows
        BoxCollider zone = root.AddComponent<BoxCollider>();
        zone.isTrigger = true;
        zone.center = new Vector3(0f, 2f, 0f);
        zone.size = new Vector3(8f, 4f, 6f);

        // a small world space canvas above the zone
        GameObject canvasObject = new GameObject("PromptCanvas", typeof(RectTransform));
        canvasObject.transform.SetParent(root.transform, false);
        canvasObject.transform.localPosition = new Vector3(0f, 4.5f, 0f);
        canvasObject.transform.localScale = Vector3.one * 0.02f;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 120f);

        CanvasGroup group = canvasObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;

        Image back = NewImage("Back", canvasObject.transform, new Color(0.02f, 0.04f, 0.08f, 0.8f));
        Stretch(back.gameObject);

        TextMeshProUGUI label = NewText("Label", canvasObject.transform, "SPACE   Jump", 52f, Neon, TextAlignmentOptions.Center, FontStyles.Bold);
        Stretch(label.gameObject);

        ControlPrompt prompt = root.AddComponent<ControlPrompt>();
        prompt.Group = group;

        Selection.activeGameObject = root;
    }
}
#endif
