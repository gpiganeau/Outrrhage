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

    private void OpenEntrance(PathDirection direction)
    {
        // ── Centre du bon bord ────────────────────────────────────────────
        Vector3 origin = direction switch
        {
            PathDirection.North => transform.position + new Vector3(0f, 0f,  HalfY),
            PathDirection.South => transform.position + new Vector3(0f, 0f, -HalfY),
            PathDirection.East  => transform.position + new Vector3( HalfX, 0f, 0f),
            PathDirection.West  => transform.position + new Vector3(-HalfX, 0f, 0f),
            _ => transform.position
        };

        // ── SphereCast et destroy tout ce qui est sur le layer Wall ───────
        Collider[] hits = Physics.OverlapSphere(origin, WALL_SIZE, LayerMask.GetMask("Walls"));
        foreach (var hit in hits)
            Destroy(hit.gameObject);

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

            if (isWall && false)
            {
                if (x == 0 || y == 0) continue;

                // ── Skip si pas un multiple de WALL_SIZE (évite le Z-fight) ──
                bool isXWall = x == -HalfX || x == HalfX - 1;
                bool isYWall = y == -HalfY || y == HalfY - 1;

                if (isXWall && (y + HalfY) % WALL_SIZE != 0) continue;
                if (isYWall && (x + HalfX) % WALL_SIZE != 0) continue;

                // ── Centre du segment (décalage de WALL_SIZE/2) ───────────────
                Vector3 segmentOffset = isXWall
                    ? new Vector3(0f, 0f, (WALL_SIZE / 2f) - 0.5f)
                    : new Vector3((WALL_SIZE / 2f) - 0.5f, 0f, 0f);

                float wallRotation = isXWall ? 90f : 0f;

                GameObject wall = Instantiate(WallsPrefabs.Random(),
                                              (transform.position + spawnPosition + segmentOffset).WithY(-0.5f),
                                              Quaternion.Euler(0f, wallRotation, 0f));
                wall.transform.SetParent(_cellsElementHolder);
                roomCell.OwnedElement = wall;
                roomCell.Type = RoomCell.CellType.Wall;
            }
            else
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

            OpenEntrance(RoomEnter);
        }
    }
}
}