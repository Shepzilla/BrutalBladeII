using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    BoxCollider boxCollider;
    PlayerController playerController;
    Rigidbody rigidBody;
    Transform startPos;

    ///
    /// WHAT YOU NEED TO DO
    /// - Get the sword bound to the targetParent right hand.
    /// - Get the hands bound to the sword.
    /// - Interp rotation to targetRotation following collision.
    /// - Adjust torque values to get the swing right.
    /// - Figure out a way to get claming to work (negative forces that are multiplied as the sword moves further away? (Would that cause a slingshot effect?))
    /// Side note. This may be the solution to our problems.
    ///
	// Use this for initialization
	void Start () {
        boxCollider = GetComponent<BoxCollider>();
        playerController = GetComponentInParent<PlayerController>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.angularVelocity = new Vector3(0, 0, 0);
        startPos = transform;
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            playerController.CollisionReact();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject);
        playerController.CollisionReact();
        rigidBody.isKinematic = false;
    }
    
    private void OnCollisionExit(Collision collision)
    {
        rigidBody.isKinematic = true;
    }
}
