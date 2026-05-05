using UnityEngine;

public class HabsJugador : BaseState
{
    protected new JugadorSM sm;

    public HabsJugador(JugadorSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        if (!sm.poder.CanCast()) { this.TerminarAtaque(); }
        Vector3 pos = MouseController.GetMouseWorldPosition();
        sm.poder.Castear(pos);
        sm.animador.Atacar(5, pos);
        sm.animador.FinalAtaque += TerminarAtaque;
    }

    public override void Exit()
    {
        sm.animador.FinalAtaque -= TerminarAtaque;
    }

    private void TerminarAtaque() { sm.ChangeState(sm.idle); }
}
