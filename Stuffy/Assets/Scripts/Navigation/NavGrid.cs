using UnityEngine;
using System.Collections;

public class NavGridNode {
    public readonly Vector2 Data;
    public readonly NavGridEdge[] Edges;
    
    public NavGridNode(Vector2 data) {
        Data = data;
        Edges = new NavGridEdge[4];
    }
}

public class NavGridEdge {
    public readonly NavGridNode Node1;
    public readonly NavGridNode Node2;
    
    public NavGridEdge(NavGridNode node1, NavGridNode node2) {
        Node1 = node1;
        Node2 = node2;
    }
}

public class NavGrid {
    public NavGridNode[,] Nodes { get; private set; }

    private Rect _area;
    private int _interval;

    public NavGrid(Rect area, int interval) {
        _area = area;
        _interval = interval;

        CreateGrid();
    }

    private void CreateGrid() {
        int width = (int)(_area.width / _interval);
        int height = (int)(_area.height / _interval);
        
        Nodes = new NavGridNode[width, height];

        float startX = _area.xMin + (_interval / 2.0f);
        float startY = _area.yMin + (_interval / 2.0f);
        
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                Vector2 data = new Vector2(startX + (x * _interval), startY + (y * _interval));
                Nodes[x, y] = new NavGridNode(data);
            }
        }
    }
}
