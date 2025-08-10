using System;
using System.Collections;
using UnityEngine;

public class DashToTarget : MonoBehaviour
{
    [SerializeField]
    private GameObject HitEffect;
    private float chargeTime = 1.5f;
    private float startRadius = 5f;
    private float endRadius = 0.2f;
    private float startSpeed = 10f;
    private float maxCircleSpeed = 50f;
    private float dashSpeed = 100f;

    private bool isCharging = false;
    private bool isDashing = false;

    private Transform target;
    private string targetTag = "FinalBoss";
    private static event Action OnCrystalRemoved;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isCharging && !isDashing)
        {
            StartCoroutine(ChargeAndDash());
        }
    }

    IEnumerator ChargeAndDash()
    {
        isCharging = true;

        float elapsed = 0f;
        float angle = 0f;

        Vector3 center = transform.position;

        while (elapsed < chargeTime)
        {
            float t = elapsed / chargeTime;

            float radius = Mathf.Lerp(startRadius, endRadius, t);

            float speed = Mathf.Lerp(startSpeed, maxCircleSpeed, t);

            angle += speed * Time.deltaTime;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 offset = new Vector3(x, 0f, z);

            transform.position = center + offset;
            transform.LookAt(target.position);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isCharging = false;
        isDashing = true;

        Vector3 targetPos = target.position + Vector3.up * 3f;

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 50f
            );

            transform.rotation *= Quaternion.Euler(0f, 0f, 75f);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                dashSpeed * Time.deltaTime
            );

            yield return null;
        }

        isDashing = false;
        target.GetComponent<FinalBossControl>().setGetHit(true);
        removeCrystal();
        GameObject hitEffect = Instantiate(HitEffect, transform.position, Quaternion.identity);
        float hitEffectDuration = hitEffect.GetComponent<ParticleSystem>().main.duration;
        Destroy(hitEffect, hitEffectDuration);
        Destroy(this.gameObject);
    }

    public void removeCrystal()
    {
        OnCrystalRemoved?.Invoke();
    }

    public void subscribeToCrystal(Action action)
    {
        OnCrystalRemoved += action;
    }

    public void unsubscribeToCrystal(Action action)
    {
        OnCrystalRemoved -= action;
    }
}
