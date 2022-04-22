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
        [SerializeField] private Transform endPosition;

        private Vector3 hammerSpeed;
        private Vector3 hammerLastPosition;
        private Vector3 initialPosition;
        private float time;

        private void Awake()
        {
            initialPosition = transform.position;
            gameObject.SetActive(false);
            hammerLastPosition = hammer.transform.position;
            GameAudioManager.instance.PlaySoundAt(GameAudioManager.SoundType.BOING, string.Empty, transform.position);
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
                transform.position = Vector3.Lerp(initialPosition, endPosition.position, 10*time);
                time += Time.deltaTime;
            }
            else
            {
                transform.position = endPosition.position;
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
                GameAudioManager.instance.PlaySoundAt(GameAudioManager.SoundType.BONK, string.Empty, transform.position);
                Instantiate(pouf, transform.position, transform.rotation);
                gameObject.SetActive(false);
            }
        }
    }
}
