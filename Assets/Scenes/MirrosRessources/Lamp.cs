using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinthe
{
    public class Lamp : MonoBehaviour
    {
        void Update()
        {
            if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                ToggleLights();
            }
        }

        void ToggleLights()
        {
            foreach(Light light in GetComponentsInChildren<Light>())
            {
                light.enabled = !light.enabled;
            }
        }
    }
}
