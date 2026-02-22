using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RunSystemController : MonoBehaviour
{
    [SerializeField] private RunSettings runSettings;

    public static RunSystemController Instance;

    [Header("Debug")]
    [SerializeField] private int _roomCount;
    [SerializeField] private GameRoom _hubRoom;
    [SerializeField] private List<GameRoom> _pathRooms;
    [SerializeField] private GameRoom _bossRoom;
    [SerializeField] private GameRoom _currentRoom;

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
        RunSettings s = runSettings;

        _roomCount = s.RoomCount;
        
        // -- Select HUB Room
        _hubRoom = s.HUBRooms.Random();
        
        // -- Select Boss Room
        _bossRoom = s.BossRooms.Random();

        // -- Select Core Path

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

        Logger.Core($"Generated a path with {_pathRooms.Count} rooms");

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


        for (int i = 1; i < runSettings.RoomCount; i++)
        {
            Vector3 roomPos = transform.position +  (i * transform.forward * roomSize);
            GameRoom newRoom = Instantiate(_pathRooms[i - 1], roomPos, Quaternion.identity);
            newRoom.RegenerateRoom();
        }

        var bossRoomPos = transform.position + (runSettings.RoomCount) * transform.forward * roomSize;
        GameRoom bossRoom = Instantiate(_bossRoom, bossRoomPos, Quaternion.identity);
        bossRoom.RegenerateRoom();

    }

    void OnDrawGizmos()
    {
        float roomSize = runSettings.HUBRooms[0].Dimensions.x;
        Vector3 roomSizeVec = new Vector3(roomSize, 1, roomSize);

        // -- Hub Room
        Gizmos.color = Color.rebeccaPurple;
        Gizmos.DrawCube(transform.position, roomSizeVec);


        // -- Path Room
        Gizmos.color = Color.cyan;

        for (int i = 1; i < runSettings.RoomCount; i++)
        {
            Vector3 roomPos = transform.position +  (i * transform.forward * roomSize);
            Gizmos.DrawCube(roomPos, roomSizeVec);
        }

        // -- Boss Room
        Gizmos.color = Color.yellowNice;
        var bossRoomPos = transform.position + (runSettings.RoomCount) * transform.forward * roomSize;
        Gizmos.DrawCube(bossRoomPos, roomSizeVec);

        Gizmos.DrawSphere(transform.position + (Vector3.forward * roomSize * 0.5f), 1f);
    }
}