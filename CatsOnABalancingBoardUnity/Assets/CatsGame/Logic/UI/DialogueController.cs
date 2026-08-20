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

        private Dictionary<int, int> shakeCharacters = new Dictionary<int, int>(); // start vert -> value

        private Dictionary<int, int> shakeGroupVertices = new Dictionary<int, int>(); // start vert -> value
        private List<int> shakeGroupBreaks = new List<int>();

        private Dictionary<int, int> pauseCharacters = new Dictionary<int, int>(); // character index to stop before -> value

        private static Regex startRegex = new Regex("<style=\"(\\D*)(\\d)\">");
        private static Regex endRegex = new Regex("</style>");

        private struct StyleInfo {
            public bool isStart;
            public int index;
            public string styleName;
            public int styleValue;
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

        private void Update() {
            UpdateWriteText();
            UpdateAnimateText();
        }

        private void ResetDialogue() {
            textInput.maxVisibleCharacters = 0;
            visibleCharacters = 0;
            animateCooldown = 0f;
            writeCooldown = 0f;
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
                    styleValue = math.max(0, GetScaledValue(int.TryParse(match.Groups[2].Value, out int value) ? value : 0)),
                    index = match.Index
                });
            }

            // helpers
            Stack<StyleInfo> styleStack = new Stack<StyleInfo>();
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
                            styleActive[result.styleName] = active + result.styleValue;
                            styleStack.Push(result);
                        } else {
                            if (styleStack.Count <= 0) {
                                continue;
                            }
                            StyleInfo styleToRemove = styleStack.Pop();
                            if (styleActive.TryGetValue(styleToRemove.styleName, out int active)) {
                                styleActive[styleToRemove.styleName] = math.max(0, active - styleToRemove.styleValue);
                                if (active > 0 && styleActive[styleToRemove.styleName] == 0) {
                                    styleBreaks.Add(styleToRemove.styleName);
                                }
                            }
                        }
                    } else {
                        break;
                    }
                }


                // handling the individual style cases depending on if they are active or not

                const string shakeGroupPrefix = "shakegroup";
                if (styleActive.TryGetValue(shakeGroupPrefix, out int shakeGroupValue) && shakeGroupValue > 0) {
                    shakeGroupVertices[character.vertexIndex] = shakeGroupValue;
                }
                if (styleBreaks.Contains(shakeGroupPrefix)) {
                    shakeGroupBreaks.Add(character.vertexIndex - 1);
                }

                const string shakePrefix = "shake";
                if (styleActive.TryGetValue(shakePrefix, out int shakeValue) && shakeValue > 0) {
                    shakeCharacters[character.vertexIndex] = shakeValue;
                }

                const string pausePrefix = "pause";
                if (styleActive.TryGetValue(pausePrefix, out int pauseValue) && pauseValue > 0) {
                    pauseCharacters[characterIndex + 1] = pauseValue;
                }
            }

            maxVisibleCharacters = characterIndex + 1;
            parsed = true;
        }

        private void UpdateWriteText() {
            if (visibleCharacters < maxVisibleCharacters) {
                writeCooldown -= Time.deltaTime;
                if (writeCooldown <= 0) {
                    visibleCharacters++;
                    textInput.maxVisibleCharacters = visibleCharacters;
                    if (pauseCharacters.TryGetValue(visibleCharacters, out int pauseValue)) {
                        writeCooldown = 0.3333f * pauseValue;
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
                int lastVertex = visibleCharacters * 4 - 1;
                ApplyShake(animatedVertices, lastVertex);
                ApplyShakeGroup(animatedVertices, lastVertex);
                animateCooldown = animationDelay;
            }

            textInput.textInfo.meshInfo[0].vertices = animatedVertices;
            textInput.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyShakeGroup(Vector3[] vertices, int lastVertex) {
            int nextBreak = 0;
            Vector3 randomShake = GetRandomShake(shakeGroupVertices.Values.First() * 4f);
            foreach (int vertex in shakeGroupVertices.Keys) {
                if (vertex > lastVertex) {
                    break;
                }
                if (nextBreak < shakeGroupBreaks.Count && vertex > shakeGroupBreaks[nextBreak]) {
                    randomShake = GetRandomShake(shakeGroupVertices[vertex] * 4f);
                    nextBreak++;
                }
                vertices[vertex] += randomShake;
                vertices[vertex + 1] += randomShake;
                vertices[vertex + 2] += randomShake;
                vertices[vertex + 3] += randomShake;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyShake(Vector3[] vertices, int lastVertex) {
            foreach (int vertex in shakeCharacters.Keys) {
                if (vertex >= lastVertex) {
                    break;
                }
                Vector3 randomShake = GetRandomShake(shakeCharacters[vertex] * 4f);
                vertices[vertex] += randomShake;
                vertices[vertex + 1] += randomShake;
                vertices[vertex + 2] += randomShake;
                vertices[vertex + 3] += randomShake;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetScaledValue(int value) {
            return math.max(0, (value * 2) - 1);  // goes up as 1, 3, 5, etc. can combine tags to reach values in between
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetRandomShake(float intensity) {
            return (Vector3.up * UnityEngine.Random.Range(-1f, 1f) + Vector3.right * UnityEngine.Random.Range(-1f, 1f)) * intensity;
        }
    }
}
