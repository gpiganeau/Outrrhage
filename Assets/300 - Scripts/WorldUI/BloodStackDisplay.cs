using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
public class BloodStackDisplay: BillboardWorldUI
{

	[SerializeField] private BloodSlot _bloodSlotPrefab;
	private BloodStack _stack;
	private List<BloodSlot> _slots;


    public void Initialize(BloodStack stack)
	{
		_slots = new ();
		_stack = stack;

		for (int i = 0; i < _stack.MaxBlood; i++)
		{
			CreateSlot();
		}
    }

	public void Sync(int amount)
	{
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < amount)
                _slots[i].Show();
			else
				_slots[i].Hide();
        }
	}

	private void CreateSlot()
	{
		_slots.Add(Instantiate(_bloodSlotPrefab, transform));
	}

	private void RemoveSlot()
	{
		if (_slots.Count == 0) return;
        
        BloodSlot lastSlot = _slots[_slots.Count - 1];
        _slots.RemoveAt(_slots.Count - 1);
        
        lastSlot.transform.DOScale(0f, 0.2f).OnComplete(() => Destroy(lastSlot.gameObject));
	}

	private void RemoveSlots(int amount)
	{
		for (int i = 0; i < amount; i++)
        {
            RemoveSlot();
        }
	}

	private void ClearSlots()
	{
		foreach (var slot in _slots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        _slots.Clear();
	}
}
