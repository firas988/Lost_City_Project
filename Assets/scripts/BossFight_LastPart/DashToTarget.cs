using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the dash attack behavior for the final boss fight crystal
/// </summary>
public class DashToTarget : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Visual effect prefab to spawn when hitting the target
    /// </summary>
    [SerializeField]
    private GameObject HitEffect;
    #endregion

    #region Movement Parameters
    /// <summary>
    /// Time spent charging before dashing (seconds)
    /// </summary>
    private float chargeTime = 1.5f;

    /// <summary>
    /// Initial radius of the charging circle
    /// </summary>
    private float startRadius = 5f;

    /// <summary>
    /// Final radius of the charging circle
    /// </summary>
    private float endRadius = 0.2f;

    /// <summary>
    /// Initial rotation speed during charge phase
    /// </summary>
    private float startSpeed = 10f;

    /// <summary>
    /// Maximum rotation speed during charge phase
    /// </summary>
    private float maxCircleSpeed = 50f;

    /// <summary>
    /// Speed of the dash attack
    /// </summary>
    private float dashSpeed = 100f;
    #endregion

    #region State Variables
    /// <summary>
    /// Whether the crystal is currently charging
    /// </summary>
    private bool isCharging = false;

    /// <summary>
    /// Whether the crystal is currently dashing
    /// </summary>
    private bool isDashing = false;
    #endregion

    #region Target References
    /// <summary>
    /// Transform of the target to dash towards
    /// </summary>
    private Transform target;

    /// <summary>
    /// GameObject of the target to dash towards
    /// </summary>
    private GameObject targetGameObject;

    /// <summary>
    /// Tag of the target GameObject
    /// </summary>
    private string targetTag = "FinalBoss";
    #endregion

    #region Events
    /// <summary>
    /// Event triggered when the crystal is removed
    /// </summary>
    private static event Action OnCrystalRemoved;
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        // Find the target GameObject by tag and store its transform
        targetGameObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetGameObject)
        {
            target = targetGameObject.transform;
        }
    }

    #endregion

    #region Dash Behavior

    public void startDash()
    {
        if (!isCharging && !isDashing && targetGameObject)
        {
            StartCoroutine(ChargeAndDash());
        }
        else if (!targetGameObject)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Main coroutine that handles the charge and dash sequence
    /// </summary>
    IEnumerator ChargeAndDash()
    {
        // Start the charging phase
        isCharging = true;

        float elapsed = 0f;
        float angle = 0f;

        // Store the initial position as the center of the charging circle
        Vector3 center = transform.position;

        // Phase 1: Charge by circling around the target
        while (elapsed < chargeTime)
        {
            float t = elapsed / chargeTime;

            // Gradually reduce the radius from start to end for spiral effect
            float radius = Mathf.Lerp(startRadius, endRadius, t);

            // Gradually increase rotation speed for dynamic movement
            float speed = Mathf.Lerp(startSpeed, maxCircleSpeed, t);

            // Update the angle based on current speed
            angle += speed * Time.deltaTime;

            // Calculate circular movement offset using trigonometry
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 offset = new Vector3(x, 0f, z);

            // Apply the offset to center position and look at target
            transform.position = center + offset;
            transform.LookAt(target.position);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // End charging phase and start dashing
        isCharging = false;
        isDashing = true;

        // Set target position slightly above the target for better visual effect
        Vector3 targetPos = target.position + Vector3.up * 3f;

        // Phase 2: Dash directly toward the target
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            // Calculate direction vector to target
            Vector3 direction = (targetPos - transform.position).normalized;

            // Smoothly rotate toward the target direction
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 50f
            );

            // Apply additional rotation for dramatic visual effect
            transform.rotation *= Quaternion.Euler(0f, 0f, 75f);

            // Move toward target at dash speed
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                dashSpeed * Time.deltaTime
            );

            yield return null;
        }

        // End dashing phase
        isDashing = false;

        // Deal damage to the target
        target.GetComponent<FinalBossControl>().setGetHit(true);

        // Trigger crystal removal event
        removeCrystal();

        // Spawn hit effect at impact position
        GameObject hitEffect = Instantiate(HitEffect, transform.position, Quaternion.identity);
        float hitEffectDuration = hitEffect.GetComponent<ParticleSystem>().main.duration;

        // Clean up hit effect after it finishes playing
        Destroy(hitEffect, hitEffectDuration);

        // Destroy this crystal GameObject
        Destroy(this.gameObject);
    }
    #endregion

    #region Event Management
    /// <summary>
    /// Triggers the crystal removed event
    /// </summary>
    public void removeCrystal()
    {
        OnCrystalRemoved?.Invoke();
    }

    /// <summary>
    /// Subscribe to crystal removed event
    /// </summary>
    /// <param name="action">Action to execute when crystal is removed</param>
    public void subscribeToCrystal(Action action)
    {
        OnCrystalRemoved += action;
    }

    /// <summary>
    /// Unsubscribe from crystal removed event
    /// </summary>
    /// <param name="action">Action to remove from subscription</param>
    public void unsubscribeToCrystal(Action action)
    {
        OnCrystalRemoved -= action;
    }
    #endregion
}
