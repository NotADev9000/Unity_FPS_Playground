using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponsManagement : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;
    [SerializeField] GameObject impactParent;

    void Awake()
    {
        currentWeapon.impactParent = impactParent;
    }

    void Update()
    {
        GetFireInput();
    }

    void GetFireInput()
    {
        if (HasFired())
        {
            currentWeapon.TryShoot(); 
        }
    }

    bool HasFired()
    {
        switch(currentWeapon.ShootType)
        {
            case WeaponShootType.Manual:
                return Input.GetButtonDown("Fire1");
            case WeaponShootType.Automatic:
                return Input.GetButton("Fire1");
            default:
                return false;
        }
    }
}
