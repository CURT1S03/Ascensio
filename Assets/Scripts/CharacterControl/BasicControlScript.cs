using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(CharacterInputController))]
public class BasicControlScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private Rigidbody rbody;
    private CharacterInputController cinput;
    private ConstantForce cforce;
    private GameObject meshObject;
    private Material mesh;
    private Color defaultColor;

    private float jumpStart = 0;
    private bool addForce = false;
    private bool holdingJump = false;
    private bool setGrav = false;
    private Vector3 forceVector = new Vector3(0, 0, 0);
    private bool isTurning = false;
    private bool isMoving = false;
    private bool isRunning = false;
    private float inputForward;
    private float inputTurn;

    public Color hitColor = new Color(0f, 0f, 0f);

    [Header("Movement Settings")]
    public float forwardMaxSpeed = 5f;
    public float turnMaxSpeed = 120f;

    [Header("Jump Settings")]
    public float jumpForce = 1f;

    [Tooltip("Time (s) space held down for max jump height")]
    public float maxJumpTime = 1f;

    [Tooltip("Default character gravity")]
    public float gravity = Physics.gravity.y;

    [Tooltip("Factor to reduce gravity by while holding space")]
    public float jumpGravity = 0;

    public float jumpableGroundNormalMaxAngle = 45f;
    public bool closeToJumpableGround;
    private int groundContactCount = 0;

    public bool IsGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }

    private bool isGrounded = true;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null) Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();
        if (rbody == null) Debug.Log("Rigid body could not be found");

        cinput = GetComponent<CharacterInputController>();
        if (cinput == null) Debug.Log("CharacterInputController could not be found");

        cforce = GetComponent<ConstantForce>();
        if (cforce == null) Debug.Log("Constant Force could not be found");

        meshObject = transform.Find("Model/Kitty_001")?.gameObject;
        if (meshObject == null)
            Debug.Log("Model could not be found");
        else
        {
            var renderer = meshObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                mesh = renderer.material;
                defaultColor = mesh.color;
            }
        }

        // Freeze rotation to prevent tipping
        rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        if (anim) anim.applyRootMotion = false;
        cforce.force = new Vector3(0f, gravity - Physics.gravity.y, 0f);
    }

    void FixedUpdate()
    {
        // --- UPDATED GROUND CHECK ---
        // Uses the 'groundContactCount' which we updated in OnCollisionEnter/Exit below
        isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(
            this.transform.position,
            jumpableGroundNormalMaxAngle,
            1f,
            1f,
            out closeToJumpableGround
        );

        inputForward = 0f;
        inputTurn = 0f;

        if (cinput.enabled)
        {
            inputForward = cinput.Forward;
            inputTurn = cinput.Turn;
        }

        const float dead = 0.05f;
        if (Mathf.Abs(inputForward) < dead)
        {
            inputForward = 0f;
            isMoving = false;
        }
        else isMoving = true;

        if (Mathf.Abs(inputTurn) < dead)
        {
            inputTurn = 0f;
            isTurning = false;
        }
        else isTurning = true;

        if (inputForward < 0f) inputTurn = -inputTurn;

        if (addForce)
        {
            rbody.AddForce(forceVector, ForceMode.Impulse);
            addForce = false;
        }

        // Calculate gravity logic
        float newGrav = gravity * (gravity + 2 * Physics.gravity.y) / (jumpGravity * gravity + 2 * Physics.gravity.y * jumpGravity + gravity) - Physics.gravity.y;

        if (jumpGravity > 0 && setGrav && holdingJump)
        {
            cforce.force = new Vector3(0f, newGrav, 0f);
            setGrav = false;
        }

        if (setGrav && !holdingJump)
        {
            cforce.force = new Vector3(0f, gravity - Physics.gravity.y, 0f);
            setGrav = false;
        }

        // Movement
        float runMultiplier = isRunning ? 1.8f : 1.0f;

        if (isMoving)
        {
            float moveSpeed = forwardMaxSpeed * runMultiplier;
            rbody.MovePosition(rbody.position + inputForward * moveSpeed * Time.fixedDeltaTime * transform.forward);
        }

        // Rotation
        float turnSpeed = isTurning && !isMoving ? turnMaxSpeed * 0.6f : turnMaxSpeed;
        rbody.angularVelocity = Vector3.zero;

        if (isTurning && turnSpeed > 0f)
        {
            var deltaRot = Quaternion.AngleAxis(inputTurn * Time.fixedDeltaTime * turnSpeed, Vector3.up);
            rbody.MoveRotation(rbody.rotation * deltaRot);
        }

        // Animator Logic
        if (anim)
        {
            bool isTurningOnly = isTurning && !isMoving;

            if (!isGrounded)
            {
                SetAnimFloat(anim, "Vert", 0f, 0.08f, Time.deltaTime);
                SetAnimFloat(anim, "State", 0f, 0.08f, Time.deltaTime);
                anim.SetBool("isFalling", true);
                anim.speed = 1f;
            }
            else
            {
                float targetVert = isTurningOnly ? 1f : (isMoving ? 1f : 0f);
                float targetState = isTurningOnly ? 0f : ((isRunning && isGrounded) ? 1f : 0f);

                SetAnimFloat(anim, "Vert", targetVert, 0.06f, Time.deltaTime);
                SetAnimFloat(anim, "State", targetState, 0.06f, Time.deltaTime);
                anim.SetBool("isFalling", false); // Added this back so falling animation stops when grounded
                anim.speed = 1f;
            }
        }
    }

    void Update()
    {
        if (!addForce) forceVector = Vector3.zero;

        // Jump Logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpStart = Time.time;
            forceVector += Vector3.up * jumpForce;
            holdingJump = true;
            addForce = true;
            setGrav = true;
        }

        if ((!Input.GetKey(KeyCode.Space) || (IsGrounded && Time.time - jumpStart > .05f) || Time.time - jumpStart >= maxJumpTime) && holdingJump)
        {
            holdingJump = false;
            setGrav = true;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;
    }

    void SetAnimFloat(Animator animator, string name, float value, float dampTime, float deltaTime)
    {
        if (animator.GetFloat(name) > .001 || value > 0)
            animator.SetFloat(name, value, dampTime, deltaTime);
        else if (value == 0)
            animator.SetFloat(name, 0);
    }

    // --- PHYSICS CALLBACKS UPDATED HERE ---

    void OnCollisionEnter(Collision collision)
    {
        // UPDATED: Added check for "Plane" tag using CompareTag (better performance)
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("Plane"))
            ++groundContactCount;

        if (collision.gameObject.CompareTag("enemy"))
        {
            if (meshObject != null && mesh != null)
                mesh.color = defaultColor + hitColor;

            EventManager.TriggerEvent<EnemyCollisionEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // UPDATED: Added check for "Plane" tag
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("Plane"))
            --groundContactCount;

        if (collision.gameObject.CompareTag("enemy"))
        {
            if (meshObject != null && mesh != null)
                mesh.color = defaultColor;

            EventManager.TriggerEvent<HissEvent, Vector3>(this.gameObject.transform.position);
        }
    }
}