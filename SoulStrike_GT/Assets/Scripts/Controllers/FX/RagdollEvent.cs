using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    public class RagdollEvent : MonoBehaviour
    {
        public GameObject ragdollPrefab;

        public void Replace()
        {
            GameObject ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            //사용하지 않도록 설정해야 합니다. 그렇지 않으면 계층 객체 위치/rotation 위로 복사할 때 래그돌이 매번 다음 작업을 시도합니다
            //부착된 조인트를 "수정"하여 변형/결함 인스턴스로 이어집니다
            ragdollInstance.SetActive(false);

            EnemyController baseController = GetComponent<EnemyController>();

            Transform ragdollCurrent = ragdollInstance.transform;
            Transform current = transform;
            bool first = true;

            while (current != null && ragdollCurrent != null)
            {
                if (first || ragdollCurrent.name == current.name)
                {
                    //we only match part of the hierarchy that are named the same, except for the very first, as the 2 objects will have different name (but must have the same skeleton)
                    ragdollCurrent.rotation = current.rotation;
                    ragdollCurrent.position = current.position;
                    first = false;
                }

                if (current.childCount > 0)
                {
                    // Get first child.
                    current = current.GetChild(0);
                    ragdollCurrent = ragdollCurrent.GetChild(0);
                }
                else
                {
                    while (current != null)
                    {
                        if (current.parent == null || ragdollCurrent.parent == null)
                        {
                            // No more transforms to find.
                            current = null;
                            ragdollCurrent = null;
                        }
                        else if (current.GetSiblingIndex() == current.parent.childCount - 1 ||
                                 current.GetSiblingIndex() + 1 >= ragdollCurrent.parent.childCount)
                        {
                            // Need to go up one level.
                            current = current.parent;
                            ragdollCurrent = ragdollCurrent.parent;
                        }
                        else
                        {
                            // Found next sibling for next iteration.
                            current = current.parent.GetChild(current.GetSiblingIndex() + 1);
                            ragdollCurrent = ragdollCurrent.parent.GetChild(ragdollCurrent.GetSiblingIndex() + 1);
                            break;
                        }
                    }
                }
            }


            ragdollInstance.SetActive(true);
            Destroy(gameObject);
        }
    }
}