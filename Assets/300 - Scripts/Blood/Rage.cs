using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Rage
{
    [SerializeField] private float _rageDuration;
    [SerializeField] private int _currentAmount;
    [SerializeField] private int _maxAmount;
    public int Amount => _currentAmount;
    public int Maximum => _maxAmount;

    public void SetMaxAmount(int newMaxAmount) => _maxAmount = newMaxAmount;

    public UnityEvent<int, int> OnRageChanged = new UnityEvent<int, int>();
    public UnityEvent<float> OnRageEnter = new UnityEvent<float>();
    public UnityEvent<float> OnRageExit = new UnityEvent<float>();

    public bool IsFull => Amount == Maximum;

    bool Enraged = false;

    public void ForceStop()
    {
        Enraged = false;
        _currentAmount = 0;
        OnRageExit?.Invoke(0);
        
    }

    public int Consume(int amount)
    {
        if (Enraged) return _currentAmount;

        _currentAmount -= amount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, _maxAmount);
        OnRageChanged?.Invoke(_currentAmount, _maxAmount);
        return _currentAmount;
    }

    public int Regain(int amount)
    {
        if (Enraged) return _currentAmount;

        _currentAmount += amount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, _maxAmount);
        OnRageChanged?.Invoke(_currentAmount, _maxAmount);
        return _currentAmount;
    }

    public Rage (CharacterSetupData data)
    {
        InitializeEmpty(data.maxRage);
        _rageDuration = data.RageDuration;
        OnRageChanged.AddListener(CheckForInRage);
        Enraged = false;
    }

    private void CheckForInRage(int current, int max)
    {
        if (current == max)
        {
            Enraged = true;
            OnRageEnter?.Invoke(_rageDuration);
            DOVirtual.DelayedCall(_rageDuration, () =>
            {
                Enraged = false;
                Consume(max);
                OnRageExit?.Invoke(_rageDuration);
            });
        }
    }

    public void Initialize(int Max)
    {
        this.SetMaxAmount(Max);
        _currentAmount = Max;
        OnRageChanged?.Invoke(_currentAmount, _maxAmount);
    }

    public void InitializeEmpty(int Max)
    {
        SetMaxAmount(Max);
        _currentAmount = 0;
        OnRageChanged?.Invoke(_currentAmount, _maxAmount);
    }

    public void Initialize()
    {
        Initialize(_maxAmount);
    }
}