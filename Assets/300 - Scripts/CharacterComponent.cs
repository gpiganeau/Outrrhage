using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Pilots other components, specifically translates inputs into I/O for attached controllers
/// </summary>
public class CharacterComponent : PilotComponent, ISkillConstrainer, IJuicable
{
	[SerializeField] private CharacterSetupData setupData;
	private SkillsController skillsController;
	private MovementController movementController;
    private DamageController damageController;
    private AnimController animController;

    [SerializeField] private Blood blood;
    [SerializeField] private Rage rage;
    public static Blood Blood;
    public static Rage Rage;
    public CameraController PlayerCameraController { get; set; }
    private List<Renderer> Renderers;

    private  UnityEngine.Events.UnityAction<Vector2>  onMovementAction;
    private UnityEngine.Events.UnityAction onSlot1Action, onSlot2Action, onSlot3Action, onSlot4Action, onSlot5Action;
    private UnityEngine.Events.UnityAction onSlot1ReleasedAction, onSlot2ReleasedAction, onSlot3ReleasedAction, onSlot4ReleasedAction, onSlot5ReleasedAction;

    
    public static bool InRage => Rage.IsFull;
    Coroutine _rageTick;

    IEnumerator RageTickRoutine()
    {
        var gps = SettingsManager.Instance.GameplaySettings;
        
        while (true)
        {
            yield return new WaitForSeconds(gps.LossRageTick);
            if (!InRage) Rage.Consume(gps.LossRageAmount);
        }
    }

    void Start()
	{
        // -- Initialize Blood Singleton for CharacterComponent.Blood() -- YES I ASSUME THIS WILL BE A SOLO GAME FOREVER 
        blood = new Blood(setupData.maxBlood);
        Blood = blood;

        rage = new Rage(setupData);
        Rage = rage;

        Rage.OnRageEnter.AddListener(OnRageEnter);
        Rage.OnRageExit.AddListener(OnRageExit);
        Rage.OnRageChanged.AddListener(OnRageChanged);



        // -- Setup Components
        animController = GetComponent<AnimController>();
        animController?.Initialize(setupData);
        skillsController = GetComponent<SkillsController>();
        skillsController.Initialize(setupData, animController);
        skillsController.AddConstrainer(this);
        movementController = GetComponent<MovementController>();
        movementController.Initialize(setupData, animController);
        damageController = GetComponent<DamageController>();
        damageController.Initialize(setupData, animController);


        // -- Setup Callback & Listeners
        damageController.OnDamaged.AddListener((currentHealth, maxHealth) => OnDamaged(currentHealth, maxHealth));
        damageController.OnHealed.AddListener((currentHealth, maxHealth) => OnHealed(currentHealth, maxHealth));
        damageController.OnDied.AddListener(() => OnDeath());

        skillsController.OnSkillExecuted += OnSkillExecuted;

        // -- Inputs Settings, only once
        onMovementAction = OnInputVector;
        onSlot1Action = () => skillsController.CallSkillStrategy(0);
        onSlot2Action = () => skillsController.CallSkillStrategy(1);
        onSlot3Action = () => skillsController.CallSkillStrategy(2);
        onSlot4Action = () => skillsController.CallSkillStrategy(3);
        onSlot5Action = () => skillsController.CallSkillStrategy(4);
        
        onSlot1ReleasedAction = () => skillsController.CallSkillStrategyReleased(0);
        onSlot2ReleasedAction = () => skillsController.CallSkillStrategyReleased(1);
        onSlot3ReleasedAction = () => skillsController.CallSkillStrategyReleased(2);
        onSlot4ReleasedAction = () => skillsController.CallSkillStrategyReleased(3);
        onSlot5ReleasedAction = () => skillsController.CallSkillStrategyReleased(4);

        // -- Rage Tick
        _rageTick = StartCoroutine(RageTickRoutine());
    
        // Subscribe
        EnableControls();

        HUD.Instance.Initialize(skillsController, damageController);

        Renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

        // -- When everything is fine
        if (SettingsManager.Instance.GameplaySettings.OpenSkillSelectorOnRunStart) return;
    }

    public void EnableControls()
    {
        InputManager.Instance.OnCharacterMovement.AddListener(onMovementAction);
        InputManager.Instance.OnCharacterSlot1.AddListener(onSlot1Action);
        InputManager.Instance.OnCharacterSlot2.AddListener(onSlot2Action);
        InputManager.Instance.OnCharacterSlot3.AddListener(onSlot3Action);
        InputManager.Instance.OnCharacterSlot4.AddListener(onSlot4Action);
        InputManager.Instance.OnCharacterSlot5.AddListener(onSlot5Action);
        
        InputManager.Instance.OnCharacterSlot1Released.AddListener(onSlot1ReleasedAction);
        InputManager.Instance.OnCharacterSlot2Released.AddListener(onSlot2ReleasedAction);
        InputManager.Instance.OnCharacterSlot3Released.AddListener(onSlot3ReleasedAction);
        InputManager.Instance.OnCharacterSlot4Released.AddListener(onSlot4ReleasedAction);
        InputManager.Instance.OnCharacterSlot5Released.AddListener(onSlot5ReleasedAction);
        
        if(skillsController != null) skillsController.enabled = true;
        if(movementController != null) movementController.enabled = true;
    }

    public void DisableControls()
    {
        InputManager.Instance.OnCharacterMovement.RemoveListener(onMovementAction);
        InputManager.Instance.OnCharacterSlot1.RemoveListener(onSlot1Action);
        InputManager.Instance.OnCharacterSlot2.RemoveListener(onSlot2Action);
        InputManager.Instance.OnCharacterSlot3.RemoveListener(onSlot3Action);
        InputManager.Instance.OnCharacterSlot4.RemoveListener(onSlot4Action);
        InputManager.Instance.OnCharacterSlot5.RemoveListener(onSlot5Action);
        
        InputManager.Instance.OnCharacterSlot1Released.RemoveListener(onSlot1ReleasedAction);
        InputManager.Instance.OnCharacterSlot2Released.RemoveListener(onSlot2ReleasedAction);
        InputManager.Instance.OnCharacterSlot3Released.RemoveListener(onSlot3ReleasedAction);
        InputManager.Instance.OnCharacterSlot4Released.RemoveListener(onSlot4ReleasedAction);
        InputManager.Instance.OnCharacterSlot5Released.RemoveListener(onSlot5ReleasedAction);
        
        if(skillsController != null) skillsController.enabled = false;
        if(movementController != null) movementController.enabled = false;
    }

    #region Listeners & Callback

    public override void OnProjectileHit(Projectile projectile, DamageController damageController, SkillData data)
    {
        Rage.Regain(data.RageGain);
    }


    private void OnSkillExecuted(SkillStrategy strategy, int index)
    {
        if (InRage) return;
        
        Blood.Consume(strategy.SkillData.BloodCost);
    }

    private void OnRageChanged(int current, int max)
    {
        
    }

    private void OnRageExit(float duration)
    {
        Juicer.I.StopRage(duration);
        AudioManager.Instance.PlayClipAtPoint(setupData.RageExitClip.Random(), transform.position);
    }

    private void OnRageEnter(float duration)
    {

        if (damageController.IsDead) return;
        Juicer.I.StartRage(duration);
        AudioManager.Instance.PlayClipAtPoint(setupData.RagerEnterClip.Random(), transform.position);
    }

    private void OnDamaged(int currentHealth, int maxHealth)
    {
        Blood.Regain(1);
        Rage.Regain(1);
        Juicer.I.PlayerDamagedImpact(GetRenderers());
        var fx = Instantiate(setupData.BloodSplasherPrefab, transform.position.WithY(1f), Quaternion.identity).GetComponentInChildren<VisualEffect>(); 
        fx.Play();
        AudioManager.Instance.PlayClipAtPoint(setupData.HitClip.Random(), transform.position);

    }

    private void OnHealed(int currentHealth, int maxHealth)
    {
        Juicer.I.PlayerHealedEffect(GetRenderers());
        AudioManager.Instance.PlayClipAtPoint(setupData.HealClip.Random(), transform.position);

    }

    private void OnDeath()
    {
        if (damageController.IsDead) return;
        var settings = SettingsManager.Instance.GameplaySettings;

        damageController.IsDead = true;
        DisableControls();  
        Rage.ForceStop();
        Juicer.I.PlayerDeathEffect();
        AudioManager.Instance.PlayClipAtPoint(setupData.DeathClip.Random(), transform.position);
        animController.Die();   
        if (settings.ClearRoomOnDeath) EntityManager.Instance.FullClearRoom();
        DOVirtual.DelayedCall(settings.DeathTimeBeforeReload, () => GameManager.Instance.ReloadCurrentScene());
    }
    #endregion

    #region Input Handling

    void OnInputVector(Vector2 newVector)
    {
        movementController.SetMovementDirection(CharacterInputFromCameraPerspective(newVector));
    }

    #endregion

    #region Computations

    private Vector3 CharacterInputFromCameraPerspective(Vector2 inputVector)
    {
        Vector3 rValue;
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Vector3 cameraVect = inputVector.x * PlayerCameraController.Right + inputVector.y * PlayerCameraController.Up;
        cameraVect.Normalize();
        rValue = Vector3.ProjectOnPlane(cameraVect, groundPlane.normal);
        return rValue;
    }

    #endregion

    #region SkillConstrainer

    public bool CanUseSkill(SkillData skillData, MovementController movementController)
    {
        return InRage || skillData.BloodCost <= Blood.Amount;
    }

    #endregion

    #region IJUicable

    public List<Renderer> GetRenderers()
    {
        return Renderers;
    }

    #endregion
}
