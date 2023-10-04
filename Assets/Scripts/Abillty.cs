using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateManager
{
    public struct StateInfo
    {
        public int maxHp { get; private set; }
        public int attack { get; private set; }
        public int defence { get; private set; }
        public float attackSpeed { get; private set; }
        public float moveSpeed { get; private set; }

        public void SetMaxHp(int hp)
        {
            maxHp = hp;
        }
        public void SetAttack(int atk)
        {
            attack = atk;
        }
        public void SetDefence(int def)
        {
            defence = def;
        }
        public void SetAttackSpeed(float speed)
        {
            attackSpeed = speed;
        }
        public void SetMoveSpeed(float speed)
        {
            attackSpeed = speed;
        }
    }

    public static StateInfo currentInfo;
    public static StateInfo monsterInfo;

    public static void PlayerInfoAwake(int hp, int atk, int def, float atkSpeed, float moveSpeed)
    {
        currentInfo.SetMaxHp(hp);
        currentInfo.SetAttack(atk);
        currentInfo.SetDefence(def);
        currentInfo.SetAttackSpeed(atkSpeed);
        currentInfo.SetMoveSpeed(moveSpeed);
    }

    public static void MonsterInfoSet()
    {
        monsterInfo = currentInfo;
    }

    public static void HpUpdate(int hp)
    {
        int updateHp = currentInfo.maxHp + hp;

        if (updateHp < 1)
        {
            updateHp = 1;
        }

        currentInfo.SetMaxHp(updateHp);
    }

    public static void AtkUpdate(int atk)
    {
        int updateAtk = currentInfo.attack + atk;

        if (updateAtk < 1)
        {
            updateAtk = 1;
        }

        currentInfo.SetAttack(updateAtk);
    }

    public static void DefUpdate(int def)
    {
        int updateDef = currentInfo.defence + def;

        currentInfo.SetDefence(updateDef);
    }

    public static void AtkSpUpdate(float speed)
    {
        float updateSp = currentInfo.attackSpeed + speed;

        currentInfo.SetAttackSpeed(updateSp);
    }
    public static void MoveSpUpdate(float speed)
    {
        float updateSp = currentInfo.moveSpeed + speed;

        currentInfo.SetMoveSpeed(updateSp);
    }
}