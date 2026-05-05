using Unity.Cinemachine;
using UnityEngine;

public class WalkJugador : MovJugador
{
    public WalkJugador(JugadorSM c) : base(c)
    {
        sm = c;
    }

    public override void OnMovimiento(Vector3 dir)
    {
        if (dir != Vector3.zero) { }
        //  pj.ChangeState(pj.mover);
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        if (sm.estados.Paralizado || Pausar.Detenido)
        {
            return;
        }
        if (sm.direccion.sqrMagnitude < 0.01f)
        {
            sm.ChangeState(sm.idle);
            return;
        }
        Vector3 desplazamiento = sm.direccion * sm.velocidad.Velocidad * Time.fixedDeltaTime;
        Vector3 nextPos = sm.cuerpo.position + desplazamiento;

        // evita traspasar la pared
        if (sm.cuerpo.SweepTest(
            sm.direccion.normalized,
            out RaycastHit hit,
            desplazamiento.magnitude,
            QueryTriggerInteraction.Ignore))
        {
            nextPos = sm.cuerpo.position + sm.direccion.normalized * (hit.distance - 0.01f);
        }

        sm.cuerpo.MovePosition(nextPos);
        sm.animador.SetMovimiento(sm.direccion);
    }

}
