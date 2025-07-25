using UnityEngine;

[System.Serializable]
public class SkillTreeData
{
    public int totalSkillPoints;
    public int spent;
    public int strengthLevel;
    public int speedLevel;
    public int defenseLevel;
    public int healthLevel;

    public SkillTreeData(SkillTreeManager skillTreeManager)
    {
        this.totalSkillPoints = skillTreeManager.getSkillAmountLimit().GetTotalSkillPoints();
        this.spent = skillTreeManager.getSkillAmountLimit().GetTotalSpent();
        this.strengthLevel = skillTreeManager.getStrengthLevel();
        // this.speedLevel = skillTreeManager.getSpeedLevel();
        // this.defenseLevel = skillTreeManager.getDefenseLevel();
        // this.healthLevel = skillTreeManager.getHealthLevel();
    }
}
