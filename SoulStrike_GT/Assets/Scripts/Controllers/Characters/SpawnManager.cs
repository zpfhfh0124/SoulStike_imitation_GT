using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
        float _spawnTime = 0;
        private const float MAX_MONSTER_COOLTIME = 10.0f; 

        void Awake()
        {
            _nma = _objMonster.GetComponent<NavMeshAgent>();
            _navMesh = _objPlane.GetComponent<NavMeshSurface>();
            _navMeshSize = _navMesh.size;
        }

        private void Start()
        {
            
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
            GameObject monster = GameObject.Instantiate(_objMonster);
            monster.SetActive(true);

            // ���� ��ġ
            Vector3 randPos;
            float randX = Random.Range(0, _navMeshSize.x);
            float randZ = Random.Range(0, _navMeshSize.z);
            randPos = new Vector3(randX, 0, randZ);

            NavMeshPath path = new NavMeshPath();
            if(!_nma.CalculatePath(randPos, path))
            {
                monster.transform.position = randPos;
                _monsterCount++;
            }
        }
    }
}