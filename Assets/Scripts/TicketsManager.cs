using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TicketsManager : MonoBehaviour
{
    public static TicketsManager Instance;

    [SerializeField] int tickets;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        tickets = PlayerPrefs.GetInt("Tickets", 0);
        OnTicketsChange();
    }
    
    public static void AddTickets(int amount)
    {
        Instance.tickets += amount;
    }

    public static bool RemoveTickets(int amount)
    {
        if(Instance.tickets >= amount)
        {
            Instance.tickets -= amount;
            return true;
        }
        return false;
    }

    public static int GetTickets()
    {
        return Instance.tickets;
    }

    private void OnTicketsChange()
    {
        PlayerPrefs.SetInt("Tickets", tickets);
        Menu.MenuManager menuManager = FindObjectOfType<Menu.MenuManager>();
        if(menuManager != null)
        {
            menuManager.OnTicketsChange();
        }
    }
}
