using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Scriptable Objects/Settings/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    [Header("Skills & Combat")]
    public float baseStaticTimeOnSkillUse;
    public float baseMinTimeBetweenSkills;
    public float InvicibleTime = 0.1f;

    [Header("Blood")]
    public float BloodDispersionRadius  = 3f;

    [Header("Rage")]
    public int LossRageAmount = 1;
    public float LossRageTick = 0.5f;

    [Header("Game Over")]
    public float DeathTimeBeforeReload = 3f;
    public bool ClearRoomOnDeath = true;
    
    [Header("Spawning")]
    public float spawnerTimeBetweenSpawns = 0.5f;
    public float YSpawnOffset = 1f;

    [Header("Pre run UI For Skills")]
    public bool OpenSkillSelectorOnRunStart = true;

    [Header("Debug")]
    public bool HideRielDebugElements;
}


