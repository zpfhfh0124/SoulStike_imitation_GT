using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    public class UIFollow3D : MonoBehaviour
    {
        Canvas _canvas_billboard;
        string _canvas_name = "Canvas_Billboard";
        [SerializeField] private const float FOLLOW_SPEED = 5.0f;
        [SerializeField] Transform _target;

        void Awake()
        {
            _canvas_billboard = transform.parent.GetComponent<Canvas>();
            if (_canvas_billboard == null || _canvas_billboard.gameObject.name != _canvas_name)
            {
                var findCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (var canvas in findCanvas)
                {
                    if (canvas.name == _canvas_name)
                    {
                        transform.SetParent(canvas.transform);
                    }
                }
            }
        }

        void Update()
        {
            // 오브젝트에 따른 UI 위치 이동
            transform.position = Camera.main.WorldToScreenPoint(_target.position + new Vector3(0, 1.5f, 0));
        }

        public void SetUITarget(Transform tr)
        {
            _target = tr;
        }
    }
}