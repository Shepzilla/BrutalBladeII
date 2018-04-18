using UnityEngine;
using System.Collections;

/// <summary>
/// Brutal Blade II - Player Controller v0.21
/// 
/// About: Utilises animation controllers, physics forces and inverse kinematics to move a character
///        and their arms in a combination of baked and physics-driven animation.
///         
/// Author: Robert J Harper
/// Last Update: 18/4/2018
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Public Variables.
    public int playerNumber = 1;                //Stores player number for correct input.
    public float movementDamping = 0.1f;        //Smooths stick input.
    public float swingMultiplier = 5.0f;        //How fast can the player swing their sword?
    public float legacySwordDamping = 0.2f;     //(LEGACY)Smooths sword movement.
    public float legacySwingExtreme = 0.4f;     //(LEGACY)Angle limit for sword movement.
    public float swingExtreme = 200.0f;         //Angle limit for sword movement.
    public float modifierExtreme = 0.8f;        //Angle limit for grip modifier.
    public float recoilMultiplier = 10.0f;      //The force multiplier exerted on sword collisions.
    public int criticalMultiplier = 2;          //How much extra damage is taken when a critical limb is hit.
    public GameObject armParent;                //Quick and easy way to move both arms and keep them in sync.
    public GameObject sword;                    //Reference to the sword the player is holding.
    public Transform opponent;                  //Reference to the position of the current target.
    public Transform rightIkTarget = null;      //Where the right hand goes.
    public Transform leftIkTarget = null;       //Where the left hand goes.
    public ParticleSystem leftFootParticles;    //Particles for the left foot.
    public ParticleSystem rightFootParticles;   //Particles for the right foot.
    public ParticleSystem clashParticles;       //Particles to play when swords collide.
    public AudioSource footStepSourceL;         //The sound to play when a footstep is executed. (Left)
    public AudioSource footStepSourceR;         //The sound to play when a footstep is executed. (Right)
    public bool legacyMovement = false;         //Toggles between using linear interpolation or smoothDamp.
    public bool ikActive = true;                //Whether IK on the arms is enabled or not (for testing).

    //Private Variables.
    private float horizontalSword = 0;          //The value of armParent rotation across the horizontal axis.
    private float verticalSword = 0;            //The value of armParent rotation across the vertical axis.
    private float swordModifier = 0;            //The value of armParent rotation spinning around its position.
    private float xVelocity = 0.0f;             //No idea
    private float yVelocity = 0.0f;             //No idea
    private PlayerHealth playerHealth;          //The number of hitPoints the player has remaining.
    private Quaternion targetRotation;          //(LEGACY) The rotation to move the armParent towards.
    private Quaternion originRotation;          //The rotation to reset the armParent to.

    //Misc Variables.
    Animator animator;
    Rigidbody rigidBody;
    Vector3 opponentCoord;
    Quaternion chestTarget;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        //Gets the default rotation for the armParent.
        originRotation = armParent.transform.localRotation;

        //Locks and hides the mouse cursor within the viewport.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerHealth = GetComponent<PlayerHealth>();
    }
    
    //FixedUpdate is called once per tick so input responsiveness is not affected by frame-rate.
    void FixedUpdate()
    {
        MoveCharacter();
        EscapeGame();
        //Toggles between different sword control methods. 
        if(legacyMovement)
        {
            LegacyMoveSword();
        }
        else
        {
            PhysicsMoveSword();
            PhysicsCenterSword();
        }
    }

    /// <summary>
    /// Handles non-sword related character movement.
    /// Takes input from the gamepad's left stick and applies it to the animation controller.
    /// It also rotates the player's rigidBody to keep them facing their target.
    /// </summary>
    void MoveCharacter()
    {
        //Reads input device for input
        //Movement
        float horizontal = Input.GetAxis("Horizontal" + playerNumber);
        float vertical = Input.GetAxis("Vertical" + playerNumber);

        //Applies input values to animator controller variables.
        animator.SetFloat("ForwardBackward", vertical, movementDamping, Time.deltaTime);
        animator.SetFloat("LeftRight", horizontal, movementDamping, Time.deltaTime);

        //Gets the opponent's position and sets the y to zero to prevent undersired vertical rotations.
        opponentCoord = new Vector3(opponent.transform.position.x, 0, opponent.transform.position.z);
        rigidBody.transform.LookAt(opponentCoord);
    }

    /// <summary>
    /// Moves the sword by applying forces to the TargetParent's rigidbody.
    /// </summary>
    void PhysicsMoveSword()
    {
        //Reads the right stick.
        horizontalSword = Mathf.Clamp(Input.GetAxis("HorizontalSword" + playerNumber), -legacySwingExtreme, legacySwingExtreme);
        verticalSword = Mathf.Clamp(Input.GetAxis("VerticalSword" + playerNumber), -legacySwingExtreme, legacySwingExtreme);

        //Reads the triggers (both triggers affect the same floating point value, with left being -1 and right being 1).
        swordModifier = Mathf.Clamp(Input.GetAxis("GripModifier" + playerNumber), -modifierExtreme, modifierExtreme);

        //Applies input values to armParent to move the sword around the screen.
        armParent.GetComponent<Rigidbody>().AddRelativeTorque(verticalSword * swingMultiplier, horizontalSword * swingMultiplier, swordModifier * swingMultiplier, ForceMode.Acceleration);
    }

    /// <summary>
    /// Applies forces which rotate the armParent towards its default rotation.
    /// These forces get multiplied as the armParent moves further away from its initial rotation.
    /// </summary>
    void PhysicsCenterSword()
    {
        //Gets the current rotation of the armParent.
        Quaternion currentRotation = armParent.transform.localRotation;

        //Finds the direction to move the armParent back towards its origin and multiplies the result by the size of the angle between the two quaternions.
        Vector3 resetForce = new Vector3(currentRotation.x - originRotation.x, currentRotation.y - originRotation.y, currentRotation.z - originRotation.z) * Mathf.Clamp(Quaternion.Angle(currentRotation, targetRotation), -swingExtreme, swingExtreme);

        //Applies the calculated torque to the armParent.
        armParent.GetComponent<Rigidbody>().AddRelativeTorque(resetForce, ForceMode.Acceleration);
    }

    /// <summary>
    /// This is the legacy version of sword control.
    /// It calculates a target rotation based on the gamepad's right stick and moves the sword
    /// towards the targetRotation via linear interpolation.
    /// </summary>
    void LegacyMoveSword()
    {
        //Sword movement (with interpolation)
        horizontalSword = Mathf.Lerp(horizontalSword, Input.GetAxis("HorizontalSword" + playerNumber), legacySwordDamping * Time.deltaTime);
        verticalSword = Mathf.Lerp(verticalSword, Input.GetAxis("VerticalSword" + playerNumber), legacySwordDamping * Time.deltaTime);
        swordModifier = Mathf.Lerp(swordModifier, Input.GetAxis("GripModifier" + playerNumber), legacySwordDamping * Time.deltaTime);

        //Clamps values of sword rotations to the specified extreme.
        horizontalSword = Mathf.Clamp(horizontalSword, -legacySwingExtreme, legacySwingExtreme);
        verticalSword = Mathf.Clamp(verticalSword, -legacySwingExtreme, legacySwingExtreme);
        swordModifier = Mathf.Clamp(swordModifier, -modifierExtreme, modifierExtreme);

        //Applies input values to armParent to move the sword around the screen.
        targetRotation = new Quaternion(verticalSword, horizontalSword, swordModifier, 1);//transform.rotation.w);
        armParent.transform.localRotation = targetRotation;
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

    //Allows the editor game to be stopped with the escape key.
    void EscapeGame()
    {
        if (Input.GetKey("escape"))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    //Makes the armParent to react to sword collisions.
    public void CollisionReact(Vector3 opposingForce)
    {
        //Gets the current angular velocity of the armParent.
        Vector3 currentForce = armParent.GetComponent<Rigidbody>().angularVelocity;

        //Applies a multiplied force in the opposing direction, giving the illusion of collision detection.
        armParent.GetComponent<Rigidbody>().AddRelativeTorque(((currentForce * -1) - opposingForce) * recoilMultiplier, ForceMode.VelocityChange);

        //Play particle system.
        clashParticles.Play();
    }
    
    //Detracts health from the playerHealth script and plays hit animation.
    public void Hurt(float damage, bool critical)
    {
        
        //If the limb is a critical limb, it will deal more damage.
        if(critical)
        {
            playerHealth.TakeDamage(damage * 2);

            if (animator)
            animator.SetTrigger("Hit");
        }
        //Otherwise, don't multiply the damage.
        else
        {
            playerHealth.TakeDamage(damage);
        }
        
    }

    //This gets fired from events in the character animations whenever a footstep occurs.
    public void Footstep(string foot)
    {
        //With the string passed from the animation event, decide which foot to trigger.
        if (foot == "left")
        {
            footStepSourceL.pitch = Random.Range(0.8f, 1.2f);
            footStepSourceL.Play();
            leftFootParticles.Play();
        }
        else
        {
            footStepSourceR.pitch = Random.Range(0.8f, 1.2f);
            footStepSourceR.Play();
            rightFootParticles.Play();
        }
    }
}


