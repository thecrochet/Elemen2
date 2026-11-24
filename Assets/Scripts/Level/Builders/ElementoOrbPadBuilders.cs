#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// EDITOR ONLY. Builds the Recharge Orbs and the Earth Hover Pad in the scene. Each menu says what it will create and ASKS FIRST.
//   GameObject > Elemento > Level > Recharge Orb (Fire)
//   GameObject > Elemento > Level > Recharge Orb (Air)
//   GameObject > Elemento > Level > Recharge Orb (Water)
//   GameObject > Elemento > Level > Earth Hover Pad
//
// The materials are only created if they don't exist yet (nothing you made is ever replaced).
// Colors follow the ball and the gates, so an element looks the same everywhere.
// Once it is in the scene, tune it by hand, then drag it into your prefab folder.
public static class ElementoOrbPadBuilders
{
    // ------------------------------------------------------------------
    // THE COLORS (HDR). Same family as the gates and the ball materials.
    // ------------------------------------------------------------------
    private static Color EdgeColor(PowerUpType element)
    {
        switch (element)
        {
            case PowerUpType.Fire: return new Color(3.0f, 1.0f, 0.12f);
            case PowerUpType.Air: return new Color(1.5f, 2.5f, 2.1f);
            case PowerUpType.Water: return new Color(0.4f, 1.7f, 3.0f);
            case PowerUpType.Earth: return new Color(1.9f, 1.1f, 0.5f);
        }
        return Color.white;
    }

    // the inside of the glass: a dark tint of the element (the core uses a brighter one)
    private static Color FillColor(PowerUpType element)
    {
        switch (element)
        {
            case PowerUpType.Fire: return new Color(0.25f, 0.06f, 0f);
            case PowerUpType.Air: return new Color(0.08f, 0.14f, 0.12f);
            case PowerUpType.Water: return new Color(0f, 0.08f, 0.18f);
            case PowerUpType.Earth: return new Color(0.06f, 0.03f, 0.01f);
        }
        return Color.black;
    }

    // the particle colors: the same as Vfx.cs
    private static Color ParticleColor(PowerUpType element)
    {
        switch (element)
        {
            case PowerUpType.Fire: return new Color(1f, 0.45f, 0.1f, 1f);
            case PowerUpType.Air: return new Color(0.8f, 0.95f, 1f, 1f);
            case PowerUpType.Water: return new Color(0.3f, 0.85f, 1f, 1f);
            case PowerUpType.Earth: return new Color(0.8f, 0.5f, 0.25f, 1f);
        }
        return Color.white;
    }

    // ------------------------------------------------------------------
    // MENUS
    // ------------------------------------------------------------------
    [MenuItem("GameObject/Elemento/Level/Recharge Orb (Fire)", false, 14)]
    private static void OrbFire(MenuCommand command) { BuildOrb(PowerUpType.Fire, "an extra DASH until you land", command); }

    [MenuItem("GameObject/Elemento/Level/Recharge Orb (Air)", false, 15)]
    private static void OrbAir(MenuCommand command) { BuildOrb(PowerUpType.Air, "an extra air JUMP (triple jump) until you land", command); }

    [MenuItem("GameObject/Elemento/Level/Recharge Orb (Water)", false, 16)]
    private static void OrbWater(MenuCommand command) { BuildOrb(PowerUpType.Water, "an extra GRAVITY FLIP until you land", command); }

    [MenuItem("GameObject/Elemento/Level/Earth Hover Pad", false, 17)]
    private static void HoverPad(MenuCommand command)
    {
        const PowerUpType e = PowerUpType.Earth;

        bool ok = EditorUtility.DisplayDialog("Earth Hover Pad",
            "This will create, in the scene:\n\n" +
            "- HoverPad_Earth: a hexagon you can stand on. A ground pound shatters it and launches you (the next pad is your next step).\n" +
            "  It holds a solid Mesh Collider, a trigger Box Collider, the NeonShapeMesh hexagon and the EarthHoverPad script.\n" +
            "- FX_PadBreak and FX_PadDebris: copies of your FX_PadLaunchPound and FX_Crumble prefabs, linked to the script (if they are found).\n" +
            "- FX_PadReturn: a small ring flash made by this tool, shown when the pad comes back.\n\n" +
            "And, only if they don't exist yet, this material in your materials folder:\n" +
            "- M_Shape_Earth (NeonShape, amber)\n\n" +
            "Nothing existing is changed. Continue?",
            "Create", "Cancel");

        if (!ok) return;

        Material earth = GetOrMakeMaterial("M_Shape_Earth", EdgeColor(e), FillColor(e), 0.3f, 0.15f);
        if (earth == null) return;

        GameObject root = new GameObject("HoverPad_Earth");
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(root, "Create Earth Hover Pad");

        MeshRenderer meshRenderer = root.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = earth;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        NeonShapeMesh shape = root.AddComponent<NeonShapeMesh>();
        shape.Shape = NeonShapeMesh.ShapeType.Prism;
        shape.Sides = 6;
        shape.Radius = 1.5f;
        shape.Height = 0.4f;
        shape.FitBoxCollider = false;   // the Box Collider below is the TRIGGER: it must not be resized to the hexagon

        // the solid pad the ball stands on
        MeshCollider solid = root.AddComponent<MeshCollider>();
        solid.convex = true;

        // the trigger: reaches 0.4 above the top face (the top is at 0.4)
        BoxCollider trigger = root.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(2.4f, 0.7f, 1.8f);
        trigger.center = new Vector3(0f, 0.45f, 0f);

        EarthHoverPad pad = root.AddComponent<EarthHoverPad>();

        shape.Build();

        // the effects
        pad.BreakEffect = AddPrefabParticles("FX_PadLaunchPound", "FX_PadBreak", root.transform);
        pad.DebrisEffect = AddPrefabParticles("FX_Crumble", "FX_PadDebris", root.transform);

        Material ringMaterial = ElementoMaterials.Find("P_Ring", "Elemento/NeonParticle");
        pad.ReturnEffect = MakeRingFlash(root.transform, "FX_PadReturn", ringMaterial, ParticleColor(e), 0.4f, 3f, new Vector3(0f, 0.45f, 0f));

        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // THE ORB
    // ------------------------------------------------------------------
    private static void BuildOrb(PowerUpType element, string gives, MenuCommand command)
    {
        string e = element.ToString();

        bool ok = EditorUtility.DisplayDialog("Recharge Orb (" + e + ")",
            "This will create, in the scene:\n\n" +
            "- Orb_" + e + ": gives " + gives + " (only when the ball has " + e + "). A trigger Sphere Collider (radius 0.8) and the RechargeOrb script.\n" +
            "- Visual > Core (a glowing disc) and Ring (a hollow ring): both made with NeonShapeMesh.\n" +
            "- FX_RechargeUse (a burst when taken) and FX_RechargeReturn (a small ring flash when it comes back): ParticleSystems made by this tool, linked to the script.\n\n" +
            "And, only if they don't exist yet, these materials in your materials folder:\n" +
            "- M_Orb_" + e + "_Core and M_Orb_" + e + "_Ring (NeonShape)\n\n" +
            "Nothing existing is changed. Continue?",
            "Create", "Cancel");

        if (!ok) return;

        Color edge = EdgeColor(element);
        Color fill = FillColor(element);

        // the core: a tinted glass disc with a bright rim (NOT white: the white-hot amount is low)
        Material coreMaterial = GetOrMakeMaterial("M_Orb_" + e + "_Core", edge * 0.9f, edge * 0.28f, 0.8f, 0.1f);
        // the ring: a thin glowing outline, almost clear inside
        Material ringMaterial = GetOrMakeMaterial("M_Orb_" + e + "_Ring", edge, fill, 0.05f, 0.1f);

        if (coreMaterial == null || ringMaterial == null) return;

        GameObject root = new GameObject("Orb_" + e);
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(root, "Create Recharge Orb");

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.8f;

        RechargeOrb orb = root.AddComponent<RechargeOrb>();
        orb.Power = element;
        orb.ReturnDelay = 2f;
        orb.SpinSpeed = 0f;   // the discs face the camera; set about 90 for a coin-flip spin

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        orb.Visual = visual.transform;

        // the core: a small disc, stood up to face the camera (Rotation X = 90)
        GameObject core = new GameObject("Core");
        core.transform.SetParent(visual.transform, false);
        core.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        AddShape(core, coreMaterial, NeonShapeMesh.ShapeType.Prism, 24, 0.26f, 0.06f, 0f);

        // the hollow ring around it
        GameObject ring = new GameObject("Ring");
        ring.transform.SetParent(visual.transform, false);
        ring.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        AddShape(ring, ringMaterial, NeonShapeMesh.ShapeType.Ring, 48, 0.55f, 0.05f, 0.45f);

        // the effects
        Material circle = ElementoMaterials.Find("P_Circle", "Elemento/NeonParticle");
        Material flash = ElementoMaterials.Find("P_Ring", "Elemento/NeonParticle");
        Color particleColor = ParticleColor(element);

        orb.UseEffect = MakeBurst(root.transform, "FX_RechargeUse", circle, particleColor, 20, 5f, 0.15f, 0.4f);
        orb.ReturnEffect = MakeRingFlash(root.transform, "FX_RechargeReturn", flash, particleColor, 0.3f, 1.6f, Vector3.zero);

        Selection.activeGameObject = root;
    }

    // a mesh child: NeonShapeMesh + MeshRenderer, no collider
    private static void AddShape(GameObject go, Material material, NeonShapeMesh.ShapeType type, int sides, float radius, float height, float innerRadius)
    {
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        NeonShapeMesh shape = go.AddComponent<NeonShapeMesh>();
        shape.Shape = type;
        shape.Sides = sides;
        shape.Radius = radius;
        shape.Height = height;
        shape.InnerRadius = innerRadius;
        shape.FitBoxCollider = false;
        shape.Build();
    }

    // ------------------------------------------------------------------
    // MATERIALS
    // ------------------------------------------------------------------
    // Only an EXACT name counts as "already there" (the shared Find() would return any NeonShape material)
    private static Material FindExact(string name)
    {
        foreach (string guid in AssetDatabase.FindAssets(name + " t:Material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

            if (material != null && material.name == name)
                return material;
        }

        return null;
    }

    private static Material GetOrMakeMaterial(string name, Color edge, Color fill, float fillAlpha, float coreWhite)
    {
        Material existing = FindExact(name);

        if (existing != null)
            return existing;

        Shader shader = Shader.Find("Elemento/NeonShape");

        if (shader == null)
        {
            EditorUtility.DisplayDialog("Elemento", "The shader Elemento/NeonShape was not found, so no material could be made.", "OK");
            return null;
        }

        // the same folder as the other shape materials
        string folder = "Assets";
        Material sibling = FindExact("M_Shape_Diamond");

        if (sibling != null)
            folder = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(sibling)).Replace('\\', '/');

        Material material = new Material(shader);
        material.name = name;
        material.SetColor("_EdgeColor", edge);
        material.SetColor("_FillColor", fill);
        material.SetFloat("_FillAlpha", fillAlpha);
        material.SetFloat("_CoreWhite", coreWhite);
        material.SetFloat("_GlowStrength", 0.8f);
        material.SetFloat("_EdgeWidth", 0.03f);

        AssetDatabase.CreateAsset(material, folder + "/" + name + ".mat");
        AssetDatabase.SaveAssets();
        return material;
    }

    // ------------------------------------------------------------------
    // PARTICLES
    // ------------------------------------------------------------------
    // A copy of one of YOUR effect prefabs (FX_PadLaunchPound...), as a child. Returns its ParticleSystem (null if the prefab isn't found).
    private static ParticleSystem AddPrefabParticles(string prefabName, string newName, Transform parent)
    {
        foreach (string guid in AssetDatabase.FindAssets(prefabName + " t:Prefab"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null || prefab.name != prefabName) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = newName;
            instance.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            return instance.GetComponentInChildren<ParticleSystem>();
        }

        Debug.LogWarning("Elemento builder: the prefab '" + prefabName + "' was not found, so '" + newName + "' was skipped. Add one by hand.");
        return null;
    }

    // A one-time burst of dots going out in every direction
    private static ParticleSystem MakeBurst(Transform parent, string name, Material material, Color color, int count, float speed, float size, float life)
    {
        ParticleSystem ps = NewSystem(parent, name, material, Vector3.zero);

        ParticleSystem.MainModule main = ps.main;
        main.startLifetime = life;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)count) });

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        ParticleSystem.ColorOverLifetimeModule fade = ps.colorOverLifetime;
        fade.enabled = true;
        fade.color = FadeOut();

        return ps;
    }

    // ONE ring that grows and fades: a small "it's back" flash
    private static ParticleSystem MakeRingFlash(Transform parent, string name, Material material, Color color, float life, float size, Vector3 localPosition)
    {
        ParticleSystem ps = NewSystem(parent, name, material, localPosition);

        ParticleSystem.MainModule main = ps.main;
        main.startLifetime = life;
        main.startSpeed = 0f;
        main.startSize = size;
        main.startColor = color;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)1) });

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = false;

        // grows from 30% to 100% of its size
        ParticleSystem.SizeOverLifetimeModule grow = ps.sizeOverLifetime;
        grow.enabled = true;
        grow.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 0.3f), new Keyframe(1f, 1f)));

        ParticleSystem.ColorOverLifetimeModule fade = ps.colorOverLifetime;
        fade.enabled = true;
        fade.color = FadeOut();

        return ps;
    }

    private static ParticleSystem NewSystem(Transform parent, string name, Material material, Vector3 localPosition)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystemRenderer particleRenderer = go.GetComponent<ParticleSystemRenderer>();

        if (material != null)
            particleRenderer.sharedMaterial = material;
        else
            Debug.LogWarning("Elemento builder: the particle material was not found for '" + name + "'. Drag a P_* material into its Renderer.");

        return ps;
    }

    // white, fully visible then fading to nothing (the Start Color gives the real color)
    private static ParticleSystem.MinMaxGradient FadeOut()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        return new ParticleSystem.MinMaxGradient(gradient);
    }
}
#endif
