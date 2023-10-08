using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
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

    public void SetUp()
    {
        mInfo = GetComponent<MonsterInfo>();
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

        //Vector3 dir = new Vector3(controller.moveLR, 0, controller.moveFB);

        //moveVec = cameraRot * dir;
        //Debug.Log($"전 : {moveVec}");
        //if (moveVec.magnitude > 1f)
        //{
        //    moveVec.Normalize();
        //}
        //Debug.Log($"후{moveVec}");
        if (unitState == UnitState.NIdle || unitState == UnitState.Idle)
        {
            Move();
            Rotate();

            ani.SetFloat("Move", moveVec.magnitude);
        }
    }

    private void Update()
    {
        /*
        //if (unitState == UnitState.NIdle ||unitState == UnitState.Idle)
        //{
        //    var forward = worldCam.transform.forward;
        //    forward.y = 0f;
        //    forward.Normalize();
        //    var right = worldCam.transform.right;
        //    right.y = 0f;
        //    right.Normalize();
        //    moveVec = forward * controller.moveFB;
        //    moveVec += right * controller.moveLR;
        //    if (moveVec.magnitude > 1f)
        //    {
        //        moveVec.Normalize();
        //    }
        */

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
        //if (moveVec.sqrMagnitude == 0)
        //    return;

        //var dirQuat = Quaternion.LookRotation(moveVec * moveSpeed * Time.deltaTime);
        //Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, rotateSpeed);
        //rb.MoveRotation(moveQuat);


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

    // 이벤트
    public void ReturnIdle()
    {
        switch (unitState)
        {
            case UnitState.Attack:
                Input.ResetInputAxes();
                unitState = UnitState.Idle;
                ani.SetBool("Attack_2", false);
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
}
