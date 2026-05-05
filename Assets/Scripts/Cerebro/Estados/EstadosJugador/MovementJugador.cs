using Unity.Cinemachine;
using UnityEngine;

public class MovJugador : BaseState
{
    protected new JugadorSM sm;

    public MovJugador(JugadorSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        if (!sm.cuerpo.linearVelocity.AlmostZero()) sm.cuerpo.linearVelocity = Vector3.zero;
    }

    public override void OnAtaque()
    {
        sm.ChangeState(sm.atacar);
    }

    public override void OnPoder()
    {
        if(sm.poder.CanCast())sm.ChangeState(sm.habilidades);
    }

    public override void OnFavor()
    { 
        Vector3 pos = MouseController.GetMouseWorldPosition();
        sm.favor.Castear(pos);
    }


}
