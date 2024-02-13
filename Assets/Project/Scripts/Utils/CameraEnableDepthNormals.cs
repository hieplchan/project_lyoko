using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartledSeal
{
    public class CameraEnableDepthNormals : MonoBehaviour
    {
        void Start()
        {
            var cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
        }
    }
}
