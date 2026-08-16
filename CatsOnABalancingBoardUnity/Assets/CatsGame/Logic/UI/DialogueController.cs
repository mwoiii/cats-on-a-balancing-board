using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace OMC.UI {
    public class DialogueController : MonoBehaviour {

        public float defaultDelay = 0.05f;

        [SerializeField]
        private TextMeshProUGUI textInput;

        private int visibleCharacters;

        private bool ready;

        private List<int> shakeVertices = new List<int>();

        private List<int> shakeBreaks = new List<int>();

        private static Regex startRegex = new Regex("<style=\"(\\w*)\">");

        private static Regex endRegex = new Regex("</style>");

        private struct StyleInfo {
            public bool isStart;
            public int index;
            public string styleName;
        }


        private void OnEnable() {
            if (textInput && ready) {
                RunDialogue();
            }
        }

        private void Start() {
            if (!textInput) {
                textInput = GetComponent<TextMeshProUGUI>();
            }

            if (textInput) {

                //foreach (var x in styleStack) {
                //    Debug.Log($"{x.pushMode}, {x.styleName}, {x.index}");
                //}
                ready = true;
                RunDialogue();
            }
        }

        private void RunDialogue() {
            ParseStyleTags();
            textInput.maxVisibleCharacters = 0;
            StartCoroutine(WriteText());
            StartCoroutine(AnimateText());
        }

        private IEnumerator WriteText() {
            for (visibleCharacters = 0; visibleCharacters < textInput.text.Length; visibleCharacters++) {
                if (textInput) {
                    textInput.maxVisibleCharacters = visibleCharacters;
                    yield return new WaitForSeconds(defaultDelay);
                }
            }
        }

        private void ParseStyleTags() {
            // get queue of style starts and ends, in order
            Queue<StyleInfo> styleQueue = new Queue<StyleInfo>();
            MatchCollection startMatches = startRegex.Matches(textInput.text);
            MatchCollection endMatches = endRegex.Matches(textInput.text);
            var combinedMatches = startMatches.Concat(endMatches).OrderBy(x => x.Index);
            foreach (Match match in combinedMatches) {
                styleQueue.Enqueue(new StyleInfo {
                    isStart = match.Groups.Count > 1,
                    styleName = match.Groups[1].Value.ToLower(),
                    index = match.Index
                });
            }

            // run through the process of recording which vertices fall between tags
            Stack<string> styleStack = new Stack<string>();
            Dictionary<string, int> styleActive = new Dictionary<string, int>();

            bool shakeWasActive = false; // to make it such that each shake block is separate

            textInput.ForceMeshUpdate(false, false);
            foreach (var character in textInput.textInfo.characterInfo) {
                if (!character.isVisible) {
                    continue;
                }

                while (styleQueue.TryPeek(out StyleInfo result)) {
                    if (result.index < character.index) {
                        styleQueue.Dequeue();
                        if (result.isStart) {
                            styleActive.TryGetValue(result.styleName, out int active);
                            styleActive[result.styleName] = active + 1;
                            styleStack.Push(result.styleName);
                        } else {
                            if (styleStack.Count <= 0) {
                                continue;
                            }
                            string styleName = styleStack.Pop();
                            styleActive.TryGetValue(styleName, out int active);
                            styleActive[styleName] = math.max(active - 1, 0);
                        }
                    } else {
                        break;
                    }
                }

                if (styleActive.TryGetValue("shake", out int value) && value > 0) {
                    shakeWasActive = true;
                    AddVertices(shakeVertices, character.vertexIndex);
                } else if (value <= 0 && shakeWasActive) {
                    shakeWasActive = false;
                    shakeBreaks.Add(character.vertexIndex + 3);
                }
            }
        }

        private IEnumerator AnimateText() {
            while (true) {
                textInput.ForceMeshUpdate(false, false);
                var vertices = textInput.textInfo.meshInfo[0].vertices;

                int lastVisible = visibleCharacters * 4 - 1;

                ApplyShake(vertices, lastVisible);

                textInput.textInfo.meshInfo[0].vertices = vertices;
                textInput.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

                yield return new WaitForSeconds(defaultDelay);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyShake(Vector3[] vertices, int lastVisible) {
            int nextBreak = 0;
            Vector3 randomShake = GetRandomShake();
            foreach (int vertex in shakeVertices) {
                if (vertex > lastVisible) {
                    break;
                }
                if (nextBreak < shakeBreaks.Count && vertex > shakeBreaks[nextBreak]) {
                    randomShake = GetRandomShake();
                    nextBreak++;
                }
                vertices[vertex] += randomShake;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddVertices(List<int> vertices, int start) {
            vertices.Add(start);
            vertices.Add(start + 1);
            vertices.Add(start + 2);
            vertices.Add(start + 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetRandomShake() {
            return Vector3.up * UnityEngine.Random.Range(-20f, 20f) + Vector3.right * UnityEngine.Random.Range(-20f, 20f);
        }
    }
}
