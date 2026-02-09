using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Pilots other components, specifically translates inputs into I/O for attached controllers
/// </summary>
public class CharacterComponent : MonoBehaviour, ISkillConstrainer, IJuicable
{
	[SerializeField] private CharacterSetupData setupData;
	private SkillsController skillsController;
	private MovementController movementController;
    private DamageController damageController;
    private AnimController animController;

    [SerializeField] private Blood blood;
    public static Blood Blood;
    public CameraController PlayerCameraController { get; set; }
    private bool isDead = false;
    
    void Start()
	{
        // -- Initialize Blood Singleton for CharacterComponent.Blood() -- YES I ASSUME THIS WILL BE A SOLO GAME FOREVER 
        blood = new Blood(setupData.maxBlood);
        Blood = blood;

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
        damageController.OnDied.AddListener(() => OnDeath());

        InputManager.Instance.OnCharacterMovement.AddListener(OnInputVector);
        InputManager.Instance.OnCharacterSlot1.AddListener(() => skillsController.CallSkillStrategy(0));
        InputManager.Instance.OnCharacterSlot2.AddListener(() => skillsController.CallSkillStrategy(1));
        InputManager.Instance.OnCharacterSlot3.AddListener(() => skillsController.CallSkillStrategy(2));
        InputManager.Instance.OnCharacterSlot4.AddListener(() => skillsController.CallSkillStrategy(3));
        InputManager.Instance.OnCharacterSlot5.AddListener(() => skillsController.CallSkillStrategy(4));

        HUD.Instance.Initialize(skillsController, damageController);
    }

    #region Listeners & Callback
    private void OnDamaged(int currentHealth, int maxHealth)
    {
        Juicer.I.PlayerDamagedImpact(GetRenderers());
        var fx = Instantiate(setupData.BloodSplasherPrefab, transform.position.WithY(1f), Quaternion.identity).GetComponent<VisualEffect>(); 
    }

    private void OnDeath()
    {
          if (isDead) return;
            Juicer.I.PlayerDeathEffect();
            skillsController.enabled = false;
            movementController.enabled = false;
            damageController.enabled = false;   
            isDead = true;
            animController.Die();   
            DOVirtual.DelayedCall(animController.ClipLength("Dying"), () => GameManager.Instance.ReloadCurrentScene());
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
        return skillData.BloodCost <= Blood.Amount;
    }

    #endregion

    #region IJUicable

    public List<Renderer> GetRenderers()
    {
        return animController.Renderers;
    }

    #endregion
}
