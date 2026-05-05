using UnityEngine;

public class FollowRango: FollowEnemy
{
    protected new RangedSM sm;

    public FollowRango(RangedSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        sm.Agent.isStopped = false;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float distancia = Vector3.Distance(sm.transform.position, sm.Jugador.position);

        // Si está muy cerca huye
        if (distancia < sm.RangoHuida)
        {
            sm.ChangeState(sm.Huida);
            return;
        }

        // Si está en rango de ataque poder
        if (distancia < sm.RangoPoder && sm.HandlerPoder.CanCast() && !sm.Estados.Paralizado && !sm.Estados.Ciego)
        {
            sm.ChangeState(sm.Poder);
            return;
        }
    }

}

