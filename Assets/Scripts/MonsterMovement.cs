using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MonsterMovement : MonoBehaviour
{
    public GameObject spawner;
    private Vector3 knockbackVec;
    private Vector3 spawnPos;
    private Coroutine currentCo;
    private PlayerWeapons weapons;
    private MonsterInfo mInfo;
    private Animator ani;
    private Rigidbody rb;

    public float rotateSpeed = 180f;
    public float fightTimer = 10f;
    private Vector3 moveVec = Vector3.zero;

    private GameObject target = null;
    public bool attack {get; private set;}

    public UnitState unitState {  get; private set; }

    public GameObject[] hitRanges;
    public GameObject FindRange;

    public void SetUp()
    {
        mInfo = GetComponent<MonsterInfo>();
        mInfo.onDeath += Die;
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<PlayerWeapons>();
        ani = GetComponent<Animator>();
        ani.runtimeAnimatorController = weapons.GetAni();
        unitState = UnitState.NIdle;
        spawnPos = rb.transform.position;
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
        if (target == null)
            return;

        if(Vector3.Distance(spawner.transform.position, rb.transform.position)>12)
        {
            attack = false;

            StopCoroutine(finder);
        }

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

    private IEnumerator UpdateFath()
    {
        while(attack)
        {
            yield return new WaitForSeconds(0.25f);
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
    private IEnumerator SpearSSkillMove()
    {
        var startPos = rb.transform.position;
        var endPos = rb.transform.position + knockbackVec * 3f;
        float duration = 0.15f;
        float moveTimer = 0f;

        while (moveTimer < duration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, endPos, moveTimer / duration));
            moveTimer += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(endPos);
    }
    public void Knockback(Vector3 vec)
    {
        knockbackVec = vec;

        StartCoroutine(SpearSSkillMove());
    }

    // ÀÌº¥Æ®

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

    public void HitKB()
    {
        unitState = UnitState.Knockback;
        ani.SetBool("Fight", true);
        ani.SetTrigger("Knockback");
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
                if (ani.GetBool("Attack_2"))
                {
                    break;
                }
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

            case UnitState.Knockback:
                unitState = UnitState.Idle;
                break;
        }
    }
    public void ReturnIdle_Attack()
    {
        unitState = UnitState.Idle;
        ani.SetBool("Attack_2", false);
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
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void SwordFSkill_1()
    {
        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void SwordFSkill_2()
    {
        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void SwordFSkill_3()
    {
        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void SwordSSkill()
    {
        BoxCollider col = hitRanges[2].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[2].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk *3f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
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
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void AxeFSkill_1()
    {

        BoxCollider col = hitRanges[3].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[3].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void AxeFSkill_2()
    {

        SphereCollider col = hitRanges[4].GetComponent<SphereCollider>();

        Vector3 center = col.bounds.center;
        float half = col.radius;

        Collider[] colliders = Physics.OverlapSphere(center, half);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }
    public void AxeFSkill_3()
    {

        SphereCollider col = hitRanges[4].GetComponent<SphereCollider>();

        Vector3 center = col.bounds.center;
        float half = col.radius;

        Collider[] colliders = Physics.OverlapSphere(center, half);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }
    public void AxeFSkill_4()
    {

        SphereCollider col = hitRanges[4].GetComponent<SphereCollider>();

        Vector3 center = col.bounds.center;
        float half = col.radius;

        Collider[] colliders = Physics.OverlapSphere(center, half);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
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
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk *3f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void SpearAttack()
    {
        BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {

                var com = obj.gameObject.GetComponent<PlayerMovement>();
                switch (unitState)
                {
                    case UnitState.Attack:
                        obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
                        if (com.unitState == UnitState.Stun ||
                            com.unitState == UnitState.Down ||
                            com.unitState == UnitState.Air ||
                            com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                            continue;

                        com.Hit();
                        break;

                    case UnitState.Skill_S:
                        obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 3f));
                        com.Knockback(rb.transform.forward);

                        if (com.unitState == UnitState.Stun ||
                            com.unitState == UnitState.Down ||
                            com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Die)
                            continue;

                        com.HitKB();
                        break;
                }

            }
        }
    }

    public void SpearFSkill_1()
    {
        BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }

    public void SpearFSkill_2()
    {
        BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }
    public void SpearFSkill_3()
    {
        BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }
    public void SpearFSkill_4()
    {
        BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                    continue;

                com.Hit();
            }
        }
    }
}
