using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    #region Audio Components
    [Header("Audio")]
    [SerializeField] private GameObject droneAudio;
    [SerializeField] private AudioSource endSound;
    //[SerializeField] private MainScript audioScript;
    #endregion

    #region Gun Properties
    [Header("Gun Properties")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private float shootDelay = 0.1f;
    public float bulletSpeed = 100f;
    [Range(0f, 1000f)]
    public int damageBullet = 10;
    #endregion

    #region Particle Systems
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem shootingSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;
    //   [SerializeField] private ParticleSystem overHeatParticle;   OverHeat Particle reference
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private LayerMask mask;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Slider overheatSlider;
    [SerializeField] private Button fireButton;
    #endregion

    #region Aim Assist Settings
    [Header("Aim Assist Settings")]
    [SerializeField] private float aimAssistRadius = 200f;
    [SerializeField] private float horizontalAimAssistAngle = 45f;
    [SerializeField] private float verticalAimAssistAngle = 20f;
    #endregion

    #region Heat System Settings
    [Header("Heat System Settings")]
    public float maxHeatTime = 3f;
    [SerializeField] private float cooldownTime = 2f;
    public float cooldownSpeed = 0.5f;
    [SerializeField] private float gradualCooldownSpeed = 0.2f;
    #endregion

    #region Private Fields
    private float lastShootTime;
    private bool isGunOverheated;
    private bool isFireButtonPressed;
    private float currentHeatValue;

    private readonly Color greenColor = Color.green;
    private readonly Color yellowColor = Color.yellow;
    private readonly Color redColor = Color.red;
    #endregion

    #region Unity Methods
    private void Start()
    {
        SetupFireButton();
        ResetHeatValues();
    }

    private void Update()
    {
        HandleGunSystem();
        UpdateSliderColor();
    }
    #endregion

    #region Heat System
    private void HandleGunSystem()
    {
        if (isGunOverheated)
        {
            //            overHeatParticle.Play();                             overHeat particle play
            HandleOverheatedState();
            return;
        }

        if (isFireButtonPressed)
        {
            HandleFiring();
        }
        else
        {
            HandleCooldown();
        }

        UpdateHeatSlider();
    }

    private void HandleOverheatedState()
    {
        float timeSinceOverheat = Time.time - lastShootTime;

        if (timeSinceOverheat >= cooldownTime)
        {
            StartGradualCooldown();
            droneAudio?.SetActive(true);
        }
    }

    private void StartGradualCooldown()
    {
        isGunOverheated = false;
        currentHeatValue = maxHeatTime;
        UpdateHeatSlider();
    }

    private void HandleFiring()
    {
        currentHeatValue += Time.deltaTime;
        currentHeatValue = Mathf.Min(currentHeatValue, maxHeatTime);

        if (currentHeatValue >= maxHeatTime)
        {
            TriggerOverheat();
        }
        else if (CanShoot())
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    private void HandleCooldown()
    {
        if (currentHeatValue > 0)
        {
            float cooldownMultiplier = isGunOverheated ? gradualCooldownSpeed : cooldownSpeed;
            currentHeatValue -= Time.deltaTime * cooldownMultiplier;
            currentHeatValue = Mathf.Max(currentHeatValue, 0f);
        }
    }

    private void TriggerOverheat()
    {
        isGunOverheated = true;
        lastShootTime = Time.time;
        endSound?.Play();
        droneAudio?.SetActive(false);
        Debug.Log("Qurol qizib ketdi! Sovushni kutyapti...");
    }

    private bool CanShoot()
    {
        return Time.time >= lastShootTime + shootDelay && !isGunOverheated;
    }

    private void ResetHeatValues()
    {
        currentHeatValue = 0f;
        isGunOverheated = false;
        overheatSlider.value = 0f;
    }

    private void UpdateHeatSlider()
    {
        overheatSlider.value = currentHeatValue / maxHeatTime;
    }
    #endregion

    #region Shooting System
    public void Shoot()
    {
        if (isGunOverheated) return;

        shootingSystem.Play();

        Vector3 direction = AimAssist();
        if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, mask))
        {
            HandleBulletHit(hit);
        }
        else
        {
            HandleBulletMiss(direction, hit);
        }
    }

    private void HandleBulletHit(RaycastHit hit)
    {
        TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hit, hit.point, hit.normal, true));

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (hit.collider.TryGetComponent<DetectBullet>(out DetectBullet enemy))
            {
                enemy.TakeDamage(damageBullet);
            }
        }
        else
        {
            Debug.Log($"O'q {hit.collider.gameObject.layer} qatlamiga tegdi: {hit.point}");
        }
    }

    private void HandleBulletMiss(Vector3 direction, RaycastHit hit)
    {
        Vector3 farPoint = bulletSpawnPoint.position + direction * 1000f;
        TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hit, farPoint, Vector3.zero, false));
    }
    #endregion

    #region Aim Assist
    private Vector3 AimAssist()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aimAssistRadius, enemyLayer);

        if (enemies.Length > 0)
        {
            Collider nearestEnemy = FindNearestEnemy(enemies);
            if (nearestEnemy != null)
            {
                return (nearestEnemy.bounds.center - bulletSpawnPoint.position).normalized;
            }
        }

        return GetDirection();
    }

    private Collider FindNearestEnemy(Collider[] enemies)
    {
        Collider nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider enemy in enemies)
        {
            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;

            if (IsWithinAimAssistCone(directionToEnemy) &&
                distanceToEnemy < nearestDistance &&
                IsEnemyVisible(enemy))
            {
                nearestEnemy = enemy;
                nearestDistance = distanceToEnemy;
            }
        }

        return nearestEnemy;
    }

    private bool IsWithinAimAssistCone(Vector3 directionToEnemy)
    {
        Vector3 forward = transform.forward;

        float horizontalDot = Vector3.Dot(Vector3.ProjectOnPlane(directionToEnemy, Vector3.up).normalized, forward);
        float verticalDot = Vector3.Dot(directionToEnemy.normalized, forward);

        float horizontalAngleRad = horizontalAimAssistAngle * 0.5f * Mathf.Deg2Rad;
        float verticalAngleRad = verticalAimAssistAngle * 0.5f * Mathf.Deg2Rad;

        return Mathf.Acos(horizontalDot) <= horizontalAngleRad &&
               Mathf.Acos(verticalDot) <= verticalAngleRad;
    }

    private bool IsEnemyVisible(Collider enemy)
    {
        Vector3 directionToEnemy = enemy.bounds.center - bulletSpawnPoint.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        return Physics.Raycast(bulletSpawnPoint.position, directionToEnemy, out RaycastHit hit, distanceToEnemy, enemyLayer) &&
               hit.collider == enemy;
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (addBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-0.05f, 0.05f),
                Random.Range(-0.05f, 0.05f),
                Random.Range(-0.05f, 0.05f)
            );
            direction.Normalize();
        }

        return direction;
    }
    #endregion

    #region UI Controls
    private void SetupFireButton()
    {
        EventTrigger trigger = fireButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entryPointerDown.callback.AddListener((data) => { OnFireButtonDown((PointerEventData)data); });
        trigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entryPointerUp.callback.AddListener((data) => { OnFireButtonUp((PointerEventData)data); });
        trigger.triggers.Add(entryPointerUp);
    }

    private void UpdateSliderColor()
    {
        if (overheatSlider.fillRect != null)
        {
            Image fillImage = overheatSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float value = overheatSlider.value;
                fillImage.color = value < 0.5f ?
                    Color.Lerp(greenColor, yellowColor, value * 2f) :
                    Color.Lerp(yellowColor, redColor, (value - 0.5f) * 2f);
            }
        }
    }

    private void OnFireButtonDown(PointerEventData eventData)
    {
        isFireButtonPressed = true;
    }

    private void OnFireButtonUp(PointerEventData eventData)
    {
        isFireButtonPressed = false;
    }
    #endregion

    #region Visual Effects
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, Vector3 hitPoint, Vector3 hitNormal, bool madeImpact)
    {
        DetectBullet enemy = hit.collider.GetComponent<DetectBullet>();
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= bulletSpeed * Time.deltaTime;
            yield return null;
        }

        trail.transform.position = hitPoint;

        if (madeImpact)
        {
            Instantiate(impactParticleSystem, hitPoint, Quaternion.LookRotation(hitNormal));
        }
        if (enemy != null)
        {
            enemy.OnHit(hit.point, hit.normal);
        }
        Destroy(trail.gameObject, trail.time);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aimAssistRadius);

        DrawAimAssistCone(transform.position, transform.forward, horizontalAimAssistAngle, verticalAimAssistAngle, Color.magenta);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(bulletSpawnPoint.position, AimAssist() * 100f);
    }

    private void DrawAimAssistCone(Vector3 position, Vector3 forward, float horizontalAngle, float verticalAngle, Color color)
    {
        float horizontalRadius = Mathf.Tan(horizontalAngle * 0.5f * Mathf.Deg2Rad) * aimAssistRadius;
        float verticalRadius = Mathf.Tan(verticalAngle * 0.5f * Mathf.Deg2Rad) * aimAssistRadius;

        Vector3 topRight = position + (Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Quaternion.AngleAxis(verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 topLeft = position + (Quaternion.AngleAxis(-horizontalAngle, Vector3.up) * Quaternion.AngleAxis(verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 bottomRight = position + (Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Quaternion.AngleAxis(-verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 bottomLeft = position + (Quaternion.AngleAxis(-horizontalAngle, Vector3.up) * Quaternion.AngleAxis(-verticalAngle, Vector3.right) * forward * aimAssistRadius);

        Gizmos.color = color;

        Gizmos.DrawLine(position, topRight);
        Gizmos.DrawLine(position, topLeft);
        Gizmos.DrawLine(position, bottomRight);
        Gizmos.DrawLine(position, bottomLeft);

        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
    #endregion
}