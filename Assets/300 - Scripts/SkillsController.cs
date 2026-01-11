using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class SkillsController: MonoBehaviour
{
    private List<SkillStrategy> activeSkillStrategies;
    private MovementController movementController;

    //Still have to move the inputs into the CharacterComponent
    public void Initialize(ActorData actorData)
    {
        movementController = GetComponent<MovementController>();
        //delete old objects if they exist
        if (activeSkillStrategies != null)
        {
            foreach (SkillStrategy strategy in activeSkillStrategies)
            {
                Destroy(strategy.gameObject);
            }
        }
        activeSkillStrategies = new List<SkillStrategy>();

        foreach (SkillData data in actorData.skillData)
        {
            SkillStrategy skillStrategy = Instantiate(data.SkillStrategyPrefab, transform).GetComponent<SkillStrategy>();
            skillStrategy.Initialize(data);
            activeSkillStrategies.Add(skillStrategy);
        }
    }

    public void CallSkillStrategy(int strategyIndex)
    {
        if (strategyIndex >= 0 && strategyIndex < activeSkillStrategies.Count)
        {
            activeSkillStrategies[strategyIndex].Call(transform.position, movementController.GetFacingDirection());
        }
    }
}
