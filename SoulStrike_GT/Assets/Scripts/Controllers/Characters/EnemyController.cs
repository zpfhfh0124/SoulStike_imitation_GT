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
        SPITTER,
        MAX
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
        [SerializeField] private BoxCollider _atkCollider;
        
        [Header("플레이어 탐색")] 
        [SerializeField] private PlayerController _targetPlayer;
        private float _playerDist = 0; // 플레이어와의 거리
        private float _speed = 2; // 추적 속도

        [Header("상태 및 정보 - EnemyType을 정확히 지정할 것")] 
        [SerializeField] private EnemyType _enemyType;
        public EnemyType EnemyType { get { return _enemyType; } } 
        private EnemyState _state = EnemyState.IDLE;
        public EnemyState EnemyState { get { return _state; } }
        private EnemyData[] _enemyDatas; // Chomper, Grenadier, Spitter 순으로 저장할 것
        private EnemyData _enemyData;
        public EnemyData EnemyData { get { return _enemyData; } }

        [Header("AI")]
        private NavMeshAgent _navMeshAgent;

        [Header("Animation")] 
        private EnemyAnimManager _animManager;

        [SerializeField] UIFollow3D _uiFollower;
        [SerializeField] ObjectUI _objectUI;

        void _Init()
        {
            _rigid = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animManager = GetComponent<EnemyAnimManager>();
            _uiFollower.SetUITarget(transform);
            SetTarget();
            _SetAttackCollider(false);
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
            _objectUI.InitHp(_enemyData.hp);
        }

        private void Start()
        {
            _Init();
        }

        private void Update()
        {
            _TrackingTargetPlayer();

            if(_enemyData.hp <= 0)
            {
                // Die
            }
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

        /// <summary>
        /// 애니메이션
        /// </summary>
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
                        OnAttack();
                    }
                    else return;
                    break;
                case EnemyState.HIT:
                    _animManager.TriggerHit();
                    break;
            }

            _state = state;
        }

        /// <summary>
        /// 공격, 피격 처리
        /// </summary>
        /// <param name="collision"></param>
        void _SetAttackCollider(bool isOn)
        {
            _atkCollider.enabled = isOn;
        }

        IEnumerator OnAttack()
        {
            yield return new WaitForSeconds(0.1f);
            _SetAttackCollider(true);

            yield return new WaitForSeconds(0.1f);
            _SetAttackCollider(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.tag == "Weapon")
            {
                _SetStateAnim(EnemyState.HIT);
                var weapon = collision.transform.GetComponent<Weapon>();
                GetDamaged(weapon.WeaponData.atk);
                Debug.Log($"몬스터 피격");
            }
        }

        void GetDamaged(int damageValue)
        {
            damageValue -= (int)Math.Round(_enemyData.def * 0.2f);
            int addValue = UnityEngine.Random.Range(-5, 5);
            damageValue += addValue;
            Debug.Log($"플레이어 -> 몬스터 공격 최종 데미지 : {damageValue}");
            AddHp(damageValue * -1);
        }

        void AddHp(int value)
        {
            _enemyData.hp += value;
            _objectUI.SetCurHp(_enemyData.hp);
        }

        /// <summary>
        /// FX 설정
        /// </summary>
        void FxDeath()
        {

        }
    }
}