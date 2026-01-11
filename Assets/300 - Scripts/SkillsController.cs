using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class SkillsController: MonoBehaviour
{
    private List<SkillStrategy> activeSkillStrategies;
    private MovementController movementController;
    private List<string> skillsDisabledSources;

    //Still have to move the inputs into the CharacterComponent
    public void Initialize(ActorData actorData)
    {
        movementController = GetComponent<MovementController>();
        skillsDisabledSources = new List<string>();
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
            skillStrategy.Initialize(this, data);
            activeSkillStrategies.Add(skillStrategy);
        }
    }

    public void CallSkillStrategy(int strategyIndex)
    {
        if (strategyIndex >= 0 && strategyIndex < activeSkillStrategies.Count)
        {
            activeSkillStrategies[strategyIndex].Call(movementController);
        }
    }

    //We might prefer the player not being able to use lots of skills at once. He is blocked of using other skills when he is using one.
    //Can also be used if stunned or silenced
    public void SetSkillsDisabled(bool value, string source)
    {
        if (value)
            skillsDisabledSources.Add(source);
        else
        {
            if(skillsDisabledSources.Contains(source))
                skillsDisabledSources.Remove(source);
            else
                Debug.LogWarning($"Tried to enable skills from source {source} which was not disabling them.");
        }
    }
}
