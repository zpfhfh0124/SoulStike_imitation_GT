using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GT
{
    /// <summary>
    /// �÷��̾�, ���� ���� ����
    /// </summary>
    public class SpawnManager : SingletonMB<SpawnManager>
    {
        [Header("����")]
        [SerializeField] GameObject _objMonster;
        const int MAX_MONSTER_COUNT = 30;
        int _monsterCount = 0;

        [Header("���� ����")]
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

        public Transform GetNearestMonster(Vector3 basePos)
        {
            EnemyController nearestEnemy;
            float nearestDist = 0;
            int nearestIdx = 0;
            var monsters = _spawnPool.transform.GetComponentsInChildren<EnemyController>();
            
            for (int i = 0; i < monsters.Length; i++)
            {
                float dist = (basePos - monsters[i].transform.position).magnitude;

                if (i > 0)
                {
                    if (nearestDist < dist)
                    {
                        nearestDist = dist;
                        nearestIdx = i;
                    }
                }
                else nearestDist = dist;
            }

            return monsters[nearestIdx].transform;
        }

        void Update()
        {
            if (CheckMonsterSpawn() && _spawnTime <= 0) 
            {
                // ���� ��Ÿ�� ����
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

            // ���� ��ġ
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