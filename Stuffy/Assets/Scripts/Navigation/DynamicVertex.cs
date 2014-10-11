using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class DynamicVertex : MonoBehaviour {
    public delegate void LinkCreatedHandler(DynamicVertex v1, DynamicVertex v2);
    public event LinkCreatedHandler OnLinkCreated = delegate { };

    public delegate void LinkDestroyedHandler(DynamicVertex v1, DynamicVertex v2);
    public event LinkDestroyedHandler OnLinkDestroyed = delegate { };

    public Vector2 Normal = new Vector3(0, 0, 1);
    public DynamicVertex LinkedVertex;

    private bool _linked;
    private Vector3 _lastPosition;

    public DynamicMesh ParentMesh {
        get { return transform.parent.GetComponent<DynamicMesh>(); }
    }

	void Awake() {

	}

	void Update() {
        // Detect when a link has been created through drag-and-drop.
        if(!_linked && LinkedVertex != null) {
            HandleVertexLinkCreated();
        }

        // Safeguard against link being broken using Unity editor.
        if(_linked && LinkedVertex == null)
            _linked = false;

        // TODO - there's a bug with this where the mesh won't update correctly if you
        // drag one of the vertices but it updates correctly when you drag the other.
        // It probably has to do with the order the meshes are being recreated. One of
        // them is probably being recreated with the old position of the vertex.
        if(LinkedVertex != null && transform.position != _lastPosition)
            LinkedVertex.transform.position = transform.position;

        _lastPosition = transform.position;
	}

    public void BreakVertexLink() {
        if(LinkedVertex == null) return;

        DynamicVertex temp = LinkedVertex;
        LinkedVertex = null;

        _linked = false;

        OnLinkDestroyed(this, temp);

        temp.BreakVertexLink();
    }

    // The vertex link is actually created by drag-and-dropping, but we 
    // react to it in this method and trigger the event handler.
    private void HandleVertexLinkCreated() {
        if(LinkedVertex == null) return;

        if(ParentMesh == LinkedVertex.ParentMesh) {
            Debug.LogError("Can not link to vertex in same mesh.");
            BreakVertexLink();
            return;
        }

        _linked = true;
        
        LinkedVertex.LinkedVertex = this;
        LinkedVertex.transform.position = transform.position;
        
        OnLinkCreated(this, LinkedVertex);
    }
}
