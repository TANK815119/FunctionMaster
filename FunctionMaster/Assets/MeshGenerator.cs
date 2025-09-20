using Assets;
using Assets.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public const int xVertice = 10;
    public const int yVertice = 10;
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        int k = 0;
        // Define vertices (positions in 3D space)
        Function function = FunctionParser.parse("(x - 5) * (y - 5)");

        Vector3[] vertices = new Vector3[xVertice * yVertice * 2];
        for (int l = 0; l < 2; l++)
        {
            for (int i = 0; i < xVertice; i++)
            {
                for (int j = 0; j < yVertice; j++)
                {
                    vertices[k++] = new Vector3(i, (int)function.calculate(i, j), j);
                }
            }
        }

        mesh.vertices = vertices;

        // Define triangle indices (clockwise order)

        int[] triangles = new int[24 * (xVertice - 1) * (yVertice - 1)];
        Debug.Log(vertices[23]);
        Debug.Log(vertices[23 + xVertice * yVertice]);
        k = 0;
        for (int i = 0; i < xVertice - 1; i++)
        {
            for(int j = 0; j < yVertice - 1; j++)
            {
                int topLeft = i + j * xVertice;
                int topRight = (i + 1) + j * xVertice;
                int bottomLeft = i + (j + 1) * xVertice;
                int bottomRight = (i + 1) + (j + 1) * xVertice;
                int toReverse = xVertice * yVertice;

                // Top face (normals point up)
                triangles[k++] = topLeft;
                triangles[k++] = bottomLeft;
                triangles[k++] = topRight;

                triangles[k++] = topRight;
                triangles[k++] = bottomLeft;
                triangles[k++] = bottomRight;

                // Bottom face (normals point down ? reverse winding)
                triangles[k++] = topLeft + toReverse;
                triangles[k++] = topRight + toReverse;
                triangles[k++] = bottomLeft + toReverse;

                triangles[k++] = topRight + toReverse;
                triangles[k++] = bottomRight + toReverse;
                triangles[k++] = bottomLeft + toReverse;
            }
        }

        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh; // assign generated mesh
        mc.convex = false; // keep false for static concave mesh; true if moving
    }
}
