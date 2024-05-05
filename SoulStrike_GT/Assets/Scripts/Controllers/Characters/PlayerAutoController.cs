using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GT
{
    /// <summary>
    /// Player의 자동 조작을 위한 클래스.
    /// </summary>
    public class PlayerAutoController : MonoBehaviour
    {
        PlayerController _player;

        [Header("타겟")] 
        private EnemyController _targetEnemy;

        [Header("자동이동")]
        NavMeshAgent _nma;

        private void Update()
        {
            
        }

        void _TrackingNearestMonster()
        {
            var nearestMonster = SpawnManager.Instance.GetNearestMonster(transform.position);

        }
    }
}
