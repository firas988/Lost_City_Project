using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int level;
    public float currentXP;
    public float XPtoNextLevel;

    public LevelData(LevelManager levelManager)
    {
        this.level = levelManager.getLevel();
        this.currentXP = levelManager.getCurrentXP();
        this.XPtoNextLevel = levelManager.getXPtoNextLevel();
    }
}
