using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class ExplosionEffect : MonoBehaviour {
    public static ExplosionEffect instance;

    public Texture2D explosionTexture;

    Sprite explosionSprite;

    public AudioClip explosionSound;

    public Texture2D supernovaTexture;

    Sprite supernovaSprite;

    public AudioClip supernovaSound;

    public float lifetime = 1;

    public float spriteScale = 1;

    public float volume = 0.3f;

    public float minPitch = 0.9f;

    public float maxPitch = 1.1f;

    public int maxConcurrentSounds = 1000;
    AudioSource[] sourcePool;
    int nextSourceIndex;

    Camera mainCamera;

    void Awake() {
        instance = this;
        explosionSprite = Sprite.Create(explosionTexture, new Rect(0, 0, explosionTexture.width, explosionTexture.height), new Vector2(0.5f, 0.5f));
        supernovaSprite = Sprite.Create(supernovaTexture, new Rect(0, 0, supernovaTexture.width, supernovaTexture.height), new Vector2(0.5f, 0.5f));
        mainCamera = Camera.main;

        sourcePool = new AudioSource[maxConcurrentSounds];
        for (int i = 0; i < maxConcurrentSounds; i++)
        {
            GameObject a = new($"ExplosionSource_{i}");
            a.transform.SetParent(transform);
            AudioSource b = a.AddComponent<AudioSource>();
            b.spatialBlend = 1;
            b.playOnAwake = false;
            sourcePool[i]=b;
        }
    }

    AudioSource GetNextSource()
    {
        AudioSource b = sourcePool[nextSourceIndex];
        nextSourceIndex = (nextSourceIndex + 1) % maxConcurrentSounds;
        return b;
    }

    void PlaySoundAt(AudioClip clip, Vector3 pos)
    {
        if (clip == null){return;}

        AudioSource b = GetNextSource();
        b.Stop();
        b.transform.position = pos;
        b.clip = clip;
        b.volume = volume;
        b.pitch = Random.Range(minPitch, maxPitch);
        b.Play();
    }

    public void PlayAt(Vector3 position) {
        GameObject hi = new GameObject("ExplosionVFX");
        hi.transform.position = position;

        PlaySoundAt(explosionSound, position);

        if (explosionTexture != null) {
            if (Camera.main != null) {
                hi.transform.rotation = Camera.main.transform.rotation;
            }

            SpriteRenderer s = hi.AddComponent<SpriteRenderer>();
            s.sprite = explosionSprite;
            hi.transform.localScale = Vector3.one * spriteScale;
        }

        Destroy(hi, lifetime);
    }

    public void SuperNovaAt(Vector3 position) {
        GameObject hi = new GameObject("SupernovaVFX");
        hi.transform.position = position;

        PlaySoundAt(supernovaSound, position);

        if (supernovaTexture != null) {
            if (Camera.main != null) {
                hi.transform.rotation = mainCamera.transform.rotation;
            }

            SpriteRenderer s = hi.AddComponent<SpriteRenderer>();
            s.sprite = supernovaSprite;
            hi.transform.localScale = Vector3.one * spriteScale;
        }

        Destroy(hi, lifetime);
    }
}
