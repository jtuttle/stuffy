using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DynamicVertex))]
public class DynamicVertexEditor : Editor {
    public override void OnInspectorGUI () {
        if((target as DynamicVertex).LinkedVertex != null) {
            if(GUILayout.Button("Break Vertex Link"))
                (target as DynamicVertex).BreakVertexLink();
        }

        base.OnInspectorGUI();
	}
}
