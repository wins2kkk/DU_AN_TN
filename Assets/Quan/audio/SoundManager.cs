using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Settings")]
    public AudioMixer audioMixer; // Tham chiếu đến Audio Mixer
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetSFXVolume(sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); // Đổi sang dB
    }

    public void PlaySFX(AudioClip clip)
    {
        GameObject sfxObject = new GameObject("SFX_Sound");
        AudioSource source = sfxObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
        Destroy(sfxObject, clip.length); // Xóa sau khi phát xong
    }
}
