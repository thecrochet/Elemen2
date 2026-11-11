using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Builds a hexagon pad, a pyramid, a triangle spike... from CODE, with no ProBuilder and no imported model.
// You choose the shape and the number of sides in the Inspector, the mesh builds itself.
//
//   Shape  = Prism   (a flat-topped column: a hexagon pad is a Prism with 6 sides, a tall one is a pillar)
//            Pyramid (a point on top: spikes. 4 sides = a classic pyramid, 3 sides = a triangle spike)
//   Count  = how many shapes in a row (a whole spike pit is ONE object, ONE mesh)
//
// It works with the "Elemento/NeonShape" shader, which draws the glowing outline of every face.
// (The mesh stores, for each corner, how far it is from the edges of its face. The shader uses that to draw the lines.)
//
// COLLIDERS (the script can help)
//   - A Box Collider on the same object is resized to fit the whole row (Fit Box Collider).
//     Collider Height = how much of the height counts. 0.6 on spikes = a bit forgiving, only the lower part kills.
//   - A Mesh Collider on the same object (Convex ticked) gets the mesh when Count is 1.
//
// The mesh is rebuilt every time the scene starts, so nothing is saved in the scene file.
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NeonShapeMesh : MonoBehaviour
{
    public enum ShapeType
    {
        Prism,
        Pyramid
    }

    public ShapeType Shape = ShapeType.Prism;

    // Number of sides of the base: 6 = hexagon, 4 = square, 3 = triangle
    [Range(3, 12)] public int Sides = 6;

    // Distance from the center to a corner of the base (in world units, at scale 1)
    public float Radius = 1.5f;

    // Height of the shape
    public float Height = 0.4f;

    // How many shapes in a row (along X)
    [Range(1, 30)] public int Count = 1;

    // Extra space between two shapes of the row (0 = they touch)
    public float Spacing = 0f;

    // Turns the base in degrees. For a square pyramid, 45 puts its flat sides along the axes.
    public float RotationOffset = 0f;

    [Header("Colliders")]
    // Resize the Box Collider of this object to fit the row
    public bool FitBoxCollider = true;

    // How much of the height the Box Collider covers (1 = all, 0.6 = the lower 60%)
    [Range(0.2f, 1f)] public float ColliderHeight = 1f;

    private const string MeshName = "NeonShape (generated)";

    private void Awake()
    {
        Build();
    }

#if UNITY_EDITOR
    // Runs in the Editor every time you change a value in the Inspector
    private void OnValidate()
    {
        // building inside OnValidate is not allowed by Unity, so we wait one editor step
        EditorApplication.delayCall += () =>
        {
            if (this != null)
                Build();
        };
    }
#endif

    [ContextMenu("Rebuild")]
    public void Build()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector3> edgeData = new List<Vector3>();
        List<int> triangles = new List<int>();

        // the corners of the base polygon (on the floor, around the center)
        Vector3[] baseCorners = new Vector3[Sides];
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i < Sides; i++)
        {
            float angle = (RotationOffset + 360f * i / Sides) * Mathf.Deg2Rad;
            baseCorners[i] = new Vector3(Mathf.Cos(angle) * Radius, 0f, Mathf.Sin(angle) * Radius);

            minX = Mathf.Min(minX, baseCorners[i].x);
            maxX = Mathf.Max(maxX, baseCorners[i].x);
            minZ = Mathf.Min(minZ, baseCorners[i].z);
            maxZ = Mathf.Max(maxZ, baseCorners[i].z);
        }

        float shapeWidth = maxX - minX;
        float step = shapeWidth + Spacing;

        for (int c = 0; c < Count; c++)
        {
            // the row is centered on the pivot of the object
            Vector3 offset = new Vector3((c - (Count - 1) * 0.5f) * step - (minX + maxX) * 0.5f, 0f, -(minZ + maxZ) * 0.5f);

            if (Shape == ShapeType.Prism)
                AddPrism(baseCorners, offset, vertices, normals, edgeData, triangles);
            else
                AddPyramid(baseCorners, offset, vertices, normals, edgeData, triangles);
        }

        Mesh mesh = new Mesh();
        mesh.name = MeshName;
        mesh.hideFlags = HideFlags.HideAndDontSave;
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(1, edgeData);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        // remove the mesh we made last time (so nothing piles up in memory)
        Mesh old = meshFilter.sharedMesh;

        meshFilter.sharedMesh = mesh;

        if (old != null && old.name == MeshName)
            DestroyMesh(old);

        FitColliders(mesh, (Count - 1) * step + shapeWidth, maxZ - minZ);
    }

    // ------------------------------------------------------------------
    // THE SHAPES
    // ------------------------------------------------------------------
    // The "edge data" of a corner is 3 numbers. Each number says how far the corner is from one EDGE of its triangle
    // (1 on the corner opposite the edge, 0 on the edge itself). The shader looks at the smallest of the three to draw the line.
    // A number kept at 1 everywhere means "this edge is not drawn" (used for the diagonal cut of a square face).
    private void AddPrism(Vector3[] baseCorners, Vector3 offset, List<Vector3> vertices, List<Vector3> normals, List<Vector3> edgeData, List<int> triangles)
    {
        Vector3 up = new Vector3(0f, Height, 0f);
        Vector3 center = offset + up;

        for (int i = 0; i < Sides; i++)
        {
            int j = (i + 1) % Sides;

            Vector3 bottomI = baseCorners[i] + offset;
            Vector3 bottomJ = baseCorners[j] + offset;
            Vector3 topI = bottomI + up;
            Vector3 topJ = bottomJ + up;

            // the top: a fan of triangles from the center. Only the outer edge is drawn.
            AddTriangle(center, topI, topJ,
                        new Vector3(1, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 1),
                        Vector3.up, vertices, normals, edgeData, triangles);

            // the side: a square made of 2 triangles. The diagonal cut is not drawn.
            Vector3 outward = (bottomI + bottomJ - offset * 2f).normalized;

            AddTriangle(bottomI, bottomJ, topJ,
                        new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
                        outward, vertices, normals, edgeData, triangles);

            AddTriangle(bottomI, topJ, topI,
                        new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1),
                        outward, vertices, normals, edgeData, triangles);
        }
    }

    private void AddPyramid(Vector3[] baseCorners, Vector3 offset, List<Vector3> vertices, List<Vector3> normals, List<Vector3> edgeData, List<int> triangles)
    {
        Vector3 apex = offset + new Vector3(0f, Height, 0f);

        for (int i = 0; i < Sides; i++)
        {
            int j = (i + 1) % Sides;

            Vector3 bottomI = baseCorners[i] + offset;
            Vector3 bottomJ = baseCorners[j] + offset;

            // pointing away from the middle of the pyramid, and a bit up
            Vector3 outward = (bottomI + bottomJ - offset * 2f).normalized + Vector3.up;

            // a triangle with all 3 edges drawn
            AddTriangle(bottomI, bottomJ, apex,
                        new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1),
                        outward, vertices, normals, edgeData, triangles);
        }
    }

    // Adds one triangle. "outward" is the direction the face must look to: if the corners are in the wrong
    // order for that, two of them are swapped (a face is only visible/solid from the side its corners go clockwise).
    private static void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2,
                                    Vector3 e0, Vector3 e1, Vector3 e2,
                                    Vector3 outward,
                                    List<Vector3> vertices, List<Vector3> normals, List<Vector3> edgeData, List<int> triangles)
    {
        Vector3 faceNormal = Vector3.Cross(p1 - p0, p2 - p0);

        if (Vector3.Dot(faceNormal, outward) < 0f)
        {
            Vector3 tempPoint = p1;
            p1 = p2;
            p2 = tempPoint;

            Vector3 tempEdge = e1;
            e1 = e2;
            e2 = tempEdge;

            faceNormal = -faceNormal;
        }

        faceNormal.Normalize();

        int start = vertices.Count;

        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);

        normals.Add(faceNormal);
        normals.Add(faceNormal);
        normals.Add(faceNormal);

        edgeData.Add(e0);
        edgeData.Add(e1);
        edgeData.Add(e2);

        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
    }

    // ------------------------------------------------------------------
    // COLLIDERS
    // ------------------------------------------------------------------
    private void FitColliders(Mesh mesh, float totalWidth, float depth)
    {
        if (FitBoxCollider && TryGetComponent(out BoxCollider box))
        {
            float colliderHeight = Height * ColliderHeight;
            box.size = new Vector3(totalWidth, colliderHeight, depth);
            box.center = new Vector3(0f, colliderHeight * 0.5f, 0f);
        }

        // a Mesh Collider can only follow a single shape (a convex collider would fill the gaps of a row)
        if (Count == 1 && TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

    private void OnDestroy()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.name == MeshName)
            DestroyMesh(meshFilter.sharedMesh);
    }

    private static void DestroyMesh(Mesh mesh)
    {
        if (Application.isPlaying)
            Destroy(mesh);
        else
            DestroyImmediate(mesh);
    }

#if UNITY_EDITOR
    // ------------------------------------------------------------------
    // MENU: right click in the Hierarchy > Elemento
    // ------------------------------------------------------------------
    // The materials: the first material that uses the "Elemento/NeonShape" shader is picked automatically.
    private static Material FindNeonMaterial()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

            if (material != null && material.shader != null && material.shader.name == "Elemento/NeonShape")
                return material;
        }

        return null;
    }

    private static GameObject CreateShapeObject(string objectName, MenuCommand command)
    {
        GameObject go = new GameObject(objectName);
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + objectName);

        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        Material material = FindNeonMaterial();

        if (material != null)
            meshRenderer.sharedMaterial = material;

        return go;
    }

    // A hexagon bounce pad: a solid hexagon column + a trigger on top with the BouncePad script
    [MenuItem("GameObject/Elemento/Neon Hex Pad", false, 11)]
    private static void CreateHexPad(MenuCommand command)
    {
        GameObject go = CreateShapeObject("NeonHexPad", command);

        NeonShapeMesh shape = go.AddComponent<NeonShapeMesh>();
        shape.Shape = ShapeType.Prism;
        shape.Sides = 6;
        shape.Radius = 1.5f;
        shape.Height = 0.4f;

        // the solid pad (the player stands on it): a Mesh Collider with Convex ticked follows the hexagon exactly
        MeshCollider solid = go.AddComponent<MeshCollider>();
        solid.convex = true;

        // the trigger on the top: the BouncePad script wants about 0.5 units tall
        GameObject trigger = new GameObject("LaunchTrigger");
        trigger.transform.SetParent(go.transform, false);
        trigger.transform.localPosition = new Vector3(0f, 0.55f, 0f);

        BoxCollider triggerCollider = trigger.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(2f, 0.5f, 1.6f);   // stays inside the hexagon

        trigger.AddComponent<BouncePad>();

        shape.Build();
        Selection.activeGameObject = go;
    }

    // A pit of spikes: a row of 6 pyramids + a trigger with the Hazard script
    [MenuItem("GameObject/Elemento/Neon Spike Row", false, 12)]
    private static void CreateSpikeRow(MenuCommand command)
    {
        GameObject go = CreateShapeObject("NeonSpikeRow", command);

        NeonShapeMesh shape = go.AddComponent<NeonShapeMesh>();
        shape.Shape = ShapeType.Pyramid;
        shape.Sides = 4;
        shape.RotationOffset = 45f;
        shape.Radius = 0.7071f;     // 0.7071 = a pyramid exactly 1 unit wide
        shape.Height = 1f;
        shape.Count = 6;
        shape.ColliderHeight = 0.6f;

        BoxCollider box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;

        go.AddComponent<Hazard>();

        shape.Build();
        Selection.activeGameObject = go;
    }

    // A triangle spike (3 sides), for a more aggressive look
    [MenuItem("GameObject/Elemento/Neon Triangle Spike Row", false, 13)]
    private static void CreateTriangleSpikeRow(MenuCommand command)
    {
        GameObject go = CreateShapeObject("NeonTriangleSpikeRow", command);

        NeonShapeMesh shape = go.AddComponent<NeonShapeMesh>();
        shape.Shape = ShapeType.Pyramid;
        shape.Sides = 3;
        shape.RotationOffset = 90f;     // one corner points toward +Z, a flat side faces the camera
        shape.Radius = 0.6f;
        shape.Height = 1f;
        shape.Count = 6;
        shape.ColliderHeight = 0.6f;

        BoxCollider box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;

        go.AddComponent<Hazard>();

        shape.Build();
        Selection.activeGameObject = go;
    }
#endif
}
