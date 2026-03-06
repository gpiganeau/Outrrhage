using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;
using DG.Tweening;
using System.Collections;

public enum PathDirection { North, East, South, West }

public enum RoomType { Hub, Normal, Boss, Safe }

public class RunSystemController : MonoBehaviour
{

    #region Fields
    [Serializable]
    private class RoomWrapper
    {
        public GameRoom room; // -- prefab ref
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

    int _roomSequenceIndex = 0;
    RoomSequencer _currentSequencer;

    int MAX_ROOM_CALL = 10;
    int CURRENT_CALL = 0;
    #endregion

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        OnRoomComplete.AddListener(SpawnNextRoom);
        EntityManager.Instance.OnEnemyDied.AddListener(OnEnemyDeath);
        GenerateCriticalPath();
    }

    public void StartRun(List<SkillData> skills)
    {
        Sequence startSeq = DOTween.Sequence();
        startSeq.AppendInterval(0.1f);
        startSeq.AppendCallback( () =>
        {
            SpawnNextRoom();
            var w = Rooms[0];
            var pos = w.position + w.room.GetSpawnPoint().transform.position;
            GameManager.Instance.SpawnRiel(pos.WithY(SettingsManager.Instance.GameplaySettings.YSpawnOffset), skills);
            });
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
        _spawnPositions.Clear();

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

            for (int i = 0; i <= runSettings.RoomCount; i++)
            {
                // -- Last Room for Boss
                if (i == runSettings.RoomCount)
                {
                    PathDirection lastDir = _pathDirections.Last();
                    currentPos += GetDirection(lastDir) * ROOM_SIZE;
                    _spawnPositions.Add(currentPos);
                    break;
                }

                PathDirection dir = _pathDirections[i];
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

    public void StartRoomSequenceManual(RoomSequencer seq)
    {
        if (seq == null) { Logger.LogWarning(Logger.LogCategory.Core, "NULL SEQUENCER"); return; }
        
        _roomSequenceIndex = 0;
        _currentSequencer = seq;

        // ← capture locale, pas de référence à _currentSequencer
        var cachedSeq = seq;
        var step = cachedSeq.RoomSequence[0];

        DOVirtual.DelayedCall(cachedSeq.DelayBeforeRoomStart, () => {
            // Vérifie que c'est toujours le bon sequencer
            if (_currentSequencer != cachedSeq) return;
            
            LogRoomStart();
            cachedSeq.OnRoomStart?.Invoke();

            DOVirtual.DelayedCall(step.delay + 0.1f, () => {
                if (_currentSequencer != cachedSeq) return;
                LogRoomInitStep();
                step.stepEvent?.Invoke();
            });
        });
    }

            
    private void LogRoomStart() => Logger.Core($"Room Start : {CurrentRoom} - {_currentSequencer} - Seq Items : {_currentSequencer.RoomSequence.Count} - index : {_roomIndex} - Room seq index { _roomSequenceIndex}");
    private void LogRoomEnd() => Logger.Core($"Room End : {CurrentRoom} - {_currentSequencer} - index : {_roomIndex} - Room seq index { _roomSequenceIndex}");
    private void LogRoomStep() => Logger.Core($"STEP IN : {CurrentRoom} - {_currentSequencer} - index : {_roomIndex} - Room seq index { _roomSequenceIndex}");
    private void LogRoomInitStep() => Logger.Core($"STEP IN ZERO: {CurrentRoom} - {_currentSequencer} - index : {_roomIndex} - Room seq index { _roomSequenceIndex}");


    private int GetStepThreshold => 
    _currentSequencer != null && _roomSequenceIndex < _currentSequencer.RoomSequence.Count 
    ? _currentSequencer.RoomSequence[_roomSequenceIndex].TriggerAtEnemyCount 
    : 999;

    public void OnEnemyDeath(EntityType type)
    {
        if (_currentSequencer == null) return; // ← guard
        if (EntityManager.Instance.Bots.Count > GetStepThreshold) return;
        CurrentSeqStepIn(); 
    }

    private void CurrentSeqStepIn()
    {
        _roomSequenceIndex++;
        LogRoomStep();


        if (_roomSequenceIndex < _currentSequencer.RoomSequence.Count)
        {
            var step = _currentSequencer.RoomSequence[_roomSequenceIndex];

            Juicer.I.LastEnemyEffect();

                DOVirtual.DelayedCall(step.delay, () => {

                step.stepEvent?.Invoke();
            });

        } else
        {

            Juicer.I.LastEnemyEffect();
            DOVirtual.DelayedCall(2f, () => QueryRoomEnd(true));
        }
    }

    public bool QueryRoomEnd(bool ByPassEnemyCount = false)
    {
        Logger.Core($"QUERY ROOM END ! Bypasss : {ByPassEnemyCount} - Enemy Count {EntityManager.Instance.Bots.Count}");
        if (EntityManager.Instance.Bots.Count > 0 && !ByPassEnemyCount) return false;
        return RoomEnd();
    }

    private bool RoomEnd()
    {
        LogRoomEnd();
        Juicer.I.RoomEndEffect();
        _currentRoom.customRoomSequencer.OnRoomComplete?.Invoke(); // -- Current Complete
        DOVirtual.DelayedCall(2f, () => OnRoomComplete?.Invoke()); // -- Then Spawn next
        return true;
    }

    private void SpawnNextRoom()
    {
        if (_roomIndex <= runSettings.RoomCount)
        {
            _roomIndex++;
            Logger.Core($"Spawn next room with index {_roomIndex}");
            CreateRoom(Rooms[_roomIndex]);
        } else
        {
            GameManager.Instance.TriggerDemoEnd();
        }
    }

    private void CreateRoom(RoomWrapper wrapper)
    {
        if (CURRENT_CALL >= MAX_ROOM_CALL) return;
        CURRENT_CALL++;

        _currentRoom = Instantiate(wrapper.room, wrapper.position, Quaternion.identity);
        _currentRoom.Init(wrapper.sequencer, wrapper.entry, runSettings.Walls);
        //OpenEntrance(_currentRoom.transform.position, wrapper.entry);
        StartRoomSequenceManual(_currentRoom.customRoomSequencer);
    }

    private void OpenEntrance(Vector3 roomPos, PathDirection direction)
    {
        var origin = roomPos - (GetDirection(direction) * ROOM_SIZE * 0.5f);
        Collider[] hits = Physics.OverlapSphere(origin, 3.8F, LayerMask.GetMask("Walls"));
        foreach (var hit in hits)
            Destroy(hit.gameObject);

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

        for (int i = 0; i <= runSettings.RoomCount; i++)
        {
            // -- Last Room for Boss
            if (i == runSettings.RoomCount)
            {
                Gizmos.color = BossColor;
                PathDirection lastDir = _pathDirections.Last();
                currentPos += GetDirection(lastDir) * ROOM_SIZE;
                Gizmos.DrawCube(currentPos, roomSizeVec);
                Gizmos.DrawSphere(currentPos - (GetDirection(lastDir) * ROOM_SIZE * 0.5f)+ new Vector3(0, 1, 0), 2);

                break;
            }
            PathDirection dir = _pathDirections[i];
            currentPos += GetDirection(dir) * ROOM_SIZE;


            Gizmos.DrawSphere(currentPos - (GetDirection(dir) * ROOM_SIZE * 0.5f)+ new Vector3(0, 1, 0), 2);


            Gizmos.DrawWireCube(currentPos, roomSizeVec);
        }

        Gizmos.color = Color.rebeccaPurple;
        for (int k = 0; k < _pathDirections.Count; k++)
        {
            var p = GetDirection(_pathDirections[k]);
            Gizmos.DrawSphere(p, 2);

        }
    }
}