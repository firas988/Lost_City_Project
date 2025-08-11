using UnityEngine;

public class ItemToFindTopenTheMiddel_Hnadler : MonoBehaviour
{
    [SerializeField]
    private GameObject crystal;

    private bool isMovingUp = false;
    private Vector3 targetPosition;

    void Start()
    {
        crystal.SetActive(false);
    }

    void Update() {
      if(Input.GetKeyDown(KeyCode.E))
      {
        foundIT();
      }

      if (isMovingUp)
        {
            crystal.transform.position = Vector3.Lerp(
                crystal.transform.position,
                targetPosition,
                Time.deltaTime * 1
            );

            // لو اقتربت من الهدف توقّف الحركة
            if (Vector3.Distance(crystal.transform.position, targetPosition) < 0.01f)
            {
                crystal.transform.position = targetPosition;
                isMovingUp = false;
            }
        }
    }

    public void foundIT()
    {
        crystal.SetActive(true);
        targetPosition = new Vector3(
            crystal.transform.position.x,
            crystal.transform.position.y + 15f,
            crystal.transform.position.z
        );
        isMovingUp = true;
    }
}
