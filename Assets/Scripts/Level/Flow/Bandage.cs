using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

// The hidden bandage of a level (like the bandages of Super Meat Boy). Only some levels have one, well hidden or hard to reach.
//
//   - It is NOT a coin and has NOTHING to do with the stars (stars only depend on the finish time).
//   - Touching it hides it. If you die, it comes back, unless you reached a checkpoint after taking it.
//   - It only COUNTS when you finish the level while holding it. Then it is saved for good.
//   - A saved bandage stays saved: replaying the level never removes it.
//
// For the hub: Bandage.IsCollected("SceneName") tells if that level's bandage was found.
// If a level ever has more than one, give each its own "Id" (0, 1, 2...).
//
// Put it on an object with a Collider that has "Is Trigger" checked.
[DisallowMultipleComponent]
public class Bandage : MonoBehaviour, IResettable
{
    // 0 for the first bandage of the level
    public int Id = 0;

    [Header("Look (all optional)")]
    // A ParticleSystem (Play On Awake OFF, Looping OFF): a small burst when taken
    public ParticleSystem CollectEffect;

    // Slow turn around the vertical axis, in degrees per second (0 = still)
    public float SpinSpeed = 45f;

    // ------------------------------------------------------------------
    // SAVED DATA
    // ------------------------------------------------------------------
    private static string Key(string sceneName, int id)
    {
        return "Bandage_" + sceneName + "_" + id;
    }

    // Was the bandage of this level (and this Id) ever found and carried to the finish?
    public static bool IsCollected(string sceneName, int id = 0)
    {
        return PlayerPrefs.GetInt(Key(sceneName, id), 0) == 1;
    }

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private bool _held;             // taken during this run
    private bool _committedHeld;    // the "save" of the last checkpoint
    private bool _saved;            // already written to PlayerPrefs

    private void Start()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_held) return;
        if (!other.TryGetComponent(out CC_ELEMENTO _)) return;

        _held = true;
        Sfx.Play(SfxType.PowerUp);

        if (CollectEffect != null)
        {
            CollectEffect.transform.position = transform.position;
            CollectEffect.Play();
        }

        SetVisible(false);
    }

    private void Update()
    {
        if (!_held)
        {
            // the bandage turns slowly (the visual only: the collider can stay)
            if (SpinSpeed != 0f)
                transform.Rotate(0f, SpinSpeed * Time.deltaTime, 0f, Space.World);

            return;
        }

        // taken and the level is finished: it counts now
        if (!_saved && LevelManager.Instance != null && LevelManager.Instance.IsFinished)
        {
            _saved = true;
            PlayerPrefs.SetInt(Key(SceneManager.GetActiveScene().name, Id), 1);
            PlayerPrefs.Save();
        }
    }

    // The visual and the collider are hidden together by turning off the children and the collider
    // (the object itself stays active so this script keeps running and keeps its save/load).
    private void SetVisible(bool visible)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            renderer.enabled = visible;

        foreach (Collider collider in GetComponents<Collider>())
            collider.enabled = visible;
    }

    // SAVE (start of the level and at each checkpoint)
    public void CommitState()
    {
        _committedHeld = _held;
    }

    // LOAD (at each respawn)
    public void ResetObject()
    {
        _held = _committedHeld;
        SetVisible(!_held);
    }

#if UNITY_EDITOR
    private static Material FindShapeMaterial()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

            if (material != null && material.shader != null && material.shader.name.StartsWith("Elemento/NeonShape"))
                return material;
        }

        return null;
    }

    // A small plus-shaped bandage: two crossed bars, tilted 45 degrees
    [MenuItem("GameObject/Elemento/Flow/Bandage", false, 18)]
    private static void CreateBandage(MenuCommand command)
    {
        GameObject go = new GameObject("Bandage");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Bandage");

        SphereCollider trigger = go.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.6f;

        go.AddComponent<Bandage>();

        Material material = FindShapeMaterial();

        Vector3[] scales = { new Vector3(0.9f, 0.3f, 0.15f), new Vector3(0.3f, 0.9f, 0.15f) };

        for (int i = 0; i < 2; i++)
        {
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = i == 0 ? "BarA" : "BarB";
            bar.transform.SetParent(go.transform, false);
            bar.transform.localScale = scales[i];
            DestroyImmediate(bar.GetComponent<Collider>());

            MeshRenderer renderer = bar.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            if (material != null)
                renderer.sharedMaterial = material;
        }

        go.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        Selection.activeGameObject = go;
    }
#endif
}
