using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvadeState : BaseState
{
    private float endTime;
    private float targetFrameTime;
    private float timer;
    private Vector3 startPos;
    private Vector3 endPos;

    public EvadeState(Movement movement) : base(movement)
    {
    }

    public override void Open()
    {
        endTime = 0.8f;
        targetFrameTime = 1.0f / 30.0f;
        
        endTime = Mathf.Ceil(endTime / targetFrameTime) * targetFrameTime;

        timer = 0f;

        startPos = movement.rb.transform.position;
        endPos = movement.rb.transform.position + movement.rb.transform.forward * 5f;

        movement.ani.SetTrigger("evade");
    }

    public override void Close()
    {
    }

    public override void OnUpdate()
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
                    nowPos = EaseInOutQuad(startPos, endPos, timer / endTime);
                }
            }
        }
        else
        {
            nowPos = EaseInOutQuad(startPos, endPos, timer / endTime);
        }

        if (timer > endTime)
        {
            movement.rb.MovePosition(nowPos);
            movement.stateMachine.ChangeState(UnitState.Idle);
        }
    }
    
    public override void OnFixedUpdate()
    {
    }

    public Vector3 EaseInOutQuad(Vector3 start, Vector3 end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value + start;
        value--;
        return -end * 0.5f * (value * (value - 2) - 1) + start;
    }

}
