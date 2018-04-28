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
        weapon = followTarget.GetComponentInChildren<Sword>().transform;
        ppProfile = GetComponent<PostProcessingBehaviour>().profile;
    }

    //FixedUpdate here because these things shouldn't be tied to framerate.
    void FixedUpdate()
    {
        MoveCamera();
        AdaptiveDepthOfField();
        HurtFX();
    }

    //Interpolates the camera relative to the offset from the player's current position while rotating to face the enemy.
    void MoveCamera()
    {
        //Calculate desired position.
        Vector3 targetPosition = followTarget.transform.TransformPoint(offsetTransform);// + weapon.transform.TransformPoint(offsetTransform)) / 2;
        transform.LookAt(new Vector3(enemyTarget.position.x, enemyTarget.position.y + 1.0F, enemyTarget.position.z));

        //Apply results
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    //Adjusts depth of field to keep the enemy in focus.
    void AdaptiveDepthOfField()
    {
        //Gets the current depth of field settings and then gets the distance between the camera and the player's enemy.
        DepthOfFieldModel.Settings ppdDof = ppProfile.depthOfField.settings;
        ppdDof.focusDistance = ((transform.position - enemyTarget.position).magnitude);

        //Sets focus distance to distance calculated.
        ppProfile.depthOfField.settings = ppdDof;
    }

    //Post processing tweaks to emphasise player damage.
    void HurtFX()
    {
        //Gets the current chromatic aberration settings.
        ChromaticAberrationModel.Settings ppChromatic = ppProfile.chromaticAberration.settings;

        //Calculates a new intensity depending on player damage.
        ppChromatic.intensity = Mathf.Clamp((followTarget.GetComponent<PlayerHealth>().startingHealth - followTarget.GetComponent<PlayerHealth>().getHealth()) / 50, 0, 2);

        //Applies new intensity.
        ppProfile.chromaticAberration.settings = ppChromatic;
    }
}
