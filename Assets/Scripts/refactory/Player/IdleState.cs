using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IdleState : BaseState
{
    private float rotateSpeed;
    private float currentValue;
    private Vector3 moveVec;
    private Camera worldCam;

    public IdleState(Movement movement) : base(movement)
    {
    }

    public override void Open()
    {
        rotateSpeed = 1000f;
        currentValue = 0f;
        moveVec = Vector3.zero;
        worldCam = Camera.main;
    }

    public override void Close()
    {
        rotateSpeed = 0f;
        currentValue = 0f;
        moveVec = Vector3.zero;
        worldCam = null;
    }

    public override void OnUpdate()
    {
        MoveVecSet();

        if (movement.controller.evade)
        {
            movement.stateMachine.ChangeState(UnitState.Evade);
        }

        if(movement.controller.attack)
        {
            movement.stateMachine.ChangeState(UnitState.Attack);
        }

        if(movement.controller.firstSkill && !movement.fSkillDelay)
        {
            movement.stateMachine.ChangeState(UnitState.Skill_F);
        }

        if (movement.controller.secondSkill && !movement.sSkillDelay)
        {
            movement.stateMachine.ChangeState(UnitState.Skill_S);
        }
    }

    public override void OnFixedUpdate()
    {
        Move();
        Rotate();

        movement.ani.SetFloat("Move", currentValue);
    }

    private void MoveVecSet()
    {
        var forward = worldCam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        var right = worldCam.transform.right;
        right.y = 0f;
        right.Normalize();

        moveVec = forward * movement.controller.moveFB;
        moveVec += right * movement.controller.moveLR;

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
        var position = movement.rb.position;
        position += moveVec * movement.info.state.movSp * Time.deltaTime;
        movement.rb.MovePosition(position);
    }
    private void Rotate()
    {
        if (moveVec == Vector3.zero)
            return;

        var rotation = movement.rb.rotation;
        var targetRotateion = Quaternion.LookRotation(moveVec, Vector3.up);
        rotation = Quaternion.RotateTowards(rotation, targetRotateion, rotateSpeed * Time.deltaTime);
        movement.rb.MoveRotation(rotation);
    }
}
