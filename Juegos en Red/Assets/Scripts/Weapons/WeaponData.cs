using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "ScriptableObject/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public BulletData bulletType;
    public float fireRate;
    public int magazineSize;
    public float reloadTime;
    public bool automatic;
}
