using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarClipping {
    private List<Vector2> _vertexList;

    public EarClipping(List<Vector2> vertexList) {
        _vertexList = vertexList;
    }

    public int[] GetTriangles() {
        LinkedList<Vector2> vertices = new LinkedList<Vector2>(_vertexList);

        LinkedList<Vector2> convex = new LinkedList<Vector2>();
        LinkedList<Vector2> reflex = new LinkedList<Vector2>();
        LinkedList<Vector2> ears = new LinkedList<Vector2>();

        // Calculate convex and reflex angles.
        for(LinkedListNode<Vector2> node = vertices.First; node != null; node = node.Next) {
            LinkedListNode<Vector2> prevNode = node.Previous ?? node.List.Last;
            LinkedListNode<Vector2> nextNode = node.Next ?? node.List.First;

            float angle = GetAngleForPoints(prevNode.Value, node.Value, nextNode.Value);

            //Debug.Log("angle for " + currentNode.Value + ": " + angle);

            if(angle < 180)
                convex.AddLast(node.Value);
            else
                reflex.AddLast(node.Value);

            node = nextNode;
        }

        PrintList("convex: ", convex);
        PrintList("reflex: ", reflex);

        // Perform ear test on all vertices.
        for(LinkedListNode<Vector2> node = convex.First; node != null; node = node.Next) {
            if(IsEar(node, reflex))
                ears.AddLast(node.Value);
        }

        PrintList("ears: ", ears);

        return null;
    }

    private bool IsEar(LinkedListNode<Vector2> node, LinkedList<Vector2> reflex) {
        LinkedListNode<Vector2> prevNode = node.Previous ?? node.List.Last;
        LinkedListNode<Vector2> nextNode = node.Next ?? node.List.First;

        for(LinkedListNode<Vector2> reflexNode = reflex.First; reflexNode != null; reflexNode = reflexNode.Next) {
            if(PointInTriangle(reflexNode.Value, prevNode.Value, node.Value, nextNode.Value))
                return false;
        }

        return true;
    }

    private void PrintList(string prefix, LinkedList<Vector2> list) {
        LinkedListNode<Vector2> current = list.First;
        string output = prefix;
        
        while(current != null) {
            output += current.Value.ToString();
            current = current.Next;
        }
        
        Debug.Log(output);
    }

    private float GetAngleForPoints(Vector2 p0, Vector2 p1, Vector2 p2) {
        Vector2 v1 = p0 - p1;
        Vector2 v2 = p2 - p1;

        float degrees = Vector2.Angle(v1, v2);
        float cross = Vector3.Cross(v1, v2).z;

        if(cross < 0)
            degrees = 360 - degrees;

        return degrees;
    }

    /*
    private bool PointInTriangle(Vector2 point, Vector2 v0, Vector2 v1, Vector2 v2) {
        double area = 0.5 * (-v1.y * v2.x + v0.y * (-v1.x + v2.x) + v0.x * (v1.y - v2.y) + v1.x * v2.y);
        
        double s = 1 / (2 * area) * (v0.y * v2.x - v0.x * v2.y + (v2.y - v0.y) * point.x + (v0.x - v2.x) * point.y);
        double t = 1 / (2 * area) * (v0.x * v1.y - v0.y * v1.x + (v0.y - v1.y) * point.x + (v1.x - v0.x) * point.y);
        
        return s >= 0 && s <= 1 && t >= 0 && t <= 1 && s + t <= 1;
    }
    */

    private bool PointInTriangle(Vector2 point, Vector2 v0, Vector2 v1, Vector2 v2) {
        double area = 0.5 * (-v1.y * v2.x + v0.y * (-v1.x + v2.x) + v0.x * (v1.y - v2.y) + v1.x * v2.y);

        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            
        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
}
