using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DynamicMesh))]
public class DynamicMeshEditor : Editor {
    public override void OnInspectorGUI () {
        if(GUILayout.Button("Add Vertex"))
            AddVertex();

        if(GUILayout.Button("Remove Vertex"))
            RemoveVertex();

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
    }

    private void RemoveVertex() {
        DynamicVertex[] vertices = (target as DynamicMesh).DynamicVertices;

        DynamicVertex[] newVertices = new DynamicVertex[vertices.Length - 1];

        for(int i = 0; i < newVertices.Length; i++)
            newVertices[i] = vertices[i];

        GameObject.DestroyImmediate(vertices[vertices.Length - 1].gameObject);

        (target as DynamicMesh).DynamicVertices = newVertices;
    }

    private DynamicVertex CreateNewVertex() {
        GameObject prototype = (GameObject)Resources.Load("Prefabs/DynamicVertex");

        GameObject go = (GameObject)GameObject.Instantiate(prototype);
        go.transform.parent = (target as DynamicMesh).gameObject.transform;

        DynamicVertex[] vertices = (target as DynamicMesh).DynamicVertices;

        go.name = "DynamicVertex" + (vertices.Length + 1);

        if(vertices.Length > 1) {
            Vector3 firstPos = vertices[0].transform.localPosition;
            Vector3 lastPos = vertices[vertices.Length - 1].transform.localPosition;

            go.transform.localPosition = MathUtils.MidPoint(firstPos, lastPos);
        } else {
            go.transform.localPosition = Vector3.zero;
        }

        return go.GetComponent<DynamicVertex>();
    }
}
