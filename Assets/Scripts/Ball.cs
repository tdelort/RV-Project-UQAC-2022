using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField]
    private float destructionTime = 5f;

    private bool hasTriggered = false;

    public AudioSource impactSound;

    public Counter counter;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(destructionTime);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        GetComponent<Rigidbody>().useGravity = true;
        if (!hasTriggered && collision.gameObject.CompareTag("Player")) 
        {
            Debug.Log("Collision with player");
            hasTriggered = true;
            impactSound.Play();
            counter.Increment();
        }
    }
}
