using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] TMPro.TMP_Text taupeText;
        [SerializeField] GameObject hammer;

        private int score = 0;
        private int spawnedTaupe = 0;
        List<Color> colorInit = new List<Color>();

        // Start is called before the first frame update
        void Start()
        {
            foreach(Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                colorInit.Add(render.material.color);
            }
        }

        IEnumerator SpawnTaupe()
        {
            for (int i = 0; i < spawnMax; i++)
            {
                yield return new WaitForSeconds(timeRespawn);
                taupeText.text = "Taupes restantes : " + (spawnMax - 1 - i).ToString();
                int taupe = Random.Range(0, taupeArray.Length - 1);
                taupeArray[taupe].gameObject.SetActive(true);
                spawnedTaupe += 1;
                UpdateText();
                yield return new WaitForSeconds(timeActive);
                taupeArray[taupe].gameObject.SetActive(false);
            }
            TicketsManager.AddTickets((int) score / 2);
        }

        public void OnStart()
        {
            StopAllCoroutines();
            score = 0;
            spawnedTaupe = 0;
            StartCoroutine(StartCorou());
        }

        private IEnumerator StartCorou()
        {
            float time = startTime;
            while (time > 0)
            {
                scoreText.text = "Début dans " + time.ToString();
                time -= 1;
                yield return new WaitForSeconds(1f);
            }
            taupeText.text = "Taupes restantes : " + spawnMax.ToString();
            UpdateText();
            StartCoroutine(SpawnTaupe());
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
            SceneManager.LoadScene("Menu");
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
            int i = 0;
            foreach (Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                render.material.color = Color.green;
            }
            OVRInput.SetControllerVibration(0.25f, 1, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.2f);
            foreach (Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                render.material.color = colorInit[i];
                i++;
            }
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }

        IEnumerator WrongVibration()
        {
            int i = 0;
            foreach (Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                render.material.color = Color.red;
            }
            OVRInput.SetControllerVibration(0.75f, 1, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.1f);
            foreach (Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                render.material.color = colorInit[i];
                i++;
            }
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.15f);
            foreach (Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                render.material.color = Color.red;
            }
            OVRInput.SetControllerVibration(0.75f, 1, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.1f);
            i = 0;
            foreach (Renderer render in hammer.GetComponentsInChildren<Renderer>())
            {
                render.material.color = colorInit[i];
                i++;
            }
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}