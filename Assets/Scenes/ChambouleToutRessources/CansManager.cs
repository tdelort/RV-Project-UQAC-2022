using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChamboulTout
{
    public class CansManager : MonoBehaviour
    {
        [SerializeField] float thresholdHeight = 1f;
        [SerializeField] GameObject pyramidPrefab;
        List<GameObject> cans = new List<GameObject>();

        public UnityEvent<GameObject> onCanFallen = new UnityEvent<GameObject>();

        void Start()
        {
            OnReset();
            onCanFallen.AddListener(OnCanFallen);
        }

        void Update()
        {
            List<GameObject> cansThatFell = new List<GameObject>();
            foreach(GameObject can in cans)
            {
                if(can.transform.position.y < thresholdHeight)
                {
                    cansThatFell.Add(can);
                }
            }
            
            foreach(GameObject can in cansThatFell)
                onCanFallen.Invoke(can);
        }

        void OnCanFallen(GameObject go)
        {
            cans.Remove(go);
            Destroy(go);
        }

        public void OnReset()
        {
            Debug.Log("Reset");
            // Destroy all cans
            foreach(GameObject can in cans)
                Destroy(can);
            cans.Clear();
            
            // Spawn new cans
            GameObject pyramid = Instantiate(pyramidPrefab, transform.position, transform.rotation);
            foreach(Transform child in pyramid.transform)
                cans.Add(child.gameObject);
        }
    }
}
