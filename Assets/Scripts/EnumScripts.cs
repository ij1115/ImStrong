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
    Portal,
}

public enum UnitState
{
    NIdle, // �⺻
    Idle, // ���
    Attack, // ����
    Skill_F, // ��ų 1
    Skill_S, // ��ų 2
    Impact, // �ǰ�
    Die, // ���
    Stun, // �ൿ�Ұ�
    Down, // �Ѿ���
    Air, // �ξ�
    Knockback, // �и�
    Evade, // ȸ��
}
