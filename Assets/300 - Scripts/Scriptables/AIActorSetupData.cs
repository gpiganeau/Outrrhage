using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAIActorSetupData", menuName = "Scriptable Objects/Game/AIActorSetupData")]
public class AIActorSetupData : ActorSetupData
{
    [Header("AI Setup Datas")]
    public MovementStrategySetupData movementSetupData;
    public AttackStrategySetupData attackSetupData;

}