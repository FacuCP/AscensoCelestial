using UnityEngine;

public class AtaqueJugador : BaseState
{
    protected new JugadorSM sm;
    private float nerfVelocidad = 2f;

    public AtaqueJugador(JugadorSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        if (sm.estados.Paralizado)
        {
            return;
        }
        if (sm.direccion.sqrMagnitude < 0.01f)
        {
            sm.ChangeState(sm.idle);
            return;
        }
        Vector3 desplazamiento = sm.direccion * sm.velocidad.Velocidad/nerfVelocidad * Time.fixedDeltaTime;
        Vector3 nextPos = sm.cuerpo.position + desplazamiento;

        // evita traspasar la pared
        if (sm.cuerpo.SweepTest(sm.direccion, out RaycastHit hit, desplazamiento.magnitude))
        {
            nextPos = hit.point - sm.direccion * 0.01f; // queda justo antes del impacto
        }

        sm.cuerpo.MovePosition(nextPos);
        sm.cuerpo.MovePosition(sm.cuerpo.position + desplazamiento);
    }
    public override void Enter()
    {
        Vector3 pos = MouseController.GetMouseWorldPosition();
        sm.ataque.Atacar(pos);
        sm.animador.FinalAtaque += TerminarAtaque;
    }

    public override void Exit()
    {
        sm.animador.FinalAtaque -= TerminarAtaque;
    }

    private void TerminarAtaque() { sm.ChangeState(sm.idle); }
}