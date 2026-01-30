using DG.Tweening;
using UnityEngine;

/// <summary>
/// Pilots other components, specifically translates inputs into I/O for attached controllers
/// </summary>
public class CharacterComponent : MonoBehaviour
{
	[SerializeField] private CharacterSetupData setupData;
    [SerializeField] private CameraController playerCameraController;
	private SkillsController skillsController;
	private MovementController movementController;
    private DamageController damageController;
    private AnimController animController;

    [SerializeField] private Blood blood;
    public static Blood Blood;

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
        movementController = GetComponent<MovementController>();
        movementController.Initialize(setupData, animController);
        damageController = GetComponent<DamageController>();
        damageController.Initialize(setupData, animController);


        // -- Setup Callback & Listeners
        damageController.OnDied.AddListener(() =>
        {
            animController.Die();   
            DOVirtual.DelayedCall(animController.ClipLength("Dying"), () => GameManager.Instance.ReloadCurrentScene());
        });

        InputManager.Instance.OnCharacterMovement.AddListener(OnInputVector);
        InputManager.Instance.OnCharacterSlot1.AddListener(() => skillsController.CallSkillStrategy(0));
        InputManager.Instance.OnCharacterSlot2.AddListener(() => skillsController.CallSkillStrategy(1));
        InputManager.Instance.OnCharacterSlot3.AddListener(() => skillsController.CallSkillStrategy(2));
        InputManager.Instance.OnCharacterSlot4.AddListener(() => skillsController.CallSkillStrategy(3));
        InputManager.Instance.OnCharacterSlot5.AddListener(() => skillsController.CallSkillStrategy(4));
    }

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
        Vector3 cameraVect = inputVector.x * playerCameraController.Right + inputVector.y * playerCameraController.Up;
        cameraVect.Normalize();
        rValue = Vector3.ProjectOnPlane(cameraVect, groundPlane.normal);
        return rValue;
    }

    #endregion
}
