using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavGridNode {
    public readonly Vector2 Data;
    //public readonly NavGridEdge[] Edges;
    public readonly Dictionary<NavGridDirection, NavGridNode> Neighbors;
    
    public NavGridNode(Vector2 data) {
        Data = data;

        //Edges = new NavGridEdge[4];
        Neighbors = new Dictionary<NavGridDirection, NavGridNode>();
        Neighbors[NavGridDirection.N] = null;
        Neighbors[NavGridDirection.E] = null;
        Neighbors[NavGridDirection.S] = null;
        Neighbors[NavGridDirection.W] = null;
    }
}

/*
public class NavGridEdge {
    public readonly NavGridNode Node1;
    public readonly NavGridNode Node2;
    
    public NavGridEdge(NavGridNode node1, NavGridNode node2) {
        Node1 = node1;
        Node2 = node2;
    }

    public NavGridNode GetOther
}
*/

public class NavGrid {
    public NavGridNode[,] Nodes { get; private set; }

    private Rect _area;
    private int _interval;

    public NavGrid(Rect area, List<DynamicMesh> meshes, int interval) {
        _area = area;
        _interval = interval;

        CreateGrid(meshes);
    }

    public List<NavGridNode> FindPath(NavGridNode from, NavGridNode to) {
        List<NavGridNode> path = new List<NavGridNode>();

        PriorityQueue<NavGridNode> frontier = new PriorityQueue<NavGridNode>(100);
        frontier.Enqueue(from, 0);

        Dictionary<NavGridNode, NavGridNode> history = new Dictionary<NavGridNode, NavGridNode>();
        history[from] = null;

        Dictionary<NavGridNode, float> costHistory = new Dictionary<NavGridNode, float>();
        costHistory[from] = 0;

        while(frontier.Count > 0) {
            NavGridNode current = frontier.Dequeue();

            foreach(NavGridNode neighbor in current.Neighbors.Values) {
                if(neighbor == null) continue;

                float newCost = costHistory[current] + 1;

                if(!history.ContainsKey(neighbor) || newCost < costHistory[neighbor]) {
                    costHistory[neighbor] = newCost;

                    float priority = MathUtils.ManhattanDistance(current.Data, neighbor.Data);
                    frontier.Enqueue(neighbor, (double)priority);
                    history[neighbor] = current;
                }

                // Target reached. Reconstruct and return the path.
                if(neighbor == to) {
                    current = to;

                    while(current != null) {
                        path.Add(current);
                        current = history[current];
                    }

                    path.Reverse();

                    return path;
                }
            }
        }

        return null;
    }

    private void CreateGrid(List<DynamicMesh> meshes) {
        int width = (int)(_area.width / _interval);
        int height = (int)(_area.height / _interval);
        
        Nodes = new NavGridNode[width, height];

        float startX = _area.xMin + (_interval / 2.0f);
        float startY = _area.yMin + (_interval / 2.0f);

        // OPT - I'm sure this can be optimized. At the very least, you can slap a square
        // around each grid and only consider points in the squares instead of looping over 
        // the entire area.
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                Vector2 point = new Vector2(startX + (x * _interval), startY + (y * _interval));

                if(PointInMeshes(point, meshes)) {
                    NavGridNode newNode = new NavGridNode(point);
                    Nodes[x, y] = newNode;

                    if(y > 0) {
                        NavGridNode above = Nodes[x, y - 1];

                        if(above != null) {
                            newNode.Neighbors[NavGridDirection.N] = above;
                            above.Neighbors[NavGridDirection.S] = newNode;
                        }
                    }

                    if(x > 0) {
                        NavGridNode left = Nodes[x - 1 , y];

                        if(left != null) {
                            newNode.Neighbors[NavGridDirection.W] = left;
                            left.Neighbors[NavGridDirection.E] = newNode;
                        }
                    }
                }
            }
        }
    }

    private bool PointInMeshes(Vector2 point, List<DynamicMesh> meshes) {
        foreach(DynamicMesh mesh in meshes) {
            if(mesh.PointInMesh(point))
                return true;
        }
        return false;
    }
}
