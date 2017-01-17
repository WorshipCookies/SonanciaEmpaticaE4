using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBuilder {

    int size_x;
    int size_z;
    float tileSize;

    public Texture2D terrainTiles;
    public int tileResolution;

    Dictionary<int, Vector3[]> lookUpTable;

    public MeshBuilder(int size_x, int size_z, int tileSize)
    {
        this.size_x = size_x;
        this.size_z = size_z;
        this.tileSize = tileSize;

        MeshMap.BuildMesh(size_x, size_z, tileSize);
    }

    public MeshBuilder(DMap map)
    {
        // Distribute the parameters
        this.size_x = map.getXSize();
        this.size_z = map.getYSize();
        tileSize = map.getTileSize();


    }

    public Mesh create3DRoom(List<DTile> tiles)
    {

        List<Vector3> verticeList = calcTotalVertices(tiles);
        int numVerts = verticeList.Count;

        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uvs = new Vector2[numVerts];


        int numTriangles = tiles.Count * 2; // Each square is composed of 2 triangles.
        int[] triangles = new int[numTriangles * 3];

        int counter = 0;
        foreach (Vector3 vert in verticeList)
        {
            vertices[counter] = vert;
            normals[counter] = Vector3.up;
            uvs[counter] = new Vector2(0f, 1f);
        }

        counter = 0;
        foreach (DTile tile in tiles)
        {
            
        }




        Mesh mesh = new Mesh();
        return mesh;
    }

    public void createLookUpTable(int size_x, int size_z, int tileSize)
    {
        this.lookUpTable = new Dictionary<int, Vector3[]>();

        int numTiles = size_x * size_z;
        int numTris = numTiles * 2;

        int vsize_x = size_x + 1;
        int vsize_z = size_z + 1;
        int numVerts = vsize_x * vsize_z;

        int idCounter = 0;
        for (int z = 0; z < size_z; z++)
        {
            for (int x = 0; x < size_x; x++)
            {
                Vector3[] vertices = new Vector3[4];

                vertices[0] = new Vector3( x * tileSize, 0, z * tileSize );
                vertices[1] = new Vector3( ( x * tileSize ) + tileSize, 0, z * tileSize );
                vertices[2] = new Vector3( x * tileSize, 0, ( z * tileSize ) + tileSize );
                vertices[3] = new Vector3( ( x * tileSize ) + tileSize, 0, ( z * tileSize ) + tileSize );

                lookUpTable.Add(idCounter, vertices);
                idCounter++;
            }
        }

    }

    // Bah not the ideal solution... but it works I guess... Returns the number of vertices of the mesh.
    public List<Vector3> calcTotalVertices(List<DTile> tiles)
    {
        List<Vector3> addedVertices = new List<Vector3>();

        // For each tile that does not have a neighbour sum an extra vertice.
        foreach (DTile t in tiles)
        {
            Vector3[] vertices = lookUpTable[t.getID()]; // Obtain all vertices of this tiles.
            foreach (Vector3 vertice in vertices)
            {
                if (!compareVertices(addedVertices, vertice))
                {
                    addedVertices.Add(vertice);
                }
            }
        }
        return addedVertices;
    }

    // Compares two Vector3 values. If equal returns True, if not returns False.
    public bool compareVertices(List<Vector3> vertices, Vector3 vertice)
    {
        foreach (Vector3 compVert in vertices)
        {
            if (compVert.x == vertice.x && compVert.y == vertice.y && compVert.z == vertice.z)
            {
                return true;
            }
        }
        return false;
    }

}
