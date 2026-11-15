#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// EDITOR ONLY. Menus that build the prefab-ready objects of the Prefab Build Guide in one click.
//   Assets > Create > Elemento > Level Materials      creates every colored material (run it once)
//   GameObject > Elemento > Hazards > Stomp Block (Teeth) / Stomp Block (Plain) / Saw (Disc) / Saw (Teeth) / Cannon / Fire Jet
//   GameObject > Elemento > Level > Cracked Block
// Every menu picks the materials by NAME (M_Level_Danger...). If one does not exist yet, the first material
// that uses the right shader is used, so run "Level Materials" first.

// Finds and creates materials
public static class ElementoMaterials
{
    public const string Folder = "Assets/Art/Materials/Level";

    // The material called "name", or else the first material that uses the shader
    public static Material Find(string name, string shaderName)
    {
        foreach (string guid in AssetDatabase.FindAssets(name + " t:Material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

            if (material != null && material.name == name)
                return material;
        }

        foreach (string guid in AssetDatabase.FindAssets("t:Material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

            if (material != null && material.shader != null && material.shader.name == shaderName)
                return material;
        }

        return null;
    }

    private static void EnsureFolder()
    {
        string[] parts = Folder.Split('/');
        string path = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = path + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(path, parts[i]);

            path = next;
        }
    }

    // Creates the material if it does not exist yet (an existing one is never overwritten)
    private static Material Make(string name, string shaderName)
    {
        string path = Folder + "/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
            return null;

        Shader shader = Shader.Find(shaderName);

        if (shader == null)
        {
            Debug.LogWarning("Elemento: shader '" + shaderName + "' not found, '" + name + "' was not created. Is the shader file in the project?");
            return null;
        }

        Material material = new Material(shader);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    [MenuItem("Assets/Create/Elemento/Level Materials")]
    private static void CreateAll()
    {
        EnsureFolder();
        Material m;

        // ---- NeonGrid (for boxes) ----
        m = Make("M_Level_Orange", "Elemento/NeonGrid");
        if (m != null)
        {
            m.SetColor("_EdgeColor", new Color(2.8f, 1.4f, 0f));
            m.SetColor("_FillColor", new Color(0.03f, 0.012f, 0f));
            m.SetColor("_GridColor", new Color(0.7f, 0.35f, 0f));
        }

        m = Make("M_Level_Cyan_Ice", "Elemento/NeonGrid");
        if (m != null)
        {
            m.SetColor("_EdgeColor", new Color(1.2f, 2.6f, 3.2f));
            m.SetColor("_FillColor", new Color(0.02f, 0.05f, 0.08f));
            m.SetColor("_GridColor", new Color(0.4f, 0.8f, 1f));
            m.SetFloat("_CoreWhite", 0.8f);
            m.SetFloat("_TopBoost", 2f);
        }

        m = Make("M_Level_Danger", "Elemento/NeonGrid");
        if (m != null)
        {
            m.SetColor("_EdgeColor", new Color(3f, 0.15f, 0.35f));
            m.SetColor("_FillColor", new Color(0.03f, 0f, 0.005f));
            m.SetColor("_GridColor", new Color(0.7f, 0.05f, 0.12f));
            m.SetFloat("_PulseAmount", 0.45f);
            m.SetFloat("_PulseSpeed", 4f);
        }

        // ---- NeonShape (for spikes, saws, teeth, slopes) ----
        m = Make("M_Shape_Danger", "Elemento/NeonShape");
        if (m != null)
        {
            m.SetColor("_EdgeColor", new Color(3f, 0.15f, 0.35f));
            m.SetColor("_FillColor", new Color(0.03f, 0f, 0.005f));
            m.SetFloat("_PulseAmount", 0.45f);
            m.SetFloat("_PulseSpeed", 4f);
        }

        m = Make("M_Shape_Orange", "Elemento/NeonShape");
        if (m != null)
        {
            m.SetColor("_EdgeColor", new Color(2.8f, 1.4f, 0f));
            m.SetColor("_FillColor", new Color(0.03f, 0.012f, 0f));
        }

        m = Make("M_Shape_Diamond", "Elemento/NeonShape");
        if (m != null)
        {
            m.SetColor("_EdgeColor", new Color(1.5f, 2.8f, 3.4f));
            m.SetColor("_FillColor", new Color(0.03f, 0.08f, 0.1f));
            m.SetFloat("_FillAlpha", 0.4f);
            m.SetFloat("_CoreWhite", 0.8f);
        }

        // ---- the cracks and the dash zone ----
        m = Make("M_Crack", "Elemento/CrackOverlay");

        m = Make("M_DashZone_Fire", "Elemento/DashZone");
        if (m != null)
        {
            m.SetColor("_Color", new Color(3f, 1.1f, 0.1f));
            m.SetColor("_FrameColor", new Color(1.6f, 0.5f, 0.05f));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Elemento: level materials are in " + Folder + " (existing ones were left untouched).");
    }
}

// The builders
public static class ElementoPrefabBuilders
{
    // ------------------------------------------------------------------
    // small helpers
    // ------------------------------------------------------------------
    private static GameObject NewObject(string objectName, MenuCommand command)
    {
        GameObject go = new GameObject(objectName);
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + objectName);
        return go;
    }

    private static GameObject NewChild(string childName, Transform parent, Vector3 position)
    {
        GameObject child = new GameObject(childName);
        child.transform.SetParent(parent, false);
        child.transform.localPosition = position;
        return child;
    }

    // a cube picture with the NeonGrid material and NO collider
    private static GameObject NewCube(string cubeName, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = cubeName;
        Object.DestroyImmediate(cube.GetComponent<Collider>());
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = position;
        cube.transform.localScale = scale;

        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        if (material != null)
            renderer.sharedMaterial = material;

        return cube;
    }

    // an empty object with a NeonShapeMesh (no collider)
    private static NeonShapeMesh NewShape(string shapeName, Transform parent, Vector3 position, Vector3 euler,
                                          NeonShapeMesh.ShapeType type, int sides, float radius, float height,
                                          float rotationOffset, int count, float spacing, Material material)
    {
        GameObject go = NewChild(shapeName, parent, position);
        go.transform.localRotation = Quaternion.Euler(euler);

        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        if (material != null)
            renderer.sharedMaterial = material;

        NeonShapeMesh shape = go.AddComponent<NeonShapeMesh>();
        shape.Shape = type;
        shape.Sides = sides;
        shape.Radius = radius;
        shape.Height = height;
        shape.RotationOffset = rotationOffset;
        shape.Count = count;
        shape.Spacing = spacing;
        shape.FitBoxCollider = false;
        shape.Build();
        return shape;
    }

    private static Material GridMaterial(string name)
    {
        return ElementoMaterials.Find(name, "Elemento/NeonGrid");
    }

    private static Material ShapeMaterial(string name)
    {
        return ElementoMaterials.Find(name, "Elemento/NeonShape");
    }

    // ------------------------------------------------------------------
    // STOMP BLOCKS
    // ------------------------------------------------------------------
    // Teeth version: a 3 x 2 body with 3 triangle teeth under it. Trigger = about 90% of the picture.
    [MenuItem("GameObject/Elemento/Hazards/Stomp Block (Teeth)", false, 20)]
    private static void CreateStompTeeth(MenuCommand command)
    {
        GameObject root = NewObject("StompBlock_Teeth", command);
        BuildStomp(root, true);
    }

    // Plain version: a 3 x 3 pulsing block, no teeth
    [MenuItem("GameObject/Elemento/Hazards/Stomp Block (Plain)", false, 21)]
    private static void CreateStompPlain(MenuCommand command)
    {
        GameObject root = NewObject("StompBlock_Plain", command);
        BuildStomp(root, false);
    }

    private static void BuildStomp(GameObject root, bool teeth)
    {
        BoxCollider trigger = root.AddComponent<BoxCollider>();
        trigger.isTrigger = true;

        root.AddComponent<Hazard>();
        StompBlock stomp = root.AddComponent<StompBlock>();   // adds the kinematic Rigidbody too

        // the part that shakes: it holds the body and the teeth
        GameObject visual = NewChild("Visual", root.transform, Vector3.zero);
        stomp.Visual = visual.transform;

        Material danger = GridMaterial("M_Level_Danger");

        if (teeth)
        {
            // body: 3 wide, 2 tall, 2 deep. Teeth: 3 triangle pyramids under it, pointing down.
            NewCube("Body", visual.transform, Vector3.zero, new Vector3(3f, 2f, 2f), danger);

            // Radius 0.5 + Spacing 0.134 = one tooth every 1 unit; Rotation Z 180 = points down; y -1 = the bottom of the body
            NewShape("Teeth", visual.transform, new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 180f),
                     NeonShapeMesh.ShapeType.Pyramid, 3, 0.5f, 0.6f, 90f, 3, 0.134f, ShapeMaterial("M_Shape_Danger"));

            trigger.size = new Vector3(2.7f, 2.3f, 1.6f);
            trigger.center = new Vector3(0f, -0.25f, 0f);
        }
        else
        {
            NewCube("Body", visual.transform, Vector3.zero, new Vector3(3f, 3f, 2f), danger);

            trigger.size = new Vector3(2.7f, 2.7f, 1.6f);
            trigger.center = Vector3.zero;
        }

        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // SAWS
    // ------------------------------------------------------------------
    // Disc version: a circle (24 sides) with a triangle inside, so you can SEE it spin
    [MenuItem("GameObject/Elemento/Hazards/Saw (Disc)", false, 22)]
    private static void CreateSawDisc(MenuCommand command)
    {
        GameObject root = NewObject("Saw_Disc", command);
        BuildSaw(root, false);
    }

    // Teeth version: a smaller disc with a ring of 12 triangle teeth
    [MenuItem("GameObject/Elemento/Hazards/Saw (Teeth)", false, 23)]
    private static void CreateSawTeeth(MenuCommand command)
    {
        GameObject root = NewObject("Saw_Teeth", command);
        BuildSaw(root, true);
    }

    private static void BuildSaw(GameObject root, bool teeth)
    {
        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = teeth ? 1.0f : 0.85f;   // the picture reaches 1.2 (teeth) or 1.0 (disc): about 85%

        root.AddComponent<Hazard>();
        RotatingSaw saw = root.AddComponent<RotatingSaw>();   // adds the kinematic Rigidbody too

        // the part that spins around its Z axis (the default Spin Axis 0,0,1)
        saw.Visual = BuildBladeVisual(root.transform, teeth, 1f);

        Selection.activeGameObject = root;
    }

    // The picture of a saw blade, with NO scripts and NO colliders: "Visual" (an empty that gets spun by a script)
    // holding the Disc, the InnerTriangle and, for the teeth version, 12 teeth.
    // "scale" 1 = the saw (disc radius 1, or 0.85 with teeth). 0.5 = the blade at the end of a SpinArm.
    private static Transform BuildBladeVisual(Transform parent, bool teeth, float scale)
    {
        GameObject visual = NewChild("Visual", parent, Vector3.zero);
        Material material = ShapeMaterial("M_Shape_Danger");
        float discRadius = (teeth ? 0.85f : 1f) * scale;

        // The disc: a Prism lying down (Rotation X 90 makes its height point along Z), moved back by half its thickness
        NewShape("Disc", visual.transform, new Vector3(0f, 0f, -0.1f * scale), new Vector3(90f, 0f, 0f),
                 NeonShapeMesh.ShapeType.Prism, 24, discRadius, 0.2f * scale, 0f, 1, 0f, material);

        // a triangle inside, a bit thicker, so the spin is visible (a plain disc looks the same turning)
        NewShape("InnerTriangle", visual.transform, new Vector3(0f, 0f, -0.15f * scale), new Vector3(90f, 0f, 0f),
                 NeonShapeMesh.ShapeType.Prism, 3, discRadius * 0.55f, 0.3f * scale, 90f, 1, 0f, material);

        if (teeth)
        {
            const int count = 12;

            for (int i = 0; i < count; i++)
            {
                float angle = 360f * i / count;
                float radians = angle * Mathf.Deg2Rad;

                // the tooth base sits on the rim, the tip points away from the center (Rotation Z = angle - 90)
                Vector3 position = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * (discRadius - 0.02f * scale);

                NewShape("Tooth_" + i, visual.transform, position, new Vector3(0f, 0f, angle - 90f),
                         NeonShapeMesh.ShapeType.Pyramid, 3, 0.26f * scale, 0.35f * scale, 90f, 1, 0f, material);
            }
        }

        return visual.transform;
    }

    // Only the picture of a blade, to make a prefab (SawBlade_Disc) that the Saw and the SpinArm both reuse
    [MenuItem("GameObject/Elemento/Hazards/Saw Blade Picture (Disc)", false, 26)]
    private static void CreateBladeDisc(MenuCommand command)
    {
        GameObject root = NewObject("SawBlade_Disc", command);
        BuildBladeVisual(root.transform, false, 1f);
        Selection.activeGameObject = root;
    }

    [MenuItem("GameObject/Elemento/Hazards/Saw Blade Picture (Teeth)", false, 27)]
    private static void CreateBladeTeeth(MenuCommand command)
    {
        GameObject root = NewObject("SawBlade_Teeth", command);
        BuildBladeVisual(root.transform, true, 1f);
        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // SPIN ARM
    // ------------------------------------------------------------------
    // A pole, an arm that turns around it, and a saw blade at the tip. Only the BLADE kills.
    [MenuItem("GameObject/Elemento/Hazards/Spin Arm", false, 28)]
    private static void CreateSpinArm(MenuCommand command)
    {
        GameObject root = NewObject("SpinArm", command);

        // the pole does not turn. A solid collider lets the ball stand on it (delete it if you do not want that)
        GameObject pole = NewCube("Pole", root.transform, Vector3.zero, new Vector3(0.8f, 0.8f, 1.2f), GridMaterial("M_Level_Cyan"));
        pole.AddComponent<BoxCollider>();

        // the part that turns: the Rotor has the script and the Rigidbody, and NO Hazard (it has no collider, it kills nothing)
        GameObject rotor = NewChild("Rotor", root.transform, Vector3.zero);
        SpinArm bar = rotor.AddComponent<SpinArm>();   // adds the kinematic Rigidbody too
        bar.SpinAxis = Vector3.forward;
        bar.SpinSpeed = 90f;
        bar.BladeSpinSpeed = 360f;

        // the arm: harmless
        NewCube("Arm", rotor.transform, new Vector3(2.5f, 0f, 0f), new Vector3(5f, 0.15f, 0.15f), GridMaterial("M_Level_Danger"));

        // the blade: the only part that kills
        GameObject blade = NewChild("Blade", rotor.transform, new Vector3(5f, 0f, 0f));
        SphereCollider trigger = blade.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.45f;
        blade.AddComponent<Hazard>();

        // BladeVisual must be an UNROTATED empty: the script spins it around its own Z. The rotated discs are inside it.
        bar.BladeVisual = BuildBladeVisual(blade.transform, false, 0.5f);

        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // ONE WAY PLATFORM
    // ------------------------------------------------------------------
    // A thin platform: you jump up through it from below and land on top. Hold DOWN to drop through.
    [MenuItem("GameObject/Elemento/Level/One Way Platform", false, 15)]
    private static void CreateOneWay(MenuCommand command)
    {
        GameObject root = NewObject("OneWayPlatform", command);

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        Object.DestroyImmediate(visual.GetComponent<Collider>());
        visual.transform.SetParent(root.transform, false);

        MeshRenderer renderer = visual.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        Material material = GridMaterial("M_Level_Cyan_Thin");

        if (material != null)
            renderer.sharedMaterial = material;

        NeonBlock block = root.AddComponent<NeonBlock>();   // adds the Box Collider
        block.Visual = visual.transform;
        block.CellSize = 0.5f;
        block.Size = new Vector3Int(8, 1, 4);   // 4 long, 0.5 tall, 2 deep
        block.Anchor = NeonBlock.AnchorType.TopLeft;
        block.Apply();

        root.AddComponent<OneWayPlatform>();

        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // CANNON
    // ------------------------------------------------------------------
    [MenuItem("GameObject/Elemento/Hazards/Cannon", false, 24)]
    private static void CreateCannon(MenuCommand command)
    {
        GameObject root = NewObject("Cannon", command);

        // solid: the ball can stand on it
        BoxCollider solid = root.AddComponent<BoxCollider>();
        solid.size = new Vector3(1.4f, 1.4f, 1.4f);

        Cannon cannon = root.AddComponent<Cannon>();
        Material material = GridMaterial("M_Level_Orange");

        NewCube("Body", root.transform, Vector3.zero, new Vector3(1.4f, 1.4f, 1.4f), material);
        NewCube("Barrel", root.transform, new Vector3(1f, 0f, 0f), new Vector3(0.9f, 0.6f, 0.6f), material);

        GameObject muzzle = NewChild("Muzzle", root.transform, new Vector3(1.5f, 0f, 0f));
        cannon.Muzzle = muzzle.transform;

        // the projectile template: a trigger sphere that kills, with a pyramid picture pointing along +X
        GameObject projectile = NewChild("Projectile", root.transform, new Vector3(1.5f, 0f, 0f));
        SphereCollider hit = projectile.AddComponent<SphereCollider>();
        hit.isTrigger = true;
        hit.radius = 0.28f;
        projectile.AddComponent<Hazard>();

        // Rotation Z -90 turns the pyramid's tip toward +X; X -0.3 centers it on the collider
        NewShape("Picture", projectile.transform, new Vector3(-0.3f, 0f, 0f), new Vector3(0f, 0f, -90f),
                 NeonShapeMesh.ShapeType.Pyramid, 3, 0.3f, 0.6f, 90f, 1, 0f, ShapeMaterial("M_Shape_Danger"));

        cannon.Projectile = projectile;

        Selection.activeGameObject = root;
    }

    // ------------------------------------------------------------------
    // FIRE JET
    // ------------------------------------------------------------------
    [MenuItem("GameObject/Elemento/Hazards/Fire Jet", false, 25)]
    private static void CreateFireJet(MenuCommand command)
    {
        GameObject root = NewObject("FireJet", command);

        // the nozzle is solid: 1 wide, 0.5 tall
        BoxCollider solid = root.AddComponent<BoxCollider>();
        solid.size = new Vector3(1f, 0.5f, 1f);
        solid.center = new Vector3(0f, 0.25f, 0f);

        FireJet jet = root.AddComponent<FireJet>();

        NewCube("Nozzle", root.transform, new Vector3(0f, 0.25f, 0f), new Vector3(1f, 0.5f, 1f), GridMaterial("M_Level_Orange"));

        // the flame kills: a tall trigger (picture is 3 tall, the trigger covers about 85% of it)
        GameObject kill = NewChild("KillZone", root.transform, new Vector3(0f, 1.8f, 0f));
        BoxCollider trigger = kill.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(0.7f, 2.6f, 0.8f);
        kill.AddComponent<Hazard>();
        jet.KillZone = trigger;

        jet.Flame = NewFlame("Flame", root.transform, true);
        jet.WarnEffect = NewFlame("WarnEffect", root.transform, false);

        Selection.activeGameObject = root;
    }

    // A looping particle system pointing up. The flame is big and hot, the warning is a few small sparks.
    private static ParticleSystem NewFlame(string objectName, Transform parent, bool big)
    {
        GameObject go = NewChild(objectName, parent, new Vector3(0f, 0.5f, 0f));
        go.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);   // the cone points up

        ParticleSystem system = go.AddComponent<ParticleSystem>();
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = system.main;
        main.loop = true;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = big ? new ParticleSystem.MinMaxCurve(0.45f, 0.7f) : new ParticleSystem.MinMaxCurve(0.4f, 0.6f);
        main.startSpeed = big ? new ParticleSystem.MinMaxCurve(4f, 5.5f) : new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = big ? new ParticleSystem.MinMaxCurve(0.3f, 0.55f) : new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startColor = big ? new Color(3f, 1.1f, 0.1f, 1f) : new Color(2.5f, 1.4f, 0.3f, 1f);
        main.maxParticles = big ? 120 : 20;

        ParticleSystem.EmissionModule emission = system.emission;
        emission.rateOverTime = big ? 70f : 14f;

        ParticleSystem.ShapeModule shape = system.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = big ? 5f : 25f;
        shape.radius = big ? 0.2f : 0.3f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.9f, 0.4f), 0f), new GradientColorKey(new Color(1f, 0.25f, 0.02f), 0.5f), new GradientColorKey(new Color(0.4f, 0.02f, 0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.6f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = gradient;

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = system.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        Material particle = ElementoMaterials.Find("P_Circle", "Elemento/NeonParticle");

        if (particle != null)
            renderer.sharedMaterial = particle;

        return system;
    }

    // ------------------------------------------------------------------
    // CRACKED BLOCK
    // ------------------------------------------------------------------
    // A 2 x 2 breakable block: NeonBlock + DestructibleObject + a "Cracks" quad in front of it.
    // HitsToBreak 3 shows the 3 crack pictures one after the other. Set PreCracked to start damaged.
    [MenuItem("GameObject/Elemento/Level/Cracked Block", false, 14)]
    private static void CreateCrackedBlock(MenuCommand command)
    {
        GameObject root = NewObject("CrackedBlock", command);

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        Object.DestroyImmediate(visual.GetComponent<Collider>());
        visual.transform.SetParent(root.transform, false);

        MeshRenderer renderer = visual.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        Material orange = GridMaterial("M_Level_Orange");

        if (orange != null)
            renderer.sharedMaterial = orange;

        NeonBlock block = root.AddComponent<NeonBlock>();   // adds the Box Collider
        block.Visual = visual.transform;
        block.Size = new Vector3Int(2, 2, 2);
        block.Apply();

        // the cracks: a Quad in front of the block (the camera looks along +Z, so "front" is -Z).
        // The block spans Z 0..? around its center, so the quad sits just before the front face.
        GameObject cracks = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cracks.name = "Cracks";
        Object.DestroyImmediate(cracks.GetComponent<Collider>());
        cracks.transform.SetParent(root.transform, false);

        MeshRenderer crackRenderer = cracks.GetComponent<MeshRenderer>();
        crackRenderer.shadowCastingMode = ShadowCastingMode.Off;
        crackRenderer.receiveShadows = false;

        Material crack = ElementoMaterials.Find("M_Crack", "Elemento/CrackOverlay");

        if (crack != null)
            crackRenderer.sharedMaterial = crack;

        // TopLeft anchor: the block is 2 x 2 hanging from the pivot, its center is (1, -1). Front face at z = -1 (depth 2, centered).
        cracks.transform.localPosition = new Vector3(1f, -1f, -1.02f);
        cracks.transform.localScale = new Vector3(2f, 2f, 1f);

        DestructibleObject destructible = root.AddComponent<DestructibleObject>();
        destructible.HitsToBreak = 3;
        destructible.CrackRenderer = crackRenderer;

        Selection.activeGameObject = root;
    }
}
#endif
