using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMoveStayInRangeOfPlayerSetupData", menuName = "Scriptable Objects/Game/AI/MoveStayInRangeOfPlayerSetupData")]
public class MoveStayInRangeOfPlayerSetupData: MovementStrategySetupData
{
    public int minDistanceFromPlayer;
    public int maxDistanceFromPlayer;

    public List<MoveStayInRangeOfPlayerStrategy.Pallier> palliers;
}
