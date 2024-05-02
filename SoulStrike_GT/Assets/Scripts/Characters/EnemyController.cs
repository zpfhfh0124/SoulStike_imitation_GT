using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GT
{
    public enum EnemyState
    {
        IDLE,
        WEEK_SEARCH,
        STRONG_SEARCH,
        ATTACK,
        DIE
    }

    public class EnemyController : MonoBehaviour
    {
        [Header("물리 요소")] 
        private Rigidbody _rigid;
        private BoxCollider _collider;
        
        [Header("플레이어 탐색")] 
        [SerializeField] private PlayerController _targetPlayer;
        private float _playerDist = 0; // 플레이어와의 거리

        [Header("상태")] 
        private EnemyState _state;
        private EnemyInfo _enemyInfo = new EnemyInfo();
        
        [Header("AI")]
        private NavMeshAgent _navMeshAgent;

        [Header("Animation")] 
        private Animator _animator;
        public Animator Animator
        {
            get { return _animator; }
        }
        
        void _Init()
        {
            _rigid = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
            
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _Init();
        }

        private void Update()
        {
            _TrackingTargetPlayer();
        }

        
        /// <summary>
        /// 플레이어 추적
        /// </summary>
        void _TrackingTargetPlayer()
        {
            _navMeshAgent.SetDestination(_targetPlayer.transform.position);
        }
    }
}