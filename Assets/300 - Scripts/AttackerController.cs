using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("Instanciated on the character to control the flow of a skill")]
public class AttackerController : MonoBehaviour
{
    List<SkillStrategy> activeSkillStrategies;

    public void Initialize(CharacterData characterData)
    {
        InputManager.Instance.OnCharacterSlot1.RemoveAllListeners();
        InputManager.Instance.OnCharacterSlot2.RemoveAllListeners();
        InputManager.Instance.OnCharacterSlot3.RemoveAllListeners();
        InputManager.Instance.OnCharacterSlot4.RemoveAllListeners();
        InputManager.Instance.OnCharacterSlot5.RemoveAllListeners();
        //delete old objects if they exist
        if (activeSkillStrategies != null)
        {
            foreach (SkillStrategy strategy in activeSkillStrategies)
            {
                Destroy(strategy.gameObject);
            }
        }
        activeSkillStrategies = new List<SkillStrategy>();

        foreach (SkillData data in characterData.skillData)
        {
            SkillStrategy skillStrategy = Instantiate(data.SkillStrategyPrefab, transform).GetComponent<SkillStrategy>();
            skillStrategy.Initialize(data);
            activeSkillStrategies.Add(skillStrategy);
        }

        if (activeSkillStrategies.Count > 0)
            InputManager.Instance.OnCharacterSlot1.AddListener(activeSkillStrategies[0].Call);
        if (activeSkillStrategies.Count > 1)
            InputManager.Instance.OnCharacterSlot2.AddListener(activeSkillStrategies[1].Call);
        if (activeSkillStrategies.Count > 2)
            InputManager.Instance.OnCharacterSlot3.AddListener(activeSkillStrategies[2].Call);
        if (activeSkillStrategies.Count > 3)
            InputManager.Instance.OnCharacterSlot4.AddListener(activeSkillStrategies[3].Call);
        if (activeSkillStrategies.Count > 4)
            InputManager.Instance.OnCharacterSlot5.AddListener(activeSkillStrategies[4].Call);
    }
    
}
