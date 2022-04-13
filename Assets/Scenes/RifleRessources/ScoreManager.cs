using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TireCarabine
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text scoreText;
        [SerializeField] TMPro.TMP_Text countdownText;
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

        public void OnTargetHit(int targetScore)
        {
            Score += targetScore;
        }

        public void SetTimeLeft(float timeLeft)
        {
            countdownText.text = timeLeft.ToString("N0");
        }

        public void OnReset()
        {
            Score = 0;
        }
    }
}