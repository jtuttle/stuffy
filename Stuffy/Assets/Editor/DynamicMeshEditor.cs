using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DynamicMesh))]
public class DynamicMeshEditor : Editor {
    public override void OnInspectorGUI () {
        if(GUILayout.Button("Add Vertex"))
            (target as DynamicMesh).AddVertex();

        if(GUILayout.Button("Remove Vertex"))
            (target as DynamicMesh).RemoveVertex();

        // These clear methods are kind of dangerous. Might
        // be best to just provide one-by-one removal.
        /*
        if(GUILayout.Button("Clear"))
            (target as DynamicMesh).ClearVertices();
        */

        base.OnInspectorGUI();
	}
}
