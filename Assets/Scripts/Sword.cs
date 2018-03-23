using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    BoxCollider boxCollider;
    PlayerController playerController;

	// Use this for initialization
	void Start () {
        boxCollider = GetComponent<BoxCollider>();
        playerController = GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject);
        playerController.CollisionReact();
    }
}
