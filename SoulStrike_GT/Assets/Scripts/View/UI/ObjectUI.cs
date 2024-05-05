using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GT
{
    public class ObjectUI : MonoBehaviour
    {
        [SerializeField] Slider _hpSlider;
        [SerializeField] Slider _spSlider;
        [SerializeField] TextMeshProUGUI _damageTMP;

        int _hpMax;
        int _curHp;
        int _spMax;
        int _curSp;

        private void Awake()
        {
            _InitDamageTMP();
        }

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

        void _InitDamageTMP()
        {
            _damageTMP.gameObject.SetActive(false);
        }

        public void SetDamageText(int damage)
        {
            _damageTMP.gameObject.SetActive(true);
            _damageTMP.text = damage.ToString();
            StartCoroutine(_HideDamageTMP());
        }

        IEnumerator _HideDamageTMP()
        {
            yield return new WaitForSeconds(2.0f);
            _damageTMP.gameObject.SetActive(false);
        } 

        public void DestroyUI()
        {
            Destroy(gameObject);
        }
    }
}