using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarClipping {
    private List<Vector2> _vertexList;

    public EarClipping(List<Vector2> vertexList) {
        _vertexList = vertexList;
    }

    public int[] GetTriangles() {
        List<int> tris = new List<int>();

        LinkedList<Vector2> vertices = new LinkedList<Vector2>(_vertexList.ToArray());
        LinkedList<Vector2> convex = new LinkedList<Vector2>();
        LinkedList<Vector2> reflex = new LinkedList<Vector2>();
        LinkedList<Vector2> ears = new LinkedList<Vector2>();

        // Calculate convex and reflex angles.
        for(LinkedListNode<Vector2> node = vertices.First; node != null; node = node.Next) {
            LinkedListNode<Vector2> prevNode = node.Previous ?? node.List.Last;
            LinkedListNode<Vector2> nextNode = node.Next ?? node.List.First;

            float angle = GetAngleForPoints(prevNode.Value, node.Value, nextNode.Value);

            if(angle < 180)
                convex.AddLast(node.Value);
            else
                reflex.AddLast(node.Value);
        }

        // Perform ear test on all vertices.
        for(LinkedListNode<Vector2> node = convex.First; node != null; node = node.Next) {
            LinkedListNode<Vector2> testNode = vertices.Find(node.Value);

            if(IsEar(testNode, reflex))
                ears.AddLast(testNode.Value);
        }

        // TODO - a lot of this can be improved with some book-keeping.
        while(vertices.Count > 3) {
            LinkedListNode<Vector2> firstEar = vertices.Find(ears.First.Value);

            LinkedListNode<Vector2> prevNode = firstEar.Previous ?? firstEar.List.Last;
            LinkedListNode<Vector2> nextNode = firstEar.Next ?? firstEar.List.First;

            LinkedListNode<Vector2> prevPrevNode = prevNode.Previous ?? prevNode.List.Last;
            LinkedListNode<Vector2> nextNextNode = nextNode.Next ?? nextNode.List.First;

            ears.Remove(firstEar.Value);
            vertices.Remove(firstEar.Value);

            tris.Add(_vertexList.IndexOf(prevNode.Value));
            tris.Add(_vertexList.IndexOf(firstEar.Value));
            tris.Add(_vertexList.IndexOf(nextNode.Value));

            // Skip this part if there are only two nodes left.
            if(prevPrevNode.Value != nextNode.Value) {
                float prevAngleBefore = GetAngleForPoints(prevPrevNode.Value, prevNode.Value, firstEar.Value);
                float prevAngleAfter = GetAngleForPoints(prevPrevNode.Value, prevNode.Value, nextNode.Value);

                if(prevAngleBefore < 180 && prevAngleAfter >= 180) {
                    convex.Remove(prevNode.Value);
                    reflex.AddLast(prevNode.Value);
                } else if(prevAngleBefore >= 180 && prevAngleAfter < 180) {
                    reflex.Remove(prevNode.Value);

                    LinkedListNode<Vector2> newConvex = convex.AddLast(prevNode.Value);
                    
                    if(IsEar(newConvex, reflex))
                        ears.AddLast(newConvex.Value);
                }

                float nextAngleBefore = GetAngleForPoints(firstEar.Value, nextNode.Value, nextNextNode.Value);
                float nextAngleAfter = GetAngleForPoints(prevNode.Value, nextNode.Value, nextNextNode.Value);

                if(nextAngleBefore < 180 && nextAngleAfter >= 180) {
                    convex.Remove(nextNode.Value);
                    reflex.AddLast(nextNode.Value);
                } else if(nextAngleBefore >= 180 && nextAngleAfter < 180) {
                    reflex.Remove(nextNode.Value);

                    LinkedListNode<Vector2> newConvex = convex.AddLast(nextNode.Value);

                    if(IsEar(newConvex, reflex))
                        ears.AddLast(newConvex.Value);
                }
            }
        }

        // Add the last remaining triangle.
        tris.Add(_vertexList.IndexOf(vertices.First.Value));
        tris.Add(_vertexList.IndexOf(vertices.First.Next.Value));
        tris.Add(_vertexList.IndexOf(vertices.Last.Value));

        return tris.ToArray();
    }

    private bool IsEar(LinkedListNode<Vector2> node, LinkedList<Vector2> reflex) {
        LinkedListNode<Vector2> prevNode = node.Previous ?? node.List.Last;
        LinkedListNode<Vector2> nextNode = node.Next ?? node.List.First;

        for(LinkedListNode<Vector2> reflexNode = reflex.First; reflexNode != null; reflexNode = reflexNode.Next) {
            if(reflexNode.Value != prevNode.Value
               && reflexNode.Value != node.Value
               && reflexNode.Value != nextNode.Value
               && PointInTriangle(reflexNode.Value, prevNode.Value, node.Value, nextNode.Value)) {

                return false;
            }
        }

        return true;
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

    // Transcribed from here: http://www.blackpawn.com/texts/pointinpoly/
    private bool PointInTriangle(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2) {
        Vector2 v0 = p2 - p0;
        Vector2 v1 = p1 - p0;
        Vector2 v2 = point - p0;

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

    /*
    private bool PointInTriangle(Vector2 point, Vector2 v0, Vector2 v1, Vector2 v2) {
        double area = 0.5 * (-v1.y * v2.x + v0.y * (-v1.x + v2.x) + v0.x * (v1.y - v2.y) + v1.x * v2.y);
        
        double s = 1 / (2 * area) * (v0.y * v2.x - v0.x * v2.y + (v2.y - v0.y) * point.x + (v0.x - v2.x) * point.y);
        double t = 1 / (2 * area) * (v0.x * v1.y - v0.y * v1.x + (v0.y - v1.y) * point.x + (v1.x - v0.x) * point.y);
        
        return s >= 0 && s <= 1 && t >= 0 && t <= 1 && s + t <= 1;
    }
    */

    private void PrintList(string prefix, LinkedList<Vector2> list) {
        LinkedListNode<Vector2> current = list.First;
        string output = prefix;
        
        while(current != null) {
            output += current.Value.ToString();
            current = current.Next;
        }
        
        Debug.Log(output);
    }

    private int GetIndex(LinkedListNode<Vector2> targetNode) {
        int index = 0;

        for(LinkedListNode<Vector2> node = targetNode.List.First; node != null; node = node.Next) {
            if(node.Value == targetNode.Value)
                return index;

            index++;
        }

        return -1;
    }
}
