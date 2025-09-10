using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource music;

    public void ToggleMusic()
    {
        if (music == null)
        {
            Debug.LogWarning("AudioSource nav");
            return;
        }

        if (music.isPlaying)
        {
            music.Pause();
        }
        else
        {
            music.UnPause();
        }
    }
}
