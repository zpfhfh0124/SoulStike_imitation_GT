using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    public class CameraShake : MonoBehaviour
    {
        public float shakeTime = 1.0f;
        public float shakeSpeed = 2.0f;
        public float shakeAmount = 1.0f;

        private Transform cam;

        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        public void ShakeCam()
        {
            StartCoroutine(Shake());
        }

        IEnumerator Shake()
        {
            Vector3 originPos = cam.localPosition;
            float elapsedTime = 0.0f;

            while (elapsedTime < shakeTime)
            {
                Vector3 randPoint = originPos + Random.insideUnitSphere * shakeAmount;
                cam.localPosition = Vector3.Lerp(cam.localPosition, randPoint, Time.deltaTime * shakeSpeed);

                yield return null;

                elapsedTime += Time.deltaTime;
            }

            cam.localPosition = originPos;
        }
    }
}