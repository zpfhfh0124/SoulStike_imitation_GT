using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Enemy Chomper의 애니메이션 관리
/// </summary>
namespace GT
{
    public class EnemyChomperAnimManager : MonoBehaviour
    {
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

        [Header("콘트롤러")]
        [SerializeField] private EnemyController _enemyController;
        [SerializeField] private PlayerController _targetPlayercontroller;
        
        public EnemyController EnemyController
        {
            get { return _enemyController; }
        }

        public PlayerController TargetPlayerController
        {
            get { return _targetPlayercontroller; }
        }
        
        public void StartPursuit()
        {
            _enemyController.Animator.SetBool(hashInPursuit, true);
        }
        
        public void StopPursuit()
        {
            _enemyController.Animator.SetBool(hashInPursuit, false);
        }
        
        public void TriggerAttack()
        {
            _enemyController.Animator.SetTrigger(hashAttack);
        }

        public void TriggerHit()
        {
            _enemyController.Animator.SetTrigger(hashHit);
        }

        public void TriggerDeath()
        {
            _enemyController.Animator.SetTrigger(hashThrown);
        }
    }
}