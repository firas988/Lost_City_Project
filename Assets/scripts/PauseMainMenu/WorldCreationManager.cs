using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class WorldCreationManager : MonoBehaviour
{
   
    [SerializeField]
    private TMP_InputField worldNameInputField;

   [SerializeField]
    private GameObject worldList;

    [SerializeField]
    private GameObject confirmPanel;


    private GameObject worldToReplace;

    private bool isTryingToReplace;



    

    void Update(){
      if(isTryingToReplace && !confirmPanel.activeSelf) {
        confirmPanel.SetActive(true);
      }
    }
  
    void Awake(){
        isTryingToReplace = false;
        Button[] worldNames = worldList.GetComponentsInChildren<Button>();
        confirmPanel.SetActive(false);
        string[] directoryNames = Directory.GetDirectories(Application.persistentDataPath + "/gameData/");

       if(directoryNames.Length > 0){
        for(int i = 0; i < Mathf.Min(directoryNames.Length, worldNames.Length); i++){
            TMP_Text worldNameText = worldNames[i].transform.Find("WorldName").GetComponent<TMP_Text>();
            TMP_Text worldModifiedText = worldNames[i].transform.Find("LastModified").GetComponent<TMP_Text>();
            worldNameText.text = Path.GetFileName(directoryNames[i]);
            worldModifiedText.text = "Last Modified: " + lastModifiedTime(directoryNames[i]);
        }
       }
       else{
        Debug.Log("No worlds found");
       }

    }

    public string lastModifiedTime(string path){
        return new FileInfo(path).LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    public void goToWorldList(){
        Debug.Log("goToWorldList");
        if(!string.IsNullOrEmpty(worldNameInputField.text)){
            GetComponent<UIMenuManager>().worldListPanelPosition();
        }else{
            Debug.Log("World name is not empty");
        }
    }

    public void replaceWorld(){
        string pathToWorld = Application.persistentDataPath + "/gameData/" + worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text;
        DirectoryInfo worldToReplaceInfo = new DirectoryInfo(pathToWorld);
        worldToReplaceInfo.Delete(true);
        worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text = worldNameInputField.text;
        string worldPath = Application.persistentDataPath + "/gameData/" + worldNameInputField.text;
        Directory.CreateDirectory(worldPath);
        hideConfirmPanel();
    }
    public void createWorld(){
      worldToReplace = EventSystem.current.currentSelectedGameObject;

        string worldName = worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text;

        if (Directory.Exists(Application.persistentDataPath + "/gameData/" + worldNameInputField.text)){
            Debug.Log("World already exists");
        }
        else if(Directory.Exists(Application.persistentDataPath + "/gameData/" + worldName)){
            isTryingToReplace = true;
        }else{
            worldToReplace.transform.Find("WorldName").GetComponent<TMP_Text>().text = worldNameInputField.text;

            string worldPath = Application.persistentDataPath + "/gameData/" + worldNameInputField.text;
            Directory.CreateDirectory(worldPath);
        }

    }

    public void hideConfirmPanel(){
        confirmPanel.SetActive(false);
        isTryingToReplace = false;
    }




}
