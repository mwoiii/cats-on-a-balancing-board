using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using System.Collections;

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

    public int maxConcurrentSounds = 100;
    public int maxConcurrentSprites = 1000;
    AudioSource[] sourcePool;
    SpriteRenderer [] spritePool;
    int nextSourceIndex;
    int nextSpriteIndex;

    Camera mainCamera;

    void Awake() {
        instance = this;
        explosionSprite = Sprite.Create(explosionTexture, new Rect(0, 0, explosionTexture.width, explosionTexture.height), new Vector2(0.5f, 0.5f));
        supernovaSprite = Sprite.Create(supernovaTexture, new Rect(0, 0, supernovaTexture.width, supernovaTexture.height), new Vector2(0.5f, 0.5f));
        mainCamera = Camera.main;

        Transform pool = new GameObject("the pool").transform;

        sourcePool = new AudioSource[maxConcurrentSounds];
        for (int i = 0; i < maxConcurrentSounds; i++)
        {
            GameObject a = new($"ExplosionSource_{i}");
            a.transform.SetParent(pool);
            AudioSource b = a.AddComponent<AudioSource>();
            b.spatialBlend = 1;
            b.playOnAwake = false;
            sourcePool[i]=b;
        }
        
        spritePool = new SpriteRenderer[maxConcurrentSprites];
        for (int i = 0; i < maxConcurrentSprites; i++)
        {
            GameObject a = new($"ExplosionSprite_{i}");
            a.transform.SetParent(pool);
            SpriteRenderer b = a.AddComponent<SpriteRenderer>();
            b.sprite = explosionSprite;
            if (Camera.main != null) {
                b.transform.rotation = Camera.main.transform.rotation;
            }
            b.transform.localScale = Vector3.one * spriteScale;
            b.enabled = false;
            spritePool[i]=b;
        }

    }

    AudioSource GetNextSource()
    {
        AudioSource b = sourcePool[nextSourceIndex];
        nextSourceIndex = (nextSourceIndex + 1) % maxConcurrentSounds;
        return b;
    }

    SpriteRenderer GetNextSprite()
    {
        SpriteRenderer b = spritePool[nextSpriteIndex];
        nextSpriteIndex = (nextSpriteIndex + 1) % maxConcurrentSprites;
        return b;
    }

    void PlaySoundAt(AudioClip clip, Texture texture, Vector3 pos)
    {
        if (clip == null || texture == null){return;}

        AudioSource b = GetNextSource();
        b.Stop();
        b.transform.position = pos;
        b.clip = clip;
        b.volume = volume;
        b.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        b.Play();

        SpriteRenderer c = GetNextSprite();
        c.transform.position = pos;
        c.enabled = true;
    }

    IEnumerator Disable(SpriteRenderer c)
	{
		yield return new WaitForSeconds (lifetime);
        c.enabled = false;
	}

    public void PlayAt(Vector3 position) {
        PlaySoundAt(explosionSound, explosionTexture, position);
    }

    public void SuperNovaAt(Vector3 position) {
        PlaySoundAt(supernovaSound, supernovaTexture, position);
    }
}
