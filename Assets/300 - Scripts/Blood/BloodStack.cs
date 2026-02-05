using System;
using UnityEngine;

[System.Serializable]
public class BloodStack : MonoBehaviour
{
    [SerializeField] private Blood blood; 
    [SerializeField] private BloodStackDisplay display;

    private ActorSetupData data;

    public int GetStackedValue () => blood.Amount;
    public int MaxBlood => blood.Maximum;

    void Start()
    {
        if (display == null) GetComponentInChildren<BloodStackDisplay>();
    }

    public void Initialize(AIActorSetupData setupData)
    {
        data = setupData;
        blood.InitializeEmpty(data.maxBloodStack);
        display.Initialize(this);

        // -- Get the base blood stack in
        Increase(data.BaseBloodDrop);
    }

    public void Increase(int amount)
    {
        int currentStack = blood.Regain(amount);
        display.Sync(currentStack);
    }
}