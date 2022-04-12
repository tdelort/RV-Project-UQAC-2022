using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChamboulTout
{
    public class Can : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Ball")
            {
                //TODO : play sound
            }
        }
    }
}
