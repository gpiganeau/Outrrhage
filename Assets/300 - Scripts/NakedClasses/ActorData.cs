using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorData
{
    public int maxHealth;
    public int currentHealth;
    public List<SkillData> skillData;
    public float movementSpeed;

    public ActorData(ActorSetupData setupData)
    {
        maxHealth = setupData.maxHealth;
        currentHealth = maxHealth;
        movementSpeed = setupData.movementSpeed;
        
        skillData = new List<SkillData>();
        foreach (SkillData data in setupData.startingSkillSet)
        {
            skillData.Add(data);
        }
    }
}
