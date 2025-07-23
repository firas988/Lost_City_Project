using UnityEngine;

[System.Serializable]
public class SkillTreeData
{
    public int available;
    public int spent;
    public int strengthLevel;
    public int speedLevel;
    public int defenseLevel;
    public int healthLevel;

    public SkillTreeData(SkillTreeManager skillTreeManager)
    {
        this.available = skillTreeManager.getSkillAmountLimit().GetAvailable();
        this.spent = skillTreeManager.getSkillAmountLimit().GetTotalSpent();
        this.strengthLevel = skillTreeManager.getStrengthLevel();
        // this.speedLevel = skillTreeManager.getSpeedLevel();
        // this.defenseLevel = skillTreeManager.getDefenseLevel();
        // this.healthLevel = skillTreeManager.getHealthLevel();
    }
}
