using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;


public enum PathDirection { North, East, South, West }

public class RunSystemController : MonoBehaviour
{
    [SerializeField] private RunSettings runSettings;

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

    public GameRoom CurrentRoom => _currentRoom;

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

        Logger.Core($"Generated a path with {_pathRooms.Count} rooms");

    }

    private void GenerateDirectionPath()
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
            // Shuffle les directions disponibles
            List<PathDirection> available = new List<PathDirection>();
            foreach (var dir in allDirections)
            {
                Vector2Int next = current + ToGrid(dir);
                if (!visited.Contains(next))
                    available.Add(dir);
            }
            
            if (available.Count == 0)
            {
                // Dead end : on backtrack ou on force une direction déjà visitée
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

    bool _currentRoomSequenceOver = false;
    IEnumerator StartRoomSequence (RoomSequencer seq)
    {
        seq.OnRoomStart?.Invoke();
        _currentRoomSequenceOver = false;

        foreach (var step in seq.RoomSequence)
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

        _currentRoomSequenceOver = true;
    }

    private void StartRun()
    {
        _currentRoom = Instantiate(_hubRoom);
        _currentRoom.RegenerateRoom();
        StartCoroutine(StartRoomSequence(runSettings.Sequencers[0]));

        float roomSize = runSettings.HUBRooms[0].Dimensions.x;

        Vector3 currentPos = transform.position;

        for (int i = 1; i <= runSettings.RoomCount; i++)
        {
            // -- Last Room for Boss
            if (i == runSettings.RoomCount)
            {
                PathDirection lastDir = _pathDirections.Last();
                currentPos += GetDirection(lastDir) * roomSize;
                GameRoom bossRoom = Instantiate(_bossRoom, currentPos, Quaternion.identity);
                bossRoom.RegenerateRoom();
                break;
            }

            PathDirection dir = _pathDirections[i - 1];
            currentPos += GetDirection(dir) * roomSize;

            GameRoom newRoom = Instantiate(_pathRooms[i - 1], currentPos, Quaternion.identity);
            newRoom.RegenerateRoom();
        }
    }

    void OnValidate()
    {
        GenerateCriticalPath();
    }

    void OnDrawGizmos()
    {
        float roomSize = runSettings.HUBRooms[0].Dimensions.x;
        Vector3 roomSizeVec = new Vector3(roomSize, 1, roomSize);

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
                currentPos += GetDirection(lastDir) * roomSize;
                Gizmos.DrawCube(currentPos, roomSizeVec);
                break;
            }
            PathDirection dir = _pathDirections[i - 1];
            currentPos += GetDirection(dir) * roomSize;

            Vector3 roomPos = transform.position +  (i * transform.forward * roomSize);
            Gizmos.DrawCube(currentPos, roomSizeVec);
        }
    }
}