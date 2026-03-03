using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Scriptable Objects/Settings/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    public float baseStaticTimeOnSkillUse;
    public float baseMinTimeBetweenSkills;

    public float spawnerTimeBetweenSpawns = 0.5f;
    public float BloodDispersionRadius  = 3f;

    [Header("Rage")]
    public int LossRageAmount = 1;
    public float LossRageTick = 0.5f;

    [Header("Game Over")]
    public float DeathTimeBeforeReload = 3f;
    public bool ClearRoomOnDeath = true;

    public float YSpawnOffset = 1f;
}


