using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Events;
using System;
using DG.Tweening;

public enum PathDirection { North, East, South, West }

public enum RoomType { Hub, Normal, Boss, Safe }

public class RunSystemController : MonoBehaviour
{

    [Serializable]
    private class RoomWrapper
    {
        public GameRoom room;
        public Vector3 position;
        public RoomSequencer sequencer;
        public PathDirection entry;
        public RoomType type = RoomType.Normal;

        public RoomWrapper(GameRoom room, Vector3 pos, RoomSequencer sequencer, PathDirection entrance, RoomType type)
        {
            this.room = room;
            this.position = pos;
            this.sequencer = sequencer;
            this.entry = entrance;
            this.type = type;
        }
    }

    [SerializeField] List<RoomWrapper> Rooms = new();

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

    [Header("Gizmos")]
    public Color BossColor;
    public Color NormalColor;
    public Color HubColor;

    [Header("Debug")]
    [SerializeField] private int _roomCount;
    [SerializeField] private List<GameRoom> _pathRooms;
    [SerializeField] private GameRoom _bossRoom;
    [SerializeField] private GameRoom _currentRoom;
    [SerializeField] private List<PathDirection> _pathDirections;

    [SerializeField] private List<Vector3> _spawnPositions;

    public UnityEvent OnRoomComplete = new();

    public GameRoom HUB => Rooms[0].room;
    public RespawnPoint MainRespawn => HUB.GetSpawnPoint();

    public GameRoom CurrentRoom => _currentRoom;
    [SerializeField] private int _roomIndex = 0;
    bool _currentRoomSequenceOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        OnRoomComplete.AddListener(SpawnNextRoom);
        GenerateCriticalPath();
        StartRun();
    }

    private void GenerateCriticalPath ()
    {
        // -- Resets everything
        RunSettings s = runSettings;
        _pathRooms.Clear();
        _roomCount = s.RoomCount;
        _roomIndex = -1;
        var currentPos = transform.position;
        Rooms.Clear();

        // -- Compute Datas
        GenerateDirectionPath();
        GenerateSpawnPositions();
        var selectedRooms = SelectRooms();

        // -- Hub Room
        RoomWrapper hub = new RoomWrapper(s.HUBRooms.Random(), transform.position, null, PathDirection.South, RoomType.Hub);
        Rooms.Add(hub);

        // Normal Rooms
        for (int i = 0; i < _roomCount; i++)
        {
            var r = selectedRooms[i];
            var p = _spawnPositions[i];
            var sq = GetSequencer(i);
            var d = _pathDirections[i];
            var t = RoomType.Normal;

            RoomWrapper newRoom = new RoomWrapper(r, p, sq, d, t);
            Rooms.Add(newRoom);
        }

        // -- Boss Room
        RoomWrapper boss = new RoomWrapper(s.BossRooms.Random(), _spawnPositions.Last(), null, _pathDirections.Last(), RoomType.Boss);
        Rooms.Add(boss);

        RoomSequencer GetSequencer(int index)
        {
            if (index < s.Sequencers.Length)
            {
                return s.Sequencers[index];
            } else
            {
                return null;
            }
        }

        
        List<GameRoom> SelectRooms() {
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

            return _pathRooms;
        }

        void GenerateSpawnPositions()
        {

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

        if (seq == null)
        {
            yield break;
        }

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

        if (seq.AutoComplete)
        {
            RoomEnd();
        }
    }

    public bool QueryRoomEnd()
    {
        if (!_currentRoomSequenceOver) return false;
        if (EntityManager.Instance.Bots.Count > 0) return false;
        return RoomEnd();
    }

    private bool RoomEnd()
    {
        _currentRoom.customRoomSequencer.OnRoomComplete?.Invoke();
        OnRoomComplete?.Invoke();
        return true;
    }

    private void StartRun()
    {
        Sequence startSeq = DOTween.Sequence();
        startSeq.AppendInterval(2f);
        startSeq.AppendCallback( () =>
        {
            SpawnNextRoom();
            var w = Rooms[0];
            var pos = w.position + w.room.GetSpawnPoint().transform.position;
            GameManager.Instance.RoomStart(pos);
            });
    }

    private void SpawnNextRoom()
    {
        if (_roomIndex <= runSettings.RoomCount)
        {
            _roomIndex++;
            CreateRoom(Rooms[_roomIndex]);
        } else
        {
            GameManager.Instance.TriggerGameEnd();
        }
    }

    int MAX_ROOM_CALL = 10;
    int CURRENT_CALL = 0;
    private void CreateRoom(RoomWrapper wrapper)
    {
        if (CURRENT_CALL >= MAX_ROOM_CALL) return;
        CURRENT_CALL++;

        _currentRoom = Instantiate(wrapper.room, wrapper.position, Quaternion.identity);
        _currentRoom.Init(wrapper.sequencer, wrapper.entry, runSettings.Walls);
        StartCoroutine(StartRoomSequence(_currentRoom.customRoomSequencer));
    }

    void OnValidate()
    {
        GenerateCriticalPath();
    }

    void OnDrawGizmos()
    {
        Vector3 roomSizeVec = new Vector3(ROOM_SIZE, 1, ROOM_SIZE);

        // -- Hub Room
        Gizmos.color = HubColor;
        Gizmos.DrawCube(transform.position, roomSizeVec);

        Vector3 currentPos = transform.position;

        // -- Path Room
        Gizmos.color = NormalColor;

        for (int i = 1; i <= runSettings.RoomCount; i++)
        {
            // -- Last Room for Boss
            if (i == runSettings.RoomCount)
            {
                Gizmos.color = BossColor;
                PathDirection lastDir = _pathDirections.Last();
                currentPos += GetDirection(lastDir) * ROOM_SIZE;
                Gizmos.DrawCube(currentPos, roomSizeVec);
                break;
            }
            PathDirection dir = _pathDirections[i - 1];
            currentPos += GetDirection(dir) * ROOM_SIZE;

            Gizmos.DrawCube(currentPos, roomSizeVec);
        }
    }
}