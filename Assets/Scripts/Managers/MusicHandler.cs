using UnityEngine;

/// <summary>
/// Handles the background music for the game.
/// TODO(#21) "[Refactor] Audio system based on FMOD"
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicHandler : Singleton<MusicHandler>
{
    private AudioSource musicAudioSource;
    
    private void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();
    }
    public void SetMusic(AudioClip clip)
    {
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }
}
