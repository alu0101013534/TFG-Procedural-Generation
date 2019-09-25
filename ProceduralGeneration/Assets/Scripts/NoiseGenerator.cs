using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    public enum NormalizeMode { Local,Global}

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int NumberOctaves, float persistance, float lacunarity, Vector2 offset,NormalizeMode normalizeMode)
    {
        //force same random results with a seed. 
        System.Random prng = new System.Random(seed);
        Vector2[] octaves = new Vector2[NumberOctaves];
        float maxHeight =0f;
        float amplitude = 1;
        float frequency;

        for (int i = 0; i < NumberOctaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaves[i] = new Vector2(offsetX, offsetY);

            maxHeight += amplitude;
            amplitude *= persistance;

        }
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;
        float widhtHalf = mapWidth / 2f;
        float heightHalf = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                for (int i=0;i< NumberOctaves; i++) {
                    float sampleX = (x- widhtHalf + octaves[i].x) / scale * frequency;
                    float sampleY = (y- heightHalf + octaves[i].y) / scale * frequency ;

                    // perlin noise returns a value between 0 to 1, multiplying by 2 and substracting 1 we can get a range from -1 to 1  
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2 -1;
                    noiseHeight += perlinValue * amplitude;
                    //update amplitude and frecuency values
                    amplitude *= persistance;
                    frequency *= lacunarity;

                }
                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                } else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        //Normalize Values from noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / ( maxHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight,0,int.MaxValue) ;
                }
            }
        }
        return noiseMap;
    }
}