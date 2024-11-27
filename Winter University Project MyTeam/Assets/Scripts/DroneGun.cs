using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGun : MonoBehaviour
{
    public Transform gunTransform;
    public GameObject bulletPrefab;
    public float fireRate = 6;
    private float waitTilNextFire = 0.0f;
    private void Update()
    {
        ShootingBulet();
    }
    void ShootingBulet()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (waitTilNextFire <= 0)
            {
                Instantiate(bulletPrefab, gunTransform.position, gunTransform.rotation);
                waitTilNextFire = 1f;
            }
        }
        waitTilNextFire -= fireRate * Time.deltaTime;
    }
}
