using UnityEngine;
using System.Collections;


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

    // Use this for initialization
    void Start()
	{
        skillsController = GetComponent<SkillsController>();
        skillsController.Initialize(setupData);
        movementController = GetComponent<MovementController>();
        movementController.Initialize(setupData);
        damageController = GetComponent<DamageController>();
        damageController.Initialize(setupData);


        InputManager.Instance.OnCharacterMovement.AddListener(OnInputVector);
        InputManager.Instance.OnCharacterSlot1.AddListener(() => skillsController.CallSkillStrategy(0));
        InputManager.Instance.OnCharacterSlot2.AddListener(() => skillsController.CallSkillStrategy(1));
        InputManager.Instance.OnCharacterSlot3.AddListener(() => skillsController.CallSkillStrategy(2));
        InputManager.Instance.OnCharacterSlot4.AddListener(() => skillsController.CallSkillStrategy(3));
        InputManager.Instance.OnCharacterSlot5.AddListener(() => skillsController.CallSkillStrategy(4));
    }

	// Update is called once per frame
	void Update()
	{

	}

    #region Input Handling

    void OnInputVector(Vector2 newVector)
    {
        movementController.SetMovementDirection(CharacterInputFromCameraPerspective(newVector));
        //Debug.Log("PlayerMovementController: OnInputVector: " + newVector);
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
