using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    public int chunkSizeX = 16;
    public int chunkSizeY = 16;
    public int chunkSizeZ = 16;

    public int chunkX;
    public int chunkY;
    public int chunkZ;

    private float tUnit = 0.25f;
    private Vector2 tWhite = new Vector2(0, 3);
    private Vector2 tLightTrans = new Vector2(1, 3);
    private Vector2 tMidTrans = new Vector2(2, 3);
    private Vector2 tHeavyTrans = new Vector2(3, 3);

    private Mesh mesh;
    private MeshCollider col;

    private int faceCount;

    public GameObject cellManagerObject;
    private CellManager cellManager;

    // Use this for initialization
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        cellManager = cellManagerObject.GetComponent<CellManager>();

        //GenerateMesh();
        UpdateMesh();
    }

    void GenerateMesh()
    {
        byte threshold = 25;
        byte purewhite = 70;
        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                for (int z = 0; z < chunkSizeZ; z++)
                {
                    //This code will run for every block in the chunk
                    if(Block(x, y, z) > purewhite)
                    {
                        CubeTop(x, y, z, Block(x, y, z));
                        CubeBot(x, y, z, Block(x, y, z));
                        CubeNorth(x, y, z, Block(x, y, z));
                        CubeEast(x, y, z, Block(x, y, z));
                        CubeSouth(x, y, z, Block(x, y, z));
                        CubeWest(x, y, z, Block(x, y, z));
                    }
                    else if (Block(x, y, z) > threshold)
                    {
                        //If the block is solid

                        if (Block(x, y + 1, z) < threshold)
                        {
                            //Block above is air
                            CubeTop(x, y, z, Block(x, y, z));
                        }

                        if (Block(x, y - 1, z) < threshold)
                        {
                            //Block below is air
                            CubeBot(x, y, z, Block(x, y, z));

                        }

                        if (Block(x + 1, y, z) < threshold)
                        {
                            //Block east is air
                            CubeEast(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) < threshold)
                        {
                            //Block west is air
                            CubeWest(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) < threshold)
                        {
                            //Block north is air
                            CubeNorth(x, y, z, Block(x, y, z));
                            
                        }

                        if (Block(x, y, z - 1) < threshold)
                        {
                            //Block south is air
                            CubeSouth(x, y, z, Block(x, y, z));

                        }

                    }
                }
            }
        }
        UpdateMesh();
    }

    public void UpdateMesh()
    {

        if(newVertices.Count == 0)
        {
            Destroy(this);
        }
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        //MeshUtility.Optimize(mesh);
        //mesh.RecalculateNormals();

        col.sharedMesh = null;
        col.sharedMesh = mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }

    public void Cube(Vector2 texturePos)
    {

        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++; // Add this line
    }

    public void CubeTop(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        Vector2 texturePos = getTexture(block);

        Cube(texturePos);
    }

    public void CubeNorth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos = getTexture(block);

        Cube(texturePos);
    }

    public void CubeEast(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 texturePos = getTexture(block);

        Cube(texturePos);
    }

    public void CubeSouth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        Vector2 texturePos = getTexture(block);

        Cube(texturePos);
    }

    public void CubeWest(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        Vector2 texturePos = getTexture(block);

        Cube(texturePos);
    }

    public void CubeBot(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos = getTexture(block);

        Cube(texturePos);
    }

    public byte Block(int x, int y, int z)
    {
        return cellManager.Block(x + chunkX, y + chunkY, z + chunkZ);
    }

    public Vector2 getTexture(byte block)
    {
        if (block > 70)
            return tWhite;
        else if (block > 50)
            return tLightTrans;
        else if (block > 30)
            return tMidTrans;
        else
            return tHeavyTrans;
    }
    
}
