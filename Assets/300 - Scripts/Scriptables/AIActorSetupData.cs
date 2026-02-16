using UnityEngine;

[CreateAssetMenu(fileName = "NewAIActorSetupData", menuName = "Scriptable Objects/Game/AIActorSetupData")]
public class AIActorSetupData : ActorSetupData
{
    [Header("AI Setup Datas")]
    public MovementStrategySetupData movementSetupData;
    public AttackStrategySetupData attackSetupData;

    public float TimeBeforeBrainActivation = 0.5f; // -- Time before the AI starts acting, to let player react to spawn

}