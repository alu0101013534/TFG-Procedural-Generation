using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Generator : MonoBehaviour {

    public enum DrawMode {NoiseMap,ColourMap,Mesh}      //What is going to generate in editor mode
    public DrawMode drawMode;       

    public NoiseGenerator.NormalizeMode normalizeMode;
    public const int mapChunkSize = 241;        

    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persinstance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;


    [Range(0, 6)]
    public int levelOfDetailEditor;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool randomize;
    public bool autoUpdate;


    public TerrainType[] terrains;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue= new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();




    MapData GenerateMapData(Vector2 center) {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed,noiseScale, octaves, persinstance,lacunarity, center+offset , normalizeMode);

        //Draw the map

        Color[] colourMap = new Color[mapChunkSize*  mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < terrains.Length; i++)
                {
                    if (currentHeight >= terrains[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = terrains[i].colour;
                    }
                    else
                    {
                        break;
                    }


                }
            }
        }


        return new MapData(noiseMap, colourMap);
       
    }

    // Used to Generate the map in editor mode 
    public void DrawMap()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetailEditor), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }

    //map data threading
    public void RequestMapData(Vector2 center, Action<MapData> callback) 
    {
        ThreadStart threadStart= delegate {

            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    
    }

    void MapDataThread(Vector2 center,Action<MapData> callback) {

        MapData mapData = GenerateMapData(center);

        //avoid mapdata to be accesed at the same time from multiple places 

        lock (mapDataThreadInfoQueue) { 
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }//when one thread enters this section no other thread cant enter this section.

    }

    //mesh data threading
    public void RequestMeshData(MapData mapData,int lod, Action<MeshData> callback)
    {

        ThreadStart threadStart = delegate {

            MeshDataThread(mapData,lod, callback);        

        };

        new Thread(threadStart).Start();
    }


    void MeshDataThread(MapData mapData, int lod ,Action<MeshData> callback) {

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);

        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));

        }
    }


    private void OnValidate()
    {
     
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }

    public void Start()
    {

        if (randomize)
            seed = (int)UnityEngine.Random.Range(1, 10000);
        DrawMap();
    }

    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i=0; i< mapDataThreadInfoQueue.Count;i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    //struct to store mapdata for the threading, height map or colormap
    struct MapThreadInfo<T>
    {

        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}

//Structure to organize different tipes of terrains. 
[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;

}

//Struct to store the heightMap and colour map
public struct MapData {      
    public readonly float[,] heightMap;
    public readonly Color [] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }

}