using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Scriptable Objects/Settings/AudioSettings")]
public class AudioSettings : ScriptableObject
{
    public bool DisableAudio = false;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float ambientVolume;
    public bool muteOnFocusLoss = true;

}
