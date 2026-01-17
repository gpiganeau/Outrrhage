using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private Standards standards;
    [SerializeField] private CameraSettings cameraSettings;
    [SerializeField] private GameplaySettings gameplaySettings;
    [SerializeField] private VisualSettings visualSettings;

    public Standards Standards => standards;
    public CameraSettings CameraSettings => cameraSettings;
    public GameplaySettings GameplaySettings => gameplaySettings;
    public VisualSettings VisualSettings => visualSettings;

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
