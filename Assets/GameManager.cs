using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int roundLimit = 3;              //The amount of round wins required to win the game.
    public float timeToStart = 3.0f;        //The delay before a round starts.
    public float timeToFinish = 5.0f;       //The delay before the level is reset.

    private WaitForSeconds startDelay;
    private WaitForSeconds endDelay;
    private int currentRound = 0;           //The round the game is currently on.

	// Use this for initialization
	void Start () {
        //Sets up time delays according to values assigned in the inspector.
        startDelay = new WaitForSeconds(timeToStart);
        endDelay = new WaitForSeconds(timeToFinish);

        //Executes main coroutine.
	}
	
    //This handles the calling of the different phases of a round.
	private IEnumerator GameLoop()
    {
        //This will not return until the start of a round has completed.
        yield return StartCoroutine(RoundStart());

        //This will not return until the round is finished.
        //yield return StartCoroutine(RoundPlay());

        //This will not return until the end of a round has completed.
        //yield return StartCoroutine(RoundEnd());

        //Checks to see if an overall winner exists, if so. Reset the game.
        /* if (GameWinner)
         * {
         *      Application.LoadLevel(Application.loadedLevel);
         * }
        */
        //Otherwise, restart the coroutine (begin another round).
        StartCoroutine(GameLoop());
    }

    private IEnumerator RoundStart()
    {
        currentRound++;
        yield return startDelay;
    }
}
