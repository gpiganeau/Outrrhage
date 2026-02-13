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
        _self.fieldOfView = cameraSettings.cameraFOV;
        transform.rotation = Quaternion.Euler(cameraSettings.cameraAngleVert, cameraSettings.cameraAngleSide, 0);
    }
    public void ResetCameraSettings()
    {
        SettingsManager.Instance.SetCameraSettings(defaultSettings);
        _self.fieldOfView = SettingsManager.Instance.CameraSettings.cameraFOV;
        transform.rotation = Quaternion.Euler(SettingsManager.Instance.CameraSettings.cameraAngleVert, SettingsManager.Instance.CameraSettings.cameraAngleSide, 0);
    }

    void Update()
    {
        if (target == null) return; // -- Get une target 

        targetPosition = target.position - transform.forward * SettingsManager.Instance.CameraSettings.cameraFollowDistance;
        if(usePrediction)
        {
            targetPosition += targetMovementController.Velocity 
                * SettingsManager.Instance.CameraSettings.cameraPredictionRatio;
        }
        switch (followMode)
        {
            case TargetFollowMode.Linear:
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, SettingsManager.Instance.CameraSettings.cameraLinearFollowSpeed * Time.deltaTime);
                break;
            case TargetFollowMode.Geometric:

                Vector3 futureMovement = (targetPosition - transform.position) * Mathf.Clamp01(SettingsManager.Instance.CameraSettings.cameraGeometricFollowSpeed);
                if (futureMovement.sqrMagnitude > 0.05f)
                {
                    transform.position += futureMovement * Time.deltaTime;
                }
                break;
        }
        if (forceRefresh)
        {
           ResetCameraSettings();
        }
    }
}
