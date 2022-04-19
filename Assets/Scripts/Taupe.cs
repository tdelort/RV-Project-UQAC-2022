using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapeTaupe
{
    public class Taupe : MonoBehaviour
    {
        private Vector3 initialPosition;
        private float time;

        private void Awake()
        {
            initialPosition = transform.position;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            transform.position = initialPosition;
            time = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (time < 0.1)
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
            Debug.Log("ok");
            if (other.CompareTag("Marteau"))
            {
                GetComponentInParent<GestionTaupes>().AddScore(1);
                gameObject.SetActive(false);
            }
        }
    }
}
