using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace GT
{
    public enum EnemyType
    {
        CHOMPER = 0,
        GRENADIER,
        SPITTER
    }

    public enum EnemyState
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
        HIT,
    }

    [Serializable]
    public class EnemyData
    {
        public int hp;
        public int atk;
        public int def;
        public float pursutied_distance;
        public float near_distance;
        public float far_distance;
        public float slow_speed;
        public float fast_speed;
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
        

        [Header("상태 및 정보 - EnemyType을 정확히 지정할 것")] 
        [SerializeField] private EnemyType _enemyType;
        private EnemyState _state = EnemyState.IDLE;
        private EnemyData[] _enemyDatas; // Chomper, Grenadier, Spitter 순으로 저장할 것
        private EnemyData _enemyData;
        public EnemyData EnemyInfo { get { return _enemyData; } }
        
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

            SetTarget();
        }

        void _GetJsonEnemyData()
        {
            var json_text = File.ReadAllText(JsonDataManager.Instance.FILEPATH_ENEMYDATA);
            _enemyDatas = JsonConvert.DeserializeObject<EnemyData[]>(json_text);
        }

        void _SetEnemyData()
        {
            _enemyData = _enemyDatas[(int)_enemyType];
            Debug.Log($"EnemyController - 읽어들인 JsonEnemyData -> HP : {_enemyData.hp}, ATK : {_enemyData.atk}, DEF : {_enemyData.def}");
        }

        private void Awake()
        {
            _GetJsonEnemyData();
            _SetEnemyData();
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
        public void SetTarget()
        {
            _targetPlayer = FindObjectOfType<PlayerController>();
            if(_targetPlayer == null) Debug.LogError("Enemy가 추적할 플레이어 타겟을 할당하지 못했다. Scene에 PlayerController 오브젝트가 있는지 확인 필요!");
        }
        
        void _TrackingTargetPlayer()
        {
            _CulurateDistance(_targetPlayer.transform.position);

            if (_playerDist < _enemyData.pursutied_distance)
            {
                _speed = _enemyData.fast_speed;
                _navMeshAgent.isStopped = false;
                _SetStateAnim(EnemyState.ATTACK);
            }
            else if (_playerDist < _enemyData.near_distance)
            {
                _speed = _enemyData.fast_speed;
                _SetStateAnim(EnemyState.RUN);
                _navMeshAgent.isStopped = false;
                _SetNavMeshPursuit();
            }
            else if (_playerDist < _enemyData.far_distance)
            {
                _speed = _enemyData.slow_speed;
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