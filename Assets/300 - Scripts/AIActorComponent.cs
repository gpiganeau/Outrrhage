using UnityEngine;
using System.Collections;

public class AIActorComponent: MonoBehaviour
{
    [SerializeField] private ActorSetupData setupData;
    private ActorData actorData;
    private SkillsController skillsController;
    private MovementController movementController;

    //Will use a movement strategy to coordinate movement and a skills strategy to use skills
    //This will allow to define an enemy's behavior by a set of skills and 2 strategies

    // Use this for initialization
    void Start()
	{
        actorData = new ActorData(setupData);

        skillsController = GetComponent<SkillsController>();
        skillsController.Initialize(actorData);
        movementController = GetComponent<MovementController>();
        movementController.Initialize(actorData);
    }

	// Update is called once per frame
	void Update()
	{

	}
}
