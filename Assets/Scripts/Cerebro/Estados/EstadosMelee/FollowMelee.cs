using UnityEngine;

public class FollowMelee: FollowEnemy
{

    protected new MeleeSM sm;

    public FollowMelee(MeleeSM stateMachine) : base(stateMachine)
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

        // Si está en rango de ataque  atacar
        if (distancia < sm.RangoAtaqueJugador && !sm.Estados.Paralizado && !sm.Estados.Ciego)
        {
            sm.ChangeState(sm.Attack);
            return;
        }
    }

}

