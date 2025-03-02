using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Audio/Audio Settings")]
public class AudioSettingsData : ScriptableObject
{
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;
}
