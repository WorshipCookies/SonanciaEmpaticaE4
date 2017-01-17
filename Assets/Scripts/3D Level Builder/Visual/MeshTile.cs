using UnityEngine;
using System.Collections;

public class MeshTile {

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;

    int[] triangles;


    public MeshTile(Vector3[] vertices)
    {
        this.vertices = vertices;
    }

    public Vector3[] getVertices()
    {
        return vertices;
    }

}
