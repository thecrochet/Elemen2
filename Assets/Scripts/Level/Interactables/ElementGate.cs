using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// An element gate: the ball runs THROUGH it and gets that element's power (this replaces the old power-up pickups).
//
//   - Only one power at a time: the LAST gate crossed wins.
//   - Crossing the gate you already have does not change anything, but the gate flashes a little.
//   - The gate never disappears and has nothing to save: the checkpoint already remembers the current power.
//
// SETUP (the menu GameObject > Elemento > Interactables > Element Gate does all of this):
//   root object = this script + a Box Collider (Is Trigger)  (about 1.2 wide, 3.2 tall, 1.5 deep)
//   child "Visual" = a Quad (scale about 1.5 x 3.4) with a M_Gate_<Element> material (shader Elemento/Gate/ElementGate)
//
// In the Editor, changing "Element" picks the material named M_Gate_<Element> by itself
// (Element "None" = the Base gate = M_Gate_Base).
[DisallowMultipleComponent]
public class ElementGate : MonoBehaviour
{
    [Header("Power")]
    public PowerUpType Element = PowerUpType.Fire;

    [Header("Feedback (all optional)")]
    // The gate itself flashes brighter for a moment when crossed
    public bool FlashGate = true;
    public float FlashTime = 0.35f;
    [Range(0f, 3f)] public float FlashStrength = 1.2f;

    // A ParticleSystem (Play On Awake OFF, Looping OFF): the same small ring burst as the checkpoint's "Activate Effect"
    public ParticleSystem CrossEffect;

    // Move the burst to the ball before it plays (otherwise it plays where it is placed)
    public bool BurstAtBall = true;

    // The old collect burst of the power-up pickups, in the element's color (off = subtle)
    public bool PlayCollectBurst = false;

    [Header("Visual (found by itself if empty)")]
    public Renderer GateRenderer;

    private static readonly int FlashId = Shader.PropertyToID("_Flash");

    private MaterialPropertyBlock _block;
    private float _flashTimer;

    private void Awake()
    {
        if (GateRenderer == null)
            GateRenderer = GetComponentInChildren<Renderer>();

        _block = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        bool changed = player.CurrentPower != Element;

        if (changed)
        {
            player.SetPower(Element);
            Sfx.Play(SfxType.PowerUp);
        }

        if (PlayCollectBurst)
            Vfx.PlayTinted(VfxType.PowerUpCollect, other.transform.position, Quaternion.identity, Element);

        if (CrossEffect != null)
        {
            if (BurstAtBall)
                CrossEffect.transform.position = other.transform.position;

            CrossEffect.Play();
        }

        if (FlashGate && GateRenderer != null && FlashTime > 0f)
        {
            _flashTimer = FlashTime;
        }
    }

    // the flash fades out (nothing happens here the rest of the time)
    private void Update()
    {
        if (_flashTimer <= 0f || GateRenderer == null) return;

        _flashTimer -= Time.deltaTime;

        float k = Mathf.Clamp01(_flashTimer / FlashTime);
        float amount = k * k * FlashStrength;       // fast at first, then a slow fade

        GateRenderer.GetPropertyBlock(_block);
        _block.SetFloat(FlashId, amount);
        GateRenderer.SetPropertyBlock(_block);
    }

#if UNITY_EDITOR
    // ------------------------------------------------------------------
    // EDITOR HELPERS
    // ------------------------------------------------------------------
    private const string MaterialFolder = "Assets/Art/Materials/Gate";

    // Runs in the Editor when a value changes in the Inspector: the matching material is picked
    private void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            ApplyMaterial();
        };
    }

    [ContextMenu("Apply Element Material")]
    private void ApplyMaterial()
    {
        Renderer visual = GateRenderer != null ? GateRenderer : GetComponentInChildren<Renderer>();
        if (visual == null) return;

        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialFolder + "/M_Gate_" + MaterialName(Element) + ".mat");
        if (material == null || visual.sharedMaterial == material) return;

        Undo.RecordObject(visual, "Element Gate material");
        visual.sharedMaterial = material;
        EditorUtility.SetDirty(visual);
    }

    // name, main, accent, inside tint, inside opacity
    private static readonly (PowerUpType element, Color a, Color b, Color fill, float alpha)[] Palette =
    {
        (PowerUpType.Fire,   new Color(2.5f, 1.1f, 0.25f), new Color(4f, 2.5f, 0.8f),  new Color(0.25f, 0.06f, 0f),    0.18f),
        (PowerUpType.Earth,  new Color(1.8f, 1.05f, 0.5f), new Color(3f, 2f, 1.1f),    new Color(0.15f, 0.08f, 0.03f), 0.18f),
        (PowerUpType.Water,  new Color(0.5f, 1.8f, 2.6f),  new Color(2f, 3.5f, 4f),    new Color(0f, 0.08f, 0.15f),    0.18f),
        (PowerUpType.Air,    new Color(1.6f, 2.1f, 2.4f),  new Color(3f, 3.5f, 4f),    new Color(0.08f, 0.12f, 0.15f), 0.14f),
        (PowerUpType.Aether, new Color(1.2f, 0.4f, 2.2f),  new Color(3f, 1.5f, 4.5f),  new Color(0.01f, 0f, 0.03f),    0.9f),
        (PowerUpType.None,   new Color(3f, 2.6f, 0.6f),    new Color(4f, 3.6f, 1.2f),  new Color(0.15f, 0.12f, 0f),    0.15f),
    };

    // the Base ball is the element "None" in the code, but its material is called M_Gate_Base
    private static string MaterialName(PowerUpType element)
    {
        return element == PowerUpType.None ? "Base" : element.ToString();
    }

    private static string KeywordFor(PowerUpType element)
    {
        switch (element)
        {
            case PowerUpType.Fire: return "_ELEMENT_FIRE";
            case PowerUpType.Earth: return "_ELEMENT_EARTH";
            case PowerUpType.Water: return "_ELEMENT_WATER";
            case PowerUpType.Air: return "_ELEMENT_AIR";
            case PowerUpType.Aether: return "_ELEMENT_AETHER";
            default: return "_ELEMENT_BASE";
        }
    }

    private static int IndexFor(PowerUpType element)
    {
        switch (element)
        {
            case PowerUpType.Fire: return 0;
            case PowerUpType.Earth: return 1;
            case PowerUpType.Water: return 2;
            case PowerUpType.Air: return 3;
            case PowerUpType.Aether: return 4;
            default: return 5;
        }
    }

    // Makes the six materials the first time (M_Gate_Fire, Earth, Water, Air, Aether, Base).
    // A material that exists is never touched, so your colors are safe.
    [MenuItem("Assets/Create/Elemento/Gate Materials")]
    public static void CreateGateMaterials()
    {
        Shader shader = Shader.Find("Elemento/Gate/ElementGate");

        if (shader == null)
        {
            Debug.LogWarning("Shader Elemento/Gate/ElementGate not found: import the shader first.");
            return;
        }

        System.IO.Directory.CreateDirectory(MaterialFolder);
        AssetDatabase.Refresh();

        string[] keywords = { "_ELEMENT_FIRE", "_ELEMENT_EARTH", "_ELEMENT_WATER", "_ELEMENT_AIR", "_ELEMENT_AETHER", "_ELEMENT_BASE" };

        for (int i = 0; i < Palette.Length; i++)
        {
            string path = MaterialFolder + "/M_Gate_" + MaterialName(Palette[i].element) + ".mat";
            if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) continue;

            Material material = new Material(shader);
            material.SetFloat("_Element", IndexFor(Palette[i].element));

            foreach (string keyword in keywords)
                material.DisableKeyword(keyword);

            material.EnableKeyword(KeywordFor(Palette[i].element));
            material.SetColor("_ColorA", Palette[i].a);
            material.SetColor("_ColorB", Palette[i].b);
            material.SetColor("_FillColor", Palette[i].fill);
            material.SetFloat("_FillAlpha", Palette[i].alpha);

            AssetDatabase.CreateAsset(material, path);
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("GameObject/Elemento/Interactables/Element Gate", false, 16)]
    private static void CreateElementGate(MenuCommand command)
    {
        CreateGateMaterials();

        GameObject go = new GameObject("ElementGate");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Element Gate");

        BoxCollider box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(1.2f, 3.2f, 1.5f);

        ElementGate gate = go.AddComponent<ElementGate>();

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Quad);
        visual.name = "Visual";
        visual.transform.SetParent(go.transform, false);
        visual.transform.localScale = new Vector3(1.5f, 3.4f, 1f);
        DestroyImmediate(visual.GetComponent<Collider>());

        MeshRenderer renderer = visual.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        gate.GateRenderer = renderer;
        gate.ApplyMaterial();

        Selection.activeGameObject = go;
    }
#endif
}
