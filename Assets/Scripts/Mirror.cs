using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] Vector2Int resolution = new Vector2Int(720, 1280);
    Camera playerCamera;
    Camera mirrorCamera;

    void Start()
    {
        RenderTextureDescriptor desc = new RenderTextureDescriptor();
        desc.width = resolution.x;
        desc.height = resolution.y;
        desc.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        Debug.Log("res : " + desc.width + " " + desc.height);
        desc.colorFormat = RenderTextureFormat.ARGB32;
        desc.volumeDepth = 1;
        desc.msaaSamples = 1;
        RenderTexture rt = new RenderTexture(desc);

        mirrorCamera = GetComponentInChildren<Camera>();
        mirrorCamera.targetTexture = rt;
        //GetComponent<Renderer>().material.mainTextureScale = transform.localScale;
        GetComponentInChildren<Renderer>().material.mainTexture = rt;

        playerCamera = GameObject.Find("LeftEyeAnchor").GetComponent<Camera>();
    }

    void Update()
    {
        //mirrorCamera.transform.position = playerCamera.transform.position;
        //mirrorCamera.transform.rotation = playerCamera.transform.rotation;

        mirrorCamera.transform.localPosition = transform.InverseTransformPoint(playerCamera.transform.position);
        mirrorCamera.transform.localRotation = Quaternion.Inverse(transform.rotation) * playerCamera.transform.rotation;
    }
}
