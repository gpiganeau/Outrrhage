using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPanelVisual : SettingsPanelBase
{
    public SettingsPanelVisual(VisualElement root, SettingsManager manager)
        : base(root, manager) { }

    protected override void Populate()
    {
        var s = Manager.VisualSettings;
        if (s == null) return;

        // Init dropdown choices (safe to do multiple times)
        InitDropdown("vis-quality",    new List<string> { "Low", "Medium", "High", "Ultra" });
        InitDropdown("vis-aa",         new List<string> { "None", "FXAA", "TAA", "MSAA x4" });
        InitDropdown("vis-shadow",     new List<string> { "Off", "Low", "Medium", "High" });
        InitDropdown("vis-resolution", new List<string> { "1280x720", "1920x1080", "2560x1440", "3840x2160" });

        /*

        SetDropdown("vis-quality",    s.qualityPreset);
        SetDropdown("vis-aa",         s.antiAliasing);
        SetDropdown("vis-shadow",     s.shadowQuality);
        SetDropdown("vis-resolution", s.resolution);

        SetSlider("vis-brightness", s.brightness);
        SetSlider("vis-gamma",      s.gamma);
        SetToggle("vis-vsync",      s.vSync);
        SetToggle("vis-motionblur", s.motionBlur);

        */
    }

    public override void Apply()
    {
        var s = Manager.VisualSettings;
        
        if (s == null) return;

/*
        s.qualityPreset = GetDropdown("vis-quality");
        s.antiAliasing  = GetDropdown("vis-aa");
        s.shadowQuality = GetDropdown("vis-shadow");
        s.resolution    = GetDropdown("vis-resolution");
        s.brightness    = GetSlider("vis-brightness");
        s.gamma         = GetSlider("vis-gamma");
        s.vSync         = GetToggle("vis-vsync");
        s.motionBlur    = GetToggle("vis-motionblur");

        // Apply quality level immediately
        int qualityIndex = new List<string> { "Low", "Medium", "High", "Ultra" }
                           .IndexOf(s.qualityPreset);
        if (qualityIndex >= 0)
            QualitySettings.SetQualityLevel(qualityIndex, true);

      //  QualitySettings.vSyncCount = s.vSync ? 1 : 0;

      */
    }

    public override void Reset() => Populate();

    // ── Helper ───────────────────────────────────────────────────────────────

    private void InitDropdown(string name, List<string> choices)
    {
        var d = Q<DropdownField>(name);
        if (d != null && (d.choices == null || d.choices.Count == 0))
            d.choices = choices;
    }
}
