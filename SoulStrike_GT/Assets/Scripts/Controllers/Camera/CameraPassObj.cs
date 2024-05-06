using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    public class CameraPassObj : MonoBehaviour
    {
        [SerializeField] Transform _target;

        private void Awake()
        {

        }

        void LateUpdate()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Mathf.Infinity,
                                1 << LayerMask.NameToLayer("Props"));

            for (int i = 0; i < hits.Length; i++)
            {
                TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

                for (int j = 0; j < obj.Length; j++)
                {
                    obj[j].BecomeTransparent();
                }
            }
        }
    }
}