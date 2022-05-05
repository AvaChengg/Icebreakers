using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Fade Curve")]
    [SerializeField] private AnimationCurve _fadeMusicCurve;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup _masterMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _ambientLoopMixerGroup;
    [SerializeField] private AudioMixerGroup _ambientOneShotMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    private AudioSource _musicAudioSource;
    private AudioSource _sfxAudioSource;
    private AudioSource _ambientLoopAudioSource;
    private AudioSource _ambientOneShotAudioSource;

    private Coroutine _playMusicCo;

    protected override void OnAwake()
    {
        base.OnAwake();

        if (_musicAudioSource == null)
        {
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
            if (_musicMixerGroup != null)
                _musicAudioSource.outputAudioMixerGroup = _musicMixerGroup;
        }
        if (_ambientLoopAudioSource == null)
        {
            _ambientLoopAudioSource = gameObject.AddComponent<AudioSource>();
            if (_ambientLoopMixerGroup != null)
                _ambientLoopAudioSource.outputAudioMixerGroup = _ambientLoopMixerGroup;
        }

        if (_ambientOneShotAudioSource == null)
        {
            _ambientOneShotAudioSource = gameObject.AddComponent<AudioSource>();
            if (_ambientOneShotMixerGroup != null)
                _ambientOneShotAudioSource.outputAudioMixerGroup = _ambientOneShotMixerGroup;
        }

        if (_sfxAudioSource == null)
        {
            _sfxAudioSource = gameObject.AddComponent<AudioSource>();
            if (_sfxMixerGroup != null)
                _sfxAudioSource.outputAudioMixerGroup = _sfxMixerGroup;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1f, bool fadeOutCurrentMusic = true, float startVolume = 0, float endVolume = 1, float pitch = 1, bool isLoop = true)
    {
        PlayMusicWrapper(clip, fadeDuration, fadeOutCurrentMusic, startVolume, endVolume, pitch, isLoop);
    }

    private void PlayMusicWrapper(AudioClip clip, float fadeDuration = 1f, bool fadeOutCurrentMusic = true, float startVolume = 0, float endVolume = 1, float pitch = 1, bool isLoop = true)
    {
        if (_musicAudioSource == null || clip == null)
        {
            return;
        }

        PlayMusicCoSafetyWrapper();
        _playMusicCo = StartCoroutine(PlayMusicCo(fadeDuration, startVolume, endVolume, fadeOutCurrentMusic, delegate
        {
            _musicAudioSource.loop = isLoop;
            _musicAudioSource.clip = clip;
            _musicAudioSource.volume = startVolume;
            _musicAudioSource.pitch = pitch;
        }));
    }

    private IEnumerator PlayMusicCo(float duration, float startVolume, float endVolume, bool fadeOutCurrentMusic, Action onComplete)
    {
        if (_musicAudioSource.isPlaying == true && fadeOutCurrentMusic == true)
        {
            yield return StopMusicCo(duration);
        }

        onComplete?.Invoke(); //Sets up new clips after stopping previous
        _musicAudioSource?.Play();

        yield return FadeMusic(startVolume, endVolume, duration);
    }

    private IEnumerator FadeMusic(float startingVolume, float endVolume, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float normalizedValue = time / duration;
            float lerpVolume = Mathf.Lerp(startingVolume, endVolume, normalizedValue);
            _musicAudioSource.volume = _fadeMusicCurve.Evaluate(lerpVolume);
            time += Time.deltaTime;
            yield return null;
        }

        _musicAudioSource.volume = endVolume;
    }

    public void StopMusic(float fadeDuration = 0.5f)    {
        PlayMusicCoSafetyWrapper();
        _playMusicCo = StartCoroutine(StopMusicCo(fadeDuration));
    }

    private IEnumerator StopMusicCo(float fadeDuration)
    {
        yield return FadeMusic(_musicAudioSource.volume, 0, fadeDuration);
        _musicAudioSource?.Stop();
    }

    private void PlayMusicCoSafetyWrapper()
    {
        if (_playMusicCo != null)
        {
            StopCoroutine(_playMusicCo);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1)
    {
        if (_sfxAudioSource == null || clip == null)
        {
            return;
        }

        _sfxAudioSource?.PlayOneShot(clip, volume);
    }

    public void PlayAmbientLoop(AudioClip clip, float volume = 1)
    {
        if (_ambientLoopAudioSource == null || clip == null)
        {
            return;
        }

        _ambientLoopAudioSource?.PlayOneShot(clip, volume);
    }

    public void PlayAmbientOneShot(AudioClip clip, float volume = 1)
    {
        if (_ambientOneShotAudioSource == null || clip == null)
        {
            return;
        }

        _ambientOneShotAudioSource?.PlayOneShot(clip, volume);
    }
}
  
