using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [SerializeField] Transform otherHand;

    [SerializeField] GameObject bulletPrefab;

    void Update()
    {
        Quaternion localRotation = transform.localRotation;
        transform.LookAt(otherHand.position, Vector3.up);
        transform.localRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            localRotation.z
        );

        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;
    }
}
