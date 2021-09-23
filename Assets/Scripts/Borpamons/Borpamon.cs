using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Borpamon
{
    [SerializeField] BorpamonBase _base;
    [SerializeField] int level;
    public BorpamonBase Base { get { return _base; }  }
    public int Level { get { return level; } }
    
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public void Init() 
    {
        HP = MaxHp;

        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if(Moves.Count >= 4)
                break;
            
        }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((_base.MaxHp * Level) / 100f) + 10; }
    }
    public int Attack
    {
        get { return Mathf.FloorToInt((_base.Attack * Level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense * Level) / 100f) + 5; }
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((_base.SpAttack * Level) / 100f) + 5; }
    }
    public int SpDefense
    {
        get { return Mathf.FloorToInt((_base.SpDefense * Level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * Level) / 100f) + 5; }
    }
    
    public DamageDetails TakeDamage(Move move, Borpamon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f) //Critical hits have a 6.25 chance to occur
            critical = 1.5f;


        float type = TypeChart.GetEffectiveness(move.Base.Type, this._base.Type1) 
            * TypeChart.GetEffectiveness(move.Base.Type, this._base.Type2);   

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial) ? attacker.SpDefense : attacker.Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;

        float d = a * move.Base.Power * ((float)attack / defense);
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
