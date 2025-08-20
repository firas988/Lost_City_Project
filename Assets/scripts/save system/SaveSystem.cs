using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string StatisticsPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "statistics.dat");
        }
    }

    private static string InventoryPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "inventory.dat");
        }
    }

    private static string SkillsPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "skills.dat");
        }
    }

    private static string LevelPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "level.dat");
        }
    }

    private static string PlayerPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "player.dat");
        }
    }
    private static string QuestPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "quest.dat");
        }
    }

    public static void SaveStatistics(StatisticsHandler statisticsHandler)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StatisticsPath));
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(StatisticsPath, FileMode.Create);

        StatisticsData statisticsData = new StatisticsData(statisticsHandler);

        formatter.Serialize(Stream, statisticsData);
        Stream.Close();
    }

    public static StatisticsData LoadStatistics()
    {
        if (File.Exists(StatisticsPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(StatisticsPath, FileMode.Open);
            StatisticsData statisticsData = (StatisticsData)formatter.Deserialize(Stream);
            Stream.Close();
            return statisticsData;
        }
        else
        {
            return null;
        }
    }

    public static void SaveInventory(Inventory inventory)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(InventoryPath));
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(InventoryPath, FileMode.Create);

        InventroyData inventroyData = new InventroyData(inventory);

        formatter.Serialize(Stream, inventroyData);
        Stream.Close();
    }

    public static InventroyData LoadInventory()
    {
        if (File.Exists(InventoryPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(InventoryPath, FileMode.Open);
            InventroyData inventroyData = (InventroyData)formatter.Deserialize(Stream);
            Stream.Close();
            return inventroyData;
        }
        else
        {
            return null;
        }
    }

    public static void SaveSkills(SkillTreeManager skillTreeManager)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SkillsPath));
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(SkillsPath, FileMode.Create);

        SkillTreeData skillTreeData = new SkillTreeData(skillTreeManager);

        formatter.Serialize(Stream, skillTreeData);
        Stream.Close();
    }

    public static SkillTreeData LoadSkills()
    {
        if (File.Exists(SkillsPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(SkillsPath, FileMode.Open);
            SkillTreeData skillTreeData = (SkillTreeData)formatter.Deserialize(Stream);
            Stream.Close();
            return skillTreeData;
        }
        else
        {
            return null;
        }
    }

    public static void SaveLevel(LevelManager levelManager)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LevelPath));
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(LevelPath, FileMode.Create);

        LevelData levelData = new LevelData(levelManager);

        formatter.Serialize(Stream, levelData);
        Stream.Close();
    }

    public static LevelData LoadLevel()
    {
        if (File.Exists(LevelPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(LevelPath, FileMode.Open);
            LevelData levelData = (LevelData)formatter.Deserialize(Stream);
            Stream.Close();
            return levelData;
        }
        else
        {
            return null;
        }
    }

    public static void SaveQuest(QuestManager questManager)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(QuestPath));
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(QuestPath, FileMode.Create);

        QuestData questData = new QuestData(questManager);
        formatter.Serialize(Stream, questData);
        Stream.Close();
    }

    public static QuestData LoadQuest()
    {
        if (File.Exists(QuestPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream Stream = new FileStream(QuestPath, FileMode.Open);

            QuestData questData = (QuestData)formatter.Deserialize(Stream);

            Stream.Close();

            return questData;
        }
        else
        {
            return null;
        }
    }

    public static void SavePlayer(StartPlayer startPlayer)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(PlayerPath));
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(PlayerPath, FileMode.Create);

        PlayerData playerData = new PlayerData(startPlayer);

        formatter.Serialize(Stream, playerData);
        Stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(PlayerPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(PlayerPath, FileMode.Open);
            PlayerData playerData = (PlayerData)formatter.Deserialize(Stream);
            Stream.Close();
            return playerData;
        }
        else
        {
            return null;
        }
    }
}
