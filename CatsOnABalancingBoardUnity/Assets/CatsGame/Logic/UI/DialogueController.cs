using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace OMC.UI {
    public class DialogueController : MonoBehaviour {

        public float writeDelay = 0.05f;

        public float animationDelay = 0.05f;

        [SerializeField]
        private TextMeshProUGUI textInput;

        private int maxVisibleCharacters;

        private int visibleCharacters;

        private float writeCooldown;

        private float animateCooldown;

        private bool parsed;

        private Vector3[] animatedVertices;

        private List<int> shakeVertices = new List<int>();
        private List<int> shakeBreaks = new List<int>();

        private Dictionary<int, int> pauseCharacters = new Dictionary<int, int>();

        private static Regex startRegex = new Regex("<style=\"(\\w*)\">");
        private static Regex endRegex = new Regex("</style>");

        private const float PauseDelay = 1f;

        private struct StyleInfo {
            public bool isStart;
            public int index;
            public string styleName;
        }

        private void OnEnable() {
            if (textInput && parsed) {
                ResetDialogue();
            }
        }

        private void Start() {
            if (!textInput) {
                textInput = GetComponent<TextMeshProUGUI>();
            }

            if (textInput) {
                ParseText();
                ResetDialogue();
            }
        }

        private void ResetDialogue() {
            textInput.maxVisibleCharacters = 0;
            visibleCharacters = 0;
            animateCooldown = 0f;
            writeCooldown = 0f;
        }

        private void Update() {
            UpdateWriteText();
            UpdateAnimateText();
        }

        private void UpdateWriteText() {
            if (visibleCharacters < maxVisibleCharacters) {
                writeCooldown -= Time.deltaTime;
                if (writeCooldown <= 0) {
                    visibleCharacters++;
                    textInput.maxVisibleCharacters = visibleCharacters;
                    if (pauseCharacters.TryGetValue(visibleCharacters, out int pauseValue)) {
                        writeCooldown = PauseDelay * pauseValue;
                    } else {
                        writeCooldown = writeDelay;
                    }
                }
            }
        }

        private void UpdateAnimateText() {
            animateCooldown -= Time.deltaTime;
            if (animateCooldown <= 0) {
                textInput.ForceMeshUpdate(false, false);
                animatedVertices = textInput.textInfo.meshInfo[0].vertices;
                int lastVisible = visibleCharacters * 4 - 1;
                ApplyShake(animatedVertices, lastVisible);
                animateCooldown = animationDelay;
            }

            textInput.textInfo.meshInfo[0].vertices = animatedVertices;
            textInput.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }

        private void ParseText() {
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

            // helpers
            Stack<string> styleStack = new Stack<string>();
            Dictionary<string, int> styleActive = new Dictionary<string, int>();
            HashSet<string> styleBreaks = new HashSet<string>();
            int characterIndex = -1;

            // run through the process of recording which vertices fall between tags
            textInput.ForceMeshUpdate(false, false);
            foreach (var character in textInput.textInfo.characterInfo) {
                characterIndex++;
                if (!character.isVisible) {
                    continue;
                }

                // update values for every tag that appeared before the current character but after the last
                styleBreaks.Clear();
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
                            if (active == 1) {
                                styleBreaks.Add(styleName);
                            }
                            styleActive[styleName] = math.max(active - 1, 0);
                        }
                    } else {
                        break;
                    }
                }

                // handling the individual style cases depending on if they are active or not
                if (styleActive.TryGetValue("shake", out int shakeValue) && shakeValue > 0) {
                    AddVertices(shakeVertices, character.vertexIndex);
                }
                if (styleBreaks.Contains("shake")) {
                    shakeBreaks.Add(character.vertexIndex - 1);
                }

                if (styleActive.TryGetValue("pause", out int pauseValue) && pauseValue > 0) {
                    pauseCharacters[characterIndex + 1] = pauseValue;
                }
            }

            maxVisibleCharacters = characterIndex + 1;
            parsed = true;
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
