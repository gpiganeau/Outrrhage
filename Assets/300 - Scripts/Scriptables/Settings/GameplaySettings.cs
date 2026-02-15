using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Scriptable Objects/Settings/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    public float baseStaticTimeOnSkillUse;
    public float baseMinTimeBetweenSkills;

    public float spawnerTimeBetweenSpawns = 0.5f;
}
