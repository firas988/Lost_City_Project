using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldCreationManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField worldNameInputField;

    [SerializeField]
    private GameObject worldList;

    [SerializeField]
    private GameObject confirmPanel;

    private SceneHandler sceneHandler;

    private GameObject worldToReplace;

    private GameObject worldToLoad;

    private bool isTryingToReplace;

    private bool isLoadingWorldList;

    void Update()
    {
        if (isTryingToReplace && !confirmPanel.activeSelf)
        {
            confirmPanel.SetActive(true);
        }
    }

    void Awake()
    {
        sceneHandler = GameObject.Find("SceneManager").GetComponent<SceneHandler>();
        isTryingToReplace = false;
        isLoadingWorldList = false;
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        confirmPanel.SetActive(false);
        string[] directoryNames = Directory.GetDirectories(
            Application.persistentDataPath + "/gameData/"
        );

        if (directoryNames.Length > 0)
        {
            for (int i = 0; i < Mathf.Min(directoryNames.Length, worldNames.Length); i++)
            {
                TMP_Text worldNameText = worldNames[i]
                    .transform.Find("WorldName")
                    .GetComponent<TMP_Text>();
                TMP_Text worldModifiedText = worldNames[i]
                    .transform.Find("LastModified")
                    .GetComponent<TMP_Text>();
                worldNameText.text = Path.GetFileName(directoryNames[i]);
                worldModifiedText.text = "Last Modified: " + lastModifiedTime(directoryNames[i]);
            }
        }
        else
        {
            Debug.Log("No worlds found");
        }
    }

    public string lastModifiedTime(string path)
    {
        return new FileInfo(path).LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    public void goToWorldList()
    {
        if (!string.IsNullOrEmpty(worldNameInputField.text))
        {
            GetComponent<UIMenuManager>().worldListPanelPosition();
        }
        else
        {
            Debug.Log("World name is not empty");
        }
    }

    public void replaceWorld()
    {
        string pathToWorld =
            Application.persistentDataPath
            + "/gameData/"
            + worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text;
        DirectoryInfo worldToReplaceInfo = new DirectoryInfo(pathToWorld);
        worldToReplaceInfo.Delete(true);
        worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text =
            worldNameInputField.text;
        string worldPath = Application.persistentDataPath + "/gameData/" + worldNameInputField.text;
        Directory.CreateDirectory(worldPath);

        loadWorld(worldPath);
    }

    public void createWorld()
    {
        worldToReplace = EventSystem.current.currentSelectedGameObject;

        string worldName = worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text;

        if (
            Directory.Exists(
                Application.persistentDataPath + "/gameData/" + worldNameInputField.text
            )
        )
        {
            Debug.Log("World already exists");
        }
        else if (Directory.Exists(Application.persistentDataPath + "/gameData/" + worldName))
        {
            isTryingToReplace = true;
        }
        else
        {
            worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text =
                worldNameInputField.text;

            string worldPath =
                Application.persistentDataPath + "/gameData/" + worldNameInputField.text;
            Directory.CreateDirectory(worldPath);
            loadWorld(worldPath);
        }
    }

    public void loadWorld()
    {
        worldToLoad = EventSystem.current.currentSelectedGameObject;

        string worldPath =
            Application.persistentDataPath
            + "/gameData/"
            + worldToLoad.transform.Find("WorldName").GetComponent<TMP_Text>().text;

        if (Directory.Exists(worldPath))
        {
            loadWorld(worldPath);
        }
        else
        {
            Debug.Log("World does not exist");
        }
    }

    public void loadWorld(string worldPath)
    {
        PlayerPrefs.SetString("worldPath", worldPath);
        PlayerData playerData = SaveSystem.LoadPlayer();
        if (playerData == null)
        {
            sceneHandler.LoadScene(1);
        }
        else
        {
            sceneHandler.LoadScene(playerData.SceneIndex);
        }
    }

    public void switchToLoadMode()
    {
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        foreach (Button worldName in worldNames)
        {
            worldName.onClick.RemoveAllListeners();
            worldName.onClick.AddListener(() => loadWorld());
        }
    }

    public void switchToCreateMode()
    {
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        foreach (Button worldName in worldNames)
        {
            worldName.onClick.RemoveAllListeners();
            worldName.onClick.AddListener(() => createWorld());
        }
    }

    public void hideConfirmPanel()
    {
        confirmPanel.SetActive(false);
        isTryingToReplace = false;
    }
}
