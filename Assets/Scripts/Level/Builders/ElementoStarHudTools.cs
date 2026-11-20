#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Editor tools for the star HUD. No outside assets: the star picture and the "star lost" sound are DRAWN BY CODE.
//
//   Elemento > UI > Create Star Sprite       draws a white 5 point star and saves it as Assets/Art/UI/Star.png (a Sprite)
//   Elemento > UI > Set Up Star HUD          finds your LevelHUD, puts the star sprite on the PaceStar / FinishStar images,
//                                            fills the three lists and the Next Star Target Text, sets the gold / dim colours.
//                                            Missing icons are created. Icons you already have keep their position and size.
//   Elemento > UI > Create Star Lost Sound   writes a short falling blip, Assets/Audio/Sfx/SFX_StarLost.wav, and puts it on the
//                                            StarLost line of the Sfx object of the open scene (after the Sfx.cs edit of the README).
public static class ElementoStarHudTools
{
    private const string StarFolder = "Assets/Art/UI";
    private const string StarPath = "Assets/Art/UI/Star.png";
    private const string SoundFolder = "Assets/Audio/Sfx";
    private const string SoundPath = "Assets/Audio/Sfx/SFX_StarLost.wav";

    // ------------------------------------------------------------------
    // STAR SPRITE
    // ------------------------------------------------------------------
    [MenuItem("Elemento/UI/Create Star Sprite")]
    public static void CreateStarSprite()
    {
        Sprite sprite = MakeStarSprite();
        if (sprite != null)
        {
            Selection.activeObject = sprite;
            EditorGUIUtility.PingObject(sprite);
            Debug.Log("Star sprite saved: " + StarPath);
        }
    }

    private static Sprite LoadOrMakeStar()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(StarPath);
        return sprite != null ? sprite : MakeStarSprite();
    }

    private static Sprite MakeStarSprite()
    {
        const int size = 128;
        const int samples = 4;     // 4 x 4 sub pixels per pixel = smooth edges

        // the ten corners: a point at the top, then inner, outer, inner...
        Vector2[] poly = new Vector2[10];
        const float outer = 0.47f;
        const float inner = 0.20f;

        for (int i = 0; i < 10; i++)
        {
            float angle = Mathf.PI * 0.5f + i * Mathf.PI / 5f;
            float radius = (i % 2 == 0) ? outer : inner;
            poly[i] = new Vector2(0.5f + Mathf.Cos(angle) * radius, 0.45f + Mathf.Sin(angle) * radius);
        }

        Color32[] pixels = new Color32[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int hit = 0;

                for (int sy = 0; sy < samples; sy++)
                {
                    for (int sx = 0; sx < samples; sx++)
                    {
                        Vector2 p = new Vector2((x + (sx + 0.5f) / samples) / size, (y + (sy + 0.5f) / samples) / size);
                        if (Inside(poly, p)) hit++;
                    }
                }

                byte alpha = (byte)Mathf.RoundToInt(255f * hit / (samples * samples));
                pixels[y * size + x] = new Color32(255, 255, 255, alpha);
            }
        }

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply();

        EnsureFolder(StarFolder);
        File.WriteAllBytes(StarPath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(StarPath);
        TextureImporter importer = AssetImporter.GetAtPath(StarPath) as TextureImporter;

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(StarPath);
    }

    // even-odd point in polygon
    private static bool Inside(Vector2[] poly, Vector2 p)
    {
        bool inside = false;

        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            bool crosses = (poly[i].y > p.y) != (poly[j].y > p.y);

            if (crosses)
            {
                float xAt = (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x;
                if (p.x < xAt) inside = !inside;
            }
        }

        return inside;
    }

    // ------------------------------------------------------------------
    // SET UP THE HUD
    // ------------------------------------------------------------------
    [MenuItem("Elemento/UI/Set Up Star HUD")]
    public static void SetUpStarHud()
    {
        LevelHUD hud = null;

        if (Selection.activeGameObject != null)
            hud = Selection.activeGameObject.GetComponentInParent<LevelHUD>();

        if (hud == null)
            hud = Object.FindFirstObjectByType<LevelHUD>();

        if (hud == null)
        {
            EditorUtility.DisplayDialog("Star HUD", "No LevelHUD in the open scene. Open a level scene (it is on LevelUI).", "OK");
            return;
        }

        Sprite star = LoadOrMakeStar();

        Transform paceParent = hud.TimerText != null ? hud.TimerText.transform.parent : hud.transform;
        Image[] pace = Collect(paceParent, "PaceStar", 3, new Vector2(0.5f, 1f), new Vector2(-44f, -90f), 44f, 36f, star);

        Image[] finish = null;
        TMP_Text next = null;

        if (hud.FinishPanel != null)
        {
            finish = Collect(hud.FinishPanel.transform, "FinishStar", 3, new Vector2(0.5f, 0.5f), new Vector2(-76f, 20f), 76f, 64f, star);

            Transform nextTransform = hud.FinishPanel.transform.Find("NextStarText");
            if (nextTransform != null) next = nextTransform.GetComponent<TMP_Text>();

            if (next != null)
            {
                Undo.RecordObject(next, "Next star text");
                Undo.RecordObject(next.rectTransform, "Next star text");

                // wide enough for "3 stars: 0:07.00 or less" on ONE line, centred
                next.alignment = TextAlignmentOptions.Center;
                next.overflowMode = TextOverflowModes.Overflow;
                next.rectTransform.sizeDelta = new Vector2(700f, Mathf.Max(next.rectTransform.sizeDelta.y, 50f));
                EditorUtility.SetDirty(next);
            }
            else
            {
                Debug.LogWarning("Star HUD: no child called NextStarText under the FinishPanel, so that field stays empty.");
            }
        }

        Undo.RecordObject(hud, "Set up star HUD");
        hud.PaceStarIcons = pace;
        hud.FinishStarIcons = finish;
        hud.NextStarTargetText = next;
        hud.LitStarColor = new Color(1f, 0.84f, 0.2f, 1f);
        hud.DimStarColor = new Color(0.35f, 0.35f, 0.35f, 0.45f);
        EditorUtility.SetDirty(hud);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(hud.gameObject.scene);
        Debug.Log("Star HUD set up: " + pace.Length + " pace stars, " + (finish != null ? finish.Length : 0) + " finish stars. Press Play to see them.");
    }

    // Finds the child images that start with "prefix" (in hierarchy order). Creates them if there are none.
    private static Image[] Collect(Transform parent, string prefix, int count, Vector2 anchor, Vector2 firstPosition, float step, float size, Sprite star)
    {
        System.Collections.Generic.List<Image> found = new System.Collections.Generic.List<Image>();

        for (int i = 0; i < parent.childCount && found.Count < count; i++)
        {
            Transform child = parent.GetChild(i);
            if (!child.name.StartsWith(prefix)) continue;

            Image image = child.GetComponent<Image>();
            if (image == null) image = Undo.AddComponent<Image>(child.gameObject);
            found.Add(image);
        }

        // none yet: create them in a row
        if (found.Count == 0)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = new GameObject(prefix + (i + 1), typeof(RectTransform), typeof(Image));
                Undo.RegisterCreatedObjectUndo(go, "Create star icon");
                go.transform.SetParent(parent, false);

                RectTransform rect = (RectTransform)go.transform;
                rect.anchorMin = anchor;
                rect.anchorMax = anchor;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = firstPosition + new Vector2(step * i, 0f);

                found.Add(go.GetComponent<Image>());
            }
        }

        for (int i = 0; i < found.Count; i++)
        {
            Undo.RecordObject(found[i], "Star sprite");
            found[i].sprite = star;
            found[i].preserveAspect = true;
            found[i].raycastTarget = false;
            found[i].color = Color.white;
            EditorUtility.SetDirty(found[i]);
        }

        return found.ToArray();
    }

    // ------------------------------------------------------------------
    // THE "STAR LOST" SOUND (a falling blip, made by code)
    // ------------------------------------------------------------------
    [MenuItem("Elemento/UI/Create Star Lost Sound")]
    public static void CreateStarLostSound()
    {
        const int rate = 44100;
        const float duration = 0.32f;
        int count = (int)(rate * duration);

        EnsureFolder(SoundFolder);

        using (FileStream stream = new FileStream(SoundPath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int dataBytes = count * 2;

            writer.Write(new[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + dataBytes);
            writer.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);          // PCM
            writer.Write((short)1);          // mono
            writer.Write(rate);
            writer.Write(rate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(new[] { 'd', 'a', 't', 'a' });
            writer.Write(dataBytes);

            float phase = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)rate;
                float k = t / duration;

                // the pitch falls from 740 Hz to 330 Hz, the volume fades out: "you lost something"
                float frequency = Mathf.Lerp(740f, 330f, k);
                phase += 2f * Mathf.PI * frequency / rate;

                float envelope = Mathf.Min(1f, t / 0.006f) * Mathf.Exp(-5f * k);
                float sample = (Mathf.Sin(phase) * 0.65f + Mathf.Sin(phase * 2f) * 0.15f) * envelope;

                writer.Write((short)(Mathf.Clamp(sample, -1f, 1f) * 30000f));
            }
        }

        AssetDatabase.ImportAsset(SoundPath);
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath);

        Sfx sfx = Object.FindFirstObjectByType<Sfx>();
        bool assigned = false;

        if (sfx != null && sfx.Sounds != null)
        {
            for (int i = 0; i < sfx.Sounds.Length; i++)
            {
                if (sfx.Sounds[i] != null && sfx.Sounds[i].Type == SfxType.StarLost)
                {
                    Undo.RecordObject(sfx, "Star lost sound");
                    sfx.Sounds[i].Clips = new[] { clip };
                    sfx.Sounds[i].Volume = 0.7f;
                    EditorUtility.SetDirty(sfx);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sfx.gameObject.scene);
                    assigned = true;
                    break;
                }
            }
        }

        Selection.activeObject = clip;
        EditorGUIUtility.PingObject(clip);

        if (assigned)
            Debug.Log("Star lost sound saved (" + SoundPath + ") and put on the StarLost line of the Sfx object.");
        else
            Debug.Log("Star lost sound saved: " + SoundPath + ". No StarLost line found on the Sfx object yet: click the Sfx object once so the list adds it, then run this menu again (or drag the clip in by hand).");
    }

    // ------------------------------------------------------------------
    private static void EnsureFolder(string path)
    {
        string[] parts = path.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
#endif
