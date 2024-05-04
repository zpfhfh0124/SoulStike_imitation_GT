using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace GT
{
    public enum PlayerState
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
        SKILL,
        HIT
    }
    
    [Serializable]
    public class PlayerData
    {
        public int hp;
        public int sp;
        public int atk;
        public int def;
    }
    
    /// <summary>
    /// 플레이어 컨트롤러 관련 (애니메이션, 이동, 회전)
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("플레이어 정보")] 
        protected PlayerData _playerData = new PlayerData();
        public PlayerData PlayerInfo { get { return _playerData; } }
        
        [Header("Movement")]
        protected const float SPEED_BASE = 5;
        protected float _hAxis;
        protected float _vAxis;
        protected Vector3 _moveVec;

        [Header("Joystick")] 
        public VariableJoystick _joystick;

        [Header("Animation")] 
        public Animator _animator;
        protected PlayerState _playerState = PlayerState.IDLE;
        protected const float MOVE_SPEED_RUN_PARAM = 0.3f;
        protected const float MOVE_SPEED_WALK_PARAM = 0.05f;
        protected const string ANIM_PARAM_MOVESPEED = "MoveSpeed";
        protected const string ANIM_PARAM_ATTACK = "Attack";

        [Header("물리 요소")] 
        [SerializeField] private GameObject _objWeapon;
        [SerializeField] private GameObject _objPlayerCollider;
        
        void Awake()
        {
            _InitAnim();
            _GetJsonPlayerData();
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
            SetPlayerAnimeMoveSpeed(distNormal);
        }

        /// <summary>
        /// 플레이어 정보 관련
        /// </summary>
        void _GetJsonPlayerData()
        {
            var json_text = File.ReadAllText(JsonDataManager.Instance.FILEPATH_PLAYERDATA);
            _playerData = JsonConvert.DeserializeObject<PlayerData>(json_text);
            Debug.Log($"PlayerController - 읽어들인 JsonPlayerData -> HP : {_playerData.hp}, SP : {_playerData.sp}, ATK : {_playerData.atk}, DEF : {_playerData.def}");
        }
        
        /// <summary>
        /// 애니메이션 관련
        /// </summary>
        void _InitAnim()
        {
            _animator = GetComponent<Animator>();
        }

        // 조이스틱 거리를 이용해서 Run/Walk/Idle 상태의 애니메이션을 처리한다.
        protected void SetPlayerAnimeMoveSpeed(float dist)
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
            
            SetPlayerAnimState(state, dist);
        }
        
        protected void SetPlayerAnimState(PlayerState state, float speed = 0.0f)
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
                case PlayerState.SKILL :
                    _animator.SetBool(ANIM_PARAM_ATTACK, true);
                    int randSkill = UnityEngine.Random.Range(1, 3);
                    StringBuilder skillTrigger = new StringBuilder();
                    skillTrigger.Append("Skill");
                    skillTrigger.Append(randSkill.ToString());
                    _animator.SetTrigger(skillTrigger.ToString());
                    
                    break;
                case PlayerState.HIT :
                    
                    _animator.SetTrigger("Hit");
                    
                    break;
            }

            _playerState = state;
        }

        /// -------------------------------------------------------------

        /// <summary>
        /// 플레이어 공격 및 피격
        /// </summary>
        void _OnAttack()
        {
            SetPlayerAnimState(PlayerState.ATTACK);
        }

        public void HitDamage()
        {
            SetPlayerAnimState(PlayerState.HIT);
        }
    }
}