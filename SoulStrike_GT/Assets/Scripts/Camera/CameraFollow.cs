using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    private Vector3 _offsetPos;
    private Vector3 _offsetRotate;
    
    void Start()
    {
        _SetQuaterViewOffset();
    }

    private void Update()
    {
        _UpdateCamTransform();
    }

    void _SetQuaterViewOffset()
    {
        _offsetPos = new Vector3(0, 10, -5);
        _offsetRotate = new Vector3(60, 0, 0);
    }

    void _UpdateCamTransform()
    {
        transform.position = _targetTransform.position + _offsetPos;
        transform.rotation = Quaternion.Euler(_offsetRotate);
    }
}
