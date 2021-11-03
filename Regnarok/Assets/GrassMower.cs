using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassMower : MonoBehaviour
{
	public GameObject grassMesh;
	public float minGrassHeight = 1.5f;
	public float maxGrassHeight = 8f;
	// Start is called before the first frame update
	void Start()
    {
		MowIt();
	}
    public void MowIt()
	{
		Mesh meshGrass = grassMesh.GetComponent<MeshFilter>().mesh;
		int[] triangles = meshGrass.triangles;
		Vector3[] vertices = meshGrass.vertices;
		Vector2[] uv = meshGrass.uv;
		Vector3[] normals = meshGrass.normals;
		List<Vector3> vertList = new List<Vector3>();
		List<Vector2> uvList = new List<Vector2>();
		List<Vector3> normalsList = new List<Vector3>();
		List<int> trianglesList = new List<int>();


		int i = 0;
		while (i < grassMesh.GetComponent<MeshFilter>().mesh.vertices.Length)
		{
			vertList.Add(vertices[i]);
			uvList.Add(uv[i]);
			normalsList.Add(normals[i]);
			i++;
			new WaitForSeconds(0.01f);
		}
		for (int triCount = 0; triCount < triangles.Length; triCount += 3)
		{
			if ((transform.TransformPoint(vertices[triangles[triCount]]).y > minGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 1]]).y > minGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 2]]).y > minGrassHeight))
			{
				if ((transform.TransformPoint(vertices[triangles[triCount]]).y < maxGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 1]]).y < maxGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 2]]).y < maxGrassHeight))
				{
					trianglesList.Add(triangles[triCount]);
					trianglesList.Add(triangles[triCount + 1]);
					trianglesList.Add(triangles[triCount + 2]);

					new WaitForSeconds(0.45f);
				}
			}
		}


		triangles = trianglesList.ToArray();
		vertices = vertList.ToArray();
		uv = uvList.ToArray();
		normals = normalsList.ToArray();
		grassMesh.GetComponent<MeshFilter>().mesh.triangles = triangles;
		grassMesh.GetComponent<MeshFilter>().mesh.vertices = vertices;
		grassMesh.GetComponent<MeshFilter>().mesh.uv = uv;
		grassMesh.GetComponent<MeshFilter>().mesh.normals = normals;
		grassMesh.SetActive(true);
	}
}
