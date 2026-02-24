using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public void Init(RoomSequencer sequencer, PathDirection entrance)
    {
        if (!OverrideRoomSequencer) customRoomSequencer = sequencer;
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

        HalfX = (int)Dimensions.x / 2;
        HalfY = (int)Dimensions.y / 2;

        //Logger.Core($"Generate Room with {HalfX} and {HalfY} and {Dimensions} ");

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
}
