using System.IO;
using UnityEditor;
using UnityEngine;

public static class BlueBlockPrefabCreator
{
    private const string YellowDashWallPath = "Assets/PREFABS/LEVEL/Interactables/DashWall.prefab";
    private const string YellowDebrisPath = "Assets/PREFABS/LEVEL/Interactables/DebrisCube_Yellow.prefab";
    private const string BlueMaterialPath = "Assets/ART/Materials/Level/M_Level_Blue_Touch.mat";
    private const string CyanSourceMaterialPath = "Assets/ART/Materials/Level/M_Level_Cyan.mat";
    private const string BlueDebrisPath = "Assets/PREFABS/LEVEL/Interactables/DebrisCube_Blue.prefab";
    private const string BlueTouchWallPath = "Assets/PREFABS/LEVEL/Interactables/TouchWall_Blue.prefab";

    [MenuItem("Elemento/Setup Blue TouchWall & Shorten DashWall Hitstop")]
    public static void Setup()
    {
        ShortenDashWallHitStop();
        Material blueMat = EnsureBlueMaterial();
        GameObject blueDebrisPrefab = CreateBlueDebrisPrefab(blueMat);
        CreateTouchWallBluePrefab(blueMat, blueDebrisPrefab);
        UpdateTouchBlocksToPurple();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("BlueBlockPrefabCreator: Setup complete! DashWall hitstop shortened, DebrisCube_Blue and TouchWall_Blue created & made purple.");
    }

    [MenuItem("Elemento/Update Touch Blocks To Purple")]
    public static void UpdateTouchBlocksToPurple()
    {
        // 1. Update M_Level_Blue_Touch.mat to glowing neon purple matching M_Level_Purple / Purple Ball
        Material touchMat = AssetDatabase.LoadAssetAtPath<Material>(BlueMaterialPath);
        if (touchMat == null)
        {
            touchMat = EnsureBlueMaterial();
        }

        if (touchMat != null)
        {
            // Exact Purple Ball HDR hue from M_Level_Purple.mat
            if (touchMat.HasProperty("_EdgeColor"))
                touchMat.SetColor("_EdgeColor", new Color(1.75f, 0.0f, 2.8f, 1.0f));
            if (touchMat.HasProperty("_GridColor"))
                touchMat.SetColor("_GridColor", new Color(0.4375f, 0.0f, 0.7f, 1.0f));
            if (touchMat.HasProperty("_Line_Color"))
                touchMat.SetColor("_Line_Color", new Color(1.75f, 0.0f, 2.8f, 1.0f));
            if (touchMat.HasProperty("_FillColor"))
                touchMat.SetColor("_FillColor", new Color(0.009375f, 0.0f, 0.015f, 1.0f));
            if (touchMat.HasProperty("_BaseColor"))
                touchMat.SetColor("_BaseColor", Color.white);
            if (touchMat.HasProperty("_Color"))
                touchMat.SetColor("_Color", Color.white);

            EditorUtility.SetDirty(touchMat);
            Debug.Log("[UpdateTouchBlocksToPurple] Updated M_Level_Blue_Touch.mat to neon purple.");
        }

        // 2. Update DebrisCube_Blue.prefab
        GameObject debrisPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BlueDebrisPath);
        if (debrisPrefab != null)
        {
            MeshRenderer mr = debrisPrefab.GetComponent<MeshRenderer>();
            if (mr != null && touchMat != null)
            {
                mr.sharedMaterial = touchMat;
                EditorUtility.SetDirty(debrisPrefab);
                PrefabUtility.SavePrefabAsset(debrisPrefab);
                Debug.Log("[UpdateTouchBlocksToPurple] Updated DebrisCube_Blue.prefab material.");
            }
        }

        // 3. Update TouchWall_Blue.prefab
        GameObject wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BlueTouchWallPath);
        if (wallPrefab != null)
        {
            Transform visual = wallPrefab.transform.Find("Visual");
            if (visual != null)
            {
                MeshRenderer mr = visual.GetComponent<MeshRenderer>();
                if (mr != null && touchMat != null)
                {
                    mr.sharedMaterial = touchMat;
                }
            }

            BreakableWallFeedback feedback = wallPrefab.GetComponent<BreakableWallFeedback>();
            if (feedback != null)
            {
                SerializedObject so = new SerializedObject(feedback);
                so.Update();

                if (debrisPrefab != null)
                    so.FindProperty("debrisCubePrefab").objectReferenceValue = debrisPrefab;

                if (touchMat != null)
                    so.FindProperty("debrisMaterial").objectReferenceValue = touchMat;

                // Zero hitstop for smooth rolling
                so.FindProperty("hitStopDuration").floatValue = 0f;
                so.FindProperty("hitStopTimeScale").floatValue = 0f;

                // Purple VFX colors matching Purple Ball
                so.FindProperty("sparkHdrColorMin").colorValue = new Color(1.5f, 0.0f, 2.5f, 1f);
                so.FindProperty("sparkHdrColorMax").colorValue = new Color(2.0f, 0.2f, 2.8f, 1f);
                so.FindProperty("shockwaveColor").colorValue = new Color(1.8f, 0.1f, 2.8f, 1f);
                so.FindProperty("blueSparkHdrColorMin").colorValue = new Color(1.5f, 0.0f, 2.5f, 1f);
                so.FindProperty("blueSparkHdrColorMax").colorValue = new Color(2.0f, 0.2f, 2.8f, 1f);

                SerializedProperty wispGradProp = so.FindProperty("blueSparkTwinkleGradient");
                if (wispGradProp != null)
                {
                    Gradient wispGrad = new Gradient();
                    wispGrad.SetKeys(
                        new GradientColorKey[]
                        {
                            new GradientColorKey(new Color(1.8f, 0.1f, 2.8f), 0.0f),
                            new GradientColorKey(new Color(1.2f, 0.05f, 2.2f), 0.5f),
                            new GradientColorKey(new Color(0.6f, 0.02f, 1.2f), 1.0f)
                        },
                        new GradientAlphaKey[]
                        {
                            new GradientAlphaKey(0.0f, 0.0f),
                            new GradientAlphaKey(1.0f, 0.2f),
                            new GradientAlphaKey(0.8f, 0.7f),
                            new GradientAlphaKey(0.0f, 1.0f)
                        }
                    );
                    wispGradProp.gradientValue = wispGrad;
                }

                // Neon purple fireball gradient
                SerializedProperty gradProp = so.FindProperty("fireballGradient");
                if (gradProp != null)
                {
                    Gradient purpleGrad = new Gradient();
                    purpleGrad.SetKeys(
                        new GradientColorKey[]
                        {
                            new GradientColorKey(new Color(1.8f, 0.1f, 2.8f), 0.0f),
                            new GradientColorKey(new Color(1.2f, 0.05f, 2.2f), 0.35f),
                            new GradientColorKey(new Color(0.5f, 0.02f, 1.2f), 0.70f),
                            new GradientColorKey(new Color(0.1f, 0.0f, 0.3f), 1.0f)
                        },
                        new GradientAlphaKey[]
                        {
                            new GradientAlphaKey(1.0f, 0.0f),
                            new GradientAlphaKey(0.85f, 0.6f),
                            new GradientAlphaKey(0.0f, 1.0f)
                        }
                    );
                    gradProp.gradientValue = purpleGrad;
                }

                so.ApplyModifiedProperties();
            }

            EditorUtility.SetDirty(wallPrefab);
            PrefabUtility.SavePrefabAsset(wallPrefab);
            Debug.Log("[UpdateTouchBlocksToPurple] Updated TouchWall_Blue.prefab to neon purple visuals & VFX.");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[UpdateTouchBlocksToPurple] Completed! TouchWall_Blue and DebrisCube_Blue are now neon purple.");
    }

    [MenuItem("Elemento/Disable Debug Overlay Across FINAL_PURPLE")]
    public static void DisableDebugOverlayAcrossAll()
    {
        // 1. Update LevelCore prefab
        GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Managers/LevelCore.prefab");
        if (corePrefab != null)
        {
            var debugOverlays = corePrefab.GetComponentsInChildren<DebugOverlay>(true);
            foreach (var d in debugOverlays)
            {
                d.StartVisible = false;
                d.enabled = false;
                d.gameObject.SetActive(false);
                EditorUtility.SetDirty(d);
            }

            var playerLabs = corePrefab.GetComponentsInChildren<PlayerLab>(true);
            foreach (var lab in playerLabs)
            {
                lab.ShowPanel = false;
                lab.MixPowers = false;
                lab.enabled = false;
                lab.gameObject.SetActive(false);
                EditorUtility.SetDirty(lab);
            }

            Transform jp = corePrefab.transform.Find("JumpProbe");
            if (jp != null) jp.gameObject.SetActive(false);

            Transform dgo = corePrefab.transform.Find("DebugOverlay");
            if (dgo != null) dgo.gameObject.SetActive(false);

            PrefabUtility.SavePrefabAsset(corePrefab);
            Debug.Log("[DisableDebugOverlay] Saved changes to LevelCore.prefab");
        }

        // 2. Batch update every scene in FINAL_PURPLE
        string dir = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE";
        if (Directory.Exists(dir))
        {
            string[] sceneFiles = Directory.GetFiles(dir, "Level_*.unity", SearchOption.TopDirectoryOnly);
            foreach (string scenePath in sceneFiles)
            {
                string normPath = scenePath.Replace('\\', '/');
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(normPath, UnityEditor.SceneManagement.OpenSceneMode.Single);

                bool modified = false;

                foreach (var d in Object.FindObjectsByType<DebugOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    d.StartVisible = false;
                    d.enabled = false;
                    d.gameObject.SetActive(false);
                    EditorUtility.SetDirty(d);
                    modified = true;
                }

                foreach (var lab in Object.FindObjectsByType<PlayerLab>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    lab.ShowPanel = false;
                    lab.MixPowers = false;
                    lab.enabled = false;
                    lab.gameObject.SetActive(false);
                    EditorUtility.SetDirty(lab);
                    modified = true;
                }

                GameObject jp = GameObject.Find("JumpProbe");
                if (jp != null && jp.activeSelf)
                {
                    jp.SetActive(false);
                    EditorUtility.SetDirty(jp);
                    modified = true;
                }

                GameObject dgo = GameObject.Find("DebugOverlay");
                if (dgo != null && dgo.activeSelf)
                {
                    dgo.SetActive(false);
                    EditorUtility.SetDirty(dgo);
                    modified = true;
                }

                if (modified)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                    Debug.Log($"[DisableDebugOverlay] Disabled overlay in {Path.GetFileName(normPath)}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[DisableDebugOverlay] Completed disabling debug overlay across all scenes.");
    }

    [MenuItem("Elemento/Validate Levels 16 to 20")]
    public static void ValidateLevels16To20()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_16.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_17.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_18.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_19.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_20.unity"
        };

        int totalErrors = 0;
        foreach (string path in scenes)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
            int errs = ElementoLevelValidator.Run();
            Debug.Log($"[ValidateBatch] {System.IO.Path.GetFileName(path)}: {errs} errors");
            totalErrors += errs;
        }

        Debug.Log($"[ValidateBatch] COMPLETED VALIDATION: {totalErrors} total errors.");
    }

    [MenuItem("Elemento/Validate Levels 21 to 25")]
    public static void ValidateLevels21To25()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_21.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_22.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_23.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_24.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_25.unity"
        };

        int totalErrors = 0;
        foreach (string path in scenes)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
            int errs = ElementoLevelValidator.Run();
            Debug.Log($"[ValidateBatch] {System.IO.Path.GetFileName(path)}: {errs} errors");
            totalErrors += errs;
        }

        Debug.Log($"[ValidateBatch] COMPLETED VALIDATION: {totalErrors} total errors.");
    }

    [MenuItem("Elemento/Validate Levels 26 to 30")]
    public static void ValidateLevels26To30()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_26.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_27.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_28.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_29.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_30.unity"
        };

        int totalErrors = 0;
        foreach (string path in scenes)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
            int errs = ElementoLevelValidator.Run();
            Debug.Log($"[ValidateBatch] {System.IO.Path.GetFileName(path)}: {errs} errors");
            totalErrors += errs;
        }

        Debug.Log($"[ValidateBatch] COMPLETED VALIDATION: {totalErrors} total errors.");
    }

    [MenuItem("Elemento/Validate Levels 31 to 35")]
    public static void ValidateLevels31To35()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_31.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_32.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_33.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_34.unity",
            "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_35.unity"
        };

        int totalErrors = 0;
        foreach (string path in scenes)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
            int errs = ElementoLevelValidator.Run();
            Debug.Log($"[ValidateBatch] {System.IO.Path.GetFileName(path)}: {errs} errors");
            totalErrors += errs;
        }

        Debug.Log($"[ValidateBatch] COMPLETED VALIDATION: {totalErrors} total errors.");
    }

    [MenuItem("Elemento/Restore Purple Ball Abilities")]
    public static void RestorePurpleBallAbilities()
    {
        // 1. Update Player.prefab
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Player/Player.prefab");
        if (playerPrefab != null)
        {
            var cc = playerPrefab.GetComponent<CC_ELEMENTO>();
            if (cc != null)
            {
                cc.DefaultDash = true;
                cc.DefaultDoubleJump = true;
                cc.DefaultWallJump = true;
                cc.DefaultGroundPound = true;
                cc.DefaultGlide = true;
                cc.GlideNeedsNewPress = false;
                cc.StartingPower = PowerUpType.Aether;
                EditorUtility.SetDirty(cc);
                PrefabUtility.SavePrefabAsset(playerPrefab);
                Debug.Log("[RestorePurpleBallAbilities] Configured CC_ELEMENTO on Player.prefab");
            }
        }

        // 2. Update LevelCore.prefab
        GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PREFABS/Managers/LevelCore.prefab");
        if (corePrefab != null)
        {
            var ccList = corePrefab.GetComponentsInChildren<CC_ELEMENTO>(true);
            foreach (var cc in ccList)
            {
                cc.DefaultDash = true;
                cc.DefaultDoubleJump = true;
                cc.DefaultWallJump = true;
                cc.DefaultGroundPound = true;
                cc.DefaultGlide = true;
                cc.GlideNeedsNewPress = false;
                cc.StartingPower = PowerUpType.Aether;
                EditorUtility.SetDirty(cc);
            }
            PrefabUtility.SavePrefabAsset(corePrefab);
            Debug.Log($"[RestorePurpleBallAbilities] Configured {ccList.Length} CC_ELEMENTO in LevelCore.prefab");
        }

        // 3. Batch verify & update all FINAL_PURPLE scenes
        string dir = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE";
        if (Directory.Exists(dir))
        {
            string[] sceneFiles = Directory.GetFiles(dir, "Level_*.unity", SearchOption.TopDirectoryOnly);
            foreach (string scenePath in sceneFiles)
            {
                string normPath = scenePath.Replace('\\', '/');
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(normPath, UnityEditor.SceneManagement.OpenSceneMode.Single);
                bool modified = false;

                var ccList = Object.FindObjectsByType<CC_ELEMENTO>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var cc in ccList)
                {
                    cc.DefaultDash = true;
                    cc.DefaultDoubleJump = true;
                    cc.DefaultWallJump = true;
                    cc.DefaultGroundPound = true;
                    cc.DefaultGlide = true;
                    cc.GlideNeedsNewPress = false;
                    cc.StartingPower = PowerUpType.Aether;
                    EditorUtility.SetDirty(cc);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(cc);
                    modified = true;
                }

                if (modified)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                    Debug.Log($"[RestorePurpleBallAbilities] Saved abilities in {Path.GetFileName(normPath)}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[RestorePurpleBallAbilities] Completed! Purple Ball abilities restored across all prefabs & scenes.");
    }

    [MenuItem("Elemento/Verify Abilities in Level_01")]
    public static void VerifyAbilitiesInLevel01()
    {
        string scenePath = "Assets/Scenes/Levels/World1_Fire/FINAL_PURPLE/Level_01.unity";
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);

        var cc = Object.FindFirstObjectByType<CC_ELEMENTO>();
        if (cc == null)
        {
            Debug.LogError("[VerifyAbilities] No CC_ELEMENTO found in Level_01!");
            return;
        }

        Debug.Log($"[VerifyAbilities - EditMode] StartingPower={cc.StartingPower}, DefaultDash={cc.DefaultDash}, DefaultDoubleJump={cc.DefaultDoubleJump}, DefaultWallJump={cc.DefaultWallJump}, DefaultGroundPound={cc.DefaultGroundPound}, DefaultGlide={cc.DefaultGlide}");

        // Call SetPower directly to test state resolution
        cc.SetPower(cc.StartingPower);

        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        bool hasDash = (bool)typeof(CC_ELEMENTO).GetField("hasDash", flags).GetValue(cc);
        bool hasDoubleJump = (bool)typeof(CC_ELEMENTO).GetField("hasDoubleJump", flags).GetValue(cc);
        bool hasWallJump = (bool)typeof(CC_ELEMENTO).GetField("hasWallJump", flags).GetValue(cc);
        bool hasGroundPound = (bool)typeof(CC_ELEMENTO).GetField("hasGroundPound", flags).GetValue(cc);
        bool hasGlide = (bool)typeof(CC_ELEMENTO).GetField("hasGlide", flags).GetValue(cc);

        Debug.Log($"[VerifyAbilities - Initialized] CurrentPower={cc.CurrentPower}, hasDash={hasDash}, hasDoubleJump={hasDoubleJump}, hasWallJump={hasWallJump}, hasGroundPound={hasGroundPound}, hasGlide={hasGlide}");

        if (hasDash && hasDoubleJump && hasWallJump && hasGroundPound && hasGlide)
        {
            Debug.Log("[VerifyAbilities] SUCCESS: All Purple Ball powers (Dash, Double Jump, Wall Jump, Ground Pound, Glide) are ACTIVE!");
        }
        else
        {
            Debug.LogError($"[VerifyAbilities] FAILED: Some powers are inactive! hasDash={hasDash}, hasDoubleJump={hasDoubleJump}, hasWallJump={hasWallJump}, hasGroundPound={hasGroundPound}, hasGlide={hasGlide}");
        }
    }

    private static void ShortenDashWallHitStop()
    {
        GameObject dashWallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(YellowDashWallPath);
        if (dashWallPrefab == null)
        {
            Debug.LogError($"Could not find DashWall prefab at {YellowDashWallPath}");
            return;
        }

        BreakableWallFeedback feedback = dashWallPrefab.GetComponent<BreakableWallFeedback>();
        if (feedback != null)
        {
            SerializedObject so = new SerializedObject(feedback);
            SerializedProperty prop = so.FindProperty("hitStopDuration");
            if (prop != null)
            {
                prop.floatValue = 0.02f;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(dashWallPrefab);
                Debug.Log("BlueBlockPrefabCreator: Shortened DashWall.prefab hitStopDuration to 0.02s.");
            }
        }
    }

    private static Material EnsureBlueMaterial()
    {
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(BlueMaterialPath);
        if (existing != null) return existing;

        Material cyanSource = AssetDatabase.LoadAssetAtPath<Material>(CyanSourceMaterialPath);
        Material blueMat;
        if (cyanSource != null)
        {
            blueMat = new Material(cyanSource);
        }
        else
        {
            Shader neonShader = Shader.Find("Elemento/NeonShape");
            if (neonShader == null) neonShader = Shader.Find("Universal Render Pipeline/Lit");
            blueMat = new Material(neonShader);
        }

        blueMat.name = "M_Level_Blue_Touch";
        if (blueMat.HasProperty("_EdgeColor"))
            blueMat.SetColor("_EdgeColor", new Color(0.0f, 2.5f, 3.2f, 1.0f));
        if (blueMat.HasProperty("_GridColor"))
            blueMat.SetColor("_GridColor", new Color(0.0f, 0.8f, 1.2f, 1.0f));
        if (blueMat.HasProperty("_Line_Color"))
            blueMat.SetColor("_Line_Color", new Color(0.0f, 2.2f, 3.5f, 1.0f));
        if (blueMat.HasProperty("_BaseColor"))
            blueMat.SetColor("_BaseColor", Color.white);
        if (blueMat.HasProperty("_Color"))
            blueMat.SetColor("_Color", Color.white);

        AssetDatabase.CreateAsset(blueMat, BlueMaterialPath);
        Debug.Log($"BlueBlockPrefabCreator: Created blue material at {BlueMaterialPath}");
        return blueMat;
    }

    private static GameObject CreateBlueDebrisPrefab(Material blueMat)
    {
        GameObject yellowDebris = AssetDatabase.LoadAssetAtPath<GameObject>(YellowDebrisPath);
        if (yellowDebris == null)
        {
            Debug.LogError($"Could not find Yellow Debris prefab at {YellowDebrisPath}");
            return null;
        }

        // Instantiate in memory
        GameObject instance = Object.Instantiate(yellowDebris);
        instance.name = "DebrisCube_Blue";

        MeshRenderer mr = instance.GetComponent<MeshRenderer>();
        if (mr != null && blueMat != null)
        {
            mr.sharedMaterial = blueMat;
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, BlueDebrisPath);
        Object.DestroyImmediate(instance);

        Debug.Log($"BlueBlockPrefabCreator: Created DebrisCube_Blue prefab at {BlueDebrisPath}");
        return prefab;
    }

    private static void CreateTouchWallBluePrefab(Material blueMat, GameObject blueDebrisPrefab)
    {
        GameObject yellowDashWall = AssetDatabase.LoadAssetAtPath<GameObject>(YellowDashWallPath);
        if (yellowDashWall == null)
        {
            Debug.LogError($"Could not find Yellow DashWall prefab at {YellowDashWallPath}");
            return;
        }

        GameObject instance = Object.Instantiate(yellowDashWall);
        instance.name = "TouchWall_Blue";

        // Update Visual mesh material
        Transform visual = instance.transform.Find("Visual");
        if (visual != null)
        {
            MeshRenderer mr = visual.GetComponent<MeshRenderer>();
            if (mr != null && blueMat != null)
            {
                mr.sharedMaterial = blueMat;
            }
        }

        // Configure DestructibleObject
        DestructibleObject destructible = instance.GetComponent<DestructibleObject>();
        if (destructible != null)
        {
            destructible.BreakableByTouch = true;
            destructible.BreakableByDash = true;
            destructible.BreakableByGroundPound = true;
            destructible.BreakableBySpeed = true;
            destructible.HitsToBreak = 1;
            destructible.SpeedShattersInstantly = true;
            destructible.ChainReaction = true;
        }

        // Configure BreakableWallFeedback
        BreakableWallFeedback feedback = instance.GetComponent<BreakableWallFeedback>();
        if (feedback != null)
        {
            SerializedObject so = new SerializedObject(feedback);
            so.Update();

            if (blueDebrisPrefab != null)
                so.FindProperty("debrisCubePrefab").objectReferenceValue = blueDebrisPrefab;

            if (blueMat != null)
                so.FindProperty("debrisMaterial").objectReferenceValue = blueMat;

            // Zero hitstop for smooth rolling
            so.FindProperty("hitStopDuration").floatValue = 0f;
            so.FindProperty("hitStopTimeScale").floatValue = 0f;

            // Subtle snappy camera kick
            so.FindProperty("shakeForceHorizontal").floatValue = 0.35f;
            so.FindProperty("shakeForceVertical").floatValue = 0.25f;
            so.FindProperty("shakeDuration").floatValue = 0.12f;
            so.FindProperty("cameraShakeCooldown").floatValue = 0.08f;

            // Blue / cyan VFX colors
            so.FindProperty("sparkHdrColorMin").colorValue = new Color(0.0f, 1.8f, 2.2f, 1f);
            so.FindProperty("sparkHdrColorMax").colorValue = new Color(0.2f, 1.2f, 2.5f, 1f);
            so.FindProperty("shockwaveColor").colorValue = new Color(0.1f, 1.8f, 2.8f, 1f);

            // Cyan/blue fireball gradient
            SerializedProperty gradProp = so.FindProperty("fireballGradient");
            if (gradProp != null)
            {
                Gradient cyanGrad = new Gradient();
                cyanGrad.SetKeys(
                    new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(0.1f, 1.8f, 2.5f), 0.0f),
                        new GradientColorKey(new Color(0.05f, 0.9f, 2.0f), 0.35f),
                        new GradientColorKey(new Color(0.02f, 0.2f, 0.8f), 0.70f),
                        new GradientColorKey(new Color(0.0f, 0.05f, 0.2f), 1.0f)
                    },
                    new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(1.0f, 0.0f),
                        new GradientAlphaKey(0.85f, 0.6f),
                        new GradientAlphaKey(0.0f, 1.0f)
                    }
                );
                gradProp.gradientValue = cyanGrad;
            }

            so.ApplyModifiedProperties();
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, BlueTouchWallPath);
        Object.DestroyImmediate(instance);

        Debug.Log($"BlueBlockPrefabCreator: Created TouchWall_Blue prefab at {BlueTouchWallPath}");
    }
}
