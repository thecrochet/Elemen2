using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// A zone of scrolling chevron arrows (>>>) that says "go this way, fast". Made for the fire world.
//
// FOUR MODES (field "Mode"):
//   Hint   = only the look. The zone has a Direction and knows when the player is inside (PlayerInside).
//   Lane   = while the ball is inside, its dash always goes along the arrows (needs the Fire power). Leave the zone: free dash again.
//   SuperDash = entering loads the NEXT dash to go Super Dash Multiplier times further (2 = double). Until the ball lands or dies. Needs the Fire power.
//   Charge = entering gives extra dashes (Charge Count, 1 = one more dash). They last until the ball lands or dies. Needs the Fire power.
//   Hold = the BARREL. The ball is caught, hovers in the middle of the zone and waits. JUMP throws it along the arrows
//          (Hold Launch Speed for Hold Launch Time, then it keeps Hold Exit Speed). After Hold Max Time it is thrown by itself.
//          The dash is given back. The zone can move (put it on a moving platform) or sit right in front of another one.
//   Boost = the DASH RING. The moment the ball enters, it is thrown along the arrows at Boost Speed for Boost Time,
//           like a dash (no gravity, it needs no power), and the dash is given back. When the boost ends the ball KEEPS
//           Exit Speed for a moment (as long as the player keeps pushing the same way), so the speed carries on.
//           It needs the updated CC_ELEMENTO (BoostLaunch).
//
// STRUCTURE (the menu GameObject > Elemento > Interactables > Dash Zone builds it):
//   DashZone        this script + a Box Collider [trigger]
//    +- Arrows     a Quad with the Elemento/DashZone material (the picture only)
//
// The arrows point along the RED arrow (+X) of the DashZone object: rotate it around Z to aim it (0 = right, 180 = left, 90 = up).
// Resize with the Size field, never by scaling the root.
[DisallowMultipleComponent]
public class DashZone : MonoBehaviour
{
    public enum ZoneMode
    {
        Hint,
        Boost,
        Lane,
        Charge,
        SuperDash,
        Hold     // appended last, keep the order
    }

    public ZoneMode Mode = ZoneMode.Hint;

    // Width (along the arrows) and height of the zone, in world units
    public Vector2 Size = new Vector2(6f, 3f);

    [Header("Super Dash mode only")]
    // How much further the next dash goes (2 = double: about 8.4 units instead of 4.2)
    public float SuperDashMultiplier = 2f;

    [Header("Charge mode only")]
    // How many extra dashes the zone gives (on top of the normal one)
    public int ChargeCount = 1;

    [Header("Boost mode only")]
    // Units per second while the boost lasts (a normal dash is 28)
    public float BoostSpeed = 26f;

    // How long the boost lasts, in seconds. Distance = speed x time (26 x 0.25 = 6.5 units)
    public float BoostTime = 0.25f;

    // The speed you keep when the boost ends (the walking speed is 11)
    public float ExitSpeed = 18f;

    [Header("Hold mode only")]
    // Units per second while the ball flies out of the zone
    public float HoldLaunchSpeed = 26f;

    // How long the throw lasts, in seconds
    public float HoldLaunchTime = 0.25f;

    // The speed you keep after the throw (as long as you keep pushing the same way)
    public float HoldExitSpeed = 18f;

    // After this many seconds the zone throws the ball by itself (0 = waits for ever)
    public float HoldMaxTime = 4f;

    // Plays when the ball is boosted or charged (optional, Play On Awake off, Looping off)
    public ParticleSystem BoostEffect;

    // The picture (a child Quad). Filled by the menu.
    public Transform Arrows;

    // True while the player is inside the zone
    public bool PlayerInside { get; private set; }

    // Where the arrows point, in world space
    public Vector3 Direction
    {
        get { return transform.right; }
    }

    private void Awake()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    // Puts the collider and the picture at the size you chose
    public void Apply()
    {
        Size = new Vector2(Mathf.Max(0.5f, Size.x), Mathf.Max(0.5f, Size.y));
        ChargeCount = Mathf.Clamp(ChargeCount, 1, 5);
        SuperDashMultiplier = Mathf.Clamp(SuperDashMultiplier, 1.5f, 4f);

        if (TryGetComponent(out BoxCollider box))
        {
            box.isTrigger = true;
            box.size = new Vector3(Size.x, Size.y, 2f);
            box.center = Vector3.zero;
        }

        if (Arrows != null)
        {
            Arrows.localPosition = Vector3.zero;
            Arrows.localRotation = Quaternion.identity;
            Arrows.localScale = new Vector3(Size.x, Size.y, 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;

        PlayerInside = true;
        player.ActivateBoostImpulse();

        if (Mode == ZoneMode.Lane)
        {
            player.EnterLane(this, Direction);
            Sfx.PlayAt(SfxType.LaneEnter, transform.position);
            PlayEffect(other.bounds.center);
        }

        if (Mode == ZoneMode.SuperDash)
        {
            player.AddSuperDash(SuperDashMultiplier);
            Sfx.Play(SfxType.ChargeGain);
            PlayEffect(other.bounds.center);
        }

        if (Mode == ZoneMode.Charge)
        {
            player.AddDashCharge(ChargeCount);
            Sfx.Play(SfxType.ChargeGain);
            PlayEffect(other.bounds.center);
        }

        if (Mode == ZoneMode.Hold)
        {
            player.EnterHold(this);
            PlayEffect(other.bounds.center);
        }

        if (Mode == ZoneMode.Boost)
        {
            player.BoostLaunch(Direction, BoostSpeed, BoostTime, ExitSpeed);
            Sfx.Play(SfxType.DashBoost);

            PlayEffect(other.bounds.center);
        }
    }

    private void PlayEffect(Vector3 position)
    {
        if (BoostEffect == null) return;
        BoostEffect.transform.position = position;
        BoostEffect.Play(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CC_ELEMENTO player))
        {
            PlayerInside = false;
            player.ExitLane(this);
            player.ExitHoldZone(this);
        }
    }

    // after a respawn the ball is somewhere else
    private void OnDisable()
    {
        PlayerInside = false;
    }

#if UNITY_EDITOR
    [ContextMenu("Point Right")] private void PointRight() { transform.localRotation = Quaternion.Euler(0, 0, 0); }
    [ContextMenu("Point Up")] private void PointUp() { transform.localRotation = Quaternion.Euler(0, 0, 90); }
    [ContextMenu("Point Left")] private void PointLeft() { transform.localRotation = Quaternion.Euler(0, 0, 180); }
    [ContextMenu("Point Down")] private void PointDown() { transform.localRotation = Quaternion.Euler(0, 0, 270); }

    [MenuItem("GameObject/Elemento/Interactables/Dash Lane", false, 32)]
    private static void CreateDashLane(MenuCommand command)
    {
        CreateDashZone(command);
        DashZone zone = Selection.activeGameObject.GetComponent<DashZone>();
        zone.gameObject.name = "DashLane";
        zone.Mode = ZoneMode.Lane;
        zone.Size = new Vector2(8f, 3f);
        zone.Apply();
    }

    [MenuItem("GameObject/Elemento/Interactables/Dash Hold", false, 35)]
    private static void CreateDashHold(MenuCommand command)
    {
        CreateDashZone(command);
        DashZone zone = Selection.activeGameObject.GetComponent<DashZone>();
        zone.gameObject.name = "DashHold";
        zone.Mode = ZoneMode.Hold;
        zone.Size = new Vector2(2f, 2f);
        zone.Apply();
    }

    [MenuItem("GameObject/Elemento/Interactables/Dash Super", false, 34)]
    private static void CreateDashSuper(MenuCommand command)
    {
        CreateDashZone(command);
        DashZone zone = Selection.activeGameObject.GetComponent<DashZone>();
        zone.gameObject.name = "DashSuper";
        zone.Mode = ZoneMode.SuperDash;
        zone.Size = new Vector2(2f, 2f);
        zone.Apply();
    }

    [MenuItem("GameObject/Elemento/Interactables/Dash Charge", false, 33)]
    private static void CreateDashCharge(MenuCommand command)
    {
        CreateDashZone(command);
        DashZone zone = Selection.activeGameObject.GetComponent<DashZone>();
        zone.gameObject.name = "DashCharge";
        zone.Mode = ZoneMode.Charge;
        zone.Size = new Vector2(2f, 2f);
        zone.Apply();
    }

    // The dash ring: a smaller zone (3 x 3) in Boost mode
    [MenuItem("GameObject/Elemento/Interactables/Dash Boost", false, 31)]
    private static void CreateDashBoost(MenuCommand command)
    {
        CreateDashZone(command);
        DashZone zone = Selection.activeGameObject.GetComponent<DashZone>();
        zone.gameObject.name = "DashBoost";
        zone.Mode = ZoneMode.Boost;
        zone.Size = new Vector2(3f, 3f);
        zone.Apply();
    }

    [MenuItem("GameObject/Elemento/Interactables/Dash Zone", false, 30)]
    private static void CreateDashZone(MenuCommand command)
    {
        GameObject go = new GameObject("DashZone");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Dash Zone");

        go.AddComponent<BoxCollider>();
        DashZone zone = go.AddComponent<DashZone>();

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "Arrows";
        DestroyImmediate(quad.GetComponent<Collider>());
        quad.transform.SetParent(go.transform, false);

        MeshRenderer renderer = quad.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        Material material = ElementoMaterials.Find("M_DashZone_Fire", "Elemento/DashZone");

        if (material != null)
            renderer.sharedMaterial = material;

        zone.Arrows = quad.transform;
        zone.Apply();
        Selection.activeGameObject = go;
    }
#endif

#if UNITY_EDITOR
    // Scene view (select the zone): the arrow direction, and what the zone does
    private void OnDrawGizmosSelected()
    {
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Color color = Mode == ZoneMode.Boost ? new Color(1f, 0.6f, 0.1f, 0.9f) : new Color(1f, 0.9f, 0.3f, 0.8f);
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(Size.x, Size.y, 0.2f));

        // the arrow along the zone (local +X)
        float half = Size.x * 0.4f;
        Gizmos.DrawLine(new Vector3(-half, 0f, 0f), new Vector3(half, 0f, 0f));
        Gizmos.DrawLine(new Vector3(half, 0f, 0f), new Vector3(half - 0.4f, 0.3f, 0f));
        Gizmos.DrawLine(new Vector3(half, 0f, 0f), new Vector3(half - 0.4f, -0.3f, 0f));

        Gizmos.matrix = old;

        string text = Mode.ToString();

        if (Mode == ZoneMode.Boost)
        {
            float distance = BoostSpeed * BoostTime;
            Vector3 end = transform.position + Direction * distance;

            Gizmos.DrawLine(transform.position, end);
            Gizmos.DrawWireSphere(end, 0.25f);
            text += ": " + distance.ToString("0.0") + " u at " + BoostSpeed.ToString("0") + " u/s, exit " + ExitSpeed.ToString("0") + " u/s";
        }
        else if (Mode == ZoneMode.SuperDash)
        {
            text += ": next dash x" + SuperDashMultiplier.ToString("0.0");
        }
        else if (Mode == ZoneMode.Hold)
        {
            text += ": Jump throws it, " + HoldLaunchSpeed.ToString("0") + " u/s x " + HoldLaunchTime.ToString("0.00") + " s (auto after " + HoldMaxTime.ToString("0") + " s)";
        }
        else if (Mode == ZoneMode.Charge)
        {
            text += ": +" + ChargeCount + " dash";
        }

        UnityEditor.Handles.Label(transform.position + Vector3.up * (Size.y * 0.5f + 0.4f), text);
    }
#endif
}
