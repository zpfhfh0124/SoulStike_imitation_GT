using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    private Vector3 offset;
    
    void Start()
    {
        _SetQuaterViewOffset();
    }

    private void Update()
    {
        transform.position = _targetTransform.position + offset;
    }

    void _SetQuaterViewOffset()
    {
        offset = new Vector3(0, 0, 5);
    }
}
