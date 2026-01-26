using UnityEngine;

[System.Serializable]
public class Blood
{
    private int _currentAmount;
    private int _maxAmount;
    public int Amount => _currentAmount;
    public int Maximum => _maxAmount;

    public void SetMaxAmount(int newMaxAmount) => _maxAmount = newMaxAmount;

    public int Consume(int amount)
    {
        _currentAmount -= amount;
        return _currentAmount;
    }
}
