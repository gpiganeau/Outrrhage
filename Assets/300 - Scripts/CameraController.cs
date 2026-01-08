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
    }

    void Update()
    {
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
    }
}
