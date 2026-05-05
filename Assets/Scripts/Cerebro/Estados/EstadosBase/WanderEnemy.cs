using UnityEngine;

public class WanderEnemy : MovEnemy
{
    private float tiempoProximoMovimiento;
    public WanderEnemy(BaseEnemySM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        tiempoProximoMovimiento = 0f;
        sm.Agent.isStopped = false;
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        float distancia = Vector3.Distance(sm.transform.position, sm.Jugador.position);


        if (sm.direccion.magnitude < 0.1f)
        {
            sm.Animador.Frenar();
        }
        if (distancia < sm.RangoDetectarJugador)
        {
            sm.ChangeState(sm.Follow);
            return;
        }
        if (Time.time >= tiempoProximoMovimiento)
        {
            sm.MoverRandom(8);
            tiempoProximoMovimiento = Time.time + Random.Range(1f, 3f);
        }
    }
}
