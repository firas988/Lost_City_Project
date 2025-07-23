using UnityEngine;

public class PlayerDistanceTracker : MonoBehaviour
{
    private Vector3 lastPosition;
    private float totalDistance;

    void Start()
    {
        lastPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        totalDistance = 0f;
    }

    void Update()
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        totalDistance += distanceMoved;
        lastPosition = currentPosition;
    }

    public float getDistance()
    {
        return totalDistance;
    }
}
