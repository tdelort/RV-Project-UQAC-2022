using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EviteBalleMinigame : MonoBehaviour
{   
    [SerializeField]
    private float minigameDuration = 20f;

    [SerializeField]
    private BallThrower[] ballThrower;

    [SerializeField]
    private TMPro.TMP_Text timerDisplay;

    [SerializeField]
    private Counter counter;


    IEnumerator Start()
    {
        // small 3 2 1
        timerDisplay.text = "3";
        yield return new WaitForSeconds(1f);
        timerDisplay.text = "2";
        yield return new WaitForSeconds(1f);
        timerDisplay.text = "1";
        yield return new WaitForSeconds(1f);


        for(int i = 0; i < ballThrower.Length; i++)
            ballThrower[i].StartShooting();

        for(float time = 0; time < minigameDuration; time += Time.deltaTime)
        {
            timerDisplay.text = (minigameDuration - time).ToString("0.00");
            yield return null;
        }
        timerDisplay.text = "0.00";

        for(int i = 0; i < ballThrower.Length; i++)
            ballThrower[i].StopShooting();

        //Score gestion des tickets
        TicketsManager.AddTickets(counter.GetScore()/2);
    }

    public void OnRetourAuMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnReset()
        {
            Debug.Log("Reset");
            StopAllCoroutines();
            //Arret des tirs
            for(int i = 0; i < ballThrower.Length; i++)
                ballThrower[i].StopShooting();
            counter.Reset();
            //Relancement du jeu
            StartCoroutine(Start());
            
        }
}
