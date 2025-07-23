using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string attackName { get; set; }
    public float attackRange { get; set; }
    public float attackTime { get; set; }
    public float attackDamage { get; set; }
    public float attackRadius { get; set; }
}

public class EnemyAttackesConvert : MonoBehaviour
{
    [SerializeField]
    private TextAsset jsonFile;

    private List<Dictionary<string, List<Attack>>> enemyAttacks;

    public List<Attack> getEnemyAttacks(string enemyType)
    {
        foreach (var enemy in enemyAttacks)
        {
            foreach (var type in enemy)
            {
                if (type.Key == enemyType)
                {
                    return type.Value;
                }
            }
        }
        return null;
    }

    void Awake()
    {
        enemyAttacks = JsonConvert.DeserializeObject<List<Dictionary<string, List<Attack>>>>(
            jsonFile.text
        );
    }
}
