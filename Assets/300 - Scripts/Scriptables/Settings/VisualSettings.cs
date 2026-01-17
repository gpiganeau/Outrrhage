using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Settings", menuName = "Scriptable Objects/Settings/Visuals Settings")]
public class VisualSettings : ScriptableObject
{
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
