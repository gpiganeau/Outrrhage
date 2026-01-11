using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ActorData
{
    public int currentRage;
    public int maxBlood;
    public int currentBlood;

    public CharacterData(CharacterSetupData setupData) : base(setupData)
    {        
        currentRage = 0;
        maxBlood = setupData.maxBlood;
        currentBlood = 0;
    }
}
