using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInputSign : MonoBehaviour
{
    
    [System.Serializable]
    public struct Entry
    {
        public OVRInput.RawButton button;
        public bool isGreen;

        public GameObject greenImage;

        public GameObject redImage;
    }
    [SerializeField] List<Entry> entries;

    // Update is called once per frame
    void Update()
    {
        foreach(Entry E in entries) {
            OVRInput.RawButton button = E.button;

            if(E.isGreen) {
                E.greenImage.SetActive(OVRInput.Get(button));
                E.redImage.SetActive(false);
            } else {
                E.redImage.SetActive(OVRInput.Get(button));
                E.greenImage.SetActive(false);
            }
        }
    }

    
}
