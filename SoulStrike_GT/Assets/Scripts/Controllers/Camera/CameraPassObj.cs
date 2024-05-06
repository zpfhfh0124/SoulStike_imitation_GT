using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    public class CameraPassObj : MonoBehaviour
    {
        PlayerController _player;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>();
        }

        void LateUpdate()
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Mathf.Infinity,
                                1 << LayerMask.NameToLayer("Prop"));

            for (int i = 0; i < hits.Length; i++)
            {
                TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

                for (int j = 0; j < obj.Length; j++)
                {
                    obj[j]?.BecomeTransparent();
                }
            }
        }
    }
}