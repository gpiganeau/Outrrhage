using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.AI;
using System;

public class MovementController: MonoBehaviour, IAnimatable
{
    [SerializeField] private Rigidbody _rigidbody;
    private Vector3 _preferedMovementDirection;
    private Vector3 _movementVector;
    private Vector3 _facingVector;
    private float _baseMovementSpeed;

    private bool _isAiming;
    private AimPreviewController _cachedAimPreviewController;

    private List<string> immobilizationSources;
    private Dictionary<string, float> speedAlterationSources = new Dictionary<string, float>();

	AnimController animController;

    public Vector3 Velocity =>  _preferedMovementDirection * _baseMovementSpeed * ComputeSpeedAlteration();  

    public void Initialize(ActorSetupData data, AnimController animController = null)
    {
        _facingVector = Vector3.forward;
        _baseMovementSpeed = data.movementSpeed;
        immobilizationSources = new List<string>();
        speedAlterationSources = new Dictionary<string, float>();
        speedAlterationSources["base"] = 1f;

        this.animController = animController;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _preferedMovementDirection = direction.normalized;
        if (_isAiming && _cachedAimPreviewController != null)
        {
           _cachedAimPreviewController.UpdatePreviewMovement(_preferedMovementDirection);
        }
    }

    public void SetFacingDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
            _facingVector = direction.normalized;
    }

    private void FixedUpdate()
    {
        if(immobilizationSources.Count > 0){   
            _rigidbody.linearVelocity = Vector3.zero; // ← stop le glissement
            return;
        }

        if(_isAiming)
        {
            UpdateMovementVector(Vector3.zero);
            _rigidbody.MoveRotation(Quaternion.LookRotation(_facingVector));
            return;
        }

        UpdateMovementVector(_preferedMovementDirection  * _baseMovementSpeed * ComputeSpeedAlteration());
        _rigidbody.MovePosition(_rigidbody.position + _movementVector * Time.fixedDeltaTime);
        if(_movementVector.sqrMagnitude == 0 && _facingVector.sqrMagnitude > 0)
            _rigidbody.MoveRotation(Quaternion.LookRotation(_facingVector));

        // -- Look Toward Movement
        if (_movementVector.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(_movementVector);
        }
        
        animController?.SetSpeed(_movementVector.magnitude > 0 ? 1 : 0);

        if (_movementVector.sqrMagnitude == 0)
            _rigidbody.linearVelocity = Vector3.zero; // ← stop le glissement

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

    #region Immobilize for aim or channel

    public void StartAimingMode(PreviewData data)
    {
        _isAiming = true;
        if(_cachedAimPreviewController != null)
        {
            _cachedAimPreviewController.StartPreview(data);
        }
        else
        {
            _cachedAimPreviewController = GetComponentInChildren<AimPreviewController>();
            if(_cachedAimPreviewController != null)
            {
                _cachedAimPreviewController.StartPreview(data);
            }
        }
    }

    public void StopAimingMode()
    {
        _isAiming = false;
        if(_cachedAimPreviewController != null)
        {
            _cachedAimPreviewController.HidePreview();
        }
    }

    public void PlayExplosionEffect(Action onComplete = null)
    {
        _cachedAimPreviewController?.PlayExplosionEffect(onComplete);
    }

    #endregion

    #region Movement Effects

    public void Dash(Vector3 direction, float dashDistance, float dashDuration, bool ignoreCollisions, System.Action onComplete = null)
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
                onComplete?.Invoke();
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
                onComplete?.Invoke();
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

    public float ComputeSpeedAlteration()
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

    public Vector3 GetAimPosition()
    {
        return _cachedAimPreviewController != null 
            ? _cachedAimPreviewController.AimPosition 
            : transform.position;
    }

    #endregion

    #region  Animatable Implementation
    public AnimController AnimController => animController;
    #endregion
}