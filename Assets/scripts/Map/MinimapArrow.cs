using UnityEngine;

public class MinimapArrow : MonoBehaviour
{
    private string minimapCameraTag = "MiniMapCamera";

    private Camera minimapCamera;
    [SerializeField]
    private RectTransform minimapRect;
    [SerializeField]
    private RectTransform arrowIcon;
    [SerializeField]
    private RectTransform dotIcon;

    private string playerTag = "Player";
    private Transform player;

    [SerializeField]
    private Vector3 targetPosition;

    [Range(0.1f, 1f)]
    [SerializeField]
    private float edgeOffset = 0.9f;

    void Update()
    {
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        minimapCamera = GameObject.FindGameObjectWithTag(minimapCameraTag).GetComponent<Camera>();
        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(targetPosition);

        Vector2 minimapPos = new Vector2(
            (viewportPos.x - 0.5f) * minimapRect.sizeDelta.x,
            (viewportPos.y - 0.5f) * minimapRect.sizeDelta.y
        );

        float radius = (minimapRect.sizeDelta.x / 2f) * edgeOffset;

        bool isInside = minimapPos.magnitude <= radius && viewportPos.z > 0;

        if (isInside)
        {
            dotIcon.gameObject.SetActive(true);
            arrowIcon.gameObject.SetActive(false);
            dotIcon.anchoredPosition = minimapPos;
        }
        else
        {
            dotIcon.gameObject.SetActive(false);
            arrowIcon.gameObject.SetActive(true);

            minimapPos = minimapPos.normalized * radius;
            arrowIcon.anchoredPosition = minimapPos;

            Vector3 dir = (targetPosition - player.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            float cameraYRotation = minimapCamera.transform.eulerAngles.y;
            arrowIcon.localRotation = Quaternion.Euler(0, 0, -(angle - cameraYRotation));
        }

    }

    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }
}
