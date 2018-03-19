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
    private PlayerController playerController;
    private int baseDamage = 10;

	// Use this for initialization
	void Start () {
        collider = GetComponent<CapsuleCollider>();
        blood = GetComponent<ParticleSystem>();
        playerController = GetComponentInParent<PlayerController>();
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
            playerController.Hurt(baseDamage, isCritical);
        }
    }
}
