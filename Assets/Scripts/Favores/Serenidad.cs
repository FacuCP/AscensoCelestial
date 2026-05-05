using UnityEngine;

public class Serenidad : FavorBase
{
    private HandlerEstados estados;
    private HandlerVida vida;
    [SerializeField] private float cantidadCura = 10f;
    [SerializeField] private float tiempo = 4f;
    [SerializeField] private float mejoraCuraNivel = 2.5f;
    [SerializeField] private float mejoraTiempoNivel = 0.25f;
    [SerializeField] private float fuerzaBendicion = 15;
    [SerializeField] private float mejoraBendicion = 5;

    public override void Setup(StateMachine padre)
    {
        base.Setup(padre);
        vida = padre.GetComponentInChildren<HandlerVida>();
        estados = padre.GetComponentInChildren<HandlerEstados>();
    }

    public override void Lanzar(Vector3 punto)
    {
        vida.RecibirCuraProlongado(cantidadCura + mejoraCuraNivel, tiempo + mejoraTiempoNivel);
        estados.AplicarBendito(tiempo + mejoraTiempoNivel, fuerzaBendicion + mejoraBendicion);
    }

    public override string GetDescripcion() {
        return "Cura al jugador y lo bendice durante un breve período de tiempo.";
    }

    public override string GetNombre() { return "Serenidad"; }
}
