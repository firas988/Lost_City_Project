using UnityEngine;

[System.Serializable]
public class LevelData
{
    [SerializeField]
    private int level;

    [SerializeField]
    private float currentXP;

    [SerializeField]
    private float xPtoNextLevel;

    public LevelData(LevelManager levelManager)
    {
        this.level = levelManager.getLevel();
        this.currentXP = levelManager.getCurrentXP();
        this.xPtoNextLevel = levelManager.getXPtoNextLevel();
    }

    public int Level => level;
    public float CurrentXP => currentXP;
    public float XPtoNextLevel => xPtoNextLevel;
}
