using System.Collections;
using UnityEngine;

public static class TextureGenerator
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
	public static Texture2D TextureFromHeightMap(float[,] heighMap)
	{
		int width = heighMap.GetLength(0);
		int height = heighMap.GetLength(1);

		Color[] colourmap = new Color[width * height];	
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colourmap[y * width + x] = Color.Lerp(Color.black, Color.white, heighMap[x, y]);
			}
		}
		return TextureFromColourMap(colourmap,width,height);
	}
}