using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected Movement movement { get; private set; }

    public BaseState(Movement movement)
    {
        this.movement = movement;
    }

    public abstract void Open();
    public abstract void Close();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
}
