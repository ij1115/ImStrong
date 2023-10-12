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
    NIdle, // 기본
    Idle, // 경계
    Attack, // 공격
    Skill_F, // 스킬 1
    Skill_S, // 스킬 2
    Impact, // 피격
    Die, // 사망
    Stun, // 행동불가
    Down, // 넘어짐
    Air, // 부양
    Knockback, // 밀림
    Evade, // 회피
}
