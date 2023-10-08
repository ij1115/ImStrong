using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public GameObject spawner;

    private Coroutine currentCo;
    private PlayerWeapons weapons;
    private MonsterInfo mInfo;
    private Animator ani;
    private Rigidbody rb;

    public float rotateSpeed = 180f;
    public float fightTimer = 10f;
    private Vector3 moveVec = Vector3.zero;

    private GameObject target = null;

    private bool attack = true;

    public UnitState unitState {  get; private set; }

    public GameObject[] hitRanges;


    public void SetUp()
    {
        mInfo = GetComponent<MonsterInfo>();
        mInfo.onDeath += Die;
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<PlayerWeapons>();
        ani = GetComponent<Animator>();
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
        if (target == null)
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

            default:
                return;
        }
    }

    private void IdleUpdate()
    {
        MoveVecSet();

        if (!attack)
        {
            moveVec = Vector3.zero;
            unitState = UnitState.Attack;
            ani.SetTrigger("Attack_1");
        }
    }

    private void NIdleUpdate()
    {
        MoveVecSet();

        if (!attack)
        {
            moveVec = Vector3.zero;
            unitState = UnitState.Attack;
            ani.SetBool("Fight", true);
            ani.SetTrigger("Attack_1");
        }
    }

    private void AttackUpdate()
    {
        if (attack)
        {
            ani.SetBool("Attack_2", true);
        }
    }

    private void MoveVecSet()
    {

        if (moveVec.magnitude > 1f)
        {
            moveVec.Normalize();
        }

    }

    private void Move()
    {
        var position = rb.position;
        position += moveVec * mInfo.state.movSp * Time.deltaTime;
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

    // ¿Ã∫•∆Æ

    public void Die()
    {
        unitState = UnitState.Die;

        var colls = GetComponents<Collider>();
        foreach (var coll in colls)
        {
            coll.enabled = false;
        }

        ani.SetTrigger("Die");
        spawner.GetComponent<Spawner>().Die(this.gameObject);
    }

    public void Hit()
    {
        unitState = UnitState.Impact;
        ani.SetBool("Fight", true);
        ani.SetTrigger("Impact");
    }

    public void ReturnIdle()
    {
        switch (unitState)
        {
            case UnitState.Attack:
                unitState = UnitState.Idle;
                ani.SetBool("Attack_2", false);
                break;

            case UnitState.Skill_F:
                unitState = UnitState.Idle;
                break;

            case UnitState.Skill_S:
                unitState = UnitState.Idle;
                break;

            case UnitState.Impact:
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
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
            }
        }
    }
}
