using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Settings", menuName = "Scriptable Objects/Settings/Visuals Settings")]
public class VisualSettings : ScriptableObject
{
    public bool EnableJuicer = true;

    [Header("Feedback")]
    [ColorUsage(true, true)] public Color PlayerHealColor; 
    [ColorUsage(true, true)] public Color PlayerDamagedColor; 
    [ColorUsage(true, true)] public Color PlayerRageColor; 
    [ColorUsage(true, true)] public Color PlayerDeathColor; 

    [Header("Rage")]
    [Range(0.1f, 1f)] public float RageVignetteIntensity = 0.6f;
    [Range(-1, 1)] public float RageLensDistortionIntensity = - 0.3f;
    public float RageChromaIntensity = .75f;
    public bool RageUseColorAdjustement = false;

    [Header("Subtitles")]
    public ColorPalette RielSubtitleColorPalette;

    [SerializeField] ColorPalette _bloodPalette, _techPalette, _naturePalette, _cityPalette;
    public ColorPalette BloodPalette => _bloodPalette;
    public ColorPalette TechPalette => _techPalette;
    public ColorPalette NaturePalette => _cityPalette;
    public ColorPalette CityPalette => _cityPalette;
}

[System.Serializable]
public class ColorPalette
{
    [SerializeField] public string Name;
    [SerializeField] public Texture2D Ramp;
    [SerializeField][ColorUsage(true, true)] public Color Main, Secondary, Ternary;
};
