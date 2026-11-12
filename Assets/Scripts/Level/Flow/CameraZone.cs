using Unity.Cinemachine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// A box in the level that changes how the camera frames the action while the player is inside it:
// zoom out for a big jump section, zoom in for a tight precision room, look higher for a tall climb...
//
// HOW TO USE
//   1) Menu: Hierarchy > Elemento > Camera Zone (it creates a box with the trigger already ticked).
//   2) Scale the box to cover the area. The blue wire box in the Scene view shows it.
//   3) Set the Camera Distance and/or the Target Offset you want inside it.
//   4) Done. There is nothing to connect: the zone finds the player's camera by itself.
//
// WHAT IT CHANGES (on your Cinemachine Position Composer)
//   - Camera Distance: how far the camera is (bigger = you see more).
//   - Target Offset:   moves the framing (for example Y 3 = the player sits lower on the screen).
//
// RULES
//   - Leaving the zone gives back the normal framing, smoothly.
//   - If two zones overlap, the one you entered LAST wins. When you leave it, the other one takes over.
//   - After a death the camera goes straight back to the normal framing (the retry stays instant).
//     If you respawn inside a zone, it blends in again.
//   - The player's controls are not affected: only the camera moves.
[RequireComponent(typeof(BoxCollider))]
public class CameraZone : MonoBehaviour, IResettable
{
    [Header("Camera Distance")]
    // tick to change the distance inside this zone
    public bool ChangeDistance = true;

    // the distance inside the zone (your normal one is on the Position Composer, for example 32)
    public float CameraDistance = 40f;

    [Header("Target Offset")]
    // tick to change the framing offset inside this zone
    public bool ChangeOffset = false;

    // the offset inside the zone (your normal one is on the Position Composer, for example 0, 1, 0)
    public Vector3 TargetOffset = new Vector3(0f, 1f, 0f);

    [Header("Blend")]
    // seconds the camera takes to reach these values (smaller = snappier)
    [Min(0.05f)] public float BlendTime = 0.8f;

    // ------------------------------------------------------------------
    // PLAYER ENTERS / LEAVES
    // ------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // only the player (TryGetComponent doesn't create garbage)
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;
        if (player.PlayerCamera == null) return;

        CameraZoneBlender.Get(player.PlayerCamera)?.Enter(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out CC_ELEMENTO player)) return;
        if (player.PlayerCamera == null) return;

        CameraZoneBlender.Get(player.PlayerCamera)?.Exit(this);
    }

    // ------------------------------------------------------------------
    // RESPAWN (same "save / load" idea as pickups and breakable blocks)
    // ------------------------------------------------------------------
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

    // nothing to save: a zone never changes
    public void CommitState()
    {
    }

    // after a death: the camera goes back to normal at once. If the player respawns inside a zone, the
    // trigger fires again and the blend restarts.
    public void ResetObject()
    {
        if (CameraZoneBlender.Current != null)
            CameraZoneBlender.Current.ClearAll();
    }

    // when you add the component: the box must be a trigger
    private void Reset()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

#if UNITY_EDITOR
    // the wire box in the Scene view
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.15f);
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 1f);
        Gizmos.DrawWireCube(box.center, box.size);
    }

    [MenuItem("GameObject/Elemento/Flow/Camera Zone", false, 15)]
    private static void CreateCameraZone(MenuCommand command)
    {
        GameObject go = new GameObject("CameraZone");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Camera Zone");

        BoxCollider box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(20f, 12f, 10f);

        go.AddComponent<CameraZone>();
        Selection.activeGameObject = go;
    }
#endif
}
