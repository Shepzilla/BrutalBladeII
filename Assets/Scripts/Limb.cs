using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attach these to bones to enable collision detection.
/// Will spurt blood when hit.
/// </summary>
public class Limb : MonoBehaviour {

    //Public Variables
    public bool isCritical;

    //Private Variables
    private Collider collider;
    private ParticleSystem blood;
    private PlayerHealth playerHealth;

	// Use this for initialization
	void Start () {
        collider = GetComponent<CapsuleCollider>();
        blood = GetComponent<ParticleSystem>();
        playerHealth = GetComponentInParent<PlayerHealth>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Collision
    void OnTriggerEnter (Collider otherCol)
    {
        //Makes sure it's a weapon
        if (otherCol.tag == "Weapon")
        {
            blood.Play();
            playerHealth.TakeDamage(10);
        }
    }
}
