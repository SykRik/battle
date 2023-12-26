using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    #region ===== Fields =====

    private Dictionary<AudioEnum, AudioClip> audioClips = null;
    private Pool<AudioSource> poolAudioSource = null;

    #endregion

    #region ===== Methods =====

    public override void Init()
    {
        audioClips      = new Dictionary<AudioEnum, AudioClip>();
        poolAudioSource = new Pool<AudioSource>(gameObject, CreateAudioSource);
        foreach (var source in GetComponentsInChildren<AudioSource>(true))
        {
            DestroyImmediate(source.gameObject);
        }
    }

    public void Play(AudioEnum audioType)
    {
        if (audioClips.TryGetValue(audioType, out var audioClip))
        {
            var audioSource = poolAudioSource.Request();
            audioSource.clip = audioClip;
            audioSource.Play();
            StartCoroutine(StopAsync(audioSource));
        }
    }

    public void Stop(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.Stop();
        audioSource.clip    = null;
        audioSource.volume  = 1f;
        audioSource.pitch   = 1f;

        poolAudioSource.Return(audioSource);
    }

    private IEnumerator StopAsync(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);

        Stop(audioSource);
    }

    private AudioSource CreateAudioSource()
    {
        var go      = new GameObject("AudioSource");
        var source  = go.AddComponent<AudioSource>();
        return source;
    }

    #endregion
}

public enum AudioEnum
{
    BUTTON_CLICK,
    HIT,
    ATTACK,
    DEATH,
}