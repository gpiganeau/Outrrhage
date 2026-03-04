using UnityEngine;

[CreateAssetMenu(fileName = "Bark", menuName = "Scriptable Objects/Barks/Bark")]
public class Bark : ScriptableObject
{

    public AudioClip Clip;
    public BarkPriority Priority = BarkPriority.Normal;
    public bool OneShot = true;

    [Header("Subtitles")]
    public bool ShowSubtitles = false;
    public string Text = "Il faut <em>ABSOLUMENT</em> fuir, le <c=#FF4444>danger</c> approche.";
    public Color MainColor = Color.wheat;
    public Color SubtitleBackgroundColor = new Color(0f, 0f, 0f, 0.6f);
}
