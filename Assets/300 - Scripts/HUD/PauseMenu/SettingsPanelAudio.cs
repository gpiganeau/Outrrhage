using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPanelAudio : SettingsPanelBase
{
    public SettingsPanelAudio(VisualElement root, SettingsManager manager)
        : base(root, manager) { }

    protected override void Populate()
    {
        var s = Manager.AudioSettings;
        if (s == null) return;

        SetSlider("aud-master",  s.masterVolume);
        SetSlider("aud-music",   s.musicVolume);
        SetSlider("aud-sfx",     s.sfxVolume);
        SetSlider("aud-ambient", s.ambientVolume);
        SetToggle("aud-mute-unfocused", s.muteOnFocusLoss);
    }

    public override void Apply()
    {
        var s = Manager.AudioSettings;
        if (s == null) return;

        s.masterVolume   = GetSlider("aud-master");
        s.musicVolume    = GetSlider("aud-music");
        s.sfxVolume      = GetSlider("aud-sfx");
        s.ambientVolume  = GetSlider("aud-ambient");
        s.muteOnFocusLoss = GetToggle("aud-mute-unfocused");

        AudioListener.volume = s.masterVolume;
    }

    public override void Reset() => Populate();
}
