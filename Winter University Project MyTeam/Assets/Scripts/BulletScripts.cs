using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScripts : MonoBehaviour
{
    public float bulletForce = 4000.0f;
    private float maxDistance = 500f;
    private float currentDistance;
    public GameObject bulletHole_prefab;
    public GameObject bloodHole_Prefab;
    private void Awake()
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletForce);
        currentDistance = maxDistance;
    }
    private void FixedUpdate()
    {

        BulletLife();
    }

    void BulletLife()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            currentDistance = hit.distance;
            Debug.Log("Tegdi: " + hit.collider.gameObject.name);
            Debug.Log(hit.distance);
            if (hit.collider.gameObject.name == "Cube" && (hit.distance < 3))
            {
                Vector3 position = hit.point + (hit.normal);
                Vector3 lookRotation = hit.normal;
                GameObject bulletHole = Instantiate(bloodHole_Prefab, position, Quaternion.LookRotation(lookRotation));
                Destroy(gameObject);
                Destroy(bulletHole, 4f);
            }
            if (hit.distance < 3)
            {
                Vector3 position = hit.point + (hit.normal);
                Vector3 lookRotation = hit.normal;
                GameObject bulletHole = Instantiate(bulletHole_prefab, position, Quaternion.LookRotation(lookRotation));
                Destroy(gameObject);
                Destroy(bulletHole, 4f);
            }

        }
        Debug.DrawRay(origin, direction * currentDistance, Color.yellow);
        Destroy(gameObject, 1.0f);

    }

}
