using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
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
    public GameRoom _currentRoom;

    public UnityEvent OnGameStart;
    public CameraController CameraController => _cameraController;

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
                case GameMode.Rooms:
                    RoomStart();
                    break;
            }
    }

    private void RoomStart(){
        Sequence spawnSeq = DOTween.Sequence();

        spawnSeq.AppendCallback( () => _currentRoom = Instantiate(_roomsList[0], Vector3.zero, Quaternion.identity));
        spawnSeq.AppendInterval(0.5F);
        spawnSeq.AppendCallback( () =>
        {
            RespawnPoint spawnPoint = _currentRoom.GetSpawnPoint();
            var riel = Instantiate(_rielPrefab, spawnPoint.transform.position, Quaternion.identity) as CharacterComponent;
            Riel = riel;
            _cameraController.SetTarget(Riel.transform);
            _cameraController.transform.position = spawnPoint.transform.position + new Vector3(0, 100, 0);
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
       if (_currentRoom.QueryRoomEnd())
        {
            // -- Todo : Next Room Logic.
        }
    }


    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
