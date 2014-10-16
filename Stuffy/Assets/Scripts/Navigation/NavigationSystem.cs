using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NavigationSystem : MonoBehaviour {
    public GameObject Scene;

    private List<DynamicMesh> _meshes;
    private NavGrid _navGrid;

    //private List<MeshLink> _meshLinks;

    void Awake() {
        // Collect existing meshes and add link listeners.
        _meshes = new List<DynamicMesh>(gameObject.GetComponentsInChildren<DynamicMesh>());

        /*
        foreach(DynamicMesh mesh in _meshes) {
            mesh.OnVertexLinkCreated += HandleVertexLinkCreated;
            mesh.OnVertexLinkDestroyed += HandleVertexLinkDestroyed;
        }
        */

        // Collect existing mesh links.
        //_meshLinks = new List<MeshLink>(gameObject.GetComponentsInChildren<MeshLink>());
    }

	void Update() {

	}

    public void AddMesh() {
        DynamicMesh newMesh = CreateNewMesh();
        
        //newMesh.OnVertexLinkCreated += HandleVertexLinkCreated;
        //newMesh.OnVertexLinkDestroyed += HandleVertexLinkDestroyed;

        _meshes.Add(newMesh);
    }
    
    public void RemoveMesh() {
        DynamicMesh lastMesh = _meshes[_meshes.Count - 1];
        _meshes.Remove(lastMesh);
        
        //lastMesh.OnVertexLinkCreated -= HandleVertexLinkCreated;
        //lastMesh.OnVertexLinkDestroyed -= HandleVertexLinkDestroyed;
        
        GameObject.DestroyImmediate(lastMesh.gameObject);
    }

    private DynamicMesh CreateNewMesh() {
        GameObject prototype = (GameObject)Resources.Load("Prefabs/DynamicMesh");
        
        GameObject go = (GameObject)GameObject.Instantiate(prototype);
        go.transform.parent = gameObject.transform;
        go.name = "DynamicMesh" + (_meshes.Count + 1);
        go.transform.localPosition = Vector3.zero;
        
        return go.GetComponent<DynamicMesh>();
    }

    public void RefreshGrid() {
        GameObject.DestroyImmediate(GameObject.Find("NavGridPoints"));

        Rect area = new Rect(-Scene.transform.localScale.x / 2,
                             -Scene.transform.localScale.y / 2,
                             Scene.transform.localScale.x,
                             Scene.transform.localScale.y);

        int interval = 60;

        _navGrid = new NavGrid(area, interval);

        VisualizeNavGrid();
    }

    private void VisualizeNavGrid() {
        GameObject navGridPoints = new GameObject("NavGridPoints");
        navGridPoints.transform.parent = gameObject.transform;
        navGridPoints.transform.localPosition = Vector3.zero;

        for(int y = 0; y < _navGrid.Nodes.GetLength(1); y++) {
            for(int x = 0; x < _navGrid.Nodes.GetLength(0); x++) {
                NavGridNode node = _navGrid.Nodes[x,y];

                if(PointInMesh(node.Data)) {
                    GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    point.transform.parent = navGridPoints.transform;
                    point.transform.localPosition = new Vector3(node.Data.x, node.Data.y, -50);
                    point.transform.localScale = new Vector3(10, 10, 10);
                }
            }
        }
    }

    private bool PointInMesh(Vector2 point) {
        foreach(DynamicMesh mesh in _meshes) {
            if(mesh.PointInMesh(point))
                return true;
        }

        return false;
    }

    /*
    private void CreateNavGrid() {
        GameObject navGridPoints = new GameObject("NavGridPoints");
        navGridPoints.transform.parent = gameObject.transform;
        navGridPoints.transform.localPosition = Vector3.zero;

        int size = 80;

        int width = (int)(Scene.transform.localScale.x / size);
        int height = (int)(Scene.transform.localScale.y / size);

        float startX = -(Scene.transform.localScale.x / 2) + (size / 2.0f);
        float startY = -(Scene.transform.localScale.y / 2) + (size / 2.0f);

        _navGrid = new GameObject[width, height];

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.transform.parent = navGridPoints.transform;
                point.transform.localPosition = new Vector3(startX + (x * size), startY + (y * size), -50);
                point.transform.localScale = new Vector3(10, 10, 10);

                _navGrid[x, y] = point;
            }
        }
    }
    */

    /*
    private void HandleVertexLinkCreated(DynamicVertex v1, DynamicVertex v2) {
        Debug.Log("link created");

        VertexLink vertexLink = new VertexLink(v1, v2);
        MeshLink meshLink = GetExistingMeshLinkForVertexLink(vertexLink);

        // No link exists between the two meshes.
        if(meshLink == null)
            meshLink = CreateNewMeshLink();

        // Link is already recorded.
        if(meshLink.HasExactVertexLink(vertexLink))
            return;

        // User tries to create a third link between two meshes.
        if(meshLink.IsFull()) {
            Debug.LogError("Can only link two pairs of vertices between a mesh.");
            v1.BreakVertexLink();
            return;
        }

        meshLink.AddVertexLink(vertexLink);
        _meshLinks.Add(meshLink);
    }

    private MeshLink GetExistingMeshLinkForVertexLink(VertexLink vertexLink) {
        foreach(MeshLink meshLink in _meshLinks) {
            if(meshLink.HasMatchingVertexLink(vertexLink))
                return meshLink;
        }

        return null;
    }

    private void HandleVertexLinkDestroyed(DynamicVertex v1, DynamicVertex v2) {
        Debug.Log("link destroyed");

        VertexLink vertexLink = new VertexLink(v1, v2);
        MeshLink meshLink = GetExistingMeshLinkForVertexLink(vertexLink);

        if(meshLink != null) {
            meshLink.RemoveVertexLink(vertexLink);

            if(meshLink.IsEmpty()) {
                _meshLinks.Remove(meshLink);
                GameObject.DestroyImmediate(meshLink.gameObject);
            }
        }
    }

    private MeshLink CreateNewMeshLink() {
        GameObject prototype = (GameObject)Resources.Load("Prefabs/MeshLink");

        GameObject go = (GameObject)GameObject.Instantiate(prototype);
        go.transform.parent = gameObject.transform;
        go.name = "MeshLink" + (_meshLinks.Count + 1);
        
        return go.GetComponent<MeshLink>();
    }
    */
}
