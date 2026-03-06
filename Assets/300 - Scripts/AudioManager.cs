using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundGroup { Ambient, SFX, Music, Bark, Global }
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSettings settings;

    [SerializeField] public List<AudioClip> Songs;
    [SerializeField] public AudioClip UISelectClip;
    [SerializeField] public AudioClip UIValidateClip;
    [SerializeField] public AudioClip UIConfirmClip;
    public AudioSource musicSource;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        settings = SettingsManager.Instance.AudioSettings;
        musicSource = GetComponent<AudioSource>();
    }

    public void PlayRandomMusic()
    {
        if (musicSource == null) musicSource = GetComponent<AudioSource>();     
        musicSource.Stop();   
        musicSource.clip = Songs.Random();
        musicSource.volume = settings.musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayClipAtPoint(AudioClip clip, Vector3 pos, SoundGroup group = SoundGroup.Global)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, pos, GetVolume(group));
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

    internal void PlayUiSelect()
    {
        PlayClipAtPoint(UISelectClip, Camera.main.transform.position, SoundGroup.SFX);
    }

    internal void PlayUIValidate()
    {
        PlayClipAtPoint(UIValidateClip, Camera.main.transform.position, SoundGroup.SFX);
    }

    internal void PlayUICOnfirm()
    {
        PlayClipAtPoint(UIConfirmClip, Camera.main.transform.position, SoundGroup.SFX);
    }
}