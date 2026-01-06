using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] CameraController playerCameraController;
    [SerializeField] Rigidbody _rigidbody;
    private Vector2 _inputVector;
    private Vector3 _movementVector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance.OnCharacterMovement.AddListener(OnInputVectorChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnInputVectorChange(Vector2 newVector)
    {
        _inputVector = newVector;
        //Debug.Log("PlayerMovementController: OnInputVectorChange: " + _inputVector);
    }

    private Vector3 CharacterInputFromCameraPerspective() {
        Vector3 rValue;
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Vector3 cameraVect = _inputVector.x * playerCameraController.Right + _inputVector.y * playerCameraController.Up;
        cameraVect.Normalize();
        rValue = Vector3.ProjectOnPlane(cameraVect, groundPlane.normal);
        return rValue;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(transform.position, CharacterInputFromCameraPerspective() * 5);
    //}
}
