using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public _StateMachine stateMachine = new _StateMachine();
    public Rigidbody rb;
    public Animator ani;
    public PlayerInfo info;
    public PlayerController controller;
    public PlayerWeapons weapons;

    public Slider fSkillSlider;
    public bool fSkillDelay;
    private float fSkillTimer = 0f;
    public float fSkillTimerSet = 10f;

    public Slider sSkillSlider;
    public bool sSkillDelay;
    private float sSkillTimer = 0f;
    public float sSkillTimerSet = 20f;

    public CinemachineVirtualCamera vCamera;
    public LayerMask mask;

    public GameObject[] hitRanges;


    private void Start()
    {
        InitStateMachine();

        fSkillDelay = false;
        sSkillDelay = false;

        stateMachine.ChangeState(UnitState.Idle);
    }

    private void InitStateMachine()
    {
        stateMachine.AddState(UnitState.Idle, new IdleState(this));
        stateMachine.AddState(UnitState.Evade, new EvadeState(this));
        stateMachine.AddState(UnitState.Attack, new AttackState(this));
        stateMachine.AddState(UnitState.Skill_F, new FSkillState(this));
        stateMachine.AddState(UnitState.Skill_S, new SSkillState(this));
    }

    private void Update()
    {
        stateMachine?.UpdateState();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdateState();
    }

    public void FSkillDelayOn()
    {
        fSkillDelay = true;
        StartCoroutine(FSkillDelay());
    }

    private IEnumerator FSkillDelay()
    {
        fSkillTimer = fSkillTimerSet;
        fSkillSlider.gameObject.SetActive(true);
        fSkillSlider.maxValue = fSkillTimerSet;
        fSkillSlider.minValue = 0;

        while (fSkillTimer > 0)
        {
            fSkillTimer -= Time.deltaTime;
            fSkillSlider.value = fSkillTimer;
            yield return null;
        }
        fSkillSlider.gameObject.SetActive(false);
        fSkillDelay = false;
    }


    public void SwordAttack()
    {
        SoundManager.Instance.SfxPlay("Sword_Attack");
        BoxCollider col = hitRanges[0].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[0].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                var com = obj.gameObject.GetComponent<MonsterMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die ||
                    com.unitState == UnitState.Skill_F ||
                    com.unitState == UnitState.Skill_S)
                    continue;

                com.Hit();
            }
        }
    }
    public void SwordFSkill_1()
    {
        SoundManager.Instance.SfxPlay("Sword_FSkill");

        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die ||
                    com.unitState == UnitState.Skill_F ||
                    com.unitState == UnitState.Skill_S)
                    continue;

                com.Hit();
            }
        }
    }
    public void SwordFSkill_2()
    {
        SoundManager.Instance.SfxPlay("Sword_FSkill");

        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die ||
                    com.unitState == UnitState.Skill_F ||
                    com.unitState == UnitState.Skill_S)
                    continue;

                com.Hit();
            }
        }
    }
    public void SwordFSkill_3()
    {
        SoundManager.Instance.SfxPlay("Sword_FSkill");

        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                var com = obj.gameObject.GetComponent<MonsterMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die ||
                    com.unitState == UnitState.Skill_F ||
                    com.unitState == UnitState.Skill_S)
                    continue;

                com.Stun(2f);
            }
        }
    }
    public void SwordSSkill()
    {
        SoundManager.Instance.SfxPlay("Sword_SSkill");

        BoxCollider col = hitRanges[2].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[2].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 3f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die ||
                    com.unitState == UnitState.Skill_F ||
                    com.unitState == UnitState.Skill_S)
                    continue;

                com.Stun(5f);
            }
        }
    }

    public void ChangeState(string state)
    {
        if(Enum.TryParse(typeof(UnitState),state, out var result))
        {
            stateMachine.ChangeState((UnitState)result);
        }
    }
}
