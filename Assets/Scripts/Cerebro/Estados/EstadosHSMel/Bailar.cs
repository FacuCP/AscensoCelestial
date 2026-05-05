using UnityEngine;

public class Bailar : BaseState
{
    protected new MelSM sm;
    private Vector3 direccion;
    private Vector3 posicionSpot;


    private Vector3 inicio;
    private float progreso = 0f;

    private int saltosTotales = 4;
    private float alturaSalto = 1.5f;
    private float duracion = 1.5f;

    private int saltosDetectados = 0;
    private bool estabaEnElSuelo = true;

    public Bailar(MelSM stateMachine) : base(stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        inicio = sm.Cuerpo.position;
        posicionSpot = sm.SpotlightManager.SpotlightActivo.transform.position;

        progreso = 0f;
        saltosDetectados = 0;
        estabaEnElSuelo = true;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        progreso += Time.fixedDeltaTime / duracion;
        progreso = Mathf.Clamp01(progreso);

        // Movimiento horizontal
        Vector3 horizontal = Vector3.Lerp(inicio, posicionSpot, progreso);

        // Movimiento vertical (saltos)
        float fase = progreso * saltosTotales * Mathf.PI; // media onda por salto
        float y = Mathf.Sin(fase) * alturaSalto;
        y = Mathf.Max(0f, y); // nunca bajo del piso

        Vector3 nuevaPos = new Vector3(horizontal.x, y, horizontal.z);
        sm.Cuerpo.MovePosition(nuevaPos);

        // Detectar toque de piso
        bool enSuelo = y <= 0.01f;
        if (enSuelo && !estabaEnElSuelo)
        {
            Tocar();
        }
        estabaEnElSuelo = enSuelo;

        // Fin del movimiento
        if (progreso >= 1f)
        {
            sm.Cuerpo.MovePosition(new Vector3(posicionSpot.x, 0f, posicionSpot.z));
            sm.ChangeState(sm.atacar);
        }
    }

    private void Tocar()
    {
        sm.SpotlightManager.AparecerFuego(sm.transform.position);
    }
}
