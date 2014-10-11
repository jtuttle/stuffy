using UnityEngine;
using System.Collections;

public class VertexLink {
    public DynamicVertex Vertex1 { get; private set; }
    public DynamicVertex Vertex2 { get; private set; }

    public VertexLink(DynamicVertex v1, DynamicVertex v2) {
        Vertex1 = v1;
        Vertex2 = v2;
    }

    // Two VertexLinks should be considered equal if they hold 
    // the same two vertices, regardless of which is which.
    public bool HasEqualVertices(VertexLink other) {
        if((System.Object)other == null) return false;

        return (Vertex1 == other.Vertex1 && Vertex2 == other.Vertex2) ||
            (Vertex1 == other.Vertex2 && Vertex2 == other.Vertex1);
    }

    public bool HasSameMeshes(VertexLink other) {
        return (Vertex1.ParentMesh == other.Vertex1.ParentMesh && Vertex2.ParentMesh == other.Vertex2.ParentMesh) ||
            (Vertex1.ParentMesh == other.Vertex2.ParentMesh && Vertex2.ParentMesh == other.Vertex1.ParentMesh);
    }
}
