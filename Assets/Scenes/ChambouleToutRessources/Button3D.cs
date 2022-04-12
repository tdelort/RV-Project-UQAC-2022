using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChamboulTout
{
    public class Button3D : MonoBehaviour
    {
        public UnityEvent onPress = new UnityEvent();

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                onPress.Invoke();
            }
        }
    }
}