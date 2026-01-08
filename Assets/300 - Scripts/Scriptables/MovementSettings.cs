using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Tooltip("Number of Unity units per second")]
    public float characterSpeed;
    [Tooltip("number of seconds in takes to be at max speed in opposite direction"), Range(0, 2)]
    public float timeToReverse;
}
