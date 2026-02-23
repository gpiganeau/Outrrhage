using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Rage

{
    [SerializeField] private int _currentAmount;
    [SerializeField] private int _maxAmount;
    public int Amount => _currentAmount;
    public int Maximum => _maxAmount;

    public void SetMaxAmount(int newMaxAmount) => _maxAmount = newMaxAmount;

    public UnityEvent<int, int> OnRageChanged = new UnityEvent<int, int>();

    public bool IsFull => Amount == Maximum;

    public int Consume(int amount)
    {
        _currentAmount -= amount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, _maxAmount);
        OnRageChanged?.Invoke(_currentAmount, _maxAmount);
        return _currentAmount;
    }

    public int Regain(int amount)
    {
        _currentAmount += amount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, _maxAmount);
        OnRageChanged?.Invoke(_currentAmount, _maxAmount);
        return _currentAmount;
    }

    public Rage (int Max)
    {
        Initialize(Max);
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