using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class BloodStackDisplay: MonoBehaviour
{
	// -- Todo : Show Max Slot with grey value / Empty version  ?

	[SerializeField] private BloodSlot _bloodSlotPrefab;
	private BloodStack _stack;
	private List<BloodSlot> _slots;


    public void Initialize(BloodStack stack)
	{
		_slots = new ();
		_stack = stack;

		for (int i = 0; i < _stack.MaxBlood; i++)
		{
			AddSlot();
		}
    }

	public void Sync(int amount)
	{
		int index = 0;
		foreach (var s in _slots)
		{
			if (index < amount)
			{
				s.Show();
			} else
			{
				s.Show();
			}

			index++;
		}
	}

	private void AddSlot()
	{
		BloodSlot slot = Instantiate(_bloodSlotPrefab, transform);
		_slots.Add(slot);
		slot.Show();
	}

	private void RemoveSlot()
	{
		
	}

	private void RemoveSlots(int amount)
	{
		
	}

	private void ClearSlots()
	{
		transform.Clear();
	}
}
