using UnityEngine;
using System.Collections;

/// <summary>
/// Brutal Blade II - Player Controller v0.03
/// 
/// About: Utilises animation controllers and IK to move around a character
///         and their arms in a combination of baked and procedural animation.
///         
/// Author: Robert J Harper
/// Last Update: 13/2/2018
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Public Variables.
    public int playerNumber = 1;                //Stores player number for correct input.
    public float movementDamping = 0.1f;        //Smooths stick input.
    public float swingSpeed = 5.0f;             //How fast can the player swing their sword?
    public float legacySwordDamping = 0.2f;     //Smooths sword movement. (Legacy)
    public float swingExtreme = 0.4f;           //Angle limit for sword movement.
    public float modifierExtreme = 0.8f;        //Angle limit for grip modifier.
    public GameObject armParent;                 //Quick and easy way to move both arms and keep them in sync.
    public Transform opponent;                  //Reference to the position of the current target.
    public bool ikActive = true;                //Whether IK on the arms is enabled or not (for testing).
    public Transform rightIkTarget = null;      //Where the right hand goes.
    public Transform leftIkTarget = null;       //Where the left hand goes.
    public int criticalMultiplier = 2;          //How much extra damage is taken when a critical limb is hit.
    public bool legacyMovement = false;         //Toggles between using linear interpolation or smoothDamp.

    //Private Variables.
    private float horizontalSword = 0;
    private float verticalSword = 0;
    private float swordModifier = 0;
    private PlayerHealth playerHealth;
    private Quaternion targetRotation;
    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;

    Animator animator;
    Rigidbody rigidBody;
    Vector3 opponentCoord;
    Quaternion chestTarget;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        //Locks and hides the mouse cursor within the viewport.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(legacyMovement)
        {
            LegacyMoveCharacterAndSword();
        }
        else
        {
            PhysicsMoveCharacterAndSword();
        }
    }

    /// <summary>
    /// This moves the player's sword around according to input from both triggers and the right analogue stick.
    /// </summary>
    void MoveCharacterAndSword()
    {
        //Reads input device for input
        //Movement
        float horizontal = Input.GetAxis("Horizontal" + playerNumber);
        float vertical = Input.GetAxis("Vertical" + playerNumber);

        //Applies input values to animator controller variables.
        animator.SetFloat("ForwardBackward", vertical, movementDamping, Time.deltaTime);
        animator.SetFloat("LeftRight", horizontal, movementDamping, Time.deltaTime);

        //Sword movement (with interpolation)
        horizontalSword = Mathf.SmoothDamp(horizontalSword, Input.GetAxis("HorizontalSword" + playerNumber), ref xVelocity, legacySwordDamping * Time.deltaTime, swingSpeed, Time.deltaTime);
        verticalSword = Mathf.SmoothDamp(verticalSword, Input.GetAxis("VerticalSword" + playerNumber), ref yVelocity, legacySwordDamping * Time.deltaTime, swingSpeed, Time.deltaTime);
        swordModifier = Mathf.Lerp(swordModifier, Input.GetAxis("GripModifier" + playerNumber), legacySwordDamping * Time.deltaTime);

        //Clamps values of sword rotations to the specified extreme.
        horizontalSword = Mathf.Clamp(horizontalSword, -swingExtreme, swingExtreme);
        verticalSword = Mathf.Clamp(verticalSword, -swingExtreme, swingExtreme);
        swordModifier = Mathf.Clamp(swordModifier, -modifierExtreme, modifierExtreme);

        //Allows the editor game to be stopped (since the mouse in bound to the game screen.)
        if (Input.GetKey("escape"))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        //Applies input values to armParent to move the sword around the screen.
        targetRotation = new Quaternion(verticalSword, horizontalSword, swordModifier, 1);//transform.rotation.w);
        armParent.transform.localRotation = targetRotation;

        //Gets the opponent's position and sets the y to zero to prevent undersired vertical rotations.
        opponentCoord = new Vector3(opponent.transform.position.x, 0, opponent.transform.position.z);
        rigidBody.transform.LookAt(opponentCoord);
    }


    /// <summary>
    /// This is the legacy version of sword control which utilises linear interpolation.
    /// </summary>
    void LegacyMoveCharacterAndSword()
    {
        //Reads input device for input
        //Movement
        float horizontal = Input.GetAxis("Horizontal" + playerNumber);
        float vertical = Input.GetAxis("Vertical" + playerNumber);

        //Applies input values to animator controller variables.
        animator.SetFloat("ForwardBackward", vertical, movementDamping, Time.deltaTime);
        animator.SetFloat("LeftRight", horizontal, movementDamping, Time.deltaTime);

        //Sword movement (with interpolation)
        horizontalSword = Mathf.Lerp(horizontalSword, Input.GetAxis("HorizontalSword" + playerNumber), legacySwordDamping * Time.deltaTime);
        verticalSword = Mathf.Lerp(verticalSword, Input.GetAxis("VerticalSword" + playerNumber), legacySwordDamping * Time.deltaTime);
        swordModifier = Mathf.Lerp(swordModifier, Input.GetAxis("GripModifier" + playerNumber), legacySwordDamping * Time.deltaTime);

        //Clamps values of sword rotations to the specified extreme.
        horizontalSword = Mathf.Clamp(horizontalSword, -swingExtreme, swingExtreme);
        verticalSword = Mathf.Clamp(verticalSword, -swingExtreme, swingExtreme);
        swordModifier = Mathf.Clamp(swordModifier, -modifierExtreme, modifierExtreme);

        //Allows the editor game to be stopped (since the mouse in bound to the game screen.)
        if (Input.GetKey("escape"))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        //Applies input values to armParent to move the sword around the screen.
        targetRotation = new Quaternion(verticalSword, horizontalSword, swordModifier, 1);//transform.rotation.w);
        armParent.transform.localRotation = targetRotation;

        //Gets the opponent's position and sets the y to zero to prevent undersired vertical rotations.
        opponentCoord = new Vector3(opponent.transform.position.x, 0, opponent.transform.position.z);
        rigidBody.transform.LookAt(opponentCoord);
    }

    void PhysicsMoveCharacterAndSword()
    {
         //Reads input device for input
        //Movement
        float horizontal = Input.GetAxis("Horizontal" + playerNumber);
        float vertical = Input.GetAxis("Vertical" + playerNumber);

        //Applies input values to animator controller variables.
        animator.SetFloat("ForwardBackward", vertical, movementDamping, Time.deltaTime);
        animator.SetFloat("LeftRight", horizontal, movementDamping, Time.deltaTime);

        //Sword movement (with interpolation)
        horizontalSword = Mathf.Lerp(horizontalSword, Input.GetAxis("HorizontalSword" + playerNumber), legacySwordDamping * Time.deltaTime);
        verticalSword = Mathf.Lerp(verticalSword, Input.GetAxis("VerticalSword" + playerNumber), legacySwordDamping * Time.deltaTime);
        swordModifier = Mathf.Lerp(swordModifier, Input.GetAxis("GripModifier" + playerNumber), legacySwordDamping * Time.deltaTime);

        //Clamps values of sword rotations to the specified extreme.
        horizontalSword = Mathf.Clamp(horizontalSword, -swingExtreme, swingExtreme);
        verticalSword = Mathf.Clamp(verticalSword, -swingExtreme, swingExtreme);
        swordModifier = Mathf.Clamp(swordModifier, -modifierExtreme, modifierExtreme);

        //Allows the editor game to be stopped (since the mouse in bound to the game screen.)
        if (Input.GetKey("escape"))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        //Applies input values to armParent to move the sword around the screen.
        targetRotation = new Quaternion(verticalSword, horizontalSword, swordModifier, 1);//transform.rotation.w);
        armParent.GetComponent<Rigidbody>().AddRelativeTorque(verticalSword, horizontalSword, swordModifier, ForceMode.Acceleration);

        //Gets the opponent's position and sets the y to zero to prevent undersired vertical rotations.
        opponentCoord = new Vector3(opponent.transform.position.x, 0, opponent.transform.position.z);
        rigidBody.transform.LookAt(opponentCoord);
    }

    //Reference to Animator Controller IK callback
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                chestTarget.SetEulerRotation(armParent.transform.localRotation.x, armParent.transform.localRotation.y, Mathf.Clamp(armParent.transform.localRotation.z, -0.3f, 0.3f));
                animator.SetBoneLocalRotation(HumanBodyBones.Chest, chestTarget);

                //animator.SetBoneLocalRotation(HumanBodyBones.Chest, new Quaternion(armParent.localRotation.x, armParent.localRotation.y, Mathf.Clamp(armParent.localRotation.z, -0.1f, 0.1f));//Quaternion.Slerp(transform.rotation, armParent.localRotation, Time.deltaTime));
                
                //Set the right hand target position and rotation, if it exists.
                if (rightIkTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightIkTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightIkTarget.rotation);
                }
                //Set the left hand target position and rotation, if it exists.
                if (leftIkTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftIkTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftIkTarget.rotation);
                }
            }

            //if the IK is not active, set the position and rotation of the hands back to the default positions.
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }

    /*void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            animator.SetTrigger("Die");
        }
    }*/

    //Makes the player react to sword collisions.
    public void CollisionReact()
    {
        //print("YEAH!");
        targetRotation = new Quaternion(-horizontalSword, -verticalSword, -swordModifier, 1);
    }
    
    //Detracts health from the playerHealth script and plays hit animation.
    public void Hurt(int damage, bool critical)
    {
        //If the limb is a critical limb, it will deal more damage.
        if(critical)
        {
            playerHealth.TakeDamage(damage * 2);
            animator.SetTrigger("Hit");
        }
        else
        {
            playerHealth.TakeDamage(damage);
        }
    }
}


