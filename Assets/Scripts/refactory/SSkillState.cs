using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSkillState : BaseState
{
    public SSkillState(Movement movement) : base(movement)
    {
    }

    public override void Open()
    {

    }
    public override void Close()
    {
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


}
