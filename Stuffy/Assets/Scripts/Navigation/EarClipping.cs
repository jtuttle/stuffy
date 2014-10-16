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

        LinkedListNode<Vector2> headNode = vertices.First;
        LinkedListNode<Vector2> currentNode = null;

        while(currentNode != headNode) {
            if(currentNode == null) currentNode = headNode;

            LinkedListNode<Vector2> prevNode = currentNode.Previous ?? currentNode.List.Last;
            LinkedListNode<Vector2> nextNode = currentNode.Next ?? currentNode.List.First;

            /*
            Vector2 p0 = current.Previous.Value;
            Vector2 p1 = current.Value;
            Vector2 p2 = current.Next.Value;
            */
           
            float angle = GetAngleForPoints(prevNode.Value, currentNode.Value, nextNode.Value);

            Debug.Log("angle for " + currentNode.Value + ": " + angle);

            if(angle < 180)
                convex.AddLast(currentNode.Value);
            else
                reflex.AddLast(currentNode.Value);

            currentNode = nextNode;
        }

        PrintList(convex);
        PrintList(reflex);

        return null;
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

    private void PrintList(LinkedList<Vector2> list) {
        LinkedListNode<Vector2> current = list.First;
        string output = "";

        while(current != null) {
            output += current.Value.ToString();
            current = current.Next;
        }

        Debug.Log(output);
    }
}
