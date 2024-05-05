using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Gamekit3D;

/// <summary>
/// Enemy Chomper의 애니메이션 관리
/// </summary>
namespace GT
{
    public class EnemyAnimManager : MonoBehaviour
    {
        private Animator _animator;
        public Animator Animator
        {
            get { return _animator; }
        }

        EnemyController _enemy;

        [Header("Enemy 요소")]
        [SerializeField] MeleeWeapon meleeWeapon;
        GameObject shield;
        float m_ShieldActivationTime = 0;
        Damageable m_Damageable;


        [Header("애니메이터 파라미터")]
        public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashVerticalDot = Animator.StringToHash("VerticalHitDot");
        public static readonly int hashHorizontalDot = Animator.StringToHash("HorizontalHitDot");
        public static readonly int hashThrown = Animator.StringToHash("Thrown");
        public static readonly int hashGrounded = Animator.StringToHash("Grounded");
        public static readonly int hashVerticalVelocity = Animator.StringToHash("VerticalVelocity");
        public static readonly int hashSpotted = Animator.StringToHash("Spotted");
        public static readonly int hashNearBase = Animator.StringToHash("NearBase");
        public static readonly int hashIdleState = Animator.StringToHash("ChomperIdle");

        [Header("Audio")]
        public RandomAudioPlayer deathAudioPlayer;
        public RandomAudioPlayer damageAudioPlayer;
        public RandomAudioPlayer footstepAudioPlayer;
        public RandomAudioPlayer throwAudioPlayer;
        public RandomAudioPlayer punchAudioPlayer;

        private void Awake()
        {
            Init();
        }
        
        void Init()
        {
            _enemy = GetComponent<EnemyController>();
            _animator = GetComponent<Animator>();
            m_Damageable = GetComponentInChildren<Damageable>();

            // Enemy Type 별로 지정
            switch (_enemy.EnemyType)
            {
                case EnemyType.CHOMPER:
                    meleeWeapon = GetComponentInChildren<MeleeWeapon>();
                    break;
                case EnemyType.GRENADIER:
                    shield = GetComponentInChildren<GrenadierShield>().gameObject;
                    meleeWeapon = GetComponentInChildren<MeleeWeapon>();
                    break;
                case EnemyType.SPITTER:
                    break;
                case EnemyType.MAX:
                default:
                    break;
            }
            
            
        }
        
        public void StartPursuit()
        {
            _animator.SetBool(hashInPursuit, true);
        }
        
        public void StopPursuit()
        {
            _animator.SetBool(hashInPursuit, false);
        }

        public void SetNearBase(bool isNearBase)
        {
            _animator.SetBool(hashNearBase, isNearBase);
        }
        
        public void TriggerAttack()
        {
            _animator.SetTrigger(hashAttack);
        }

        public void TriggerHit()
        {
            _animator.SetTrigger(hashHit);
        }

        public void TriggerDeath()
        {
            _animator.SetTrigger(hashThrown);
        }

        /// <summary>
        /// 애니메이션 이벤트
        /// </summary>
        public void PlayStep()
        {
            footstepAudioPlayer.PlayRandomClip();
        }

        public void AttackBegin()
        {
            meleeWeapon.BeginAttack(false);
        }

        public void AttackEnd()
        {
            meleeWeapon.EndAttack();
        }

        public void ActivateShield()
        {
            shield.SetActive(true);
            m_ShieldActivationTime = 3.0f;
            m_Damageable.SetColliderState(false);

            StartCoroutine(DeactivateShield(m_ShieldActivationTime));
        }

        IEnumerator DeactivateShield(float time)
        {
            yield return new WaitForSeconds(time);
            shield.SetActive(false);
            m_Damageable.SetColliderState(true);
        }
    }
}