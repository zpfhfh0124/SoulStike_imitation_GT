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
    public class SpawnManager : SingletonMB<SpawnManager>
    {
        [Header("몬스터")]
        [SerializeField] GameObject _objMonster;
        const int MAX_MONSTER_COUNT = 30;
        int _monsterCount = 0;

        [Header("스폰 관련")]
        [SerializeField] GameObject _objPlane;
        private GameObject _spawnPool;
        NavMeshSurface _navMesh;
        Vector3 _spawnPos;
        private float _spawnRadius = 15.0f;
        float _spawnTime = 0;
        private const float MAX_MONSTER_COOLTIME = 10.0f;

        protected override void Awake()
        {
            base.Awake();
            _navMesh = _objPlane.GetComponent<NavMeshSurface>();
            _spawnPool = new GameObject("SpawnningPool");
        }

        public int GetEnemyCount()
        {
            return _monsterCount;
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
            NavMeshAgent nma = monster.GetComponent<NavMeshAgent>();

            // 스폰 위치
            Vector3 randPos;
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;
            
            NavMeshPath path = new NavMeshPath();
            if(nma.CalculatePath(randPos, path))
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