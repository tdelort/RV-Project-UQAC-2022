using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EviteBalleMinigame : MonoBehaviour
{   
    private const float minigameDuration = 20f;

    [SerializeField]
    private BallThrower ballThrower;
    void StartMiniGame() {
        ballThrower.StartShooting();
        StartCoroutine(EndMiniGame());
    }

    IEnumerator EndMiniGame()
    {   
        yield return new WaitForSeconds(minigameDuration);
        ballThrower.StopShooting();
    }

    void Start()
    {
        StartMiniGame();
    }
}
