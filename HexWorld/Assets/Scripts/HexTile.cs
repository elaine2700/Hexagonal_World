using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Modified script from: https://www.youtube.com/watch?v=EPaSmQ2vtek
public struct Face
{
    public List<Vector3> vertices {  get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> _vertices, List<int> _triangles, List<Vector2> _uvs)
    {
        this.vertices = _vertices;
        this.triangles = _triangles;
        this.uvs = _uvs;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class HexTile : MonoBehaviour
{
    Mesh mesh;
    MeshFilter filter;
    MeshRenderer meshRenderer;
    
    List<Face> faces;

    public Material material;
    public float innerSize = 1f; // innerRadius
    public float outerSize = 0f; // outerRadius
    public float height = 1f;
    public bool isFlatTopped = false;
 

    private void DrawFaces()
    {
        Debug.Log("Drawing Faces");
        faces = new();

        // Top faces.
        for(int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerSize, outerSize, height, height, point));
        }

        // Bottom Faces
        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, true));
        }

        // Outer Faces
        for( int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(outerSize, outerSize, height, 0f, point, true));
        }

        // Inner Faces
        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerSize, innerSize, height, 0f, point, false));
        }
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1:0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>()
        {
            pointA, pointB, pointC, pointD
        };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        if(reverse)
            vertices.Reverse();
        
        return new Face(vertices, triangles, uvs);
    }

    private Vector3 GetPoint(float size, float height, int index)
    {
        // internal angle of hexagon is 60 -> 60deg * 6 = 360deg
        float angleDeg = isFlatTopped ? 60 * index : 60 * index - 30;
        float angleRad = Mathf.PI / 180f * angleDeg;
        return new Vector3(
            size * Mathf.Cos(angleRad),
            height,
            size * Mathf.Sin(angleRad));
    }

    private void CombineFaces()
    {
        Debug.Log("Combining Faces");
        List<Vector3> vertices = new();
        List<int> tris = new();
        List<Vector2> uvs = new();

        for (int i = 0; i < faces.Count; i++)
        {
            // Add the vertices.
            vertices.AddRange(faces[i].vertices);
            uvs.AddRange(faces[i].uvs);

            // Offset the triangles.
            int offset = (4 * i);
            foreach(int triangle in faces[i].triangles)
            {
                tris.Add(triangle + offset);
            }
        }

        // Asing vertices and triangles to mesh.
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    public void DrawMesh()
    {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "Hex";
        filter.mesh = mesh;

        meshRenderer.material = material;
        meshRenderer.enabled = true;

        DrawFaces();
        CombineFaces();
    }

    public void AddCollider()
    {
        MeshCollider collider = this.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

    }
}
