using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterInputController))]
public class BasicControlScript : MonoBehaviour
{
    private Animator anim;  
    private Rigidbody rbody;
    private CharacterInputController cinput;

    private Transform leftFoot;
    private Transform rightFoot;

    public float forwardMaxSpeed = 5f;
    public float turnMaxSpeed = 45f;
    
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

        anim = GetComponent<Animator>();

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


    void Update() {

        float inputForward=0f;
        float inputTurn=0f;

        if (cinput.enabled)
        {
            inputForward = cinput.Forward; // Gets W/S key input
            inputTurn = cinput.Turn;
        }
        
        //switch turn around if going backwards
        if(inputForward < 0f)
        inputTurn = -inputTurn;

        //onCollisionXXX() doesn't always work for checking if the character is grounded from a playability perspective
        //Uneven terrain can cause the player to become technically airborne, but so close the player thinks they're touching ground.
        //Therefore, an additional raycast approach is used to check for close ground
        bool isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(this.transform.position, jumpableGroundNormalMaxAngle, 0.1f, 1f, out closeToJumpableGround);


        // --- JUMP LOGIC ---
        // Check if the Space Bar was pressed and if the character is grounded.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Apply an instant upward force for the jump.
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            // Tell the animator to play the jump animation.
            // NOTE: You must create a "doJump" Trigger parameter in your Animator Controller.
            anim.SetTrigger("doJump");
        }


        // This moves the character forward/backward based on the direction it is currently facing (transform.forward).
        rbody.MovePosition(rbody.position +  this.transform.forward * inputForward * Time.deltaTime * forwardMaxSpeed);
        
        //This rotates the character left and right.
        rbody.MoveRotation(rbody.rotation * Quaternion.AngleAxis(inputTurn * Time.deltaTime * turnMaxSpeed, Vector3.up));


        // anim.SetFloat("velx", inputTurn); 
        anim.SetFloat("vely", inputForward);
        anim.SetBool("isFalling", !isGrounded);

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