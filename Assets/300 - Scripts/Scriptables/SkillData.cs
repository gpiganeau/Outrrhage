using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSkillData", menuName = "Scriptable Objects/Game/SkillData")]
public class SkillData : ScriptableObject
{
    public GameObject SkillStrategyPrefab;
    public GameObject SkillProjectilePrefab;
    public float Cooldown;
    public float ProjectileSpeed;
    public int ProjectileDamage;
    [Space]
    public float movementDistance;
    public float movementDuration;
}
