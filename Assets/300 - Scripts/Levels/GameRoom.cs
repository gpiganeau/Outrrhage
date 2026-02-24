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

    public bool IsProceduralRoom = false;
    public bool OverrideRoomSequencer = false;
    public RoomSequencer customRoomSequencer;

    public string GetRoomName() => _roomName;

    public List<GameObject> CorridorsPrefabs;
    public List<Transform> CorridorSpawnPoints;
    public GameRoom NextRoom;

    [SerializeField] RespawnPoint _spawnPoint;
    public RespawnPoint GetSpawnPoint() => _spawnPoint;

    [SerializeField] Transform _cellsElementHolder;

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

    public void RegenerateRoom()
    {
        if (!IsProceduralRoom) return;

        foreach (RoomCell c in _cells)
        {
            if (c.IsEmpty) continue;
            Destroy(c.OwnedElement);
        }

        GenerateRoomElements();
    }

    public Transform GetConnectionPoint()
    {
        return this.transform;
    }

    public void SpawnNextRoom()
    {
        var t = CorridorSpawnPoints.Random();
        var  nextPos = transform.position + t.position;
        Instantiate(CorridorsPrefabs.Random(), nextPos, Quaternion.identity);
        var dt =  nextPos + t.forward * 16;
        Instantiate(NextRoom, dt, Quaternion.identity);
    }


// -- Todo : MultiLayering (Obstacles, GPE, Props, Collectibles, Decorations, etc...)
// -- Todo : Delay routine for srtylisation 
    private void GenerateRoomElements()
    {

        HalfX = (int)Dimensions.x / 2;
        HalfY = (int)Dimensions.y / 2;

        Logger.Core($"Generate Room with {HalfX} and {HalfY} and {Dimensions} ");

        for (int x = -HalfX; x < HalfX; x++)
        {
            for (int y = -HalfY; y < HalfY; y++)
            {

                RoomCell roomCell = new RoomCell(x, y, null);
                _cells.Add(roomCell);

                if (Random.value < _spawnChancePercentage / 100f)
                {
                    Vector3 spawnPosition = new Vector3(x + 0.5f, _floorLevelOffset, y + 0.5f);;
                    GameObject element = Instantiate(_spawnableElements.Random(), transform.position + spawnPosition, Quaternion.identity);
                    element.transform.SetParent(_cellsElementHolder);
                    roomCell.OwnedElement = element;
                }
            }
        }
    }

    public bool QueryRoomEnd()
    {
        if (!_isCompleted) return false;
        if (EntityManager.Instance.Bots.Count > 0) return false;
        OnRoomComplete?.Invoke();
        return true;

    }
}
