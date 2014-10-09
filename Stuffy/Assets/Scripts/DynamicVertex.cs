using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DynamicVertex : MonoBehaviour {
    public Vector2 Normal = new Vector3(0, 0, 1);

    public DynamicVertex LinkedVertex;

    public DynamicMesh ParentMesh { get; private set; }

    private Vector3 _lastPosition;

	void Awake() {
        ParentMesh = transform.parent.GetComponent<DynamicMesh>();
	}

	void Update() {
        if(LinkedVertex != null) {
            if(LinkedVertex.LinkedVertex == null)
                LinkedVertex.LinkedVertex = this;

            if(transform.position != _lastPosition)
                LinkedVertex.transform.position = transform.position;
        }

        _lastPosition = transform.position;
	}
}
