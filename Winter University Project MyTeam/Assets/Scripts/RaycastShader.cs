using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RaycastShader : MonoBehaviour
{
    #region Variables
    [Header("Raycast Settings")]
    [SerializeField] private float raycastLength = 10f;
    [SerializeField] private LayerMask bombLayer;
    [SerializeField] private LayerMask mineLayer;
    [SerializeField] private LayerMask buildingLayer;

    [Header("Rope Settings")]
    [SerializeField] private LineRenderer ropeRenderer;
    [SerializeField] private float liftSpeed = 5f;

    [Header("Physics Settings")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private float positionSmoothing = 10f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float fallingGravityMultiplier = 100f;

    [Header("UI Elements")]
    [SerializeField] private Button bombButton;
    [SerializeField] private Button mineButton;
    [SerializeField] private Button throwButton;

    [Header("Save Settings")]
    [SerializeField] private float saveInterval = 1f;

    private GameObject attachedObject;
    private Rigidbody attachedRigidbody;
    private Coroutine liftCoroutine;
    private List<GameObject> bombsInScene = new List<GameObject>();
    private List<GameObject> minesInScene = new List<GameObject>();

    private bool isHolding;
    private bool isOverSklad;
    private float saveTimer;
    private const float desiredDistance = 2f;

    private int index = 0;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        HandleSaveTimer();
        CastRay();
        UpdateRopeVisual();
        if (throwButton.gameObject.activeSelf)
        {
            if ((bombsInScene[index] == null || minesInScene[index] == null))
            {
                isHolding = false;
                CleanupAttachedObject();
                UpdateButtonStates();
            }
        }
    }

    private void FixedUpdate()
    {
        HandleAttachedObjectPhysics();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {

        FindObjects();
        UpdateButtonStates();
    }

    private void FindObjects()
    {
        bombsInScene = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bomb"));
        minesInScene = new List<GameObject>(GameObject.FindGameObjectsWithTag("Mina"));
    }
    #endregion

    #region UI Management
    private void UpdateButtonStates()
    {
        if (bombButton != null)
        {
            bool shouldShowBombButton = isOverSklad && bombsInScene.Count > 0 && !isHolding;
            bombButton.interactable = shouldShowBombButton;
            bombButton.gameObject.SetActive(shouldShowBombButton);
        }

        if (mineButton != null)
        {
            bool shouldShowMineButton = isOverSklad && minesInScene.Count > 0 && !isHolding;
            mineButton.interactable = shouldShowMineButton;
            mineButton.gameObject.SetActive(shouldShowMineButton);
        }

        if (throwButton != null)
        {
            throwButton.gameObject.SetActive(isHolding);
        }
    }
    #endregion

    #region Raycast System
    private void CastRay()
    {
        CheckBuildingRaycast();
        CheckItemRaycast();
    }

    private void CheckBuildingRaycast()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, raycastLength, buildingLayer))
        {
            isOverSklad = IsSkladBuilding(hit.collider.gameObject.name);
        }
        else
        {
            isOverSklad = false;
        }
        UpdateButtonStates();
    }

    private bool IsSkladBuilding(string buildingName)
    {
        string lowerName = buildingName.ToLower();
        return lowerName.Contains("payload1") ||
               lowerName.Contains("payload2") ||
               lowerName.Contains("payload3");
    }

    private void CheckItemRaycast()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, raycastLength, bombLayer | mineLayer))
        {
            if ((hit.collider.CompareTag("Bomb") || hit.collider.CompareTag("Mina")) && !isHolding)
            {
                Debug.Log($"{hit.collider.gameObject.name} obyekti aniqlandi");
            }
        }
    }
    #endregion

    #region Object Interaction
    public void TakeBomb()
    {
        if (bombsInScene.Count > 0 && !isHolding && isOverSklad)
        {
            index = Random.Range(0, bombsInScene.Count);
            TryAttachObject(bombsInScene[index]);
        }
    }

    public void TakeMine()
    {
        if (minesInScene.Count > 0 && !isHolding && isOverSklad)
        {
            index = Random.Range(0, minesInScene.Count);
            TryAttachObject(minesInScene[index]);
        }
    }

    private void TryAttachObject(GameObject objectToAttach)
    {
        if (objectToAttach == null || isHolding) return;

        attachedObject = objectToAttach;
        attachedRigidbody = attachedObject.GetComponent<Rigidbody>();
        isHolding = true;
        attachedRigidbody.useGravity = false;

        if (liftCoroutine != null)
        {
            StopCoroutine(liftCoroutine);
        }
        liftCoroutine = StartCoroutine(LiftObject());

        RemoveObjectFromList(objectToAttach);
        UpdateButtonStates();
    }

    private void RemoveObjectFromList(GameObject obj)
    {
        if (obj.CompareTag("Bomb"))
        {
            bombsInScene.Remove(obj);
        }
        else if (obj.CompareTag("Mina"))
        {
            minesInScene.Remove(obj);
        }
    }

    public void ThrowObject()
    {
        if (attachedObject == null)
        {

            return;
        }

        isHolding = false;
        attachedRigidbody.useGravity = true;

        Vector3 throwDirection = transform.forward + transform.up * 0.5f;
        Vector3 throwVelocity = throwDirection.normalized * throwForce;

        attachedRigidbody.linearVelocity = throwVelocity;
        attachedRigidbody.AddForce(Vector3.down * gravity * fallingGravityMultiplier * attachedRigidbody.mass);

        CleanupAttachedObject();
        UpdateButtonStates();
    }

    private void CleanupAttachedObject()
    {
        attachedObject = null;
        attachedRigidbody = null;

        if (liftCoroutine != null)
        {
            StopCoroutine(liftCoroutine);
            liftCoroutine = null;
        }
    }
    #endregion

    #region Physics and Visual Effects
    private IEnumerator LiftObject()
    {
        if (attachedObject == null) yield break;

        Vector3 initialPosition = attachedObject.transform.position;
        float startTime = Time.time;

        while (attachedObject != null && isHolding)
        {
            Vector3 targetPosition = transform.position - transform.up * desiredDistance;
            float journeyLength = Vector3.Distance(initialPosition, targetPosition);
            float distanceCovered = (Time.time - startTime) * liftSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            if (attachedObject != null)
            {
                attachedObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, fractionOfJourney);
            }
            else
            {
                yield break;
            }

            if (fractionOfJourney >= 1f) break;

            yield return null;
        }
    }

    private void HandleAttachedObjectPhysics()
    {
        if (!isHolding || attachedObject == null) return;

        Vector3 targetPosition = transform.position - transform.up * desiredDistance;
        Vector3 currentPosition = attachedObject.transform.position;

        Vector3 smoothedPosition = Vector3.Lerp(
            currentPosition,
            targetPosition,
            Time.fixedDeltaTime * positionSmoothing
        );

        attachedRigidbody.linearVelocity = Vector3.Lerp(
            attachedRigidbody.linearVelocity,
            (smoothedPosition - currentPosition) / Time.fixedDeltaTime,
            Time.fixedDeltaTime * damping
        );

        attachedRigidbody.AddForce(Vector3.down * gravity * attachedRigidbody.mass);
    }

    private void UpdateRopeVisual()
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

    private void HandleSaveTimer()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= saveInterval)
        {
            saveTimer = 0f;
            FindObjects();
        }
    }
    #endregion
}