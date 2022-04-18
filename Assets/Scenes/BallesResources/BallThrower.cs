using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour
{   
    [SerializeField]
    private float MAX_ROTATION_Y = 20f;

    [SerializeField]
    private float MAX_ROTATION_X = 10f;

    [SerializeField]
    private float INTERVAL = 3f;

    [SerializeField]
    private float delayBeforeShooting = 1f;

    [SerializeField]
    private float randomTimeDeviation = 0.5f;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject ball;

    [SerializeField]
    private GameObject shootStart;

    [SerializeField]
    private float shootSpeed = 5f;

    [SerializeField]
    private float smoothSpeed = 5f;

    [SerializeField]
    private Counter counter;

    private Quaternion target = Quaternion.Euler(0,0,0);


    private bool isShooting = false;

    // Update is called once per frame
    void Update()
    {   
        if(isShooting)
            transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime *smoothSpeed* (1f/INTERVAL));
    }

    public void StartShooting() {
        StartCoroutine(Shoot());
    }

    public void StopShooting() {
        StopAllCoroutines();
        isShooting = false;
    }



    IEnumerator Shoot()
    {   
        yield return new WaitForSeconds(delayBeforeShooting);
        while(true)
        {
            float interval = INTERVAL + Random.Range(-randomTimeDeviation, randomTimeDeviation);
            yield return new WaitForSeconds(interval/6f);
            Quaternion lookDirection = Quaternion.LookRotation(player.transform.position - shootStart.transform.position);
            float rotateHorizontal = Random.Range(-MAX_ROTATION_Y, MAX_ROTATION_Y);
            float rotateVertical = Random.Range(-MAX_ROTATION_X, MAX_ROTATION_X);
            //Quaternion.Euler(rotateVertical, + rotateHorizontal, 0)
            target = lookDirection ;
            isShooting = true;

            yield return new WaitForSeconds(5f * interval /6f);

            //ICI JE SHOOT
            GameObject myBall = Instantiate(ball, shootStart.transform.position, transform.rotation);
            myBall.GetComponent<Rigidbody>().velocity = transform.TransformDirection(0,0,shootSpeed);
            myBall.GetComponent<Ball>().counter = counter;
            //transform.Rotate(rotateVertical, rotateHorizontal, 0, Space.Self);
            //Balle : ajouter compteur dedans, destroy, if destroysans trigger increment, play sound on invoc, play sound on trigger, voir comment trigger
            
        }
    }
}
