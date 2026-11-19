#if UNITY_EDITOR
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// TutorialBuilders: editor-only menu entries that build the objects for the tutorial opening, so you can rebuild
// them instead of hand-editing. They are placed at the tutorial values from the Tutorial Level Spec.
//   GameObject > Elemento > Flow > Tutorial Intro          (camera, pivot, black overlay, logo, IntroSequence)
//   GameObject > Elemento > Flow > Grid Reveal Trigger     (the trigger that fades the grid in)
public static class TutorialBuilders
{
    [MenuItem("GameObject/Elemento/Flow/Tutorial Intro", false, 20)]
    private static void CreateTutorialIntro()
    {
        GameObject root = new GameObject("Tutorial_Intro");
        Undo.RegisterCreatedObjectUndo(root, "Create Tutorial Intro");
        IntroSequence sequence = root.AddComponent<IntroSequence>();

        // where the camera orbits: the top centre of the cube (cube centre (-2, -2, 0), top at y = 0)
        GameObject pivot = new GameObject("IntroPivot");
        pivot.transform.SetParent(root.transform, false);
        pivot.transform.position = new Vector3(-2f, 0.5f, 0f);

        // the intro camera: a CinemachineCamera with no Follow or Look At; the script moves its Transform
        GameObject cam = new GameObject("IntroCam");
        cam.transform.SetParent(root.transform, false);
        cam.AddComponent<CinemachineCamera>();

        // the overlay canvas
        GameObject canvasObject = new GameObject("IntroCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        canvasObject.transform.SetParent(root.transform, false);
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        // full-screen black (alpha 0 here so the Game view is usable while building; the script sets 1 in Play mode)
        GameObject black = new GameObject("BlackOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        black.transform.SetParent(canvasObject.transform, false);
        Stretch(black.GetComponent<RectTransform>());
        Image blackImage = black.GetComponent<Image>();
        blackImage.color = Color.black;
        blackImage.raycastTarget = false;
        CanvasGroup blackGroup = black.GetComponent<CanvasGroup>();
        blackGroup.alpha = 0f;
        blackGroup.blocksRaycasts = false;
        blackGroup.interactable = false;

        // the logo group with one Image: assign the Made with Unity sprite to it
        GameObject logoGroupObject = new GameObject("LogoGroup", typeof(RectTransform), typeof(CanvasGroup));
        logoGroupObject.transform.SetParent(canvasObject.transform, false);
        Stretch(logoGroupObject.GetComponent<RectTransform>());
        CanvasGroup logoGroup = logoGroupObject.GetComponent<CanvasGroup>();
        logoGroup.alpha = 0f;
        logoGroup.blocksRaycasts = false;
        logoGroup.interactable = false;

        GameObject logo = new GameObject("Logo", typeof(RectTransform), typeof(Image));
        logo.transform.SetParent(logoGroupObject.transform, false);
        RectTransform logoRect = logo.GetComponent<RectTransform>();
        logoRect.anchorMin = logoRect.anchorMax = new Vector2(0.5f, 0.5f);
        logoRect.sizeDelta = new Vector2(640f, 320f);
        Image logoImage = logo.GetComponent<Image>();
        logoImage.preserveAspect = true;
        logoImage.raycastTarget = false;

        sequence.Player = Object.FindFirstObjectByType<CC_ELEMENTO>();
        sequence.IntroCamera = cam;
        sequence.Pivot = pivot.transform;
        sequence.BlackOverlay = blackGroup;
        sequence.LogoGroup = logoGroup;
        EditorUtility.SetDirty(sequence);

        if (sequence.Player == null)
        {
            Debug.LogWarning("Tutorial Intro: no CC_ELEMENTO was found in the scene, so the Player slot is empty. Drag the Player in.", root);
        }

        Selection.activeGameObject = root;
    }

    [MenuItem("GameObject/Elemento/Flow/Grid Reveal Trigger", false, 21)]
    private static void CreateGridReveal()
    {
        GameObject go = new GameObject("GridReveal");
        Undo.RegisterCreatedObjectUndo(go, "Create Grid Reveal Trigger");
        go.transform.position = new Vector3(26f, 1.5f, 0f);

        BoxCollider box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(2f, 6f, 4f);
        go.AddComponent<GridReveal>();

        Selection.activeGameObject = go;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
#endif
