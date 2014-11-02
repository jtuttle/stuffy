using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NavigationSystem : MonoBehaviour {
    public tk2dCamera Camera;

    public GameObject Player;
    public GameObject Scene;

    private List<DynamicMesh> _meshes;
    private NavGrid _navGrid;

    private Queue<NavGridNode> _navPath;

    void Awake() {
        _meshes = new List<DynamicMesh>(gameObject.GetComponentsInChildren<DynamicMesh>());

        RefreshGrid();
    }

    void Start() {

    }

	void Update() {
        // TODO - this stuff should be split into a game state machine state so that it's not the 
        // responsibility of the navigation system.
        if(Input.GetMouseButtonDown(0)) {
            Vector2 newTarget = Camera.ScreenCamera.ScreenToWorldPoint(Input.mousePosition);

            NavGridNode targetNearest = GetNearestNode(newTarget);
            NavGridNode playerNearest = GetNearestNode(Player.transform.position);

            List<NavGridNode> path = _navGrid.FindPath(playerNearest, targetNearest);
            _navPath = new Queue<NavGridNode>(path);

            Debug.Log(_navPath.Count);

            Player.GetComponent<Animator>().SetBool("Walking", true);
        }

        if(_navPath != null && _navPath.Count > 0) {
            NavGridNode target = _navPath.Peek();

            float distX = target.Data.x - Player.transform.position.x;
            float distY = target.Data.y - Player.transform.position.y;
            float angle = Mathf.Atan2(distY, distX);

            float vx = Mathf.Min(Mathf.Cos(angle) * 6, Mathf.Abs(distX));
            float vy = Mathf.Min(Mathf.Sin(angle) * 6, Mathf.Abs(distY));

            //Vector3 prevPos = Player.transform.position;

            Player.transform.position += new Vector3(vx, vy, 0);

            /*
            Vector3 moveDirection = (Player.transform.position - prevPos).normalized;
            Vector3 rotation = Quaternion.Euler(80, 0, 0) * moveDirection;

            Player.transform.localEulerAngles = rotation;
            */

            /*
            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation,
                                                        Quaternion.LookRotation(prevPos - Player.transform.position),
                                                        Time.fixedDeltaTime * 1);
            */

            //Player.transform.rotation = Quaternion.LookRotation(prevPos + new Vector3(vx, vy, 0));

            if(Vector2.Distance(Player.transform.position, target.Data) < 0.05) {
                _navPath.Dequeue();

                if(_navPath.Count == 0) Player.GetComponent<Animator>().SetBool("Walking", false);
            }
        }
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

                if(node == null) continue;

                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.transform.parent = navGridPoints.transform;
                point.transform.localPosition = new Vector3(node.Data.x, node.Data.y, -50);
                point.transform.localScale = new Vector3(10, 10, 10);
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

    private NavGridNode GetNearestNode(Vector2 target) {
        NavGridNode nearest = null;
        float nearestDistance = float.PositiveInfinity;

        for(int y = 0; y < _navGrid.Nodes.GetLength(1); y++) {
            for(int x = 0; x < _navGrid.Nodes.GetLength(0); x++) {
                NavGridNode node = _navGrid.Nodes[x,y];

                if(node == null) continue;

                Vector2 data = node.Data;

                float distance = Vector2.Distance(data, target);

                if(nearest == null || distance < nearestDistance) {
                    nearest = node;
                    nearestDistance = distance;
                }
            }
        }

        return nearest;
    }
}
