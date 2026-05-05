using UnityEngine;

public class Sangrado : ForjaBase
{ 
    [SerializeField] float porcentajeBase = 5; 
    [SerializeField] float porcentajeNivel = 1;
    [SerializeField] float porcentajeFuerza = 0.01f;
    [SerializeField] float tiempoBase = 2;
    [SerializeField] float tiempoNivel = 0.25f;
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        HandlerVida vida = objetivo.GetComponentInChildren<HandlerVida>();
        if (vida != null)
        {
            vida.RecibirDmgPorcentualProlongado(GetValor(), handler, tiempoBase + Nivel * tiempoNivel);
        }
    }
    public override string GetDescripcion()
    {
        return "Provoca que el enemigo entre en estado de hemorragia, recibiendo daño continuo a lo largo del tiempo.";
    }
    public override string GetNombre() { return "Sangrado"; }
    private float GetValor() { 
        float dmg = porcentajeBase + porcentajeNivel * Nivel + porcentajeFuerza * handler.Fuerza;
        return Handler.ModDmg * dmg;
    }
}
