using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EntityClass
{
    Paladin = 0,
    Knight = 1
}

[Serializable]
public class Entity
{
    public string entityName;
    public int health = 100;
    public float speed;
    public int damage;
    public int defense;
    public int weaponDamage;
    public int armorDefense;
    public Animator animator;
    public EntityClass entityClass;
    public GameObject prefab;
    public Slider healthSlider;
    public int kills = 0;
}
