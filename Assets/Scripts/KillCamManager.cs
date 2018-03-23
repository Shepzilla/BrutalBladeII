using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This moves the actor to the centerpoint between two or more players.
/// The purpose of this actor is to carry positions for potential kill cameras to cut to.
/// </summary>
public class KillCamManager : MonoBehaviour {

    GameObject[] players;           //Array of players present in the level.
    Vector3 midPoint;               //The calculated point in between all the players.
	
    // Use this for initialization
    // Gets a reference to all the players spawned in the level.
	void Start () {
        if (players == null)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
	}
	
	// Update is called once per frame
	void Update () {
        //Gathers the position of all the players in the level.
		foreach (GameObject player in players)
        {
            midPoint +=  player.transform.position;
        }
        //Divides by the length of the player array to calculate the average position.
        midPoint /= players.Length;
        transform.position = midPoint;                              //Set the gameObject transform to the calculated average.
        transform.rotation = players[0].transform.rotation;         //Set the gameObject rotation to match that of player 1.
	}
}
