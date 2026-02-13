using UnityEngine;

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

    private MovementController targetMovementController;
    private bool usePrediction;

    private Vector3 targetPosition;

    public Vector3 Up => transform.up;
    public Vector3 Forward => transform.forward;
    public Vector3 Right => transform.right;

    private CameraSettings defaultSettings;

    private void Start()
    {
        if (_self == null)
        {
            _self = GetComponent<Camera>();
        }

        defaultSettings = SettingsManager.Instance.CameraSettings;
        ResetCameraSettings();
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

    public void SetCameraSettings(CameraSettings cameraSettings)
    {
        SettingsManager.Instance.SetCameraSettings(cameraSettings);
        RecalculateFOVAndRotation();
    }
    public void ResetCameraSettings()
    {
        SettingsManager.Instance.SetCameraSettings(defaultSettings);
        RecalculateFOVAndRotation();
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

        var settings = SettingsManager.Instance.CameraSettings;

        targetPosition = target.position - transform.forward * settings.cameraFollowDistance;
        if(usePrediction)
        {
            targetPosition += targetMovementController.Velocity 
                * settings.cameraPredictionRatio;
        }
        switch (followMode)
        {
            case TargetFollowMode.Linear:
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, settings.cameraLinearFollowSpeed * Time.deltaTime);
                break;
            case TargetFollowMode.Geometric:

                Vector3 futureMovement = (targetPosition - transform.position) * Mathf.Clamp01(settings.cameraGeometricFollowSpeed);
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
}
