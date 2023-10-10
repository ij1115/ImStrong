using Cinemachine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Coroutine currentCo;
    private Vector3 knockbackVec;
    private PlayerController controller;
    private PlayerWeapons weapons;
    private Animator ani;
    private Rigidbody rb;
    private PlayerInfo info;

    public float rotateSpeed = 360f;
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
            ani.speed = StateManager.Instance.atkSp;
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
            ani.speed = StateManager.Instance.atkSp;
            ani.SetBool("Fight", true);
            ani.SetTrigger("Attack_1");
            return;
        }

        if (controller.firstSkill && !fSkillDelay)
        {
            unitState = UnitState.Skill_F;
            fSkillDelay = true;
            ani.speed = StateManager.Instance.atkSp;
            StartCoroutine(FSkillDelayOn());
            ani.SetBool("Fight", true);
            ani.SetTrigger("Skill_F");
            return;
        }

        if (controller.secondSkill && !sSkillDelay)
        {
            unitState = UnitState.Skill_S;
            ani.speed = StateManager.Instance.atkSp;
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
    // ÀÌº¥Æ®

    public void Knockback(Vector3 vec)
    {
        knockbackVec = vec;

        StartCoroutine(SpearSSkillMove());
    }

    public void ReturnIdle_Attack()
    {
        ani.SetBool("Attack_2", false);
        ani.speed = 1f;
        unitState = UnitState.Idle;
    }
    public void ReturnIdle()
    {
        switch (unitState)
        {
            case UnitState.Attack:
                if(ani.GetBool("Attack_2"))
                {
                    break;
                }
                moveVec = Vector3.zero;
                ani.speed = 1f;
                unitState = UnitState.Idle;
                ani.SetBool("Attack_2", false);
                break;

                default:
                ani.speed = 1f;
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
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                var com = obj.gameObject.GetComponent<MonsterMovement>();

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback||
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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk*0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk*0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk*3f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 3f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {

                var com = obj.gameObject.GetComponent<MonsterMovement>();
                switch (unitState)
                {
                    case UnitState.Attack:
                        obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                        if (com.unitState == UnitState.Stun ||
                            com.unitState == UnitState.Down ||
                            com.unitState == UnitState.Air ||
                            com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die)
                            continue;

                        com.Hit();
                        break;

                    case UnitState.Skill_S:
                        obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk *3f));
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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(info.state.atk);
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
            if (obj.CompareTag("Monster") && !obj.GetComponent<MonsterInfo>().dead)
            {
                obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 0.5f));
                var com = obj.gameObject.GetComponent<MonsterMovement>();

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
