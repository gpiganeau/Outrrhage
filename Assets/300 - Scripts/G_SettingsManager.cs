using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private Standards standards;
    [SerializeField] private MovementSettings movementSettings;
    [SerializeField] private CameraSettings cameraSettings;

    public Standards Standards => standards;
    public MovementSettings MovementSettings => movementSettings;
    public CameraSettings CameraSettings => cameraSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
