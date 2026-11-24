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
        Pyramid,
        Wedge,      // a slope: a triangle seen from the side, pushed along Z (see "Wedge Size")
        Ring        // a flat hollow ring (see "Inner Radius"); Sides = how smooth it is (32 is good). Stand it up with Rotation X = 90
    }

    public ShapeType Shape = ShapeType.Prism;

    // Number of sides of the base: 6 = hexagon, 4 = square, 3 = triangle
    [Range(3, 32)] public int Sides = 6;

    // Distance from the center to a corner of the base (in world units, at scale 1)
    public float Radius = 1.5f;

    // Height of the shape
    public float Height = 0.4f;

    // RING ONLY: the radius of the hole (must be smaller than Radius). The ring is Radius minus Inner Radius wide.
    public float InnerRadius = 0.42f;

    // How many shapes in a row (along X)
    [Range(1, 30)] public int Count = 1;

    // Extra space between two shapes of the row (0 = they touch)
    public float Spacing = 0f;

    // Turns the base in degrees. For a square pyramid, 45 puts its flat sides along the axes.
    public float RotationOffset = 0f;

    // WEDGE ONLY: Width (X) x Height (Y) x Depth (Z). The slope rises toward +X.
    // 2 x 2 x 2 is a 45 degree slope. The pivot is at the bottom, centered in X and Z.
    // Flip the slope by setting the object's Scale X to -1 on the prefab ROOT's child, or rotate it 180 around Y.
    public Vector3 WedgeSize = new Vector3(2f, 2f, 2f);

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

        if (Shape == ShapeType.Wedge)
        {
            BuildWedge(meshFilter);
            return;
        }

        if (Shape == ShapeType.Ring)
        {
            BuildRing(meshFilter);
            return;
        }

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

    // A slope: bottom, a vertical wall on the +X side, the sloped top, and a triangle on the front and the back.
    private void BuildWedge(MeshFilter meshFilter)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector3> edgeData = new List<Vector3>();
        List<int> triangles = new List<int>();

        float w = Mathf.Max(WedgeSize.x, 0.01f) * 0.5f;
        float h = Mathf.Max(WedgeSize.y, 0.01f);
        float d = Mathf.Max(WedgeSize.z, 0.01f) * 0.5f;

        Vector3 lowFront = new Vector3(-w, 0f, -d);
        Vector3 lowBack = new Vector3(-w, 0f, d);
        Vector3 rightFront = new Vector3(w, 0f, -d);
        Vector3 rightBack = new Vector3(w, 0f, d);
        Vector3 topFront = new Vector3(w, h, -d);
        Vector3 topBack = new Vector3(w, h, d);

        Vector3 corner0 = new Vector3(1, 0, 0);
        Vector3 corner1 = new Vector3(0, 1, 0);
        Vector3 corner2 = new Vector3(0, 0, 1);

        // the two triangles (front faces the camera, back is behind): all 3 edges drawn
        AddTriangle(lowFront, rightFront, topFront, corner0, corner1, corner2, Vector3.back, vertices, normals, edgeData, triangles);
        AddTriangle(lowBack, rightBack, topBack, corner0, corner1, corner2, Vector3.forward, vertices, normals, edgeData, triangles);

        // the bottom, the vertical wall and the slope
        AddQuad(lowFront, rightFront, rightBack, lowBack, Vector3.down, vertices, normals, edgeData, triangles);
        AddQuad(rightFront, rightBack, topBack, topFront, Vector3.right, vertices, normals, edgeData, triangles);
        AddQuad(lowFront, lowBack, topBack, topFront, new Vector3(-h, w * 2f, 0f), vertices, normals, edgeData, triangles);

        Mesh mesh = new Mesh();
        mesh.name = MeshName;
        mesh.hideFlags = HideFlags.HideAndDontSave;
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(1, edgeData);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        Mesh old = meshFilter.sharedMesh;
        meshFilter.sharedMesh = mesh;

        if (old != null && old.name == MeshName)
            DestroyMesh(old);

        FitColliders(mesh, w * 2f, d * 2f);
    }

    // A flat hollow ring: a top and a bottom face, an outer wall and an inner wall.
    // Only the OUTLINES are drawn: the two circles on each face (no line between the small flat pieces of the ring).
    private void BuildRing(MeshFilter meshFilter)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector3> edgeData = new List<Vector3>();
        List<int> triangles = new List<int>();

        int segments = Mathf.Max(Sides, 6);
        float outer = Mathf.Max(Radius, 0.02f);
        float inner = Mathf.Clamp(InnerRadius, 0.01f, outer - 0.01f);
        float h = Mathf.Max(Height, 0.005f);
        Vector3 up = new Vector3(0f, h, 0f);

        for (int i = 0; i < segments; i++)
        {
            int j = (i + 1) % segments;

            float angleI = (RotationOffset + 360f * i / segments) * Mathf.Deg2Rad;
            float angleJ = (RotationOffset + 360f * j / segments) * Mathf.Deg2Rad;
            Vector3 dirI = new Vector3(Mathf.Cos(angleI), 0f, Mathf.Sin(angleI));
            Vector3 dirJ = new Vector3(Mathf.Cos(angleJ), 0f, Mathf.Sin(angleJ));
            Vector3 middle = (dirI + dirJ).normalized;

            Vector3 outerI = dirI * outer;
            Vector3 outerJ = dirJ * outer;
            Vector3 innerI = dirI * inner;
            Vector3 innerJ = dirJ * inner;

            // top face (outer circle and inner circle drawn), bottom face, outer wall, inner wall
            AddEdgeQuad(outerI + up, outerJ + up, innerJ + up, innerI + up, Vector3.up, vertices, normals, edgeData, triangles);
            AddEdgeQuad(outerI, outerJ, innerJ, innerI, Vector3.down, vertices, normals, edgeData, triangles);
            AddEdgeQuad(outerI, outerJ, outerJ + up, outerI + up, middle, vertices, normals, edgeData, triangles);
            AddEdgeQuad(innerI, innerJ, innerJ + up, innerI + up, -middle, vertices, normals, edgeData, triangles);
        }

        Mesh mesh = new Mesh();
        mesh.name = MeshName;
        mesh.hideFlags = HideFlags.HideAndDontSave;
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(1, edgeData);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        Mesh old = meshFilter.sharedMesh;
        meshFilter.sharedMesh = mesh;

        if (old != null && old.name == MeshName)
            DestroyMesh(old);

        // a ring is decoration: no collider is fitted
    }

    // A face made of 2 triangles where ONLY the first edge (a-b) and the opposite edge (d-c) are drawn.
    // The two short sides and the diagonal are not drawn (so a smooth circle shows no lines between its pieces).
    private static void AddEdgeQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 outward,
                                    List<Vector3> vertices, List<Vector3> normals, List<Vector3> edgeData, List<int> triangles)
    {
        AddTriangle(a, b, c,
                    new Vector3(1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1),
                    outward, vertices, normals, edgeData, triangles);

        AddTriangle(a, c, d,
                    new Vector3(1, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 1),
                    outward, vertices, normals, edgeData, triangles);
    }

    // A square face made of 2 triangles, the diagonal cut is not drawn.
    // Corners in order: b0 and b1 on one edge, t1 next to b1, t0 next to b0.
    private static void AddQuad(Vector3 b0, Vector3 b1, Vector3 t1, Vector3 t0, Vector3 outward,
                                List<Vector3> vertices, List<Vector3> normals, List<Vector3> edgeData, List<int> triangles)
    {
        AddTriangle(b0, b1, t1,
                    new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
                    outward, vertices, normals, edgeData, triangles);

        AddTriangle(b0, t1, t0,
                    new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1),
                    outward, vertices, normals, edgeData, triangles);
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
        if (Shape != ShapeType.Wedge && FitBoxCollider && TryGetComponent(out BoxCollider box))
        {
            float colliderHeight = Height * ColliderHeight;
            box.size = new Vector3(totalWidth, colliderHeight, depth);
            box.center = new Vector3(0f, colliderHeight * 0.5f, 0f);
        }

        // a Mesh Collider can only follow a single shape (a convex collider would fill the gaps of a row)
        if ((Count == 1 || Shape == ShapeType.Wedge) && TryGetComponent(out MeshCollider meshCollider))
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
        Material preferred = ElementoMaterials.Find("M_Level_Shape", "Elemento/NeonShape");

        if (preferred != null)
            return preferred;

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
    [MenuItem("GameObject/Elemento/Level/Bounce Pad", false, 11)]
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

    // A 45 degree slope for the foundation: a solid wedge, 2 x 2 x 2, on a convex Mesh Collider.
    // The player walks up it (the controller allows slopes up to 60 degrees).
    [MenuItem("GameObject/Elemento/Level/Slope 45", false, 13)]
    private static void CreateSlope45(MenuCommand command)
    {
        GameObject go = CreateShapeObject("Slope_45", command);

        // a slope is safe ground: use the normal level shape material, not the red pulsing one
        Material safe = ElementoMaterials.Find("M_Level_Shape", "Elemento/NeonShape");

        if (safe != null)
            go.GetComponent<MeshRenderer>().sharedMaterial = safe;

        NeonShapeMesh shape = go.AddComponent<NeonShapeMesh>();
        shape.Shape = ShapeType.Wedge;
        shape.WedgeSize = new Vector3(2f, 2f, 2f);

        MeshCollider solid = go.AddComponent<MeshCollider>();
        solid.convex = true;

        shape.Build();
        Selection.activeGameObject = go;
    }

    // A row of spikes (a trigger with the Hazard script).
    // It starts as TRIANGLES (3 sides = danger in your shape language).
    // For a pyramid: set Sides to 4 and Rotation Offset to 45 (or use the Inspector menu > "Make Pyramid").
    [MenuItem("GameObject/Elemento/Hazards/Spike Row", false, 12)]
    private static void CreateSpikeRow(MenuCommand command)
    {
        GameObject go = CreateShapeObject("SpikeRow", command);

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

    // Right click the component title > Make Pyramid: 4 sides, flat faces along the axes, exactly 1 unit wide
    [ContextMenu("Make Pyramid (4 sides)")]
    private void MakePyramid()
    {
        Shape = ShapeType.Pyramid;
        Sides = 4;
        RotationOffset = 45f;
        Radius = 0.7071f;
        Build();
    }

    // Right click the component title > Make Triangle Spikes (3 sides)
    [ContextMenu("Make Triangle Spikes (3 sides)")]
    private void MakeTriangle()
    {
        Shape = ShapeType.Pyramid;
        Sides = 3;
        RotationOffset = 90f;
        Radius = 0.6f;
        Build();
    }
#endif
}
