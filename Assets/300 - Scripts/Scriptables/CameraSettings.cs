using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Scriptable Objects/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    public float cameraLinearFollowSpeed;
    public float cameraQuadraticFollowSpeed;
    public float cameraFollowDistance;
    public float cameraFOV;
}
