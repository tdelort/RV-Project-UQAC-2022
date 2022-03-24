using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taupe : MonoBehaviour
{
    [SerializeField] private Vector3 initialPosition;
    private float time;

    private void OnEnable()
    {
        transform.localPosition = initialPosition;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(time < 0.1)
        {
            transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + Time.deltaTime * 10,
            transform.localPosition.z);
            time += Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Marteau"))
        {
            GetComponentInParent<GestionTaupes>().AddScore(1);
            gameObject.SetActive(false);
        }
    }
}
