using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChamboulTout
{
    public class BallsManager : MonoBehaviour
    {
        [SerializeField] float thresholdHeight = 1f;
        [SerializeField] List<Transform> originalBalls = new List<Transform>();
        List<Vector3> ballSpawnPointsPositions = new List<Vector3>();

        [SerializeField] GameObject ballPrefab;

        List<(GameObject, int)> balls = new List<(GameObject, int)>();

        public UnityEvent<GameObject, int> onBallFallen = new UnityEvent<GameObject, int>();

        void Start()
        {
            onBallFallen.AddListener(OnBallFallen);
            for(int i = 0; i < originalBalls.Count; i++)
            {
                GameObject newBall = originalBalls[i].gameObject;
                ballSpawnPointsPositions.Add(originalBalls[i].position);
                balls.Add((newBall, i));
            }
        }

        void SetBall(int i)
        {
            GameObject b = balls.Find(x => x.Item2 == i).Item1;
            b.transform.position = ballSpawnPointsPositions[i];
            b.GetComponent<Rigidbody>().velocity = Vector3.zero;
            b.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        void Update()
        {
            List<(GameObject, int)> ballsThatFell = new List<(GameObject, int)>();
            foreach((GameObject, int) ball in balls)
            {
                if(ball.Item1.transform.position.y < thresholdHeight)
                {
                    ballsThatFell.Add(ball);
                }
            }
            
            foreach((GameObject, int) ball in ballsThatFell)
                onBallFallen.Invoke(ball.Item1, ball.Item2);
        }

        void OnBallFallen(GameObject go, int index)
        {
            //balls.Remove((go, index));
            //Destroy(go);
            //SpawnBall(index);
        }

        public void OnReset()
        {
            Debug.Log("Reset");

            // Destroy all balls
            foreach((GameObject, int) ball in balls)
            {
                OVRGrabbable grabbable = ball.Item1.GetComponent<OVRGrabbable>();
                if(grabbable.isGrabbed)
                {
                    Debug.Log("Reset grab before reset ball");
                    grabbable.grabbedBy.GetComponent<CustomGrabber>().EndGrab();
                }
            }

            // Spawn new balls
            for(int i = 0; i < originalBalls.Count; i++)
                SetBall(i);
        }

        public int NbBallsNearSpawn()
        {
            int nbBallsNearSpawn = 0;
            foreach((GameObject, int) ball in balls)
            {
                if((ball.Item1.transform.position - ballSpawnPointsPositions[ball.Item2]).magnitude < 0.1f)
                    nbBallsNearSpawn++;
            }
            return nbBallsNearSpawn;
        }
    }
}
