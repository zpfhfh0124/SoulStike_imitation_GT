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

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            if ( _player.IsAuto )
            { 
                _TrackingNearestMonster();
            }
        }

        void Init()
        {
            _player = GetComponent<PlayerController>();
            _nma = GetComponent<NavMeshAgent>();
        }

        void _TrackingNearestMonster()
        {
            var nearestMonster = SpawnManager.Instance.GetNearestMonster(transform.position);

            if (nearestMonster != null)
            {
                // 캐릭터 회전 적용
                _targetEnemy = nearestMonster.GetComponent<EnemyController>();
                transform.LookAt(_targetEnemy.transform.position);
                // 타겟 몬스터와의 거리
                float dist = (transform.position - _targetEnemy.transform.position).magnitude;
                float distSpeed = 0f;
                PlayerState state = PlayerState.IDLE;
                if (dist > _player.MOVE_SPEED_RUN_PARAM * 10)
                {
                    state = PlayerState.RUN;
                    distSpeed = _player.MOVE_SPEED_RUN_PARAM;
                    _player.SetPlayerAnimState(state, distSpeed);
                }
                else if (_player.MOVE_SPEED_WALK_PARAM <= dist && dist <= _player.MOVE_SPEED_RUN_PARAM)
                {
                    state = PlayerState.WALK;
                    distSpeed = _player.MOVE_SPEED_WALK_PARAM;
                    _player.SetPlayerAnimState(state, distSpeed);
                }
                else
                {
                    _player.OnAttack();
                    distSpeed = _player.MOVE_SPEED_WALK_PARAM;
                }

                _nma.SetDestination(_targetEnemy.transform.position);
                _nma.speed = _player.PlayerData.speed * distSpeed;
            }
            else
            {
                _player.SetPlayerAnimState(PlayerState.IDLE);
                return;
            }
            
        }
    }
}
