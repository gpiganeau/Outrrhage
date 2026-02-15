using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    enum TargetFollowMode
    {
        Linear,
        Geometric,
    }
    [SerializeField] private TargetFollowMode followMode;
    [SerializeField] private Camera _self;
    [SerializeField] private Transform target;
    [SerializeField] private bool forceRefresh = false;
    
    [Header("Transition Settings")]
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private Ease _transitionEase = Ease.OutCubic;
    
    private MovementController targetMovementController;
    private bool usePrediction;
    private Vector3 targetPosition;
    
    public Vector3 Up => transform.up;
    public Vector3 Forward => transform.forward;
    public Vector3 Right => transform.right;
    
    private CameraSettings defaultSettings;
    
    #region Runtime Tweener Values
    private float _currentFollowDistance;
    private float _currentPredictionRatio;
    private float _currentLinearFollowSpeed;
    private float _currentGeometricFollowSpeed;
    
    private Sequence settingsTweenSequence;

    #endregion

    private void Start()
    {
        if (_self == null)
        {
            _self = GetComponent<Camera>();
        }
        defaultSettings = SettingsManager.Instance.CameraSettings;
        
        InitializeRuntimeValues(defaultSettings);
        ResetCameraSettings(0f); 
    }

    private void InitializeRuntimeValues(CameraSettings settings)
    {
        _currentFollowDistance = settings.cameraFollowDistance;
        _currentPredictionRatio = settings.cameraPredictionRatio;
        _currentLinearFollowSpeed = settings.cameraLinearFollowSpeed;
        _currentGeometricFollowSpeed = settings.cameraGeometricFollowSpeed;
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (newTarget.TryGetComponent(out targetMovementController))
        {
            usePrediction = true;
        }
        else
            usePrediction = false;
    }
    
    public void SetCameraSettings(CameraSettings cameraSettings, float? customDuration = null)
    {
        float duration = customDuration ?? _transitionDuration;
        TweenToSettings(cameraSettings, duration);
    }
    
    public void ResetCameraSettings(float? customDuration = null)
    {
        float duration = customDuration ?? _transitionDuration;
        TweenToSettings(defaultSettings, duration);
    }
    
    private void TweenToSettings(CameraSettings newSettings, float duration)
    {
        settingsTweenSequence?.Kill();
        settingsTweenSequence = DOTween.Sequence();
            
        // FOV
        settingsTweenSequence.Join(
            DOTween.To(() => _self.fieldOfView, x => _self.fieldOfView = x, newSettings.cameraFOV, duration)
        );
    
        // Rotation
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(newSettings.cameraAngleVert, newSettings.cameraAngleSide, 0);
        settingsTweenSequence.Join(
            DOTween.To(() => 0f, x => transform.rotation = Quaternion.Slerp(startRotation, targetRotation, x), 1f, duration)
        );

        // Follow Distance
        settingsTweenSequence.Join(
            DOTween.To(() => _currentFollowDistance, 
                x => _currentFollowDistance = x, 
                newSettings.cameraFollowDistance, 
                duration)
        );

        // Prediction Ratio
        settingsTweenSequence.Join(
            DOTween.To(() => _currentPredictionRatio, 
                x => _currentPredictionRatio = x, 
                newSettings.cameraPredictionRatio, 
                duration)
        );

        // Linear Follow Speed
        settingsTweenSequence.Join(
            DOTween.To(() => _currentLinearFollowSpeed, 
                x => _currentLinearFollowSpeed = x, 
                newSettings.cameraLinearFollowSpeed, 
                duration)
        );

        // Geometric Follow Speed
        settingsTweenSequence.Join(
            DOTween.To(() => _currentGeometricFollowSpeed, 
                x => _currentGeometricFollowSpeed = x, 
                newSettings.cameraGeometricFollowSpeed, 
                duration)
        );

        settingsTweenSequence.SetEase(_transitionEase);
        settingsTweenSequence.OnComplete(() => 
        {
            SettingsManager.Instance.SetCameraSettings(newSettings);
        });
    }
    
    private void RecalculateFOVAndRotation()
    {
        var settings = SettingsManager.Instance.CameraSettings;
        _self.fieldOfView = settings.cameraFOV;
        transform.rotation = Quaternion.Euler(settings.cameraAngleVert, settings.cameraAngleSide, 0);
    }
    
    void Update()
    {
        if (target == null) return;
        
        targetPosition = target.position - transform.forward * _currentFollowDistance;
        
        if(usePrediction)
        {
            targetPosition += targetMovementController.Velocity * _currentPredictionRatio;
        }
        
        switch (followMode)
        {
            case TargetFollowMode.Linear:
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _currentLinearFollowSpeed * Time.deltaTime);
                break;
            case TargetFollowMode.Geometric:
                Vector3 futureMovement = (targetPosition - transform.position) * Mathf.Clamp01(_currentGeometricFollowSpeed);
                if (futureMovement.sqrMagnitude > 0.05f)
                {
                    transform.position += futureMovement * Time.deltaTime;
                }
                break;
        }
        
        if (forceRefresh)
        {
           RecalculateFOVAndRotation();
        }
    }

    private void OnDestroy()
    {
        settingsTweenSequence?.Kill();
    }
}