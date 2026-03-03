using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Room Sequencer", menuName = "Scriptable Objects/Game/Room Sequencer")]
public class RoomSequencer : ScriptableObject
{

    [Header("Settings")]
    [Tooltip("Use this for testing custom enemy spawn")] public AIActorComponent customType;

    [Header("Core Events")]
    public UnityEvent OnRoomStart;
    public List<ChaosStep> RoomSequence = new List<ChaosStep>();
    public UnityEvent OnRoomComplete;

    [Tooltip("Room Auto Complete and spawn next on Sequence end")] public bool AutoComplete = false;
    
    #region Run Controls

    [HideInInspector] public GameRoom affectedRoom; 


    public void ZZ_SpawnCustomEnnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            EntityManager.Instance.SpawnCustomActor(customType, 8);
        }
    }

    public void ZZ_SpawnDrones(int count)
    {
        SpawnEnemies(EntityType.Drones, count);
    }

    public void ZZ_SpawnHumans(int count)
    {
        SpawnEnemies(EntityType.Humanoid, count);
    }

    public void ZZ_SpawnBull(int count)
    {
        SpawnEnemies(EntityType.Bull, count);
    }

    public void ZZ_SpawnTourelle(int count)
    {
        SpawnEnemies(EntityType.Tourelle, count);
    }

    private void SpawnEnemies(EntityType type, int count)
    {
        for (int i = 0; i < count; i++)
        {
            //EntityManager.Instance.SpawnEntities(type, 1, GameManager.Instance.Riel.transform.position, 8);
            EntityManager.Instance.SpawnEntities(type, 1, affectedRoom.GetRandomGeneratorPosition(), 8);
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

    public void ZZ_PlayRandomMusic()
    {
        AudioManager.Instance.PlayRandomMusic();
    }

    #endregion
}

