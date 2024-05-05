using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        public Gamekit3D.RandomAudioPlayer deathAudioPlayer;
        public Gamekit3D.RandomAudioPlayer damageAudioPlayer;
        public Gamekit3D.RandomAudioPlayer footstepAudioPlayer;
        public Gamekit3D.RandomAudioPlayer throwAudioPlayer;
        public Gamekit3D.RandomAudioPlayer punchAudioPlayer;
        private void Start()
        {
            _animator = GetComponent<Animator>();
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
    }
}