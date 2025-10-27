using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(CharacterInputController))]
public class BasicControlScript : MonoBehaviour
{
    [SerializeField] private Animator anim;  
    private Rigidbody rbody;
    private CharacterInputController cinput;
    private ConstantForce cforce;

    private float jumpStart = 0;
    private bool addForce = false;
    private bool holdingJump = false;
    private Vector3 forceVector = new(0, 0, 0);
    private bool isTurning = false;
    private bool isMoving = false;
    private bool isRunning = false;
    private float inputForward;
    private float inputTurn;
    
    
    [Header("Movement Settings")]
    public float forwardMaxSpeed = 5f;
    public float turnMaxSpeed = 120f;

    // NEW: Public variable to control jump height from the Inspector.
    [Header("Jump Settings")]
    public float jumpForce = 1f;

    [Tooltip("Time (s) space held down for max jump height")]
    public float maxJumpTime = 1f;

    [Tooltip("Default character gravity")]
    public float gravity = Physics.gravity.y;

    [Tooltip("Factor to reduce gravity by while holding space")]
    public float jumpGravity = 0;

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

    private bool isGrounded = true;


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

        cforce = GetComponent<ConstantForce>();

        if (cforce == null)
            Debug.Log("Constant Force could not be found");

        // --- FIX ---
        // Freeze rotation on all axes to prevent the character from tipping over.
        // This prevents any OTHER objects from causing the rigidbody to rotate.
        // Script components can still rotate it
        rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }


    void Start()
    {
        /*
            example of how to get access to certain limbs
            leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
            rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

            if (leftFoot == null || rightFoot == null)
                Debug.Log("One of the feet could not be found");
        */

        anim.applyRootMotion = false;

        //set custom gravity
        cforce.force = new(0f, gravity - Physics.gravity.y, 0f);
    }

    void FixedUpdate()
    {
        // Add force in fixed update for consistent jump height
        if (addForce)
        {
            rbody.AddForce(forceVector, ForceMode.Impulse);
            addForce = false;
        }

        // --- Run toggle (hold Shift to run) ---
        float runMultiplier = isRunning ? 1.8f : 1.0f;
        
        // --- Movement ---
        if (isMoving)
        {
            float moveSpeed = forwardMaxSpeed * runMultiplier;
            rbody.MovePosition(rbody.position + inputForward * moveSpeed * Time.fixedDeltaTime * transform.forward);
            //Debug.Log(Time.time + " new position: " + (rbody.position + inputForward * moveSpeed * Time.fixedDeltaTime * transform.forward));
        }

        // --- Rotation ---
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
        // reset the force to add if we've already added it in FixedUpdate
        if (!addForce)
            forceVector = new(0f, 0f, 0f);

        inputForward = 0f;
        inputTurn = 0f;

        if (cinput.enabled)
        {
            inputForward = cinput.Forward; // -1..1
            inputTurn = cinput.Turn;    // -1..1
        }

        // Dead-zone filtering to prevent noise
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

        // Reverse turning when moving backward
        if (inputForward < 0f) inputTurn = -inputTurn;

        // Ground check (your existing helper)
        isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(
            this.transform.position,
            jumpableGroundNormalMaxAngle,
            1f,
            1f,
            out closeToJumpableGround
        );

        // Jump (your cat anim may not have a jump state; this is harmless)

        // Start jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpStart = Time.time;
            forceVector += Vector3.up * jumpForce;
            holdingJump = true;
            addForce = true;

            // Reduce the gravity

            // Calculate new force from jumpGravity using an exponential curve: gravity / (jumpGravity + 1) 
            //      taking the offset Physics.gravity.y applied to the GameObject into account
            // Gets an exponential curve that hits gravity - Physics.gravity.y at jumpGravity = 0 and Physics.gravity.y at jumpGravity = inf

            // Could add a check here to set  occurances of Physics.gravity.y to 0 if gravity isn't being applied to the GameObject
            float newGrav = gravity*(gravity + 2 * Physics.gravity.y) / (jumpGravity * gravity + 2 * Physics.gravity.y * jumpGravity + gravity) - Physics.gravity.y;
            if (jumpGravity > 0)
                cforce.force = new(0f, newGrav, 0f);

            //if (anim) anim.SetTrigger("doJump");
        }

        // Set gravity back to normal when space is released, char hits the ground, or its jumped for the maximum time
        if ((!Input.GetKey(KeyCode.Space) || (isGrounded && Time.time - jumpStart > .05f) || Time.time - jumpStart >= maxJumpTime) && holdingJump)
        {
            holdingJump = false;
            cforce.force = new (0f, gravity - Physics.gravity.y, 0f);
        }

        // --- Run toggle (hold Shift to run) ---
        isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        // --- Animator + turn/run logic (with airborne handling) ---
        if (anim)
        {
            bool isTurningOnly = isTurning && !isMoving;

            // Rigidbody vertical speed (optional, for future ascent/descent logic)
            float vy = rbody.linearVelocity.y;

            // If airborne, kill locomotion blend so legs don't stride mid-air
            if (!isGrounded)
            {
                SetAnimFloat(anim, "Vert", 0f, 0.08f, Time.deltaTime);     // idle side of tree
                SetAnimFloat(anim, "State", 0f, 0.08f, Time.deltaTime);    // walk, not run
                anim.SetBool("isFalling", true);
                // If your controller uses Grounded instead:
                // anim.SetBool("Grounded", false);

                // Optional: slightly slow/steady the pose in air
                anim.speed = 1f;
            }
            else
            {
                float targetVert = isTurningOnly ? 1f : (isMoving ? 1f : 0f); // turn-in-place looks like walking
                float targetState = isTurningOnly ? 0f : ((isRunning && isGrounded) ? 1f : 0f); // Run only when moving forward on ground

                // Change floats smoothly if the target is > 0 or it hasn't reached .001 yet
                SetAnimFloat(anim, "Vert", targetVert, 0.06f, Time.deltaTime);
                SetAnimFloat(anim, "State", targetState, 0.06f, Time.deltaTime);
                //anim.SetBool("isFalling", false);
                // or: anim.SetBool("Grounded", true);
                anim.speed = 1f;
            }
        }
    }

    // Helper to send floats to the anim so they can reach 0
    void SetAnimFloat(Animator animator, string name, float value, float dampTime, float deltaTime)
    {
        if (animator.GetFloat(name) > .001 || value > 0)
            animator.SetFloat(name, value, dampTime, deltaTime);
        else if (value == 0)
            animator.SetFloat(name, 0);
    }


    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag == "ground")
            ++groundContactCount;

        if (collision.transform.gameObject.tag == "enemy")
            EventManager.TriggerEvent<EnemyCollisionEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);                            
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.tag == "ground")
            --groundContactCount;

        if (collision.transform.gameObject.tag == "enemy")
            EventManager.TriggerEvent<HissEvent, Vector3>(this.gameObject.transform.position);
    
    }
    
}