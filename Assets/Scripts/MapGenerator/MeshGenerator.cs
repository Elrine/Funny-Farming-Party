using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    public static MeshData generateTerrainMesh (float[, ] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetails) {
        AnimationCurve heightCurve = new AnimationCurve (_heightCurve.keys);
        
        int simpleIncrement = (levelOfDetails == 0) ? 1 : (levelOfDetails * 2);

        int borderSize = heightMap.GetLength (0);
        int meshSize = borderSize - 2 * simpleIncrement;
        int meshSizeUnsimplified = borderSize - 2;
        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        int verticesPerLines = (meshSize - 1) / simpleIncrement + 1;

        MeshData mesh = new MeshData (verticesPerLines);
        int[, ] vertexIndicesMap = new int[borderSize, borderSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderSize; y += simpleIncrement) {
            for (int x = 0; x < borderSize; x += simpleIncrement) {
                bool isBorder = x == 0 || x == borderSize - 1 || y == 0 || y == borderSize - 1;

                if (isBorder) {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                } else {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < borderSize; y += simpleIncrement) {
            for (int x = 0; x < borderSize; x += simpleIncrement) {
                int vertexIndex = vertexIndicesMap[x, y];
                Vector2 percent = new Vector2 ((x - simpleIncrement) / (float) meshSize, (y - simpleIncrement) / (float) meshSize);
                float height = heightCurve.Evaluate (heightMap[x, y]) * heightMultiplier;
                Vector3 vertexPos = new Vector3 (topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - meshSizeUnsimplified * percent.y);

                mesh.AddVertex (vertexPos, percent, vertexIndex);

                if (x < borderSize - 1 && y < borderSize - 1) {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + simpleIncrement, y];
                    int c = vertexIndicesMap[x, y + simpleIncrement];
                    int d = vertexIndicesMap[x + simpleIncrement, y + simpleIncrement];
                    mesh.AddTriangle (a, d, c);
                    mesh.AddTriangle (d, a, b);
                }
            }
        }
        mesh.bakeNormals();
        return mesh;
    }
}

public class MeshData {
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    Vector3[] borderVertices;
    int[] borderTriangles;

    Vector3[] bakedNormals;

    int trianglesIndex;
    int borderTrianglesIndex;

    public MeshData (int VerticesPerLine) {
        vertices = new Vector3[VerticesPerLine * VerticesPerLine];
        triangles = new int[(VerticesPerLine - 1) * (VerticesPerLine - 1) * 6];
        borderVertices = new Vector3[VerticesPerLine * 4 + 4];
        borderTriangles = new int[VerticesPerLine * 24];
        uvs = new Vector2[VerticesPerLine * VerticesPerLine];
    }

    public void AddVertex (Vector3 vertexPos, Vector2 vertexUV, int vertexIndex) {
        if (vertexIndex < 0) {
            borderVertices[-vertexIndex - 1] = vertexPos;
        } else {
            vertices[vertexIndex] = vertexPos;
            uvs[vertexIndex] = vertexUV;
        }
    }

    public void AddTriangle (int a, int b, int c) {
        if (a < 0 || b < 0 || c < 0) {
            borderTriangles[borderTrianglesIndex] = a;
            borderTriangles[borderTrianglesIndex + 1] = b;
            borderTriangles[borderTrianglesIndex + 2] = c;
            borderTrianglesIndex += 3;
        } else {
            triangles[trianglesIndex] = a;
            triangles[trianglesIndex + 1] = b;
            triangles[trianglesIndex + 2] = c;
            trianglesIndex += 3;
        }
    }

    Vector3[] CalculateNormals () {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0) {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if (vertexIndexB >= 0) {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if (vertexIndexC >= 0) {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }

        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals[i].Normalize ();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices (int indexA, int indexB, int indexC) {
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        Vector3 vectorAB = pointA - pointB;
        Vector3 vectorAC = pointA - pointC;

        return Vector3.Cross (vectorAB, vectorAC).normalized;
    }

    public void bakeNormals() {
        bakedNormals = CalculateNormals();
    }

    public Mesh createMesh () {
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = bakedNormals;
        return mesh;
    }
}