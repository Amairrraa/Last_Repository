using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;
    public AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        audioSource.Play();
    }
}
