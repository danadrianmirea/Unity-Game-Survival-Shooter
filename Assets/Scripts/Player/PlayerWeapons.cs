using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Weapon
{
    public readonly GameObject[] gameObjects;
    public bool IsUnlocked
    {
        get; set;
    }

    public readonly WeaponHandler handler;

    public Weapon(GameObject[] gameObjects)
    {
        this.gameObjects = gameObjects;
        this.handler = this.gameObjects[0].GetComponent<WeaponHandler>();
        this.IsUnlocked = false;
        Disable();
    }

    public bool Enable()
    {
        if (!IsUnlocked) return false;

        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(true);
        }

        return true;
    }

    public void Disable()
    {
        for (int i = 0;i< gameObjects.Length;i++)
        {
            gameObjects[i].SetActive(false);
        }
    }
}

public class PlayerWeapons : MonoBehaviour
{
    readonly Weapon[] weapons = new Weapon[4];
    int idxWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        var gun = new GameObject[2];
        gun[0] = GameObject.Find("GunBarrelEnd");
        gun[1] = GameObject.Find("Gun");
        weapons[0] = new Weapon(gun);

        var shotgun = new GameObject[2];
        shotgun[0] = GameObject.Find("ShotGunBarrelEnd");
        shotgun[1] = GameObject.Find("ShotGun");
        weapons[1] = new Weapon(shotgun);

        var sword = new GameObject[1];
        sword[0] = GameObject.Find("Sword");
        weapons[2] = new Weapon(sword);

        var bow = new GameObject[1];
        bow[0] = GameObject.Find("Bow");
        weapons[3] = new Weapon(bow);

        UnlockWeapon(WeaponType.SimpleGun);
        UnlockWeapon(WeaponType.Sword);
        UnlockWeapon(WeaponType.ShotGun);
        UnlockWeapon(WeaponType.Bow);
        SelectWeapon(idxWeapon);

    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                idxWeapon = i;
                SelectWeapon(idxWeapon);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            IncrementIdxWeapon();
            SelectWeapon(idxWeapon);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            DecrementIdxWeapon();
            SelectWeapon(idxWeapon);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            IncrementIdxWeapon();
            SelectWeapon(idxWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            DecrementIdxWeapon();
            SelectWeapon(idxWeapon);
        }
    }

    private int GetIdxWeapon(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.SimpleGun => 0,
            WeaponType.ShotGun => 1,
            WeaponType.Sword => 2,
            WeaponType.Bow => 3,
            _ => -1,
        };
    }

    public bool LevelUp(WeaponType weaponType)
    {
        int idx = GetIdxWeapon(weaponType);
        if (idx == -1) return false;

        weapons[idx].handler.IncrementLevel();
        return true;
    }

    public bool UnlockWeapon(WeaponType weaponType)
    {
        int idx = GetIdxWeapon(weaponType);
        if (idx == -1) return false;

        weapons[idx].IsUnlocked = true;
        return true;
    }

    private void IncrementIdxWeapon()
    {
        idxWeapon++;
        idxWeapon %= weapons.Length;
    }

    private void DecrementIdxWeapon()
    {
        idxWeapon--;
        idxWeapon = (idxWeapon + weapons.Length) % weapons.Length;
    }

    private void SelectWeapon(int idx)
    {
        Debug.Log("SelectWeapon: " + idx);
        DisableAllWeapons();
        var success = EnableWeapon(idx);
        if (!success)
        {
            Debug.LogWarning("Selecting locked weapon");
        }
    }

    private bool EnableWeapon(int idx)
    {
        return weapons[idx].Enable();
    }

    private void DisableAllWeapons()
    {
        for (var i = 0; i < weapons.Length; i++)
        {
            weapons[i].Disable();
        }
    }
}
