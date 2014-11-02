using System;
using System.Collections.Generic;
using UnityEngine;

public class PolygonIntersection {
    private class Edge {
        public readonly Vector2 From;
        public readonly Vector2 To;

        public Edge(Vector2 from, Vector2 to) {
            From = from;
            To = to;
        }
    }

    public static Vector2[] Intersection(Vector2[] poly1, Vector2[] poly2) {
        if(poly1.Length < 3 || poly2.Length < 3) {
            throw new ArgumentException(
                string.Format("The polygons passed in must have at least 3 points: subject={0}, clip={1}", 
                              poly1.Length.ToString(), poly2.Length.ToString()));
        }

        List<Vector2> outputList = new List<Vector2>(poly1);

        if(!IsClockwise(poly1))
            outputList.Reverse();

        foreach(Edge clipEdge in IterateEdgesClockwise(poly2)) {
            List<Vector2> inputList = new List<Vector2>(outputList.ToArray());
            outputList.Clear();

            if(inputList.Count == 0)
                break;

            Vector2 S = inputList[inputList.Count - 1];

            foreach(Vector2 E in inputList) {
                if(IsInside(clipEdge, E)) {
                    if(!IsInside(clipEdge, S)) {
                        Vector2? vector = GetIntersect(S, E, clipEdge.From, clipEdge.To);

                        if(vector == null)
                            throw new ApplicationException("Line segments don't intersect"); // may be colinear, or may be a bug
                        else
                            outputList.Add(vector.Value);
                    }

                    outputList.Add(E);
                } else if(IsInside(clipEdge, S)) {
                    Vector2? vector = GetIntersect(S, E, clipEdge.From, clipEdge.To);
                    
                    if(vector == null)
                        throw new ApplicationException("Line segments don't intersect"); // may be colinear, or may be a bug
                    else
                        outputList.Add(vector.Value);
                }

                S = E;
            }
        }

        return outputList.ToArray();
    }

    private static IEnumerable<Edge> IterateEdgesClockwise(Vector2[] polygon) {
        if(IsClockwise(polygon)) {
            for(int i = 0; i < polygon.Length - 1; i++)
                yield return new Edge(polygon[i], polygon[i + 1]);

            yield return new Edge(polygon[polygon.Length - 1], polygon[0]);
        } else {
            for(int i = polygon.Length - 1; i > 0; i--)
                yield return new Edge(polygon[i], polygon[i - 1]);

            yield return new Edge(polygon[0], polygon[polygon.Length - 1]);
        }
    }

    private static Vector2? GetIntersect(Vector2 line1From, Vector2 line1To, Vector2 line2From, Vector2 line2To) {
        Vector2 direction1 = line1To - line1From;
        Vector2 direction2 = line2To - line2From;
        double dotPerp = (direction1.x * direction2.y) - (direction1.y * direction2.x);

        if(IsNearZero(dotPerp))
            return null;

        Vector2 c = line2From - line1From;
        double t = (c.x * direction2.y - c.y * direction2.x) / dotPerp;

        return line1From + ((float)t * direction1);
    }

    private static bool IsNearZero(double testValue) {
        return Math.Abs(testValue) <= 0.000000001d;
    }

    private static bool IsInside(Edge edge, Vector2 test) {
        bool? isLeft = IsLeftOf(edge, test);

        if(isLeft == null)
            return true;

        return !isLeft.Value;
    }

    private static bool IsClockwise(Vector2[] polygon) {
        for(int i = 2; i < polygon.Length; i++) {
            bool? isLeft = IsLeftOf(new Edge(polygon[0], polygon[1]), polygon[i]);

            if(isLeft != null)
                return isLeft.Value;
        }

        throw new ArgumentException("All the points in the polygon are colinear");
    }

    private static bool? IsLeftOf(Edge edge, Vector2 test) {
        Vector2 tmp1 = edge.To - edge.From;
        Vector2 tmp2 = test - edge.To;

        // dot product of perpendicular
        double x = (tmp1.x * tmp2.y) - (tmp1.y * tmp2.x);

        if(x < 0)
            return false;
        else if(x > 0)
            return true;
        else
            return null; // colinear
    }
}
