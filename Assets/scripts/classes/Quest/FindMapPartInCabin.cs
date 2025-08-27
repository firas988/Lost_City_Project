using UnityEngine;

[CreateAssetMenu(fileName = "FindMapPartInCabin", menuName = "Quests/FindMapPartInCabin")]
public class FindMapPartInCabin : StoryQuest
{

[SerializeField]
  private static GameObject hint;

  public FindMapPartInCabin(Quest quest):base(quest){
    hint = null;
  }


    public void setHint(GameObject hint){
      FindMapPartInCabin.hint = hint;
    }

    public GameObject getHint(){
      return FindMapPartInCabin.hint;
    }

     public override void CompleteQuest(){

        GameObject door = GameObject.Find("Cabin").transform.Find("Door").gameObject;
        hint.SetActive(true);

        door.GetComponent<Animator>().SetBool("IsClosed", false);
        
        base.CompleteQuest();

     }


     

}
