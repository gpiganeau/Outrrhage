using System;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        GenerateCriticalPath();
        StartRun();
    }

    private void GenerateCriticalPath ()
    {
        
        // -- Select HUB Room
        
        // -- Select Boss Room

        // -- Select Core Path

    }

    private void StartRun()
    {
        // -- Spawn Hub Room, Spawn Riel, etc...
    }
    
}