using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinthe
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text timeText;
        [SerializeField] TMPro.TMP_Text endTimeText;
        [SerializeField] Transform player;

        bool isStarted = false;
        float time = 0;
        Vector3 startPosition;
        Quaternion startRotation;

        void Start()
        {
            startPosition = player.position;
            startRotation = player.rotation;
        }

        void Update()
        {
            if(OVRInput.GetDown(OVRInput.Button.One |
                                OVRInput.Button.Two |
                                OVRInput.Button.Three |
                                OVRInput.Button.Four))
            {
                OnReset();
                FindObjectOfType<StickFactory>().OnReset();
            }
        }

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

        public void StopLevel()
        {
            if(isStarted)
                isStarted = false;

            StopAllCoroutines();
            UpdateTimeTexts(time);
        }

        public void OnReset()
        {
            if(isStarted)
                isStarted = false;

            StopAllCoroutines();
            time = 0;
            UpdateTimeTexts(time);
            player.position = startPosition;
            player.rotation = startRotation;
        }

        void UpdateTimeTexts(float time)
        {
            timeText.text = time.ToString("0.00");
            endTimeText.text = time.ToString("0.00");
        }

        IEnumerator Timer()
        {
            time = 0;
            while(true)
            {
                yield return null;
                time += Time.deltaTime;
                UpdateTimeTexts(time);
            }
        }
    }
}