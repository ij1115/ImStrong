using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSkillState : BaseState
{
    enum Phase
    {
        First,
        Second,
        Third,
    }

    private Phase _phase;
    private float aniLength;
    private float frameTime;

    private float time;

    private float targetFrameTime;

    private float timer;

    private Vector3 startPos;
    private Vector3 endPos;

    public FSkillState(Movement movement) : base(movement)
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

        timer = 0f;
        _phase = Phase.First;
        movement.FSkillDelayOn();
        movement.ani.speed = movement.info.state.atkSp;
        movement.ani.SetBool("Fight", true);
        movement.ani.SetTrigger("Skill_F");
    }

    public override void Close()
    {
        movement.ani.speed = 1f;
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
    }

    private void SkillMove()
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
                            nowPos = EaseInSine(startPos, endPos, timer / time);
                            break;

                        case Weapons.Axe:
                        case Weapons.Spear:
                            nowPos = EaselInOutCircle(startPos, endPos, timer / time);
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
                    nowPos = EaseInSine(startPos, endPos, timer / time);
                    break;

                case Weapons.Axe:
                case Weapons.Spear:
                    nowPos = EaselInOutCircle(startPos, endPos, timer / time);
                    break;
            }
        }

        if (timer > time)
        {
            movement.rb.MovePosition(nowPos);
            movement.stateMachine.ChangeState(UnitState.Idle);
        }
    }

    private void SwordSetting()
    {
        aniLength = 3.667f;
        frameTime = aniLength / 110f;

        switch (_phase)
        {
            case Phase.First:
                time = (25f * frameTime) / movement.info.state.atkSp;

                startPos = movement.rb.transform.position;
                endPos = movement.rb.transform.position + movement.rb.transform.forward;
                break;

            case Phase.Second:
                time = (10f * frameTime) / movement.info.state.atkSp;

                startPos = movement.rb.transform.position;
                endPos = movement.rb.transform.position + movement.rb.transform.forward;
                break;

             case Phase.Third:
                time = (15f * frameTime) / movement.info.state.atkSp;

                startPos = movement.rb.transform.position;
                endPos = movement.rb.transform.position + movement.rb.transform.forward *2f;
                break;
        }
        targetFrameTime = 1.0f/ 30.0f;
     
        timer -= time;

        time = Mathf.Ceil(time / targetFrameTime) * targetFrameTime;
    }

    private void AxeSetting()
    {
        aniLength = 2.900f;
        frameTime = aniLength / 87f;

        switch (_phase)
        {
            case Phase.First:
                time = (60f * frameTime) / movement.info.state.atkSp;

                startPos = movement.rb.transform.position;
                endPos = movement.rb.transform.position + movement.rb.transform.forward * 4.5f;
                break;
        }
        targetFrameTime = 1.0f / 30.0f;

        time = Mathf.Ceil(time / targetFrameTime) * targetFrameTime;
    }

    private void SpearSetting()
    {
        aniLength = 1.733f;
        frameTime = aniLength / 52f;

        switch (_phase)
        {
            case Phase.First:
                time = (46f * frameTime) / movement.info.state.atkSp;

                startPos = movement.rb.transform.position;
                endPos = movement.rb.transform.position + movement.rb.transform.forward * 3f;
                break;
        }
        targetFrameTime = 1.0f / 30.0f;

        time = Mathf.Ceil(time / targetFrameTime) * targetFrameTime;
    }

    public Vector3 EaseInCubic(Vector3 start, Vector3 end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }


    public Vector3 EaseInSine(Vector3 start, Vector3 end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
    }

    public Vector3 EaselInOutCircle(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

}
