using System;
using UnityEngine;

[System.Serializable]
public class BloodStack : MonoBehaviour
{
    [SerializeField] private Blood blood; 
    [SerializeField] private BloodStackDisplay display;

    public int GetStackedValue () => blood.Amount;

    void Start()
    {
        blood = new Blood(0); // -- Init Empty.
        
        //display.Initialize();
    }

    // todo : Events
}