using System.Collections;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    // public
    public Renderer textureRenderer;
    public MeshFilter meshFilther;
    public MeshRenderer meshRenderer;
    //private
    public void DrawTexture(Texture2D texture)
	{
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}
    public void DrawMesh(MeshData meshData,Texture2D texture)
	{
        meshFilther.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

    }
}