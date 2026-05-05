using Unity.VisualScripting;
using UnityEngine;

public class CerrarActo : BaseState
{
    protected new MelSM sm;
    public CerrarActo(MelSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }


    private Vector3 direccion;
    private float espera = 2f;
    private bool esperando = false;
    private float tiempo = 0f;

    public override void Enter()
    {
        base.Enter();
        direccion = (sm.posicionInicial - sm.transform.position).normalized;
        esperando = false;
        tiempo = 0f;
        sm.HandlerPoder.Cambio(1);
    }

    public override void Exit()
    {
        base.Exit();
        sm.HandlerPoder.Cambio(-1);
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        if (esperando) {
            tiempo += Time.deltaTime;
            if (tiempo > espera)
                sm.ChangeState(sm.iniciar);
            return;
        }
        Vector3 desplazamiento = direccion * sm.Velocidad * Time.fixedDeltaTime;
        Vector3 nextPos = sm.Cuerpo.position + desplazamiento;

        sm.Cuerpo.MovePosition(nextPos);

        if ((sm.posicionInicial - sm.transform.position).magnitude < 0.1f)
        {
            Terminar();
        }
    }

    private void Terminar()
    {
        direccion = Vector3.zero;
        esperando = true;
        sm.HandlerPoder.Castear(Vector3.zero);
    }
}
