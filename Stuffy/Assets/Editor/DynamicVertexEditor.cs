using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DynamicVertex))]
public class DynamicVertexEditor : Editor {
    public override void OnInspectorGUI () {
        if((target as DynamicVertex).LinkedVertex != null) {
            if(GUILayout.Button("Break Vertex Link"))
                BreakVertexLink();
        }

        base.OnInspectorGUI();
	}

    private void BreakVertexLink() {
        DynamicVertex linkedVertex = (target as DynamicVertex).LinkedVertex;

        linkedVertex.LinkedVertex = null;
        (target as DynamicVertex).LinkedVertex = null;
    }
}
