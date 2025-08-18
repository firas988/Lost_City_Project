using UnityEngine;

public class CharacterPrevController : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryCamera;

    [SerializeField]
    private GameObject light;

    private void Awake()
    {
        inventoryCamera.SetActive(false);
        light.SetActive(false);
    }

    public void showCharacterPreview()
    {
        inventoryCamera.SetActive(true);
        light.SetActive(true);
    }

    public void hideCharacterPreview()
    {
        inventoryCamera.SetActive(false);
        light.SetActive(false);
    }
}
