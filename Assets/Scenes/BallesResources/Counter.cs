using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    private int score = 0;

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

    public void Increment(){
        score++;
        scoreDisplay.text = score.ToString();
    }
}
