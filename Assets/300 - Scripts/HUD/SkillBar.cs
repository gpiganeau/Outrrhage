using System.Collections.Generic;
using UnityEngine;

public class SkillBar : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private SkillSlot _slotPrefab;

    [SerializeField] private List<SkillSlot> _slots;

    public void Init(List<SkillStrategy> strategies)
    {
        // -- Clear before init in case of multiples / skill changing

        foreach(var s in strategies)
        {
            // -- Setup Slots
            SkillSlot slot = Instantiate(_slotPrefab, this.transform);
            slot.Init(s);
            // -- Register Event/Callback like, when the skill is triggered, enter cooldown...?
            // -- Todo 
        }
    }
}
