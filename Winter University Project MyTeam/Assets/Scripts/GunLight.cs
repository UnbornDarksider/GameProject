using UnityEngine;

public class GunLight : MonoBehaviour
{
    [SerializeField]
    private Light pointLight;
    [SerializeField]
    private LayerMask mask;


    private void Start()
    {
        pointLight.enabled = true;
    }

    private void Update()
    {

        //CheckRaycastHit();

    }

    private void CheckRaycastHit()
    {
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity, mask))
        {
            float distance = Vector3.Distance(transform.position, hit.point);

            pointLight.intensity = distance;


            Debug.DrawLine(transform.position, transform.position + direction * 100, Color.red, 1.0f);
        }
    }
}