using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flipClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip mismatchClip;
    [SerializeField] private AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayFlip() => audioSource.PlayOneShot(flipClip);
    public void PlayMatch() => audioSource.PlayOneShot(matchClip);
    public void PlayMismatch() => audioSource.PlayOneShot(mismatchClip);
    public void PlayGameOver() => audioSource.PlayOneShot(gameOverClip);
}