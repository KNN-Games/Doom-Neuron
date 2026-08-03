using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : Singleton<MusicHandler>
{
    private AudioSource musicAudioSource;
    void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();
    }
    public void SetMusic(AudioClip clip)
    {
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }
}
