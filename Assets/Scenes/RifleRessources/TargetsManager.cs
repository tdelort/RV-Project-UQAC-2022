using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TireCarabine
{
    public class TargetsManager : MonoBehaviour
    {
        [System.Serializable]
        struct TargetRow
        {
            public Transform startPoint;
            public Transform endPoint;
            public float speed;
            public GameObject targetPrefab;
            public float minSpawnDelay;
            public float probability;
            public int score;
        }

        [SerializeField] TargetRow[] targetRows;

        [SerializeField] float fullDuration = 10f;

        public UnityEvent<int> onTargetHit = new UnityEvent<int>();

        [SerializeField] ScoreManager scoreManager;

        void Start()
        {
            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            scoreManager.SetTimeLeft(3);
            yield return new WaitForSeconds(1);
            scoreManager.SetTimeLeft(2);
            yield return new WaitForSeconds(1);
            scoreManager.SetTimeLeft(1);
            yield return new WaitForSeconds(1);
            for(int row = 0; row < targetRows.Length; row++)
            {
                StartCoroutine(SpawnTargetsForRow(row));
            }

            StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            float timeLeft = fullDuration;
            while(timeLeft > 0)
            {
                scoreManager.SetTimeLeft(timeLeft);
                timeLeft -= Time.deltaTime;
                yield return null;
            }
            scoreManager.SetTimeLeft(0);
        }

        IEnumerator SpawnTargetsForRow(int i)
        {
            TargetRow row = targetRows[i];
            float time = 0f;
            while(time < fullDuration)
            {
                yield return new WaitForSeconds(row.minSpawnDelay);
                time += row.minSpawnDelay;


                float random = Random.Range(0f, 1f);
                if(random < row.probability)
                {
                    GameObject target = Instantiate(row.targetPrefab);
                    target.transform.position = row.startPoint.position;
                    target.GetComponent<Target>().Init(row.endPoint, row.speed, row.score, this);
                }
            }
        }

        public void OnReset()
        {
            StopAllCoroutines();

            foreach(Target target in FindObjectsOfType<Target>())
                Destroy(target.gameObject);

            StartCoroutine(StartRoutine());
        }
    }
}