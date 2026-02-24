using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameMode { Forest, Rooms }

    public GameMode CurrentGameMode = GameMode.Rooms;

    [HideInInspector] public CharacterComponent Riel;

    [Header("Core Prefabs References")]
    public CharacterComponent _rielPrefab;
    public Level _startLevelPrefab;

    public List<GameRoom> _roomsList;

    [Header("Managers References")]
    [SerializeField] CameraController _cameraController;

    [Header("Readonly References for Debug")]
    public Level _currentLevel;
    [SerializeField] private RunSystemController runController;

    public UnityEvent OnGameStart;
    public CameraController CameraController => _cameraController;

    private static bool _gameOver = false;
    public static bool GameOver { get => _gameOver; set => _gameOver = value;}

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void Start(){
            switch (CurrentGameMode)
            {
                case GameMode.Forest:
                    ForestStart();
                    break;
            }
    }

    public void RoomStart(Vector3 rielSpawnPos){
        Sequence spawnSeq = DOTween.Sequence();

        spawnSeq.AppendCallback( () =>
        {
            var riel = Instantiate(_rielPrefab, rielSpawnPos, Quaternion.identity);
            Riel = riel;
            _cameraController.SetTarget(Riel.transform);
            _cameraController.transform.position = riel.transform.position.WithY(100);
            riel.PlayerCameraController = _cameraController;
            EntityManager.Instance.Riel = riel;
            OnGameStart.Invoke();
        });
    }

    private void ForestStart()
    {
        Sequence spawnSeq = DOTween.Sequence();
        
        spawnSeq.AppendCallback( () => _currentLevel = Instantiate(_startLevelPrefab, Vector3.zero, Quaternion.identity));
        spawnSeq.AppendInterval(0.5F);
        spawnSeq.AppendCallback( () =>
        {
            RespawnPoint spawnPoint = _currentLevel.GetSpawnPoint();
            var riel = Instantiate(_rielPrefab, spawnPoint.transform.position, Quaternion.identity) as CharacterComponent;
            Riel = riel;
            _cameraController.SetTarget(Riel.transform);
            _cameraController.transform.position = spawnPoint.transform.position + new Vector3(0, 100, 0);
            riel.PlayerCameraController = _cameraController;
            EntityManager.Instance.Riel = riel;
            OnGameStart.Invoke();

        });
        spawnSeq.Play();
    }

    public void CheckForRoomEnd()
    {
       RunSystemController.Instance.QueryRoomEnd();
    }


    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    internal void TriggerGameEnd()
    {
        Logger.Core("GAME END");
    }
}
