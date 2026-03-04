using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ActorSetupData: ScriptableObject
{
    [Header ("Character")]
    public string Name;
    public int maxHealth;
    public float movementSpeed;
    public int maxBloodStack = 10;

    [Header("Combat")]
    public Team team = Team.Neutral;
    public List<SkillData> startingSkillSet;

    [Header("Death and Loot")]
    public bool LootOnDeath = false;
    public List<GameObject> _itemsLootsOnDeath;
    public bool LootBloodOnDeath = true;
    public int BaseBloodDrop = 1;
    public GameObject BloodPrefab;

    [Header("Common Visuals")]
    public GameObject BloodSplasherPrefab;

    [Header("Common Audio")]
    public AudioClip[] HitClip;
    public AudioClip[] DeathClip, HealClip;
}

