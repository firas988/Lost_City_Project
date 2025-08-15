using UnityEngine;

public class CharacterPrevController : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private GameObject light;

    private void Awake()
    {
        camera.SetActive(false);
        light.SetActive(false);
    }

    public void showCharacterPreview()
    {
        camera.SetActive(true);
        light.SetActive(true);
    }

    public void hideCharacterPreview()
    {
        camera.SetActive(false);
        light.SetActive(false);
    }
}
