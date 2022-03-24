using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomBall : MonoBehaviour
{
    [SerializeField] Vector3 gravity = new Vector3(0, -9.81f, 0);
    [SerializeField] float lifeTime = 5f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        StartCoroutine(DestroyAfterTime());
    }

    void FixedUpdate()
    {
        rb.AddForce(gravity * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        // insert pre destroy actions here
        Destroy(gameObject);
    }
}
