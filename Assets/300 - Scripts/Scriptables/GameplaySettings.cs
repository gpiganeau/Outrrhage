using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Scriptable Objects/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    public float baseStaticTimeOnSkillUse;
    public float baseMinTimeBetweenSkills;
}
