using UnityEngine;
using UnityEngine.AI;

public class HuidaRango: MovEnemy
{
    protected new RangedSM sm;
    public HuidaRango(RangedSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        sm.Agent.isStopped = false;
        sm.Agent.velocity /= 2;
     }

    public override void Exit()
    {
        base.Exit();
        sm.Agent.velocity *= 2;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float distancia = Vector3.Distance(sm.transform.position, sm.Jugador.position);

        // Si ya no está en zona de peligro follow
        if (distancia > sm.RangoHuida + 1f)
        {
            sm.ChangeState(sm.Follow);
            return;
        }
        if (sm.HandlerPoder.CanCast())
        {
            sm.ChangeState(sm.Poder);
            return;
        }
        // Huir = ir en la dirección contraria al jugador
        Vector3 direccionHuida = (sm.transform.position - sm.Jugador.position).normalized;
        Vector3 destino = sm.transform.position + direccionHuida * 5f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destino, out hit, 5f, NavMesh.AllAreas))
        {
            sm.Agent.SetDestination(hit.position);
        }
    }

}
