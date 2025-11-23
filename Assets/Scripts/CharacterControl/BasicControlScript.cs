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
    public class Feet
    {
        public GameObject Foot {get;set;}
        public bool StepTriggered {get;set;}
    }
    private Feet[] feet=new Feet[4];

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
    public float footstepWeight = 1;
    private CharacterCommon groundChecker;
    public CharacterCommon.GroundHit playerGround;

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

        groundChecker = GetComponent<CharacterCommon>();
        if (groundChecker == null) Debug.Log("CharacterCommon could not be found");

        playerGround = new CharacterCommon.GroundHit();
        playerGround.DistanceToGround = 100f;
        playerGround.IsJumpable = false;

        for(int i = 0; i < 4; i++)
        {
            feet[i] = new Feet();
        }

        feet[0].Foot = transform.Find("Model/Kitty_001/Kitty_001_rig/Root/spine.005/shoulder.L/thigh.L/shin.L/foot.L/toe.L")?.gameObject;
        feet[1].Foot = transform.Find("Model/Kitty_001/Kitty_001_rig/Root/spine.005/shoulder.R/thigh.R/shin.R/foot.R/toe.R")?.gameObject;
        feet[2].Foot = transform.Find("Model/Kitty_001/Kitty_001_rig/Root/spine.005/spine.006/spine.007/front_shoulder.L/front_thigh.L/front_shin.L/front_foot.L")?.gameObject;
        feet[3].Foot = transform.Find("Model/Kitty_001/Kitty_001_rig/Root/spine.005/spine.006/spine.007/front_shoulder.R/front_thigh.R/front_shin.R/front_foot.R")?.gameObject;

        for(int i = 0; i < 4; i++)
        {
            if(!feet[i].Foot)
                Debug.Log("Foot " + (i + 1) + " not found.");
            feet[i].StepTriggered = false;
        }
            
        // Freeze rotation to prevent tipping
        rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }

    void Start()
    {
        if (anim) anim.applyRootMotion = false;
        if (cforce) cforce.force = new Vector3(0f, gravity - Physics.gravity.y, 0f);
    }

    void FixedUpdate()
    {
        if (addForce)
        {
            rbody.AddForce(forceVector, ForceMode.Impulse);
            addForce = false;
        }

        // Calculate gravity logic
        float newGrav = gravity * (gravity + 2 * Physics.gravity.y) / (jumpGravity * gravity + 2 * Physics.gravity.y * jumpGravity + gravity) - Physics.gravity.y;

        if (jumpGravity > 0 && setGrav && holdingJump)
        {
            if (cforce) cforce.force = new Vector3(0f, newGrav, 0f);
            setGrav = false;
        }

        if (setGrav && !holdingJump)
        {
            if (cforce) cforce.force = new Vector3(0f, gravity - Physics.gravity.y, 0f);
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
    }

    void Update()
    {
        if (!addForce) forceVector = Vector3.zero;

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

        // --- UPDATED GROUND CHECK ---
        // Uses the 'groundContactCount' which we updated in OnCollisionEnter/Exit below
        if(groundChecker != null)
        {
            groundChecker.CheckGroundNear(
                this.transform.position,
                jumpableGroundNormalMaxAngle,
                100f,
                1f
            );
            playerGround = groundChecker.gh;
            closeToJumpableGround = groundChecker.gh.IsJumpable;
            isGrounded = IsGrounded || groundChecker.gh.DistanceToGround < 1f;
        }
        else
        {
            isGrounded = IsGrounded;
        }
        

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

        // Footstep
        if(isMoving || isTurning)
        {
            for(int i = 0; i < feet.Length; i++)
            {
                if(feet[i].Foot)
                {
                    float comparison = -0.715f; //hind foot walking comparison

                    if(i > 1) //front foot comparison
                    {
                        if(isRunning) comparison = -0.765f;
                        else comparison = -0.705f;
                    }
                    else if (isRunning) comparison = -0.758f; //hind foot running comparison
                    
                    float pos = feet[i].Foot.transform.position.y - transform.position.y;

                    if(pos >= comparison) feet[i].StepTriggered = false;

                    //trigger a step sound the first time foot y pos is below threshold
                    else if(!feet[i].StepTriggered)
                    {
                        if (groundChecker && groundChecker.gh.ClosestGround != null)
                        EventManager.TriggerEvent<FootstepEvent, Vector3, float, GameObject>(feet[i].Foot.transform.position, footstepWeight, groundChecker.gh.ClosestGround);
                        feet[i].StepTriggered = true;
                        //Don't need to check the rest of the feet if one of them plays a sound
                        break;
                    }   

                } 
                
            }
            
        }
        
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