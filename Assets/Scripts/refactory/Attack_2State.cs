using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_2State : BaseState
{
    private bool inputAttack;
    private float delayTimer;

    private float aniLength;
    private float endTime;
    private float timer;

    private Vector3 startPos;
    private Vector3 endPos;

    public Attack_2State(Movement movement) : base(movement)
    {
    }

    public override void Open()
    {
        switch (movement.weapons.type)
        {
            case Weapons.Sword:
                SwordSetting();
                break;

            case Weapons.Axe:
                AxeSetting();
                break;

            case Weapons.Spear:
                SpearSetting();
                break;
        }
    }

    public override void Close()
    {
        startPos = Vector3.zero;
        endPos = Vector3.zero;
        endTime = 0f;
        timer = 0f;
        aniLength = 0f;
        movement.ani.SetBool("Attack_2", false);
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {
        if (movement.controller.evade)
        {
            movement.stateMachine.ChangeState(UnitState.Evade);
        }

        AttackMove();
    }

    private void AttackMove()
    {
        timer += Time.deltaTime;

        Vector3 nowPos = movement.rb.position;

        var rbT = movement.rb.transform.position;
        rbT.y = 1.5f;
        var ray = new Ray(rbT, movement.rb.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, movement.mask))
        {
            var target = hitInfo.collider.gameObject;

            if (target != null)
            {
                if (target.CompareTag("Map"))
                {
                    nowPos = movement.rb.position;
                }
                else
                {
                    switch (movement.weapons.type)
                    {
                        case Weapons.Sword:
                            nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                            break;

                        case Weapons.Axe:
                        case Weapons.Spear:
                            nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                            break;
                    }
                }
            }
        }

        else
        {
            switch (movement.weapons.type)
            {
                case Weapons.Sword:
                    nowPos = EaseInOutExpo(startPos, endPos, timer / endTime);
                    break;

                case Weapons.Axe:
                case Weapons.Spear:
                    nowPos = EaseInOutQuart(startPos, endPos, timer / endTime);
                    break;
            }
        }

        if (timer > endTime)
        {
            movement.rb.MovePosition(nowPos);
        }
    }

    public Vector3 EaseInOutQuart(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value + start;
        value -= 2;
        return -end * 0.5f * (value * value * value * value - 2) + start;
    }

    public Vector3 EaseInOutExpo(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    private void SwordSetting()
    {
        aniLength = 1.367f;
        endTime = (6f / 30f) * aniLength / movement.info.state.atkSp;
        timer = 0f;

        startPos = movement.rb.transform.position;
        endPos = movement.rb.transform.position + movement.rb.transform.forward * 0.7f;
    }

    private void AxeSetting()
    {
        aniLength = 2.500f;
        endTime = (18f / 30f) * aniLength / movement.info.state.atkSp;
        timer = 0f;

        startPos = movement.rb.transform.position;
        endPos = movement.rb.transform.position + movement.rb.transform.forward * 1f;
    }

    private void SpearSetting()
    {
        aniLength = 1.167f;
        endTime = (14f / 30f) * aniLength / movement.info.state.atkSp;
        timer = 0f;

        startPos = movement.rb.transform.position;
        endPos = movement.rb.transform.position + movement.rb.transform.forward * 1f;
    }
}
