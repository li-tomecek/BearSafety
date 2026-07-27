using UnityEngine;

public class AudioSceneDefaultMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;


    private void Start()
    {
        if (musicClip != null)
            AudioManager.Instance?.PlayMusic(musicClip);
        else
            Debug.LogError("Issue with AudioManager while trying to play AudioSceneDefaultMusic");
    }
}
