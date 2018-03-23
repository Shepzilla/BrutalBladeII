using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class FollowCamera : MonoBehaviour {

    //PUBLIC VARIABLES
    public PlayerController followTarget;       //The player character the camera will follow.
    public float smoothTime = 0.3F;             //The speed of interpolation.
    public Vector3 offsetTransform;             //Offset from player's position.

    //PRIVATE VARIABLES
    private Vector3 velocity = Vector3.zero;    //Velocity?
    private Transform enemyTarget;              //The player's opponent's position.
    private Transform weapon;                   //The player's weapon.
    private PostProcessingProfile ppProfile;    //The post processing profile to be tweaked.


    //Find everything.
    void Start()
    {
        enemyTarget = followTarget.opponent;
        weapon = followTarget.GetComponentInChildren<Rigidbody>().transform;
        ppProfile = GetComponent<PostProcessingBehaviour>().profile;

        //Test findings.
        if (weapon != null)
        {
            print("gateem");
        }
    }

    //FixedUpdate here because these things shouldn't be tied to framerate.
    void FixedUpdate()
    {
        MoveCamera();
        AdaptiveDepthOfField();
    }

    //Interpolates the camera relative to the offset from the player's current position while rotating to face the enemy.
    void MoveCamera()
    {
        //Calculate desired position.
        Vector3 targetPosition = (followTarget.transform.TransformPoint(offsetTransform) + weapon.transform.TransformPoint(offsetTransform))/ 2;
        transform.LookAt(new Vector3(enemyTarget.position.x, enemyTarget.position.y + 1.0F, enemyTarget.position.z));

        //Apply results
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    //Adjusts depth of field to keep the enemy in focus.
    void AdaptiveDepthOfField()
    {
        //Gets the current depth of field settings and then gets the distance between the camera and the player's enemy.
        DepthOfFieldModel.Settings dofFocus = ppProfile.depthOfField.settings;
        dofFocus.focusDistance = ((transform.position - enemyTarget.position).magnitude);

        //Sets focus distance to distance calculated.
        ppProfile.depthOfField.settings = dofFocus;
    }
}
