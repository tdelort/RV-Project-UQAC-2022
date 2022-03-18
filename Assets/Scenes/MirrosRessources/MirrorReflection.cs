// refrence http://wiki.unity3d.com/index.php/MirrorReflection4

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This is in fact just the Water script from Pro Standard Assets,
// just with refraction stuff removed.

// Make mirror live-update even when not in play mode

public class MirrorReflection : MonoBehaviour
{
    const string ReflectionTexParamStr = "_ReflectionTex";
    static int nestCountMax = 5;
    private static int nestCount;


    public bool m_DisablePixelLights = true;
    public int m_TextureSize = 256;
    public float m_ClipPlaneOffset = 0.07f;

    public LayerMask m_ReflectLayers = -1;

    Dictionary<Camera, (Camera, RenderTexture)> mirrorDatas = new Dictionary<Camera, (Camera, RenderTexture)>();



    Material mat;

    private void Awake()
    {
        mat = GetComponent<Renderer>()?.material;
    }

    Stack<Texture> texs = new Stack<Texture>();
    private void OnRenderObject()
    {
        if (texs.Any())
        {
            var tex = texs.Pop();
            mat.SetTexture(ReflectionTexParamStr, tex);
        }
        //Debug.Log($"End [{Camera.current.name}] {name} ");
    }

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
    {
        if (!enabled || (mat == null))
            return;

        Camera cam = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();
        if (cam == null)
            return;


        //Debug.Log($"[{cam.name}] {name} {nestCount}");

        Texture reflectionTex = Texture2D.blackTexture;
        // Safeguard from recursive reflections.        
        if (nestCount <= nestCountMax)
        {
            nestCount++;

            // find out the reflection plane: position and normal in world space
            Vector3 pos = transform.position;
            //Vector3 normal = transform.up;
            Vector3 normal = transform.forward;

            var camTrans = cam.transform;
            var cull = Vector3.Dot(camTrans.position - pos, normal) >= 0f;
            if (!cull)
            {
                var (refCam, reftex) = GetMirrorData(cam);


                // Optionally disable pixel lights for reflection
                int oldPixelLightCount = QualitySettings.pixelLightCount;
                if (m_DisablePixelLights)
                    QualitySettings.pixelLightCount = 0;

                UpdateCameraModes(cam, refCam);

                // Render reflection
                // Reflect camera around reflection plane
                float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
                Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

                Matrix4x4 reflection = Matrix4x4.zero;
                CalculateReflectionMatrix(ref reflection, reflectionPlane);
                refCam.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                Vector4 clipPlane = CameraSpacePlane(refCam, pos, normal, 1.0f);
                Matrix4x4 projection = cam.CalculateObliqueMatrix(clipPlane);
                refCam.projectionMatrix = projection;

                refCam.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
                refCam.targetTexture = reftex;

                GL.invertCulling = nestCount % 2 == 1;

                refCam.Render();

                GL.invertCulling = !GL.invertCulling;


                // Restore pixel light count
                if (m_DisablePixelLights)
                    QualitySettings.pixelLightCount = oldPixelLightCount;

                reflectionTex = reftex;
            }


            nestCount--;
        }

        if (nestCount > 0) texs.Push(mat.GetTexture(ReflectionTexParamStr));
        mat.SetTexture(ReflectionTexParamStr, reflectionTex);
    }


    // Cleanup all the objects we possibly have created
    void OnDisable()
    {
        mirrorDatas.Values.ToList().ForEach(data =>
        {
            DestroyImmediate(data.Item1);
            DestroyImmediate(data.Item2);
        });
    }


    private void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        // set camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    // On-demand create any objects we need
    private (Camera, RenderTexture) GetMirrorData(Camera cam)
    {
        mirrorDatas.TryGetValue(cam, out var data);
        var (refCam, refTex) = data;

        // Reflection render texture
        if (refTex == null || refTex.width != m_TextureSize || refTex.height != m_TextureSize)
        {
            if (refTex != null)
                DestroyImmediate(refTex);
            refTex = new RenderTexture(m_TextureSize, m_TextureSize, 16);
            refTex.name = "__MirrorReflection" + GetInstanceID();
            refTex.isPowerOfTwo = true;
            refTex.hideFlags = HideFlags.DontSave;
        }

        // Camera for reflection
        if (refCam == null) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        {
            GameObject go = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + cam.GetInstanceID(), typeof(Camera), typeof(Skybox));
            refCam = go.GetComponent<Camera>();
            refCam.enabled = false;
            refCam.transform.position = transform.position;
            refCam.transform.rotation = transform.rotation;
            //reflectionCamera.gameObject.AddComponent("FlareLayer");
            go.hideFlags = HideFlags.HideAndDontSave;
        }

        data = (refCam, refTex);
        mirrorDatas[cam] = data;

        return data;
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = -m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}

/*
using UnityEngine.XR;
 
 
// This is in fact just the Water script from Pro Standard Assets,
// just with refraction stuff removed.
 
[ExecuteInEditMode] // Make mirror live-update even when not in play mode
public class MirrorReflection : MonoBehaviour
{
    public bool m_DisablePixelLights = true;
    public int m_TextureSize = 256;
    public float m_ClipPlaneOffset = 0.07f;
    public int m_framesNeededToUpdate = 0;
 
    public LayerMask m_ReflectLayers = -1;
 
    private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();
 
    private RenderTexture m_ReflectionTextureLeft = null;
    private RenderTexture m_ReflectionTextureRight = null;
    private int m_OldReflectionTextureSize = 0;
 
    private int m_frameCounter = 0;
 
    private static bool s_InsideRendering = false;
 
    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
    {
        if (m_frameCounter > 0)
        {
            m_frameCounter--;
            return;
        }
 
        var rend = GetComponent<Renderer>();
        if (!enabled || !rend || !rend.sharedMaterial || !rend.enabled)
            return;
 
        Camera cam = Camera.current;
        if (!cam)
            return;
 
        // Safeguard from recursive reflections.  
        if (s_InsideRendering)
            return;
        s_InsideRendering = true;
 
        m_frameCounter = m_framesNeededToUpdate;
 
        RenderCamera(cam, rend, Camera.StereoscopicEye.Left, ref m_ReflectionTextureLeft);
        if (cam.stereoEnabled)
            RenderCamera(cam, rend, Camera.StereoscopicEye.Right, ref m_ReflectionTextureRight);
    }
 
    private void RenderCamera(Camera cam, Renderer rend, Camera.StereoscopicEye eye, ref RenderTexture reflectionTexture)
    {
        Camera reflectionCamera;
        CreateMirrorObjects(cam, eye, out reflectionCamera, ref reflectionTexture);
 
        // find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;
 
        // Optionally disable pixel lights for reflection
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;
 
        CopyCameraProperties(cam, reflectionCamera);
 
        // Render reflection
        // Reflect camera around reflection plane
        float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
 
        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);
 
        Vector3 oldEyePos;
        Matrix4x4 worldToCameraMatrix;
        if (cam.stereoEnabled)
        {
            worldToCameraMatrix = cam.GetStereoViewMatrix(eye) * reflection;
            Vector3 eyeOffset;
            List<XRNodeState> ns = new List<XRNodeState>();
            InputTracking.GetNodeStates(ns);
            var left = ns.FirstOrDefault(x => x.nodeType == XRNode.LeftEye);
            var right = ns.FirstOrDefault(x => x.nodeType == XRNode.RightEye);
            left.TryGetPosition(out Vector3 leftOffset);
            right.TryGetPosition(out Vector3 rightOffset); // ME
            if (eye == Camera.StereoscopicEye.Left)
                eyeOffset = leftOffset;
            else
                eyeOffset = rightOffset;
            eyeOffset.z = 0.0f;
            oldEyePos = cam.transform.position + cam.transform.TransformVector(eyeOffset);
        }
        else
        {
            worldToCameraMatrix = cam.worldToCameraMatrix * reflection;
            oldEyePos = cam.transform.position;
        }
 
        Vector3 newEyePos = reflection.MultiplyPoint(oldEyePos);
        reflectionCamera.transform.position = newEyePos;
 
        reflectionCamera.worldToCameraMatrix = worldToCameraMatrix;
 
        // Setup oblique projection matrix so that near plane is our reflection
        // plane. This way we clip everything below/above it for free.
        Vector4 clipPlane = CameraSpacePlane(worldToCameraMatrix, pos, normal, 1.0f);
 
        Matrix4x4 projectionMatrix;
 
 
 
        //if (cam.stereoEnabled) projectionMatrix = HMDMatrix4x4ToMatrix4x4(cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left));
        //else
        //if (cam.stereoEnabled)
        //    projectionMatrix = HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix((Valve.VR.EVREye)eye, cam.nearClipPlane, cam.farClipPlane));
        //else
        if (cam.stereoEnabled)
            projectionMatrix = cam.GetStereoProjectionMatrix(eye);
        else
            projectionMatrix = cam.projectionMatrix;
        //projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);
        MakeProjectionMatrixOblique(ref projectionMatrix, clipPlane);
 
        reflectionCamera.projectionMatrix = projectionMatrix;
        reflectionCamera.cullingMask = m_ReflectLayers.value;
        reflectionCamera.targetTexture = reflectionTexture;
        GL.invertCulling = true;
        //Vector3 euler = cam.transform.eulerAngles;
        //reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
        reflectionCamera.transform.rotation = cam.transform.rotation;
        reflectionCamera.Render();
        //reflectionCamera.transform.position = oldEyePos;
        GL.invertCulling = false;
        Material[] materials = rend.sharedMaterials;
        string property = "_ReflectionTex" + eye.ToString();
        foreach (Material mat in materials)
        {
            if (mat.HasProperty(property))
                mat.SetTexture(property, reflectionTexture);
        }
 
        // Restore pixel light count
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;
 
        s_InsideRendering = false;
    }
 
 
    // Cleanup all the objects we possibly have created
    void OnDisable()
    {
        if (m_ReflectionTextureLeft)
        {
            DestroyImmediate(m_ReflectionTextureLeft);
            m_ReflectionTextureLeft = null;
        }
        if (m_ReflectionTextureRight)
        {
            DestroyImmediate(m_ReflectionTextureRight);
            m_ReflectionTextureRight = null;
        }
        foreach (var kvp in m_ReflectionCameras)
            DestroyImmediate(((Camera)kvp.Value).gameObject);
        m_ReflectionCameras.Clear();
    }
 
 
    private void CopyCameraProperties(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        // set camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }
 
    // On-demand create any objects we need
    private void CreateMirrorObjects(Camera currentCamera, Camera.StereoscopicEye eye, out Camera reflectionCamera, ref RenderTexture reflectionTexture)
    {
        reflectionCamera = null;
 
 
        // Reflection render texture
        if (!reflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
        {
            if (reflectionTexture)
                DestroyImmediate(reflectionTexture);
            reflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
            reflectionTexture.name = "__MirrorReflection" + eye.ToString() + GetInstanceID();
            reflectionTexture.isPowerOfTwo = true;
            reflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = m_TextureSize;
        }
 
        // Camera for reflection
        if (!m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera)) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        {
            GameObject go = new GameObject("Mirror Reflection Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
            reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent<FlareLayer>();
            go.hideFlags = HideFlags.DontSave;
            m_ReflectionCameras.Add(currentCamera, reflectionCamera);
        }
    }
 
    // Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }
 
    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Vector3 cpos = worldToCameraMatrix.MultiplyPoint(offsetPos);
        Vector3 cnormal = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
 
    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);
 
        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);
 
        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);
 
        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
 
    // taken from http://www.terathon.com/code/oblique.html
    private static void MakeProjectionMatrixOblique(ref Matrix4x4 matrix, Vector4 clipPlane)
    {
        Vector4 q;
 
        // Calculate the clip-space corner point opposite the clipping plane
        // as (sgn(clipPlane.x), sgn(clipPlane.y), 1, 1) and
        // transform it into camera space by multiplying it
        // by the inverse of the projection matrix
 
        q.x = (sgn(clipPlane.x) + matrix[8]) / matrix[0];
        q.y = (sgn(clipPlane.y) + matrix[9]) / matrix[5];
        q.z = -1.0F;
        q.w = (1.0F + matrix[10]) / matrix[14];
 
        // Calculate the scaled plane vector
        Vector4 c = clipPlane * (2.0F / Vector3.Dot(clipPlane, q));
 
        // Replace the third row of the projection matrix
        matrix[2] = c.x;
        matrix[6] = c.y;
        matrix[10] = c.z + 1.0F;
        matrix[14] = c.w;
    }
 
}
*/ 