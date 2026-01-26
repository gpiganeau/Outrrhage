using UnityEngine;
using System.Collections;

public abstract class AttackStrategy : MonoBehaviour
{
	public abstract void Initialize(AttackStrategySetupData setupData);
}
