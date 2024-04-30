using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GT
{
    public enum PlayerState
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
        HIT
    }
    
    /// <summary>
    /// 플레이어 컨트롤러 관련 (애니메이션, 이동, 회전)
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        private const float SPEED_BASE = 15;
        private float _hAxis;
        private float _vAxis;
        private Vector3 _moveVec;

        [Header("Joystick")] 
        public VariableJoystick _joystick;

        [Header("Animation")] 
        public Animator _animator;
        private List<AnimatorControllerParameter> _animParams = new List<AnimatorControllerParameter>();
        private PlayerState _playerState = PlayerState.IDLE;
        private const float MOVE_SPEED_RUN_PARAM = 0.3f;
        private const float MOVE_SPEED_WALK_PARAM = 0.05f;
        private const string ANIM_PARAM_MOVESPEED = "MoveSpeed";
        private const string ANIM_PARAM_ATTACK = "Attack";

        [Header("공격/피격")] 
        [SerializeField] private GameObject _objWeapon;
        [SerializeField] private GameObject _objPlayerCollider;
        
        void Awake()
        {
            _InitAnim();
        }

        void Update()
        {
            /// PC, 모바일 조이패드 입력 
            // 플레이어의 이동과 회전을 계산
            _hAxis = _joystick.Horizontal;
            _vAxis = _joystick.Vertical;
            Vector3 moveVec = new Vector3(_hAxis, 0, _vAxis);
            
            // 캐릭터 회전 적용
            if (moveVec != Vector3.zero)
            {
                Quaternion dicQ = Quaternion.LookRotation(_moveVec);
                transform.rotation = dicQ;
            }
            
            // 스피드 계산 및 이동 적용
            float distNormal = _joystick.Direction.sqrMagnitude;
            float speed = SPEED_BASE * distNormal;
            _moveVec = moveVec.normalized;
            transform.position += _moveVec * speed * Time.deltaTime;
            
            // 이동 관련 애니메이션 세팅
            _SetPlayerAnimeMoveSpeed(distNormal);
            
            // 공격
            if (Input.GetMouseButtonUp(0))
            {
                _OnAttack();
            }
        }

        /// <summary>
        /// 애니메이션 관련
        /// </summary>
        void _InitAnim()
        {
            _animator = GetComponent<Animator>();
            _animParams = _animator.parameters.ToList();
        }

        // 조이스틱 거리를 이용해서 Run/Walk/Idle 상태의 애니메이션을 처리한다.
        void _SetPlayerAnimeMoveSpeed(float dist)
        {
            PlayerState state;
            
            if ( dist > MOVE_SPEED_RUN_PARAM )
            {
                state = PlayerState.RUN;
            }
            else if ( MOVE_SPEED_WALK_PARAM <= dist && dist <= MOVE_SPEED_RUN_PARAM )
            {
                state = PlayerState.WALK;
            }
            else
            {
                state = PlayerState.IDLE;
            }
            
            _SetPlayerAnimState(state, dist);
        }
        
        void _SetPlayerAnimState(PlayerState state, float speed = 0.0f)
        {
            if (state == _playerState) return; 
            
            switch (state)
            {
                case PlayerState.IDLE :
                case PlayerState.WALK :
                case PlayerState.RUN : 
                    
                    _animator.SetFloat(ANIM_PARAM_MOVESPEED, speed);
                    
                    break;
                case PlayerState.ATTACK : 
                    
                    _animator.SetBool(ANIM_PARAM_ATTACK, true);
                    
                    break;
                case PlayerState.HIT :
                    break;
            }

            _playerState = state;
        }

        /// -------------------------------------------------------------

        /// <summary>
        /// 플레이어 공격 및 피격 처리
        /// </summary>
        void _OnAttack()
        {
            _SetPlayerAnimState(PlayerState.ATTACK);
        }
    }
}