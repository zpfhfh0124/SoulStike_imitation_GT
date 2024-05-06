using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GT
{
    [Serializable]
    public class SpawnData
    {
        public int monster_max_count;
        public float monster_max_cooltime;
        public float spawn_radius;
    }

    /// <summary>
    /// 플레이어, 몬스터 스폰 관련
    /// </summary>
    public class SpawnManager : SingletonMB<SpawnManager>
    {
        [Header("몬스터")]
        [SerializeField] List<GameObject> _objAllMonsters;
        [SerializeField] GameObject _objMonster;

        [Header("스폰 관련")]
        private GameObject _spawnPool;
        Vector3 _spawnPos;
        float _spawnTime = 0;

        [Header("Data")]
        SpawnData _spawnData;

        protected override void Awake()
        {
            base.Awake();
            _spawnPool = new GameObject("SpawnningPool");
            _GetJsonSpawnData();
        }

        void ResetMonsterType()
        {
            int idx = Random.Range(0, (int)EnemyType.MAX);
            _objMonster = _objAllMonsters[idx];
            Debug.Log($"스폰될 몬스터 타입 : {(EnemyType)idx}");
        }

        void _GetJsonSpawnData()
        {
            string json_text = File.ReadAllText(JsonDataManager.Instance.FILEPATH_SPAWNDATA);
            _spawnData = JsonConvert.DeserializeObject<SpawnData>(json_text);
            Debug.Log($"SpawnManager - 읽어들인 JsonSpawnData -> monster_max_count : {_spawnData.monster_max_count}, monster_max_cooltime : {_spawnData.monster_max_cooltime}, spawn_radius : {_spawnData.spawn_radius}");
        }

        public Transform GetNearestMonster(Vector3 basePos)
        {
            float nearestDist = 0;
            int nearestIdx = 0;
            var monsters = _spawnPool.transform.GetComponentsInChildren<EnemyController>();
            
            if(monsters == null || monsters.Length == 0)
            {
                return null;
            }

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
                // 스폰 쿨타임 랜덤
                _spawnTime = Random.Range(0, _spawnData.monster_max_cooltime);
                StartCoroutine(Spawn(_spawnTime));
            }
        }

        bool CheckMonsterSpawn()
        {
            bool isAdded = (GetSpawnedMonsterCount() < _spawnData.monster_max_count) ? true : false;
            return isAdded;
        }

        public int GetSpawnedMonsterCount()
        {
            var monsters = _spawnPool.transform.GetComponentsInChildren<EnemyController>();
            return monsters.Length;
        }

        IEnumerator Spawn(float coolTime)
        {
            yield return new WaitForSeconds(coolTime);
            ResetMonsterType();

            // 스폰 위치 지정한 영역 내에서 랜덤하게
            Vector3 randPos;
            Vector3 randDir = Random.insideUnitSphere * _spawnData.spawn_radius;
            randPos = _spawnPos + randDir;
            randPos.y = 0f;

            // 몬스터 생성
            GameObject monster = GameObject.Instantiate(_objMonster, _spawnPool.transform);
            NavMeshAgent nma = monster.GetComponent<NavMeshAgent>();
            Debug.Log($"스폰 몬스터 NavMeshAgent : {nma.pathStatus}");
            monster.transform.position = randPos;

            if (nma.pathStatus != NavMeshPathStatus.PathInvalid && 
                nma.CalculatePath(randPos, nma.path))
            {
                _spawnPos = randPos;
            }
            else
            {
                monster.GetComponent<EnemyController>().UI.DestroyUI();
                Destroy(monster);
            }

            _spawnTime = 0;
        }

    }
}