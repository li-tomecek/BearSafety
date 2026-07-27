
using UnityEngine;
using UnityEngine.Audio;

public class SpatialAudioPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    public bool IsPlaying => _audioSource.isPlaying;


    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 1f;
    }

    public void Setup(AudioMixerGroup mixerGroup) { _audioSource.outputAudioMixerGroup = mixerGroup; }


    public void PlayAtLocation(AudioClip clip, Vector3 position, bool isLooping = false)
    {
        if (clip == null) return;

        PlaySound(clip, 1f, isLooping, position);
    }


    private void PlaySound(AudioClip clip, float volume, bool isLooping, Vector3 position)
    {
        position.z = 0f;
        transform.position = position;

        if (isLooping)
        {
            _audioSource.loop = true;
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.Play();
        }
        else
        {
            _audioSource.loop = false;
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}
