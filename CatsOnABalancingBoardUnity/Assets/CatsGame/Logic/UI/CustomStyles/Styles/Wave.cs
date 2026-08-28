using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace OMC.UI.CustomStyles {
    internal class Wave : IVertexStyle {
        public string prefix => "wave";

        private Dictionary<int, int> waveVertices = new Dictionary<int, int>();

        private Dictionary<int, Vector3> waveVectors = new Dictionary<int, Vector3>();

        public void ReceiveStartVertex(int index, int value, bool broken) {
            waveVertices[index] = value;
        }

        public void UpdateVertices(int lastVisible) {
            foreach (int vertex in waveVertices.Keys) {
                waveVectors[vertex] = Vector3.up * math.sin(20f * (Time.time - vertex * 0.09f)) * waveVertices[vertex] * 10f;
            }
        }

        public void ApplyVertices(Vector3[] vertices, int lastVisible) {
            foreach (int vertex in waveVectors.Keys) {
                if (vertex >= lastVisible) {
                    break;
                }
                Vector3 offset = waveVectors[vertex];
                vertices[vertex] += offset;
                vertices[vertex + 1] += offset;
                vertices[vertex + 2] += offset;
                vertices[vertex + 3] += offset;
            }
        }
    }
}
