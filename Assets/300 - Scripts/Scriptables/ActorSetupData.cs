using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActorSettings", menuName = "Scriptable Objects/Game/ActorSetupData")]
public class ActorSetupData: ScriptableObject
{
    public int maxHealth;
    public float movementSpeed;
    public List<SkillData> startingSkillSet;
}

