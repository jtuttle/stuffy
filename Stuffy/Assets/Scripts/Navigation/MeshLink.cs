using UnityEngine;
using System.Collections;

public class MeshLink : MonoBehaviour {
    public VertexLink VertexLink1 { get; private set; }
    public VertexLink VertexLink2 { get; private set; }

    // Two MeshLink should be considered equal if they hold vertex
    // links that are also equal, regardless of which is which.
    public bool HasEqualVertexLinks(MeshLink other) {
        if((object)other == null) return false;
        
        return (VertexLink1.Equals(other.VertexLink1) && VertexLink2.Equals(other.VertexLink2)) ||
            (VertexLink1.Equals(other.VertexLink2) && VertexLink2.Equals(other.VertexLink1));
    }

    public bool HasMatchingLink(VertexLink vertexLink) {
        return VertexLink1 != null && VertexLink1.HasSameMeshes(vertexLink) ||
            VertexLink2 != null && VertexLink2.HasSameMeshes(vertexLink);
    }

    public bool IsFull() {
        return VertexLink1 != null && VertexLink2 != null;
    }

    public void AddVertexLink(VertexLink vertexLink) {
        if(VertexLink1 == null)
            VertexLink1 = vertexLink;
        else if(VertexLink2 == null)
            VertexLink2 = vertexLink;
    }
}
