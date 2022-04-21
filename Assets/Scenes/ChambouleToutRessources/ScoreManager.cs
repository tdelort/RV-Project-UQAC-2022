using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChamboulTout
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text scoreText;
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
            Score = 0;
        }

        public void OnRetourAuMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}