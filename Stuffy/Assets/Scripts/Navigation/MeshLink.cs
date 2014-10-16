using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MeshLink : MonoBehaviour {
    private VertexLink VertexLink1;
    private VertexLink VertexLink2;

    void Update() {
        bool full = IsFull();

        if(full) {
            Vector3 pos1 = VertexLink1.Vertex1.transform.position;
            Vector3 pos2 = VertexLink2.Vertex1.transform.position;
            gameObject.transform.position = MathUtils.MidPoint(pos1, pos2) + new Vector3(0, 0, -50);
        }

        GetComponent<MeshRenderer>().enabled = full;
    }

    // Two MeshLink should be considered equal if they hold vertex
    // links that are also equal, regardless of which is which.
    public bool HasEqualVertexLinks(MeshLink other) {
        if((object)other == null) return false;
        
        return (VertexLink1.Equals(other.VertexLink1) && VertexLink2.Equals(other.VertexLink2)) ||
            (VertexLink1.Equals(other.VertexLink2) && VertexLink2.Equals(other.VertexLink1));
    }

    public bool HasExactVertexLink(VertexLink vertexLink) {
        return (VertexLink1 != null && VertexLink1.HasEqualVertices(vertexLink)) ||
            (VertexLink2 != null && VertexLink2.HasEqualVertices(vertexLink));
    }

    public bool HasMatchingVertexLink(VertexLink vertexLink) {
        return (VertexLink1 != null && VertexLink1.HasSameMeshes(vertexLink)) ||
            (VertexLink2 != null && VertexLink2.HasSameMeshes(vertexLink));
    }

    public bool IsFull() {
        return VertexLink1 != null && VertexLink2 != null;
    }

    public bool IsEmpty() {
        return VertexLink1 == null && VertexLink2 == null;
    }

    public void AddVertexLink(VertexLink vertexLink) {
        if(VertexLink1 == null) {
            VertexLink1 = vertexLink;
        } else if(VertexLink2 == null) {
            VertexLink2 = vertexLink;
        }
    }

    public void RemoveVertexLink(VertexLink vertexLink) {
        if(VertexLink1 != null && VertexLink1.HasEqualVertices(vertexLink))
            VertexLink1 = null;
        else if(VertexLink2 != null && VertexLink2.HasEqualVertices(vertexLink))
            VertexLink2 = null;
    }
}
