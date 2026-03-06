using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Attack Strategy", menuName = "Scriptable Objects/Game/AI/AS Boss ")]
public class BossAttackStrategySetupData : AttackStrategySetupData
{    
    [Header("Strategies")]

    public List<BossStrategy> Strategies;

    public int BattleEndAtHealth = 3;
    public List<AttackWrapper> AttackWrappers;

    public GameObject BloodDropPrefab;

    [Serializable]
    public class BossStrategy {
            
        public string strategyName = "Nom de la phase";
        public float HealthThreshold = 0;
        public float TimeToNextSkills;
        public List<int> AttackOrders = new List<int> { 0, 0, 1 };
    }
}
