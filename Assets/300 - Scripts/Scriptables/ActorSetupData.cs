using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSetupData: ScriptableObject
{
    [Header ("Character")]
    public string Name;
    public int maxHealth;
    public float movementSpeed;

    [Header("Combat")]
    public Team team = Team.Neutral;
    public List<SkillData> startingSkillSet;

    [Header("Death and Loot")]
    public bool LootOnDeath = false;
    public List<GameObject> _itemsLootsOnDeath;
}

