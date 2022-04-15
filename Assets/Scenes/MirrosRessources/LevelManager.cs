using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinthe
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text timeText;
        bool isStarted = false;

        void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                StartLevel();
            }
        }

        public void StartLevel()
        {
            if(!isStarted)
            {
                isStarted = true;
                StartCoroutine(Timer());
            }
        }

        IEnumerator Timer()
        {
            float time = 0;
            while(true)
            {
                yield return null;
                time += Time.deltaTime;
                timeText.text = time.ToString("0.00");
            }
        }
    }
}