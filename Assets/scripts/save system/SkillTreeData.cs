using UnityEngine;

[System.Serializable]
public class SkillTreeData
{
    [SerializeField]
    private int totalSkillPoints;

    [SerializeField]
    private int spent;

    [SerializeField]
    private int strengthLevel;

    [SerializeField]
    private int speedLevel;

    [SerializeField]
    private int defenseLevel;

    [SerializeField]
    private int healthLevel;

    public SkillTreeData(SkillTreeManager skillTreeManager)
    {
        this.totalSkillPoints = skillTreeManager.getSkillAmountLimit().GetTotalSkillPoints();
        this.spent = skillTreeManager.getSkillAmountLimit().GetTotalSpent();
        this.strengthLevel = skillTreeManager.getStrengthLevel();
        this.speedLevel = skillTreeManager.getSpeedLevel();
        this.defenseLevel = skillTreeManager.getDefenseLevel();
        this.healthLevel = skillTreeManager.getHealthLevel();
    }

    public int TotalSkillPoints => totalSkillPoints;
    public int Spent => spent;
    public int StrengthLevel => strengthLevel;
    public int SpeedLevel => speedLevel;
    public int DefenseLevel => defenseLevel;
    public int HealthLevel => healthLevel;
}
