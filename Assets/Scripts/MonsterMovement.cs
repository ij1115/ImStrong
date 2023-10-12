using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    public LayerMask whitIsTarget;
    NavMeshAgent pathFinder;
    public GameObject spawner;
    private Vector3 knockbackVec;
    private Vector3 spawnPos;
    private Coroutine currentCo;
    private Coroutine moveCo;
    private Coroutine aiCo;
    private PlayerWeapons weapons;
    private MonsterInfo mInfo;
    private Animator ani;
    private Rigidbody rb;

    public float fightTimer = 10f;
    public float attackDelay = 10f;
    private bool attackBool = false;

    private GameObject target = null;
    public bool attack {get; private set;}
    public bool finder { get; private set;}
    public UnitState unitState {  get; private set; }

    public GameObject[] hitRanges;
    public GameObject FindRange;

    public void SetUp()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        mInfo = GetComponent<MonsterInfo>();
        mInfo.onDeath += Die;
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<PlayerWeapons>();
        ani = GetComponent<Animator>();
        ani.runtimeAnimatorController = weapons.GetAni();
        unitState = UnitState.NIdle;
        finder = true;
        attack = false;
        spawnPos = rb.transform.position;
        pathFinder.speed = mInfo.state.movSp;

        aiCo = StartCoroutine(UpdateFath());
    }

    public void RunTimeSwap()
    {
        weapons.RunTimeSwap();
        ani.runtimeAnimatorController = weapons.GetAni();
    }

    private void FixedUpdate()
    {
        if (unitState == UnitState.NIdle || unitState == UnitState.Idle)
        {
            ani.SetFloat("Move", pathFinder.desiredVelocity.magnitude);
        }
    }

    private void Update()
    {
        if (aiCo == null)
        {
            aiCo = StartCoroutine(UpdateFath());
        }

        if (target == null)
            return;
        

        if (Vector3.Distance(spawner.transform.position, rb.transform.position) > 12)
        {
            attack = false;
            finder = false;
        }
    }

    private void AttackWeapons()
    {
        switch(weapons.type)
        {
            case Weapons.Sword:
                {
                    BoxCollider col = hitRanges[0].GetComponent<BoxCollider>();

                    Vector3 center = col.bounds.center;
                    Vector3 half = col.bounds.extents;

                    Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[0].transform.rotation);

                    foreach (var obj in colliders)
                    {
                        if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                        {
                            attackBool = true;
                            StartCoroutine(AttackDelay());
                            unitState = UnitState.Attack;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Attack_1");
                            ani.SetBool("Attack_2", true);
                            return;
                        }
                    }
                }
                break;

            case Weapons.Axe:
                {
                    BoxCollider col = hitRanges[3].GetComponent<BoxCollider>();

                    Vector3 center = col.bounds.center;
                    Vector3 half = col.bounds.extents;

                    Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[3].transform.rotation);

                    foreach (var obj in colliders)
                    {
                        if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                        {
                            attackBool = true;
                            StartCoroutine(AttackDelay());
                            unitState = UnitState.Attack;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Attack_1");
                            ani.SetBool("Attack_2", true);
                            return;
                        }
                    }
                }
                break;

            case Weapons.Spear:
                {
                    BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

                    Vector3 center = col.bounds.center;
                    Vector3 half = col.bounds.extents;

                    Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

                    foreach (var obj in colliders)
                    {
                        if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                        {
                            attackBool = true;
                            StartCoroutine(AttackDelay());
                            unitState = UnitState.Attack;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Attack_1");
                            ani.SetBool("Attack_2", true);
                            return;
                        }
                    }
                }
                break;
        }
    }

    private IEnumerator UpdateFath()
    {
        while (!mInfo.dead)
        {
            if (attack)
            {
                if (unitState == UnitState.NIdle || unitState == UnitState.Idle)
                {
                    pathFinder.isStopped = false;
                    pathFinder.SetDestination(target.transform.position);

                    if (!attackBool)
                        AttackWeapons();
                }
                else
                {
                    pathFinder.isStopped = true;
                }
            }
            else
            {
                if (finder)
                {
                    pathFinder.isStopped = true;

                    if (unitState == UnitState.Idle || unitState == UnitState.NIdle)
                    {
                        SphereCollider col = FindRange.GetComponent<SphereCollider>();
                        Collider[] colliders = Physics.OverlapSphere(col.gameObject.transform.position, 12f, whitIsTarget);

                        for (int i = 0; i < colliders.Length; ++i)
                        {
                            var p = colliders[i].GetComponent<PlayerInfo>();
                            if (p != null && !p.dead)
                            {
                                target = p.gameObject;
                                attack = true;
                                break;
                            }
                        }
                    }
                }
                else if (!finder)
                {
                    pathFinder.isStopped = false;
                    pathFinder.SetDestination(spawnPos);

                    if (!pathFinder.pathPending)
                    {
                        if (pathFinder.remainingDistance <= pathFinder.stoppingDistance)
                        {
                            finder = true;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.25f);
            
        }
        finder = true;
        attack = false;
        aiCo = null;
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

    private IEnumerator AttackDelay()
    {
        float timer = attackDelay;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
        attackBool = false;
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

    public void HitKB(Vector3 vec)
    {
        knockbackVec = vec;
        finder = true;
        pathFinder.isStopped = true;
        unitState = UnitState.Knockback;
        ani.SetBool("Fight", true);
        ani.SetTrigger("Knockback");
        StartCoroutine(SpearSSkillMove());
    }

    public void Hit()
    {
        finder = true;
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
                ani.speed = 1f;
                ani.SetBool("Attack_2", false);
                break;

            case UnitState.Skill_F:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                break;

            case UnitState.Skill_S:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                break;

            case UnitState.Impact:
                pathFinder.isStopped = false;
                ani.speed = 1f;
                unitState = UnitState.Idle;
                break;

            case UnitState.Knockback:
                pathFinder.isStopped = false;
                ani.speed = 1f;
                unitState = UnitState.Idle;
                break;
        }
    }
    public void ReturnIdle_Attack()
    {
        ani.SetBool("Attack_2", false);
        ani.speed = 1f;
        unitState = UnitState.Idle;
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
        float endTime = (14f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
        float endTime = (6f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
        float frameTime = ANI_LENGTH / 110f;

        float firTime = (25f * frameTime) / mInfo.state.atkSp;
        float secTime = (10f * frameTime) / mInfo.state.atkSp;
        float tirTime = (15f * frameTime) / mInfo.state.atkSp;

        float targetFrameTime = 1.0f / 30.0f;
        firTime = Mathf.Ceil(firTime / targetFrameTime) * targetFrameTime;
        secTime = Mathf.Ceil(secTime / targetFrameTime) * targetFrameTime;
        tirTime = Mathf.Ceil(tirTime / targetFrameTime) * targetFrameTime;

        float timer = 0f;

        bool first = false;
        bool sceond = false;

        var startPos = rb.transform.position;
        var firPos = rb.transform.position + rb.transform.forward;
        var secPos = rb.transform.position + rb.transform.forward * 2f;
        var tirPos = rb.transform.position + rb.transform.forward * 4f;

        while (true)
        {
            timer += Time.deltaTime;

            if (!first)
            {
                rb.MovePosition(EaseOutSine(startPos, firPos, timer / firTime));
                if (timer > firTime)
                {
                    timer -= firTime;
                    rb.MovePosition(EaseInSine(firPos, secPos, timer / secTime));
                    first = true;
                }
            }
            else if (!sceond)
            {
                rb.MovePosition(EaseInSine(firPos, secPos, timer / secTime));
                if (timer > secTime)
                {
                    timer -= secTime;
                    rb.MovePosition(EaseInCubic(secPos, tirPos, timer / tirTime));
                    sceond = true;
                }
            }
            else if (timer < tirTime)
            {
                rb.MovePosition(EaseInCubic(secPos, tirPos, timer / tirTime));
            }
            else if (timer > tirTime)
            {
                timer = tirTime;
                rb.MovePosition(EaseInCubic(secPos, tirPos, timer / tirTime));
                break;
            }
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator SwordSkillTwoMovePlay()
    {
        const float ANI_LENGTH = 3.067f;
        float endTime = (22f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 1f;

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
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                var com = obj.gameObject.GetComponent<PlayerMovement>();
          
                if(com.unitState==UnitState.Evade)
                {
                    continue;
                }

                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die||
                    com.unitState == UnitState.Skill_F||
                    com.unitState == UnitState.Skill_S)
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
                var com = obj.gameObject.GetComponent<PlayerMovement>();

                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
        BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

        Vector3 center = col.bounds.center;
        Vector3 half = col.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

        foreach (var obj in colliders)
        {
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk *3f));
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

    public void AxeAttackMovePlay()
    {
        moveCo = StartCoroutine(AxeAttackMove());
    }
    public void AxeAttackComboMovePlay()
    {
        moveCo = StartCoroutine(AxeAttackComboMove());
    }
    public void AxeSkillMovePlay()
    {
        moveCo = StartCoroutine(AxeSkillFirstMove());
    }
    public void AxeSkillTwoMove()
    {
        moveCo = StartCoroutine(AxeSkillTwoMovePlay());
    }

    private IEnumerator AxeAttackMove()
    {
        const float ANI_LENGTH = 2.500f;
        float endTime = (18f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
        float endTime = (6f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
    private IEnumerator AxeSkillFirstMove()
    {
        const float ANI_LENGTH = 2.900f;
        float frameTime = ANI_LENGTH / 87f;
        float endTime = (60f * frameTime) / mInfo.state.atkSp;
        float targetFrameTime = 1.0f / 30.0f;
        endTime = Mathf.Ceil(endTime / targetFrameTime) * targetFrameTime;

        float timer = 0f;

        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 4.5f;

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
    private IEnumerator AxeSkillTwoMovePlay()
    {
        const float ANI_LENGTH = 2.467f;
        float endTime = (24f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk *3f));

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

    public void SpearAttackMovePlay()
    {
        switch (unitState)
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
    public void SpearSkillMovePlay()
    {
        moveCo = StartCoroutine(SpearFristSkillMove());
    }

    private IEnumerator SpearFristSkillMove()
    {
        const float ANI_LENGTH = 1.733f;
        float frameTime = ANI_LENGTH / 52f;
        float endTime = (46 * frameTime) / mInfo.state.atkSp;
        float targetFrameTime = 1.0f / 30.0f;
        endTime = Mathf.Ceil(endTime / targetFrameTime) * targetFrameTime;

        float timer = 0f;

        Debug.Log(endTime);
        var startPos = rb.transform.position;
        var endPos = rb.transform.position + rb.transform.forward * 3f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > endTime)
            {
                timer = endTime;
                Vector3 nowPos = EaseOutBounce(startPos, endPos, timer / endTime);
                rb.MovePosition(nowPos);
                break;
            }
            rb.MovePosition(EaseOutBounce(startPos, endPos, timer / endTime));
            yield return null;
        }
        moveCo = null;
    }
    private IEnumerator SpearAttackMove()
    {
        const float ANI_LENGTH = 1.167f;
        float endTime = (14f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
        float endTime = (6f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
        float endTime = (14f / 30f) * ANI_LENGTH / mInfo.state.atkSp;
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
            if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
            {
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                switch (unitState)
                {
                    case UnitState.Attack:
                        obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);
                        if (com.unitState == UnitState.Stun ||
                            com.unitState == UnitState.Down ||
                            com.unitState == UnitState.Air ||
                            com.unitState == UnitState.Knockback ||
                            com.unitState == UnitState.Die ||
                            com.unitState == UnitState.Skill_F ||
                            com.unitState == UnitState.Skill_S)
                            continue;

                        com.Hit();
                        break;

                    case UnitState.Skill_S:
                        obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 3f));
                        com.Knockback(rb.transform.forward);

                        if (com.unitState == UnitState.Stun ||
                            com.unitState == UnitState.Down ||
                            com.unitState == UnitState.Air ||
                            com.unitState == UnitState.Die ||
                            com.unitState == UnitState.Skill_F ||
                            com.unitState == UnitState.Skill_S)
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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));
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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(mInfo.state.atk);

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
                var com = obj.gameObject.GetComponent<PlayerMovement>();
                if (com.unitState == UnitState.Evade)
                {
                    continue;
                }
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk * 0.5f));

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
    public Vector3 EaseOutQuint(Vector3 start, Vector3 end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
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
        if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
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
        if (value < 1) return end * 0.5f * value * value + start;
        value--;
        return -end * 0.5f * (value * (value - 2) - 1) + start;
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
    public Vector3 EaseOutBounce(Vector3 start, Vector3 end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }

    private AnimationClip GetCurrentClip()
    {
        AnimatorStateInfo stateInfo = ani.GetCurrentAnimatorStateInfo(0);

        AnimationClip clip = ani.GetCurrentAnimatorClipInfo(0)[0].clip;

        return clip;
    }
}

