using UnityEngine;
using System.Collections;

/// <summary>
/// Brutal Blade II - Player Controller v0.01
/// 
/// About: Utilises animation controllers and IK to move around a character
///         and their arms in a combination of baked and procedural animation.
///         
/// Author: Robert J Harper
/// Last Update: 8/2/2018
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Public Variables.
    public float MovementDamping = 0.1f;        //Smooths stick input.
    public float swingExtreme = 0.4f;          //Angle limit for sword movement.
    public Transform armParent;                 //Quick and easy way to move both arms and keep them in sync.
    public Transform opponent;                  //Reference to the position of the current target.
    public bool ikActive = true;                //Whether IK on the arms is enabled or not (for testing).
    public Transform rightIkTarget = null;      //Where the right hand goes.
    public Transform leftIkTarget = null;       //Where the left hand goes.

    Animator animator;
    Rigidbody rigidBody;
    Vector3 opponentCoord;
    
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Reads input device for input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float horizontalSword = Input.GetAxis("HorizontalSword");
        float verticalSword = Input.GetAxis("VerticalSword");
        //bool fire = Input.GetButtonDown("Fire1");

        //Applies input values to animator controller variables.
        animator.SetFloat("ForwardBackward", vertical, MovementDamping, Time.deltaTime);
        animator.SetFloat("LeftRight", horizontal, MovementDamping, Time.deltaTime);
        //animator.SetBool("Fire", fire);

        //Applies input values to armParent to move the sword around the screen.
        armParent.localRotation = new Quaternion(verticalSword, horizontalSword, rigidBody.transform.rotation.z, rigidBody.transform.rotation.w);

        //Gets the opponent's position and sets the y to zero to prevent undersired vertical rotations.
        opponentCoord = new Vector3(opponent.transform.position.x, 0, opponent.transform.position.z);
        rigidBody.transform.LookAt(opponentCoord);
    }

    //Reference to Animator COntroller IK callback
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the right hand target position and rotation, if one has been assigned
                if (rightIkTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightIkTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightIkTarget.rotation);
                }

                if (rightIkTarget != null)
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

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            animator.SetTrigger("Die");
        }
    }
}


