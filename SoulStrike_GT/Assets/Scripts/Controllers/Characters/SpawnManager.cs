using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GT
{
    /// <summary>
    /// 플레이어, 몬스터 스폰 관련
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("몬스터")]
        [SerializeField] GameObject _objMonster;
        NavMeshAgent _nma;
        const int MAX_MONSTER_COUNT = 30;
        int _monsterCount = 0;

        [Header("스폰 관련")]
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
            // 스폰 쿨타임 랜덤
            yield return new WaitForSeconds(Random.Range(0, _spawnTime));
            GameObject monster = GameObject.Instantiate(_objMonster);

            // 스폰 위치
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