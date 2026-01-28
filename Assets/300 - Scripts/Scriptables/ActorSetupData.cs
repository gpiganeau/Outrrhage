using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSetupData: ScriptableObject
{
    public int maxHealth;
    public float movementSpeed;
    public List<SkillData> startingSkillSet;

    public Team team = Team.Neutral;

    [Header("Death and Loot")]
    public bool LootOnDeath = false;
    public List<GameObject> _itemsLootsOnDeath;
}

