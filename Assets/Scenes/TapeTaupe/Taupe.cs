using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapeTaupe
{
    public class Taupe : MonoBehaviour
    {
        [SerializeField] private float minSpeed;
        [SerializeField] private GameObject hammer;
        [SerializeField] private ParticleSystem pouf;

        private Vector3 hammerSpeed;
        private Vector3 hammerLastPosition;
        private Vector3 initialPosition;
        private float time;

        private void Awake()
        {
            initialPosition = transform.position;
            gameObject.SetActive(false);
            hammerLastPosition = hammer.transform.position;
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
            hammerSpeed = (hammer.transform.position - hammerLastPosition) / Time.deltaTime;
            hammerLastPosition = hammer.transform.position;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Marteau"))
            {
                Debug.Log(hammerSpeed);
                if(Mathf.Abs(hammerSpeed.y) < minSpeed)
                {
                    GetComponentInParent<GestionTaupes>().AddScore(0);
                    return;
                }
                GetComponentInParent<GestionTaupes>().AddScore(1);
                Instantiate(pouf, transform.position, transform.rotation);
                gameObject.SetActive(false);
            }
        }
    }
}
