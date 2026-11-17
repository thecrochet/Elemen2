#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Writes the key values of every ParticleSystem in the VFX prefabs to AI_CONTEXT/PARTICLE_DUMP.txt
// so they can be compared with the plan. It only READS prefabs (nothing is changed or saved).
//
// Menus:  Elemento > Debug > Dump Particle Values (VFX prefabs)
//         Elemento > Debug > Dump Particle Values (all prefabs)
public static class ElementoParticleDump
{
    [MenuItem("Elemento/Debug/Dump Particle Values (VFX prefabs)")]
    private static void DumpVfx()
    {
        Dump("Assets/Prefabs/VFX", "PARTICLE_DUMP.txt");
    }

    [MenuItem("Elemento/Debug/Dump Particle Values (all prefabs)")]
    private static void DumpAll()
    {
        Dump("Assets/Prefabs", "PARTICLE_DUMP_ALL.txt");
    }

    private static void Dump(string folder, string fileName)
    {
        StringBuilder sb = new StringBuilder();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });
        int systems = 0;
        int prefabs = 0;

        System.Array.Sort(guids, (a, b) => string.CompareOrdinal(AssetDatabase.GUIDToAssetPath(a), AssetDatabase.GUIDToAssetPath(b)));

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            ParticleSystem[] list = prefab.GetComponentsInChildren<ParticleSystem>(true);
            if (list.Length == 0) continue;

            prefabs++;
            sb.AppendLine("== " + path);

            foreach (ParticleSystem ps in list)
            {
                systems++;
                Describe(sb, ps, prefab.transform);
            }

            sb.AppendLine();
        }

        string root = Directory.GetParent(Application.dataPath).FullName;
        string dir = Path.Combine(root, "AI_CONTEXT");
        Directory.CreateDirectory(dir);
        string file = Path.Combine(dir, fileName);
        File.WriteAllText(file, sb.ToString(), new UTF8Encoding(false));

        Debug.Log("Particle dump: " + prefabs + " prefabs, " + systems + " systems -> " + file);
    }

    private static void Describe(StringBuilder sb, ParticleSystem ps, Transform root)
    {
        ParticleSystem.MainModule main = ps.main;
        ParticleSystem.EmissionModule emission = ps.emission;
        ParticleSystem.ShapeModule shape = ps.shape;

        sb.AppendLine("  [" + PathOf(ps.transform, root) + "]");
        sb.AppendLine("    main: duration " + F(main.duration) + ", loop " + main.loop + ", prewarm " + main.prewarm
            + ", playOnAwake " + main.playOnAwake + ", space " + main.simulationSpace + ", max " + main.maxParticles);
        sb.AppendLine("    start: delay " + C(main.startDelay) + ", lifetime " + C(main.startLifetime)
            + ", speed " + C(main.startSpeed) + ", size " + C(main.startSize)
            + ", rotation " + C(main.startRotation) + ", gravity " + C(main.gravityModifier));
        sb.AppendLine("    color: " + G(main.startColor));
        sb.Append("    emission: " + (emission.enabled ? "rate " + C(emission.rateOverTime) + ", distance " + C(emission.rateOverDistance) : "off"));

        if (emission.enabled)
        {
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
            emission.GetBursts(bursts);

            foreach (ParticleSystem.Burst b in bursts)
                sb.Append(", burst t=" + F(b.time) + " n=" + C(b.count));
        }

        sb.AppendLine();
        sb.AppendLine("    shape: " + (shape.enabled ? shape.shapeType + ", radius " + F(shape.radius) + ", angle " + F(shape.angle)
            + ", scale " + shape.scale : "off"));
        sb.AppendLine("    modules: colorOverLifetime " + ps.colorOverLifetime.enabled
            + ", sizeOverLifetime " + ps.sizeOverLifetime.enabled
            + ", velocity " + ps.velocityOverLifetime.enabled
            + ", force " + ps.forceOverLifetime.enabled
            + ", noise " + ps.noise.enabled
            + ", rotation " + ps.rotationOverLifetime.enabled
            + ", trails " + ps.trails.enabled);

        ParticleSystemRenderer r = ps.GetComponent<ParticleSystemRenderer>();

        if (r != null)
        {
            string mat = r.sharedMaterial != null ? r.sharedMaterial.name : "NONE";
            sb.AppendLine("    renderer: enabled " + r.enabled + ", material " + mat + ", mode " + r.renderMode + ", sortingOrder " + r.sortingOrder);
        }
    }

    private static string PathOf(Transform t, Transform root)
    {
        string p = t.name;
        while (t != root && t.parent != null)
        {
            t = t.parent;
            p = t.name + "/" + p;
        }
        return p;
    }

    private static string F(float v)
    {
        return v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string C(ParticleSystem.MinMaxCurve c)
    {
        switch (c.mode)
        {
            case ParticleSystemCurveMode.Constant: return F(c.constant);
            case ParticleSystemCurveMode.TwoConstants: return F(c.constantMin) + ".." + F(c.constantMax);
            case ParticleSystemCurveMode.Curve: return "curve x" + F(c.curveMultiplier);
            default: return "two curves x" + F(c.curveMultiplier);
        }
    }

    private static string Col(Color c)
    {
        return "(" + F(c.r) + "," + F(c.g) + "," + F(c.b) + "," + F(c.a) + ")";
    }

    private static string G(ParticleSystem.MinMaxGradient g)
    {
        switch (g.mode)
        {
            case ParticleSystemGradientMode.Color: return Col(g.color);
            case ParticleSystemGradientMode.TwoColors: return Col(g.colorMin) + " .. " + Col(g.colorMax);
            case ParticleSystemGradientMode.Gradient: return "gradient " + GradientText(g.gradient);
            case ParticleSystemGradientMode.TwoGradients: return "gradients " + GradientText(g.gradientMin) + " .. " + GradientText(g.gradientMax);
            default: return g.mode.ToString();
        }
    }

    private static string GradientText(Gradient grad)
    {
        if (grad == null) return "none";
        return Col(grad.Evaluate(0f)) + " -> " + Col(grad.Evaluate(1f));
    }
}
#endif
