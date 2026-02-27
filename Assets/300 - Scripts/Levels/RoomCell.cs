using UnityEngine;

[System.Serializable]
public class RoomCell
{
    public GameObject OwnedElement;

    public enum CellType { Wall, Door, Normal }

    public CellType Type = CellType.Normal;

    public RoomCell(int x, int y, GameObject element = null)
    {
        Coords = new Vector2Int(x, y);
        OwnedElement = element;
    }

    public bool IsEmpty => OwnedElement == null;

    private Vector2Int _coords;
    public Vector2Int Coords
    {
        get => _coords;
        private set => _coords = value;
    }

    public int Y => Coords.y;
    public int X => Coords.x;
}
