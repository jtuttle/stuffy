using UnityEngine;
using System.Collections;

public class MathUtils {
    public static Vector3 MidPoint(Vector3 v1, Vector3 v2) {
        return new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
    }

    public static float ManhattanDistance(Vector2 v1, Vector2 v2) {
        return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
    }                                                              
}
