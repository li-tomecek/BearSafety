using UnityEngine;

public abstract class BaseState : IState
{
    protected Animator animator;


    public BaseState(Animator animator) { this.animator = animator; }


    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Update()
    {
    }
}