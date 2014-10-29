using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NavigationSystem : MonoBehaviour {
    public GameObject Player;
    public GameObject Scene;

    private List<DynamicMesh> _meshes;
    private NavGrid _navGrid;

    void Awake() {
        _meshes = new List<DynamicMesh>(gameObject.GetComponentsInChildren<DynamicMesh>());
    }

	void Update() {

	}

    public void AddMesh() {
        DynamicMesh newMesh = CreateNewMesh();
        _meshes.Add(newMesh);
    }
    
    public void RemoveMesh() {
        DynamicMesh lastMesh = _meshes[_meshes.Count - 1];
        _meshes.Remove(lastMesh);

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

        Bounds sceneBounds = Scene.GetComponent<tk2dSprite>().GetBounds();
        Rect area = new Rect(sceneBounds.min.x, sceneBounds.min.y,
                             sceneBounds.size.x, sceneBounds.size.y);

        int interval = 50;

        _navGrid = new NavGrid(area, _meshes, interval);

        VisualizeNavGrid();
    }

    private void VisualizeNavGrid() {
        GameObject navGridPoints = new GameObject("NavGridPoints");
        navGridPoints.transform.parent = gameObject.transform;
        navGridPoints.transform.localPosition = Vector3.zero;

        for(int y = 0; y < _navGrid.Nodes.GetLength(1); y++) {
            for(int x = 0; x < _navGrid.Nodes.GetLength(0); x++) {
                NavGridNode node = _navGrid.Nodes[x,y];

                if(node != null) {
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
}
