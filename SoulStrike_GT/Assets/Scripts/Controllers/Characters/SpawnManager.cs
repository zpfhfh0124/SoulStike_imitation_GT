using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GT
{
    /// <summary>
    /// �÷��̾�, ���� ���� ����
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("����")]
        [SerializeField] GameObject _objMonster;
        NavMeshAgent _nma;
        const int MAX_MONSTER_COUNT = 30;
        int _monsterCount = 0;

        [Header("���� ����")]
        [SerializeField] GameObject _objPlane;
        NavMeshSurface _navMesh;
        Vector3 _navMeshSize;
        float _spawnTime = 10.0f;

        void Awake()
        {
            _nma = _objMonster.GetComponent<NavMeshAgent>();
            _navMesh = _objPlane.GetComponent<NavMeshSurface>();
            _navMeshSize = _navMesh.size;
        }

        void Update()
        {
            while(_monsterCount < MAX_MONSTER_COUNT)
            {
                StartCoroutine(Spawn());
            }
        }

        bool CheckMonsterSpawn()
        {
            bool isAdded = (_monsterCount < MAX_MONSTER_COUNT) ? true : false;
            return isAdded;
        }

        IEnumerator Spawn()
        {
            // ���� ��Ÿ�� ����
            yield return new WaitForSeconds(Random.Range(0, _spawnTime));
            GameObject monster = GameObject.Instantiate(_objMonster);

            // ���� ��ġ
            Vector3 randPos;
            float randX = Random.Range(0, _navMeshSize.x);
            float randZ = Random.Range(0, _navMeshSize.z);
            randPos = new Vector3(randX, 0, randZ);

            NavMeshPath path = new NavMeshPath();
            if(_nma.CalculatePath(randPos, path))
            {
                yield return null;
            }
            else
            {
                monster.transform.position = randPos;
                _monsterCount++;
            }
        }
    }
}