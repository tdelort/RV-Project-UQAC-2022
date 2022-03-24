using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionTaupes : MonoBehaviour
{
    [SerializeField] private Taupe[] taupeArray;
    [SerializeField] private int spawnMax;
    [SerializeField] private int timeActive;
    [SerializeField] private int timeRespawn;

    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpanwTaupe());
    }

    IEnumerator SpanwTaupe()
    {
        for(int i = 0; i < spawnMax; i++)
        {
            yield return new WaitForSeconds(timeRespawn);
            int taupe = Random.Range(0, taupeArray.Length - 1);
            taupeArray[taupe].gameObject.SetActive(true);
            yield return new WaitForSeconds(timeActive);
            taupeArray[taupe].gameObject.SetActive(false);
        }
    }

    public void AddScore(int i)
    {
        score += i;
    }

    public int GetScore()
    {
        return score;
    }
}
