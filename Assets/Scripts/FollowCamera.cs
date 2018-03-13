using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class FollowCamera : MonoBehaviour {

    public PlayerController target;
    public float smoothTime = 0.3F;
    public Vector3 offsetTransform;
    private Vector3 velocity = Vector3.zero;
    Transform enemyTarget;

    PostProcessingProfile ppProfile;

    

    void Start()
    {
        enemyTarget = target.opponent;
        ppProfile = GetComponent<PostProcessingBehaviour>().profile;
        if (ppProfile != null)
        {
            print("gateem");
        }
    }

    void Update()
    {
        Vector3 targetPosition = target.transform.TransformPoint(offsetTransform);
        transform.LookAt(new Vector3 (enemyTarget.position.x, enemyTarget.position.y + 0.5F, enemyTarget.position.z));
        //transform.rotation.SetLookRotation(enemyTarget.position);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        AdaptiveDepthOfField();
    }

    void AdaptiveDepthOfField()
    {
        //Gets the current depth of field settings and then gets the distance between the camera and the player's enemy.
        DepthOfFieldModel.Settings dofFocus = ppProfile.depthOfField.settings;
        dofFocus.focusDistance = ((transform.position - enemyTarget.position).magnitude);

        //Sets focus distance to distance calculated.
        ppProfile.depthOfField.settings = dofFocus;
    }
}
