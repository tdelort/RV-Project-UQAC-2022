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
    void Start()
    {
        Destroy(gameObject, destructionTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() {
        if(!hasTriggered) {
            counter.Increment();
        }
    }

    void OnCollisionEnter(Collision collision){
        GetComponent<Rigidbody>().useGravity = true;
        if (collision.gameObject.name == "PlayerController"){
            hasTriggered = true;
            impactSound.Play();
        }
    }
}
