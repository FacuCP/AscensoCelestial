using UnityEngine;

public class IniciarActo : BaseState
{
    protected new MelSM sm;

    public int cantLuces { get; private set; } = 2;
    public int cantLucesMax { get; private set; } = 8;


    private bool lanzado = false;
    public IniciarActo(MelSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        if (sm.segundaFaseDisponible && !sm.segundaFaseActivada)
        {
            sm.ActivarFase();
        }
        lanzado = false;
    }

    public void Iniciar()
    {
        lanzado = true;
        sm.SpotlightManager.EmpezarBallet(cantLuces);
        sm.ChangeState(sm.bailar);
    }

    public override void Exit() {
        base.Exit();
        cantLuces = Mathf.Clamp(++cantLuces, 0, cantLucesMax);
    }


    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (sm.combateActivo && !lanzado)
        {
            Iniciar();
        }
    }
}
