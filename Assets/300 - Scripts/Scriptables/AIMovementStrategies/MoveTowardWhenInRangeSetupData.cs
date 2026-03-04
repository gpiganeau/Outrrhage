using UnityEngine;

[CreateAssetMenu(fileName = "MoveTowardWhenInRangeSetupData", menuName = "Scriptable Objects/Game/AI/MoveTowardWhenInRangeSetupData")]
public class MoveTowardWhenInRangeSetupData: MovementStrategySetupData
{
    [Range(0, 16)] public float DistanceToRiel;
}
