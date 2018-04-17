using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    BoxCollider boxCollider;
    PlayerController playerController;
    Rigidbody rigidBody;
    Transform startPos;
    AudioSource clashSound;

	// Use this for initialization
	void Start () {
        boxCollider = GetComponent<BoxCollider>();
        playerController = GetComponentInParent<PlayerController>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.angularVelocity = new Vector3(0, 0, 0);
        clashSound = GetComponent<AudioSource>();
        //print(clashSound.clip.ToString());
        startPos = transform;
	}
	
	// Update is called once per frame
	void Update () {
	}

    //Detects overlap with other sword and passes through the other sword's velocity
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            clashSound.pitch = Random.Range(0.8f, 1.2f);
            clashSound.Play();
            playerController.CollisionReact(other.GetComponentInParent<PlayerController>().armParent.GetComponent<Rigidbody>().angularVelocity);
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        print(rigidBody.angularVelocity);
        playerController.CollisionReact();
        rigidBody.isKinematic = false;
    }
    
    private void OnCollisionExit(Collision collision)
    {
        rigidBody.isKinematic = true;
    }
    */
}
