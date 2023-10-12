using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 knockbackVec;

    private Coroutine currentCo;
    public Coroutine moveCo;
    private PlayerController controller;

    private PlayerWeapons weapons;
    private Animator ani;
    private Rigidbody rb;
    private PlayerInfo info;

    public float rotateSpeed = 360f;
    public float fightTimer = 10f;

    public float currentValue = 0f;
    
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

            ani.SetFloat("Move", currentValue);
        }
    }

    private void Update()
    {
        if (controller.evade &&
            unitState!=UnitState.Evade&&
                    unitState != UnitState.Stun &&
                    unitState != UnitState.Down &&
                    unitState != UnitState.Air &&
                    unitState != UnitState.Knockback &&
                    unitState != UnitState.Die&&
                    unitState!=UnitState.Impact)
        {
            unitState = UnitState.Evade;
            ani.SetTrigger("evade");
        }

        switch (unitState)
        {
            case UnitState.NIdle:
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

        if (controller.attack)
        {
            unitState = UnitState.Attack;
            ani.speed = info.state.atkSp;
            ani.SetBool("Fight", true);
            ani.SetTrigger("Attack_1");
            return;
        }

        if (controller.firstSkill && !fSkillDelay)
        {
            unitState = UnitState.Skill_F;
            ani.speed = info.state.atkSp;
            fSkillDelay = true;
            StartCoroutine(FSkillDelayOn());
            ani.SetBool("Fight", true);
            ani.SetTrigger("Skill_F");
            return;
        }

        if (controller.secondSkill && !sSkillDelay)
        {
            unitState = UnitState.Skill_S;
            ani.speed = info.state.atkSp;
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

        if (moveVec.magnitude != 0)
        {
            currentValue += moveVec.magnitude * 10f * Time.deltaTime;
            if (currentValue > 1)
            {
                currentValue = 1;
            }
        }
        else
        {
            currentValue = Mathf.Lerp(currentValue, 0.0f, 8f * Time.deltaTime);
        }
    }

    private void Move()
    {
        var position = rb.position;
        position += moveVec * info.state.movSp * Time.deltaTime;
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
                if (ani.GetBool("Attack_2"))
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

    public void EvadeMovePlay()
    {
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
            moveCo = null;
        }
        ani.speed = 1f;
        moveCo = StartCoroutine(EvadeMove());
    }
    private IEnumerator EvadeMove()
    {
        //float aniLength = GetCurrentClip().length;
        float endTime =0f;
        switch (weapons.type)
        {
            case Weapons.Sword:
            case Weapons.Spear:
                endTime = (24f / 30f);
                break;

            case Weapons.Axe:
                endTime = (20f / 30f);
                break;

        };

        float timer = 0f;
        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 5f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutQuad(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutQuad(startPos, endPos, timer / endTime));
            yield return null;
        }
        ReturnIdle();
        moveCo = null;
    }


    public void SwordAttackMovePlay()
    {
        moveCo = StartCoroutine(SwordAttackMove());
    }
    public void SwordAttackComboMovePlay()
    {
        moveCo = StartCoroutine(SwordAttackComboMove());
    }
    public void SwordSkillMovePlay()
    {
        moveCo = StartCoroutine(SwordFirstSkillMove());
    }
    public void SwordSkillTwoMove()
    {
        moveCo = StartCoroutine(SwordSkillTwoMovePlay());
    }

    private IEnumerator SwordAttackMove()
    {
        const float ANI_LENGTH = 1.333f;
        float endTime = (14f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 0.5f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutExpo(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator SwordAttackComboMove()
    {
        const float ANI_LENGTH = 1.367f;
        float endTime = (6f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 0.7f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutExpo(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator SwordFirstSkillMove()
    {
        const float ANI_LENGTH = 3.667f;
        float firTime = (29f / 30f) * ANI_LENGTH / info.state.atkSp;
        float secTime = ((47f / 30f) * ANI_LENGTH / info.state.atkSp) - firTime;
        float tirTime = ((76f / 30f) * ANI_LENGTH / info.state.atkSp) - secTime;
        float timer = 0f;

        bool first = false;
        bool sceond = false;

        var startPos = rb.transform.position;
        var firPos = rb.transform.position + rb.transform.forward * 1f;
        var secPos = firPos + rb.transform.forward * 1f;
        var tirPos = secPos + rb.transform.forward * 2f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer < firTime && !first)
            {
                rb.MovePosition(EaseOutSine(startPos, firPos, timer / firTime));
                if(timer> firTime)
                {
                    timer -= firTime;
                    rb.MovePosition(EaseInSine(firPos, secPos, timer / secTime));
                    first = true;
                }    
            }
            else if( timer < secTime&&!sceond)
            {
                rb.MovePosition(EaseInSine(firPos, secPos, timer / secTime));
                if (timer > secTime)
                {
                    timer -= secTime;
                    rb.MovePosition(EaseInCubic(secPos, tirPos, timer / tirTime));
                    sceond = true;
                }
            }
            else if(timer<tirTime)
            {
                rb.MovePosition(EaseInCubic(secPos, tirPos, timer / tirTime));
            }
            else
            {
                timer = tirTime;
                rb.MovePosition(EaseInCubic(secPos, tirPos, timer / tirTime));
                break;
            }
            //    StartCoroutine(SwordFirstSkillMove_2(timer));
            //    break;
            //}
            //rb.MovePosition(EaseOutSine(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;

    } // 수정필요
    //private IEnumerator SwordFirstSkillMove_2(float lateTimer)
    //{
    //    float aniLength = GetCurrentClip().length;
    //    float endTime = (47f / 30f) * aniLength / info.state.atkSp;
    //    float timer = lateTimer;

    //    var startPos = rb.transform.position;
    //    var endPos = rb.transform.position + rb.transform.forward * 1f;

    //    while (true)
    //    {
    //        timer += Time.deltaTime;
    //        Debug.Log(timer);

    //        if (timer > endTime)
    //        {
    //            StartCoroutine(SwordFirstSkillMove_3(timer));
    //            break;
    //        }
    //        rb.MovePosition(EaseInSine(startPos, endPos, timer / endTime));
    //        yield return null;
    //    }


    //}

    //private IEnumerator SwordFirstSkillMove_3(float lateTimer)
    //{
    //    float aniLength = GetCurrentClip().length;
    //    float endTime = (76f / 30f) * aniLength / info.state.atkSp;
    //    float timer = lateTimer;

    //    var startPos = rb.transform.position;
    //    var endPos = rb.transform.position + rb.transform.forward * 2f;

    //    while (true)
    //    {
    //        timer += Time.deltaTime;
    //        Debug.Log(timer);

    //        if (timer > endTime)
    //        {
    //            timer = endTime;
    //            Vector3 nowPos = EaseInCubic(startPos, endPos, timer / endTime);
    //            rb.MovePosition(nowPos);
    //            break;
    //        }
    //        rb.MovePosition(EaseInCubic(startPos, endPos, timer / endTime));
    //        yield return null;
    //    }
    //}
    private IEnumerator SwordSkillTwoMovePlay()
    {
        const float ANI_LENGTH = 3.067f;
        float endTime = (22f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 2f;

        while (true)
        {
            timer += Time.deltaTime;
            Debug.Log(timer);

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaselInOutCircle(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
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

    public void AxeAttackMovePlay()
    {
        moveCo = StartCoroutine(AxeAttackMove());
    }
    public void AxeAttackComboMovePlay()
    {
        moveCo = StartCoroutine(AxeAttackComboMove());
    }
    public void AxeSkillTwoMove()
    {
        moveCo = StartCoroutine(AxeSkillTwoMovePlay());
    }
    private IEnumerator AxeAttackMove()
    {
        const float ANI_LENGTH = 2.500f;
         float endTime = (18f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 1f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutQuart(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator AxeAttackComboMove()
    {
        const float ANI_LENGTH = 2.367f;
        float endTime = (6f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 1.5f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutExpo(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator AxeSkillTwoMovePlay()
    {
        const float ANI_LENGTH = 2.467f;
        float endTime = (24f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 1f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaselInOutCircle(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
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
    public void SpearAttackMovePlay()
    {
        switch(unitState)
        {
            case UnitState.Attack:
                moveCo = StartCoroutine(SpearAttackMove());
                break;

            case UnitState.Skill_S:
                moveCo = StartCoroutine(SpearSkillTwoMovePlay());
                break;
        }    
    }
    public void SpearAttackComboMovePlay()
    {
        moveCo = StartCoroutine(SpearAttackComboMove());
    }

    private IEnumerator SpearAttackMove()
    {
        const float ANI_LENGTH = 1.167f;
        float endTime = (14f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 1f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutQuart(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator SpearAttackComboMove()
    {
        const float ANI_LENGTH = 1.167f;
        float endTime = (6f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 1f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseInOutExpo(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator SpearSkillTwoMovePlay()
    {
        const float ANI_LENGTH = 1.167f;
        float endTime = (14f / 30f) * ANI_LENGTH / info.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 3f;

        while (true)
        {
            timer += Time.deltaTime;
            Debug.Log(timer);

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaselInOutCircle(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
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
                        obj.GetComponent<MonsterInfo>().OnDamage(Mathf.RoundToInt((float)info.state.atk * 3f));
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

    public Vector3 EaseInOutExpo(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    public Vector3 EaseInQuint(Vector3 start, Vector3 end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public Vector3 EaseInCubic(Vector3 start, Vector3 end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public Vector3 EaseOutCubic(Vector3 start, Vector3 end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }
    public Vector3 EaselInOutCircle(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value< 1) return -end* 0.5f * (Mathf.Sqrt(1 - value* value) - 1) + start;
        value -= 2;
        return end* 0.5f * (Mathf.Sqrt(1 - value* value) + 1) + start;
    }

    public Vector3 EaseInOutQuart(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value + start;
        value -= 2;
        return -end * 0.5f * (value * value * value * value - 2) + start;
    }
    public Vector3 EaseInOutQuad(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value< 1) return end* 0.5f * value* value + start;
        value--;
        return -end* 0.5f * (value* (value - 2) - 1) + start;
    }
    public Vector3 EaseInSine(Vector3 start, Vector3 end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
    }

    public Vector3 EaseOutSine(Vector3 start, Vector3 end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
    }


    private AnimationClip GetCurrentClip()
    {
        AnimatorStateInfo stateInfo = ani.GetCurrentAnimatorStateInfo(0);

        AnimationClip clip = ani.GetCurrentAnimatorClipInfo(0)[0].clip;

        return clip;
    }
}
