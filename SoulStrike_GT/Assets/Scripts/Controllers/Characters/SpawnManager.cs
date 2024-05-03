using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GT
{
    /// <summary>
    /// 플레이어, 몬스터 스폰 관련
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("몬스터")]
        [SerializeField] GameObject _objMonster;
        const int MAX_MONSTER_COUNT = 30;
        int _monsterCount = 0;

        [Header("스폰 관련")]
        [SerializeField] GameObject _objPlane;
        private GameObject _spawnPool;
        NavMeshSurface _navMesh;
        Vector3 _navMeshSize;
        float _spawnTime = 0;
        private const float MAX_MONSTER_COOLTIME = 10.0f; 

        void Awake()
        {
            _navMesh = _objPlane.GetComponent<NavMeshSurface>();
            _navMeshSize = _navMesh.size;
            _spawnPool = new GameObject("SpawnningPool");
        }

        private void Start()
        {
            
        }

        void Update()
        {
            if (CheckMonsterSpawn() && _spawnTime <= 0) 
            {
                // 스폰 쿨타임 랜덤
                _spawnTime = Random.Range(0, MAX_MONSTER_COOLTIME);
                StartCoroutine(Spawn(_spawnTime));
            }
        }

        bool CheckMonsterSpawn()
        {
            bool isAdded = (_monsterCount < MAX_MONSTER_COUNT) ? true : false;
            return isAdded;
        }

        IEnumerator Spawn(float coolTime)
        {
            yield return new WaitForSeconds(coolTime);
            _spawnTime = 0;
            GameObject monster = GameObject.Instantiate(_objMonster, _spawnPool.transform);

            // 스폰 위치
            Vector3 randPos;
            float randX = Random.Range(0, _navMeshSize.x);
            float randZ = Random.Range(0, _navMeshSize.z);
            randPos = new Vector3(randX, 0, randZ);
            NavMeshAgent nma = _objMonster.GetComponent<NavMeshAgent>();
            NavMeshPath path = new NavMeshPath();
            if(!nma.CalculatePath(randPos, path))
            {
                monster.transform.position = randPos;
                _monsterCount++;
            }
            else
            {
                Destroy(monster);
            }
        }
    }
}