using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
    public Transform point;
    public Transform drone;
    public float distanceThreshold = 1000f;

    void Update()
    {
        Check();
        //float distance = Vector3.Distance(point.position, drone.position);
        //Debug.Log("Distance: " + distance);

        //if (distance > distanceThreshold)
        //{
        //    Debug.Log("Drone pointdan 1000 metr uzoqlashdi! Chegara tugadi!");

        //}
    }

    void Check()
    {

        Vector3 pointPosition = point.position;
        Vector3 dronePosition = drone.position;


        float distanceX = Mathf.Abs(pointPosition.x - dronePosition.x);
        float distanceY = Mathf.Abs(pointPosition.y - dronePosition.y);
        float distanceZ = Mathf.Abs(pointPosition.z - dronePosition.z);


        //Debug.Log($"X={distanceX}  Y= {distanceY} Z={distanceZ}");

    }
}


