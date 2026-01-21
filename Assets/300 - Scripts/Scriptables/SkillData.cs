using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSkillData", menuName = "Scriptable Objects/Game/SkillData")]
public class SkillData : ScriptableObject
{
    public GameObject SkillStrategyPrefab;
    public GameObject SkillProjectilePrefab;

    public string Name;
    public float Cooldown;
    public float ProjectileSpeed;
    public int ProjectileDamage;
    public int numberOfProjectiles;
    [Space]
    public bool ignoreCollisions;
    public float movementDistance;
    public float movementDuration;

    [Header ("Visuals")]
    public Sprite Icon;
}
