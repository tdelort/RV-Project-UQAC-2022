using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TireCarabine
{
    public class Rifle : MonoBehaviour
    {   
        [SerializeField] Transform otherHand;

        [SerializeField] GameObject bulletPrefab;
        [SerializeField] float cooldown = 0.5f;
        float timeSinceLastShot = 0;

        void Update()
        {
            Quaternion localRotation = transform.localRotation;
            transform.LookAt(otherHand.position, Vector3.up);
            transform.localRotation = Quaternion.Euler(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                localRotation.z
            );

            if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && timeSinceLastShot > cooldown)
            {
                StartCoroutine(Shoot());
                timeSinceLastShot = 0;
            }

            timeSinceLastShot += Time.deltaTime;
        }

        IEnumerator Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;

            OVRInput.SetControllerVibration(0.25f, 1, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0.25f, 1, OVRInput.Controller.RTouch);
            AudioManager.instance.PlaySoundAt(AudioManager.SoundType.SHOOT, string.Empty, transform.position);
            yield return new WaitForSeconds(Mathf.Min(0.1f, cooldown));
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}