using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class MovementController: MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    private Vector3 _preferedMovementDirection;
    private Vector3 _movementVector;
    private Vector3 _facingVector;
    private float _baseMovementSpeed;

    private List<string> immobilizationSources;
    private Dictionary<string, float> speedAlterationSources = new Dictionary<string, float>();

    public void Initialize(ActorData data)
    {
        _baseMovementSpeed = data.movementSpeed;
        immobilizationSources = new List<string>();
        speedAlterationSources = new Dictionary<string, float>();
        speedAlterationSources["base"] = 1f;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _preferedMovementDirection = direction.normalized;
    }

    private void FixedUpdate()
    {
        if(immobilizationSources.Count > 0)       
            return;
        
        UpdateMovementVector(_preferedMovementDirection  * _baseMovementSpeed * ComputeAlteredSpeed());
        _rigidbody.MovePosition(_rigidbody.position + _movementVector * Time.fixedDeltaTime);
    }

    protected void UpdateMovementVector(Vector3 newMovementVector)
    {
        if(newMovementVector.sqrMagnitude > 0)
        {
            _facingVector = newMovementVector.normalized;
        }
        _movementVector = newMovementVector;
    }

    #region Modify Movement

    public void SetImmobilized(bool value, string source)
    {
        if(value)
            immobilizationSources.Add(source);
        else
        {
            if(immobilizationSources.Contains(source))
                immobilizationSources.Remove(source);
            else
                Debug.LogWarning($"Tried to remove immobilization source {source} which was not present.");
        }
    }

    public void SetSpeedAlteration(float alteration, string source)
    {
        speedAlterationSources[source] = alteration;
    }

    public float ComputeAlteredSpeed()
    {
        float finalAlteration = 1;
        foreach(float alteration in speedAlterationSources.Values)
        {
            finalAlteration *= alteration;
        }
        return finalAlteration;
    }

    #endregion

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