using UnityEngine;

[System.Serializable]
public class Blood
{
    [SerializeField] private int _currentAmount;
    [SerializeField] private int _maxAmount;
    public int Amount => _currentAmount;
    public int Maximum => _maxAmount;

    public void SetMaxAmount(int newMaxAmount) => _maxAmount = newMaxAmount;

    public int Consume(int amount)
    {
        _currentAmount -= amount;
        return _currentAmount;
    }

    public Blood (int Max)
    {
        this.SetMaxAmount(Max);
        _currentAmount = Max;
    }
}