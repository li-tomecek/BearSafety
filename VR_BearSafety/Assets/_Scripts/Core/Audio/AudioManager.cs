using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : Singleton<AudioManager>
{
    [Header("Mixer & Groups")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;
    public AudioMixerGroup SFXMixer => sfxMixer;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private List<SpatialAudioPlayer> _atLocationSources = new List<SpatialAudioPlayer>();


    protected override void Awake()
    {
        base.Awake();

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.outputAudioMixerGroup = musicMixer;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.outputAudioMixerGroup = sfxMixer;
    }

    private void Start()
    {
        LoadVolume();
    }

    #region Volume
    private void LoadVolume()
    {
        SetMixerVolume(AudioMixerKeys.MasterVolumeKey, PlayerPrefs.GetFloat(AudioMixerKeys.MasterVolumeKey, 1.0f));
        SetMixerVolume(AudioMixerKeys.MusicVolumeKey, PlayerPrefs.GetFloat(AudioMixerKeys.MusicVolumeKey, 1.0f));
        SetMixerVolume(AudioMixerKeys.SFXVolumeKey, PlayerPrefs.GetFloat(AudioMixerKeys.SFXVolumeKey, 1.0f));
    }

    public void SetMixerVolume(string key, float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1.0f);
        audioMixer.SetFloat(key, Mathf.Log10(volume) * 20);
    }
    #endregion

    #region Plays

    #region Music
    public void PlayMusic(AudioClip music, bool isLooped = true)
    {
        if (music == null)
            return;

        PlayMusicInternal(music, isLooped);
    }

    private void PlayMusicInternal(AudioClip clip, bool loop = true, float volume = 1.0f)
    {
        if (clip == null || clip == _musicSource.clip)
            return;

        if (_musicSource.isPlaying)
            _musicSource.Stop();

        _musicSource.loop = loop;
        _musicSource.clip = clip;
        _musicSource.volume = volume;
        _musicSource.Play();
    }
    #endregion

    #region 2D
    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
            return;

        _sfxSource.PlayOneShot(sound);
    }

    public void PlayRandomSound(List<AudioClip> listOfSFX)
    {
        if (listOfSFX == null || listOfSFX.Count == 0)
            return;

        PlaySound(listOfSFX[Random.Range(0, listOfSFX.Count)]);
    }
    #endregion

    #region 3D
    public void PlaySoundAtLocation(AudioClip sound, Vector3 position, bool loop = false)
    {
        if (sound == null)
            return;

        GetAvailableSFXSource(position).PlayAtLocation(sound, position, loop);
    }

    public void PlayRandomSoundAtLocation(List<AudioClip> listOfSFX, Vector3 position, bool loop = false)
    {
        if (listOfSFX == null || listOfSFX.Count == 0)
            return;

        PlaySoundAtLocation(listOfSFX[Random.Range(0, listOfSFX.Count)], position, loop);
    }
    #endregion

    #endregion

    private SpatialAudioPlayer GetAvailableSFXSource(Vector3 pos)
    {
        foreach (SpatialAudioPlayer spatialEffectSource in _atLocationSources)
        {
            if (!spatialEffectSource.IsPlaying)
            {
                return spatialEffectSource;
            }
        }

        GameObject newObj = new GameObject("SpatialAudioPlayer");
        newObj.transform.parent = transform;

        SpatialAudioPlayer newSpatialAudioPlayer = newObj.AddComponent<SpatialAudioPlayer>();
        newSpatialAudioPlayer.Setup(sfxMixer);
        _atLocationSources.Add(newSpatialAudioPlayer);
        return newSpatialAudioPlayer;
    }
}
