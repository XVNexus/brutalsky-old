using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonMaker : MonoBehaviour
{
    void Start()
    {
        var mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = new Vector3[]
        {
            new Vector3(0f, 0f),
            new Vector3(1f, 0f),
            new Vector3(1f, 1f),
            new Vector3(0f, 1f)
        };
        mesh.uv = new Vector2[] { };
        mesh.triangles = new int[]
        {
            0b0111
        };
    }
}
