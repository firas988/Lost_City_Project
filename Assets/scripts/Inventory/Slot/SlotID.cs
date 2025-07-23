using UnityEngine;

[CreateAssetMenu(fileName = "New SlotID", menuName = "Inventory/SlotID")]
public class SlotID : ScriptableObject
{
    [SerializeField]
    [Range(0, 4)]
    private int column;

    [SerializeField]
    [Range(0, 3)]
    private int row;

    public int getColumn()
    {
        return column;
    }

    public int getRow()
    {
        return row;
    }
}
