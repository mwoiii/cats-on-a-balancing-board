using UnityEngine;

namespace OMC.UI.CustomStyles {
    public interface IVertexStyle : IStyle {
        void ReceiveStartVertex(int index, int value, bool broken);

        void UpdateValues(int lastVisible);

        void ApplyValues(Vector3[] vertices, int lastVisible);
    }
}
