using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attach these to bones to enable collision detection.
/// Will spurt blood when hit.
/// </summary>
public class Limb : MonoBehaviour {

    Collider collider;
    ParticleSystem blood;
    public bool isCritical;

	// Use this for initialization
	void Start () {
        collider = GetComponent<CapsuleCollider>();
        blood = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider otherCol)
    {
        if (otherCol.tag == "Weapon")
        {
            print("oof");
            blood.Play();
        }
    }
}
