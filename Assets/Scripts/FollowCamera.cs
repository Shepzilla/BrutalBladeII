using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public PlayerController target;
    public float smoothTime = 0.3F;
    public Vector3 offsetTransform;
    private Vector3 velocity = Vector3.zero;
    Transform enemyTarget;

    void Start()
    {
        enemyTarget = target.opponent;
    }

    void Update()
    {
        Vector3 targetPosition = target.transform.TransformPoint(offsetTransform);
        transform.LookAt(new Vector3 (enemyTarget.position.x, enemyTarget.position.y + 0.5F, enemyTarget.position.z));
        //transform.rotation.SetLookRotation(enemyTarget.position);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
