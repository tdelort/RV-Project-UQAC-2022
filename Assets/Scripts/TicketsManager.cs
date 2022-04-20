using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketsManager : MonoBehaviour
{
    public static TicketsManager Instance;

    int tickets;

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
}
