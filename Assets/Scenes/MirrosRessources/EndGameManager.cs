using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Labyrinthe
{
    public class EndGameManager : MonoBehaviour
    {
        [SerializeField] UnityEvent onEndLabyrinthe;

        void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                onEndLabyrinthe.Invoke();
            }
        }
    }
}