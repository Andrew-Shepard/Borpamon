using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] BorpamonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    
    public string Name
    {
        get { return base.name; }
    }
    public string Description
    {
        get { return description; }
    }
    public BorpamonType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Pp
    {
        get { return pp; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }

}
[System.Serializable]
public enum MoveCategory
{
    Physical, Special, Status
}
[System.Serializable]
public enum MoveTarget
{
    Foe, Self
}
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}