using OMC.UI.CustomStyles;
using System.Collections.Generic;
using UnityEngine;
using static OMC.Util.Helpers;

namespace Assets.CatsGame.Logic.UI.CustomStyles {
    internal class Shake : IVertexStyle {
        public string prefix => "shake";

        private Dictionary<int, int> shakeCharacters = new Dictionary<int, int>();

        private Dictionary<int, Vector3> shakeVectors = new Dictionary<int, Vector3>();

        public void ReceiveStartVertex(int index, int value, bool broken) {
            shakeCharacters[index] = value;
        }

        public void UpdateValues(int lastVisible) {
            foreach (int vertex in shakeCharacters.Keys) {
                if (vertex >= lastVisible) {
                    break;
                }
                shakeVectors[vertex] = GetRandomShake(shakeCharacters[vertex] * 4f);
            }
        }

        public void ApplyValues(Vector3[] vertices, int lastVisible) {
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
