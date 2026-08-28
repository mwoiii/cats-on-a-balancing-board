using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static OMC.Util.Helpers;

namespace OMC.UI.CustomStyles {
    internal class ShakeGroup : IVertexStyle {
        public string prefix => "shakegroup";

        private Dictionary<int, int> shakeGroupVertices = new Dictionary<int, int>();
        private List<int> shakeGroupBreaks = new List<int>();

        private Dictionary<int, Vector3> shakeGroupVectors = new Dictionary<int, Vector3>();

        public void ReceiveStartVertex(int index, int value, bool broken) {
            shakeGroupVertices[index] = value;
            if (broken) {
                shakeGroupBreaks.Add(index - 1);
            }
        }

        public void UpdateVertices(int lastVisible) {
            int nextBreak = 0;
            Vector3 randomShake = GetRandomShake(shakeGroupVertices.Values.First() * 4f);
            foreach (int vertex in shakeGroupVertices.Keys) {
                if (nextBreak < shakeGroupBreaks.Count && vertex > shakeGroupBreaks[nextBreak]) {
                    randomShake = GetRandomShake(shakeGroupVertices[vertex] * 4f);
                    nextBreak++;
                }
                shakeGroupVectors[vertex] = randomShake;
            }
        }

        public void ApplyVertices(Vector3[] vertices, int lastVisible) {
            foreach (int vertex in shakeGroupVectors.Keys) {
                if (vertex >= lastVisible) {
                    break;
                }
                Vector3 shake = shakeGroupVectors[vertex];
                vertices[vertex] += shake;
                vertices[vertex + 1] += shake;
                vertices[vertex + 2] += shake;
                vertices[vertex + 3] += shake;
            }
        }
    }
}
