using System.Collections;
using UnityEditor;
using UnityEngine;

public class Atacar : BaseState
{
    protected new MelSM sm;

    private int cantidadAtaques = 3;
    private int cantidadAtaquesMax = 10;
    private int lanzamiento = 0;

    private float tiempoEntreAtaques = 0.5f;
    private float timer = 0f;
    private bool esperandoCerrar = false;
    private float tiempoEsperaCerrar = 1f;

    private Vector3[] direcciones = new Vector3[]
    {
        Vector3.right,
        Vector3.left
    };

    private int indiceDireccion = 0;


    public Atacar(MelSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0f;
        lanzamiento = 0;
        esperandoCerrar = false;
        indiceDireccion = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        timer += Time.deltaTime;

        if (esperandoCerrar)
        {
            if (timer >= tiempoEsperaCerrar)
            {
                CambiarEstado();
            }
            return;
        }

        if (timer >= tiempoEntreAtaques && JugadorSM.Instancia.EstaVivo)
        {
            lanzamiento++;
            timer = 0f;
            sm.direccion = direcciones[indiceDireccion];
            indiceDireccion = (indiceDireccion + 1) % direcciones.Length;
            sm.Animador.Atacar(1/tiempoEntreAtaques, sm.direccion);
            sm.HandlerPoder.Castear(JugadorSM.Instancia.transform.position);
        }

        if(lanzamiento == cantidadAtaques)
        {
            CerrarAtaque();
        }
    }

    private void CerrarAtaque() {
        esperandoCerrar = true;
    }

    private void CambiarEstado()
    {
        sm.SpotlightManager.TerminarSpot();
        if (sm.SpotlightManager.SpotlightActivo != null)
        {

            sm.ChangeState(sm.bailar);
        }
        else
        {
            cantidadAtaques = Mathf.Clamp(++cantidadAtaques, 1, cantidadAtaquesMax);
            sm.ChangeState(sm.cerrar);
        }
    }

}
