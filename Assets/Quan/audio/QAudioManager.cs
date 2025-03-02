using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class QAudioManager : MonoBehaviour
{
    public static QAudioManager Instance;
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    private List<AudioSource> allAudioSources = new List<AudioSource>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
       FindAllAudioSources();
        InitializeVolume(); // 🔹 Lấy âm lượng từ PlayerPrefs


    }

    // 🔎 Tìm tất cả các AudioSource trong game
    void FindAllAudioSources()
    {
        allAudioSources.Clear();
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (var source in sources)
        {
            allAudioSources.Add(source);
        }
    }

    public void SetVolume(string group, float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer chưa được gán vào QAudioManager!");
            return;
        }
        float dB = (volume > 0.01f) ? Mathf.Log10(volume) * 20 : -80f; // Nếu = 0 thì đặt -80 dB để tắt âm
        audioMixer.SetFloat(group, dB);
        PlayerPrefs.SetFloat(group, volume);
    }


    // 🔄 Làm mới danh sách nếu có âm thanh mới xuất hiện
    public void RefreshAudioSources()
    {
        FindAllAudioSources();
    }

    // 🛠️ Khởi tạo âm lượng từ PlayerPrefs
    void InitializeVolume()
    {
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);
            SetVolume("music", musicSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
            SetVolume("SFX", sfxSlider.value);
        }

        if (uiSlider != null)
        {
            uiSlider.value = PlayerPrefs.GetFloat("UI", 1f);
            SetVolume("UI", uiSlider.value);
        }
    }


    // 🎚️ Cập nhật âm lượng từ Slider
    public void SetVolumeFromSlider()
    {
        SetVolume("music", musicSlider.value);
        SetVolume("SFX", sfxSlider.value);
        SetVolume("UI", uiSlider.value);
    }

    public void PlaySound(AudioClip clip, float volume, string mixerGroup)
    {
        if (clip == null || volume <= 0)
        {
            Debug.Log("❌ Không phát âm thanh: " + (clip ? clip.name : "NULL") + " - Volume: " + volume);
            return;
        }

        Debug.Log("🔊 Phát âm thanh: " + clip.name + " - Volume: " + volume);

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = volume;

        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(mixerGroup);
            if (groups.Length > 0)
            {
                audioSource.outputAudioMixerGroup = groups[0];
            }
        }

        audioSource.Play();
        Destroy(tempGO, clip.length);
    }
    



    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}