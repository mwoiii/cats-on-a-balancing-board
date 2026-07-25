using UnityEngine;
using UnityEngine.Rendering;

public class ExplosionEffect : MonoBehaviour
{
    public static ExplosionEffect Instance;

    public Texture2D explosionTexture;
    Sprite explosionSprite;
    public AudioClip explosionSound;

    public float lifetime = 1;
    public float spriteScale = 1;
    public float volume = 0.3f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    void Awake()
    {
        Instance = this;
        explosionSprite = Sprite.Create(explosionTexture, new Rect(0,0,explosionTexture.width,explosionTexture.height), new Vector2(0.5f,0.5f));
    }

    public void PlayAt(Vector3 position)
    {
        GameObject hi = new GameObject("ExplosionSFXandVFX");
        hi.transform.position = position;
        
        if (explosionSound != null)
        {
            AudioSource a = hi.AddComponent<AudioSource>();
            a.clip = explosionSound;
            a.volume = volume;
            a.pitch = Random.Range(minPitch,maxPitch);
            a.spatialBlend = 1;
            a.Play();
        }

        if (explosionTexture != null)
        {
            if (Camera.main != null)
            {
                hi.transform.rotation = Camera.main.transform.rotation;
            }

            SpriteRenderer s = hi.AddComponent<SpriteRenderer>();
            s.sprite = explosionSprite;
            hi.transform.localScale = Vector3.one * spriteScale;
        }

        Destroy(hi,lifetime);
    }
}
