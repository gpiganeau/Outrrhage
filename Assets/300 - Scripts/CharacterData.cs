using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int maxHealth;
    public int currentHealth;
    public int currentRage;
    public int maxBlood;
    public int currentBlood;
    public List<SkillData> skillData;

    public CharacterData(CharacterSettings characterSettings)
    {
        maxHealth = characterSettings.maxHealth;
        currentHealth = maxHealth;
        currentRage = 0;
        maxBlood = characterSettings.maxBlood;
        currentBlood = 0;
        skillData = new List<SkillData>();
    }

    public void DebugSetSkills(SkillData debugSkillData)
    {
        skillData.Add(debugSkillData);
    }
}
