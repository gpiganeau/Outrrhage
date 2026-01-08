using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    [SerializeField] CameraController playerCameraController;
    [SerializeField] Rigidbody _rigidbody;
    private Vector2 _inputVector;
    //the direction the movement wants to take the character in world space
    private Vector3 _inputMovementVector;
    //The same vector but shifted by inertia
    private Vector3 _movementVector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance.OnCharacterMovement.AddListener(OnInputVector);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _inputMovementVector = CharacterInputFromCameraPerspective() * SettingsManager.Instance.MovementSettings.characterSpeed;
        if(SettingsManager.Instance.MovementSettings.timeToReverse == 0)
        {
            _movementVector = _inputMovementVector;
        }
        else
        {
            _movementVector = Vector3.MoveTowards(_movementVector, _inputMovementVector, 
                2 * SettingsManager.Instance.MovementSettings.characterSpeed * Time.fixedDeltaTime / SettingsManager.Instance.MovementSettings.timeToReverse);
        }
        _rigidbody.MovePosition(_rigidbody.position + _movementVector * Time.fixedDeltaTime);
    }

    void OnInputVector(Vector2 newVector)
    {
        _inputVector = newVector;
        //Debug.Log("PlayerMovementController: OnInputVector: " + _inputVector);
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

    #region Movement Outputs

    public Vector3 GetActualMovement()
    {
        return _rigidbody.linearVelocity;
    }

    public Vector3 GetInputMovementVector()
    {
        return _movementVector;
    }


    #endregion
}
