using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChamboulTout
{
    public class Can : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Ball")
            {
                //TODO : play sound
            }
        }
    }
}
