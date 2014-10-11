using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(NavigationSystem))]
public class NavigationSystemEditor : Editor {
    public override void OnInspectorGUI () {
        if(GUILayout.Button("Add Mesh"))
            (target as NavigationSystem).AddMesh();

        if(GUILayout.Button("Remove Mesh"))
            (target as NavigationSystem).RemoveMesh();

        // These clear methods are kind of dangerous. Might
        // be best to just provide one-by-one removal.
        /*
        if(GUILayout.Button("Clear"))
            (target as DynamicMesh).ClearVertices();
        */

        base.OnInspectorGUI();
	}
}
