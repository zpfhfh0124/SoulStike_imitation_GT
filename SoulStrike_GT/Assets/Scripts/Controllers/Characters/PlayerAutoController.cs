using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GT
{
    /// <summary>
    /// Player의 자동 조작을 위한 클래스.
    /// PlayerController의 기능과 겹치는게 많아서 상속받아서 사용
    /// </summary>
    public class PlayerAutoController : PlayerController
    {
        [Header("타겟")] 
        private EnemyController _targetEnemy;
        private SpawnManager _spawnManager;
        
        
        void Start()
        {
            
        }

        void Update()
        {
            
        }
    }
}
