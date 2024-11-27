using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public float detectionRange = 100f;
    public Transform player;
    public Camera playerCamera;
    public LayerMask enemyLayer;
    public UnityEngine.UI.Text enemyPositionText;

    void Update()
    {
        RaycastHit hit;

        // Raycastni boshlash va tekshirish
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, detectionRange, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Dushman topildi
                Transform enemy = hit.transform;
                Vector3 enemyPosition = enemy.position;

                // Dushmanning pozitsiyasini ekranga chiqarish
                if (enemyPositionText != null)
                {
                    enemyPositionText.text = $"Enemy Position: {enemyPosition}";
                }

                // Kamerani dushmanga qarating
                playerCamera.transform.LookAt(enemy);
            }
        }
    }
}
