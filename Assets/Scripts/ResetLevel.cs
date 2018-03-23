using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(GameReset());
	}

    //Resets the game after 3 seconds.
    IEnumerator GameReset()
    {
        yield return new WaitForSeconds(3);
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Single);
    }
}
