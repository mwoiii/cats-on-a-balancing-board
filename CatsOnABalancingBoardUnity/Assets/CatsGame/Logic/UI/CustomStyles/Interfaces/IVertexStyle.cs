using UnityEngine;

namespace OMC.UI.CustomStyles {
    public interface IVertexStyle : IStyle {
        void ReceiveStartVertex(int index, int value, bool broken);

        void UpdateVertices(int lastVisible);

        void ApplyVertices(Vector3[] vertices, int lastVisible);
    }
}
