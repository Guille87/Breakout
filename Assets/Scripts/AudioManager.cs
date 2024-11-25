using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (instance != null && clip != null)
        {
            instance.audioSource.PlayOneShot(clip, volume);
        }
    }
}
