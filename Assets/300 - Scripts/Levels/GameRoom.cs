using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameRoom : MonoBehaviour
{

    [SerializeField] string _roomName;
    [TextArea] public string _roomDescription;

    public string GetRoomName() => _roomName;

    [SerializeField] RespawnPoint _spawnPoint;
    public RespawnPoint GetSpawnPoint() => _spawnPoint;

    public List<ChaosStep> RoomSequence = new List<ChaosStep>();

    [SerializeField] Transform _spawnPointHolder;

    public UnityEvent OnRoomStart;
    public UnityEvent OnRoomComplete;

    void Awake()
    {
        if (_spawnPoint == null) _spawnPoint = GetComponentInChildren<RespawnPoint>();
    }

    void Start()
    {
        StartCoroutine(RoomSeq());
    }

    IEnumerator RoomSeq()
    {
        OnRoomStart?.Invoke();

        foreach (var step in RoomSequence)
        {
            if (step.delay > 0)
            {
                yield return new WaitForSeconds(step.delay);
            }

            if (step.logEvent)
            {
                Logger.Log(Logger.LogCategory.Core, $"[DesignerChaos] Step Triggered: {step.stepName}");
            }
            
            step.stepEvent?.Invoke();

        }
    }

    #region Room Controls

    public void ZZ_SpawnDrones(int count)
    {
        SpawnEnemies(EntityType.Drones, count);
    }

    public void ZZ_SpawnHumans(int count)
    {
        SpawnEnemies(EntityType.Humanoid, count);
    }

    private void SpawnEnemies(EntityType type, int count)
    {
        for (int i = 0; i < count; i++)
        {   
            Vector3 randomPos = _spawnPointHolder.GetChild(Random.Range(0, _spawnPointHolder.childCount)).position;
            EntityManager.Instance.SpawnEntities(type, 1, randomPos, 0);
        }
    }

    public void ZZ_ChangeCameraSetting(CameraSettings cameraSettings)
    {
        GameManager.Instance.CameraController.SetCameraSettings(cameraSettings);
    }

    public void ZZ_ResetCameraSetting()
    {
        GameManager.Instance.CameraController.ResetCameraSettings();
    }

    public void ZZ_KillAllEnemies()
    {
        foreach (var bot in EntityManager.Instance.Bots)
        {
            bot.ForceKill();
        }
    }

    public void ZZ_HideHUD()
    {
        HUD.Instance.Hide();
    }

    public void ZZ_ShowHUD()
    {
        HUD.Instance.Show();
    }

    public void ZZ_DisablePlayerControl()
    {
        GameManager.Instance.Riel.DisableControls();
    }

    public void ZZ_EnablePlayerControl()
    {
        GameManager.Instance.Riel.EnableControls();
    }

    #endregion
}
