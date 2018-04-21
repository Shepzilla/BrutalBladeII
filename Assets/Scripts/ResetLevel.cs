using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour {

    public AudioSource riser;
    
	// Use this for initialization
	void Start () {
        StartCoroutine(GameReset());
	}

    //Resets the game.
    IEnumerator GameReset()
    {
        //After a small delay, trigger slow-motion.
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0.1f;

        yield return new WaitForSeconds(0.11f);
        riser.Play();

        //After a delay, reset to normal time.
        yield return new WaitForSeconds(0.09f);
        Time.timeScale = 1.0f;

        //After a short delay, reset the level.
        yield return new WaitForSeconds(2);
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Single);
    }
}
