using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class DynamicMesh : MonoBehaviour {
    public delegate void VertexLinkCreatedHandler(DynamicVertex v1, DynamicVertex v2);
    public event VertexLinkCreatedHandler OnVertexLinkCreated = delegate { };
    
    public delegate void VertexLinkDestroyedHandler(DynamicVertex v1, DynamicVertex v2);
    public event VertexLinkDestroyedHandler OnVertexLinkDestroyed = delegate { };

    private List<DynamicVertex> _vertices;

    public GameObject Centroid;

    // To avoid leaking meshes, set mesh access up as follows:
    // http://answers.unity3d.com/questions/14990/modifying-mesh-vertices-in-an-editor-script.html

    private MeshFilter _meshFilter = null;
    protected MeshFilter MeshFilter {
        get {
            if(_meshFilter == null)
                _meshFilter = gameObject.GetComponent<MeshFilter>();
            return _meshFilter;
        }
    }

    private Mesh _mesh = null;
    protected Mesh Mesh {
        get {
            if(_mesh != null) {
                return _mesh;
            } else {
                if(MeshFilter.sharedMesh == null) {
                    Mesh newMesh = new Mesh();
                    _mesh = MeshFilter.sharedMesh = newMesh;
                } else {
                    _mesh = MeshFilter.sharedMesh;
                }
                return _mesh;
            }
        }
    }


	void Awake() {
        /*
        if(Centroid == null)
            CreateCentroid();
        */

        // Collect existing vertices and add link listeners.
        _vertices = new List<DynamicVertex>(gameObject.GetComponentsInChildren<DynamicVertex>());

        foreach(DynamicVertex vertex in _vertices) {
            vertex.OnLinkCreated += HandleVertexLinkCreated;
            vertex.OnLinkDestroyed += HandleVertexLinkDestroyed;
        }
	}

    private void HandleVertexLinkCreated(DynamicVertex v1, DynamicVertex v2) {
        OnVertexLinkCreated(v1, v2);
    }

    private void HandleVertexLinkDestroyed(DynamicVertex v1, DynamicVertex v2) {
        OnVertexLinkDestroyed(v1, v2);
    }
	
	void Update() {
        ClearMesh();

        if(_vertices != null && _vertices.Count > 1)
            UpdateMesh();

        UpdateCentroid();
	}

    public void ClearVertices() {
        foreach(DynamicVertex vertex in _vertices)
            GameObject.DestroyImmediate(vertex.gameObject);
        
        _vertices.Clear();
    }
    
    public void AddVertex() {
        DynamicVertex newVertex = CreateNewVertex();

        newVertex.OnLinkCreated += HandleVertexLinkCreated;
        newVertex.OnLinkDestroyed += HandleVertexLinkDestroyed;

        _vertices.Add(newVertex);
    }
    
    public void RemoveVertex() {
        DynamicVertex lastVertex = _vertices[_vertices.Count - 1];
        _vertices.Remove(lastVertex);

        lastVertex.OnLinkCreated -= HandleVertexLinkCreated;
        lastVertex.OnLinkDestroyed -= HandleVertexLinkDestroyed;

        GameObject.DestroyImmediate(lastVertex.gameObject);
    }
    
    private DynamicVertex CreateNewVertex() {
        GameObject prototype = (GameObject)Resources.Load("Prefabs/DynamicVertex");
        
        GameObject go = (GameObject)GameObject.Instantiate(prototype);
        go.transform.parent = gameObject.transform;

        go.name = "DynamicVertex" + (_vertices.Count + 1);
        
        if(_vertices.Count > 1) {
            Vector3 firstPos = _vertices[0].transform.localPosition;
            Vector3 lastPos = _vertices[_vertices.Count - 1].transform.localPosition;
            
            go.transform.localPosition = MathUtils.MidPoint(firstPos, lastPos);
        } else {
            go.transform.localPosition = Vector3.zero;
        }
        
        return go.GetComponent<DynamicVertex>();
    }

    private void ClearMesh() {
        Mesh.Clear();
    }

    private void UpdateMesh() {
        Mesh.vertices = GetVertices();
        Mesh.uv = GetNormals();
        Mesh.triangles = GetTriangles();
        
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        Mesh.Optimize();
    }

    private Vector3[] GetVertices() {
        Vector3[] vertices = new Vector3[_vertices.Count];
        
        for(int i = 0; i < vertices.Length; i++)
            vertices[i] = _vertices[i].transform.localPosition;

        return vertices;
    }

    // TODO: instead of generating this, maintain an internal list that stays 
    // the same length as the list of vertices.
    private Vector2[] GetNormals() {
        Vector2[] normals = new Vector2[_vertices.Count];
        
        for(int i = 0; i < normals.Length; i++)
            normals[i] = _vertices[i].Normal;

        return normals;
    }

    // TODO: this could also just be regenerated when points change
    private int[] GetTriangles() {
        int triCount = _vertices.Count - 2;

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
    
    private void UpdateCentroid() {
        if(_vertices == null || _vertices.Count < 2) {
            Centroid.transform.localPosition = Vector3.zero;
            Centroid.GetComponent<MeshRenderer>().enabled = false;
        } else {
            Vector2 centroid = CalculateCentroid();
            Centroid.transform.localPosition = new Vector3(centroid.x, centroid.y, -50);
            Centroid.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    // Formula from: http://en.wikipedia.org/wiki/Centroid#Centroid_of_polygon
    private Vector2 CalculateCentroid() {
        float signedArea = 0;
        float cx = 0;
        float cy = 0;

        for(int i = 0; i < _vertices.Count; i++) {
            Vector3 current = _vertices[i].transform.localPosition;
            Vector3 next = _vertices[(i + 1) % _vertices.Count].transform.localPosition;

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
