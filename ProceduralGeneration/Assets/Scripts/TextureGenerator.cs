using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator 
{
    public static Texture2D TextureFromColourMap(Color[] colourMap,int width,int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;

    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

      
        //instead of creating a  texture and changing it pixel by pixel we can use a Color Array
        Color[] colourMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                //Since the heightMap has 2 dimensions and colourmap 1, in order to iterate it will use  y * width + x as iterator and interpolate the float value from heightmap between black and white
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }

        }

        return TextureFromColourMap(colourMap, width, height);
    }

}
