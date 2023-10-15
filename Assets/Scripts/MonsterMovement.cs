using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterMovement : MonoBehaviour
{
    public LayerMask mask;
    public LayerMask whitIsTarget;
    NavMeshAgent pathFinder;
    public GameObject spawner;
    private Vector3 knockbackVec;
    private Coroutine currentCo;
    private Coroutine moveCo;
    private Coroutine aiCo;
    private PlayerWeapons weapons;
    private MonsterInfo mInfo;
    private Animator ani;
    private Rigidbody rb;

    public float fightTimer = 10f;
    public float attackDelay = 0f;
    public float hitRange = 0f;
    private bool attackBool = false;
    public float fSkillDelay = 0f;
    private bool fSkillBool = false;
    public float sSkillDelay = 0f;
    private bool sSkillBool = false;
    public bool isMoving { get; private set; }

    private GameObject target = null;
    public bool finder { get; private set;}
    public UnitState unitState {  get; private set; }

    public GameObject[] hitRanges;
    public GameObject FindRange;

    private int airDamage = 0;

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
        pathFinder.speed = mInfo.state.movSp;
        HitRangeSwap();
        aiCo = StartCoroutine(UpdateFath());
    }

    public void RunTimeSwap()
    {
        weapons.RunTimeSwap();
        ani.runtimeAnimatorController = weapons.GetAni();
    }

    public void HitRangeSwap()
    {
        switch (mInfo.type)
        {
            case MonsterType.Mob:
                attackDelay = 10f;
                break;

            case MonsterType.SubBoss:
                attackDelay = 15f;
                fSkillDelay = 20f;
                break;

            case MonsterType.Boss:
                attackDelay = 20f;
                fSkillDelay = 25f;
                sSkillDelay = 30f;
                break;
        }

    }
  
    private void FixedUpdate()
    {
        EngineConverter();

        if (unitState == UnitState.NIdle || unitState == UnitState.Idle)
        {
            ani.SetFloat("Move", pathFinder.desiredVelocity.magnitude);
        }
    }

    private void Update()
    {
        if (target == null && aiCo == null)
        {
            aiCo = StartCoroutine(UpdateFath());
        }

        if (target == null)
        {
            finder = true;
            return;
        }

        switch (mInfo.type)
        {
            case MonsterType.Boss:
                if ((unitState == UnitState.NIdle || unitState == UnitState.Idle) && !sSkillBool && mInfo.hpSlider.value<0.5f)
                {
                    UseSSkill();
                }
                break;
        }

        switch (mInfo.type)
        {
            case MonsterType.SubBoss:
            case MonsterType.Boss:
                if ((unitState == UnitState.NIdle || unitState == UnitState.Idle) && !fSkillBool)
                {
                    UseFSkill();
                }
                break;
        }

        if ((unitState == UnitState.NIdle || unitState == UnitState.Idle) && !attackBool)
        {
            AttackWeapons();
        }


        if ((unitState == UnitState.NIdle || unitState == UnitState.Idle) && pathFinder.enabled)
        {
            pathFinder.SetDestination(target.transform.position);
        }
    }

    private void EngineConverter()
    {
        switch (isMoving)
        {
            case true:
                if (!pathFinder.enabled)
                    pathFinder.enabled = true;

                if (!rb.isKinematic)
                    rb.isKinematic = true;
                break;

            case false:
                if (pathFinder.enabled)
                    pathFinder.enabled = false;

                if (rb.isKinematic)
                    rb.isKinematic = false;
                break;
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
                            isMoving = false;
                            attackBool = true;
                            StartCoroutine(AttackDelay());
                            unitState = UnitState.Attack;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Attack_1");
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
                            isMoving = false;
                            attackBool = true;
                            StartCoroutine(AttackDelay());
                            unitState = UnitState.Attack;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Attack_1");
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
                            isMoving = false;
                            attackBool = true;
                            StartCoroutine(AttackDelay());
                            unitState = UnitState.Attack;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Attack_1");
                            return;
                        }
                    }
                }
                break;
        }
    }
    private void UseFSkill()
    {
        switch (weapons.type)
        {
            case Weapons.Sword:
                {
                    BoxCollider col = hitRanges[1].GetComponent<BoxCollider>();

                    Vector3 center = col.bounds.center;
                    Vector3 half = col.bounds.extents;

                    Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[1].transform.rotation);

                    foreach (var obj in colliders)
                    {
                        if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                        {
                            isMoving = false;
                            fSkillBool = true;
                            StartCoroutine(FSkillDelay());
                            unitState = UnitState.Skill_F;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Skill_F");
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
                            isMoving = false;
                            fSkillBool = true;
                            StartCoroutine(FSkillDelay());
                            unitState = UnitState.Skill_F;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Skill_F");
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
                            isMoving = false;
                            fSkillBool = true;
                            StartCoroutine(FSkillDelay());
                            unitState = UnitState.Skill_F;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Skill_F");
                        }
                    }
                }
                break;
        }
    }

    private void UseSSkill()
    {
        switch (weapons.type)
        {
            case Weapons.Sword:
                {
                    BoxCollider col = hitRanges[2].GetComponent<BoxCollider>();

                    Vector3 center = col.bounds.center;
                    Vector3 half = col.bounds.extents;

                    Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[2].transform.rotation);

                    foreach (var obj in colliders)
                    {
                        if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                        {
                            isMoving = false;
                            sSkillBool = true;
                            StartCoroutine(SSkillDelay());
                            unitState = UnitState.Skill_S;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Skill_S");
                            return;
                        }
                    }
                }
                break;

            case Weapons.Axe:
                {
                    SphereCollider col = hitRanges[5].GetComponent<SphereCollider>();

                    Vector3 center = col.bounds.center;
                    float half = col.radius;

                    Collider[] colliders = Physics.OverlapSphere(center, half);
                    foreach (var obj in colliders)
                    {
                        if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                        {
                            isMoving = false;
                            sSkillBool = true;
                            StartCoroutine(SSkillDelay());
                            unitState = UnitState.Skill_S;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Skill_S");
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
                            isMoving = false;
                            sSkillBool = true;
                            StartCoroutine(SSkillDelay());
                            unitState = UnitState.Skill_S;
                            ani.speed = mInfo.state.atkSp;
                            ani.SetBool("Fight", true);
                            ani.SetTrigger("Skill_S");
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
            if(finder)
            { 
                if (unitState == UnitState.Idle || unitState == UnitState.NIdle)
                {
                    SphereCollider col = FindRange.GetComponent<SphereCollider>();
                    Collider[] colliders = Physics.OverlapSphere(col.gameObject.transform.position, col.radius, whitIsTarget);

                    for (int i = 0; i < colliders.Length; ++i)
                    {
                        var p = colliders[i].GetComponent<PlayerInfo>();
                        if (p != null && !p.dead)
                        {
                            target = p.gameObject;
                            isMoving = true;
                            finder = false;
                            break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
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

    private IEnumerator FSkillDelay()
    {
        float timer = fSkillDelay;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
       fSkillBool = false;
    }
    private IEnumerator SSkillDelay()
    {
        float timer = sSkillDelay;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
        sSkillBool = false;
    }
    private IEnumerator SpearSSkillMove()
    {
        var startPos = rb.transform.position;
        var endPos = rb.transform.position + knockbackVec * 3f;
        float duration = 0.15f;
        float moveTimer = 0f;

        while (moveTimer < duration)
        {
            moveTimer += Time.deltaTime;

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, knockbackVec);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = Vector3.Lerp(startPos, endPos, moveTimer / duration);
                    }
                }
            }

            else
            {
                nowPos = Vector3.Lerp(startPos, endPos, moveTimer / duration);
            }

            if (moveTimer > duration)
            {
                moveTimer = duration;
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
            yield return null;
        }
    }


    // ¿Ã∫•∆Æ

    public void Die()
    {
        unitState = UnitState.Die;

        pathFinder.enabled = false;
        rb.isKinematic = true;

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
        isMoving = false;
        unitState = UnitState.Knockback;
        ani.speed = 1f;
        ani.SetBool("Fight", true);
        ani.SetTrigger("Knockback");
        StartCoroutine(SpearSSkillMove());
    }

    public void AirDamage()
    {
        mInfo.OnDamage(airDamage);
        airDamage = 0;
    }
    public void Air(int damage)
    {
        isMoving = false;
        unitState = UnitState.Air;
        airDamage = damage;
        ani.SetTrigger("Air");
    }
    public void Down()
    {
        isMoving = false;
        unitState = UnitState.Down;
        ani.SetTrigger("Down");
    }

    public void Stun(float time)
    {
        isMoving = false;
        unitState = UnitState.Stun;
        ani.speed = 1f;
        ani.SetTrigger("Stun");
        StartCoroutine(StunRelese(time));
    }

    private IEnumerator StunRelese(float time)
    {
        float stunTime = time;
        while(stunTime>0)
        {
            isMoving = false;
            stunTime -= Time.deltaTime;
            yield return null;
        }

        ReturnIdle();
        ani.SetTrigger("StunRelese");
    }
    public void Hit()
    {
        isMoving = false;
        unitState = UnitState.Impact;
        ani.speed = 1f;
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
                isMoving = true;
                ani.speed = 1f;
                ani.SetBool("Attack_2", false);
                break;

            case UnitState.Skill_F:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;

            case UnitState.Skill_S:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;

            case UnitState.Impact:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;

            case UnitState.Knockback:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;

            case UnitState.Stun:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;

            case UnitState.Down:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;

            case UnitState.Air:
                ani.speed = 1f;
                unitState = UnitState.Idle;
                isMoving = true;
                break;
        }
    }
    public void ReturnIdle_Attack()
    {
        ani.SetBool("Attack_2", false);
        ani.speed = 1f;
        isMoving = true;
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            BoxCollider col = hitRanges[0].GetComponent<BoxCollider>();

            Vector3 center = col.bounds.center;
            Vector3 half = col.bounds.extents;

            Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[0].transform.rotation);

            foreach (var obj in colliders)
            {
                if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                {
                    ani.SetBool("Attack_2", true);
                    break;
                }
                ani.SetBool("Attack_2", false);
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        if (!first)
                        {
                            nowPos = EaseOutSine(startPos, firPos, timer / firTime);
                            if (timer > firTime)
                            {
                                timer -= firTime;
                                nowPos = EaseInSine(firPos, secPos, timer / secTime);
                                first = true;
                            }
                        }

                        else if (!sceond)
                        {
                            nowPos = EaseInSine(firPos, secPos, timer / secTime);

                            if (timer > secTime)
                            {
                                timer -= secTime;
                                nowPos = EaseInCubic(secPos, tirPos, timer / tirTime);
                                sceond = true;
                            }
                        }

                        else if (timer < tirTime)
                        {
                            nowPos = EaseInCubic(secPos, tirPos, timer / tirTime);
                        }
                    }
                }
            }

            else
            {
                if (!first)
                {
                    nowPos = EaseOutSine(startPos, firPos, timer / firTime);
                    if (timer > firTime)
                    {
                        timer -= firTime;
                        nowPos = EaseInSine(firPos, secPos, timer / secTime);
                        first = true;
                    }
                }

                else if (!sceond)
                {
                    nowPos = EaseInSine(firPos, secPos, timer / secTime);

                    if (timer > secTime)
                    {
                        timer -= secTime;
                        nowPos = EaseInCubic(secPos, tirPos, timer / tirTime);
                        sceond = true;
                    }
                }
                else if (timer < tirTime)
                {
                    nowPos = EaseInCubic(secPos, tirPos, timer / tirTime);
                }
            }

            if (first && sceond && timer > tirTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

                com.Stun(2f);
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

                com.Stun(5f);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            BoxCollider col = hitRanges[3].GetComponent<BoxCollider>();

            Vector3 center = col.bounds.center;
            Vector3 half = col.bounds.extents;

            Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[3].transform.rotation);

            foreach (var obj in colliders)
            {
                if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                {
                    ani.SetBool("Attack_2", true);
                    break;
                }
                ani.SetBool("Attack_2", false);
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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
                obj.GetComponent<PlayerInfo>().OnDamage(Mathf.RoundToInt((float)mInfo.state.atk *1.5f));

                if (com.unitState == UnitState.Stun ||
                    com.unitState == UnitState.Down ||
                    com.unitState == UnitState.Air ||
                    com.unitState == UnitState.Knockback ||
                    com.unitState == UnitState.Die ||
                    com.unitState == UnitState.Skill_F ||
                    com.unitState == UnitState.Skill_S)
                    continue;

                com.Air(Mathf.RoundToInt((float)mInfo.state.atk * 1.5f));
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseOutBounce(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseOutBounce(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            BoxCollider col = hitRanges[6].GetComponent<BoxCollider>();

            Vector3 center = col.bounds.center;
            Vector3 half = col.bounds.extents;

            Collider[] colliders = Physics.OverlapBox(center, half, hitRanges[6].transform.rotation);

            foreach (var obj in colliders)
            {
                if (obj.CompareTag("Player") && !obj.GetComponent<PlayerInfo>().dead)
                {
                    ani.SetBool("Attack_2", true);
                    break;
                }
                ani.SetBool("Attack_2", false);
            }
        

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

            Vector3 nowPos = rb.position;

            var rbT = rb.transform.position;
            rbT.y = 1.5f;
            var ray = new Ray(rbT, rb.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, mask))
            {
                var target = hitInfo.collider.gameObject;

                if (target != null)
                {
                    if (target.CompareTag("Map"))
                    {
                        nowPos = rb.position;
                    }
                    else
                    {
                        nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
                    }
                }
            }

            else
            {
                nowPos = EaselInOutCircle(startPos, endPos, timer / endTime);
            }

            if (timer > endTime)
            {
                rb.MovePosition(nowPos);
                break;
            }

            rb.MovePosition(nowPos);
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

                        com.HitKB(rb.transform.forward);
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

                com.HitKB(rb.transform.forward);
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

