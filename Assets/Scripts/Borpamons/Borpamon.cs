using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borpamon
{
    public BorpamonBase Borpamon_base { get; set; }
    public int Level { get; set; }
    
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Borpamon(BorpamonBase bBase, int bLevel)
    {
        Borpamon_base = bBase;
        Level = bLevel;
        HP = MaxHp;

        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in Borpamon_base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if(Moves.Count >= 4)
                break;
            
        }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((Borpamon_base.MaxHp * Level) / 100f) + 10; }
    }
    public int Attack
    {
        get { return Mathf.FloorToInt((Borpamon_base.Attack * Level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Borpamon_base.Defense * Level) / 100f) + 5; }
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((Borpamon_base.SpAttack * Level) / 100f) + 5; }
    }
    public int SpDefense
    {
        get { return Mathf.FloorToInt((Borpamon_base.SpDefense * Level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((Borpamon_base.Speed * Level) / 100f) + 5; }
    }
    
    public DamageDetails TakeDamage(Move move, Borpamon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f) //Critical hits have a 6.25 chance to occur
            critical = 1.5f;


        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Borpamon_base.Type1) 
            * TypeChart.GetEffectiveness(move.Base.Type, this.Borpamon_base.Type2);

        Debug.Log("" + move.Base.Type.ToString() +" , "+ this.Borpamon_base.Type1.ToString()) ;
        

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };
        
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;

        float d = a * move.Base.Power * ((float)attacker.Attack / Defense);
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
