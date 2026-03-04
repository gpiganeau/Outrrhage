using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Main Attack Strategy", menuName = "Scriptable Objects/Game/AI/AS Main ")]
public class MainAttackStrategySetupData : AttackStrategySetupData
{    
    public List<AttackWrapper> AttackSettings = new();
    public float TimeToNextSkill = 1f;

}
