using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons
{
    None = -1,
    Sword,
    Axe,
    Spear,
}

public enum SceneState
{
    Title,
    Lobby,
    Dungeon,
    BossRoom,
}

public enum MonsterType
{
    Mob,
    SubBoss,
    Boss,
}

public struct StateInfo
{
    public int maxHp;
    public int attack;
    public int defence;
    public float attackSpeed;
    public float moveSpeed;

    public StateInfo(int hp, int atk, int def, float aSp, float mSp)
    {
        this.maxHp = hp;
        this.attack = atk;
        this.defence = def;
        this.attackSpeed = aSp;
        this.moveSpeed = mSp;
    }
    public StateInfo(StateInfo state)
    {
        this.maxHp = state.maxHp;
        this.attack = state.attack;
        this.defence = state.defence;
        this.attackSpeed = state.attackSpeed;
        this.moveSpeed = state.moveSpeed;
    }
    public void SetMaxHp(int hp)
    {
        this.maxHp = hp;
    }
    public void SetAttack(int attack)
    { 
        this.attack = attack;
    }
    public void SetDefence(int defence)
    {
        this.defence = defence;
    }
    public void SetAtkSpeed(float speed)
    {
        this.attackSpeed = speed;
    }
    public void SetMovSpeed(float speed)
    { 
        this.moveSpeed = speed;
    }

}