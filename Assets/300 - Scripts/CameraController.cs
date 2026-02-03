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

    private Vector3 targetPosition;

    public Vector3 Up => transform.up;
    public Vector3 Forward => transform.forward;
    public Vector3 Right => transform.right;

    private void Start()
    {
        if (_self == null)
        {
            _self = GetComponent<Camera>();
        }
        _self.fieldOfView = SettingsManager.Instance.CameraSettings.cameraFOV;
        Quaternion startRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(SettingsManager.Instance.CameraSettings.cameraAngleVert, SettingsManager.Instance.CameraSettings.cameraAngleSide, 0);
    }

    public void SetTarget(Transform newTarget) => target = newTarget;

    void Update()
    {
        if (target == null) return;

        targetPosition = target.position - transform.forward * SettingsManager.Instance.CameraSettings.cameraFollowDistance;
        switch (followMode)
        {
            case TargetFollowMode.Linear:
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, SettingsManager.Instance.CameraSettings.cameraLinearFollowSpeed * Time.deltaTime);
                break;
            case TargetFollowMode.Geometric:
                Vector3 futureMovement = (targetPosition - transform.position) * Mathf.Clamp01(SettingsManager.Instance.CameraSettings.cameraGeometricFollowSpeed);
                if (futureMovement.sqrMagnitude > 0.05f)
                {
                    transform.position += futureMovement * futureMovement.magnitude * Time.deltaTime;
                }
                break;
        }
        if (forceRefresh)
        {
            _self.fieldOfView = SettingsManager.Instance.CameraSettings.cameraFOV;
            Quaternion startRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(SettingsManager.Instance.CameraSettings.cameraAngleVert, SettingsManager.Instance.CameraSettings.cameraAngleSide, 0);
        }
    }
}
