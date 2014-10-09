using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DynamicMesh : MonoBehaviour {
    public DynamicVertex[] DynamicVertices;

    public GameObject Centroid;

	void Awake() {
        if(Centroid == null)
            CreateCentroid();
	}
	
	void Update() {
        ClearMesh();

        if(DynamicVertices != null && DynamicVertices.Length > 1) {
            UpdateMesh();
        }

        UpdateCentroid();
	}

    private void CreateCentroid() {
        Centroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Centroid.name = "Centroid";
        Centroid.transform.parent = gameObject.transform;
        Centroid.transform.localPosition = Vector3.zero;
        Centroid.transform.localScale = new Vector3(50.0f, 50.0f, 50.0f);
    }

    private void ClearMesh() {
        gameObject.GetComponent<MeshFilter>().mesh.Clear();
    }

    private void UpdateMesh() {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        mesh.vertices = GetVertices();
        mesh.uv = GetNormals();
        mesh.triangles = GetTriangles();
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    private void UpdateCentroid() {
        if(DynamicVertices == null || DynamicVertices.Length < 2) {
            Centroid.transform.localPosition = Vector3.zero;
            Centroid.GetComponent<MeshRenderer>().enabled = false;
        } else {
            Vector2 centroid = CalculateCentroid();
            Centroid.transform.localPosition = new Vector3(centroid.x, centroid.y, -50);
            Centroid.GetComponent<MeshRenderer>().enabled = true;
        }
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

    // TODO: this could also just be regenerated when points change
    private int[] GetTriangles() {
        int triCount = DynamicVertices.Length - 2;

        int[] triangles = new int[3 * triCount];

        for(int i = 0; i < triCount; i++) {
            int idx = i * 3;

            triangles[idx] = 0;
            triangles[idx + 1] = i + 1;
            triangles[idx + 2] = i + 2;

            //Debug.Log("tri with " + triangles[i] + " " + triangles[i + 1] + " " + triangles[i + 2]);
        }

        return triangles;
    }

    private Vector2 CalculateCentroid() {
        float signedArea = 0;
        float cx = 0;
        float cy = 0;

        for(int i = 0; i < DynamicVertices.Length; i++) {
            Vector3 current = DynamicVertices[i].transform.localPosition;
            Vector3 next = DynamicVertices[(i + 1) % DynamicVertices.Length].transform.localPosition;

            signedArea += current.x * next.y - next.x * current.y;
            cx += (current.x + next.x) * (current.x * next.y - next.x * current.y);
            cy += (current.y + next.y) * (current.x * next.y - next.x * current.y);
        }

        signedArea /= 2;
        cx /= (6 * signedArea);
        cy /= (6 * signedArea);

        return new Vector2(cx, cy);
    }
}
