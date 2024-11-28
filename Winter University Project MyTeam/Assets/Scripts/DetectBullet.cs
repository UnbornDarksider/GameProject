
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBullet : MonoBehaviour
{
    [Range(0, 1000)]
    public int health = 100;
    private Vector3 newPosition;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject deadEffect;
    [SerializeField] private string bulletTag = "Bullet";
    //public GameObject healthBar;
    //private void Start()
    //{
    //    healthBar.GetComponent<HealthScript>().SetMaxHealth(health);
    //}

    //private void Update()
    //{
    //    healthBar.GetComponent<HealthScript>().SetHealth(health);
    //}

    public void TakeDamage(int damage)
    {
        //healthBar.SetActive(true);
        health -= damage;
        Debug.Log("Tegdi. Damage: " + damage);


        if (health <= 0)
        {
            newPosition = transform.position;
            newPosition.y += 2;
            Debug.Log("Enemy destroyed");
            Instantiate(deadEffect, newPosition,transform.rotation);
            Destroy(gameObject);
        }
    }
    public void OnHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Instantiate the blood effect at the hit point
        Quaternion rotation = Quaternion.LookRotation(hitNormal);
        Instantiate(bloodEffectPrefab, hitPoint, rotation);
    }
}
