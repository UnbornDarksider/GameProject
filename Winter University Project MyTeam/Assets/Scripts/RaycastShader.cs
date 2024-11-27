using UnityEngine;
using UnityEngine.UI;

public class DroneController : MonoBehaviour
{
    public float raycastLength = 50f;
    public LayerMask bombLayer;
    public LineRenderer ropeRenderer;
    public float liftSpeed = 5f;
    private GameObject attachedObject;
    private Rigidbody attachedRigidbody;
    private bool isHolding = false;
    public Text bombaAniqlandiText;
    private float textDisplayDuration = 1f;
    private float desiredDistance = 1.5f;
    
    void Update()
    {
        CastRay();
        HandleInput();
        UpdateRopeVisual();
    }

    void CastRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastLength, bombLayer))
        {
            if (hit.collider.gameObject.name == "Bomba" && !isHolding)
            {
                Debug.Log("Bomba obyekti aniqlandi");
                ShowBombaAniqlandiText();
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isHolding)
            {
                TryAttachObject();
            }
            else
            {
                DetachObject();
            }
        }
    }

    void TryAttachObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastLength, bombLayer))
        {
            if (hit.collider.gameObject.name == "Bomba")
            {
                attachedObject = hit.collider.gameObject;
                attachedRigidbody = attachedObject.GetComponent<Rigidbody>();
                isHolding = true;
                StartCoroutine(LiftBomb());
            }
        }
    }

    System.Collections.IEnumerator LiftBomb()
    {
        Vector3 initialPosition = attachedObject.transform.position;
        Vector3 targetPosition = transform.position - transform.up * desiredDistance;
        float journeyLength = Vector3.Distance(initialPosition, targetPosition);
        float startTime = Time.time;

        while (attachedObject.transform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * liftSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            attachedObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, fractionOfJourney);
            yield return null;
        }
    }

    void DetachObject()
    {
        if (attachedObject != null)
        {
            isHolding = false;
            attachedObject = null;
            attachedRigidbody = null;
        }
    }

    void FixedUpdate()
    {
        if (isHolding && attachedObject != null)
        {
            Vector3 targetPosition = transform.position - transform.up * desiredDistance;
            attachedObject.transform.position = targetPosition;
        }
    }

    void ShowBombaAniqlandiText()
    {
        if (bombaAniqlandiText != null)
        {
            bombaAniqlandiText.text = "Bomba aniqlandi!";
            Invoke("HideBombaAniqlandiText", textDisplayDuration);
        }
        else
        {
            Debug.LogWarning("BombaAniqlandiText obyekti topilmadi!");
        }
    }

    void HideBombaAniqlandiText()
    {
        if (bombaAniqlandiText != null)
        {
            bombaAniqlandiText.text = "";
        }
    }

    void UpdateRopeVisual()
    {
        if (isHolding && attachedObject != null)
        {
            ropeRenderer.enabled = true;
            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, attachedObject.transform.position);
        }
        else
        {
            ropeRenderer.enabled = false;
        }
    }
}