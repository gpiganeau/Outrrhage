using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class SkillsController: MonoBehaviour
{
    private List<SkillStrategy> activeSkillStrategies;
    public List<SkillStrategy> ActiveSkillStrategies => activeSkillStrategies;
    private MovementController movementController;
    private List<string> skillsDisabledSources;
    private List<ISkillConstrainer> constraints;

    // -- Events --
    public event Action<List<SkillStrategy>> OnSkillsInitialized; 
    public event Action<SkillStrategy, int> OnSkillExecuted; // -- Skill, Slot

	private AnimController animController;
    [SerializeField] private Team _team = Team.Neutral;


    //Still have to move the inputs into the CharacterComponent
    public void Initialize(ActorSetupData actorData, AnimController animController = null)
    {
        constraints = new List<ISkillConstrainer>();
        // -- References Injection
        movementController = GetComponent<MovementController>();
        this.animController = animController;
        _team = actorData.team;

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

        foreach (SkillData data in actorData.startingSkillSet)
        {
            SkillStrategy skillStrategy = Instantiate(data.SkillStrategyPrefab, transform).GetComponent<SkillStrategy>();
            skillStrategy.Initialize(this, data);
            activeSkillStrategies.Add(skillStrategy);
        }

        OnSkillsInitialized?.Invoke(activeSkillStrategies);
    }

    public void AddConstrainer(ISkillConstrainer constrainer)
    {
        constraints.Add(constrainer);
    }

    public void CallSkillStrategy(int strategyIndex)
    {
        if (strategyIndex >= 0 && strategyIndex < activeSkillStrategies.Count)
        {
            SkillStrategy skill = activeSkillStrategies[strategyIndex];
            
            if (skill.Call(movementController, _team))
            {
                OnSkillExecuted?.Invoke(skill, strategyIndex);
                animController?.Trigger(skill.SkillData.AnimationKey);
            }
        }
    }

    public void CallSkillStrategyReleased(int strategyIndex)
    {
        if (!(strategyIndex >= 0 && strategyIndex < activeSkillStrategies.Count)) return;
        SkillStrategy skill = activeSkillStrategies[strategyIndex];
        if (!skill.SkillData.IsHold) return;
        skill.Release(movementController, _team);
    }

    public void CallRandomSkill()
    {
        CallSkillStrategy(UnityEngine.Random.Range(0, activeSkillStrategies.Count));
    }

    //We might prefer the player not being able to use lots of skills at once. He is blocked of using other skills when he is using one.
    //Can also be used if stunned or silenced
    //We might need to buffer inputs for about 0.3s to avoid them being lost when skills are disabled
    public void SetSkillsDisabled(bool value, string source)
    {
        if (value)
            skillsDisabledSources.Add(source);
        else
        {
            if(skillsDisabledSources.Contains(source))
                skillsDisabledSources.Remove(source);
            else
                Logger.LogWarning(Logger.LogCategory.Combat, $"Tried to enable skills from source {source} which was not disabling them.");
        }
    }

    public bool CheckSkillAvailability(int index)
    {
        if (skillsDisabledSources.Count > 0) return false;
        foreach (ISkillConstrainer constrainer in constraints) { 
            if (!constrainer.CanUseSkill(activeSkillStrategies[index].SkillData, movementController))
            {
                return false;
            }
        }

        return true;
    }
}
