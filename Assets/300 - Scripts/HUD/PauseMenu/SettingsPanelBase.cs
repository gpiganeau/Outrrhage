using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Base class for all settings sub-panels.
/// Each panel reads values from its ScriptableObject,
/// populates the UI, and can Apply or Reset changes.
/// </summary>
public abstract class SettingsPanelBase
{
    protected VisualElement Root { get; }
    protected SettingsManager Manager { get; }

    protected SettingsPanelBase(VisualElement root, SettingsManager manager)
    {
        Root    = root;
        Manager = manager;
        Populate();
    }

    /// <summary>Called once on construction: bind UI elements to current values.</summary>
    protected abstract void Populate();

    /// <summary>Write UI values back to the ScriptableObject.</summary>
    public abstract void Apply();

    /// <summary>Revert UI elements to the ScriptableObject's current values.</summary>
    public abstract void Reset();

    // ── Helpers ──────────────────────────────────────────────────────────────

    protected T Q<T>(string name) where T : VisualElement
        => Root.Q<T>(name);

    protected void SetSlider(string name, float value)
    {
        var s = Q<Slider>(name);
        if (s != null) s.value = value;
    }

    protected float GetSlider(string name)
    {
        var s = Q<Slider>(name);
        return s?.value ?? 0f;
    }

    protected void SetToggle(string name, bool value)
    {
        var t = Q<Toggle>(name);
        if (t != null) t.value = value;
    }

    protected bool GetToggle(string name)
    {
        var t = Q<Toggle>(name);
        return t?.value ?? false;
    }

    protected void SetDropdown(string name, string value)
    {
        var d = Q<DropdownField>(name);
        if (d != null) d.value = value;
    }

    protected string GetDropdown(string name)
    {
        var d = Q<DropdownField>(name);
        return d?.value ?? string.Empty;
    }
}
