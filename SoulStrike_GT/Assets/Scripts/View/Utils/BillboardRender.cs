using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    /// <summary>
    /// 빌보드 처리할 캔버스에 적용
    /// </summary>
    public class BillboardRender : MonoBehaviour
    {
        Transform mainCam;

        void Start()
        {
            mainCam = Camera.main.transform;
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + mainCam.rotation * Vector3.forward,
            mainCam.rotation * Vector3.up);
        }
    }
}