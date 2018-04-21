using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    //Public Variables
    public float startingHealth = 100;
    public GameObject ragdoll;
    public Slider healthBar;

    //Private Variables
    private float currentHealth;
    private bool isDead;
    private AudioSource cutSound;

    void Start()
    {
        cutSound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
        isDead = false;
    }

    //To be called by limb colliders upon impact.
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        cutSound.Play();
        //If the health drops below 0, the player is dead.
        healthBar.value = currentHealth;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public float getHealth()
    {
        return currentHealth;
    }

    //Called upon player death.
    private void Die()
    {
        isDead = true;
        cutSound.Play();
        Instantiate(ragdoll, gameObject.transform.position, gameObject.transform.rotation);
        gameObject.SetActive(false);
    }
}
