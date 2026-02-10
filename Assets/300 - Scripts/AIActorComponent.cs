using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class AIActorComponent: MonoBehaviour, ISkillConstrainer, IJuicable
{
    [SerializeField] private AIActorSetupData setupData;
    [SerializeField] private HealthBarDisplay healthBarDisplay;
    [SerializeField] private BloodStack bloodStack;

    private SkillsController skillsController;
    private MovementController movementController;
    private DamageController damageController;


    //AI Attributes
    private MovementStrategy _movementStrategy;
    private AttackStrategy _attackStrategy;

    [SerializeField] private CharacterComponent debugCharacterComponent;

    private List<Renderer> Renderers;
    private AnimController animController;



    //Will use a movement strategy to coordinate movement and a skills strategy to use skills
    //This will allow to define an enemy's behavior by a set of skills and 2 strategies

    void Start()
	{

       // -- AnimController
        animController = GetComponent<AnimController>();
        animController?.Initialize(setupData);

        // -- Skill Controller
        skillsController = GetComponent<SkillsController>();
        skillsController.Initialize(setupData, animController);
        skillsController.AddConstrainer(this);

        // -- Movement Controller
        movementController = GetComponent<MovementController>();
        movementController.Initialize(setupData, animController);
        
        // -- Damage Controller
        damageController = GetComponent<DamageController>();
        damageController.Initialize(setupData, animController);
        damageController.OnDied.AddListener(OnDeath);
        damageController.OnDamaged.AddListener(OnDamaged);

 

        // -- World UI
        healthBarDisplay.Initialize(damageController);
        bloodStack.Initialize(setupData);

        //Initialize AI Strategies
        if (setupData.movementSetupData != null)
        {
            System.Type type = setupData.movementSetupData.movementStrategyScript.GetType();
            _movementStrategy = gameObject.AddComponent(type) as MovementStrategy;
            _movementStrategy.Initialize(setupData.movementSetupData);
        }

        if (setupData.attackSetupData != null)
        {
            System.Type type = setupData.attackSetupData.attackStrategyScript.GetType();
            _attackStrategy = gameObject.AddComponent(type) as AttackStrategy;
            _attackStrategy.Initialize(setupData.attackSetupData, skillsController);
        }

        Renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

    }

    void Update()
	{
        // -- Pour esquiver la ref Serializer : @TODO : Enemy Maanger ?
        if (debugCharacterComponent == null)
        {
            debugCharacterComponent = GameManager.Instance.Riel;

            if (debugCharacterComponent == null) return;
        } 
        
        if(_movementStrategy != null)
        {
            MovementContext context = new MovementContext(this.transform.position, debugCharacterComponent.transform.position);
            Vector3 movementDirection = _movementStrategy.GetMovementDirection(context);
            Vector3 facingDirection = _movementStrategy.GetFacingDirection(context);
            movementController.SetMovementDirection(movementDirection);
            movementController.SetFacingDirection(facingDirection);
        }

        if (_attackStrategy != null)
        {
            _attackStrategy.Tick();
        }
    }

    private void OnDamaged(int a, int b)
    {
        Juicer.I.EnemyDamagedImpact(0.3f, GetRenderers());
        //Todo @Gregoire : On est sur 1 magique, mais faudrait que ça varie en fonction du spell
        bloodStack.Increase(1);
        var fx = Instantiate(setupData.BloodSplasherPrefab, transform.position.WithY(1f), Quaternion.identity).GetComponentInChildren<VisualEffect>(); 
        fx.Play();
    }

    public void ForceKill()
    {
        damageController.Damage(1000, transform.position, Team.Neutral);
    }
    private void OnDeath() 
    {
        if (setupData.LootOnDeath && setupData._itemsLootsOnDeath.Count > 0)
        {
            var loot = setupData._itemsLootsOnDeath.Random();
            Instantiate(loot, transform.position, Quaternion.identity);
        }

        if (setupData.LootBloodOnDeath)
        {
            int dropAmount = bloodStack.GetStackedValue();
            for (int i = 0; i < dropAmount; i++)
            {
                var vec = Random.insideUnitCircle;
                Vector3 offset = new Vector3(vec.x, 0.5f, vec.y);
                var pos = transform.position + offset;
                Instantiate(setupData.BloodPrefab, pos, Quaternion.identity);
            }
        }

        // -- Check if last enemy Alive ? Or From  Enemy Data (Boss, Elite...) or % Chance of procing this ?
        Juicer.I.EnemyDeathEffect();


        if (animController != null)
        {
            animController.Die();   
            DOVirtual.DelayedCall(animController.ClipLength("Dying"), () => Destroy(this.gameObject));
        } else
        {
            Destroy(this.gameObject);
        }


    }

#region Interfaces
    public bool CanUseSkill(SkillData skillData, MovementController movementController)
    {
        return true;
    }

    public List<Renderer> GetRenderers()
    {
        return Renderers;
    }
#endregion
}
