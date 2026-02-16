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
        foreach (Transform c in transform)
        {
            Destroy(c.gameObject);
        }

        int index  = 0 ;
        foreach(var s in strategies)
        {
            // -- Setup Slots
            SkillSlot slot = Instantiate(_slotPrefab, this.transform);
            slot.Init(s, index);
            _slots.Add(slot);
            index++;
        }
    }

    public void SetInCooldown(int slot)
    {
        _slots[slot].TriggerCooldown();
    }

    public void UpdateAvailability(SkillsController controller)
    {
        for(int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetAvailable(controller.CheckSkillAvailability(i));
        }
    }

}
