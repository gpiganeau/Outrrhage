using UnityEditor;
using UnityEngine;

public class ProjectileData
{
    public SkillStrategy attackStrategy;
    public float Speed;
    public int Damage;
    public bool useCustomDamageCalculation = false;
    public Team Team = Team.Neutral;

    public float Lifetime = 0.2f; // -- Default, else we get some bug like instant destructed projectile.
    public Vector3 origin;
    public Vector3 startingPosition;
    public Vector3 Target;
    //Et tout le reste ...

    public ProjectileData()
    {

    }
}
