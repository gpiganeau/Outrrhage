using UnityEngine;

public enum SoundGroup { Ambient, SFX, Music, Bark, Global }
public class AudioManager : MonoBehaviour
{


    public static AudioManager Instance;

    private AudioSettings settings;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        settings = SettingsManager.Instance.AudioSettings;
    }

    public void PlayClipAtPoint(AudioClip clip, Vector3 pos, SoundGroup group = SoundGroup.Global)
    {
        AudioSource.PlayClipAtPoint(clip, pos, GetVolume(group));
    }

    private float GetVolume(SoundGroup group)
    {
        return group switch
        {
            SoundGroup.Ambient => settings.ambientVolume,
            SoundGroup.SFX => settings.sfxVolume,
            SoundGroup.Music => settings.musicVolume,
            SoundGroup.Bark => throw new System.NotImplementedException(),
            SoundGroup.Global => settings.masterVolume,
            _ => settings.masterVolume 
        };
    }
    
}