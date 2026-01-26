using UnityEngine;
using System.Collections;

public abstract class AttackStrategy : MonoBehaviour
{
	protected SkillsController _controller;
	public abstract void Initialize(AttackStrategySetupData setupData, SkillsController controller);
	public abstract void Execute();
	public abstract void Tick();
}
