using UnityEngine;

public abstract class AttackStrategy : MonoBehaviour
{
	protected SkillsController _controller;
    protected MovementController _movement;
	public abstract void Initialize(AttackStrategySetupData setupData, SkillsController controller, MovementController movement);
	public abstract void Execute();
	public abstract void Tick(MovementContext context);

}

    [System.Serializable]
    public class AttackWrapper
    {
        public string AttackNameToHelpDesigner = "Help";
        public float MinimumDistanceToRiel;


        [Header("Marque l'arret ?")]
        public bool StopWhenAttacking = false;
        public float StopDuration = 1f;
    }
