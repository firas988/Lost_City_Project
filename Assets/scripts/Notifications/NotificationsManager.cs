// This script manages the display of notifications in the UI, specifically for top-left and middle positions.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the display of notifications in the UI, specifically for top-left, middle, and bottom-left positions.
/// Handles notification queuing, timing, and smooth animations for inventory notifications.
/// </summary>
public class NotificationsManager : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Reference to the Notification UI element for the top-left corner.
    /// </summary>
    [SerializeField]
    private Notification topLeftnotification;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private AudioSource audioSource;

    /// <summary>
    /// Reference to the Notification UI element for the middle of the screen.
    /// </summary>
    [SerializeField]
    private Notification middlenotification;

    /// <summary>
    /// Reference to the Notification UI element for the bottom-left inventory notifications.
    /// </summary>
    [SerializeField]
    private Notification bottomLeftnotificationInventory;

    /// <summary>
    /// Parent transform for bottom-left inventory notifications.
    /// </summary>
    [SerializeField]
    private Transform parentBottomLeftnotificationInventory;

    /// <summary>
    /// Spacing between inventory notifications when they stack.
    /// </summary>
    [SerializeField]
    private float spacing = 500f;

    #endregion

    #region Private Fields

    /// <summary>
    /// Queue to store messages for the top-left notification, ensuring they are shown one after another.
    /// </summary>
    private Queue<string> topLeftNotificationQueueText = new Queue<string>();

    private Queue<string> topLeftNotificationQueueAudio = new Queue<string>();

    /// <summary>
    /// Queue to store messages for the middle notification (not used for sequential display in this script).
    /// </summary>
    private Queue<string> middleNotificationQueue = new Queue<string>();

    /// <summary>
    /// List of active bottom-left inventory notifications for management.
    /// </summary>
    private List<Notification> activeBottomLeftNotificationQueueInventory =
        new List<Notification>();

    /// <summary>
    /// Flag to indicate if a top-left notification is currently being displayed.
    /// </summary>
    private bool isTopLeftNotificationActive = false;

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("GameManager").GetComponentInChildren<AudioManager>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    #endregion

    #region Top-Left Notification Methods

    /// <summary>
    /// Queues a message for display in the top-left notification area.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void queueTopLeftNotification(string message, string audioName)
    {
        topLeftNotificationQueueText.Enqueue(message);
        topLeftNotificationQueueAudio.Enqueue(audioName);

        if (!isTopLeftNotificationActive)
            StartCoroutine(showTopLeftNotification());
    }

    /// <summary>
    /// Coroutine to show a notification in the top-left corner with a message.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator showTopLeftNotification()
    {
        isTopLeftNotificationActive = true;
        while (topLeftNotificationQueueText.Count > 0)
        {
            string message = topLeftNotificationQueueText.Dequeue();
            string audioName = topLeftNotificationQueueAudio.Dequeue();
            // Set the message text in the notification UI
            topLeftnotification.SetSubtitle(message);
            // Show the notification UI
            topLeftnotification.Show();
            audioManager.playUI(audioSource, audioName);

            // Wait for 3 seconds before hiding the no tification
            yield return new WaitForSeconds(audioManager.getAudioClipLength(audioName) + 3f);

            // Hide the notification UI
            topLeftnotification.Hide();
            yield return new WaitForSeconds(5f);
            isTopLeftNotificationActive = false;
        }
    }

    #endregion

    #region Middle Notification Methods

    /// <summary>
    /// Method to show a notification in the middle of the screen with a message.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void ShowMiddleNotification(string message)
    {
        // Set the message text in the notification UI
        middlenotification.SetSubtitle(message);
        // Show the notification UI
        middlenotification.Show();
    }

    #endregion

    #region Bottom-Left Inventory Notification Methods

    /// <summary>
    /// Shows a notification in the bottom-left area for inventory-related messages.
    /// Creates a new notification instance and manages the stacking animation.
    /// </summary>
    /// <param name="message">The message to display.</param>
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

    #endregion

    #region Coroutines

    /// <summary>
    /// Removes a notification after a specified delay.
    /// </summary>
    /// <param name="notification">The notification to remove.</param>
    /// <param name="delay">The delay before removal in seconds.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator RemoveAfterDelay(Notification notification, float delay)
    {
        yield return new WaitForSeconds(delay);
        notification.Hide();
        yield return new WaitForSeconds(2f);
        activeBottomLeftNotificationQueueInventory.Remove(notification);
        Destroy(notification.gameObject);
    }

    /// <summary>
    /// Smoothly moves a RectTransform to a target position over a specified duration.
    /// </summary>
    /// <param name="rect">The RectTransform to move.</param>
    /// <param name="targetPos">The target position to move to.</param>
    /// <param name="duration">The duration of the movement in seconds.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
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

    #endregion
}
