using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{   
    private static int startScore = 30;
    private int score = startScore;

    [SerializeField]
    private TMP_Text scoreDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        scoreDisplay.text = score.ToString();
    }

    // Update is called once per frame
    public int GetScore()
    {
        return score;
    }

    public void Decrement(){
        
        if (score > 0) {
            score--;
        }
        scoreDisplay.text = score.ToString();
    }

    public void Reset() {
        score = startScore;
        scoreDisplay.text = score.ToString();
    }
}
