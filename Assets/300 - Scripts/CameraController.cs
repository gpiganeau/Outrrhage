using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum TargetFollowMode
    {
        Linear,
        Quadatratic,
    }

    [SerializeField] private TargetFollowMode followMode;
    [SerializeField] private Camera _self;
    [SerializeField] private Transform target;

    private Vector3 targetPosition;

    public Vector3 Up => transform.up;
    public Vector3 Forward => transform.forward;
    public Vector3 Right => transform.right;

    void Update()
    {
        targetPosition = target.position - transform.forward * SettingsManager.Instance.CameraSettings.cameraFollowDistance;
        switch (followMode)
        {
            case TargetFollowMode.Linear:
                transform.position = Vector3.Lerp(transform.position, targetPosition, SettingsManager.Instance.CameraSettings.cameraLinearFollowSpeed * Time.deltaTime);
                break;
            case TargetFollowMode.Quadatratic:
                Vector3 futureMovement = (targetPosition - transform.position) * Mathf.Clamp01(SettingsManager.Instance.CameraSettings.cameraQuadraticFollowSpeed);
                if (futureMovement.sqrMagnitude > 0.05f)
                {
                    transform.position += futureMovement * futureMovement.magnitude * Time.deltaTime;
                }
                break;
        }
    }
}
