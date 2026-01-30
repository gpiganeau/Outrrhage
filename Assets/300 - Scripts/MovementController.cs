using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.AI;

public class MovementController: MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    private Vector3 _preferedMovementDirection;
    private Vector3 _movementVector;
    private Vector3 _facingVector;
    private float _baseMovementSpeed;

    private List<string> immobilizationSources;
    private Dictionary<string, float> speedAlterationSources = new Dictionary<string, float>();

	AnimController animController;

    public void Initialize(ActorSetupData data, AnimController animController = null)
    {
        _facingVector = Vector3.forward;
        _baseMovementSpeed = data.movementSpeed;
        immobilizationSources = new List<string>();
        speedAlterationSources = new Dictionary<string, float>();
        speedAlterationSources["base"] = 1f;

        this.animController =animController;
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
        

        // -- Look Toward Movement
        if (_movementVector.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(_movementVector);
        }
        
        animController?.SetSpeed(_movementVector.magnitude > 0 ? 1 : 0);
    }

    protected void UpdateMovementVector(Vector3 newMovementVector)
    {
        //If enemy is static, we still want to update facing direction.
        if(_baseMovementSpeed == 0)
            _facingVector = _preferedMovementDirection.normalized;
        if (newMovementVector.sqrMagnitude > 0)
        {
            _facingVector = newMovementVector.normalized;
        }
        _movementVector = newMovementVector;
    }

    #region Movement Effects

    public void Dash(Vector3 direction, float dashDistance, float dashDuration, bool ignoreCollisions)
    {
        SetImmobilized(true, "Dash");
        Vector3 dashVector = direction.normalized * dashDistance;
        float timeScale = 1f;
        if(!ignoreCollisions){
            RaycastHit hit;
            if (Physics.Raycast(_rigidbody.position, dashVector, out hit, dashDistance))
            {
                timeScale = hit.distance / dashDistance;
                dashVector = direction.normalized * (hit.distance - 0.5f);
            }
            _rigidbody.DOMove(_rigidbody.position + dashVector, dashDuration * timeScale).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                SetImmobilized(false, "Dash");
            });
        }
        else
        {
            Vector3 destination = _rigidbody.position + dashVector;
            if(NavMesh.SamplePosition(destination, out NavMeshHit sampleHit, 0.5f, NavMesh.AllAreas))
                destination = sampleHit.position;
            else
                destination = ComputeFurthestPointAlongLine(_rigidbody.position, destination, 0.5f);
            
            dashVector = destination - _rigidbody.position;
            _rigidbody.detectCollisions = false;
            _rigidbody.DOMove(_rigidbody.position + dashVector, dashDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _rigidbody.detectCollisions = true;
                SetImmobilized(false, "Dash");
            });
        }
    }

    private Vector3 ComputeFurthestPointAlongLine(Vector3 origin, Vector3 finalPosition, float radius)
    {
        float distance = Vector3.Distance(origin, finalPosition);
        Vector3 line = finalPosition - origin;
        while (distance > 0)
        {
            Vector3 checkPoint = origin + line.normalized * distance;
            if (NavMesh.SamplePosition(checkPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            distance -= radius;
        }
        return origin;
    }

    #endregion

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
                Logger.LogWarning(Logger.LogCategory.Core, $"Tried to remove immobilization source {source} which was not present.");
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

    public bool IsMoving()
    {
        return _movementVector.magnitude > 0;
    }

    public Vector3 GetFacingDirection()
    {
        return _facingVector;
    }

    #endregion
}