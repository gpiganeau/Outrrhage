using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class AIActorComponent: PilotComponent, ISkillConstrainer, IJuicable
{
    [SerializeField] private AIActorSetupData setupData;
    [SerializeField] private HealthBarDisplay healthBarDisplay;
    [SerializeField] private BloodStack bloodStack;

    private SkillsController skillsController;
    private MovementController movementController;
    private DamageController damageController;

    public EntityType EntityType => setupData.entityType;

    //AI Attributes
    private MovementStrategy _movementStrategy;
    private AttackStrategy _attackStrategy;

    [SerializeField] private CharacterComponent debugCharacterComponent;

    private List<Renderer> Renderers;
    private AnimController animController;

    private Rigidbody _rigidbody;
    private Collider _collider;
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
        damageController.OnHealed.AddListener(OnHealed);

        // -- World UI
        healthBarDisplay.Initialize(damageController);
        bloodStack.Initialize(setupData);

        //Initialize AI Strategies
        if (setupData.movementSetupData != null)
        {
            System.Type type = setupData.movementSetupData.movementStrategyScript.GetType();
            _movementStrategy = gameObject.AddComponent(type) as MovementStrategy;

            // -- Delayed call to let player react to spawn
            DOVirtual.DelayedCall(setupData.TimeBeforeBrainActivation, () => _movementStrategy.Initialize(setupData.movementSetupData)); // -- Delayed Call to
        }

        if (setupData.attackSetupData != null)
        {
            System.Type type = setupData.attackSetupData.attackStrategyScript.GetType();
            _attackStrategy = gameObject.AddComponent(type) as AttackStrategy;
            _attackStrategy.Initialize(setupData.attackSetupData, skillsController);
        }

        Renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

    }

    void Update()
	{
        if (damageController.IsDead) return;

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

    private void OnHealed(int a, int b)
    {
        Juicer.I.EnnemyHealedEffect(GetRenderers());
        AudioManager.Instance.PlayClipAtPoint(setupData.HealClip.Random(), transform.position);

    }

    private void OnDamaged(int a, int b)
    {
        Juicer.I.EnemyDamagedImpact(0.3f, GetRenderers());
        var fx = Instantiate(setupData.BloodSplasherPrefab, transform.position.WithY(1f), Quaternion.identity).GetComponentInChildren<VisualEffect>(); 
        fx.Play();
        AudioManager.Instance.PlayClipAtPoint(setupData.HitClip.Random(), transform.position);
    }

    public void ForceKill()
    {
        damageController.Damage(1000, transform.position, Team.Neutral);
    }
    private void OnDeath() 
    {
        damageController.IsDead = true;

        EntityManager.Instance.NotifyDeath(this);

        if (setupData.LootOnDeath && setupData._itemsLootsOnDeath.Count > 0)
        {
            var loot = setupData._itemsLootsOnDeath.Random();
            Instantiate(loot, transform.position, Quaternion.identity);
        }

        if (setupData.LootBloodOnDeath)
        {
            int dropAmount =  setupData.BaseBloodDrop + bloodStack.GetStackedValue();
            for (int i = 0; i < dropAmount; i++)
            {
                var vec = Random.insideUnitCircle * SettingsManager.Instance.GameplaySettings.BloodDispersionRadius;
                Vector3 offset = new Vector3(vec.x, 0.5f, vec.y);
                var pos = transform.position + offset;
                Instantiate(setupData.BloodPrefab, pos, Quaternion.identity);
            }
        }

        // -- Check if last enemy Alive ? Or From  Enemy Data (Boss, Elite...) or % Chance of procing this ?
        Juicer.I.EnemyDeathEffect();
        AudioManager.Instance.PlayClipAtPoint(setupData.DeathClip.Random(), transform.position);
        

        // -- Hides HUD
        healthBarDisplay.gameObject.SetActive(false);
        bloodStack.gameObject.SetActive(false);

        if (animController != null)
        {
            // -- Disable Controllers & Collisions for better death animation
            if (_rigidbody != null) _rigidbody.isKinematic = true;
            if (_collider != null)  _collider.enabled = false;
            skillsController.enabled = false;
            movementController.enabled = false;
            damageController.enabled = false;   
            
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
