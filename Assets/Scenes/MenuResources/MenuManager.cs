using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        [System.Serializable]
        struct Gift 
        {
            public GameObject obj;
            [HideInInspector] public Vector3 spawnPoint;
            [HideInInspector] public Quaternion spawnRotation;
            public int price;
        }

        [SerializeField] Gift[] gifts;
        [SerializeField] TMPro.TMP_Text ticketsText;

        void Start()
        {
            for(int i = 0; i < gifts.Length; i++)
            {
                gifts[i].spawnPoint = gifts[i].obj.transform.position;
                gifts[i].spawnRotation = gifts[i].obj.transform.rotation;
            }

            OnResetToys();
            OnTicketsChange();
        }

        public void OnBuy(int i)
        {
            if(i <= gifts.Length)
            {
                if(TicketsManager.GetTickets() >= gifts[i].price)
                {
                    bool wasBought = PlayerPrefs.GetInt("Gift" + i, 0) == 1;
                    if(wasBought) return;
                    TicketsManager.RemoveTickets(gifts[i].price);
                    gifts[i].obj.SetActive(true);
                    gifts[i].obj.transform.position = gifts[i].spawnPoint;
                    gifts[i].obj.transform.rotation = gifts[i].spawnRotation;
                    PlayerPrefs.SetInt("Gift" + i, 1);
                }
            }
            OnTicketsChange();
        }

        public void OnResetToys()
        {
            for(int i = 0; i < gifts.Length; i++)
            {
                if(PlayerPrefs.GetInt("Gift" + i, 0) == 1)
                {
                    gifts[i].obj.SetActive(true);
                    gifts[i].obj.transform.position = gifts[i].spawnPoint;
                    gifts[i].obj.transform.rotation = gifts[i].spawnRotation;
                }
            }
            OnTicketsChange();
        }

        public void OnChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void OnTicketsChange()
        {
            ticketsText.text = TicketsManager.GetTickets().ToString();
        }
    }
}
