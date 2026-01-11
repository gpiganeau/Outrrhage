using UnityEngine;
using System.Collections;

public class MovementController: MonoBehaviour
{
    [SerializeField] protected Rigidbody _rigidbody;
    private Vector3 _preferedMovementDirection;
    private Vector3 _movementVector;
    private Vector3 _facingVector;
    private float _baseMovementSpeed;

    public void Initialize(ActorData data)
    {
        _baseMovementSpeed = data.movementSpeed;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _preferedMovementDirection = direction.normalized;
    }

    private void FixedUpdate()
    {
        UpdateMovementVector(_preferedMovementDirection  * _baseMovementSpeed);
        _rigidbody.MovePosition(_rigidbody.position + _movementVector * Time.fixedDeltaTime);
    }

    protected void UpdateMovementVector(Vector3 newMovementVector)
    {
        if(_movementVector.sqrMagnitude > 0)
        {
            _facingVector = newMovementVector.normalized;
        }
        _movementVector = newMovementVector;
    }

    /// <summary>
    /// Will update only the facing direction, is overwritten whenever a movement vector is updated. Use only when the actor is static, such as a turret.
    /// </summary>
    /// <param name="newFacingVector"></param>
    protected void UpdateFacingVector(Vector3 newFacingVector)
    {
        _facingVector = newFacingVector.normalized;
    }


    #region Movement Outputs

    public Vector3 GetActualMovement()
    {
        return _movementVector;
    }

    public Vector3 GetFacingDirection()
    {
        return _facingVector;
    }

    #endregion
}