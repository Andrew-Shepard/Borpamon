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
    
    public bool TakeDamage(Move move, Borpamon attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;

        float d = a * move.Base.Power * ((float)attacker.Attack / Defense);
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}
