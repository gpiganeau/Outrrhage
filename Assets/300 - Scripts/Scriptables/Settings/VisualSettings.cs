using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Settings", menuName = "Scriptable Objects/Settings/Visuals Settings")]
public class VisualSettings : ScriptableObject
{
    public bool EnableJuicer = true;

    [ColorUsage(true, true)] public Color PlayerHealColor; 
    [ColorUsage(true, true)] public Color PlayerDamagedColor; 
    [ColorUsage(true, true)] public Color PlayerRageColor; 
    [ColorUsage(true, true)] public Color PlayerDeathColor; 


    [SerializeField] ColorPalette _bloodPalette, _techPalette, _naturePalette, _cityPalette;
    public ColorPalette BloodPalette => _bloodPalette;
    public ColorPalette TechPalette => _techPalette;
    public ColorPalette NaturePalette => _cityPalette;
    public ColorPalette CityPalette => _cityPalette;
}

[System.Serializable]
public class ColorPalette
{
    [SerializeField] string Name;
    [SerializeField] Texture2D Ramp;
    [SerializeField][ColorUsage(true, true)] Color Main, Secondary, Ternary;
};
