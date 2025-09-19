using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBullet", menuName = "ScriptableObject/Bullet")]
public class BulletData : ScriptableObject
{
    public float damage;
    public float speed;
    public float lifetime;
    public GameObject prefabBullet;  // para el pool
}

