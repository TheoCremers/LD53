using UnityEngine;
using System.Linq;
using System.Collections;

public static class AudioManager
{   
    public static void PlaySFX(SFXType sfxType)
    {        
        var sfxGameObject = GameObject.Find("GlobalSFX");
        if (sfxGameObject == null)
        {
            sfxGameObject = new GameObject("GlobalSFX");
        }
        PlaySFX(sfxType, sfxGameObject, false);
    }

    public static void PlaySFX(SFXType sfxType, GameObject target, bool positional = true)
    {
        if (sfxType == SFXType.None) { return; }

        var audioSource = target.AddComponent<AudioSource>();
        audioSource.SetupAudioSource(sfxType);
        if (positional) 
        {
            audioSource.WithPositionals();
        }
        audioSource.Play();
        Object.Destroy(audioSource, audioSource.clip.length);
    }

    public static void PlaySFXVariation (SFXType sfxType)
    {        
        var sfxGameObject = GameObject.Find("GlobalSFX");
        if (sfxGameObject == null)
        {
            sfxGameObject = new GameObject("GlobalSFX");
        }
        PlaySFXVariation(sfxType, sfxGameObject, false);
    }

    public static void PlaySFXVariation (SFXType sfxType, GameObject target, bool positional = true)
    {
        if (sfxType == SFXType.None) { return; }

        var audioSource = target.AddComponent<AudioSource>();
        audioSource.SetupAudioSource(sfxType).WithVariation();
        if (positional) 
        {
            audioSource.WithPositionals();
        }
        audioSource.Play();
        Object.Destroy(audioSource, audioSource.clip.length);
    }

    public static void PlayBGM(BGMType bgmType)
    {
        var metadata =  GetBGMMetadata(bgmType);
        var bgmGameObject = GameObject.Find("BGM");
        if (bgmGameObject != null) 
        {
            ChangeBGM(bgmGameObject, metadata);
        }
        else 
        {
            bgmGameObject = new GameObject("BGM");
            // This is a bit ugly ngl
            GameAssets.Instance.Persist(bgmGameObject);
            var audioSource = bgmGameObject.AddComponent<AudioSource>();            
            audioSource.SetupAudioSource(bgmType);
            audioSource.Play();  
        }
    }

    public static void StopBGM() 
    {
        var bgmGameObject = GameObject.Find("BGM");
        if (bgmGameObject != null) 
        {
            var audioSource = bgmGameObject.GetComponent<AudioSource>();   
            if (audioSource != null) 
            {
                audioSource.Stop();
            }
        }
    }

    public static void TogglePauseBGM() 
    {
        var bgmGameObject = GameObject.Find("BGM");
        if (bgmGameObject != null) 
        {
            var audioSource = bgmGameObject.GetComponent<AudioSource>();   
            if (audioSource != null) 
            {
                if (audioSource.isPlaying) 
                {
                    audioSource.Pause();
                } 
                else
                {
                    audioSource.UnPause();
                }
            }
        }
    }

    private static AudioSource WithPositionals(this AudioSource audioSource)
    {
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        return audioSource;
    }

    private static AudioSource WithVariation(this AudioSource audioSource)
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        return audioSource;
    }

    private static AudioSource SetupAudioSource(this AudioSource audioSource, SFXType sfxType)
    {
        // TODO: Implement global volume setting
        var metadata = GetSFXMetadata(sfxType);
        audioSource.clip = metadata.AudioClip;
        audioSource.volume = metadata.BaseVolume;
        return audioSource;
    }

    private static AudioSource SetupAudioSource(this AudioSource audioSource, BGMType bgmType)
    {
        // TODO: Implement global volume setting
        var metadata = GetBGMMetadata(bgmType);
        audioSource.clip = metadata.AudioClip;
        audioSource.volume = metadata.BaseVolume;
        audioSource.loop = true;
        return audioSource;
    }

    private static void ChangeBGM(GameObject bgmGameObject, BGMMetadata bgmMetadata)
    {
        var audioSource = bgmGameObject.GetComponent<AudioSource>();   
        if (audioSource.clip == bgmMetadata.AudioClip) 
        {
            return;
        }
        audioSource.Stop();
        audioSource.clip = bgmMetadata.AudioClip;

        // TODO: Implement global volume setting
        audioSource.volume = bgmMetadata.BaseVolume;
        audioSource.loop = true;
        audioSource.Play();  
    }

    private static SFXMetadata GetSFXMetadata(SFXType sfxType) 
    {
        var sfxMetadata = GameAssets.Instance.SFXMetadata.FirstOrDefault(x => x.SfxType == sfxType);
        if (sfxMetadata != null) 
        {
            return sfxMetadata;
        } 
        else 
        {
            Debug.LogWarning("Missing SFX Metadata " + sfxType);
            return null;
        }
    }

    private static BGMMetadata GetBGMMetadata(BGMType bgmType) 
    {
        var bgmMetadata = GameAssets.Instance.BGMMetadata.FirstOrDefault(x => x.BGMType == bgmType);
        if (bgmMetadata != null) 
        {
            return bgmMetadata;
        } 
        else 
        {
            Debug.LogWarning("Missing BGM Metadata " + bgmType);
            return null;
        }
    }
}
