using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GT
{
    public class ObjectUI : MonoBehaviour
    {
        [SerializeField] Slider _hpSlider;
        [SerializeField] Slider _spSlider;

        int _hpMax;
        int _curHp;
        int _spMax;
        int _curSp;

        public void SetMaxHp(int maxValue)
        {
            _hpMax = maxValue;
        }

        public void SetMaxSp(int maxValue)
        {
            _spMax = maxValue;
        }

        public void SetCurHp(int value)
        {
            _curHp = value;
            _hpSlider.value = (_curHp / _hpMax);
        }

        public void SetCurSp(int value)
        {
            _curSp = value;
            _spSlider.value = (_curSp / _spMax);
        }
    }
}