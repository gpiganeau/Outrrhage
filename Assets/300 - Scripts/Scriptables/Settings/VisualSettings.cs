using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Settings", menuName = "Scriptable Objects/Settings/Visuals Settings")]
public class VisualSettings : ScriptableObject
{
    [SerializeField] ColorPalette _bloodPalette;
    public ColorPalette BloodPalette => _bloodPalette;
}

[System.Serializable]
public class ColorPalette
{
    [SerializeField] string Name;
    [SerializeField] Texture2D Ramp;
    [SerializeField] Color Main, Secondary, Ternary;
};
