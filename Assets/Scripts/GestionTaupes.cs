using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TapeTaupe
{
    public class GestionTaupes : MonoBehaviour
    {
        [SerializeField] private Taupe[] taupeArray;
        [SerializeField] private int spawnMax;
        [SerializeField] private float timeActive;
        [SerializeField] private float timeRespawn;
        [SerializeField] private int startTime = 3;
        [SerializeField] TMPro.TMP_Text scoreText;

        private int score = 0;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            while(startTime > 0)
            {
                scoreText.text = "Début dans " + startTime.ToString();
                startTime -= 1;
                yield return new WaitForSeconds(1f);
            }
            scoreText.text = "0";
            StartCoroutine(SpanwTaupe());
        }

        IEnumerator SpanwTaupe()
        {
            for (int i = 0; i < spawnMax; i++)
            {
                yield return new WaitForSeconds(timeRespawn);
                int taupe = Random.Range(0, taupeArray.Length - 1);
                taupeArray[taupe].gameObject.SetActive(true);
                yield return new WaitForSeconds(timeActive);
                taupeArray[taupe].gameObject.SetActive(false);
            }
        }

        public void AddScore(int i)
        {
            score += i;
            scoreText.text = score.ToString();
        }

        public int GetScore()
        {
            return score;
        }
    }
}
