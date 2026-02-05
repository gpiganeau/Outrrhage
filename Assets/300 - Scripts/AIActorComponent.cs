using UnityEngine;
using UnityEngine.VFX;

public class AIActorComponent: MonoBehaviour
{
    [SerializeField] private AIActorSetupData setupData;
    [SerializeField] private HealthBarDisplay healthBarDisplay;
    [SerializeField] private MeshRenderer mainRenderer;
    [SerializeField] private BloodStack bloodStack;

    private SkillsController skillsController;
    private MovementController movementController;
    private DamageController damageController;


    //AI Attributes
    private MovementStrategy _movementStrategy;
    private AttackStrategy _attackStrategy;

    [SerializeField] private CharacterComponent debugCharacterComponent;


    //Will use a movement strategy to coordinate movement and a skills strategy to use skills
    //This will allow to define an enemy's behavior by a set of skills and 2 strategies

    void Start()
	{
        // -- Skill Controller
        skillsController = GetComponent<SkillsController>();
        skillsController.Initialize(setupData);

        // -- Movement Controller
        movementController = GetComponent<MovementController>();
        movementController.Initialize(setupData);
        
        // -- Damage Controller
        damageController = GetComponent<DamageController>();
        damageController.Initialize(setupData);
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
            movementController.SetMovementDirection(movementDirection);
        }

        if (_attackStrategy != null)
        {
            _attackStrategy.Tick();
        }
    }

    private void OnDamaged(int a, int b)
    {
        Juicer.I.EnemyDamagedImpact(0.3f, mainRenderer);
        bloodStack.Increase(1);
        var fx = Instantiate(setupData.BloodSplasherPrefab, transform.position.WithY(1f), Quaternion.identity).GetComponent<VisualEffect>(); 
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

        // -- Check if last enemy Alive ? Or From  Enemy Data (Boss, Elite...)
        Juicer.I.SlowMotion(0.25f, 0.1f);

        Destroy(this.gameObject);
    }
}
