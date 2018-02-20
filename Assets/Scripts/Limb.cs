using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour {

    Collider collider;

	// Use this for initialization
	void Start () {
        collider = GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider otherCol)
    {
        if (otherCol.tag == "Weapon")
        {
            print("oof");
        }
    }
}
