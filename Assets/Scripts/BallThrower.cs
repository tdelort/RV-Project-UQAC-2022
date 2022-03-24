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




    // Start is called before the first frame update
    void Start()
    {
        //20 degres en y - direction axe z

        StartCoroutine(Shoot(INTERVAL));
        
    }

    // Update is called once per frame
    void Update()
    {   
        transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime *smoothSpeed* (1f/INTERVAL));
        
    }



    IEnumerator Shoot(float interval)
    {   
        yield return new WaitForSeconds(interval/6f);
        Quaternion lookDirection = Quaternion.LookRotation(player.transform.position - shootStart.transform.position);
        float rotateHorizontal = Random.Range(-MAX_ROTATION_Y, MAX_ROTATION_Y);
        float rotateVertical = Random.Range(-MAX_ROTATION_X, MAX_ROTATION_X);
        //Quaternion.Euler(rotateVertical, + rotateHorizontal, 0)
        target = lookDirection ;
        
        yield return new WaitForSeconds(5f * interval /6f);

        //ICI JE SHOOT
        GameObject myBall = Instantiate(ball, shootStart.transform.position, transform.rotation);
        myBall.GetComponent<Rigidbody>().velocity = transform.TransformDirection(0,0,shootSpeed);
        myBall.GetComponent<Ball>().counter = counter;
        //transform.Rotate(rotateVertical, rotateHorizontal, 0, Space.Self);
        StartCoroutine(Shoot(interval)); 
        //Balle : ajouter compteur dedans, destroy, if destroysans trigger increment, play sound on invoc, play sound on trigger, voir comment trigger
    }
}
