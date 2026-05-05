using UnityEngine;

public class AttackMelee: BaseState
{
    protected new MeleeSM sm;

    public AttackMelee(MeleeSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        Vector3 pos = sm.Jugador.position;
        sm.HandlerAtaque.Atacar(pos);
        sm.Animador.FinalAtaque += TerminarAtaque;
    }

    public override void Exit()
    {
        sm.Animador.FinalAtaque -= TerminarAtaque;
    }

    private void TerminarAtaque() { sm.ChangeState(sm.Wander); }
}
