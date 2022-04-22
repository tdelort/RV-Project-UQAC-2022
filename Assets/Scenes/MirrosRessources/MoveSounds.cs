using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSounds : MonoBehaviour
{
    public static float distanceBetweenSteps = 0.66f;
    Vector3 lastPosTicked;
    void Update()
    {
        float distSinceLastFrame = (transform.position - lastPosTicked).magnitude;
        if (distSinceLastFrame >= distanceBetweenSteps) 
        {
            AudioManager.instance.PlaySound(AudioManager.SoundType.WALK, string.Empty);
            lastPosTicked = transform.position;
        }
    }
}
