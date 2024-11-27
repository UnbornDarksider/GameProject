using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.1f;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private float BulletSpeed = 100;

    [Range(0f, 1000f)]
    public int DamageBullet = 10;

    private float LastShootTime;
    private bool isGunOverheated = false;
    private float spaceKeyHoldTime = 0f;
    private float cooldownTime = 2f;
    private float maxHoldTime = 3f;
    private float singleShotCooldown = 0.1f;
    private float lastSingleShotTime = 0f;

    [SerializeField]
    private Slider overheatSlider;

    private Color greenColor = Color.green;
    private Color yellowColor = Color.yellow;
    private Color redColor = Color.red;

    private void Update()
    {
        if (isGunOverheated)
        {
            if (Time.time >= LastShootTime + cooldownTime)
            {
                isGunOverheated = false;
                overheatSlider.value = 0f;
            }
            else
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            spaceKeyHoldTime += Time.deltaTime;
            overheatSlider.value = Mathf.Clamp01(spaceKeyHoldTime / maxHoldTime);

            if (spaceKeyHoldTime >= maxHoldTime)
            {
                isGunOverheated = true;
                LastShootTime = Time.time;
                Debug.Log("Gun is overheated! Cooling down...");
            }
            else if (Time.time >= lastSingleShotTime + singleShotCooldown)
            {
                Shoot();
                lastSingleShotTime = Time.time;
            }
        }
        else
        {
            spaceKeyHoldTime = Mathf.Clamp(spaceKeyHoldTime - Time.deltaTime, 0f, maxHoldTime);
            overheatSlider.value = Mathf.Clamp01(spaceKeyHoldTime / maxHoldTime);
        }

        UpdateSliderColor();
    }

    private void UpdateSliderColor()
    {
        if (overheatSlider.fillRect != null)
        {
            Image fillImage = overheatSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float value = overheatSlider.value;

                if (value < 0.5f)
                {
                    fillImage.color = Color.Lerp(greenColor, yellowColor, value * 2f);
                }
                else
                {
                    fillImage.color = Color.Lerp(yellowColor, redColor, (value - 0.5f) * 2f);
                }
            }
        }
    }

    public void Shoot()
    {
        if (isGunOverheated)
        {
            return;
        }

        if (LastShootTime + ShootDelay < Time.time)
        {
            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                // Raycast chizig'ini ko'rinadigan qilish
                Debug.DrawLine(BulletSpawnPoint.position, hit.point, Color.red, 1.0f);

                // Urilgan obyektni tekshirish va dushman bo'lsa TakeDamage metodini chaqirish
                //DetectBullet enemy = hit.collider.GetComponent<DetectBullet>();
                //if (enemy != null)
                //{
                //    enemy.TakeDamage(DamageBullet);
                //}

                LastShootTime = Time.time;
            }
            else
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

                Vector3 endPosition = BulletSpawnPoint.position + GetDirection() * 100;
                StartCoroutine(SpawnTrail(trail, endPosition, Vector3.zero, false));

                // Agar hech narsa urilmasa, Raycast chizig'ini ko'rsatish
                Debug.DrawLine(BulletSpawnPoint.position, endPosition, Color.red, 1.0f);

                LastShootTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                                    Random.Range(-0.01f, 0.01f),
                                    Random.Range(-0.01f, 0.01f),
                                    Random.Range(-0.01f, 0.01f)
            );
            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }

        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
