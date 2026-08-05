using OMC.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMC {
    public class CatManagerScript : MonoBehaviour {
        public GameLogicScript logic;

        private List<GameObject> cats = new List<GameObject>();

        public static double score;

        private const double scoreScaleFactor = 0.05d;

        public AudioClip meow;

        public float minAmbientMeowInterval = 3;

        public float maxAmbientMeowInterval = 20;

        public float initialCatCount = 100;

        public float volume = 0.3f;

        public float minPitch = 0.5f;

        public float maxPitch = 1.5f;

        public static event System.Action<int> LostCat;

        public void Awake() {
            score = 0d;
        }

        public void RegisterCat(GameObject cat) {
            cats.Add(cat);
            if (HUDController.instance) {
                HUDController.instance.UpdateRemainingCats(1);
            }
            // Debug.Log("Cats count: " + cats.Count);
        }

        public void RemoveCat(GameObject cat) {
            cats.Remove(cat);
            if (HUDController.instance) {
                HUDController.instance.UpdateRemainingCats(-1);
                LostCat?.Invoke(1);
            }
            // Debug.Log("Cats count: " + cats.Count);
            if (cats.Count == 0) {
                logic.GameOver();
            }
        }

        public void ClearAllCats() {
            foreach (GameObject cat in cats) {
                if (cat != null)
                    Destroy(cat);
            }

            cats.Clear();
        }

        public int GetCatCount() {
            return cats.Count;
        }

        void Start() {
            StartCoroutine(AmbientMeow());
        }

        public void FixedUpdate() {
            if (GameLogicScript.gameRunning) {
                score += cats.Count * Time.deltaTime * scoreScaleFactor;
            }
        }

        IEnumerator AmbientMeow() {
            yield return new WaitForSeconds(Random.Range(minAmbientMeowInterval, maxAmbientMeowInterval));
            while (cats.Count > 0) {
                GameObject luckyWinner = cats[Random.Range(0, cats.Count)];
                AudioSource player = luckyWinner.AddComponent<AudioSource>();
                player.clip = meow;
                player.volume = volume;
                player.pitch = Random.Range(minPitch, maxPitch);
                player.spatialBlend = 1;
                player.Play();

                float populationCoeff = initialCatCount / cats.Count;
                yield return new WaitForSeconds(Random.Range(populationCoeff * minAmbientMeowInterval, populationCoeff * maxAmbientMeowInterval));
            }
        }
    }
}
