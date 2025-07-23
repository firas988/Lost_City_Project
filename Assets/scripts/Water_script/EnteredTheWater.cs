using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class EnteredTheWater : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private GameObject respawn;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Check if player and respawn point are assigned
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }
        if (respawn == null)
        {
            Debug.LogError("Respawn point is not assigned in inspector!");
        }
    }

    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& player!=null &&respawn!=null)
        {
            Debug.Log(player.transform.position);
            Debug.Log("Player entered water!");
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = respawn.transform.position;
            player.transform.rotation = respawn.transform.rotation;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }

}
