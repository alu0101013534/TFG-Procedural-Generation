﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{

    const float scale = 1f;
    const float viewerMoveLimitToUpdateChunk=25f;
    const float sqrViewerMoveLimitToUpdateChunk = viewerMoveLimitToUpdateChunk * viewerMoveLimitToUpdateChunk; //avoid using square operation

    public LevelOfDetailInfo[] detailLevels;
    public static float maxViewerDistance;
    public Transform viewer;
    public Material mapMaterial;

    static MapGenerator mapGenerator;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionLast;
    int chunkSize;
    int chunksVisibleInView;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    static List<TerrainChunk> lastUpdateChunks = new List<TerrainChunk>();

    public GameObject treePrefabGO;
    [SerializeField]
    public static GameObject treePrefab;

    private static float meshHeightMultiplier;

    void Start()
    {

        maxViewerDistance = detailLevels[detailLevels.Length - 1].VisibleDistance;

        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInView = Mathf.RoundToInt(maxViewerDistance / chunkSize);


        Debug.LogWarning(maxViewerDistance);
        treePrefab = treePrefabGO;
        UpdateVisibleChunks();

        meshHeightMultiplier = mapGenerator.meshHeightMultiplier;
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z)/scale;


        //not updating the chunks every loop


        if((viewerPositionLast-viewerPosition).sqrMagnitude > sqrViewerMoveLimitToUpdateChunk) {
            viewerPositionLast = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < lastUpdateChunks.Count; i++) {

            lastUpdateChunks[i].SetVisible(false);
        }
        lastUpdateChunks.Clear();

        int currentChunkX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInView; yOffset <= chunksVisibleInView; yOffset++)
        {
            for (int xOffset = -chunksVisibleInView; xOffset <= chunksVisibleInView; xOffset++)
            {
                Vector2 viewedChunkPos = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkPos))
                {
                    //
                    terrainChunkDictionary[viewedChunkPos].UpdateTerrainChunk();

               
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkPos, new TerrainChunk(viewedChunkPos, chunkSize, detailLevels,transform, mapMaterial));
                }
            }
        }

    }

    public class TerrainChunk
    {
        GameObject meshObject;

        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LevelOfDetailInfo[] detailLevels;

        LevelOfDetailMesh[] levelOfDetailMeshes;

        MapData mapData;

        bool mapDataReceived;

        int previousLODIndex = -1;

        bool hasTrees;

        List<GameObject> myAssets = new List<GameObject>();


        public TerrainChunk(Vector2 coord, int size, LevelOfDetailInfo[] detailLevels, Transform parent, Material material)
        {

            this.detailLevels = detailLevels;
            position = coord * size;
            Vector3 position3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);

            meshObject = new GameObject("Terrain Chunk");

            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;


            //meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3 * scale;
            meshObject.transform.localScale = Vector3.one * scale;
            meshObject.transform.parent = parent;
            //meshObject.transform.localScale = Vector3.one * 2.3f;

            SetVisible(false);


            levelOfDetailMeshes = new LevelOfDetailMesh[detailLevels.Length];

            for (int i = 0; i < detailLevels.Length; i++)
            {
                levelOfDetailMeshes[i] = new LevelOfDetailMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataRecieved);
        }
        void OnMapDataRecieved(MapData mapData)
        {
            Debug.Log("MAP RECEIVED");
            // print("MAP RECIEVED");
            //  mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);

            this.mapData = mapData;
            mapDataReceived = true;


            //Apply texture to the mesh

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;


            UpdateTerrainChunk();
        }

        /*  void OnMeshDataReceived(MeshData meshData) {

              meshFilter.mesh = meshData.CreateMesh();
          }*/
        public void UpdateTerrainChunk()
        {

            if (mapDataReceived)
            {

                //nearest edge
                float viewerDistance = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistance <= maxViewerDistance;

                bool treeVisible = viewerDistance <= 300;


                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistance > detailLevels[i].VisibleDistance)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }

                    }

                    if (lodIndex != previousLODIndex)
                    {

                        LevelOfDetailMesh lodMesh = levelOfDetailMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);

                        }

                        if (lodMesh.hasMesh && treeVisible && !hasTrees)
                        {
                            GenerateTrees();
                        }
                    }

                    lastUpdateChunks.Add(this);
                }

                if (!visible)
                {
                    RemoveTrees();

                }
                SetVisible(visible);

                

                
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {

            return meshObject.activeSelf;
        }

        public void GenerateTrees()
        {
            
            Mesh mesh = this.meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            float height;
            float rand;
            for (int i = 0; i < vertices.Length; i++)
            {


                height = levelOfDetailMeshes[0].noiseHeight[i];
                rand = Mathf.PerlinNoise(vertices[i].x + MapGenerator.mapChunkSize / 10, vertices[i].y + MapGenerator.mapChunkSize  / 10);
                rand = Random.Range(0, 100);
                if (height < 0.57f && height > 0.50f && rand < 1f)
                {
                    GameObject newAsset = TreePool.getTree();

                    if (newAsset != null)
                    {
                        Vector3 treePos = new Vector3(vertices[i].x + meshObject.transform.position.x, vertices[i].y, vertices[i].z + meshObject.transform.position.z);
                        //GameObject t = Instantiate(treePrefab, treePos, Quaternion.identity);

                        //t.transform.parent = meshObject.transform;


                        newAsset.transform.position = treePos;
                        newAsset.transform.parent = meshObject.transform;
                        newAsset.SetActive(true);
                        myAssets.Add(newAsset);

                    }



                }

                if (height > 0.57f && height < 0.64f && rand < 1f)
                {
                    GameObject newAsset = TreePool.getPine();

                    if (newAsset != null)
                    {
                        Vector3 treePos = new Vector3(vertices[i].x + meshObject.transform.position.x, vertices[i].y, vertices[i].z + meshObject.transform.position.z);
                        //GameObject t = Instantiate(treePrefab, treePos, Quaternion.identity);

                        //t.transform.parent = meshObject.transform;


                        newAsset.transform.position = treePos;
                        newAsset.transform.parent = meshObject.transform;
                        newAsset.SetActive(true);
                        myAssets.Add(newAsset);

                    }



                }



            }
            hasTrees = true;

        }

        public void RemoveTrees()
        {

            for (int i = 0; i < myAssets.Count; i++)
            {
                if (myAssets[i] != null)
                    myAssets[i].SetActive(false);

            }
            myAssets.Clear();
        }
    }

    //fetch mesh from mesh generator
    class LevelOfDetailMesh {

        public Mesh mesh;

        public bool hasRequestedMesh;

        public bool hasMesh;

        public float[] noiseHeight;
        System.Action updateCallback;

        int lod; //level of detail

        public LevelOfDetailMesh(int lod, System.Action updateCallback) {

            this.lod = lod;

            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            noiseHeight = meshData.noiseHeight;
            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {

            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);

        }
    }
    [System.Serializable]
    public struct LevelOfDetailInfo {

        public int lod;
        public float VisibleDistance;// when the viewer is outside of that range change the level of detail
    }

    // class to represent a coordinate in the Tile Coordinate System
    public class TileCoordinate
    {
        public int tileXIndex;
        public int tileZIndex;
        public int coordinateXIndex;
        public int coordinateZIndex;

        public TileCoordinate(int tileXIndex, int tileZIndex, int coordinateXIndex, int coordinateZIndex)
        {
            this.tileXIndex = tileXIndex;
            this.tileZIndex = tileZIndex;
            this.coordinateXIndex = coordinateXIndex;
            this.coordinateZIndex = coordinateZIndex;
        }

       

        public static TileCoordinate ConvertToTileCoordinate(int xIndex, int zIndex, int tileDepthInVertices,int tileWidthInVertices)
        {
            // the tile index is calculated by dividing the index by the number of tiles in that axis
            int tileXIndex = (int)Mathf.Floor((float)xIndex / (float)tileWidthInVertices);
            int tileZIndex = (int)Mathf.Floor((float)zIndex / (float)tileDepthInVertices);
            // the coordinate index is calculated by getting the remainder of the division above
            // we also need to translate the origin to the bottom left corner
            int coordinateXIndex = tileWidthInVertices - (xIndex % tileDepthInVertices) - 1;
            int coordinateZIndex = tileDepthInVertices - (zIndex % tileDepthInVertices) - 1;

            TileCoordinate tileCoordinate = new TileCoordinate(tileXIndex, tileZIndex, coordinateXIndex, coordinateZIndex);
            return tileCoordinate;
        }
    }


}
