using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Coroutine currentCo;
    private PlayerController controller;
    private PlayerWeapons weapons;
    private Animator ani;
    private Rigidbody rb;
    private PlayerInfo info;

    public float rotateSpeed = 180f;

    public float fightTimer = 10f;

    public CinemachineVirtualCamera vCamera;
    private Camera worldCam;

    private Vector3 moveVec;
    public UnitState unitState { get; private set; }

    private bool fSkillDelay = false;
    private float fSkillTimer = 0f;
    public float fSkillTimerSet = 5f;

    private bool sSkillDelay = false;
    private float sSkillTimer = 0f;
    public float sSkillTimerSet = 10f;

    public GameObject[] hitRanges;

    public void Setup()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<PlayerWeapons>();
        ani = GetComponent<Animator>();
        info = GetComponent<PlayerInfo>();
        worldCam = Camera.main;
        ani.runtimeAnimatorController = weapons.GetAni();
        unitState = UnitState.NIdle;
    }

    public void RunTimeSwap()
    {
        weapons.RunTimeSwap();
        ani.runtimeAnimatorController = weapons.GetAni();
    }

    private void FixedUpdate()
    {
        if (controller == null)
            return;

        if (unitState == UnitState.NIdle || unitState == UnitState.Idle)
        {
            Move();
            Rotate();

            ani.SetFloat("Move", moveVec.magnitude);
        }
    }

    private void Update()
    {
        switch (unitState)
        {
            case UnitState.NIdle:
                NIdleUpdate();
                break;

            case UnitState.Idle:
                IdleUpdate();
                break;

            case UnitState.Attack:
                AttackUpdate();
                break;

            case UnitState.Skill_F:
                SkillUpdate();
                break;

            case UnitState.Skill_S:
                SkillUpdate();
                break;

            default:
                return;
        }
    }

    private void IdleUpdate()
    {
        MoveVecSet();

        if (controller.attack)
        {
            unitState = UnitState.Attack;
            moveVec = Vector3.zero;
            ani.SetBool("Fight", true);
            ani.SetTrigger("Attack_1");
            return;
        }

        if (controller.firstSkill && !fSkillDelay)
        {
            unitState = UnitState.Skill_F;
            moveVec = Vector3.zero;
            fSkillDelay = true;
            StartCoroutine(FSkillDelayOn());
            ani.SetBool("Fight", true);
            ani.SetTrigger("Skill_F");
            return;
        }

        if (controller.secondSkill && !sSkillDelay)
        {
            unitState = UnitState.Skill_S;
            moveVec = Vector3.zero;
            sSkillDelay = true;
            StartCoroutine(SSkillDelayOn());
            ani.SetBool("Fight", true);
            ani.SetTrigger("Skill_S");
            return;
        }
    }

    private void NIdleUpdate()
    {
        MoveVecSet();

        if (controller.attack)
        {
            unitState = UnitState.Attack;
            moveVec = Vector3.zero;
            ani.SetBool("Fight", true);
            ani.SetTrigger("Attack_1");
            return;
        }

        if (controller.firstSkill && !fSkillDelay)
        {
            unitState = UnitState.Skill_F;
            moveVec = Vector3.zero;
            fSkillDelay = true;
            StartCoroutine(FSkillDelayOn());
            ani.SetBool("Fight", true);
            ani.SetTrigger("Skill_F");
            return;
        }

        if (controller.secondSkill && !sSkillDelay)
        {
            unitState = UnitState.Skill_S;
            moveVec = Vector3.zero;
            sSkillDelay = true;
            StartCoroutine(SSkillDelayOn());
            ani.SetBool("Fight", true);
            ani.SetTrigger("Skill_S");
            return;
        }
    }

    private void AttackUpdate()
    {
        if (controller.attack)
        {
            ani.SetBool("Attack_2", true);
        }
    }

    private void SkillUpdate()
    {

    }

    private void MoveVecSet()
    {
        var forward = worldCam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        var right = worldCam.transform.right;
        right.y = 0f;
        right.Normalize();

        moveVec = forward * controller.moveFB;
        moveVec += right * controller.moveLR;

        if (moveVec.magnitude > 1f)
        {
            moveVec.Normalize();
        }
    }
    private void Move()
    {
        var position = rb.position;
        position += moveVec * StateManager.Instance.current.movSp * Time.deltaTime;
        rb.MovePosition(position);
    }
    private void Rotate()
    {
        if (moveVec == Vector3.zero)
            return;

        var rotation = rb.rotation;
        var targetRotateion = Quaternion.LookRotation(moveVec, Vector3.up);
        rotation = Quaternion.RotateTowards(rotation, targetRotateion, rotateSpeed * Time.deltaTime);
        rb.MoveRotation(rotation);
    }

    private IEnumerator IdleToNIdle()
    {
        float timer = fightTimer;

        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }

        currentCo = null;
        unitState = UnitState.NIdle;
        ani.SetBool("Fight", false);
    }

    private IEnumerator FSkillDelayOn()
    {
        fSkillTimer = fSkillTimerSet;

        while (fSkillTimer > 0)
        {
            fSkillTimer -= Time.deltaTime;
            yield return null;
        }

        fSkillDelay = false;
    }
    private IEnumerator SSkillDelayOn()
    {
        sSkillTimer = sSkillTimerSet;

        while (sSkillTimer > 0)
        {
            sSkillTimer -= Time.deltaTime;
            yield return null;
        }

        sSkillDelay = false;
    }

    // ¿Ã∫•∆Æ
    public void ReturnIdle()
    {
        switch (unitState)
        {
            case UnitState.Attack:
                Input.ResetInputAxes();
                unitState = UnitState.Idle;
                ani.SetBool("Attack_2", false);
                break;

            case UnitState.Skill_F:
                Input.ResetInputAxes();
                unitState = UnitState.Idle;
                break;

            case UnitState.Skill_S:
                Input.ResetInputAxes();
                unitState = UnitState.Idle;
                break;

            case UnitState.Impact:
                Input.ResetInputAxes();
                unitState = UnitState.Idle;
                break;
        }
    }

    public void FightCoroutine()
    {
        if (currentCo != null)
        {
            StopCoroutine(currentCo);
            currentCo = StartCoroutine(IdleToNIdle());
            return;
        }

        currentCo = StartCoroutine(IdleToNIdle());
    }

    public void SwordAttack()
    {
        BoxCollider col = hitRanges[0].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[0].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterMovement>().Hit();
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
            }
        }
    }

    public void SwordFSkill()
    {
        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterMovement>().Hit();
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
            }
        }
    }

    public void AxeAttack()
    {
        BoxCollider col = hitRanges[3].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[3].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterMovement>().Hit();
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
            }
        }
    }

    public void AxeFSkill()
    {

        SphereCollider col = hitRanges[4].GetComponent<SphereCollider>();

        Vector3 center = col.bounds.center;
        float half = col.radius;

        Collider[] colliders = Physics.OverlapSphere(center, half);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterMovement>().Hit();
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
            }
        }

    }
    public void AxeSSkill()
    {
        SphereCollider col = hitRanges[5].GetComponent<SphereCollider>();

        Vector3 center = col.bounds.center;
        float half = col.radius;

        Collider[] colliders = Physics.OverlapSphere(center, half);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterMovement>().Hit();
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
            }
        }

    } 
}
