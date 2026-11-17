using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

// The hidden ATOM of a level: a second collectible, in the 2.5D level (the diamond is reserved for the HUD / hub).
// What it unlocks or tells is NOT decided yet (maybe a story told in pieces, out of order). For now it only counts.
//
//   - It has NOTHING to do with the stars (stars only depend on the finish time).
//   - Touching it hides it. If you die, it comes back, unless you reached a checkpoint after taking it.
//   - It only COUNTS when you finish the level while holding it. Then it is saved for good.
//   - A saved atom stays saved: replaying the level never removes it.
//
// For the hub: Atom.IsCollected("SceneName") tells if that level's atom was found.
// If a level has more than one, give each its own "Id" (0, 1, 2...).
//
// LOOK: a small nucleus, three tilted orbit rings and an electron running on each ring.
// Put it on an object with a Collider that has "Is Trigger" checked (the menu does everything).
[DisallowMultipleComponent]
public class Atom : MonoBehaviour, IResettable
{
    // 0 for the first atom of the level
    public int Id = 0;

    [Header("Look (all optional)")]
    // A ParticleSystem (Play On Awake OFF, Looping OFF): a small burst when taken
    public ParticleSystem CollectEffect;

    // The 3 empty objects the electrons run around (filled by the menu). Each one turns around its own Z.
    public Transform[] ElectronPivots;

    // Degrees per second the electrons run on their rings (the 3 go at slightly different speeds)
    public float ElectronSpeed = 140f;

    // ------------------------------------------------------------------
    // SAVED DATA
    // ------------------------------------------------------------------
    private static string Key(string sceneName, int id)
    {
        return "Atom_" + sceneName + "_" + id;
    }

    // Was the atom of this level (and this Id) ever found and carried to the finish?
    public static bool IsCollected(string sceneName, int id = 0)
    {
        return PlayerPrefs.GetInt(Key(sceneName, id), 0) == 1;
    }

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private bool _held;
    private bool _committedHeld;
    private bool _saved;

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
        Sfx.Play(SfxType.AtomCollect);

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
            if (ElectronPivots != null)
            {
                for (int i = 0; i < ElectronPivots.Length; i++)
                {
                    if (ElectronPivots[i] != null)
                        ElectronPivots[i].Rotate(0f, 0f, ElectronSpeed * (1f + 0.25f * i) * Time.deltaTime, Space.Self);
                }
            }

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

    // the visuals and the collider are hidden together (the object stays active so the save/load keeps working)
    private void SetVisible(bool visible)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            // the collect effect is a child too: its renderer must stay on, it plays after the item is hidden
            if (renderer is ParticleSystemRenderer) continue;

            renderer.enabled = visible;
        }

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
    [MenuItem("GameObject/Elemento/Flow/Atom", false, 19)]
    private static void CreateAtom(MenuCommand command)
    {
        GameObject go = new GameObject("Atom");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Atom");

        SphereCollider trigger = go.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.6f;

        Atom atom = go.AddComponent<Atom>();
        Material material = ElementoMaterials.Find("M_Shape_Diamond", "Elemento/NeonShape");

        // the nucleus
        MakeSphere("Nucleus", go.transform, Vector3.zero, 0.26f, material);

        // 3 rings: each one is tilted (to look like an ellipse from the side) and turned 60 degrees from the last
        atom.ElectronPivots = new Transform[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject orbit = new GameObject("Orbit" + i);
            orbit.transform.SetParent(go.transform, false);
            orbit.transform.localRotation = Quaternion.Euler(0f, 0f, 60f * i);

            GameObject tilt = new GameObject("Tilt");
            tilt.transform.SetParent(orbit.transform, false);
            tilt.transform.localRotation = Quaternion.Euler(70f, 0f, 0f);

            MakeRing(tilt.transform, 0.5f, material);

            GameObject pivot = new GameObject("ElectronPivot");
            pivot.transform.SetParent(tilt.transform, false);
            pivot.transform.localRotation = Quaternion.Euler(0f, 0f, 120f * i);
            MakeSphere("Electron", pivot.transform, new Vector3(0.5f, 0f, 0f), 0.11f, material);

            atom.ElectronPivots[i] = pivot.transform;
        }

        Selection.activeGameObject = go;
    }

    private static void MakeSphere(string name, Transform parent, Vector3 localPosition, float diameter, Material material)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        DestroyImmediate(sphere.GetComponent<Collider>());
        sphere.transform.SetParent(parent, false);
        sphere.transform.localPosition = localPosition;
        sphere.transform.localScale = Vector3.one * diameter;

        MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        if (material != null)
            renderer.sharedMaterial = material;
    }

    private static void MakeRing(Transform parent, float radius, Material material)
    {
        GameObject ring = new GameObject("Ring");
        ring.transform.SetParent(parent, false);

        LineRenderer line = ring.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.widthMultiplier = 0.035f;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;

        const int points = 40;
        line.positionCount = points;

        for (int i = 0; i < points; i++)
        {
            float a = (i / (float)points) * Mathf.PI * 2f;
            line.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f));
        }

        if (material != null)
            line.sharedMaterial = material;
    }
#endif
}
