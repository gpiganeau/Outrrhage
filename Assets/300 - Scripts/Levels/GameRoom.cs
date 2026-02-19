using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameRoom : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] string _roomName;
    [TextArea] public string _roomDescription;
    [SerializeField] List<GameObject> _spawnableElements = new List<GameObject>();
    [SerializeField, Range (-2f, 2f)] float _floorLevelOffset = 1f;

    public string GetRoomName() => _roomName;

    [SerializeField] RespawnPoint _spawnPoint;
    public RespawnPoint GetSpawnPoint() => _spawnPoint;

    public List<ChaosStep> RoomSequence = new List<ChaosStep>();

    [SerializeField] Transform _spawnPointHolder;
    [SerializeField] Transform _cellsElementHolder;

    public UnityEvent OnRoomStart;
    public UnityEvent OnRoomComplete;

    [Header("Cells")]
    [SerializeField] private List<RoomCell> _cells = new List<RoomCell>();
    private Vector2 _dimensions = new Vector2(32, 32);
    public Vector2 Dimensions => _dimensions;
    [Range(0, 100)] public float _spawnChancePercentage = 2;

    bool _isCompleted = false;

    private int HalfX, HalfY;

    void Awake()
    {
        if (_spawnPoint == null) _spawnPoint = GetComponentInChildren<RespawnPoint>();
    }

    void Start()
    {
        
        HalfX = (int)Dimensions.x / 2;
        HalfY = (int)Dimensions.y / 2;

        OnRoomStart.AddListener(GenerateLevel);

        StartCoroutine(RoomSeq());
    }

    public void RegenerateLevel()
    {
        // -- Cleanup
        foreach (RoomCell c in _cells)
        {
            if (c.IsEmpty) continue;
            Destroy(c.OwnedElement);
        }

        GenerateLevel();
    }


// -- Todo : MultiLayering (Obstacles, GPE, Props, Collectibles, Decorations, etc...)
// -- Todo : Delay routine for srtylisation 
    private void GenerateLevel()
    {
        for (int x = -HalfX; x < HalfX; x++)
        {
            for (int y = -HalfY; y < HalfY; y++)
            {
    
                RoomCell roomCell = new RoomCell(x, y, null);
                _cells.Add(roomCell);

                // -- 10% chance to spawn an element in the cell
                if (Random.value < _spawnChancePercentage / 100f)
                {
                    Vector3 spawnPosition = new Vector3(x + 0.5f, _floorLevelOffset, y + 0.5f);;
                    GameObject element = Instantiate(_spawnableElements.Random(), spawnPosition, Quaternion.identity);
                    element.transform.position = spawnPosition;
                    element.transform.SetParent(_cellsElementHolder);
                    roomCell.OwnedElement = element;
                }

            
            }
        }
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

        _isCompleted = true;

    }

    public bool QueryRoomEnd()
    {
        if (!_isCompleted) return false;
        if (EntityManager.Instance.Bots.Count > 0) return false;
        OnRoomComplete?.Invoke();
        return true;

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
            randomPos.y = 1f;
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
