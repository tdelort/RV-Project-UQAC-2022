using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TireCarabine
{
    public class Target : MonoBehaviour
    {
        Transform endPoint;
        float speed = 0f;
        TargetsManager manager;
        int score = 0;

        public void Init(Transform endPoint, float speed, int score, TargetsManager manager)
        {
            this.endPoint = endPoint;
            this.speed = speed;
            this.score = score;
            this.manager = manager;
            StartCoroutine(Move());
        }

        IEnumerator Move()
        {
            while((transform.position - endPoint.position).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);
                yield return null;
            }
            Destroy(gameObject);
        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Ball")
            {
                manager.onTargetHit.Invoke(score);
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}