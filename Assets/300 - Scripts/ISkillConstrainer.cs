using UnityEngine;
using System.Collections;

public interface ISkillConstrainer
{
	public bool CanUseSkill(SkillData skillData, MovementController movementController);
}
