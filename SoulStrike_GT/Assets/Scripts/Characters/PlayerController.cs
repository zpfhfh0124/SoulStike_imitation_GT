using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _hAxis;
    private float _vAxis;
    private Vector3 _moveVec;

    public float _speed = 5;
    
    void Start()
    {
        
    }

    void Update()
    {
        // PC 환경 키보드 입력
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        _moveVec = new Vector3(_hAxis, 0, _vAxis).normalized;

        // 조이패드 입력




        transform.position += _moveVec * _speed * Time.deltaTime;
    }
}
