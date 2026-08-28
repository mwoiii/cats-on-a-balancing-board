using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace OMC.UI.CustomStyles {
    public class CustomStylesController : MonoBehaviour {

        public float writeDelay = 0.05f;

        public float animationDelay = 0.05f;

        [SerializeField]
        private TextMeshProUGUI textInput;

        private int lastVisibleVertex => visibleCharacters * 4 - 1;

        private int visibleCharacters;

        private int maxVisibleCharacters;

        private float writeCooldown;

        private float animateCooldown;

        private bool parsed;

        private Vector3[] visibleVertices;

        private static Regex startRegex = new Regex("<style=\"(\\D*)(\\d)\">");

        private static Regex endRegex = new Regex("</style>");

        private static Dictionary<string, Type> stylePrefixToType = new Dictionary<string, Type>();

        private Dictionary<string, IVertexStyle> vertexStyles = new Dictionary<string, IVertexStyle>();

        private Dictionary<string, IDelayStyle> delayStyles = new Dictionary<string, IDelayStyle>();

        private struct StyleInfo {
            public bool isStart;
            public int index;
            public string styleName;
            public int styleValue;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            var styles = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => typeof(IStyle).IsAssignableFrom(type)
                && !type.IsInterface
                && !type.IsAbstract
            );

            foreach (var style in styles) {
                IStyle styleInstance = (IStyle)Activator.CreateInstance(style);
                if (!stylePrefixToType.ContainsKey(styleInstance.prefix)) {
                    stylePrefixToType[styleInstance.prefix] = styleInstance.GetType();
                    Debug.Log(stylePrefixToType[styleInstance.prefix].ToString());
                } else {
                    Debug.LogError($"Duplicate custom style of name {styleInstance.prefix}! Skipping...");
                }
            }
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
            Dictionary<string, IStyle> styleInstances = new Dictionary<string, IStyle>();
            int characterIndex = -1;

            // run through the process of recording which vertices fall between tags
            textInput.ForceMeshUpdate(false, false);
            foreach (var character in textInput.textInfo.characterInfo) {
                characterIndex++;
                if (!character.isVisible) {
                    continue;
                }

                maxVisibleCharacters = characterIndex + 1;

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

                // generating style instances if not in existence, and proving them with the relevant data
                foreach (string prefix in styleActive.Keys) {
                    int value = styleActive[prefix];
                    if (value <= 0) {
                        continue;
                    }
                    if (stylePrefixToType.ContainsKey(prefix)) {
                        IStyle style;
                        if (!styleInstances.ContainsKey(prefix)) {
                            style = styleInstances[prefix] = (IStyle)Activator.CreateInstance(stylePrefixToType[prefix]);
                            if (style is IVertexStyle v1Style) {
                                vertexStyles[prefix] = v1Style;
                            }
                            if (style is IDelayStyle d1Style) {
                                delayStyles[prefix] = d1Style;
                            }
                        } else {
                            style = styleInstances[prefix];
                        }
                        bool broken = styleBreaks.Contains(prefix);
                        if (style is IVertexStyle v2Style) {
                            v2Style.ReceiveStartVertex(character.vertexIndex, value, broken);
                        }
                        if (style is IDelayStyle d2Style) {
                            d2Style.ReceiveStartCharacter(characterIndex, value, broken);
                        }
                    } else {
                        Debug.Log($"Got custom style \"{prefix}\" but found no respective class!");
                    }
                }
            }
            parsed = true;
        }

        private void UpdateWriteText() {
            if (visibleCharacters < maxVisibleCharacters) {
                writeCooldown -= Time.deltaTime;
                if (writeCooldown <= 0) {
                    while (writeCooldown < 0 && visibleCharacters < maxVisibleCharacters) {
                        visibleCharacters++;
                        textInput.maxVisibleCharacters = visibleCharacters;
                        writeCooldown += writeDelay;
                    }
                    textInput.ForceMeshUpdate(false, false);
                    ApplyVertexStyles();
                    writeCooldown = writeDelay;
                    ApplyDelayStyles(ref writeCooldown);
                }
            }
        }

        private void UpdateAnimateText() {
            animateCooldown -= Time.deltaTime;
            if (animateCooldown <= 0) {
                textInput.ForceMeshUpdate(false, false);
                visibleVertices = textInput.textInfo.meshInfo[0].vertices;
                UpdateAndApplyVertexStyles();
                animateCooldown = animationDelay;
            }

            textInput.textInfo.meshInfo[0].vertices = visibleVertices;
            textInput.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }

        private void ApplyVertexStyles() {
            foreach (IVertexStyle style in vertexStyles.Values) {
                style.ApplyVertices(visibleVertices, lastVisibleVertex);
            }
        }

        private void UpdateAndApplyVertexStyles() {
            foreach (IVertexStyle style in vertexStyles.Values) {
                style.UpdateVertices(lastVisibleVertex);
                style.ApplyVertices(visibleVertices, lastVisibleVertex);
            }
        }

        private void ApplyDelayStyles(ref float cooldown) {
            foreach (IDelayStyle style in delayStyles.Values) {
                style.ApplyDelay(visibleCharacters, ref cooldown);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetScaledValue(int value) {
            return math.max(0, (value * 2) - 1);  // goes up as 1, 3, 5, etc. can combine tags to reach values in between
        }
    }
}
