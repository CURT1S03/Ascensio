using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(CharacterInputController))]
public class BasicControlScript : MonoBehaviour
{
    [SerializeField] private Animator anim;  
    private Rigidbody rbody;
    private CharacterInputController cinput;

    private Transform leftFoot;
    private Transform rightFoot;

    public float forwardMaxSpeed = 5f;
    public float turnMaxSpeed = 120f;
    
    // NEW: Public variable to control jump height from the Inspector.
    [Header("Jump Settings")]
    public float jumpForce = 8f;

    //Useful if you implement jump in the future...
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


    void Awake()
    {

        anim = GetComponentInChildren<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

        cinput = GetComponent<CharacterInputController>();

        if (cinput == null)
            Debug.Log("CharacterInputController could not be found");

        // --- FIX ---
        // Freeze rotation on the X and Z axes to prevent the character from tipping over.
        // It can still rotate on the Y axis for turning.
        rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }


    void Start()
    {
        //example of how to get access to certain limbs
        leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

        if (leftFoot == null || rightFoot == null)
            Debug.Log("One of the feet could not be found");

        anim.applyRootMotion = false;

    }


   void Update()
{
    float inputForward = 0f;
    float inputTurn = 0f;

    if (cinput.enabled)
    {
        inputForward = cinput.Forward; // -1..1
        inputTurn    = cinput.Turn;    // -1..1
    }

    // Dead-zone filtering to prevent noise
    const float dead = 0.05f;
    if (Mathf.Abs(inputForward) < dead) inputForward = 0f;
    if (Mathf.Abs(inputTurn)    < dead) inputTurn    = 0f;

    // Reverse turning when moving backward
    if (inputForward < 0f) inputTurn = -inputTurn;

    // Ground check (your existing helper)
    bool isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(
        this.transform.position,
        jumpableGroundNormalMaxAngle,
        0.1f,
        1f,
        out closeToJumpableGround
    );

    // Jump (your cat anim may not have a jump state; this is harmless)
    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    {
        rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (anim) anim.SetTrigger("doJump");
    }

    // --- Run toggle (hold Shift to run) ---
    bool isRunning = Input.GetKey(KeyCode.LeftShift) && inputForward > 0f;
    float runMultiplier = isRunning ? 1.8f : 1.0f;

    // --- Movement ---
    float moveSpeed = forwardMaxSpeed * runMultiplier;
    rbody.MovePosition(
        rbody.position + this.transform.forward * inputForward * Time.deltaTime * moveSpeed
    );

    // --- Rotation ---
    bool isMoving  = Mathf.Abs(inputForward) > 0.001f;
    bool isTurning = Mathf.Abs(inputTurn)   > 0.001f;

    float turnSpeed = isTurning && !isMoving ? turnMaxSpeed * 0.6f : turnMaxSpeed;

    rbody.angularVelocity = Vector3.zero;
    if (isTurning && turnSpeed > 0f)
    {
        var deltaRot = Quaternion.AngleAxis(inputTurn * Time.deltaTime * turnSpeed, Vector3.up);
        rbody.MoveRotation(rbody.rotation * deltaRot);
    }


    // --- Animator + turn/run logic (with airborne handling) ---
    if (anim)
    {
        isMoving      = Mathf.Abs(inputForward) > 0.001f;
        isTurning     = Mathf.Abs(inputTurn)   > 0.001f;
        bool isTurningOnly = isTurning && !isMoving;

        // Rigidbody vertical speed (optional, for future ascent/descent logic)
        float vy = rbody.linearVelocity.y;

        // Run only when moving forward on ground
        isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving && isGrounded;

        // If airborne, kill locomotion blend so legs don't stride mid-air
        if (!isGrounded)
        {
            anim.SetFloat("Vert", 0f, 0.08f, Time.deltaTime);     // idle side of tree
            anim.SetFloat("State", 0f, 0.08f, Time.deltaTime);    // walk, not run
            anim.SetBool("isFalling", true);
            // If your controller uses Grounded instead:
            // anim.SetBool("Grounded", false);

            // Optional: slightly slow/steady the pose in air
            anim.speed = 1f;
        }
        else
        {
            float targetVert  = isTurningOnly ? 1f : (isMoving ? 1f : 0f); // turn-in-place looks like walking
            float targetState = isTurningOnly ? 0f : (isRunning ? 1f : 0f);

            anim.SetFloat("Vert",  targetVert,  0.06f, Time.deltaTime);
            anim.SetFloat("State", targetState, 0.06f, Time.deltaTime);
            //anim.SetBool("isFalling", false);
            // or: anim.SetBool("Grounded", true);
            anim.speed = 1f;
        }
    }



}


    //This is a physics callback
        void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {
            ++groundContactCount;
                    
            EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }
                                
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.tag == "ground")
        {
            --groundContactCount;

        }
    }

}