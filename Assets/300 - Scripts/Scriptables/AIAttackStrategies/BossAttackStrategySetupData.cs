using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Attack Strategy", menuName = "Scriptable Objects/Game/AI/AS Boss ")]
public class BossAttackStrategySetupData : AttackStrategySetupData
{    
    [Header("Strategies")]

    public List<BossStrategy> Strategies;

    public int BattleEndAtHealth = 3;


    [Serializable]
    public class BossStrategy {
            
        public float HealthThreshold = 0;
        public List<AttackWrapper> AttackSettings = new();
        public List<int> AttackOrders = new List<int> { 0, 0, 1 };
        public float TimeToNextSkills;
    }

}
