// This script manages the display of notifications in the UI, specifically for top-left and middle positions.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    // Reference to the Notification UI element for the top-left corner
    [SerializeField]
    private Notification topLeftnotification;

    // Reference to the Notification UI element for the middle of the screen
    [SerializeField]
    private Notification middlenotification;

    [SerializeField]
    private Notification bottomLeftnotificationInventory;

    [SerializeField]
    private Transform parentBottomLeftnotificationInventory;

    [SerializeField]
    private float spacing = 500f;

    // Queue to store messages for the top-left notification, ensuring they are shown one after another
    private Queue<string> topLeftNotificationQueue = new Queue<string>();

    // Queue to store messages for the middle notification (not used for sequential display in this script)
    private Queue<string> middleNotificationQueue = new Queue<string>();

    private List<Notification> activeBottomLeftNotificationQueueInventory =
        new List<Notification>();

    // Flag to indicate if a top-left notification is currently being displayed
    private bool isTopLeftNotificationActive = false;

    // Flag to indicate if a middle notification is currently being displayed (not used in this script)
    // private bool isMiddleNotificationActive = false;

    public void queueTopLeftNotification(string message)
    {
        topLeftNotificationQueue.Enqueue(message);

        if (!isTopLeftNotificationActive)
            StartCoroutine(showTopLeftNotification());
    }

    // Coroutine to show a notification in the top-left corner with a message
    public IEnumerator showTopLeftNotification()
    {
        isTopLeftNotificationActive = true;

        while (topLeftNotificationQueue.Count > 0)
        {
            string message = topLeftNotificationQueue.Dequeue();
            // Set the message text in the notification UI
            topLeftnotification.SetSubtitle(message);
            // Show the notification UI
            topLeftnotification.Show();

            // Wait for 3 seconds before hiding the notification
            yield return new WaitForSeconds(3f);
            // Hide the notification UI
            topLeftnotification.Hide();
            yield return new WaitForSeconds(2f);
            isTopLeftNotificationActive = false;
        }
    }

    // Method to show a notification in the middle of the screen with a message
    public void ShowMiddleNotification(string message)
    {
        // Set the message text in the notification UI
        middlenotification.SetSubtitle(message);
        // Show the notification UI
        middlenotification.Show();
    }

    public void ShowBottomLeftNotificationInventory(string message)
    {
        Notification newNotification = Instantiate(
            bottomLeftnotificationInventory,
            parentBottomLeftnotificationInventory
        );
        newNotification.SetSubtitle(message);

        List<Vector2> originalPositions = new List<Vector2>();

        foreach (var notif in activeBottomLeftNotificationQueueInventory)
        {
            RectTransform r = notif.GetComponent<RectTransform>();
            originalPositions.Add(r.anchoredPosition);
        }

        for (int i = 0; i < activeBottomLeftNotificationQueueInventory.Count; i++)
        {
            RectTransform r = activeBottomLeftNotificationQueueInventory[i]
                .GetComponent<RectTransform>();
            Vector2 targetPos = originalPositions[i] + new Vector2(0, spacing);
            StartCoroutine(MoveUpSmoothly(r, targetPos, 0.25f));
        }

        newNotification.Show();
        activeBottomLeftNotificationQueueInventory.Add(newNotification);

        StartCoroutine(RemoveAfterDelay(newNotification, 5f));
    }

    private IEnumerator RemoveAfterDelay(Notification notification, float delay)
    {
        yield return new WaitForSeconds(delay);
        notification.Hide();
        yield return new WaitForSeconds(2f);
        activeBottomLeftNotificationQueueInventory.Remove(notification);
        Destroy(notification.gameObject);
    }

    private IEnumerator MoveUpSmoothly(RectTransform rect, Vector2 targetPos, float duration)
    {
        if (rect == null)
            yield break;
        Vector2 startPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }
}
