//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DroneMoveScript : MonoBehaviour
//{
//    #region
//    private Rigidbody ourDrone;

//    [Range(-1, 1)]
//    public float sensitivity = 0.2f;

//    [Range(0, 3000)]
//    public float moveForwardSpeed = 500.0f;

//    [Range(0, 1500)]
//    public float sideMoveAmount = 300.0f;

//    [Range(0, 1500)]
//    public float Up = 450.0f;

//    [Range(0, 1000)]
//    public float Down = 200.0f;

//    [Range(0, 20)]
//    public float rotateAmountByKeys = 2.5f;

//    [Range(-1000, 3000)]
//    public float upForce;

//    private float tiltAmountForward = 0;
//    private float tiltVelocityForward;
//    private float tiltAmountSideways;
//    private float tiltAmountVelocity;
//    private float wantedYRotation;
//    [HideInInspector] public float currentYRotation;
//    private float rotataYVelocity;
//    private Vector3 velocityToSmoothDampToZero;
//    public AudioSource droneSound;

//    private float I;
//    private float J;
//    private float K;
//    private float L;


//    #endregion

//    private void Awake()
//    {
//        ourDrone = GetComponent<Rigidbody>();
//        droneSound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();
//    }




//    private void FixedUpdate()
//    {
//        //MouseMov();
//        MoveUpDown();
//        MovementForward();
//        Rotation();
//        ClampingSpeed();
//        Swerv();
//        DroneSound();
//        Turn180And360();



//        ourDrone.AddRelativeForce(Vector3.up * upForce);
//        ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));

//    }

//    private void MoveUpDown()
//    {
//        if (Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity || Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            if (Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K))
//            {
//                ourDrone.linearVelocity = ourDrone.linearVelocity;
//            }
//            else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
//            {
//                ourDrone.linearVelocity = new Vector3(ourDrone.linearVelocity.x, Mathf.Lerp(ourDrone.linearVelocity.y, 0, Time.deltaTime * 5), ourDrone.linearVelocity.z);
//                upForce = 300;// 281;
//            }
//            else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
//            {
//                ourDrone.linearVelocity = new Vector3(ourDrone.linearVelocity.x, Mathf.Lerp(ourDrone.linearVelocity.y, 0, Time.deltaTime * 5), ourDrone.linearVelocity.z);
//                upForce = 299;
//            }
//            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
//            {
//                upForce = 410;
//            }
//        }

//        if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            upForce = 150;// 135;
//        }

//        if (Input.GetKey(KeyCode.I))
//        {
//            upForce = Up;
//            if (Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//            {
//                upForce = 500;
//            }
//        }
//        if (Input.GetKey(KeyCode.K))
//        {
//            upForce = -Down;
//        }
//        if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity) )
//        {
//            upForce = 98.1f;
//        }
//    }

//    private void MovementForward()
//    {
//        if (Input.GetAxis("Vertical") != 0)
//        {
//            //upForce = 400;
//            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * moveForwardSpeed);
//            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 8 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.05f);
//        }
//    }

//    private void Rotation()
//    {
//        if (Input.GetKey(KeyCode.A))
//        {
//            wantedYRotation -= rotateAmountByKeys;
//        }
//        if (Input.GetKey(KeyCode.D))
//        {
//            wantedYRotation += rotateAmountByKeys;
//        }


//        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotataYVelocity, 0.25f);
//    }

//    private void ClampingSpeed()
//    {
//        if (Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            ourDrone.linearVelocity = Vector3.SmoothDamp(ourDrone.linearVelocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
//        }
//        else if (Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity)
//        {
//            ourDrone.linearVelocity = Vector3.SmoothDamp(ourDrone.linearVelocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
//        }
//        else if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            ourDrone.linearVelocity = Vector3.SmoothDamp(ourDrone.linearVelocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
//        }
//        else if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity)
//        {
//            ourDrone.linearVelocity = Vector3.SmoothDamp(ourDrone.linearVelocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
//        }
//    }

//    private void Swerv()
//    {
//        if (Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            ourDrone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMoveAmount);
//            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.05f);
//        }
//        else
//        {
//            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
//        }
//    }
//    private void Turn180And360()
//    {
//        if (Input.GetKey(KeyCode.Q) && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            ourDrone.AddRelativeForce(Vector3.right * Mathf.Abs(Input.GetAxis("Horizontal")) * sideMoveAmount);
//            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * Mathf.Abs(Input.GetAxis("Horizontal")), ref tiltAmountVelocity, 0.1f);
//        }
//        if (Input.GetKey(KeyCode.Q) && Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity)
//        {
//            ourDrone.AddRelativeForce(Vector3.right * Mathf.Abs(Input.GetAxis("Vertical")) * sideMoveAmount);
//            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * Mathf.Abs(Input.GetAxis("Vertical")), ref tiltAmountVelocity, 0.1f);
//        }
//        if (Input.GetKey(KeyCode.E) && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
//        {
//            ourDrone.AddRelativeForce(Vector3.right * -Mathf.Abs(Input.GetAxis("Horizontal")) * sideMoveAmount);
//            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * -Mathf.Abs(Input.GetAxis("Horizontal")), ref tiltAmountVelocity, 0.1f);
//        }
//        if (Input.GetKey(KeyCode.E) && Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity)
//        {
//            ourDrone.AddRelativeForce(Vector3.right * -Mathf.Abs(Input.GetAxis("Vertical")) * sideMoveAmount);
//            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * -Mathf.Abs(Input.GetAxis("Vertical")), ref tiltAmountVelocity, 0.1f);
//        }


//    }

//    private void DroneSound()
//    {
//        droneSound.pitch = 1 + (ourDrone.linearVelocity.magnitude / 90);
//    }
//}

using UnityEngine;

public class DroneMoveScript : MonoBehaviour
{
    #region Component References
    public Rigidbody ourDrone;
    public AudioSource droneSound;
    #endregion

    #region Control Parameters
    [Header("========== Sensitivity Settings ==========")]
    [Range(-1, 1)]
    public float sensitivity;


    [Header("========== Movement Settings ==========")]
    private float defaultspeed = 500.0f;
    [Range(0, 1500)] public float sideMoveAmount = 300.0f;
    [Range(0, 1500)] public float Up = 300.0f;
    [Range(0, 1000)] public float Down = 150.0f;
    [Range(0, 20)] public float rotateAmountByKeys = 2.5f;
    [Range(-1000, 3000)] public float upForce;
    #endregion

    #region Physics Parameters
    [Header("========== Physics Settings ==========")]
    public float accelerationSpeed = 0.0f;
    public float maxSpeed = 15f;
    public float clampingTime = 0.2f;
    public float stopThreshold = 0.25f;
    #endregion

    #region Tilt Variables
    [Header("========== Tilt Settings ==========")]
    public float tiltAmountForward = 0;
    public float tiltVelocityForward;
    public float tiltAmountSideways;
    public float tiltAmountVelocity;
    #endregion

    #region Rotation Variables
    [Header("========== Rotation Settings ==========")]
    public float wantedYRotation;
    [HideInInspector] public float currentYRotation;
    public float rotataYVelocity;
    #endregion

    #region Input Variables
    private Vector3 velocityToSmoothDampToZero;
    private Vector3 velocityToSmoothDampToZero1;
    #endregion

    #region State Variables
    public bool isActive = true;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        UpdateControlLoop();
        ApplyFinalForces();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        ourDrone = GetComponent<Rigidbody>();
        droneSound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();
    }
    #endregion

    #region Main Control Loop
    private void UpdateControlLoop()
    {
        MoveUpDown();
        MovementForward();
        Rotation();
        ClampingSpeed();
        Swerv();
        DroneSound();
    }

    private void ApplyFinalForces()
    {
        ourDrone.AddRelativeForce(Vector3.up * upForce);
        ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));
    }
    #endregion


    #region Movement Control
    public void MoveUpDown()
    {
        bool isMoving = Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity || Mathf.Abs(Mathf.Abs(Input.GetAxis("Horizontal"))) > sensitivity;
        bool isRotating = !Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) || !Input.GetKey(KeyCode.J)  || !Input.GetKey(KeyCode.L); 

        if (isMoving)
        {
            HandleMovingState();
        }

        HandleSpecialCases();
    }

    private void HandleMovingState()
    {
        if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K))
        {
            ourDrone.linearVelocity = ourDrone.linearVelocity;
        }
        else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.J) &&Input.GetKey(KeyCode.L))
        {
            StabilizeVerticalMovement(281f);
        }
        else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L))
        {
            StabilizeVerticalMovement(110f);
        }
    }

    private void HandleSpecialCases()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            upForce = 135f;
        }

        if (Input.GetKey(KeyCode.I) ||  Input.GetKey(KeyCode.K))
        {
            HandleUpwardMovement();
        }
        else if (Input.GetKey(KeyCode.I) ||   Input.GetKey(KeyCode.K))
        {
            upForce = -Down;
        }
        else if (IsIdle())
        {
            upForce = 98.1f;
        }
    }

    private void StabilizeVerticalMovement(float stabilizationForce)
    {
        ourDrone.linearVelocity = new Vector3(ourDrone.linearVelocity.x, Mathf.Lerp(ourDrone.linearVelocity.y, 0, Time.deltaTime * 5), ourDrone.linearVelocity.z);
        upForce = stabilizationForce;
    }

    private void HandleUpwardMovement()
    {
        upForce = Up;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            upForce = 500f;
        }
    }

    private bool IsIdle()
    {
        return Input.GetKey(KeyCode.I) ||   Input.GetKey(KeyCode.K) && Mathf.Abs(Mathf.Abs(Input.GetAxis("Vertical"))) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity;
    }
    #endregion

    #region Forward Movement
    public void MovementForward()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) != 0)
        {
            ApplyForwardForce();
        }
        else
        {
            ResetForwardTilt();
        }
    }

    private void ApplyForwardForce()
    {
        ourDrone.AddRelativeForce(Vector3.forward *Input.GetAxis("Vertical") * defaultspeed);
        tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 7 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);
    }

    private void ResetForwardTilt()
    {
        tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 0, ref tiltVelocityForward, 0.05f);
    }
    #endregion

    #region Rotation Control
    public void Rotation()
    {
        HandleHorizontalRotation();
        HandleVerticalMovement();
        SmoothRotation();
    }

    private void HandleHorizontalRotation()
    {
        if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.L))
        {
            wantedYRotation += rotateAmountByKeys * 0.5f;
        }
    }

    private void HandleVerticalMovement()
    {
        if (Input.GetKey(KeyCode.I) ||  Input.GetKey(KeyCode.K)) 
        {
            float clampedVerticalInput = Mathf.Clamp(0.6f, -2f, 2f);
            upForce = clampedVerticalInput * (clampedVerticalInput > 0 ? Up : Down);
        }
        else
        {
            upForce = 98.1f;
        }
    }

    private void SmoothRotation()
    {
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotataYVelocity, 0.25f);
    }
    #endregion

    #region Speed Control
    public void ClampingSpeed()
    {
        bool isJoystickActive = Mathf.Abs(Mathf.Abs(Input.GetAxis("Vertical"))) > sensitivity || Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity;
        bool isInputActive = Input.GetKey(KeyCode.J) &&  Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.I) ||   Input.GetKey(KeyCode.K) ;

        if (isJoystickActive || isInputActive)
        {
            HandleActiveMovement();
        }
        else
        {
            HandleDeceleration();
        }
    }

    private void HandleActiveMovement()
    {
        ourDrone.linearDamping = 0;
        ourDrone.angularDamping = 0;

        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * accelerationSpeed;
        ourDrone.linearVelocity = Vector3.ClampMagnitude(ourDrone.linearVelocity + targetVelocity * Time.deltaTime, maxSpeed);
    }

    private void HandleDeceleration()
    {
        ourDrone.linearVelocity = Vector3.SmoothDamp(ourDrone.linearVelocity, Vector3.zero, ref velocityToSmoothDampToZero1, clampingTime);

        if (ourDrone.linearVelocity.magnitude < stopThreshold)
        {
            ApplyStoppingForces();
        }
    }

    private void ApplyStoppingForces()
    {
        ourDrone.linearDamping = 10f;
        ourDrone.angularDamping = 10f;
        ourDrone.linearVelocity = Vector3.zero;
    }
    #endregion

    #region Side Movement
    public void Swerv()
    {
        if (Mathf.Abs(Mathf.Abs(Input.GetAxis("Horizontal"))) > sensitivity)
        {
            ApplySideMovement();
        }
        else
        {
            ResetSidewaysTilt();
        }
    }

    private void ApplySideMovement()
    {
        ourDrone.AddRelativeForce(Vector3.right * Mathf.Abs(Input.GetAxis("Horizontal")) * sideMoveAmount);
        tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Mathf.Abs(Input.GetAxis("Horizontal")), ref tiltAmountVelocity, 0.1f);
    }

    private void ResetSidewaysTilt()
    {
        tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
    }
    #endregion

    #region Audio
    public void DroneSound()
    {
        droneSound.pitch = 1 + (ourDrone.linearVelocity.magnitude / 90);
    }
    #endregion
}