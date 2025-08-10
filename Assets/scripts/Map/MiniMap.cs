using UnityEngine;

public class MiniMap : MonoBehaviour
{
  private GameObject player;
  private string playerTag = "Player";
  private string mainCameraTag = "MainCamera";
  private GameObject mainCamera;
  private float yPosition = 200f;
  private Vector3 newPosition;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        mainCamera = GameObject.FindGameObjectWithTag(mainCameraTag);
    }

    void Update()
    {
        newPosition = player.transform.position;
        newPosition.y = yPosition;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, mainCamera.transform.eulerAngles.y, 0f);
    }
}
