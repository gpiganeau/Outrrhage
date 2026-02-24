using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

public enum PathDirection { North, East, South, West }

public class RunSystemController : MonoBehaviour
{
    [SerializeField] private RunSettings runSettings;
    private const int ROOM_SIZE = 32;

    public static RunSystemController Instance;

    private Vector2Int ToGrid(PathDirection dir)
    {
        return dir switch
        {
            PathDirection.North => Vector2Int.up,
            PathDirection.East  => Vector2Int.right,
            PathDirection.South => Vector2Int.down,
            PathDirection.West  => Vector2Int.left,
            _ => Vector2Int.zero
        };
    }

    public static Vector3 GetDirection(PathDirection direction)
    {
        return direction switch
        {
            PathDirection.North => Vector3.forward,
            PathDirection.East => Vector3.right,
            PathDirection.South => -Vector3.forward,
            PathDirection.West => -Vector3.right,
            _ => throw new System.NotImplementedException(),
        };
    }

    [Header("Debug")]
    [SerializeField] private int _roomCount;
    [SerializeField] private GameRoom _hubRoom;
    [SerializeField] private List<GameRoom> _pathRooms;
    [SerializeField] private GameRoom _bossRoom;
    [SerializeField] private GameRoom _currentRoom;
    [SerializeField] private List<PathDirection> _pathDirections;

    [SerializeField] private List<Vector3> _spawnPositions;

    public UnityEvent OnRoomComplete = new();

    public GameRoom CurrentRoom => _currentRoom;
    private int roomIndex = 0;
    bool _currentRoomSequenceOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GenerateCriticalPath();
        StartRun();
    }

    private void GenerateCriticalPath ()
    {

        _pathRooms.Clear();
        
        RunSettings s = runSettings;

        _roomCount = s.RoomCount;
        
        // -- Select HUB Room
        _hubRoom = s.HUBRooms.Random();
        
        // -- Select Boss Room
        _bossRoom = s.BossRooms.Random();

        // -- Select Normals Rooms

        List<GameRoom> availablesRooms = new();
        foreach (var r in s.NormalRooms) availablesRooms.Add(r);
        

        for (int i = 0; i < _roomCount; i++)
        {
            var nextRoom = availablesRooms.Random();
            if (nextRoom == null)
            {
                _pathRooms.Add(_pathRooms[i - 1]);
            } else
            {
                _pathRooms.Add(nextRoom);
                availablesRooms.Remove(nextRoom);
            }
        }

        GenerateDirectionPath();

        // -- Get All Spawn Positions 
        var currentPos = transform.position;
        _spawnPositions.Add(currentPos);

        for (int i = 1; i <= runSettings.RoomCount; i++)
        {
            // -- Last Room for Boss
            if (i == runSettings.RoomCount)
            {
                PathDirection lastDir = _pathDirections.Last();
                currentPos += GetDirection(lastDir) * ROOM_SIZE;
                _spawnPositions.Add(currentPos);
                break;
            }

            PathDirection dir = _pathDirections[i - 1];
            currentPos += GetDirection(dir) * ROOM_SIZE;
            _spawnPositions.Add(currentPos);
        }

        void GenerateDirectionPath()
        {
            _pathDirections.Clear();

            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Vector2Int current = Vector2Int.zero;
            visited.Add(current);
        
            List<PathDirection> allDirections = new List<PathDirection> 
            { 
                PathDirection.North, PathDirection.East, 
                PathDirection.South, PathDirection.West 
            };

            for (int i = 0; i <= _roomCount; i++)
            {
                List<PathDirection> available = new List<PathDirection>();
                foreach (var dir in allDirections)
                {
                    Vector2Int next = current + ToGrid(dir);
                    if (!visited.Contains(next))
                        available.Add(dir);
                }
                
                if (available.Count == 0)
                {
                    Logger.Core($"[RunSystem] Dead end at step {i}, forcing random direction");
                    _pathDirections.Add(allDirections.Random());
                    break;
                }
                
                PathDirection chosen = available.Random();
                _pathDirections.Add(chosen);
                current += ToGrid(chosen);
                visited.Add(current);
            }
        }
    }
    IEnumerator StartRoomSequence (RoomSequencer seq)
    {
        seq.OnRoomStart?.Invoke();
        _currentRoomSequenceOver = false;

        foreach (var step in seq.RoomSequence)
        {

            if (step.skipEvent)
            {
                continue;
            } 
            else
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

        _currentRoomSequenceOver = true;
    }

    public bool QueryRoomEnd()
    {
        if (!_currentRoomSequenceOver) return false;
        if (EntityManager.Instance.Bots.Count > 0) return false;
        _currentRoom.customRoomSequencer.OnRoomComplete?.Invoke();
        OnRoomComplete?.Invoke();
        return true;
    }

    private void StartRun()
    {
        CreateRoom(_hubRoom, _spawnPositions[roomIndex], runSettings.Sequencers[roomIndex], PathDirection.South);
    }

    private void CreateRoom(GameRoom room, Vector3 pos, RoomSequencer sequencer, PathDirection entrance)
    {
        _currentRoom = Instantiate(room, pos, Quaternion.identity);
        _currentRoom.Init(sequencer, entrance);
        StartCoroutine(StartRoomSequence(_currentRoom.customRoomSequencer));
        roomIndex++;
    }

    void OnValidate()
    {
        GenerateCriticalPath();
    }

    void OnDrawGizmos()
    {
        Vector3 roomSizeVec = new Vector3(ROOM_SIZE, 1, ROOM_SIZE);

        // -- Hub Room
        Gizmos.color = Color.rebeccaPurple;
        Gizmos.DrawCube(transform.position, roomSizeVec);

        Vector3 currentPos = transform.position;

        // -- Path Room
        Gizmos.color = Color.cyan;

        for (int i = 1; i <= runSettings.RoomCount; i++)
        {
            // -- Last Room for Boss
            if (i == runSettings.RoomCount)
            {
                Gizmos.color = Color.yellowNice;
                PathDirection lastDir = _pathDirections.Last();
                currentPos += GetDirection(lastDir) * ROOM_SIZE;
                Gizmos.DrawCube(currentPos, roomSizeVec);
                break;
            }
            PathDirection dir = _pathDirections[i - 1];
            currentPos += GetDirection(dir) * ROOM_SIZE;

            Vector3 roomPos = transform.position +  (i * transform.forward * ROOM_SIZE);
            Gizmos.DrawCube(currentPos, roomSizeVec);
        }
    }
}