using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TireCarabine
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] Vector3 gravity = new Vector3(0, -9.81f, 0);
        [SerializeField] float lifeTime = 3f;
        Rigidbody rb;

        IEnumerator Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject);

        }

        void FixedUpdate()
        {
            rb.AddForce(gravity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
}
