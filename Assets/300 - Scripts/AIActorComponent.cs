using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System;

public class AIActorComponent: MonoBehaviour
{
    [SerializeField] private AIActorSetupData setupData;
    private SkillsController skillsController;
    private MovementController movementController;
    private DamageController damageController;

    //AI Attributes
    private MovementStrategy _movementStrategy;

    [SerializeField] private CharacterComponent debugCharacterComponent;


    //Will use a movement strategy to coordinate movement and a skills strategy to use skills
    //This will allow to define an enemy's behavior by a set of skills and 2 strategies

    // Use this for initialization
    void Start()
	{
        skillsController = GetComponent<SkillsController>();
        skillsController.Initialize(setupData);
        movementController = GetComponent<MovementController>();
        movementController.Initialize(setupData);
        damageController = GetComponent<DamageController>();
        damageController.Initialize(setupData);

        //Initialize AI Strategies
        if (setupData.movementSetupData != null)
        {
            Type type = setupData.movementSetupData.movementStrategyScript.GetType();
            _movementStrategy = gameObject.AddComponent(type) as MovementStrategy;
            _movementStrategy.Initialize(setupData.movementSetupData);
        }
    }

    // Update is called once per frame
    void Update()
	{
        if(_movementStrategy != null)
        {
            MovementContext context = new MovementContext(this.transform.position, debugCharacterComponent.transform.position);
            Vector3 movementDirection = _movementStrategy.GetMovementDirection(context);
            movementController.SetMovementDirection(movementDirection);
        }
    }
}
