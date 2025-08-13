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
    private float spacing = 20f;

    private bool isMovingUp = false;

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
        StartCoroutine(ShowNotificationWhenReady(message));
    }

    private IEnumerator ShowNotificationWhenReady(string message)
    {
        while (isMovingUp)
            yield return null;

        float baseY = bottomLeftnotificationInventory
            .GetComponent<RectTransform>()
            .anchoredPosition.y;

        Notification newNotification = Instantiate(
            bottomLeftnotificationInventory,
            parentBottomLeftnotificationInventory
        );
        newNotification.SetSubtitle(message);

        RectTransform newRect = newNotification.GetComponent<RectTransform>();
        float height = newRect.sizeDelta.y;

        if (activeBottomLeftNotificationQueueInventory.Count > 0)
        {
            isMovingUp = true;
            for (int i = 0; i < activeBottomLeftNotificationQueueInventory.Count; i++)
            {
                RectTransform r = activeBottomLeftNotificationQueueInventory[i]
                    .GetComponent<RectTransform>();

                Vector2 targetPos = r.anchoredPosition + new Vector2(0, height - spacing);

                yield return MoveUpSmoothly(r, targetPos, 0.25f);
            }
            isMovingUp = false;
        }

        newRect.anchoredPosition = new Vector2(newRect.anchoredPosition.x, baseY);

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
        if (rect == null || rect.Equals(null))
            yield break;

        Vector2 startPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (rect == null || rect.Equals(null))
                yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        if (rect != null && !rect.Equals(null))
            rect.anchoredPosition = targetPos;
    }

    #endregion
}
