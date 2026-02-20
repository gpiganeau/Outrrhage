using UnityEngine.UIElements;

public class SettingsPanelCamera : SettingsPanelBase
{
    public SettingsPanelCamera(VisualElement root, SettingsManager manager)
        : base(root, manager) { }

    protected override void Populate()
    {
        var s = Manager.CameraSettings;
        if (s == null) return;

        SetSlider("cam-fov",         s.cameraFOV);
        /*
        SetSlider("cam-sensitivity", s.mouseSensitivity);
        SetToggle("cam-invert-y",    s.invertY);
        SetSlider("cam-smoothing",   s.cameraSmoothing);
        SetSlider("cam-shake",       s.shakeIntensity);
        */
    }

    public override void Apply()
    {
        var s = Manager.CameraSettings;
        if (s == null) return;

        s.cameraFOV      = GetSlider("cam-fov");
        /*
        s.mouseSensitivity = GetSlider("cam-sensitivity");
        s.invertY          = GetToggle("cam-invert-y");
        s.cameraSmoothing  = GetSlider("cam-smoothing");
        s.shakeIntensity   = GetSlider("cam-shake");
        */
    }

    public override void Reset() => Populate();
}
