using UnityEngine;

public class PoderRango: BaseState
{
    protected new RangedSM sm;

    public PoderRango(RangedSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        if (!sm.HandlerPoder.CanCast()) { this.TerminarAtaque(); }
        Vector3 pos = sm.Jugador.position;
        sm.HandlerPoder.Castear(pos);
        sm.Animador.Atacar(5, pos);
        sm.Animador.FinalAtaque += TerminarAtaque;
    }

    public override void Exit()
    {
        sm.Animador.FinalAtaque -= TerminarAtaque;
    }

    private void TerminarAtaque() { sm.ChangeState(sm.Wander); }
}
