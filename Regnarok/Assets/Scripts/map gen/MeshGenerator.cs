using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail,bool flatshading,MapGenerator mg)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;
		int borderedSize = heightMap.GetLength(0);
		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
		int borderVertexIndex = -1;

		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y += meshSimplificationIncrement)
		{
			for (int x = 0; x < width; x += meshSimplificationIncrement)
			{
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
				meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1)
				{
					meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}
		mg.ColorMap();
		meshData.ProcessMesh(flatshading);
		return meshData;

	}
}
public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	Vector3[] bakedNormals;

	Vector3[] borderVertices;
	int[] outOfMeshTriangles;
	int triangleIndex;
	int borderTriangleIndex;
	int outOfMeshTriangleIndex;
	int[] borderTriangles;

	public MeshData(int meshWidth, int meshHeight)
	{
		vertices = new Vector3[meshWidth * meshHeight];
		uvs = new Vector2[meshWidth * meshHeight];
		triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
		borderTriangles = new int[24 * meshWidth];
	}
	public void AddTriangle(int a, int b, int c) {
		if (a < 0 || b < 0 || c < 0) {
			outOfMeshTriangles [outOfMeshTriangleIndex] = a;
			outOfMeshTriangles [outOfMeshTriangleIndex + 1] = b;
			outOfMeshTriangles [outOfMeshTriangleIndex + 2] = c;
			outOfMeshTriangleIndex += 3;
		} else {
			triangles [triangleIndex] = a;
			triangles [triangleIndex + 1] = b;
			triangles [triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}
	Vector3[] CalculateNormals()
	{

		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}
		int borderTriangleCount = borderTriangles.Length/3;
		for (int i = 0; i < borderTriangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderTriangles[normalTriangleIndex];
			int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
			int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			if (vertexIndexA >= 0)
			{
				vertexNormals[vertexIndexA] += triangleNormal;
			}
			if (vertexIndexB >= 0)
			{
				vertexNormals[vertexIndexB] += triangleNormal;
			}
			if (vertexIndexC >= 0)
			{
				vertexNormals[vertexIndexC] += triangleNormal;
			}
		}


		for (int i = 0; i < vertexNormals.Length; i++)
		{
			vertexNormals[i].Normalize();
		}

		return vertexNormals;

	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
	{
		Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
		Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
		Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	void FlatShading()
	{
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];
		for (int i = 0; i < triangles.Length; i++)
		{
			flatShadedUvs[i] = uvs[triangles[i]];
			flatShadedVertices[i] = vertices[triangles[i]];
			triangles[i] = i;
			new WaitForSeconds(0.001f);
		}
		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}
	public void ProcessMesh(bool flatshading)
	{
		if (flatshading)
		{
			FlatShading();
		}
		else
		{
			BakeNormals();
		}
	}
	void BakeNormals()
	{
		bakedNormals = CalculateNormals();
	}
	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}

}