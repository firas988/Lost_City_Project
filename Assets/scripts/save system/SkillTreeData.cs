using UnityEngine;

[System.Serializable]
public class SkillTreeData
{
    private int totalSkillPoints;
    private int spent;
    public int strengthLevel;
    private int speedLevel;
    private int defenseLevel;
    private int healthLevel;

    public int TotalSkillPoints => totalSkillPoints;
    public int Spent => spent;
    public int StrengthLevel => strengthLevel;
    public int SpeedLevel => speedLevel;
    public int DefenseLevel => defenseLevel;
    public int HealthLevel => healthLevel;

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
