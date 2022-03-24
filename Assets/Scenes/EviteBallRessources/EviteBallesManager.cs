using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EviteBallesManager : MonoBehaviour
{
    [Header("Countdown phase")]
    [SerializeField] TMPro.TMP_Text countdownText;
    [SerializeField] int countdownDuration = 3;

    [Header("Ball Emmitting phase")]
    [SerializeField] int nbBallsToEmit = 10;
    [SerializeField] float rateOfEmission = 0.5f;


    BallEmitter[] ballEmitters;

    void Start()
    {
        ballEmitters = FindObjectsOfType<BallEmitter>();
        Debug.Assert(ballEmitters.Length > 0, "No BallEmitter found");
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        // Countdown
        for(int countdown = countdownDuration; countdown > 0; countdown--)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
        }

        // GO
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1);

        // Emit
        for(int ball = 0; ball < nbBallsToEmit; ball++)
        {
            int randomEmitterIndex = Random.Range(0, ballEmitters.Length);
            ballEmitters[randomEmitterIndex].EmitBall();
            yield return new WaitForSeconds(1 / rateOfEmission);
        }

    }
}
