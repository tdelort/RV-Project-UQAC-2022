using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEmitter : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] float ballSpeed = 10f;

    [SerializeField] Vector3 fireDirection = Vector3.forward;

    Bounds bounds;

    void Start()
    {
        Collider collider = GetComponent<Collider>();
        Debug.Assert(collider != null, "BallEmitter needs a collider");
        Debug.Assert(collider.isTrigger, "BallEmitter needs a trigger collider");
        Debug.Assert(ballPrefab.GetComponent<CustomBall>() != null, "BallEmitter needs a ball with CustomBall script attached");
        bounds = collider.bounds;
    }

    public void EmitBall()
    {
        Vector3 randomPointInBounds = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        GameObject ball = Instantiate(ballPrefab, randomPointInBounds, Quaternion.identity);
        ball.GetComponent<Rigidbody>().velocity = transform.TransformDirection(fireDirection) * ballSpeed;
        GameAudioManager.instance.PlaySoundAt(GameAudioManager.SoundType.SHOOT, string.Empty, transform.position);
    }

}
