using UnityEngine;
using System.Collections;
using System;

public class MeshMap {

    public static void BuildMesh(int size_x, int size_z, int tileSize)
    {
        int numTiles = size_x * size_z;
        int numTris = numTiles * 2;

        int vsize_x = size_x + 1;
        int vsize_z = size_z + 1;
        int numVerts = vsize_x * vsize_z;

        // Generate the mesh data
        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];

        int x, z;
        for (z = 0; z < vsize_z; z++)
        {
            for (x = 0; x < vsize_x; x++)
            {
                vertices[z * vsize_x + x] = new Vector3(x * tileSize, 0, z * tileSize);
                normals[z * vsize_x + x] = Vector3.up;
                uv[z * vsize_x + x] = new Vector2((float)x / size_x, (float)z / size_z);
            }
        }
        Debug.Log("Done Verts!");

        for (z = 0; z < size_z; z++)
        {
            for (x = 0; x < size_x; x++)
            {
                int squareIndex = z * size_x + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 0] = z * vsize_x + x + 0;
                triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 0;
                triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 1;

                triangles[triOffset + 3] = z * vsize_x + x + 0;
                triangles[triOffset + 4] = z * vsize_x + x + vsize_x + 1;
                triangles[triOffset + 5] = z * vsize_x + x + 1;
            }
        }

        Debug.Log("Done Triangles!");

        // Add a Random Wall!
        MeshWall mw = new MeshWall();

        // Create a new Mesh and populate with the data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        
        // Assign our mesh to our filter/renderer/collider
        GameObject builder = GameObject.Find("MapBuilder");

        MeshFilter mesh_filter = builder.GetComponent<MeshFilter>();
        MeshCollider mesh_collider = builder.GetComponent<MeshCollider>();

        mesh_filter.mesh = mesh;
        mesh_collider.sharedMesh = mesh;
        Debug.Log("Done Mesh!");

        //BuildTexture(size_x, size_z, tileSize, 1);
    }

    public static void BuildTexture(int size_x, int size_z, int tileSize, int tileResolution)
    {
        int texWidth = size_x * tileResolution;
        int texHeight = size_z * tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);

        //Color[][] tiles = ChopUpTiles();

        for (int y = 0; y < size_z; y++)
        {
            for (int x = 0; x < size_x; x++)
            {
                //Color[] p = tiles[map.GetTileAt(x, y)];
                //texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, p);
                texture.SetPixel(x * tileResolution, y * tileResolution, Color.white);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        GameObject builder = GameObject.Find("MapBuilder");

        MeshRenderer mesh_renderer = builder.GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterials[0].mainTexture = texture;

        Debug.Log("Done Texture!");
    }

    public static Vector3[] mergeArray(Vector3[] a1, Vector3[] a2)
    {
        Vector3[] newArray = new Vector3[a1.Length + a2.Length];
        Array.Copy(a1, newArray, 0);
        Array.Copy(a2, 0, newArray, a1.Length, a2.Length);
        return newArray;
    }

    public static Vector2[] mergeArray(Vector2[] a1, Vector2[] a2)
    {
        Vector2[] newArray = new Vector2[a1.Length + a2.Length];
        Array.Copy(a1, newArray, 0);
        Array.Copy(a2, 0, newArray, a1.Length, a2.Length);
        return newArray;
    }

    public static int[] mergeArray(int[] a1, int[] a2)
    {
        int[] newArray = new int[a1.Length + a2.Length];
        Array.Copy(a1, newArray, 0);
        Array.Copy(a2, 0, newArray, a1.Length, a2.Length);
        return newArray;
    }
}
