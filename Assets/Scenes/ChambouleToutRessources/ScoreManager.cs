using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChamboulTout
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text scoreText;
        [SerializeField] BallsManager bm;
        int score = 0;
        public int Score 
        { 
            get => score; 
            set { score = value; scoreText.text = score.ToString(); } 
        }

        void Start()
        {
            Score = 0;
        }

        public void OnCanFallen(GameObject go)
        {
            Score++;
        }

        public void OnReset()
        {
            int nb = bm.NbBallsNearSpawn();
            Debug.LogError("nb = " + nb);
            if(nb == 0)
                TicketsManager.AddTickets(Score);
            Score = 0;
        }

        public void OnRetourAuMenu()
        {
            int nb = bm.NbBallsNearSpawn();
            Debug.LogError("nb = " + nb);
            if(nb == 0)
                TicketsManager.AddTickets(Score);
            SceneManager.LoadScene("Menu");
        }
    }
}