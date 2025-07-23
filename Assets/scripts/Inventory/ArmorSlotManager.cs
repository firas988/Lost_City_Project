using UnityEngine;

public class ArmorSlotManager : MonoBehaviour
{
    [SerializeField]
    private ArmorSlot helmetSlot;

    [SerializeField]
    private ArmorSlot chestplateSlot;

    [SerializeField]
    private ArmorSlot leggingsSlot;

    [SerializeField]
    private ArmorSlot bootsSlot;

    public void setHelmet(Item item)
    {
        helmetSlot.setItem(item);
    }

    public void setChestplate(Item item)
    {
        chestplateSlot.setItem(item);
    }

    public void setLeggings(Item item)
    {
        leggingsSlot.setItem(item);
    }

    public void setBoots(Item item)
    {
        bootsSlot.setItem(item);
    }

    public void removeHelmet()
    {
        helmetSlot.removeItem();
    }

    public void removeChestplate()
    {
        chestplateSlot.removeItem();
    }

    public void removeLeggings()
    {
        leggingsSlot.removeItem();
    }

    public void removeBoots()
    {
        bootsSlot.removeItem();
    }
}
