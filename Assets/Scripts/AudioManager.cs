using UnityEngine;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip backgroundMusic;
    public AudioClip spinSound;
    public AudioClip skipSound;
    public AudioClip claimSound;
    public AudioClip rewardSound;
    public AudioClip tickSound;

    private Dictionary<string, AudioClip> audioClips;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        audioClips = new Dictionary<string, AudioClip>
        {
            { "Spin", spinSound },
            { "Skip", skipSound },
            { "Claim", claimSound },
            { "Reward", rewardSound },
            { "Tick", tickSound }
        };
    }

    public void PlayMusic()
    {
        if (musicSource.clip != backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(string soundName)
    {
        if (audioClips.TryGetValue(soundName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}