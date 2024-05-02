using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace GT
{
    public enum EnemyState
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
        HIT,
    }

    public class EnemyController : MonoBehaviour
    {
        [Header("물리 요소")] 
        private Rigidbody _rigid;
        private BoxCollider _collider;
        
        [Header("플레이어 탐색")] 
        [SerializeField] private PlayerController _targetPlayer;
        private float _playerDist = 0; // 플레이어와의 거리
        private float _speed = 2; // 추적 속도
        private const float PURSUITED_DISTANCE = 2;
        private const float NEAR_DISTANCE = 5;
        private const float FAR_DISTANCE = 10;
        private const float SLOW_SPEED = 2;
        private const float FAST_SPEED = 5;

        [Header("상태")] 
        private EnemyState _state = EnemyState.IDLE;
        private EnemyInfo _enemyInfo = new EnemyInfo();
        
        [Header("AI")]
        private NavMeshAgent _navMeshAgent;

        [Header("Animation")] 
        private EnemyChomperAnimManager _animManager;

        void _Init()
        {
            _rigid = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animManager = GetComponent<EnemyChomperAnimManager>();
        }

        private void Start()
        {
            _Init();
        }

        private void Update()
        {
            _TrackingTargetPlayer();
        }

        private void FixedUpdate()
        {
            _FixedVelocity();
        }

        /// <summary>
        /// 플레이어 추적
        /// </summary>
        void _TrackingTargetPlayer()
        {
            _CulurateDistance(_targetPlayer.transform.position);

            if (_playerDist < PURSUITED_DISTANCE)
            {
                _speed = FAST_SPEED;
                _navMeshAgent.isStopped = false;
                _SetStateAnim(EnemyState.ATTACK);
            }
            else if (_playerDist < NEAR_DISTANCE)
            {
                _speed = FAST_SPEED;
                _SetStateAnim(EnemyState.RUN);
                _navMeshAgent.isStopped = false;
                _SetNavMeshPursuit();
            }
            else if (_playerDist < FAR_DISTANCE)
            {
                _speed = SLOW_SPEED;
                _SetStateAnim(EnemyState.WALK);
                _navMeshAgent.isStopped = false;
                _SetNavMeshPursuit();
            }
            else
            {
                // 추적 안함
                _navMeshAgent.isStopped = true;
                _animManager.StopPursuit();
                _SetStateAnim(EnemyState.IDLE);
            }

        }

        void _CulurateDistance(Vector3 targetPos)
        {
            _playerDist = (targetPos - transform.position).magnitude;
        }

        void _FixedVelocity()
        {
            _rigid.velocity = Vector3.zero;
            _rigid.angularVelocity = Vector3.zero;
        }

        void _SetNavMeshPursuit()
        {
            _navMeshAgent.speed = _speed;
            _navMeshAgent.SetDestination(_targetPlayer.transform.position);
        }

        void _SetStateAnim(EnemyState state)
        {
            if ( _state == state || _state == EnemyState.HIT ) return;
            
            switch (state)
            {
                case EnemyState.IDLE:
                    if (_state != EnemyState.RUN)
                    {
                        _animManager.SetNearBase(true);
                        _animManager.StopPursuit();
                    }
                    else return;
                    break;
                case EnemyState.WALK:
                    if (_state == EnemyState.IDLE)
                    {
                        _animManager.SetNearBase(false);
                        _animManager.StopPursuit();
                    }
                    else if (_state == EnemyState.RUN)
                    {
                        _animManager.StopPursuit();
                    }
                    else return;
                    break;
                case EnemyState.RUN:
                    if (_state != EnemyState.IDLE)
                    {
                        _animManager.StartPursuit();
                    }
                    else return;
                    break;
                case EnemyState.ATTACK:
                    if (_state != EnemyState.ATTACK)
                    {
                        _animManager.TriggerAttack();
                    }
                    else return;
                    break;
                case EnemyState.HIT:
                    _animManager.TriggerHit();
                    break;
            }

            _state = state;
        }
    }
}