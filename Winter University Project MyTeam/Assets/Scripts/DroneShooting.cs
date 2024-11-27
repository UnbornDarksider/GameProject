using UnityEngine;
using System.Collections; 

public class DroneShooting : MonoBehaviour
{
    public bool scriptEnabled = false;
    [Range(0,1000)]
    public float detectionRadius = 10f;    
    public LayerMask enemyLayer;           
    public ParticleSystem ShootingSystem;  
    public Transform bulletSpawnPoint;    
    public TrailRenderer BulletTrail;
    [Range(0.0f,1.0f)]
    public float fireRate = 1f;
    [Range(0,1000)]
    public float raycastRange = 100f;
    [Range(0,1000)]
    public int damage = 10;
    [Range(0,1000)]
    public float bulletSpeed = 20f;
    [Range(0,180)]
    public float maxRotationAngle = 20f;   

    private float nextFireTime = 0f;      

    void Update()
    {
        if (scriptEnabled)
        {
            DetectAndShoot();
        }
    }

    void DetectAndShoot()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy")) 
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                Vector3 directionToEnemy = enemy.transform.position - transform.position;
                directionToEnemy.y = 0; 

                float angle = Vector3.Angle(transform.forward, directionToEnemy);

                if (distanceToEnemy < closestDistance && angle <= maxRotationAngle)
                {
                    closestDistance = distanceToEnemy;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            if (Time.time >= nextFireTime) // O'q otish intervalini tekshirish
            {
                ShootAt(nearestEnemy);
                nextFireTime = Time.time + fireRate; // Keyingi o'q otish vaqtini yangilash
            }
        }
    }

    void ShootAt(Transform target)
    {
        // O'q otish effektini o'chirish va o'rnatish
        if (ShootingSystem != null)
        {
            ShootingSystem.transform.position = bulletSpawnPoint.position;
            ShootingSystem.Play();
        }

        Vector3 randomTargetPoint = GetRandomTargetPoint(target);
        Vector3 direction = (randomTargetPoint - bulletSpawnPoint.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(bulletSpawnPoint.position, direction, out hit, raycastRange))
        {
            Debug.DrawRay(bulletSpawnPoint.position, direction * hit.distance, Color.red, 1.0f); // Raycast chizig'ini ko'rish uchun

            // O'q izi (trail) qo'shish
            TrailRenderer trail = Instantiate(BulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit.point));

            // Urilgan obyektni tekshirish va dushman bo'lsa zararni qo'llash
            //DetectBullet enemy = hit.collider.GetComponent<DetectBullet>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);
            //}

            // Hit haqida xabar chiqarish
            Debug.Log("Hit: " + hit.collider.name);
        }
        else
        {
            // O'q izi (trail) qo'shish, agar hech narsa urilmasa
            TrailRenderer trail = Instantiate(BulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            Vector3 endPosition = bulletSpawnPoint.position + direction * raycastRange;
            StartCoroutine(SpawnTrail(trail, endPosition));
        }
    }

    Vector3 GetRandomTargetPoint(Transform target)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        Bounds bounds = targetCollider.bounds;

        // Obyektning yuqori, o'rta va pastki qismlari
        Vector3 topPoint = bounds.center + new Vector3(0, bounds.extents.y, 0);
        Vector3 middlePoint = bounds.center;
        Vector3 bottomPoint = bounds.center - new Vector3(0, bounds.extents.y, 0);

        // Tasodifiy nuqtani tanlash
        Vector3[] targetPoints = { topPoint, middlePoint, bottomPoint };
        return targetPoints[Random.Range(0, targetPoints.Length)];
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPosition)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, endPosition);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (remainingDistance / distance));
            remainingDistance -= Time.deltaTime * bulletSpeed; // Trail tezligi
            yield return null;
        }

        trail.transform.position = endPosition;
        Destroy(trail.gameObject, trail.time);
    }
}
