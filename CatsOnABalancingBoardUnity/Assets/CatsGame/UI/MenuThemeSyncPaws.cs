using UnityEngine;

public class MenuThemeSyncPaws : MonoBehaviour
{
    public AudioSource song;
    public float[] meowTimestamps;

    int next;

    void Start()
    {
        
    }

    void Update()
    {
        if (song == null || !song.isPlaying){return;}

        if (song.time >= meowTimestamps[next])
        {
            MainMenuController.instance.PlaceRandomPaw();
            next = (next + 1) % meowTimestamps.Length;
        }
    }
}
