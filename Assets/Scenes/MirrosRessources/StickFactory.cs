using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinthe
{
    public class StickFactory : MonoBehaviour
    {
        [SerializeField] GameObject stickPrefab;
        [SerializeField] Color[] colors;

        Vector3 velocity;
        Vector3 angularVelocity;

        Vector3 lastPosition;
        Quaternion lastRotation;

        List<GameObject> sticks = new List<GameObject>();

        GameObject Get()
        {
            GameObject stick = Instantiate(stickPrefab);
            sticks.Add(stick);
            Color col = colors[Random.Range(0, colors.Length)];
            stick.GetComponent<Renderer>().material.SetColor("_EmissionColor", col);
            stick.GetComponentInChildren<Light>().color = col;
            return stick;
        }

        void Start()
        {
            GameObject stick = Get();
            stick.transform.SetParent(transform);
            stick.transform.localPosition = Vector3.zero;
            stick.transform.localRotation = Quaternion.identity;
        }

        IEnumerator PrepareStick()
        {
            yield return new WaitForSeconds(1);
            GameObject stick = Get();
            stick.transform.SetParent(transform);
            stick.transform.localPosition = Vector3.zero;
            stick.transform.localRotation = Quaternion.identity;
        }

        void Update()
        {
            //update velocity
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            // handle inputs
            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                if(transform.childCount != 0)
                {
                    Transform child = transform.GetChild(0);
                    child.parent = null;
                    child.GetComponent<Rigidbody>().isKinematic = false;
                    child.GetComponent<Rigidbody>().velocity = velocity;
                    child.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

                    StartCoroutine(PrepareStick());
                }
            }
        }

        public void OnReset()
        {
            foreach(GameObject s in sticks)
            {
                Destroy(s);
            }
            sticks.Clear();

            GameObject stick = Get();
            stick.transform.SetParent(transform);
            stick.transform.localPosition = Vector3.zero;
            stick.transform.localRotation = Quaternion.identity;
        }
    }
}