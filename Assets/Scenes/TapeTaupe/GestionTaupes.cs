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
        private int spawnedTaupe = 0;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            float time = startTime;
            while(time > 0)
            {
                scoreText.text = "Début dans " + time.ToString();
                time -= 1;
                yield return new WaitForSeconds(1f);
            }
            UpdateText();
            StartCoroutine(SpawnTaupe());
        }

        IEnumerator SpawnTaupe()
        {
            for (int i = 0; i < spawnMax; i++)
            {
                yield return new WaitForSeconds(timeRespawn);
                int taupe = Random.Range(0, taupeArray.Length - 1);
                taupeArray[taupe].gameObject.SetActive(true);
                spawnedTaupe += 1;
                UpdateText();
                yield return new WaitForSeconds(timeActive);
                taupeArray[taupe].gameObject.SetActive(false);
            }
        }

        public void OnReset()
        {
            StopAllCoroutines();
            score = 0;
            spawnedTaupe = 0;
            StartCoroutine(OnResetCorou());
        }

        private IEnumerator OnResetCorou()
        {
            float time = startTime;
            foreach(Taupe taupe in taupeArray)
            {
                taupe.gameObject.SetActive(false);
            }
            while (time > 0)
            {
                scoreText.text = "Début dans " + time.ToString();
                time -= 1;
                yield return new WaitForSeconds(1f);
            }
            UpdateText();
            StartCoroutine(SpawnTaupe());
        }

        public void OnQuit()
        {
            //BackToMenu
        }

        public void AddScore(int i)
        {
            if(i == 0)
            {
                StartCoroutine(WrongVibration());
            }
            else
            {
                StartCoroutine(Vibration());
            }
            score += i;
            UpdateText();
        }

        public int GetScore()
        {
            return score;
        }

        private void UpdateText()
        {
            scoreText.text = "Score :\n" + score.ToString() + " / " + spawnedTaupe.ToString();
        }
        IEnumerator Vibration()
        {
            OVRInput.SetControllerVibration(0.25f, 1, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.2f);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }

        IEnumerator WrongVibration()
        {
            OVRInput.SetControllerVibration(0.75f, 1, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.1f);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}