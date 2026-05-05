using UnityEngine;

public class FollowEnemy : MovEnemy
{
    public FollowEnemy(BaseEnemySM stateMachine) : base(stateMachine)
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

        // Si el jugador se alejó mucho  volver a patrullar
        if (distancia > sm.RangoPerderJugador)
        {
            sm.ChangeState(sm.Wander);
            return;
        }
        // AQUÍ ESTÁ LO IMPORTANTE  seguir actualizando destino
        sm.Agent.SetDestination(sm.Jugador.position);

        // actualizar dirección para animación ESTAN EN MOV ENEMY
       // sm.direccion = sm.Agent.velocity.normalized;
       // sm.Animador.SetMovimiento(sm.direccion);
    }
}
