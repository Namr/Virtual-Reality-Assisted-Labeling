using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CellManager : MonoBehaviour
{

    public byte[,,] data;
    byte[] rawdata = File.ReadAllBytes("Ellie_Sarah_Top.nii");
    short[] dim = new short[8];
    float vox_offset;

    public int SizeX = 16;
    public int SizeY = 16;
    public int SizeZ = 16;

    public GameObject chunk;
    public GameObject[,,] chunks;
    public int chunkSize = 16;
    public int chunkHeight = 16;

    // Use this for initialization
    void Start()
    {
        loadData();
        generateChunks();
        this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        Debug.Log(Time.timeSinceLevelLoad);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadData()
    {
        //based off of https://brainder.org/2012/09/23/the-nifti-file-format/
        int count = 0;
        for (int i = 0; i < dim.Length; i++)
        {
            dim[i] = BitConverter.ToInt16(new byte[2] { rawdata[40 + count + 1], rawdata[40 + count] }, 0); // 1 2 and 3 matter (x,y, and z)
            count += 2;
        }

        vox_offset = BitConverter.ToSingle(new byte[4] { rawdata[108 + 3], rawdata[108 + 2], rawdata[108 + 1], rawdata[108] }, 0);

        /*
        data = new byte[dim[1], dim[2], dim[3]];
        int counter = (int)vox_offset;
        for (int z = 0; z < dim[3]; z++)
        {
            for (int y = 0; y < dim[2]; y++)
            {
                for (int x = 0; x < dim[1]; x++)
                {
                    data[x, y, z] = rawdata[counter];
                    counter++;
                }
            }
        }
        */
    }

    public void generateChunks()
    {
        chunks = new GameObject[Mathf.CeilToInt(SizeX / chunkSize) + 1, Mathf.CeilToInt(SizeY / chunkSize) + 1, Mathf.CeilToInt(SizeZ / chunkHeight) + 1];

        for (int z = 0; z < SizeZ; z++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    int chunkx = Mathf.CeilToInt(x / chunkSize);
                    int chunky = Mathf.CeilToInt(y / chunkSize);
                    int chunkz = Mathf.CeilToInt(z / chunkHeight);
                    if (chunks[chunkx, chunky, chunkz] == null)
                    {
                        chunks[chunkx, chunky, chunkz] = Instantiate(chunk, new Vector3(chunkx * chunkSize, chunky * chunkSize, chunkz * chunkSize), new Quaternion(0, 0, 0, 0)) as GameObject;
                        chunks[chunkx, chunky, chunkz].transform.parent = this.transform;

                        Chunk newChunkScript = chunks[chunkx, chunky, chunkz].GetComponent<Chunk>();

                        newChunkScript.cellManagerObject = gameObject;
                        newChunkScript.chunkSizeX = chunkSize;
                        newChunkScript.chunkSizeY = chunkSize;
                        newChunkScript.chunkSizeZ = SizeZ;

                        newChunkScript.chunkX = chunkx * chunkSize;
                        newChunkScript.chunkY = chunky * chunkSize;
                        newChunkScript.chunkZ = chunkz * chunkSize;
                    }
                    Chunk current = chunks[chunkx, chunky, chunkz].GetComponent<Chunk>();
                    generateMesh(x, y, z, current);
                }
            }
        }
        /*
        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {

                    chunks[x, y, z] = Instantiate(chunk, new Vector3(x * chunkSize, y * chunkSize, z * chunkSize), new Quaternion(0, 0, 0, 0)) as GameObject;
                    chunks[x, y, z].transform.parent = this.transform;

                    Chunk newChunkScript = chunks[x, y, z].GetComponent<Chunk>();

                    newChunkScript.cellManagerObject = gameObject;
                    newChunkScript.chunkSizeX = chunkSize;
                    newChunkScript.chunkSizeY = chunkSize;
                    newChunkScript.chunkSizeZ = SizeZ;

                    newChunkScript.chunkX = x * chunkSize;
                    newChunkScript.chunkY = y * chunkSize;
                    newChunkScript.chunkZ = z * chunkSize;

                }
            }
        }
        */
    }

    void generateMesh(int x, int y, int z, Chunk chunk)
    {
        byte threshold = 25;
        byte purewhite = 70;
        //This code will run for every block in the chunk
        if (Block(x, y, z) > purewhite)
        {
            chunk.CubeTop(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
            chunk.CubeBot(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
            chunk.CubeNorth(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
            chunk.CubeEast(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
            chunk.CubeSouth(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
            chunk.CubeWest(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
        }
        else if (Block(x, y, z) > threshold)
        {
            //If the block is solid

            if (Block(x, y + 1, z) < threshold)
            {
                //Block above is air
                chunk.CubeTop(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));
            }

            if (Block(x, y - 1, z) < threshold)
            {
                //Block below is air
                chunk.CubeBot(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));

            }

            if (Block(x + 1, y, z) < threshold)
            {
                //Block east is air
                chunk.CubeEast(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));

            }

            if (Block(x - 1, y, z) < threshold)
            {
                //Block west is air
                chunk.CubeWest(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));

            }

            if (Block(x, y, z + 1) < threshold)
            {
                //Block north is air
                chunk.CubeNorth(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));

            }

            if (Block(x, y, z - 1) < threshold)
            {
                //Block south is air
                chunk.CubeSouth(x - chunk.chunkX, y - chunk.chunkY, z - chunk.chunkZ, Block(x, y, z));

            }

        }
    }
    public byte Block(int x, int y, int z)
    {

        if (x >= SizeX || x < 0 || y >= SizeY || y < 0 || z >= SizeZ || z < 0)
        {
            return 0;
        }
        int pos = (int)vox_offset + (z * dim[2] + y) * dim[1] + x;
        return rawdata[pos];
    }

}

class BinaryReader2 : BinaryReader
{
    public BinaryReader2(System.IO.Stream stream) : base(stream) { }

    public override int ReadInt32()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToInt32(data, 0);
    }

    public override float ReadSingle()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToSingle(data, 0);
    }

    public override Int16 ReadInt16()
    {
        var data = base.ReadBytes(2);
        Array.Reverse(data);
        return BitConverter.ToInt16(data, 0);
    }

    public override Int64 ReadInt64()
    {
        var data = base.ReadBytes(8);
        Array.Reverse(data);
        return BitConverter.ToInt64(data, 0);
    }

    public override UInt32 ReadUInt32()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToUInt32(data, 0);
    }

}