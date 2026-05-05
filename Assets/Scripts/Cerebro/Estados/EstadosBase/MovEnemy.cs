using UnityEngine;

public class MovEnemy : BaseState
{
    protected new BaseEnemySM sm;

    public MovEnemy(BaseEnemySM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void UpdatePhysics()
    {
        if (sm.Estados.Paralizado)
        {
            // Detener movimiento completamente
            sm.Agent.isStopped = true;
            sm.Agent.velocity = Vector3.zero;

            // Detener animación
            sm.Animador.Frenar();

            return;
        }

        // Si no está paralizado, continuar movimiento normal
        sm.Agent.isStopped = false;
        sm.direccion = sm.Agent.velocity.normalized;
        sm.Animador.SetMovimiento(sm.direccion);
    }
}
