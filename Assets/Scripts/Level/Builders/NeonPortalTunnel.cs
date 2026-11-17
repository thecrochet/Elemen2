using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// A portal with REAL depth: a tube (a tunnel) that goes away from the camera, built from code.
// You look down the tunnel through the mouth of the portal, so the camera's own perspective gives the depth.
//
// HOW IT IS BUILT (the menu GameObject > Elemento > Neon Portal Tunnel does all of this for you)
//   Root object  = this script + a MeshFilter + a MeshRenderer (the tunnel, material with "Elemento/Portal/TunnelWall")
//   Child "Frame" = a Quad with the Portal_Frame material (Shape: Circle or Hexagon) at the mouth of the tunnel
//
// DIRECTION: the tunnel goes along +Z (away from a camera that looks along +Z). The Frame is at the mouth (z = 0).
// Put the portal in front of a dark wall or a gap: the tunnel is only seen through the mouth.
//
// MATCHING THE SHAPE: the tunnel and the Frame material must agree.
//   Circle  -> Sides 32, Frame Ring Radius 0.55   (right click the component title > Match Circle)
//   Hexagon -> Sides 6,  Frame Ring Radius 0.75   (Match Hexagon, then set the material Shape to Hexagon and Size to 0.75)
// The Arch shape has no tunnel: use it on its own (start arch).
//
// The mesh is rebuilt every time the scene starts, so nothing is saved in the scene file.
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NeonPortalTunnel : MonoBehaviour
{
    [Header("Tunnel")]
    // Radius of the mouth (world units, at scale 1)
    public float Radius = 1f;

    // How far the tunnel goes (world units)
    public float Depth = 8f;

    // Number of sides: 32 = a round tube, 6 = a hexagon tunnel, 4 = square
    [Range(3, 64)] public int Sides = 32;

    // Turns the tube around its axis (degrees). 30 on a 6 sided tunnel points a flat side up.
    public float RotationOffset = 0f;

    [Header("Frame (the transparent portal at the mouth)")]
    // The Quad with the portal material. Leave empty if you don't want one.
    public Transform Frame;

    // Resize the Frame automatically so the ring of the portal sits on the mouth of the tunnel
    public bool AutoFitFrame = true;

    // The "Size" of the Portal_Frame material (Circle 0.55, Hexagon 0.75)
    public float FrameRingRadius = 0.55f;

    private const string MeshName = "NeonPortalTunnel (generated)";

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

    [ContextMenu("Match Circle")]
    private void MatchCircle()
    {
        Sides = 32;
        RotationOffset = 0f;
        FrameRingRadius = 0.55f;
        Build();
    }

    [ContextMenu("Match Hexagon")]
    private void MatchHexagon()
    {
        Sides = 6;
        RotationOffset = 0f;
        FrameRingRadius = 0.75f;
        Build();
    }

    [ContextMenu("Rebuild")]
    public void Build()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv0 = new List<Vector2>();
        List<Vector2> uv1 = new List<Vector2>();
        List<int> triangles = new List<int>();

        float depth = Mathf.Max(Depth, 0.1f);
        float radius = Mathf.Max(Radius, 0.05f);

        // ---------------- THE WALLS ----------------
        // two corners for each side of the polygon (one at the mouth, one at the far end);
        // we make Sides + 1 pairs, so the last pair repeats the first one (it closes the tube with correct uv)
        for (int i = 0; i <= Sides; i++)
        {
            float angle = (RotationOffset + 360f * i / Sides) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            vertices.Add(new Vector3(x, y, 0f));
            uv0.Add(new Vector2(i, 0f));
            uv1.Add(new Vector2(1f, 0f));

            vertices.Add(new Vector3(x, y, depth));
            uv0.Add(new Vector2(i, depth));
            uv1.Add(new Vector2(1f, 0f));
        }

        for (int i = 0; i < Sides; i++)
        {
            int nearA = i * 2;
            int farA = nearA + 1;
            int nearB = nearA + 2;
            int farB = nearA + 3;

            // the direction pointing OUT of the tube at this side (the winding is fixed against it)
            float midAngle = (RotationOffset + 360f * (i + 0.5f) / Sides) * Mathf.Deg2Rad;
            Vector3 outward = new Vector3(Mathf.Cos(midAngle), Mathf.Sin(midAngle), 0f);

            AddTriangle(vertices, triangles, nearA, farA, nearB, outward);
            AddTriangle(vertices, triangles, nearB, farA, farB, outward);
        }

        // ---------------- THE FAR END (cap) ----------------
        // a fan of triangles: one vertex in the middle, the others on the border
        int centerIndex = vertices.Count;
        vertices.Add(new Vector3(0f, 0f, depth));
        uv0.Add(new Vector2(0f, depth));
        uv1.Add(new Vector2(0f, 1f));

        int firstRim = vertices.Count;

        for (int i = 0; i <= Sides; i++)
        {
            float angle = (RotationOffset + 360f * i / Sides) * Mathf.Deg2Rad;

            vertices.Add(new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, depth));
            uv0.Add(new Vector2(i, depth));
            uv1.Add(new Vector2(1f, 1f));
        }

        for (int i = 0; i < Sides; i++)
            AddTriangle(vertices, triangles, centerIndex, firstRim + i, firstRim + i + 1, Vector3.forward);

        // ---------------- THE MESH ----------------
        Mesh mesh = new Mesh();
        mesh.name = MeshName;
        mesh.hideFlags = HideFlags.HideAndDontSave;
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv0);
        mesh.SetUVs(1, uv1);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        // remove the mesh we made last time (so nothing piles up in memory)
        Mesh old = meshFilter.sharedMesh;
        meshFilter.sharedMesh = mesh;

        if (old != null && old.name == MeshName)
            DestroyMesh(old);

        // ---------------- THE FRAME ----------------
        if (Frame != null && AutoFitFrame)
        {
            // the Quad covers 2.5 "shape units" and the ring sits at FrameRingRadius of them
            float size = radius * 2.5f / Mathf.Max(FrameRingRadius, 0.05f);

            Frame.localPosition = Vector3.zero;
            Frame.localRotation = Quaternion.identity;
            Frame.localScale = new Vector3(size, size, 1f);
        }
    }

    // Adds a triangle so that its FRONT side points toward "outward".
    // (Unity: the front of a triangle is the side that Cross(p1 - p0, p2 - p0) points to.)
    // The tunnel shader uses Cull Front, so it shows the BACK side: the inside of the tube.
    private static void AddTriangle(List<Vector3> vertices, List<int> triangles, int a, int b, int c, Vector3 outward)
    {
        Vector3 normal = Vector3.Cross(vertices[b] - vertices[a], vertices[c] - vertices[a]);

        if (Vector3.Dot(normal, outward) >= 0f)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
        else
        {
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);
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
    // MENU: right click in the Hierarchy > Elemento > Neon Portal Tunnel
    // ------------------------------------------------------------------
    // Finds the first material that uses the given shader name (or one that starts with it).
    private static Material FindMaterial(string shaderName, string excludeContains)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

            if (material == null || material.shader == null)
                continue;

            string name = material.shader.name;

            if (name.StartsWith(shaderName) && (string.IsNullOrEmpty(excludeContains) || !name.Contains(excludeContains)))
                return material;
        }

        return null;
    }

    [MenuItem("GameObject/Elemento/Flow/Portal", false, 14)]
    private static void CreatePortalTunnel(MenuCommand command)
    {
        GameObject go = new GameObject("Portal");
        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Portal");

        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        // the tunnel material: the first one found, or a new one saved in Assets
        Material tunnelMaterial = FindMaterial("Elemento/Portal/TunnelWall", null);

        if (tunnelMaterial == null)
        {
            Shader shader = Shader.Find("Elemento/Portal/TunnelWall");

            if (shader != null)
            {
                tunnelMaterial = new Material(shader);
                AssetDatabase.CreateAsset(tunnelMaterial, "Assets/M_Portal_Tunnel.mat");
            }
        }

        meshRenderer.sharedMaterial = tunnelMaterial;

        NeonPortalTunnel tunnel = go.AddComponent<NeonPortalTunnel>();

        // the Frame: a Quad with the Portal_Frame material (the first one found, or a new one saved in Assets)
        GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
        frame.name = "Frame";
        frame.transform.SetParent(go.transform, false);

        // the portal is only for looking at: no collider
        DestroyImmediate(frame.GetComponent<Collider>());

        MeshRenderer frameRenderer = frame.GetComponent<MeshRenderer>();
        frameRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        frameRenderer.receiveShadows = false;

        Material frameMaterial = FindMaterial("Elemento/Portal/Frame", null);

        if (frameMaterial == null)
        {
            Shader frameShader = Shader.Find("Elemento/Portal/Frame");

            if (frameShader != null)
            {
                frameMaterial = new Material(frameShader);
                AssetDatabase.CreateAsset(frameMaterial, "Assets/M_Portal_Frame.mat");
            }
        }

        if (frameMaterial != null)
            frameRenderer.sharedMaterial = frameMaterial;

        tunnel.Frame = frame.transform;
        tunnel.Build();

        Selection.activeGameObject = go;
    }
#endif
}
