using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameRoom : MonoBehaviour
{
    [Header("Room Settings")]
    [TextArea] public string _roomDescription;
    [SerializeField] List<GameObject> _spawnableElements = new List<GameObject>();
    [SerializeField, Range (-2f, 2f)] float _floorLevelOffset = 1f;

    public PathDirection RoomEnter;
    public PathDirection RoomExit;

    public bool IsProceduralRoom = false;
    public bool OverrideRoomSequencer = false;
    public RoomSequencer customRoomSequencer;

    public List<GameObject> CorridorsPrefabs;
    [SerializeField] RespawnPoint _spawnPoint;
    public RespawnPoint GetSpawnPoint() => _spawnPoint;

    [SerializeField] Transform _cellsElementHolder;


    const int WALL_SIZE = 4;


    [Header("Populated by Run Settings")]
    public List<GameObject> WallsPrefabs;

    [Header("Cells")]
    [SerializeField] private List<RoomCell> _cells = new List<RoomCell>();
    [Range(0, 100)] public float _spawnChancePercentage = 2;

     private readonly Vector2 Dimensions = new Vector2(32, 32);
    private const int ROOM_SIZE = 32;

    private int HalfX, HalfY;

    void Awake()
    {
        if (_spawnPoint == null) _spawnPoint = GetComponentInChildren<RespawnPoint>();
    }

    public void Init(RoomSequencer sequencer, PathDirection entrance, List<GameObject> walls = null)
    {
        if (!OverrideRoomSequencer) customRoomSequencer = sequencer;
        WallsPrefabs = walls;
        RoomEnter = entrance;
        RegenerateRoom();
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
        return null;
    }


    // -- Todo : MultiLayering (Obstacles, GPE, Props, Collectibles, Decorations, etc...)
    // -- Todo : Delay routine for srtylisation 
    private void GenerateRoomElements()
{
    _cells.Clear();
    HalfX = (int)Dimensions.x / 2;
    HalfY = (int)Dimensions.y / 2;

    for (int x = -HalfX; x < HalfX; x++)
    {
        for (int y = -HalfY; y < HalfY; y++)
        {
            RoomCell roomCell = new RoomCell(x, y, null);
            _cells.Add(roomCell);
            Vector3 spawnPosition = new Vector3(x + 0.5f, _floorLevelOffset, y + 0.5f);
            bool isWall = x == -HalfX || x == HalfX - 1 || y == -HalfY || y == HalfY - 1;

            {
                bool nearWall = x == -HalfX + 1 || x == HalfX - 2
                             || y == -HalfY + 1 || y == HalfY - 2;
                if (nearWall) continue;

                if (Random.value < _spawnChancePercentage / 100f)
                {
                    GameObject element = Instantiate(_spawnableElements.Random(),
                                                     transform.position + spawnPosition,
                                                     Quaternion.identity);
                    element.transform.SetParent(_cellsElementHolder);
                    roomCell.OwnedElement = element;
                }
            }

        }
    }
}
}