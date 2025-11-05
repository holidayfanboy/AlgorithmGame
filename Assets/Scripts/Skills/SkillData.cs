using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public float cooldown;
    public int power;

    public List<GameObject> Tag = new List<GameObject>();
}

abstract class SkillEffect
{
    public abstract void Execute(List<Card> target);
}
