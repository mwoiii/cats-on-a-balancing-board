using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace OMC {
    public class BoardController : MonoBehaviour {
        public static GameObject boardInstance;

        public float slope { get; private set; }

        public Vector3 slopeDir { get; private set; }

        public float radius {get; private set;}
        Collider collider;

        public Transform relativeCamera;

        private void Awake() {
            boardInstance = gameObject;
            collider = GetComponent<Collider>();
        }

        void FixedUpdate() {
            Vector3 A = transform.up;
            A.y = 0;
            slope = A.magnitude;
            slopeDir = A.normalized;

            radius = collider.bounds.extents.x;
        }

        void Update() // TEMPORARY
        {
            if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                ChangeRadius(10);
            }
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                ChangeRadius(3);
            }
        }

        void ChangeRadius(float r, float duration = 1)
        {
            StartCoroutine(ResizeBoard(r,duration));
        }

        IEnumerator ResizeBoard(float r, float duration)
        {
            float scaleFactor = r/radius;

            Vector3 startScale = transform.localScale;
            Vector3 targetScale = new(startScale.x * scaleFactor, startScale.y, startScale.z * scaleFactor);

            Vector3 startCamPos = relativeCamera.localPosition;
            Vector3 targetCamPos = new(relativeCamera.localPosition.x,relativeCamera.localPosition.y,-(1.8f*r-5.4f));

            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0,1,t/duration));
                relativeCamera.localPosition = Vector3.Lerp(startCamPos,targetCamPos,Mathf.SmoothStep(0,1,t/duration));
                yield return null; // we don't waitforseconds around here
            }

            transform.localScale = targetScale;
        }
    }
}
