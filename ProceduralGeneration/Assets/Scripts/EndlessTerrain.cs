using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewerDistance = 450;

    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInView;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    List<TerrainChunk> lastUpdateChunks = new List<TerrainChunk>();

    void Start()
    {
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInView = Mathf.RoundToInt(maxViewerDistance / chunkSize );

    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {

        for(int i =0; i< lastUpdateChunks.Count; i++) {

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

                    if (terrainChunkDictionary[viewedChunkPos].IsVisible())
                    {
                        lastUpdateChunks.Add(terrainChunkDictionary[viewedChunkPos]);
                    }

                }
                else 
                {
                    terrainChunkDictionary.Add(viewedChunkPos, new TerrainChunk(viewedChunkPos,chunkSize,transform));
                }
            }
        }

    }

    public class TerrainChunk
    {
        GameObject meshObject;

        Vector2 position;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, int size , Transform parent)
        {
            position = coord * size;
            Vector3 position3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position,Vector2.one * size);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * size / 10f;

            SetVisible(false);
        }

        public void UpdateTerrainChunk() 
        {
            //nearest edge
            float viewerDistance = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistance <= maxViewerDistance;
            SetVisible(visible);
        
        }

        public void SetVisible(bool visible) 
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {

            return meshObject.activeSelf;
        }
    }

}
