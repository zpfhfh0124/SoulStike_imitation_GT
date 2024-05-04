using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public int atk;
}

public class Weapon : MonoBehaviour
{
    [Header("물리 요소")]
    public CapsuleCollider _collider;
    public TrailRenderer _trailFX;

    private void Awake()
    {
        SetActiveColliderTrailFX(false);
    }

    public void UseWeapon()
    {
        StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        SetActiveColliderTrailFX(true);

        yield return new WaitForSeconds(1f);
        SetActiveColliderTrailFX(false);
    }

    protected void SetActiveColliderTrailFX(bool isOn)
    {
        _collider.enabled = isOn;
        _trailFX.enabled = isOn;
    }
}
