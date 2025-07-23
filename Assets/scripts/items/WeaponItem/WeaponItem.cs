using UnityEngine;

public enum WeaponType
{
    Sword,
    Bow,
    Axe,
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Item/Weapon")]
public class WeaponItem : Item
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private float range;

    [SerializeField]
    private WeaponType weaponType;

    public override string getDescription()
    {
        return "Damage: " + damage + "\nWeapon Type: " + weaponType;
    }

    public float getDamage()
    {
        return damage;
    }

    public void setDamage(float damage)
    {
        this.damage = damage;
    }

    public float getRange()
    {
        return range;
    }

    public WeaponType getWeaponType()
    {
        return weaponType;
    }

    public GameObject getWeaponPrefab()
    {
        return itemPrefab;
    }
}
