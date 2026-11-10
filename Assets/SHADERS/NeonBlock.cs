using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// A modular block: you type HOW MANY boxes you want, the block builds itself.
//   Size X = boxes in a row (length)   Size Y = boxes tall   Size Z = boxes deep
//
// Why not duplicate one cube per box?
//   - ONE collider for the whole row = no seams between the boxes, so the ball never snags on a joint.
//   - ONE object = a lot less to move, copy and edit.
//   - The shader draws the lines between the boxes (the grid), so it still looks like separate boxes.
//
// STRUCTURE (the menu GameObject > Elemento > Neon Block builds it for you):
//   NeonBlock          this script + a Box Collider. Its own scale stays at 1, so a child
//    |                 (spikes, a pad, a pickup) is never stretched.
//    +- Visual         a cube with the NeonGrid material. This script scales it.
//
// Keep Cell Size the same as "Grid Cell Size" in the NeonGrid material (both 1 by default),
// so one grid cell = one box.
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class NeonBlock : MonoBehaviour
{
    // Where the object's pivot sits. It decides which way the block grows when you add boxes.
    public enum AnchorType
    {
        Center,     // grows to both sides
        Left,       // the pivot is the LEFT edge: adding boxes grows the row to the right
        TopLeft     // the pivot is the TOP-LEFT corner: grows to the right and downward
    }

    // Number of boxes on each axis
    public Vector3Int Size = new Vector3Int(4, 1, 1);

    // The size of one box in world units (1 = the same as the grid)
    public float CellSize = 1f;

    public AnchorType Anchor = AnchorType.Left;

    // The cube with the neon material (a child)
    public Transform Visual;

    private void Awake()
    {
        Apply();
    }

    // Runs in the Editor every time you change a value in the Inspector
    private void OnValidate()
    {
        Apply();
    }

    // Puts the collider and the cube at the right size and place
    public void Apply()
    {
        // at least one box on each axis
        Size = new Vector3Int(Mathf.Max(1, Size.x), Mathf.Max(1, Size.y), Mathf.Max(1, Size.z));
        CellSize = Mathf.Max(0.01f, CellSize);

        Vector3 size = new Vector3(Size.x, Size.y, Size.z) * CellSize;

        // the center of the block, relative to the pivot
        Vector3 center = Vector3.zero;

        if (Anchor == AnchorType.Left || Anchor == AnchorType.TopLeft)
            center.x = size.x * 0.5f;

        if (Anchor == AnchorType.TopLeft)
            center.y = -size.y * 0.5f;

        if (TryGetComponent(out BoxCollider box))
        {
            box.size = size;
            box.center = center;
        }

        if (Visual != null)
        {
            Visual.localPosition = center;
            Visual.localRotation = Quaternion.identity;
            Visual.localScale = size;
        }
    }

#if UNITY_EDITOR
    // Right click in the Hierarchy: Elemento > Neon Block
    [MenuItem("GameObject/Elemento/Neon Block", false, 10)]
    private static void CreateNeonBlock(MenuCommand command)
    {
        GameObject root = new GameObject("NeonBlock");
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(root, "Create Neon Block");

        // the visual cube, without its own collider (the root has the only collider)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        DestroyImmediate(visual.GetComponent<BoxCollider>());
        visual.transform.SetParent(root.transform, false);

        // neon lines don't need shadows
        MeshRenderer meshRenderer = visual.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        // adding the script also adds the Box Collider (RequireComponent)
        NeonBlock block = root.AddComponent<NeonBlock>();
        block.Visual = visual.transform;
        block.Apply();

        Selection.activeGameObject = root;
    }
#endif
}
