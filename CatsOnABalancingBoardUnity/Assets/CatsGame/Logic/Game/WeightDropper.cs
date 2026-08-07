using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OMC {
    public class WeightDropper : MonoBehaviour {
        public static WeightDropper instance;

        public GameObject shadowPrefab;

        public Transform board;

        public float moveSpeed = 1f;

        public float sprintModifier = 2f;

        public float surfaceOffset = 0.01f;

        public float dropHeight = 5f;

        public float shadowBoundRadius = 5f;

        public float timeBetween = 0.25f;

        public float spinSpeed = 180f; // degrees/sec

        public float shadowScale = 0.5f;

        private float lastSpawned;

        private WeightDef prevPicked;

        public GameObject nextPrefab { get; private set; }

        public List<WeightDef> currentWeightRotation = new();

        public static event System.Action FirstWeightDropped;

        bool firstWeightDropped = false;

        float spinAngle;

        GameObject shadow;

        public event System.Action<GameObject> OnNextPrefab;

        public static Dictionary<GameObject, WeightBehaviour> weightBehaviourDict = new();

        private void Awake() {
            if (currentWeightRotation.NotUnityNull().ToArray().Length == 0) {
                Debug.LogError("No WeightDefs found in the WeightDropper rotation!");
            }
            instance = this;
        }

        private void Start() {
            shadow = Instantiate(shadowPrefab, board.position + board.up * surfaceOffset, board.rotation);
            shadow.transform.localScale = Vector3.one * shadowScale;
            shadow.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            shadow.transform.SetParent(board);

            PickNext();
            spinAngle = 0f;
            lastSpawned = Time.time;
        }

        private void Update() {
            Vector2 input = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (input.sqrMagnitude > 0f) {
                if (Keyboard.current.shiftKey.isPressed) {
                    shadow.transform.localPosition += moveSpeed * sprintModifier * Time.deltaTime * new Vector3(input.x, 0f, input.y).normalized;
                } else {
                    shadow.transform.localPosition += moveSpeed * Time.deltaTime * new Vector3(input.x, 0f, input.y).normalized;
                }
            }

            // clamp to board radius
            Vector3 pos = shadow.transform.localPosition;
            Vector2 posXZ = new Vector2(pos.x, pos.z);
            posXZ = Vector2.ClampMagnitude(posXZ, shadowBoundRadius);
            pos.x = posXZ.x;
            pos.z = posXZ.y;
            shadow.transform.localPosition = pos;

            spinAngle += spinSpeed * Time.deltaTime;
            shadow.transform.localRotation = Quaternion.Euler(90f, spinAngle, 0f);

            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                if (Time.time - lastSpawned > timeBetween) {
                    if (nextPrefab) {
                        GameObject weight = Instantiate(nextPrefab, shadow.transform.position + Vector3.up * dropHeight, Quaternion.identity);
                        weightBehaviourDict[weight] = weight.GetComponent<WeightBehaviour>();
                    }
                    PickNext();
                    lastSpawned = Time.time;
                    if (!firstWeightDropped) {
                        firstWeightDropped = true;
                        FirstWeightDropped.Invoke();
                    }
                }
            }
        }

        void PickNext() {
            WeightDef picked = WeightTypeRegistry.GetRandomWeight(currentWeightRotation);
            if (WeightTypeRegistry.weightDefs.Length > 1) {
                while (picked == prevPicked) {
                    picked = WeightTypeRegistry.GetRandomWeight(currentWeightRotation);
                }
            }
            prevPicked = picked;
            nextPrefab = picked.GetRandomShapePrefab();
            OnNextPrefab?.Invoke(nextPrefab);
        }
    }
}
