using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSetupData", menuName = "Scriptable Objects/Game/CharacterSetupData")]
public class CharacterSetupData: ActorSetupData
{
    public int maxBlood;
}
