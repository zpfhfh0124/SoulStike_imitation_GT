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

        public void InitHp(int maxValue)
        {
            SetMaxHp(maxValue);
            SetCurHp(maxValue);
            Debug.Log($"초기 HP 설정 : {maxValue}");
        }

        public void InitSp(int maxValue)
        {
            SetMaxSp(maxValue);
            SetCurSp(maxValue);
        }

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
            _hpSlider.value = ((float)_curHp / (float)_hpMax);
        }

        public void SetCurSp(int value)
        {
            _curSp = value;
            _spSlider.value = ((float)_curSp / (float)_spMax);
        }
    }
}