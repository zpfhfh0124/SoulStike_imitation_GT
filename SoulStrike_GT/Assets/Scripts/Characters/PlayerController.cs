using System.Collections;
using System.Collections.Generic;
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
        public float _speed = 0;
        private float _hAxis;
        private float _vAxis;
        private Vector3 _moveVec;
        
        [Header("Animation")] 
        public Animator _animator;
        private PlayerState _playerState = PlayerState.IDLE;
        
        void Awake()
        {
            _InitAnim();
        }

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // PC 환경 키보드 입력 (테스트용)
            _hAxis = Input.GetAxisRaw("Horizontal");
            _vAxis = Input.GetAxisRaw("Vertical");
            _moveVec = new Vector3(_hAxis, 0, _vAxis).normalized;
#endif

            // PC, 모바일 조이패드 입력 
            

            transform.position += _moveVec * _speed * Time.deltaTime;
            _SetPlayerAnimeMoveSpeed(_speed);
        }

        /// <summary>
        /// 애니메이션 관련
        /// </summary>
        void _InitAnim()
        {
            _animator = GetComponent<Animator>();
        }

        void _SetPlayerAnimeMoveSpeed(float speed)
        {
            PlayerState state;
            
            if (speed > 2.0)
            {
                state = PlayerState.RUN;
            }
            else if (speed <= 2.0 && speed >= 0.1)
            {
                state = PlayerState.WALK;
            }
            else
            {
                state = PlayerState.IDLE;
            }
            
            _SetPlayerAnimState(state, speed);
        }
        
        void _SetPlayerAnimState(PlayerState state, float speed = 0.0f)
        {
            if (state == _playerState) return; 
            
            switch (state)
            {
                case PlayerState.IDLE :
                case PlayerState.WALK :
                case PlayerState.RUN : 
                    
                    _animator.SetFloat("MoveSpeed", speed);
                    
                    break;
                case PlayerState.ATTACK : 
                    break;
                case PlayerState.HIT :
                    break;
            }

            _playerState = state;
        }
        /// -------------------------------------------------------------
        
        
    }
}