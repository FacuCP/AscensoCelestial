using UnityEngine;

public class Voluntad : FavorBase
{
    private HandlerEstados estados;
    [SerializeField] private float empoderamiento = 10f;
    [SerializeField] private float tiempo = 4f;
    [SerializeField] private float mejoraEmpoderamientoNivel = 5f;
    [SerializeField] private float mejoraTiempoNivel = 0.25f;

    public override void Setup(StateMachine padre)
    {
        base.Setup(padre);
        estados = padre.GetComponentInChildren<HandlerEstados>();
    }

    public override void Lanzar(Vector3 punto)
    {
        estados.AplicarEmpoderado(tiempo + mejoraTiempoNivel, empoderamiento + mejoraEmpoderamientoNivel);
        estados.AplicarImparable(tiempo + mejoraTiempoNivel);
        estados.AplicarInvencible((tiempo + mejoraTiempoNivel) / 2);
    }

    public override string GetDescripcion()
    {
        return "Vuelve al jugador invencible, imparable y empoderado durante un breve período de tiempo.";
    }
    public override string GetNombre() { return "Voluntad"; }
}
