using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Scriptable Objects/Settings/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    public float cameraLinearFollowSpeed;
    [Tooltip("Between 0 and 1, 1 is instant movement"), Range(0, 1)] 
    public float cameraGeometricFollowSpeed;
    public float cameraFollowDistance;
    [Range(1, 5)] public float cameraPredictionRatio;
    [Range(0, 90)] public float cameraAngleVert;
    [Range(-90, 90)] public float cameraAngleSide;
    public float cameraFOV;
}
