using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Scriptable Objects/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    public float cameraLinearFollowSpeed;
    [Tooltip("Between 0 and 1, 1 is instant movement"), Range(0, 1)] 
    public float cameraGeometricFollowSpeed;
    public float cameraFollowDistance;
    public float cameraFOV;
}
