using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DynamicMesh))]
public class DynamicMeshEditor : Editor {
    public override void OnInspectorGUI () {
        if(GUILayout.Button("Add Vertex"))
            AddVertex();

        if(GUILayout.Button("Clear"))
            ClearVertices();

        base.OnInspectorGUI();
	}

    private void ClearVertices() {
        DynamicVertex[] vertices = (target as DynamicMesh).DynamicVertices;

        foreach(DynamicVertex vertex in vertices)
            GameObject.DestroyImmediate(vertex.gameObject);

        (target as DynamicMesh).DynamicVertices = new DynamicVertex[0];
    }

    private void AddVertex() {
        DynamicVertex[] vertices = (target as DynamicMesh).DynamicVertices;

        DynamicVertex[] newVertices = new DynamicVertex[vertices.Length + 1];

        for(int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        newVertices[newVertices.Length - 1] = CreateNewVertex();

        (target as DynamicMesh).DynamicVertices = newVertices;

        EditorUtility.SetDirty(target);
    }

    private DynamicVertex CreateNewVertex() {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "DynamicVertex";
        go.transform.parent = (target as DynamicMesh).gameObject.transform;
        go.transform.localPosition = Vector3.zero;

        return go.AddComponent<DynamicVertex>();
    }
}
