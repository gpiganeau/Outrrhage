using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public event Action<SkillStrategy, int> OnSkillCooldownStarted;
    public event Action<List<SkillStrategy>> OnSkillsChanged;

	private AnimController animController;
    [SerializeField] private Team _team = Team.Neutral;

    [SerializeField] private int _maxSkillSlots = 5; // -- 5 for Riel
    public int MaxSkillSlots => _maxSkillSlots;

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

        SetActiveSkillStrategies(actorData.startingSkillSet);
    }

    public void SetActiveSkillStrategies(List<SkillData> skillDatas)
    {
        var pilot = GetComponent<PilotComponent>();

        // -- Clear les anciennes
        if (activeSkillStrategies != null)
            foreach (var s in activeSkillStrategies)
                if (s != null) Destroy(s.gameObject);

        activeSkillStrategies = new List<SkillStrategy>();

        foreach (SkillData data in skillDatas)
        {
            SkillStrategy skillStrategy = Instantiate(data.SkillStrategyPrefab, transform).GetComponent<SkillStrategy>();
            skillStrategy.Initialize(this, data, pilot);
            activeSkillStrategies.Add(skillStrategy);

            int index = activeSkillStrategies.Count - 1;
            skillStrategy.OnCooldownStarted += () => OnSkillCooldownStarted?.Invoke(skillStrategy, index);
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
            SkillData data = skill.SkillData;

            if (skillsDisabledSources.Count > 0) return;
                
            foreach (ISkillConstrainer constrainer in constraints) { 

                if (!constrainer.CanUseSkill(data, movementController))
                {
                    return;
                }
            }

            
            if (skill.Call(movementController, _team))
            {
                OnSkillExecuted?.Invoke(skill, strategyIndex);
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
        int randomIndex = UnityEngine.Random.Range(0, activeSkillStrategies.Count);
        CallSkillStrategy(randomIndex);
        CallSkillStrategyReleased(randomIndex);
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

        if (index >= MaxSkillSlots)
        {
            return false;
        }

        if (index < 0 || index >= activeSkillStrategies.Count) return false;

        foreach (ISkillConstrainer constrainer in constraints) { 

            if (activeSkillStrategies[index] == null) return false;
            if (activeSkillStrategies[index].SkillData == null) return false;

            if (!constrainer.CanUseSkill(activeSkillStrategies[index].SkillData, movementController))
            {
                return false;
            }
        }

        return true;
    }

    #region In Game Modification ?
    /// <summary>
    /// Ajoute une nouvelle skill dans le premier slot vide disponible
    /// </summary>
    public bool AddSkill(SkillData skillData)
    {
        if (activeSkillStrategies.Count >= _maxSkillSlots)
        {
            Logger.LogWarning(Logger.LogCategory.Combat, "Cannot add skill: max slots reached");
            return false;
        }
        
        InstantiateSkillStrategy(skillData);
        OnSkillsChanged?.Invoke(activeSkillStrategies);
        return true;
    }
    
   
    /// <summary>
    /// Remplace une skill existante par une nouvelle
    /// </summary>
    public void ReplaceSkill(int slotIndex, SkillData newSkillData)
    {
        if (slotIndex >= 0 && slotIndex < activeSkillStrategies.Count)
        {
            // Destroy l'ancienne
            Destroy(activeSkillStrategies[slotIndex].gameObject);
            
            // Crée la nouvelle
            SkillStrategy newStrategy = InstantiateSkillStrategy(newSkillData);
            
            // Replace dans la liste
            activeSkillStrategies[slotIndex] = newStrategy;
            OnSkillsChanged?.Invoke(activeSkillStrategies);
        }
    }
    
    /// <summary>
    /// Swap deux skills entre elles
    /// </summary>
    public void SwapSkills(int slotA, int slotB)
    {
        if (slotA >= 0 && slotA < activeSkillStrategies.Count && 
            slotB >= 0 && slotB < activeSkillStrategies.Count)
        {
            var temp = activeSkillStrategies[slotA];
            activeSkillStrategies[slotA] = activeSkillStrategies[slotB];
            activeSkillStrategies[slotB] = temp;
            OnSkillsChanged?.Invoke(activeSkillStrategies);
        }
    }
    
    /// <summary>
    /// Récupère la SkillData d'un slot spécifique
    /// </summary>
    public SkillData GetSkillData(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < activeSkillStrategies.Count)
        {
            return activeSkillStrategies[slotIndex].SkillData;
        }
        return null;
    }
    
    
    private SkillStrategy InstantiateSkillStrategy(SkillData data)
    {
        SkillStrategy skillStrategy = Instantiate(data.SkillStrategyPrefab, transform).GetComponent<SkillStrategy>();
        skillStrategy.Initialize(this, data, GetComponent<PilotComponent>());
        activeSkillStrategies.Add(skillStrategy);
        return skillStrategy;
    }
    #endregion
}
