using UnityEngine;

public class IdleJugador : MovJugador
{
    public IdleJugador(JugadorSM c) : base(c)
    {
        sm = c;
    }
    public override void Enter()
    {
        base.Enter();
        sm.animador.Frenar();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (sm != null && sm.direccion != Vector3.zero) { sm.ChangeState(sm.mover); }
    }
}
