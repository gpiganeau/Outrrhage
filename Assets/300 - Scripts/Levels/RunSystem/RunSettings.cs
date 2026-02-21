using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Run Settings", menuName = "Scriptable Objects/Game/RunSettings")]
public class RunSettings : ScriptableObject
{
    [Header("Main Settings")]
    public string Name = "New Run Settings";
    [TextArea] public string Description = "Describe this run expectation here";
    [Tooltip("There is always HUB Room and Boss + RoomCount")] public int RoomCount = 3;  

    [Header("Rooms Prefabs")]
    public List <GameRoom> HUBRooms;
    public List <GameRoom> NormalRooms;
    public List <GameRoom> BossRooms;
    public List<GameObject> Corridors;
}

