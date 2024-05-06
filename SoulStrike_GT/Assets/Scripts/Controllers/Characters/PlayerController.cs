using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gamekit3D;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

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
        public float speed;
        public float skill_cooltime;
    }
    
    /// <summary>
    /// 플레이어 컨트롤러 관련 (애니메이션, 이동, 회전)
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("플레이어 정보")]
        private PlayerData _playerData = new PlayerData();
        public PlayerData PlayerData { get { return _playerData; } }
        int _maxHP;
        int _maxSP;
        
        [Header("Movement")]
        private float _hAxis;
        private float _vAxis;
        private Vector3 _moveVec;

        [Header("UI")] 
        public VariableJoystick _joystick;
        UIFollow3D _uiFollower;
        [SerializeField] ObjectUI _objectUI;
        [SerializeField] Button _btnAuto;
        [SerializeField] Image _imgBtnAutoEnable;

        [Header("Animation")] 
        public Animator _animator;
        private PlayerState _playerState = PlayerState.IDLE;
        public readonly float MOVE_SPEED_RUN_PARAM = 0.3f;
        public readonly float MOVE_SPEED_WALK_PARAM = 0.05f;
        public readonly string ANIM_PARAM_MOVESPEED = "MoveSpeed";
        public readonly string ANIM_PARAM_ATTACK = "Attack";

        [Header("물리 요소")]
        [SerializeField] Weapon _weapon;
        [SerializeField] private GameObject _objPlayerCollider;

        [Header("사운드")]
        public RandomAudioPlayer idleAudioPlayer;
        public RandomAudioPlayer attackAudioPlayer;
        public RandomAudioPlayer skillAudioPlayer;
        public RandomAudioPlayer damageAudioPlayer;
        public RandomAudioPlayer footstepAudioPlayer;
        public RandomAudioPlayer dieAudio;

        bool _isAuto = false;
        public bool IsAuto { get { return _isAuto; } }

        void Awake()
        {
            _InitAnim();
            _GetJsonPlayerData();
            _objectUI.InitHp(_playerData.hp);
            _objectUI.InitSp(_playerData.sp);
            _maxHP = _playerData.hp;
            _maxSP = _playerData.sp;
        }

        private void Start()
        {
            _btnAuto.onClick.AddListener(OnAuto);

            _weapon.AddAtk(_playerData.atk);
        }

        void Update()
        {
            if (!_isAuto)
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
                float speed = _playerData.speed * distNormal;
                _moveVec = moveVec.normalized;
                transform.position += _moveVec * speed * Time.deltaTime;

                // 이동 관련 애니메이션 세팅
                SetPlayerAnimeMoveSpeed(distNormal);
            }
        }

        private void FixedUpdate()
        {
            // 플레이어 HP, SP 자연 회복
            _AddHp(1);
            _AddSp(2);
        }

        void OnAuto()
        {
            _isAuto = !_isAuto;
            _imgBtnAutoEnable.gameObject.SetActive(_isAuto);
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
        public void SetPlayerAnimeMoveSpeed(float dist)
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
                state = _isAuto ? PlayerState.ATTACK : PlayerState.IDLE;
            }
            
            SetPlayerAnimState(state, dist);
        }

        public void SetPlayerAnimState(PlayerState state, float speed = 0.0f)
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
                    if (_isAuto && _playerData.skill_cooltime <= 0)
                    {
                        SetPlayerAnimState(PlayerState.SKILL);
                    }
                    else
                    {
                        _animator.SetTrigger(ANIM_PARAM_ATTACK);
                    }
                    break;
                case PlayerState.SKILL :
                    if (_playerData.skill_cooltime <= 0)
                    {
                        _playerData.skill_cooltime = 10f;
                        StartCoroutine(OnSkill(_playerData.skill_cooltime));
                    }
                    break;
                case PlayerState.HIT :
                    
                    _animator.SetTrigger("Hit");
                    
                    break;
            }

            _playerState = state;
        }

        /// -------------------------------------------------------------

        /// <summary>
        /// 플레이어 공격 및 피격, 회복
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.transform.tag == "Enemy")
            {
                transform.Rotate(collision.transform.forward);
                var enemy = collision.transform.GetComponentInParent<EnemyController>();
                GetDamaged(enemy.EnemyData.atk);
            }
        }

        public void OnAttack()
        {
            SetPlayerAnimState(PlayerState.ATTACK);
            _weapon.UseWeapon();
        }

        public IEnumerator OnSkill(float cooltime)
        {
            int randSkill = UnityEngine.Random.Range(1, 3);
            StringBuilder skillTrigger = new StringBuilder();
            skillTrigger.Append("Skill");
            skillTrigger.Append(randSkill.ToString());
            _animator.SetTrigger(skillTrigger.ToString());

            _weapon.SkillWeapon(randSkill - 1);
            yield return new WaitForSeconds(cooltime);
            _playerData.skill_cooltime = 0f;
        }

        public void OnHit()
        {
            SetPlayerAnimState(PlayerState.HIT);
        }

        void GetDamaged(int damageValue)
        {
            OnHit();
            damageValue -= (int)Math.Round(_playerData.def * 0.2f);
            int addValue = UnityEngine.Random.Range(-5, 5);
            damageValue += addValue;
            Debug.Log($"플레이어 <- 몬스터 피격 최종 데미지 : {damageValue}");
            _AddHp(damageValue * -1);
        }

        void _AddHp(int value)
        {
            if (value + _playerData.hp >= _maxHP) return;
                
            _playerData.hp += value;
            if (_playerData.hp >= _maxHP) _playerData.hp = _maxHP;

            _objectUI.SetCurHp(_playerData.hp);
            Debug.Log($"현재 플레이어 HP : {_playerData.hp}");
        }

        void _AddSp(int value)
        {
            if (value + _playerData.sp >= _maxSP) return;

            _playerData.sp += value;
            if (_playerData.sp >= _maxSP) _playerData.sp = _maxSP;

            _objectUI.SetCurSp(_playerData.sp);
        }

        /// <summary>
        /// 애니메이션 이벤트
        /// </summary>
        public void IdleAudio()
        {
            idleAudioPlayer.PlayRandomClip();
        }

        public void StepAudio()
        {
            footstepAudioPlayer.PlayRandomClip();
        }

        public void AtackAudio()
        {
            attackAudioPlayer.PlayRandomClip();
        }

        public void SkillAudio()
        {
            skillAudioPlayer.PlayRandomClip();
        }

        public void HitAudio()
        {
            damageAudioPlayer.PlayRandomClip();
        }
    }
}