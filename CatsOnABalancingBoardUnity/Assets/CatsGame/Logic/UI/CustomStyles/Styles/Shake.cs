using System.Collections.Generic;
using UnityEngine;
using static OMC.Util.Helpers;

namespace OMC.UI.CustomStyles {
    internal class Shake : IVertexStyle {
        public string prefix => "shake";

        private Dictionary<int, int> shakeVertices = new Dictionary<int, int>();

        private Dictionary<int, Vector3> shakeVectors = new Dictionary<int, Vector3>();

        public void ReceiveStartVertex(int index, int value, bool broken) {
            shakeVertices[index] = value;
        }

        public void UpdateVertices(int lastVisible) {
            foreach (int vertex in shakeVertices.Keys) {
                shakeVectors[vertex] = GetRandomShake(shakeVertices[vertex] * 4f);
            }
        }

        public void ApplyVertices(Vector3[] vertices, int lastVisible) {
            foreach (int vertex in shakeVectors.Keys) {
                if (vertex >= lastVisible) {
                    break;
                }
                Vector3 shake = shakeVectors[vertex];
                vertices[vertex] += shake;
                vertices[vertex + 1] += shake;
                vertices[vertex + 2] += shake;
                vertices[vertex + 3] += shake;
            }
        }
    }
}
