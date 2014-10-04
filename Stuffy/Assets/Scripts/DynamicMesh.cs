using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DynamicMesh : MonoBehaviour {
	public DynamicVertex[] DynamicVertices;

	void Awake() {

	}
	
	void Update() {
        gameObject.GetComponent<MeshFilter>().mesh.Clear();

        if(DynamicVertices != null && DynamicVertices.Length > 1) {
            Debug.Log("redrawin");

            UpdateMesh();
        }
	}

    private void UpdateMesh() {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        mesh.vertices = GetVertices();
        mesh.uv = GetNormals();
        mesh.triangles = GetTriangles();
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        
        //gameObject.GetComponent<MeshFilter>().mesh = _mesh;
    }

    private Vector3[] GetVertices() {
        Vector3[] vertices = new Vector3[DynamicVertices.Length];
        
        for(int i = 0; i < vertices.Length; i++)
            vertices[i] = DynamicVertices[i].transform.localPosition;

        return vertices;
    }

    // TODO: instead of generating this, maintain an internal list that stays 
    // the same length as the list of vertices.
    private Vector2[] GetNormals() {
        Vector2[] normals = new Vector2[DynamicVertices.Length];
        
        for(int i = 0; i < normals.Length; i++)
            normals[i] = DynamicVertices[i].Normal;

        return normals;
    }

    private int[] GetTriangles() {
        int[] triangles = new int[3 * (DynamicVertices.Length - 2)];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        return triangles;
    }
}
