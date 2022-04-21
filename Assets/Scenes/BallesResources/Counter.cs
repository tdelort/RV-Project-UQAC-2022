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
    void Update()
    {
        
    }

    public void Decrement(){
        score--;
        if (score <0) {
            score = 0;
        }
        scoreDisplay.text = score.ToString();
    }

    public void Reset() {
        score = startScore;
    }
}
