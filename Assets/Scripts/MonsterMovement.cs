using System.Collections;
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
        while(!mInfo.dead)
        {
            if(attack)
            {
                if (unitState == UnitState.NIdle || unitState == UnitState.Idle)
                {
                    pathFinder.isStopped = false;
                    pathFinder.SetDestination(target.transform.position);

                    if(!attackBool)
                        AttackWeapons();
                }
                else
                {
                    pathFinder.isStopped = true;
                }
            }
            else
            {
                if(finder)
                {
                    pathFinder.isStopped = true;
                    SphereCollider col = FindRange.GetComponent<SphereCollider>();
                    Collider[] colliders = Physics.OverlapSphere(col.gameObject.transform.position, 12f, whitIsTarget);
                    
                    for(int i=0; i<colliders.Length; ++i)
                    {
                        var p = colliders[i].GetComponent<PlayerInfo>();
                        if(p != null && !p.dead)
                        {
                            target = p.gameObject;
                            attack = true;
                            break;
                        }
                    }
                }
                else if(!finder)
                {
                    pathFinder.isStopped = false;
                    pathFinder.SetDestination(spawnPos);

                    if(!pathFinder.pathPending)
                    {
                        if(pathFinder.remainingDistance <= pathFinder.stoppingDistance)
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
        finder = true;
        pathFinder.isStopped = true;
        unitState = UnitState.Knockback;
        ani.SetBool("Fight", true);
        ani.SetTrigger("Knockback");
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
                ani.SetBool("Attack_2", false);
                break;

            case UnitState.Skill_F:
                unitState = UnitState.Idle;
                break;

            case UnitState.Skill_S:
                unitState = UnitState.Idle;
                break;

            case UnitState.Impact:
                pathFinder.isStopped = true;
                unitState = UnitState.Idle;
                break;

            case UnitState.Knockback:
                pathFinder.isStopped = true;
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
