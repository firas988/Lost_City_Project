using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Static utility class providing centralized save and load functionality for all game systems.
/// Manages file paths, binary serialization, and data persistence across game sessions.
/// Provides methods for saving and loading statistics, inventory, skills, level, quest, and player data.
/// </summary>
public static class SaveSystem
{
    #region File Path Properties
    /// <summary>
    /// Gets the file path for statistics data storage.
    /// Combines world path with statistics.dat filename.
    /// </summary>
    private static string StatisticsPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "statistics.dat");
        }
    }

    /// <summary>
    /// Gets the file path for inventory data storage.
    /// Combines world path with inventory.dat filename.
    /// </summary>
    private static string InventoryPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "inventory.dat");
        }
    }

    /// <summary>
    /// Gets the file path for skills data storage.
    /// Combines world path with skills.dat filename.
    /// </summary>
    private static string SkillsPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "skills.dat");
        }
    }

    /// <summary>
    /// Gets the file path for level data storage.
    /// Combines world path with level.dat filename.
    /// </summary>
    private static string LevelPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "level.dat");
        }
    }

    /// <summary>
    /// Gets the file path for player data storage.
    /// Combines world path with player.dat filename.
    /// </summary>
    private static string PlayerPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "player.dat");
        }
    }

    /// <summary>
    /// Gets the file path for quest data storage.
    /// Combines world path with quest.dat filename.
    /// </summary>
    private static string QuestPath
    {
        get
        {
            string currentWord = PlayerPrefs.GetString("worldPath");
            return Path.Combine(currentWord, "quest.dat");
        }
    }
    #endregion

    #region Statistics Save and Load
    /// <summary>
    /// Saves player statistics data to persistent storage.
    /// Creates directory structure and serializes StatisticsData to binary file.
    /// </summary>
    /// <param name="statisticsHandler">The StatisticsHandler component containing data to save.</param>
    public static void SaveStatistics(StatisticsHandler statisticsHandler)
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(StatisticsPath));

        // Set up binary formatter and file stream
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(StatisticsPath, FileMode.Create);

        // Create and serialize statistics data
        StatisticsData statisticsData = new StatisticsData(statisticsHandler);
        formatter.Serialize(Stream, statisticsData);

        // Clean up file stream
        Stream.Close();
    }

    /// <summary>
    /// Loads player statistics data from persistent storage.
    /// Deserializes binary file and returns StatisticsData object.
    /// </summary>
    /// <returns>StatisticsData object if file exists, null otherwise.</returns>
    public static StatisticsData LoadStatistics()
    {
        if (File.Exists(StatisticsPath))
        {
            // Set up binary formatter and file stream
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(StatisticsPath, FileMode.Open);

            // Deserialize and return statistics data
            StatisticsData statisticsData = (StatisticsData)formatter.Deserialize(Stream);
            Stream.Close();
            return statisticsData;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Inventory Save and Load
    /// <summary>
    /// Saves player inventory data to persistent storage.
    /// Creates directory structure and serializes InventroyData to binary file.
    /// </summary>
    /// <param name="inventory">The Inventory component containing data to save.</param>
    public static void SaveInventory(Inventory inventory)
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(InventoryPath));

        // Set up binary formatter and file stream
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(InventoryPath, FileMode.Create);

        // Create and serialize inventory data
        InventroyData inventroyData = new InventroyData(inventory);
        formatter.Serialize(Stream, inventroyData);

        // Clean up file stream
        Stream.Close();
    }

    /// <summary>
    /// Loads player inventory data from persistent storage.
    /// Deserializes binary file and returns InventroyData object.
    /// </summary>
    /// <returns>InventroyData object if file exists, null otherwise.</returns>
    public static InventroyData LoadInventory()
    {
        if (File.Exists(InventoryPath))
        {
            // Set up binary formatter and file stream
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(InventoryPath, FileMode.Open);

            // Deserialize and return inventory data
            InventroyData inventroyData = (InventroyData)formatter.Deserialize(Stream);
            Stream.Close();
            return inventroyData;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Skills Save and Load
    /// <summary>
    /// Saves player skill tree data to persistent storage.
    /// Creates directory structure and serializes SkillTreeData to binary file.
    /// </summary>
    /// <param name="skillTreeManager">The SkillTreeManager component containing data to save.</param>
    public static void SaveSkills(SkillTreeManager skillTreeManager)
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(SkillsPath));

        // Set up binary formatter and file stream
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(SkillsPath, FileMode.Create);

        // Create and serialize skill tree data
        SkillTreeData skillTreeData = new SkillTreeData(skillTreeManager);
        formatter.Serialize(Stream, skillTreeData);

        // Clean up file stream
        Stream.Close();
    }

    /// <summary>
    /// Loads player skill tree data from persistent storage.
    /// Deserializes binary file and returns SkillTreeData object.
    /// </summary>
    /// <returns>SkillTreeData object if file exists, null otherwise.</returns>
    public static SkillTreeData LoadSkills()
    {
        if (File.Exists(SkillsPath))
        {
            // Set up binary formatter and file stream
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(SkillsPath, FileMode.Open);

            // Deserialize and return skill tree data
            SkillTreeData skillTreeData = (SkillTreeData)formatter.Deserialize(Stream);
            Stream.Close();
            return skillTreeData;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Level Save and Load
    /// <summary>
    /// Saves player level data to persistent storage.
    /// Creates directory structure and serializes LevelData to binary file.
    /// </summary>
    /// <param name="levelManager">The LevelManager component containing data to save.</param>
    public static void SaveLevel(LevelManager levelManager)
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(LevelPath));

        // Set up binary formatter and file stream
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(LevelPath, FileMode.Create);

        // Create and serialize level data
        LevelData levelData = new LevelData(levelManager);
        formatter.Serialize(Stream, levelData);

        // Clean up file stream
        Stream.Close();
    }

    /// <summary>
    /// Loads player level data from persistent storage.
    /// Deserializes binary file and returns LevelData object.
    /// </summary>
    /// <returns>LevelData object if file exists, null otherwise.</returns>
    public static LevelData LoadLevel()
    {
        if (File.Exists(LevelPath))
        {
            // Set up binary formatter and file stream
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(LevelPath, FileMode.Open);

            // Deserialize and return level data
            LevelData levelData = (LevelData)formatter.Deserialize(Stream);
            Stream.Close();
            return levelData;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Quest Save and Load
    /// <summary>
    /// Saves player quest data to persistent storage.
    /// Creates directory structure and serializes QuestData to binary file.
    /// </summary>
    /// <param name="questManager">The QuestManager component containing data to save.</param>
    public static void SaveQuest(QuestManager questManager)
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(QuestPath));

        // Set up binary formatter and file stream
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(QuestPath, FileMode.Create);

        // Create and serialize quest data
        QuestData questData = new QuestData(questManager);
        formatter.Serialize(Stream, questData);

        // Clean up file stream
        Stream.Close();
    }

    /// <summary>
    /// Loads player quest data from persistent storage.
    /// Deserializes binary file and returns QuestData object.
    /// </summary>
    /// <returns>QuestData object if file exists, null otherwise.</returns>
    public static QuestData LoadQuest()
    {
        if (File.Exists(QuestPath))
        {
            // Set up binary formatter and file stream
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(QuestPath, FileMode.Open);

            // Deserialize and return quest data
            QuestData questData = (QuestData)formatter.Deserialize(Stream);
            Stream.Close();
            return questData;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Player Save and Load
    /// <summary>
    /// Saves player data to persistent storage.
    /// Creates directory structure and serializes PlayerData to binary file.
    /// </summary>
    /// <param name="startPlayer">The StartPlayer component containing data to save.</param>
    public static void SavePlayer(StartPlayer startPlayer)
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(PlayerPath));

        // Set up binary formatter and file stream
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream Stream = new FileStream(PlayerPath, FileMode.Create);

        // Create and serialize player data
        PlayerData playerData = new PlayerData(startPlayer);
        formatter.Serialize(Stream, playerData);

        // Clean up file stream
        Stream.Close();
    }

    /// <summary>
    /// Loads player data from persistent storage.
    /// Deserializes binary file and returns PlayerData object.
    /// </summary>
    /// <returns>PlayerData object if file exists, null otherwise.</returns>
    public static PlayerData LoadPlayer()
    {
        if (File.Exists(PlayerPath))
        {
            // Set up binary formatter and file stream
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = new FileStream(PlayerPath, FileMode.Open);

            // Deserialize and return player data
            PlayerData playerData = (PlayerData)formatter.Deserialize(Stream);
            Stream.Close();
            return playerData;
        }
        else
        {
            return null;
        }
    }
    #endregion
}
