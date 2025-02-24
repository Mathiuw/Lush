using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;

    [SerializeField] int size = 20;
    [SerializeField] bool showGizmos = false;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void CreateShape() 
    {
        vertices = new Vector3[(size + 1) * (size + 1)];

        for (int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 1.25f;
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        uv = new Vector2[vertices.Length];

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        triangles = new int[size * size * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size + 1;
                triangles[tris + 5] = vert + size + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (vertices == null) return;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.01f);
            }
        }
    }
}