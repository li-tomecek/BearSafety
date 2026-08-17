using UnityEngine;
using UnityEngine.AI;

public class BearAttackState : BaseState
{
    private NavMeshAgent _bearAgent;

    private bool _isActive = true;


    public BearAttackState(Animator animator, NavMeshAgent bearAgent) : base(animator)
    {
        _bearAgent = bearAgent;
    }


    public override void Enter()
    {
        _bearAgent.isStopped = true;

        _bearAgent.velocity = Vector3.zero;
        _bearAgent.ResetPath();

        animator.CrossFade("Attack", 0.2f);

        VRScreenFade.Instance.FadeToCompleted += ResetLevel;
        _isActive = true;
    }

    public override void Update()
    {
        if (!_isActive) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack"))
        {
            if (stateInfo.normalizedTime >= 0.225f)
            {
                VRScreenFade.Instance.FadeToBlack(1.0f);
                _isActive = false;
            }
        }
    }

    public override void Exit()
    {
        VRScreenFade.Instance.FadeToCompleted -= ResetLevel;
    }

    private void ResetLevel()
    {
        SceneService.Instance.ReloadCurrentScene();
    }
}
