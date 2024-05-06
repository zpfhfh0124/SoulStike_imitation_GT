using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GT
{
    [Serializable]
    public class WeaponData
    {
        public int atk;
    }

    public class Weapon : MonoBehaviour
    {
        [Header("물리 요소")]
        public CapsuleCollider _collider;
        WeaponData _weaponData;
        public WeaponData WeaponData { get { return _weaponData; } }

        [Header("이펙트")]
        public TrailRenderer _trailFX;
        public ParticleSystem[] _skillFXs;

        Action _comboAddCallback;
        public Action ComboAddCallback { get { return _comboAddCallback; } }

        private void Awake()
        {
            SetActiveColliderTrailFX(false);
            GetWeaponData();
        }

        void GetWeaponData()
        {
            var json_text = File.ReadAllText(JsonDataManager.Instance.FILEPATH_PLAYERDATA);
            _weaponData = JsonConvert.DeserializeObject<WeaponData>(json_text);
            Debug.Log($"PlayerController - 읽어들인 JsonPlayerData -> ATK : {_weaponData.atk}");
        }

        public void AddAtk(int value)
        {
            _weaponData.atk += value;
        }

        public void UseWeapon(int skillIdx = -1)
        {
            if (skillIdx >= 0)
            {
                StartCoroutine(SkillWeapon(skillIdx));
            }
            else
            {
                StartCoroutine(Swing());
            }
        }

        IEnumerator Swing()
        {
            yield return new WaitForSeconds(0.1f);
            SetActiveColliderTrailFX(true);

            yield return new WaitForSeconds(0.5f);
            SetActiveColliderTrailFX(false);
        }

        public IEnumerator SkillWeapon(int skillIdx)
        {
            SetActiveSkillFX(skillIdx, true);

            yield return new WaitForSeconds(3.0f);
            SetActiveSkillFX(skillIdx, false);
        }

        protected void SetActiveColliderTrailFX(bool isOn)
        {
            _collider.enabled = isOn;
            _trailFX.enabled = isOn;
        }

        public void SetActiveSkillFX(int idx, bool isOn)
        {
            if (isOn) _skillFXs[idx].Play(true);
            else _skillFXs[idx].Stop();
        }

        public void SetComboAddCallback(Action callback)
        {
            _comboAddCallback = callback;
        }
    }
}